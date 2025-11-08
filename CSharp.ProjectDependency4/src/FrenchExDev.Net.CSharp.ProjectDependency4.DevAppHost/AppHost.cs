using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.Json;

var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger("apphost");
var devAppHost = new DevAppHost(logger);
var rootConfig = new ConfigurationBuilder()
    .AddJsonFile("devapphost.json")
    .Build();
var dnsConfig = rootConfig.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? throw new InvalidOperationException("missing DnsConfiguration");

devAppHost.EnsureCertSetup(dnsConfig);

System.Environment.SetEnvironmentVariable("ASPIRE_ENVIRONMENT", rootConfig["Environment"]);
System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", rootConfig["Environment"]);
System.Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie());
System.Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie());
System.Environment.SetEnvironmentVariable("ASPNETCORE_URLS", dnsConfig.GetDashboardUrl(18443));
System.Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", dnsConfig.GetDashboardUrl(21190));
System.Environment.SetEnvironmentVariable("ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", dnsConfig.GetDashboardUrl(22187));
System.Environment.SetEnvironmentVariable("CustomDomain__Fqdn", dnsConfig.GetDashboardUrl());
System.Environment.SetEnvironmentVariable("CustomDomain__Port", 18443.ToString());

var builder = DistributedApplication.CreateBuilder(args);

devAppHost.EnsureUpdatedHosts(dnsConfig);

builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency4_Viz>("viz")
    .WithEnvironment("DOTNET_LAUNCH_PROFILE", "local-dev-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", "https://0.0.0.0:443;https://viz.pd4i1.com:443")
    .WithEnvironment("CustomDomain__Fqdn", "https://viz.pd4i1.com")
    .WithEnvironment("CustomDomain__Port", 8443.ToString())
    .WithUrl("https://viz.pd4i1.com:443")
    ;

builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency4_Viz_Api>("viz-api")
    .WithEnvironment("DOTNET_LAUNCH_PROFILE", "local-dev-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfig.CertPathOrDie())
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfig.KeyPathOrDie())
    .WithEnvironment("ASPNETCORE_URLS", "https://0.0.0.0:8443;https://viz-api.pd4i1.com:8443")
    .WithEnvironment("CustomDomain__Fqdn", "https://viz-api.pd4i1.com")
    .WithEnvironment("CustomDomain__Port", 8443.ToString())
    .WithUrl("https://viz-api.pd4i1.com:8443")
    ;

var project = builder.Build();

await project.RunAsync();

public record DnsConfiguration
{
    public string CertificatesDirectory { get; set; }
    public string CertPath { get; set; }
    public string KeyPath { get; set; }
    public string Domain { get; set; }

    public string GetDashboardUrl(int? port = null) => "https://devdash.pd4i1.com" + (port is not null ? ":" + port : string.Empty);

    public IEnumerable<string> GetAllHosts()
    {
        yield return "devdash.pd4i1.com";
        yield return "viz.pd4i1.com";
        yield return "viz-api.pd4i1.com";
    }
    public IEnumerable<string> GetHostsFileEntries()
    {
        foreach (var host in GetAllHosts())
        {
            yield return $"127.0.0.1 {host}";
        }
    }

    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    public ReadOnlySpan<char> ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static DnsConfiguration? FromJson(string json)
    {
        return JsonSerializer.Deserialize<DnsConfiguration>(json);
    }

    public string? CalculateConfigurationHash()
    {
        // Create a copy with nulled certificate paths for consistent hashing
        var configForHashing = this with { CertPath = null, KeyPath = null };

        // Use the record's built-in GetHashCode which considers all init properties
        var hashCode = configForHashing.GetHashCode();

        // Convert to hex string for readability and storage
        return hashCode.ToString("X8");
    }

    internal object DomainOrDie() => Domain ?? throw new InvalidOperationException("Domain is not set in the configuration.");
}

public interface IDevAppHost
{
    void EnsureCertSetup(DnsConfiguration config, bool? force = false);
    void EnsureUpdatedHosts(DnsConfiguration dnsConfig);
    void EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false);
}

public class DevAppHost : IDevAppHost
{
    private readonly ILogger _logger;

    public DevAppHost(ILogger logger)
    {
        _logger = logger;
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

    public void EnsureCertSetup(DnsConfiguration dnsConfig, bool? force = false)
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
                return;
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

    public void EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false)
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

    }

    public void EnsureUpdatedHosts(DnsConfiguration dnsConfig)
    {
        if (NeedsHostsFileUpdate(dnsConfig))
        {
            UpdateHostsFile(dnsConfig);
        }
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
}

