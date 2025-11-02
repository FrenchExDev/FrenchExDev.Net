using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace FrenchExDev.Net.Aspire.DevAppHost;

/// <summary>
/// Provides methods for managing development environment setup, including DNS configuration, SSL certificate
/// generation, and hosts file updates for local development applications.
/// </summary>
/// <remarks>DevAppHost is designed to automate common setup tasks required for local development environments,
/// such as ensuring the presence of necessary SSL certificates, updating the system hosts file with required DNS
/// entries, and verifying or elevating process privileges when needed. It supports both Windows and Unix-like operating
/// systems, handling platform-specific requirements transparently. This class is typically used as part of a toolchain
/// to streamline developer onboarding and environment consistency.</remarks>
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

            foreach (var entry in config.Domains)
            {
                if (!hostsContent.Contains(entry.Value.Domain))
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
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
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

            var json = config.ToJson().ObjectOrThrow();
            File.WriteAllText(configPath, json);
            _logger.LogInformation("✓ Configuration saved to {ConfigPath}", configPath);
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
                _logger.LogInformation("✓ Loaded saved configuration from {ConfigPath}", configPath);
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

        if (savedConfig != config)
        {
            _logger.LogWarning("Configuration has changed - certificates need regeneration");
            return true;
        }

        _logger.LogInformation("Configuration unchanged - certificates are still valid");
        return false;
    }

    public void EnsureMkcertSetup(DnsConfiguration config, bool? force = false)
    {
        _logger.LogInformation("Checking mkcert installation...");

        // Check if mkcert is installed
        var mkcertCheck = ExecuteCommand("mkcert", "-help", out var output);
        if (mkcertCheck.IsFailure)
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
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
        Directory.CreateDirectory(certsDir);

        var certFile = Path.Combine(certsDir, $"{config.Domain}.pem");
        var keyFile = Path.Combine(certsDir, $"{config.Domain}-key.pem");

        bool shouldRegenerate = force ?? false || !File.Exists(certFile) || !File.Exists(keyFile) || NeedsCertificateRegeneration(config);

        if (!shouldRegenerate)
        {
            _logger.LogInformation("✓ Using existing certificates: {CertFile}", certFile);
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

        if (File.Exists(certFile)) File.Delete(certFile);
        if (File.Exists(keyFile)) File.Delete(keyFile);

        var hostsArg = string.Join(" ", config.Domains.Select(x => x.Value.Domain));
        var command = $"-cert-file \"{certFile}\" -key-file \"{keyFile}\" {hostsArg}";

        var success = ExecuteCommand("mkcert", command, out var certOutput);

        if (success.IsFailure)
        {
            throw new InvalidOperationException($"Failed to generate certificates: {certOutput}");
        }

        _logger.LogInformation("✓ Certificates generated: {CertFile}", certFile);

        SaveConfiguration(config);
    }

    protected Result<bool> ExecuteCommand(string command, string arguments, out string output)
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
                return Result<bool>.Failure();
            }

            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            output = outputBuilder.Length > 0 ? outputBuilder.ToString() : errorBuilder.ToString();
            return Result<bool>.Success(process.ExitCode == 0);
        }
        catch (Exception ex)
        {
            output = ex.Message;
            return Result<bool>.Failure(ex);
        }
    }

    public void EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false)
    {
        // Check if running with elevated privileges on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!IsRunningAsAdministrator())
            {
                // Check if hosts file needs updating
                if (NeedsHostsFileUpdate(dnsConfig))
                {
                    _logger.LogError("⚠ Administrator privileges required to update hosts file.");
                    _logger.LogError("Restarting application with elevated privileges...\n");

                    if (!RestartAsAdministrator())
                    {
                        _logger.LogError("Failed to restart with administrator privileges.");
                        _logger.LogError("Please run the application as Administrator or manually update the hosts file.");
                        _logger.LogError("\nPress any key to exit...");
                        Console.ReadKey();
                        return;
                    }
                    return; // Exit this instance, elevated instance will continue
                }
            }
        }

        // Ensure mkcert is installed and certificates are generated
        EnsureMkcertSetup(dnsConfig, forceCertificateRegeneration);

        // Get certificate paths
        dnsConfig.CertPath = GetCertificatePath(dnsConfig);
        dnsConfig.KeyPath = GetKeyPath(dnsConfig);

        // Update /etc/hosts with local DNS entries
        UpdateHostsFile(dnsConfig);
    }

    public string GetCertificatePath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Domain}.pem");
    }

    public string GetKeyPath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
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

            var entries = config.Domains.Select(x => x.Value.Domain).ToList();
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
                        _logger.LogError("⚠ Not running as Administrator. Cannot update hosts file.");
                        throw new InvalidOperationException("Cannot update hosts file without Administrator privileges.");
                    }
                    return;
                }

                // Unix systems
                _logger.LogInformation("Adding entries to {HostsFilePath}...", hostsFilePath);

                try
                {
                    var tempFile = Path.GetTempFileName();
                    var contentToWrite = Environment.NewLine + "# Added by FrenchExDev.Net.CSharp.ProjectDependency3" + Environment.NewLine;
                    contentToWrite += string.Join(Environment.NewLine, missingEntries) + Environment.NewLine;
                    File.WriteAllText(tempFile, contentToWrite);

                    var success = ExecuteCommand("sudo", $"sh -c 'cat {tempFile} >> {hostsFilePath}'", out var output);

                    File.Delete(tempFile);

                    if (success.IsSuccess)
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
            else
            {
                _logger.LogInformation("✓ All hosts file entries already exist.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠ Warning: Could not update hosts file: {Message}", ex.Message);
            _logger.LogError("Please add the DNS entries manually to your hosts file.");

            throw new InvalidOperationException("Cannot update hosts file.");
        }
    }
}

