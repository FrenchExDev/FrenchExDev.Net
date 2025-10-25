using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

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

    public void EnsureMkcertSetup(DnsConfiguration config)
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
            var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
            Directory.CreateDirectory(certsDir);

            var certFile = Path.Combine(certsDir, $"{config.Suffix}.pem");
            var keyFile = Path.Combine(certsDir, $"{config.Suffix}-key.pem");

            if (!File.Exists(certFile) || !File.Exists(keyFile))
            {
                _logger.LogInformation("Generating SSL certificates...");

                var hostsArg = string.Join(" ", config.GetAllHosts());
                var command = $"-cert-file \"{certFile}\" -key-file \"{keyFile}\" {hostsArg}";

                var success = ExecuteCommand("mkcert", command, out var certOutput);
                if (success)
                {
                    _logger.LogInformation("✓ Certificates generated: {CertFile}", certFile);
                }
                else
                {
                    throw new InvalidOperationException($"Failed to generate certificates: {certOutput}");
                }
            }
            else
            {
                _logger.LogInformation("✓ Using existing certificates: {CertFile}", certFile);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to setup mkcert: {ex.Message}", ex);
        }
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
                        var contentToWrite = Environment.NewLine + "# Added by FrenchExDev.Net.CSharp.ProjectDependency3" + Environment.NewLine;
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

    public string GetCertificatePath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Suffix}.pem");
    }

    public string GetKeyPath(DnsConfiguration config)
    {
        var certsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Suffix}-key.pem");
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

    public void EnsureSetup(DnsConfiguration dnsConfig)
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
        EnsureMkcertSetup(dnsConfig);

        // Get certificate paths
        dnsConfig.CertPath = GetCertificatePath(dnsConfig);
        dnsConfig.KeyPath = GetKeyPath(dnsConfig);

        // Update /etc/hosts with local DNS entries
        UpdateHostsFile(dnsConfig);
    }
}

