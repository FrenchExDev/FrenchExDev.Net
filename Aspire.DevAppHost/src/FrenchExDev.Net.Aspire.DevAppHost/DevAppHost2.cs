using FrenchExDev.Net.CSharp.Object.Result;
using hosts.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.Json;

namespace FrenchExDev.Net.Aspire.DevAppHost;

/// <summary>
/// Represents a single entry in an /etc/hosts file, associating an IPv4 address with one or more domain names.
/// </summary>
/// <remarks>Use this record to model mappings between IPv4 addresses and domain names as defined in a system's
/// hosts file. Each entry typically corresponds to a line in the hosts file, where the IPv4 address is mapped to one or
/// more domains for local name resolution.</remarks>
public record EtcHostFileEntry
{
    public required string Ipv4 { get; init; }
    public required string[] Domain { get; init; }
}

/// <summary>
/// Represents a collection of application configurations, indexed by application name.
/// </summary>
/// <remarks>Use this class to manage and access configuration settings for multiple applications within a single
/// structure. Each entry in the collection corresponds to a specific application's configuration, identified by its
/// unique name.</remarks>
public class AppsConfiguration2
{
    public required Dictionary<string, AppConfiguration2> Apps { get; init; }
}

/// <summary>
/// Specifies the type of application being configured or executed.
/// </summary>
/// <remarks>Use this enumeration to distinguish between different application roles, such as a web API or a
/// background worker, when configuring services or application behavior.</remarks>
public enum AppKind
{
    /// <summary>
    /// Represents a user interface component for web-based interactions.
    /// </summary>
    WebUi,

    /// <summary>
    /// Represents a web API endpoint or service for handling HTTP requests and responses.
    /// </summary>
    /// <remarks>Use this class to define and expose HTTP-based APIs that can be consumed by clients over the
    /// web. The class typically provides methods for processing incoming requests, performing business logic, and
    /// returning appropriate HTTP responses. Thread safety and authentication requirements depend on the specific
    /// implementation.</remarks>
    WebApi,

    /// <summary>
    /// Represents a background worker that executes tasks independently of the main application thread.
    /// </summary>
    /// <remarks>Use this class to offload work from the main thread, improving application responsiveness.
    /// Web workers are commonly used for running long-running or computationally intensive operations without blocking
    /// user interactions.</remarks>
    WebWorker
}

/// <summary>
/// Represents the configuration settings for an application, including its domain and the number of instances to run.
/// </summary>
public record AppConfiguration2
{
    /// <summary>
    /// Gets the kind of application represented by this instance.
    /// </summary>
    public required AppKind Kind { get; init; }

    /// <summary>
    /// Gets the domain name associated with the current instance.
    /// </summary>
    public required string Domain { get; init; }

    /// <summary>
    /// Gets or sets the number of instances to create or manage.
    /// </summary>
    public required int Instances { get; init; }

    /// <summary>
    /// Gets the value assigned to the Zeroes property.
    /// </summary>
    public required string Zeroes { get; init; }
}

/// <summary>
/// Represents a DNS record containing a domain name and associated port number.
/// </summary>
public record DnsRecord2
{
    /// <summary>
    /// Gets the list of IPv4 addresses associated with this instance.
    /// </summary>
    public required string Ipv4 { get; init; }

    /// <summary>
    /// Gets the domain name associated with the current instance.
    /// </summary>
    public required string? Domain { get; init; }

    /// <summary>
    /// Gets the domain value if it is set; otherwise, throws an exception.
    /// </summary>
    /// <returns>The domain value as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the domain value is not set.</exception>
    public string DomainOrDie() => Domain ?? throw new InvalidOperationException("Domain is not set.");

    /// <summary>
    /// Gets the template string used to generate domain names dynamically.
    /// </summary>
    /// <remarks>The template may include placeholders that are replaced with specific values at runtime to
    /// construct domain names. The format and supported placeholders should be defined by the consuming
    /// application.</remarks>
    public string? DomainTemplate { get; init; }

    /// <summary>
    /// Gets the domain template string, or throws an exception if it is not set.
    /// </summary>
    /// <returns>The domain template string if it is set.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the domain template has not been set.</exception>
    public string DomainTemplateOrDie() => DomainTemplate ?? throw new InvalidOperationException("DomainTemplate is not set.");

    /// <summary>
    /// Gets the network port number used for the connection.
    /// </summary>
    public required int? Port { get; init; }

    /// <summary>
    /// Gets the starting value of the port range to be used.
    /// </summary>
    /// <remarks>The value typically represents the lowest port number in a range allocated for network
    /// operations. Ensure that the specified port is within the valid range for TCP or UDP ports (0 to 65535), and that
    /// it does not conflict with reserved or well-known ports unless explicitly intended.</remarks>
    public required int? PortRangeStart { get; init; }

    /// <summary>
    /// Determines whether the current instance represents a template domain.
    /// </summary>
    /// <returns>true if the domain is a template; otherwise, false.</returns>
    public bool IsTemplate() => !string.IsNullOrWhiteSpace(DomainTemplate);

    /// <summary>
    /// Returns the domain string, optionally substituting the specified value if the domain is a template.
    /// </summary>
    /// <remarks>If the domain is not a template, the value parameter is ignored. If the domain is a template
    /// and value is null, the placeholder will be replaced with a null value, which may result in an incomplete or
    /// invalid domain string.</remarks>
    /// <param name="value">The value to substitute for the "$i" placeholder in the domain template. If null, the placeholder will be
    /// replaced with a null value.</param>
    /// <returns>A string representing the domain. If the domain is a template, the returned string includes the specified value
    /// substituted for the "$i" placeholder; otherwise, the base domain string is returned.</returns>
    public string Get(string? value = null) => IsTemplate() ? DomainTemplateOrDie().Replace("$i", value) : DomainOrDie();
}

/// <summary>
/// Represents the configuration settings for DNS domains, certificates, and related records used for authentication or
/// encryption.
/// </summary>
/// <remarks>This record encapsulates information about the primary domain, associated subdomains and their DNS
/// records, and file system paths to certificate and key files. It provides methods for serialization, deserialization,
/// and retrieval of domain and port information. All required properties must be initialized when creating an instance.
/// The configuration is intended to be immutable after initialization, except for properties explicitly marked as
/// settable.</remarks>
public record DnsConfiguration2
{
    /// <summary>
    /// Gets the path to the directory containing certificate files required for authentication or encryption.
    /// </summary>
    public required string CertificatesDirectory { get; init; }

    /// <summary>
    /// Gets the domain associated with the current instance.
    /// </summary>
    public required string Domain { get; init; }

    /// <summary>
    /// Gets the collection of domain names and their associated DNS records.
    /// </summary>
    /// <remarks>Each entry in the dictionary maps a domain name to its corresponding DNS record. The
    /// collection is initialized during object construction and cannot be modified after initialization.</remarks>
    public required Dictionary<string, DnsRecord2> Subdomains { get; init; }

    /// <summary>
    /// Gets or sets the file system path to the certificate file used for authentication or encryption.
    /// </summary>
    public string? CertPath { get; set; }

    /// <summary>
    /// Gets or sets the file system path to the cryptographic key.
    /// </summary>
    public string? KeyPath { get; set; }

    /// <summary>
    /// Gets the file system path to the certificate. Throws an exception if the certificate has not been initialized.
    /// </summary>
    /// <returns>The file system path to the certificate.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificate has not been initialized. Call EnsureSetup before accessing this property.</exception>
    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    /// <summary>
    /// Gets the file system path to the certificate key. Throws an exception if the certificates have not been
    /// initialized.
    /// </summary>
    /// <returns>The file system path to the certificate key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificates have not been initialized. Call EnsureSetup before accessing this property.</exception>
    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    /// <summary>
    /// Retrieves the fully qualified domain name associated with the specified name.
    /// </summary>
    /// <param name="name">The key used to look up the domain. Cannot be null.</param>
    /// <returns>A result containing the fully qualified domain name if the specified name exists; otherwise, a failure result.</returns>
    public Result<string> GetDomain(string name)
    {
        var exist = Subdomains.TryGetValue(name, out var dns);
        if (!exist || dns is null) return Result<string>.Failure();
        return (dns.Domain + "." + Domain).ToSuccess();
    }

    /// <summary>
    /// Deserializes a JSON string into a new instance of the DnsConfiguration class.
    /// </summary>
    /// <param name="json">A JSON-formatted string that represents a DnsConfiguration object. Cannot be null.</param>
    /// <returns>A DnsConfiguration instance deserialized from the specified JSON string, or null if the JSON is null or invalid.</returns>
    public static Result<DnsConfiguration2> FromJson(string json)
    {
        try
        {
            return Result<DnsConfiguration2>.Success(JsonSerializer.Deserialize<DnsConfiguration2>(json) ?? throw new InvalidDataException());
        }
        catch (Exception ex)
        {
            return ex.ToFailure<DnsConfiguration2>();
        }
    }

    /// <summary>
    /// Converts the current object to its JSON string representation.
    /// </summary>
    /// <remarks>The resulting JSON string includes all public properties of the object. The output may vary
    /// depending on the object's structure and the default serialization options. If the object contains circular
    /// references or unsupported types, serialization may fail.</remarks>
    /// <returns>A JSON-formatted string that represents the current object.</returns>
    public Result<string> ToJson()
    {
        try
        {
            return JsonSerializer.Serialize(this).ToSuccess();
        }
        catch (Exception ex)
        {
            return ex.ToFailure<string>();
        }
    }

    /// <summary>
    /// Retrieves the port number associated with the specified DNS record name.
    /// </summary>
    /// <param name="name">The name of the DNS record for which to retrieve the port number. Cannot be null.</param>
    /// <returns>A <see cref="Result{T}"/> containing the port number if the DNS record exists; otherwise, a failure result.</returns>
    public Result<int> GetPortOfDns(string name)
    {
        var exist = Subdomains.TryGetValue(name, out var dns);

        if (!exist || dns is null || !dns.Port.HasValue) return Result<int>.Failure();

        return dns.Port.Value.ToSuccess();
    }
}

/// <summary>
/// Defines a contract for performing additional setup operations required by the development application host.
/// </summary>
/// <remarks>Implementations of this interface should provide logic to ensure that any necessary configuration or
/// initialization steps specific to the development environment are completed. This interface extends the capabilities
/// of a standard development application host by introducing additional setup requirements.</remarks>
public interface IDevAppHost2
{
    void EnsureSetup2();
}

public class DevAppHost2Builder
{
    protected ILogger? _logger;
    protected bool _ensureDevSetup2;

    public static IDistributedApplicationBuilder Defaults(params string[] args)
    {
        return new DevAppHost2Builder().WithDefaultLogger().EnsureDevSetup2().CreateBuilder(args);
    }

    public IDistributedApplicationBuilder CreateBuilder(params string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        if (_ensureDevSetup2 && _logger is not null)
        {
            builder.EnsureDevSetup2(_logger);
        }

        return builder;
    }

    public DevAppHost2Builder EnsureDevSetup2()
    {
        _ensureDevSetup2 = true;
        return this;
    }

    public DevAppHost2Builder WithDefaultLogger()
    {
        _logger = LoggerFactory.Create(c =>
        {
            c.SetMinimumLevel(LogLevel.Debug);
            c.AddConsole();
        }).CreateLogger("apphost");

        return this;
    }
}

/// <summary>
/// Provides functionality for configuring and managing development application hosting, including DNS and SSL
/// certificate setup, within a distributed application environment.
/// </summary>
/// <remarks>DevAppHost2 is intended for use in development scenarios where local DNS and SSL certificate
/// management are required to support distributed application workflows. It integrates with the distributed application
/// builder to ensure necessary configuration, updates the system hosts file as needed, and manages SSL certificates
/// using mkcert. This class is typically used as part of the application startup process to automate environment setup
/// and validation. Thread safety is not guaranteed; use from a single thread or ensure external synchronization if
/// accessed concurrently.</remarks>
public class DevAppHost2 : IDevAppHost2
{
    private ILogger _logger;
    private IDistributedApplicationBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the DevAppHost2 class with the specified distributed application builder and
    /// logger.
    /// </summary>
    /// <param name="builder">The distributed application builder used to configure and construct the application components.</param>
    /// <param name="logger">The logger used to record diagnostic and operational messages for the application host.</param>
    public DevAppHost2(IDistributedApplicationBuilder builder, ILogger logger)
    {
        _logger = logger;
        _builder = builder;
    }

    /// <summary>
    /// Ensures that the application is properly configured and running with the required administrator or root
    /// privileges. Registers necessary services and validates configuration sections for application and DNS settings.
    /// </summary>
    /// <remarks>This method should be called during application startup to verify that all required
    /// configuration sections are present and that the application has sufficient privileges to perform necessary setup
    /// tasks. If the DNS configuration section is missing, a warning is logged and the method returns without making
    /// changes. If the hosts file requires updates based on the DNS configuration, the method attempts to update it and
    /// logs a warning.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the application is not running with administrator or root privileges.</exception>
    public void EnsureSetup2()
    {
        _builder.Services.AddSingleton<IDevAppHost2>(this);

        if (!IsRunningAsAdministrator().ObjectOrThrow())
        {
            _logger.LogError("The application is not running with administrator/root privileges. Please restart Visual Studio with Administrative rights.");
            throw new InvalidOperationException("The application is not running with administrator/root privileges. Please restart Visual Studio with Administrative rights.");
        }

        var appsConfigurationSection = _builder.Configuration.GetSection("AppsConfiguration").Get<AppsConfiguration2>();
        var dnsConfigurationSection = _builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration2>();

        ArgumentNullException.ThrowIfNull(appsConfigurationSection, "AppsConfiguration section is missing in configuration.");
        ArgumentNullException.ThrowIfNull(dnsConfigurationSection, "DnsConfiguration section is missing in configuration.");

        UpdateHostsFileIfNeeded(dnsConfigurationSection, appsConfigurationSection);
    }

    /// <summary>
    /// Checks whether the system hosts file requires updates based on the provided DNS and application configurations,
    /// and updates it if necessary.
    /// </summary>
    /// <remarks>This method logs a warning if the hosts file may need to be updated to support development
    /// domains. It then attempts to update the hosts file with the required entries. Administrative privileges may be
    /// required to modify the hosts file.</remarks>
    /// <param name="dnsConfiguration">The DNS configuration containing domain and address mappings to be validated against the hosts file.</param>
    /// <param name="appsConfiguration">The application configuration specifying which applications and domains should be mapped in the hosts file.</param>
    protected void UpdateHostsFileIfNeeded(DnsConfiguration2 dnsConfiguration, AppsConfiguration2 appsConfiguration)
    {
        var neededDnsLinesForApps = GetAllDnses(dnsConfiguration, appsConfiguration);

        if (NeedsHostsFileUpdate(neededDnsLinesForApps).ObjectOrThrow())
        {
            _logger.LogWarning("The hosts file may need to be updated to map development domains. Please ensure the necessary entries are present.");
            UpdateHostsFile(neededDnsLinesForApps);
        }
    }

    /// <summary>
    /// Determines whether the current process is running with administrator privileges on Windows.
    /// </summary>
    /// <remarks>On non-Windows platforms, this method always returns a successful result with a value of <see
    /// langword="false"/>. If an error occurs while checking the administrator status, the result will indicate failure
    /// and contain the exception.</remarks>
    /// <returns>A <see cref="Result{Boolean}"/> indicating whether the process is running as an administrator. The result is
    /// <see langword="false"/> on non-Windows platforms or if the check fails.</returns>
    protected Result<bool> IsRunningAsAdministrator()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Result<bool>.Success(false);

        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator).ToSuccess();
        }
        catch (Exception ex)
        {
            return ex.ToFailure<bool>();
        }
    }

    /// <summary>
    /// Generates a list of fully qualified DNS names for all applications based on the provided DNS and application
    /// configurations.
    /// </summary>
    /// <remarks>If a subdomain configuration uses a template, the method generates DNS names for each
    /// application instance by replacing the template variable with the instance number. Otherwise, a single DNS name
    /// is generated per application. The order of the returned DNS names corresponds to the order of applications in
    /// the configuration.</remarks>
    /// <param name="dnsConfig">The DNS configuration containing domain and subdomain templates used to construct DNS names.</param>
    /// <param name="appsConfig">The application configuration specifying the set of applications and their associated domains and instance
    /// counts.</param>
    /// <returns>A list of strings, each representing a fully qualified DNS name for an application instance as defined by the
    /// configurations.</returns>
    protected List<EtcHostFileEntry> GetAllDnses(DnsConfiguration2 dnsConfig, AppsConfiguration2 appsConfig)
    {
        var neededDnsLinesForApps = new List<EtcHostFileEntry>();

        foreach (var appConfig in appsConfig.Apps)
        {
            var domain = appConfig.Value.Domain;
            var dnsConfigForDomain = dnsConfig.Subdomains[domain];

            if (dnsConfigForDomain.IsTemplate())
            {
                for (var i = 1; i < appConfig.Value.Instances + 1; i++)
                {
                    neededDnsLinesForApps.Add(new EtcHostFileEntry() { Domain = [dnsConfigForDomain.DomainTemplateOrDie().Replace("$i", i.ToString("D" + appConfig.Value.Zeroes)) + "." + dnsConfig.Domain], Ipv4 = dnsConfigForDomain.Ipv4 });
                }
            }
            else
            {
                neededDnsLinesForApps.Add(new EtcHostFileEntry() { Domain = [dnsConfigForDomain.DomainOrDie() + "." + dnsConfig.Domain], Ipv4 = dnsConfigForDomain.Ipv4 });
            }
        }

        return neededDnsLinesForApps;
    }

    /// <summary>
    /// Determines whether the hosts file requires an update to include the specified DNS entries.
    /// </summary>
    /// <param name="neededDnsLinesForApps">A list of DNS hostnames that should be present in the hosts file. Each entry represents a canonical hostname to
    /// check for.</param>
    /// <returns>A result containing <see langword="true"/> if any of the specified DNS entries are missing from the hosts file;
    /// otherwise, a result containing <see langword="false"/>. If an error occurs while accessing the hosts file, the
    /// result contains the exception.</returns>
    protected Result<bool> NeedsHostsFileUpdate(List<EtcHostFileEntry> neededDnsLinesForApps)
    {
        try
        {
            var file = Hosts.OpenHostsFile();

            foreach (var subdomain in neededDnsLinesForApps)
            {
                if (file.Entries.FirstOrDefault(x => subdomain.Domain.All(y => y.Equals(x.CanonicalHostname, StringComparison.InvariantCultureIgnoreCase))) == null)
                {
                    return true.ToSuccess();
                }
            }

            return false.ToSuccess();
        }
        catch (Exception ex)
        {
            return ex.ToFailure<bool>();
        }
    }

    /// <summary>
    /// Gets the full file path to the DNS configuration file based on the specified DNS configuration settings.
    /// </summary>
    /// <param name="config">The DNS configuration settings that specify the certificates directory used to construct the configuration file
    /// path. Cannot be null.</param>
    /// <returns>The full file path to the 'dns-config.json' file located within the certificates directory specified by the
    /// configuration.</returns>
    protected string GetConfigurationFilePath(DnsConfiguration2 config)
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
        return Path.Combine(certsDir, "dns-config.json");
    }

    /// <summary>
    /// Saves the specified DNS configuration to persistent storage.
    /// </summary>
    /// <remarks>If the target directory does not exist, it is created automatically. If an error occurs
    /// during the save operation, the exception is logged and no exception is thrown to the caller.</remarks>
    /// <param name="config">The DNS configuration to be saved. Cannot be null.</param>
    protected void SaveConfiguration(DnsConfiguration2 config)
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

    /// <summary>
    /// Loads a previously saved DNS configuration from persistent storage based on the specified configuration
    /// reference.
    /// </summary>
    /// <remarks>If the configuration file does not exist or cannot be loaded, the method logs an error and
    /// returns a failure result. The returned result provides details about the failure, which can be inspected by the
    /// caller.</remarks>
    /// <param name="config">A reference configuration used to determine the location of the saved configuration file.</param>
    /// <returns>A result containing the loaded DNS configuration if found and successfully deserialized; otherwise, a failure
    /// result indicating the reason for the failure.</returns>
    protected Result<DnsConfiguration2> LoadSavedConfiguration(DnsConfiguration2 config)
    {
        try
        {
            var configPath = GetConfigurationFilePath(config);
            if (!File.Exists(configPath))
            {
                _logger.LogError("No saved configuration found at {ConfigPath}", configPath);
                return Result<DnsConfiguration2>.Failure(b => b.Add("Configuration", "not found."));
            }

            return DnsConfiguration2.FromJson(File.ReadAllText(configPath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load saved configuration");
            return ex.ToFailure<DnsConfiguration2>();
        }
    }

    /// <summary>
    /// Determines whether the current DNS certificate configuration requires regeneration based on the provided
    /// configuration.
    /// </summary>
    /// <remarks>Certificates require regeneration if there is no saved configuration or if the provided
    /// configuration differs from the saved one.</remarks>
    /// <param name="config">The DNS configuration to compare against the saved configuration.</param>
    /// <returns>true if the certificates need to be regenerated; otherwise, false.</returns>
    protected bool NeedsCertificateRegeneration(DnsConfiguration2 config)
    {
        var savedConfig = LoadSavedConfiguration(config);

        if (savedConfig.IsFailure)
        {
            _logger.LogInformation("No saved configuration - certificates will be generated");
            return true;
        }

        if (savedConfig.ObjectOrThrow() != config)
        {
            _logger.LogWarning("Configuration has changed - certificates need regeneration");
            return true;
        }

        _logger.LogInformation("Configuration unchanged - certificates are still valid");
        return false;
    }

    /// <summary>
    /// Ensures that mkcert is installed and properly configured, and generates SSL certificates for the specified DNS
    /// configuration if necessary.
    /// </summary>
    /// <remarks>This method checks for the presence of mkcert, installs the local certificate authority if
    /// needed, and manages SSL certificates for the configured domain and subdomains. Existing certificates are reused
    /// unless regeneration is required due to configuration changes or the <paramref name="force"/> parameter being set
    /// to <see langword="true"/>.</remarks>
    /// <param name="config">The DNS configuration specifying the domain, subdomains, and certificate directory for which SSL certificates
    /// should be managed.</param>
    /// <param name="force">If set to <see langword="true"/>, forces regeneration of SSL certificates even if valid certificates already
    /// exist. If <see langword="false"/> or <see langword="null"/>, certificates are only generated if missing or if
    /// the configuration has changed.</param>
    /// <exception cref="InvalidOperationException">Thrown if mkcert is not installed on the system, or if certificate generation fails.</exception>
    protected void EnsureMkcertSetup(DnsConfiguration2 config, bool? force = false)
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
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
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

        var hostsArg = string.Join(" ", config.Subdomains.Select(x => x.Value.Domain + "." + config.Domain));
        var command = $"-cert-file \"{certFile}\" -key-file \"{keyFile}\" {hostsArg}";

        var success = ExecuteCommand("mkcert", command, out var certOutput);

        if (success.IsFailure)
        {
            throw new InvalidOperationException($"Failed to generate certificates: {certOutput}");
        }

        _logger.LogInformation("✓ Certificates generated: {CertFile}", certFile);

        SaveConfiguration(config);
    }

    /// <summary>
    /// Executes an external command with the specified arguments and captures its output.
    /// </summary>
    /// <remarks>If the process fails to start or an exception is thrown, the output parameter contains the
    /// error message. The method waits for the process to exit before returning.</remarks>
    /// <param name="command">The name or path of the executable file to run. Cannot be null or empty.</param>
    /// <param name="arguments">The command-line arguments to pass to the executable. Can be an empty string if no arguments are required.</param>
    /// <param name="output">When this method returns, contains the standard output or error output produced by the executed process.</param>
    /// <returns>A Result<bool> indicating whether the command executed successfully. The value is <see langword="true"/> if the
    /// process exits with code 0; otherwise, <see langword="false"/>. If the process fails to start or an exception
    /// occurs, the result indicates failure.</returns>
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

    /// <summary>
    /// Constructs the full file system path to the PEM certificate file for the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration containing the domain name and certificates directory used to build the certificate path.
    /// Cannot be null.</param>
    /// <returns>A string representing the absolute path to the PEM certificate file for the specified domain.</returns>
    protected string GetCertificatePath(DnsConfiguration2 config)
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Domain}.pem");
    }

    /// <summary>
    /// Constructs the full file system path to the private key file for the specified DNS configuration.
    /// </summary>
    /// <remarks>The returned path is constructed by combining the application's base directory, the
    /// certificates directory specified in the configuration, and the domain name with a '-key.pem' suffix. Ensure that
    /// the configuration values are valid and that the resulting path is accessible.</remarks>
    /// <param name="config">The DNS configuration containing the domain name and certificates directory used to build the key file path.
    /// Cannot be null.</param>
    /// <returns>A string representing the absolute path to the private key file associated with the specified DNS configuration.</returns>
    protected string GetKeyPath(DnsConfiguration2 config)
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.CertificatesDirectory);
        return Path.Combine(certsDir, $"{config.Domain}-key.pem");
    }

    /// <summary>
    /// Updates the system hosts file with the specified entries if they are not already present.
    /// </summary>
    /// <remarks>This method attempts to add missing entries to the system hosts file on both Windows and
    /// Unix-based platforms. Administrative or elevated privileges are required to modify the hosts file. If the
    /// process does not have sufficient permissions, the method logs a warning and does not perform the update. On
    /// failure, the method logs instructions for manual update. Existing entries are not duplicated.</remarks>
    /// <param name="entries">A list of host file entry strings to ensure are present in the system hosts file. Each entry should be formatted
    /// as a valid hosts file line (e.g., IP address followed by hostname).</param>
    /// <exception cref="InvalidOperationException">Thrown if the hosts file cannot be updated due to an unexpected error.</exception>
    protected void UpdateHostsFile(List<EtcHostFileEntry> entries)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsRunningAsAdministrator().ObjectOrThrow())
        {
            _logger.LogError("⚠ Not running as Administrator. Cannot update hosts file.");
            return;
        }

        try
        {
            _logger.LogInformation("Updating hosts file configuration...");

            var hostsFile = hosts.net.Hosts.OpenHostsFile();

            foreach (var entry in entries)
            {
                hostsFile.AddBlankEntry().SetHost(IPAddress.Parse(entry.Ipv4), entry.Domain.First());
            }

            hostsFile.Write();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠ Warning: Could not update hosts file: {Message}", ex.Message);
            _logger.LogError("Please add the DNS entries manually to your hosts file.");

            throw new InvalidOperationException("Cannot update hosts file.");
        }
    }
}

