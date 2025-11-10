using FrenchExDev.Net.CSharp.Aspire.App;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace FrenchExDev.Net.CSharp.Aspire.Dev;

/// <summary>
/// Provides functionality for configuring and hosting a distributed development application, including DNS,
/// certificate, and environment setup for local development scenarios.
/// </summary>
/// <remarks>DevAppHost manages the setup of development infrastructure such as DNS configuration, SSL
/// certificates, hosts file entries, and launch settings to enable secure and consistent local development
/// environments. It supports both self-signed and mkcert-generated certificates, and can update system files as needed.
/// The class is intended for use in development and testing scenarios, and may require elevated privileges for certain
/// operations (such as updating the hosts file on Windows). Thread safety is not guaranteed. For typical usage, call
/// EnsureSetup() to initialize the environment before building or launching application resources.</remarks>
public class DevAppHost : IDevAppHost
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IDistributedApplicationBuilder _builder;

    /// <summary>
    /// Creates a new instance of the DevAppHost using the specified distributed application builder, logger name,
    /// configuration file, and environment.
    /// </summary>
    /// <remarks>The configuration file path can include the placeholder "#Env#", which will be replaced with
    /// the specified environment name. This allows for environment-specific configuration files.</remarks>
    /// <param name="builder">A delegate that returns an IDistributedApplicationBuilder used to configure the distributed application.</param>
    /// <param name="loggerName">The name of the logger to use for application logging. If null, an exception is thrown. Defaults to "apphost".</param>
    /// <param name="jsonFile">The path to the JSON configuration file. The string "#Env#" will be replaced with the value of <paramref
    /// name="environment"/>. If null, an exception is thrown. Defaults to "devapphost.#Env#.json".</param>
    /// <param name="environment">The environment name to use for configuration. This value replaces "#Env#" in <paramref name="jsonFile"/>.
    /// Defaults to "Development".</param>
    /// <returns>A DevAppHost instance configured with the specified builder, logger, and configuration.</returns>
    /// <exception cref="InvalidDataException">Thrown if <paramref name="loggerName"/> or <paramref name="jsonFile"/> is null.</exception>
    public static DevAppHost Default(Func<IDistributedApplicationBuilder> builder, string? loggerName = "apphost", string? jsonFile = "devapphost.#Env#.json", string? environment = "Development")
    {
        var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger(loggerName ?? throw new InvalidDataException("missing loggerName"));

        var rootConfig = new ConfigurationBuilder()
            .AddJsonFile("devapphost.json")
            .AddJsonFile(jsonFile?.Replace("#Env#", environment) ?? throw new InvalidDataException("missing jsonFile"))
            .Build();

        var devAppHost = new DevAppHost(builder, logger, rootConfig);

        return devAppHost;
    }

    /// <summary>
    /// Ensures that the development application host is properly configured and ready for use. This includes validating
    /// DNS configuration and performing necessary setup steps.
    /// </summary>
    /// <param name="force">If <see langword="true"/>, forces re-setup of certificates and related configuration even if setup has already
    /// been performed; otherwise, setup is performed only if required. If <see langword="null"/>, the default behavior
    /// is used.</param>
    /// <returns>The current instance of <see cref="IDevAppHost"/> after setup is complete.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the DNS configuration section is missing or invalid in the application configuration.</exception>
    public IDevAppHost EnsureSetup(bool? force = false)
    {
        _dnsConfiguration = DevAppHostConfiguration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? throw new InvalidOperationException("missing DnsConfiguration");

        EnsureCertSetup(force);
        EnsureHostsSetup();
        EnsureLaunchSettingsSetup(DnsConfiguration.DashboardAppName, _configuration["LaunchSettingsProfile"] ?? throw new InvalidDataException("Missing LaunchSettingsProfile configuration"), DevAppHostLaunchSettingsPath());
        EnsureHostsSetup();

        return this;
    }

    /// <summary>
    /// Ensures that the launch settings file is configured with the required environment variables for the specified
    /// application host.
    /// </summary>
    /// <remarks>This method updates the specified launch settings file with environment variables necessary
    /// for ASP.NET Core and related services. The caller must ensure that the provided file path is accessible and that
    /// the configuration contains the required values.</remarks>
    /// <param name="launchProfileName">The name of the application host for which to set up launch settings. This value is used to determine
    /// environment variable values and endpoint configuration.</param>
    /// <param name="launchSettingsFilePath">The file path to the launch settings JSON file to be updated. Must refer to a valid file location.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance representing the configured application host.</returns>
    /// <exception cref="InvalidDataException">Thrown if the required 'LaunchSettingsProfile' configuration value is missing.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the required 'Environment' configuration value is missing.</exception>
    public IDevAppHost EnsureLaunchSettingsSetup(string appName, string launchProfileName, string launchSettingsFilePath)
    {
        LaunchSettingsHelper.AddOrUpdateEnvironmentVariables(
            launchSettingsPath: launchSettingsFilePath,
            profileName: launchProfileName,
            variables: new Dictionary<string, string>
            {
                { "ASPNETCORE_Kestrel__Certificates__Default__Path", DnsConfiguration.CertPathOrDie() },
                { "ASPNETCORE_Kestrel__Certificates__Default__KeyPath", DnsConfiguration.KeyPathOrDie() },
                { "ASPNETCORE_URLS", DnsConfiguration.GetAspNetCoreUrl(appName) },
                { "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", DnsConfiguration.GetUrl(appName, DnsConfiguration.Ports[$"{appName}-oltp"]) },
                { "ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", DnsConfiguration.GetUrl(appName, DnsConfiguration.Ports[$"{appName}-services"]) },
                { "ASPIRE_ENVIRONMENT", DevAppHostConfiguration["Environment"] ?? throw new InvalidOperationException() },
                { "DOTNET_ENVIRONMENT", DevAppHostConfiguration["Environment"]?? throw new InvalidOperationException() },
                { "CustomDomain__Fqdn", DnsConfiguration.GetUrl(appName) },
                { "CustomDomain__Port", DnsConfiguration.Ports[appName].ToString() }
            });

        return this;
    }

    /// <summary>
    /// Gets the apphost' application's configuration settings.
    /// </summary>
    /// <remarks>Use this property to access configuration values such as connection strings, application
    /// options, and environment-specific settings. The returned <see cref="IConfiguration"/> instance provides
    /// hierarchical access to configuration data from multiple sources.</remarks>
    public IConfiguration DevAppHostConfiguration => _configuration;

    /// <summary>
    /// Gets the current DNS configuration for the service.
    /// </summary>
    /// <remarks>Accessing this property before the setup process is complete will result in an exception.
    /// Ensure that the service has been properly initialized before retrieving the DNS configuration.</remarks>
    public DnsConfiguration DnsConfiguration => _dnsConfiguration ?? throw new InvalidOperationException("_DnsConfigurationuration is null. Please EnsureSetup() before.");

    private DnsConfiguration? _dnsConfiguration;
    private DevAppHost(Func<IDistributedApplicationBuilder> builder, ILogger logger, IConfiguration configuration)
    {
        _builder = builder();
        _logger = logger;
        _configuration = configuration;
    }

    protected string DevAppHostLaunchSettingsPath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../..", "Properties", "launchSettings.json");

    /// <summary>
    /// Determines whether the current process is running with administrator privileges on a Windows operating system.
    /// </summary>
    /// <remarks>On non-Windows platforms, this method always returns false. If the administrator status
    /// cannot be determined due to an error, the method also returns false.</remarks>
    /// <returns>true if the process is running as an administrator on Windows; otherwise, false.</returns>
    protected bool IsRunningAsAdministrator()
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

    /// <summary>
    /// Determines whether the system hosts file requires updating to include all entries specified in the provided DNS
    /// configuration.
    /// </summary>
    /// <remarks>If the hosts file cannot be read or accessed, the method assumes an update is needed and
    /// returns true. The method checks the standard hosts file location for the current operating system.</remarks>
    /// <param name="config">The DNS configuration containing the hosts file entries to check for presence in the system hosts file.</param>
    /// <returns>true if the hosts file is missing or does not contain all required entries; otherwise, false.</returns>
    protected bool NeedsHostsFileUpdate(DnsConfiguration config)
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

    /// <summary>
    /// Gets the full file path to the DNS configuration file based on the specified configuration settings.
    /// </summary>
    /// <param name="config">The DNS configuration settings used to determine the certificates directory location.</param>
    /// <returns>A string containing the absolute path to the 'dns-config.json' file within the user's certificates directory.</returns>
    protected string GetConfigurationFilePath()
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), DnsConfiguration.CertificatesDirectory);
        return Path.Combine(certsDir, "dns-config.json");
    }

    /// <summary>
    /// Saves the specified DNS configuration to persistent storage.
    /// </summary>
    /// <remarks>If the target directory does not exist, it will be created automatically. If an error occurs
    /// during saving, the method logs a warning and does not throw an exception.</remarks>
    protected void SaveConfiguration()
    {
        try
        {
            var configPath = GetConfigurationFilePath();
            var directory = Path.GetDirectoryName(configPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = DnsConfiguration.ToJson();
            File.WriteAllText(configPath, json);
            _logger.LogInformation("Configuration saved to {ConfigPath}", configPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save configuration");
        }
    }

    /// <summary>
    /// Loads the saved DNS configuration from persistent storage, if available.
    /// </summary>
    /// <remarks>If no saved configuration file is found or if loading fails, the method returns <see
    /// langword="null"/> and logs a warning. The provided <paramref name="config"/> parameter is not used to modify the
    /// loaded configuration.</remarks>
    /// <param name="config">The DNS configuration to use as a reference when loading the saved configuration. This parameter is not
    /// modified.</param>
    /// <returns>A <see cref="DnsConfiguration"/> instance representing the saved configuration if one exists and can be loaded;
    /// otherwise, <see langword="null"/>.</returns>
    protected Result<DnsConfiguration> LoadSavedConfiguration(DnsConfiguration config)
    {
        return Result<DnsConfiguration>.TryCatch<DnsConfiguration>(() =>
        {
            var configPath = GetConfigurationFilePath();
            if (!File.Exists(configPath))
            {
                _logger.LogWarning("No saved configuration found at {ConfigPath}", configPath);
                return Result<DnsConfiguration>.Failure(body: d => d.Add("File.NotFound", configPath));
            }

            var json = File.ReadAllText(configPath);
            var savedConfig = DnsConfiguration.FromJson(json).ObjectOrThrow();

            return savedConfig.ToSuccess();
        });
    }

    /// <summary>
    /// Determines whether the DNS certificate should be regenerated based on changes in the provided configuration.
    /// </summary>
    /// <remarks>Certificate regeneration is triggered if there is no saved configuration or if the
    /// configuration has changed since the last save. This method does not modify any state or perform certificate
    /// generation itself.</remarks>
    /// <param name="config">The DNS configuration to evaluate for certificate regeneration. Cannot be null.</param>
    /// <returns>true if the certificate needs to be regenerated due to missing or changed configuration; otherwise, false.</returns>
    protected bool NeedsCertificateRegeneration(DnsConfiguration config)
    {
        var savedConfig = LoadSavedConfiguration(config);

        if (savedConfig.IsFailure)
        {
            _logger.LogInformation("No saved configuration - certificates will be generated");
            return true;
        }

        var currentHash = config.CalculateConfigurationHash();
        var savedHash = savedConfig.ObjectOrThrow().CalculateConfigurationHash();

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

    /// <summary>
    /// Ensures that SSL certificate setup is performed according to the configured SSL generator and returns an
    /// application host instance with the certificate applied.
    /// </summary>
    /// <param name="force">Specifies whether to force certificate setup even if it has already been performed. If <see langword="true"/>,
    /// the setup is forced; if <see langword="false"/> or <see langword="null"/>, setup may be skipped if already
    /// completed.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance configured with the appropriate SSL certificate based on the current SSL
    /// generator setting.</returns>
    /// <exception cref="NotImplementedException">Thrown if the configured SSL generator is not supported.</exception>
    protected IDevAppHost EnsureCertSetup(bool? force = false)
    {
        switch (DnsConfiguration.SslGenerator)
        {
            case "C#":
                return EnsureCertSetupUsingGeneratedCerts(force);
            case "mkcert":
                return EnsureCertSetupUsingMkCert(force);
            default: throw new NotImplementedException(DnsConfiguration.SslGenerator);
        }
    }

    /// <summary>
    /// Ensures that self-signed certificates are generated and configured for the current domain, installing them into
    /// the appropriate certificate stores if necessary.
    /// </summary>
    /// <remarks>On Windows, the certificate is installed into the CurrentUser\My and trusted root stores,
    /// preferring LocalMachine\Root if running as administrator. On Linux and macOS, the method attempts to add the
    /// certificate to the system trust store using platform-specific commands, which may require elevated privileges.
    /// Existing certificates are reused unless regeneration is requested or required. This method logs progress and
    /// warnings for installation steps and potential issues.</remarks>
    /// <param name="force">If <see langword="true"/>, forces regeneration and installation of certificates even if valid certificates
    /// already exist; otherwise, certificates are only generated if missing or outdated.</param>
    /// <returns>The current <see cref="IDevAppHost"/> instance with certificate setup completed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if certificate generation or private key export fails.</exception>
    protected IDevAppHost EnsureCertSetupUsingGeneratedCerts(bool? force = false)
    {
        try
        {
            _logger.LogInformation("Checking generated certificate availability...");

            var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
            Directory.CreateDirectory(certsDir);

            var certPemFile = Path.Combine(certsDir, $"{DnsConfiguration.DomainOrDie()}.pem");
            var keyPemFile = Path.Combine(certsDir, $"{DnsConfiguration.DomainOrDie()}-key.pem");
            var pfxFile = Path.Combine(certsDir, $"{DnsConfiguration.DomainOrDie()}.pfx");

            bool shouldRegenerate = (force ?? false) || !File.Exists(certPemFile) || !File.Exists(keyPemFile) || NeedsCertificateRegeneration(DnsConfiguration);

            if (!shouldRegenerate)
            {
                _logger.LogInformation("✓ Using existing generated PEM certificate and key: {CertPem} / {KeyPem}", certPemFile, keyPemFile);
                DnsConfiguration.SetupCertAndKeyPaths(certPemFile, keyPemFile);
                return this;
            }

            _logger.LogInformation("Generating self-signed certificate (PEM) for domain {Domain}...", DnsConfiguration.DomainOrDie());

            using var rsa = RSA.Create(2048);
            var subjectName = new X500DistinguishedName($"CN={DnsConfiguration.DomainOrDie()}");

            var req = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach (var host in DnsConfiguration.GetAllHosts())
            {
                sanBuilder.AddDnsName(host);
            }
            // include bare domain as well
            if (!string.IsNullOrEmpty(DnsConfiguration.Domain)) sanBuilder.AddDnsName(DnsConfiguration.Domain);
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
            var pfxPassword = string.IsNullOrEmpty(DnsConfiguration.SslPassword) ? Guid.NewGuid().ToString("N") : DnsConfiguration.SslPassword;
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
                    var targetPath = $"/usr/local/share/ca-certificates/{DnsConfiguration.DomainOrDie()}.crt";
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

            DnsConfiguration.SetupCertAndKeyPaths(certPemFile, keyPemFile);

            SaveConfiguration();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate self-signed certificate (PEM): {ex.Message}", ex);
        }

        return this;
    }

    /// <summary>
    /// Ensures that SSL certificates are set up for development hosts using mkcert, generating new certificates if
    /// necessary.
    /// </summary>
    /// <remarks>This method requires mkcert to be installed and accessible on the system. It installs the
    /// local certificate authority if needed and generates certificates for all configured hosts. Existing certificates
    /// are reused unless regeneration is forced or configuration changes are detected.</remarks>
    /// <param name="force">If <see langword="true"/>, forces regeneration of SSL certificates even if valid certificates already exist;
    /// otherwise, certificates are only generated if missing or outdated.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance with SSL certificates configured for the current development environment.</returns>
    /// <exception cref="InvalidOperationException">Thrown if mkcert is not installed or if certificate generation fails.</exception>
    protected IDevAppHost EnsureCertSetupUsingMkCert(bool? force = false)
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
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
        Directory.CreateDirectory(certsDir);

        var certFile = Path.Combine(certsDir, $"{DnsConfiguration.DomainOrDie()}.pem");
        var keyFile = Path.Combine(certsDir, $"{DnsConfiguration.DomainOrDie()}-key.pem");

        bool shouldRegenerate = (force ?? false) || !File.Exists(certFile) || !File.Exists(keyFile) || NeedsCertificateRegeneration(DnsConfiguration);

        if (!shouldRegenerate)
        {
            _logger.LogInformation("✓ Using existing certificates: {CertFile}", certFile);
            // Get certificate paths
            DnsConfiguration.SetupCertAndKeyPaths(GetCertificatePath(), GetKeyPath());
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

        var hostsArg = string.Join(" ", DnsConfiguration.GetAllHosts());
        var command = $"-cert-file \"{certFile}\" -key-file \"{keyFile}\" {hostsArg}";

        var success = ExecuteCommand("mkcert", command, out var certOutput);

        if (!success)
        {
            throw new InvalidOperationException($"Failed to generate certificates: {certOutput}");
        }

        _logger.LogInformation("✓ Certificates generated: {CertFile}", certFile);

        SaveConfiguration();

        DnsConfiguration.SetupCertAndKeyPaths(GetCertificatePath(), GetKeyPath());

        return this;
    }

    /// <summary>
    /// Executes an external command with the specified arguments and captures its output.
    /// </summary>
    /// <remarks>This method starts the specified process without creating a window and redirects both
    /// standard output and standard error. If the process fails to start or an exception occurs, the output parameter
    /// will contain an error message. The method blocks until the process exits.</remarks>
    /// <param name="command">The name or path of the executable file to run. Cannot be null or empty.</param>
    /// <param name="arguments">The command-line arguments to pass to the executable. Can be an empty string if no arguments are required.</param>
    /// <param name="output">When this method returns, contains the standard output of the process if available; otherwise, contains the
    /// standard error or an error message if the process fails to start.</param>
    /// <returns>true if the command executes successfully and returns an exit code of 0; otherwise, false.</returns>
    protected bool ExecuteCommand(string command, string arguments, out string output)
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


    /// <summary>
    /// Ensures that the hosts file is configured according to the specified DNS settings, updating it if necessary.
    /// </summary>
    /// <returns>The current instance of <see cref="IDevAppHost"/> after ensuring the hosts file is set up.</returns>
    protected IDevAppHost EnsureHostsSetup()
    {
        if (NeedsHostsFileUpdate(DnsConfiguration))
        {
            UpdateHostsFile(DnsConfiguration);
        }

        return this;
    }

    /// <summary>
    /// Gets the full file system path to the PEM-encoded certificate for the configured DNS domain.
    /// </summary>
    /// <remarks>The returned path is constructed using the application's base directory and the certificate
    /// directory specified in the DNS configuration. The file is expected to have a ".pem" extension and be named after
    /// the configured domain.</remarks>
    /// <returns>A string containing the absolute path to the certificate file for the current DNS domain.</returns>
    public string GetCertificatePath()
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
        return Path.Combine(certsDir, $"{DnsConfiguration.Domain}.pem");
    }

    /// <summary>
    /// Gets the full file system path to the private key file for the configured domain.
    /// </summary>
    /// <remarks>The returned path is constructed using the application's base directory and the configured
    /// certificates directory. The file is expected to be named in the format "{Domain}-key.pem". Ensure that the
    /// domain and certificates directory are correctly configured in <see cref="DnsConfiguration"/> before calling this
    /// method.</remarks>
    /// <returns>A string containing the absolute path to the private key file for the current domain.</returns>
    public string GetKeyPath()
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
        return Path.Combine(certsDir, $"{DnsConfiguration.Domain}-key.pem");
    }

    /// <summary>
    /// Updates the system hosts file with DNS entries specified in the provided configuration.
    /// </summary>
    /// <remarks>This method attempts to add any missing DNS entries from the configuration to the system
    /// hosts file. If the update cannot be performed (for example, due to insufficient permissions), a warning is
    /// logged and manual intervention may be required. The method determines the hosts file path based on the operating
    /// system.</remarks>
    /// <param name="config">The DNS configuration containing the hosts file entries to be added. Cannot be null.</param>
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

    /// <summary>
    /// Updates the system hosts file by appending the specified entries, ensuring that missing host mappings are added
    /// for local development or network configuration.
    /// </summary>
    /// <remarks>This method requires administrative privileges to modify the hosts file. On Windows, the
    /// process must be running as an administrator; on Unix-based systems, 'sudo' is used to append entries. If
    /// automatic update fails, manual intervention may be required. The method appends a comment indicating the source
    /// of the changes for traceability.</remarks>
    /// <param name="hostsFilePath">The full path to the hosts file to update. On Windows, this is typically
    /// 'C:\Windows\System32\drivers\etc\hosts'; on Unix-based systems, it is usually '/etc/hosts'.</param>
    /// <param name="hostsContent">The existing content of the hosts file, used as the base to which new entries will be appended. May be empty if
    /// the file does not exist or is being created.</param>
    /// <param name="missingEntries">A list of host entry strings that are not currently present in the hosts file and need to be added.</param>
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

    /// <summary>
    /// Configures the application host to use a project resource instance with the specified name and optional
    /// configuration.
    /// </summary>
    /// <remarks>This method sets several environment variables and URLs for the project resource instance,
    /// including development environment settings and certificate paths. It is typically used to prepare the
    /// application host for local development scenarios.</remarks>
    /// <param name="resourceBuilder">A delegate that creates a project resource builder using the distributed application builder. The returned
    /// resource builder is used to configure the project resource instance.</param>
    /// <param name="name">The name of the project resource instance. This value is used to set environment variables and URLs for the
    /// resource.</param>
    /// <param name="configuration">An optional action to further configure the project resource builder before finalizing the resource instance. If
    /// not specified, no additional configuration is applied.</param>
    /// <returns>The current <see cref="IDevAppHost"/> instance with the project resource instance configured.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the distributed application builder is not initialized.</exception>
    public IDevAppHost WithProjectInstance(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name, Action<IResourceBuilder<ProjectResource>>? configuration = null)
    {
        var aspNetCoreUrl = DnsConfiguration.GetAspNetCoreUrl(name);

        IResourceBuilder<ProjectResource> r = resourceBuilder(_builder ?? throw new InvalidOperationException("_builder is null"))
            .WithEnvironment("DOTNET_LAUNCH_PROFILE", "local-dev-https")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", DnsConfiguration.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", DnsConfiguration.KeyPathOrDie())
            .WithEnvironment("ASPNETCORE_URLS", aspNetCoreUrl)
            .WithEnvironment("CustomDomain__Fqdn", DnsConfiguration.GetUrl(name))
            .WithEnvironment("CustomDomain__Port", DnsConfiguration.Ports[name].ToString())
            .WithUrl(DnsConfiguration.GetUrl(name, DnsConfiguration.Ports[name]));

        configuration?.Invoke(r);

        return this;
    }

    /// <summary>
    /// Builds and returns a configured instance of the distributed application.
    /// </summary>
    /// <returns>A <see cref="DistributedApplication"/> instance representing the configured distributed application.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the internal builder is not initialized. Ensure that BuilderOrDie() has been called before invoking
    /// this method.</exception>
    public DistributedApplication Build()
    {
        return _builder?.Build() ?? throw new InvalidOperationException("_buidler is null. Call BuilderOrDie().");
    }
}
