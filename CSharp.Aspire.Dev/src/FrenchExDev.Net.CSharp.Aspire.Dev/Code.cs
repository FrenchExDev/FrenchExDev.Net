using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace FrenchExDev.Net.CSharp.Aspire.Dev;


public record DnsConfiguration
{
    public string SslPassword { get; set; }
    public string SslGenerator { get; set; }
    public string CertificatesDirectory { get; set; }
    public string CertPath { get; set; }
    public string KeyPath { get; set; }
    public string Domain { get; set; }
    public string[] Domains { get; set; }
    public Dictionary<string, int> Ports { get; set; }

    public Dictionary<string, string> AspNetCoreUrls { get; set; }

    public string GetAspNetCoreUrl(string domain) => AspNetCoreUrls != null && AspNetCoreUrls.ContainsKey(domain) ? AspNetCoreUrls[domain].Replace("#port#", Ports[domain].ToString()).Replace("#subdomain#", domain).Replace("#domain#", Domain) : throw new InvalidDataException();

    public string GetCompleteUrl(string domain) => "https://" + domain + "." + Domain + (Ports != null && Ports.ContainsKey(domain) ? ":" + Ports[domain] : string.Empty);

    public string GetUrl(string domain, int? port = null) => "https://" + domain + "." + Domain + (port is not null ? ":" + port : string.Empty);

    public string GetDashboardUrl(int? port = null) => GetCompleteUrl("devdash");

    public IEnumerable<string> GetAllHosts()
    {
        foreach (var entry in Domains)
        {
            yield return $"{entry}.{Domain}";
        }
    }
    public IEnumerable<string> GetHostsFileEntries(string? ipv4 = "127.0.0.1")
    {
        foreach (var host in GetAllHosts())
        {
            yield return $"{ipv4} {host}";
        }
    }

    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    public ReadOnlySpan<char> ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this).AsSpan();
    }

    public static DnsConfiguration? FromJson(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<DnsConfiguration>(json);
    }

    public DnsConfiguration Neutral()
    {
        return this with { CertPath = null, KeyPath = null };
    }

    public string? CalculateConfigurationHash()
    {
        var c1 = this with { CertPath = null, KeyPath = null };
        var json = System.Text.Json.JsonSerializer.Serialize(c1);
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var hash = sha.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    internal object DomainOrDie() => Domain ?? throw new InvalidOperationException("Domain is not set in the configuration.");
}

public interface IDevAppHost
{
    IDevAppHost EnsureCertSetup(DnsConfiguration config, bool? force = false);
    IDevAppHost EnsureHostsSetup(DnsConfiguration dnsConfig);
    IDevAppHost EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false);
    IDevAppHost EnsureSystemSetup(IConfiguration configuration, DnsConfiguration dnsConfiguration);
    IDevAppHost SetupProject(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name);
}

public class DevAppHost : IDevAppHost
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private IDistributedApplicationBuilder? _builder;
    private Func<IDistributedApplicationBuilder> _builderProvider;

    public static DevAppHost Default(Func<IDistributedApplicationBuilder> builder, string? loggerName = "apphost", string? jsonFile = "devapphost.#Env#.json", string? environment = "Development")
    {
        var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger(loggerName ?? throw new InvalidDataException("missing loggerName"));

        var rootConfig = new ConfigurationBuilder()
            .AddJsonFile(jsonFile?.Replace("#Env#", environment) ?? throw new InvalidDataException("missing jsonFile"))
            .Build();

        var devAppHost = new DevAppHost(builder, logger, rootConfig);

        return devAppHost;
    }

    public IConfiguration Configuration => _configuration;
    public DnsConfiguration DnsConfiguration => _dnsConfiguration ?? throw new InvalidOperationException("_dnsConfiguration is null. Please EnsureSetup() before.");

    private DnsConfiguration? _dnsConfiguration;
    private DevAppHost(Func<IDistributedApplicationBuilder> builder, ILogger logger, IConfiguration configuration)
    {
        _builderProvider = builder;
        _builder = BuilderOrDie();
        _logger = logger;
        _configuration = configuration;
    }

    public void EnsureSetup()
    {
        _dnsConfiguration = Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? throw new InvalidOperationException("missing DnsConfiguration");

        EnsureCertSetup(_dnsConfiguration);
        EnsureHostsSetup(_dnsConfiguration);
        EnsureSystemSetup(Configuration, _dnsConfiguration);
        EnsureHostsSetup(_dnsConfiguration);
    }

    public IDevAppHost EnsureSystemSetup(IConfiguration rootConfig, DnsConfiguration dnsConfig)
    {
        LaunchSettingsHelper.AddOrUpdateEnvironmentVariables(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../..", "Properties", "launchSettings.json"),
            rootConfig["LaunchSettingsProfile"] ?? "IIS Express",
            new Dictionary<string, string>
            {
                { "ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie() },
                { "ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie() },
                { "ASPNETCORE_URLS", dnsConfig.GetAspNetCoreUrl("devdash") },
                { "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", dnsConfig.GetUrl("devdash", dnsConfig.Ports["devdash-oltp"]) },
                { "ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", dnsConfig.GetUrl("devdash", dnsConfig.Ports["devdash-services"]) },
                { "ASPIRE_ENVIRONMENT", rootConfig["Environment"] ?? throw new InvalidOperationException() },
                { "DOTNET_ENVIRONMENT", rootConfig["Environment"]?? throw new InvalidOperationException() },
                { "CustomDomain__Fqdn", dnsConfig.GetUrl("devdash") },
                { "CustomDomain__Port", dnsConfig.Ports["devdash"].ToString() }
            });

        return this;
    }


    public bool IsRunningAsAdministrator()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    public bool NeedsHostsFileUpdate(DnsConfiguration config)
    {
        try
        {
            var hostsFilePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   ? @"C:\Windows\System32\drivers\etc\hosts"
                 : "/etc/hosts";

            if (!File.Exists(hostsFilePath))
                return true;

            var hostsContent = File.ReadAllText(hostsFilePath);

            foreach (var entry in config.GetHostsFileEntries())
            {
                if (!hostsContent.Contains(entry))
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return true; // Assume it needs update if we can't read
        }
    }

    public bool RestartAsAdministrator()
    {
        try
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule?.FileName == null)
                return false;

            var startInfo = new ProcessStartInfo
            {
                FileName = processModule.FileName,
                Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)),
                UseShellExecute = true,
                Verb = "runas", // Request elevation
                WorkingDirectory = Environment.CurrentDirectory
            };

            Process.Start(startInfo);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart with elevation: {Message}", ex.Message);
            return false;
        }
    }

    public string GetConfigurationFilePath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
        return Path.Combine(certsDir, "dns-config.json");
    }

    public void SaveConfiguration(DnsConfiguration config)
    {
        try
        {
            var configPath = GetConfigurationFilePath(config);
            var directory = Path.GetDirectoryName(configPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = config.ToJson();
            File.WriteAllText(configPath, json);
            _logger.LogInformation("Configuration saved to {ConfigPath}", configPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save configuration");
        }
    }

    public DnsConfiguration? LoadSavedConfiguration(DnsConfiguration config)
    {
        try
        {
            var configPath = GetConfigurationFilePath(config);
            if (!File.Exists(configPath))
            {
                _logger.LogInformation("No saved configuration found at {ConfigPath}", configPath);
                return null;
            }

            var json = File.ReadAllText(configPath);
            var savedConfig = DnsConfiguration.FromJson(json);

            if (savedConfig != null)
            {
                _logger.LogInformation("Loaded saved configuration from {ConfigPath}", configPath);
            }

            return savedConfig;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load saved configuration");
            return null;
        }
    }

    public bool NeedsCertificateRegeneration(DnsConfiguration config)
    {
        var savedConfig = LoadSavedConfiguration(config);

        if (savedConfig == null)
        {
            _logger.LogInformation("No saved configuration - certificates will be generated");
            return true;
        }

        var currentHash = config.CalculateConfigurationHash();
        var savedHash = savedConfig.CalculateConfigurationHash();

        if (currentHash != savedHash)
        {
            _logger.LogWarning("Configuration has changed - certificates need regeneration");
            _logger.LogInformation("Current hash: {CurrentHash}", currentHash);
            _logger.LogInformation("Saved hash: {SavedHash}", savedHash);
            return true;
        }

        _logger.LogInformation("Configuration unchanged - certificates are still valid");
        return false;
    }

    public IDevAppHost EnsureCertSetup(DnsConfiguration dnsConfig, bool? force = false)
    {
        switch (dnsConfig.SslGenerator)
        {
            case "C#":
                return EnsureCertSetupUsingGeneratedCerts(dnsConfig, force);
            case "mkcert":
                return EnsureCertSetupUsingMkCert(dnsConfig, force);
            default: throw new NotImplementedException(dnsConfig.SslGenerator);
        }
    }

    public IDevAppHost EnsureCertSetupUsingGeneratedCerts(DnsConfiguration dnsConfig, bool? force = false)
    {
        try
        {
            _logger.LogInformation("Checking generated certificate availability...");

            var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dnsConfig.CertificatesDirectory);
            Directory.CreateDirectory(certsDir);

            var certPemFile = Path.Combine(certsDir, $"{dnsConfig.DomainOrDie()}.pem");
            var keyPemFile = Path.Combine(certsDir, $"{dnsConfig.DomainOrDie()}-key.pem");
            var pfxFile = Path.Combine(certsDir, $"{dnsConfig.DomainOrDie()}.pfx");

            bool shouldRegenerate = (force ?? false) || !File.Exists(certPemFile) || !File.Exists(keyPemFile) || NeedsCertificateRegeneration(dnsConfig);

            if (!shouldRegenerate)
            {
                _logger.LogInformation("✓ Using existing generated PEM certificate and key: {CertPem} / {KeyPem}", certPemFile, keyPemFile);
                dnsConfig.CertPath = certPemFile;
                dnsConfig.KeyPath = keyPemFile;
                return this;
            }

            _logger.LogInformation("Generating self-signed certificate (PEM) for domain {Domain}...", dnsConfig.DomainOrDie());

            using var rsa = RSA.Create(2048);
            var subjectName = new X500DistinguishedName($"CN={dnsConfig.DomainOrDie()}");

            var req = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach (var host in dnsConfig.GetAllHosts())
            {
                sanBuilder.AddDnsName(host);
            }
            // include bare domain as well
            if (!string.IsNullOrEmpty(dnsConfig.Domain)) sanBuilder.AddDnsName(dnsConfig.Domain);
            req.CertificateExtensions.Add(sanBuilder.Build());

            var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
            var notAfter = notBefore.AddYears(2);

            using var cert = req.CreateSelfSigned(notBefore, notAfter);

            // Ensure we have a certificate instance that is associated with the private key.
            X509Certificate2 certWithKey = cert;
            var createdCopy = false;
            if (!cert.HasPrivateKey)
            {
                certWithKey = cert.CopyWithPrivateKey(rsa);
                createdCopy = true;
            }

            // Export certificate and private key for PEM files
            var certDer = certWithKey.Export(X509ContentType.Cert);

            byte[] keyDer;
            try
            {
                keyDer = rsa.ExportPkcs8PrivateKey();
            }
            catch
            {
                var certRsa = certWithKey.GetRSAPrivateKey();
                if (certRsa == null)
                    throw new InvalidOperationException("Unable to export private key for PEM generation.");
                keyDer = certRsa.ExportPkcs8PrivateKey();
            }

            static string ToPem(string header, byte[] data)
            {
                var base64 = Convert.ToBase64String(data);
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"-----BEGIN {header}-----");
                for (int i = 0; i < base64.Length; i += 64)
                {
                    sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
                }
                sb.AppendLine($"-----END {header}-----");
                return sb.ToString();
            }

            var certPem = ToPem("CERTIFICATE", certDer);
            var keyPem = ToPem("PRIVATE KEY", keyDer);

            File.WriteAllText(certPemFile, certPem);
            File.WriteAllText(keyPemFile, keyPem);

            _logger.LogInformation("✓ PEM certificate and key saved: {CertPem} / {KeyPem}", certPemFile, keyPemFile);

            // Export PFX with password and import with Exportable flag so OS/browser can use the private key
            var pfxPassword = string.IsNullOrEmpty(dnsConfig.SslPassword) ? Guid.NewGuid().ToString("N") : dnsConfig.SslPassword;
            var pfxBytes = certWithKey.Export(X509ContentType.Pfx, pfxPassword);
            File.WriteAllBytes(pfxFile, pfxBytes);

            // Import certificate with Exportable flag to allow OS to use private key and future exports
            var imported = new X509Certificate2(pfxBytes, pfxPassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable);

            // Add to CurrentUser\My (Personal)
            using (var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                myStore.Open(OpenFlags.ReadWrite);
                if (!myStore.Certificates.Cast<X509Certificate2>().Any(c => c.Thumbprint == imported.Thumbprint))
                {
                    myStore.Add(imported);
                }
                myStore.Close();
            }

            _logger.LogInformation("✓ Installed certificate into CurrentUser\\My: {Subject}", imported.Subject);

            // Try to trust certificate system-wide. On Windows, prefer LocalMachine\Root when running elevated.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    if (IsRunningAsAdministrator())
                    {
                        using var lmRoot = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                        lmRoot.Open(OpenFlags.ReadWrite);
                        if (!lmRoot.Certificates.Cast<X509Certificate2>().Any(c => c.Thumbprint == imported.Thumbprint))
                        {
                            lmRoot.Add(imported);
                            _logger.LogInformation("✓ Installed certificate into LocalMachine\\Root (trusted): {Subject}", imported.Subject);
                        }
                        lmRoot.Close();
                    }
                    else
                    {
                        using var userRoot = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                        userRoot.Open(OpenFlags.ReadWrite);
                        if (!userRoot.Certificates.Cast<X509Certificate2>().Any(c => c.Thumbprint == imported.Thumbprint))
                        {
                            userRoot.Add(imported);
                            _logger.LogInformation("✓ Installed certificate into CurrentUser\\Root (trusted): {Subject}", imported.Subject);
                        }
                        userRoot.Close();
                        _logger.LogWarning("Not running as admin: certificate was added to CurrentUser\\Root. Some browsers may still require machine trust or restart.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add certificate to Windows Root store: {Message}", ex.Message);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Try to add certificate to system CA store (Debian/Ubuntu style)
                try
                {
                    var targetPath = $"/usr/local/share/ca-certificates/{dnsConfig.DomainOrDie()}.crt";
                    var tempCert = certPemFile;

                    // copy cert to target and update CA (requires sudo)
                    var cmd = $"sudo cp \"{tempCert}\" \"{targetPath}\" && sudo update-ca-certificates";
                    if (!ExecuteCommand("sh", $"-c \"{cmd}\"", out var output))
                    {
                        // try alternative: update-ca-trust (RHEL/CentOS)
                        var alt = $"sudo cp \"{tempCert}\" /etc/pki/ca-trust/source/anchors/ && sudo update-ca-trust extract";
                        if (!ExecuteCommand("sh", $"-c \"{alt}\"", out var out2))
                        {
                            _logger.LogWarning("Failed to install cert to system trust on Linux: {Output}\n{Output2}", output, out2);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("✓ Installed certificate into system CA store (Linux)");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add certificate to system trust on Linux: {Message}", ex.Message);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS: use security CLI to add to System keychain (requires sudo)
                try
                {
                    var cmd = $"sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain \"{certPemFile}\"";
                    if (!ExecuteCommand("sh", $"-c \"{cmd}\"", out var output))
                    {
                        _logger.LogWarning("Failed to add cert to macOS System keychain: {Output}", output);
                    }
                    else
                    {
                        _logger.LogInformation("✓ Installed certificate into macOS System keychain (trusted)");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add certificate to macOS trust store: {Message}", ex.Message);
                }
            }

            // Cleanup copied certificate if created
            if (createdCopy && !ReferenceEquals(certWithKey, cert))
            {
                certWithKey.Dispose();
            }

            dnsConfig.CertPath = certPemFile;
            dnsConfig.KeyPath = keyPemFile;

            // Persist configuration
            SaveConfiguration(dnsConfig);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate self-signed certificate (PEM): {ex.Message}", ex);
        }

        return this;
    }

    public IDevAppHost EnsureCertSetupUsingMkCert(DnsConfiguration dnsConfig, bool? force = false)
    {
        try
        {
            _logger.LogInformation("Checking mkcert installation...");

            // Check if mkcert is installed
            var mkcertCheck = ExecuteCommand("mkcert", "-help", out var output);
            if (!mkcertCheck)
            {
                throw new InvalidOperationException(
                    "mkcert is not installed. Please install it:\n" +
                    "Windows: choco install mkcert\n" +
                    "macOS: brew install mkcert\n" +
                    "Linux: See https://github.com/FiloSottile/mkcert#installation");
            }

            _logger.LogInformation("Installing mkcert local CA...");
            // Install local CA if not already installed
            ExecuteCommand("mkcert", "-install", out _);

            // Generate certificates for all hosts
            var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dnsConfig.CertificatesDirectory);
            Directory.CreateDirectory(certsDir);

            var certFile = Path.Combine(certsDir, $"{dnsConfig.DomainOrDie()}.pem");
            var keyFile = Path.Combine(certsDir, $"{dnsConfig.DomainOrDie()}-key.pem");

            bool shouldRegenerate = (force ?? false) || !File.Exists(certFile) || !File.Exists(keyFile) || NeedsCertificateRegeneration(dnsConfig);

            if (!shouldRegenerate)
            {
                _logger.LogInformation("✓ Using existing certificates: {CertFile}", certFile);
                // Get certificate paths
                dnsConfig.CertPath = GetCertificatePath(dnsConfig);
                dnsConfig.KeyPath = GetKeyPath(dnsConfig);
                return this;
            }

            if (force ?? false)
            {
                _logger.LogInformation("Force regeneration of SSL certificates...");
            }
            else if (!File.Exists(certFile) || !File.Exists(keyFile))
            {
                _logger.LogInformation("Generating SSL certificates...");
            }
            else
            {
                _logger.LogInformation("Configuration changed - regenerating SSL certificates...");
            }

            // Delete old certificates if they exist
            if (File.Exists(certFile)) File.Delete(certFile);
            if (File.Exists(keyFile)) File.Delete(keyFile);

            var hostsArg = string.Join(" ", dnsConfig.GetAllHosts());
            var command = $"-cert-file \"{certFile}\" -key-file \"{keyFile}\" {hostsArg}";

            var success = ExecuteCommand("mkcert", command, out var certOutput);

            if (!success)
            {
                throw new InvalidOperationException($"Failed to generate certificates: {certOutput}");
            }

            _logger.LogInformation("✓ Certificates generated: {CertFile}", certFile);

            // Save current configuration
            SaveConfiguration(dnsConfig);

            // Get certificate paths
            dnsConfig.CertPath = GetCertificatePath(dnsConfig);
            dnsConfig.KeyPath = GetKeyPath(dnsConfig);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to setup mkcert: {ex.Message}", ex);
        }

        return this;
    }

    public bool ExecuteCommand(string command, string arguments, out string output)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                output = "Failed to start process";
                return false;
            }

            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            output = outputBuilder.Length > 0 ? outputBuilder.ToString() : errorBuilder.ToString();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            output = ex.Message;
            return false;
        }
    }

    public IDevAppHost EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false)
    {
        // Check if running with elevated privileges on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsRunningAsAdministrator())
        {
            throw new InvalidOperationException("need admin rights");
        }

        if (NeedsHostsFileUpdate(dnsConfig))
        {
            UpdateHostsFile(dnsConfig);
        }

        // Ensure mkcert is installed and certificates are generated
        EnsureCertSetup(dnsConfig, forceCertificateRegeneration);

        return this;

    }

    /// <summary>
    /// Ensures that the hosts file is configured according to the specified DNS settings, updating it if necessary.
    /// </summary>
    /// <param name="dnsConfig">The DNS configuration to apply to the hosts file. Cannot be null.</param>
    /// <returns>The current instance of <see cref="IDevAppHost"/> after ensuring the hosts file is set up.</returns>
    public IDevAppHost EnsureHostsSetup(DnsConfiguration dnsConfig)
    {
        if (NeedsHostsFileUpdate(dnsConfig))
        {
            UpdateHostsFile(dnsConfig);
        }

        return this;
    }

    public string GetCertificatePath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Domain}.pem");
    }

    public string GetKeyPath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Domain}-key.pem");
    }

    public void UpdateHostsFile(DnsConfiguration config)
    {
        try
        {
            _logger.LogInformation("Updating hosts file configuration...");

            var hostsFilePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   ? @"C:\Windows\System32\drivers\etc\hosts"
                 : "/etc/hosts";

            var entries = config.GetHostsFileEntries().ToList();
            var hostsContent = File.Exists(hostsFilePath) ? File.ReadAllText(hostsFilePath) : "";
            var missingEntries = new List<string>();

            foreach (var entry in entries)
            {
                if (!hostsContent.Contains(entry))
                {
                    missingEntries.Add(entry);
                }
            }

            if (missingEntries.Any())
            {
                DoUpdateEtcHostsFile(hostsFilePath, hostsContent, missingEntries);
            }
            else
            {
                _logger.LogInformation("✓ All hosts file entries already exist.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠ Warning: Could not update hosts file: {Message}", ex.Message);
            _logger.LogWarning("Please add the DNS entries manually to your hosts file.");
        }
    }

    private void DoUpdateEtcHostsFile(string hostsFilePath, string hostsContent, List<string> missingEntries)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // On Windows with admin rights, update directly
            if (IsRunningAsAdministrator())
            {
                _logger.LogInformation("Adding entries to hosts file...");

                // Ensure there's a newline at the end of the file
                var contentToAppend = hostsContent;
                if (!string.IsNullOrEmpty(hostsContent) && !hostsContent.EndsWith(Environment.NewLine))
                {
                    contentToAppend += Environment.NewLine;
                }

                contentToAppend += Environment.NewLine;
                contentToAppend += "# Added by FrenchExDev.Net.CSharp.ProjectDependency3" + Environment.NewLine;
                foreach (var entry in missingEntries)
                {
                    contentToAppend += entry + Environment.NewLine;
                }

                File.WriteAllText(hostsFilePath, contentToAppend);
                _logger.LogInformation("✓ Hosts file updated successfully.");
            }
            else
            {
                _logger.LogWarning("⚠ Not running as Administrator. Cannot update hosts file.");
            }
        }
        else
        {
            // Unix systems
            _logger.LogInformation("Adding entries to {HostsFilePath}...", hostsFilePath);

            try
            {
                var tempFile = Path.GetTempFileName();
                var contentToWrite = Environment.NewLine + "# Added by FrenchExDev.Net.CSharp.ProjectDependency4" + Environment.NewLine;
                contentToWrite += string.Join(Environment.NewLine, missingEntries) + Environment.NewLine;
                File.WriteAllText(tempFile, contentToWrite);

                var success = ExecuteCommand("sudo", $"sh -c 'cat {tempFile} >> {hostsFilePath}'", out var output);

                File.Delete(tempFile);

                if (success)
                {
                    _logger.LogInformation("✓ Hosts file updated successfully.");
                }
                else
                {
                    _logger.LogWarning("⚠ Could not automatically update hosts file: {Output}", output);
                    _logger.LogWarning("Please run: sudo nano /etc/hosts");
                    _logger.LogWarning("And add these entries:");
                    foreach (var entry in missingEntries)
                    {
                        _logger.LogWarning("  {Entry}", entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠ Could not automatically update hosts file: {Message}", ex.Message);
                _logger.LogWarning("Please run: sudo nano /etc/hosts");
                _logger.LogWarning("And add these entries:");
                foreach (var entry in missingEntries)
                {
                    _logger.LogWarning("  {Entry}", entry);
                }
            }
        }
    }

    protected IDistributedApplicationBuilder BuilderOrDie() => _builderProvider() ?? throw new InvalidOperationException("Builder not initialized. Call SetupBuilder first.");

    public IDevAppHost SetupProject(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name)
    {
        var aspNetCoreUrl = DnsConfiguration.GetAspNetCoreUrl(name);

        resourceBuilder(_builder ?? throw new InvalidOperationException("_builder is null"))
            .WithEnvironment("DOTNET_LAUNCH_PROFILE", "local-dev-https")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", DnsConfiguration.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", DnsConfiguration.KeyPathOrDie())
            .WithEnvironment("ASPNETCORE_URLS", aspNetCoreUrl)
            .WithEnvironment("CustomDomain__Fqdn", DnsConfiguration.GetUrl(name))
            .WithEnvironment("CustomDomain__Port", DnsConfiguration.Ports[name].ToString())
            .WithUrl(DnsConfiguration.GetUrl(name, DnsConfiguration.Ports[name]));

        return this;
    }

    public DistributedApplication Build()
    {
        return _builder?.Build() ?? throw new InvalidOperationException("_buidler is null. Call BuilderOrDie().");
    }
}

// Helper to modify launchSettings.json dynamically using JSONPath-like selection
internal static class LaunchSettingsHelper
{
    /// <summary>
    /// Adds or updates a single environment variable in the specified launchSettings.json profile.
    /// Creates the file/profile/environmentVariables object if necessary.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariable(string launchSettingsPath, string profileName, string variableName, string variableValue)
    {
        AddOrUpdateEnvironmentVariables(launchSettingsPath, profileName, new Dictionary<string, string> { { variableName, variableValue } });
    }

    /// <summary>
    /// Adds or updates multiple environment variables in the specified launchSettings.json profile.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariables(string launchSettingsPath, string profileName, IDictionary<string, string> variables)
    {
        JObject root;

        if (File.Exists(launchSettingsPath))
        {
            var text = File.ReadAllText(launchSettingsPath);
            root = string.IsNullOrWhiteSpace(text) ? new JObject() : JObject.Parse(text);
        }
        else
        {
            root = new JObject();
        }

        // Ensure 'profiles' object exists
        if (root["profiles"] == null || root["profiles"].Type != JTokenType.Object)
            root["profiles"] = new JObject();

        var profiles = (JObject)root["profiles"]!;

        // Ensure the profile object exists
        if (profiles[profileName] == null || profiles[profileName].Type != JTokenType.Object)
            profiles[profileName] = new JObject();

        var profile = (JObject)profiles[profileName]!;

        // Ensure environmentVariables object exists
        if (profile["environmentVariables"] == null || profile["environmentVariables"].Type != JTokenType.Object)
            profile["environmentVariables"] = new JObject();

        var env = (JObject)profile["environmentVariables"]!;

        foreach (var kvp in variables)
        {
            env[kvp.Key] = JToken.FromObject(kvp.Value);
        }

        // Persist back to file with indentation
        var serialized = root.ToString(Formatting.Indented);
        var directory = Path.GetDirectoryName(launchSettingsPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        File.WriteAllText(launchSettingsPath, serialized);
    }

    /// <summary>
    /// Convenience: add/update environment variable for the profile named in DOTNET_LAUNCH_PROFILE env var.
    /// Falls back to provided defaultProfile if env var is not present.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariableUsingEnvProfile(string launchSettingsPath, string variableName, string variableValue, string defaultProfile = "IIS Express")
    {
        var profile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE");
        if (string.IsNullOrEmpty(profile)) profile = defaultProfile;
        AddOrUpdateEnvironmentVariable(launchSettingsPath, profile, variableName, variableValue);
    }
}
