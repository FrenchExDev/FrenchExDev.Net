using FrenchExDev.Net.CSharp.Object.Result;
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

/// <summary>
/// Represents the configuration settings for DNS and SSL certificate management, including domain information,
/// certificate paths, port mappings, and ASP.NET Core URL templates.
/// </summary>
/// <remarks>This record provides properties and methods for managing DNS-related configuration in applications
/// that require SSL certificates and domain mapping. It includes support for multiple domains, port assignments, and
/// dynamic URL generation for ASP.NET Core endpoints. Use this type to centralize and access DNS and certificate
/// configuration throughout your application.</remarks>
public record DnsConfiguration
{
    /// <summary>
    /// Gets or sets the password used to access the SSL certificate for secure connections.
    /// </summary>
    /// <remarks>If the SSL certificate is password-protected, this property must be set to the correct
    /// password before establishing a secure connection. The password should be stored and handled securely to prevent
    /// unauthorized access.</remarks>
    public string SslPassword { get;  set; }

    /// <summary>
    /// Gets or sets the name of the SSL certificate generator to use for secure connections.
    /// </summary>
    public string SslGenerator { get;  set; }

    /// <summary>
    /// Gets or sets the directory path where certificate files are stored.
    /// </summary>
    public string CertificatesDirectory { get;  set; }

    /// <summary>
    /// Gets or sets the file system path to the certificate used for authentication.
    /// </summary>
    public string CertPath { get;  set; }

    /// <summary>
    /// Gets or sets the file system path to the private key associated with the certificate.
    /// </summary>
    public string KeyPath { get;  set; }

    /// <summary>
    /// Sets the file system paths for the certificate and private key used by the instance.
    /// </summary>
    /// <param name="certPath">The file path to the certificate file. Cannot be null or empty.</param>
    /// <param name="keyPath">The file path to the private key file. Cannot be null or empty.</param>
    public void SetupCertAndKeyPaths(string certPath, string keyPath)
    {
        CertPath = certPath;
        KeyPath = keyPath;
    }

    /// <summary>
    /// Gets or sets the domain name associated with the current instance.
    /// </summary>
    public string Domain { get;  set; }

    /// <summary>
    /// Gets the list of domain names associated with the current instance.
    /// </summary>
    public string[] Domains { get;  set; }

    /// <summary>
    /// Gets the collection of named ports and their corresponding port numbers for the current instance.
    /// </summary>
    /// <remarks>The dictionary maps port names to their assigned integer values. Modifications to this
    /// collection affect the available ports for the instance. This property is protected set; it can only be modified
    /// within derived classes.</remarks>
    public Dictionary<string, int> Ports { get;  set; }

    /// <summary>
    /// Gets or sets the collection of ASP.NET Core endpoint URLs mapped by their names.
    /// </summary>
    /// <remarks>Each entry in the dictionary represents a named endpoint and its corresponding URL. This
    /// property can be used to configure or retrieve the URLs for different ASP.NET Core services within the
    /// application.</remarks>
    public Dictionary<string, string> AspNetCoreUrls { get;  set; }

    /// <summary>
    /// Generates the ASP.NET Core URL for the specified domain by replacing template placeholders with the
    /// corresponding port and domain values.
    /// </summary>
    /// <remarks>The returned URL is constructed by replacing the placeholders '#port#', '#subdomain#', and
    /// '#domain#' in the template with the appropriate values for the given domain. Ensure that the domain exists in
    /// the internal mappings before calling this method.</remarks>
    /// <param name="domain">The domain name for which to generate the ASP.NET Core URL. Must correspond to a key in the internal URL and
    /// port mappings.</param>
    /// <returns>A string containing the fully constructed ASP.NET Core URL for the specified domain.</returns>
    /// <exception cref="InvalidDataException">Thrown if the specified domain does not exist in the internal URL mapping or if the required data is missing.</exception>
    public string GetAspNetCoreUrl(string domain) => AspNetCoreUrls != null && AspNetCoreUrls.ContainsKey(domain) ? AspNetCoreUrls[domain].Replace("#port#", Ports[domain].ToString()).Replace("#subdomain#", domain).Replace("#domain#", Domain) : throw new InvalidDataException();

    /// <summary>
    /// Constructs a complete HTTPS URL for the specified domain, including the port number if one is configured.
    /// </summary>
    /// <remarks>If a port is configured for the specified domain, it is appended to the URL. Otherwise, the
    /// default HTTPS port is assumed.</remarks>
    /// <param name="domain">The domain name for which to generate the URL. Cannot be null or empty.</param>
    /// <returns>A string containing the full HTTPS URL for the specified domain, including the port number if available.</returns>
    public string GetCompleteUrl(string domain) => "https://" + domain + "." + Domain + (Ports != null && Ports.ContainsKey(domain) ? ":" + Ports[domain] : string.Empty);

    /// <summary>
    /// Constructs a secure URL using the specified domain and optional port number.
    /// </summary>
    /// <param name="domain">The subdomain or domain name to include in the URL. Cannot be null or empty.</param>
    /// <param name="port">The port number to append to the URL. If null, the default HTTPS port is used.</param>
    /// <returns>A string containing the constructed HTTPS URL with the specified domain and port.</returns>
    public string GetUrl(string domain, int? port = null) => "https://" + domain + "." + Domain + (port is not null ? ":" + port : string.Empty);

    /// <summary>
    /// Returns the complete URL for the specified dashboard instance.
    /// </summary>
    /// <param name="name">The name of the dashboard instance. If null or omitted, defaults to "devdash".</param>
    /// <param name="port">The port number to use for the dashboard URL. If null, the default port is used.</param>
    /// <returns>A string containing the full URL of the dashboard instance.</returns>
    public string GetDashboardUrl(string? name = "devdash", int? port = null) => GetCompleteUrl(name ?? "devdash");

    /// <summary>
    /// Returns an enumerable collection of fully qualified host names constructed from the configured domain entries.
    /// </summary>
    /// <remarks>Each host name is generated by appending the configured domain suffix to each entry in the
    /// domain list. The returned collection is evaluated lazily; host names are generated as the collection is
    /// enumerated.</remarks>
    /// <returns>An <see cref="IEnumerable{String}"/> containing the full host names for each entry in the domain list. The
    /// collection will be empty if no domains are configured.</returns>
    public IEnumerable<string> GetAllHosts()
    {
        foreach (var entry in Domains)
        {
            yield return $"{entry}.{Domain}";
        }
    }

    /// <summary>
    /// Generates a sequence of hosts file entry strings for all known hosts using the specified IPv4 address.
    /// </summary>
    /// <param name="ipv4">The IPv4 address to associate with each host entry. If null or not specified, defaults to "127.0.0.1".</param>
    /// <returns>An enumerable collection of strings, each representing a hosts file entry in the format "<IPv4> <host>" for
    /// every host returned by GetAllHosts().</returns>
    public IEnumerable<string> GetHostsFileEntries(string? ipv4 = "127.0.0.1")
    {
        foreach (var host in GetAllHosts())
        {
            yield return $"{ipv4} {host}";
        }
    }

    /// <summary>
    /// Gets the file system path to the certificate. Throws an exception if the certificate has not been initialized.
    /// </summary>
    /// <returns>The file system path to the certificate.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificate has not been initialized. Call EnsureSetup before accessing this property.</exception>
    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    /// <summary>
    /// Returns the file system path to the certificate key if the certificates have been initialized.
    /// </summary>
    /// <returns>The file system path to the certificate key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificates have not been initialized. Call EnsureSetup before invoking this method.</exception>
    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");

    public ReadOnlySpan<char> ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this).AsSpan();
    }

    public static Result<DnsConfiguration> FromJson(string json)
    {
        return Result<DnsConfiguration>.TryCatch<DnsConfiguration>(() => System.Text.Json.JsonSerializer.Deserialize<DnsConfiguration>(json)!.ToSuccess<DnsConfiguration>());
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

    public string DomainOrDie() => Domain ?? throw new InvalidOperationException("Domain is not set in the configuration.");
}

/// <summary>
/// Defines methods for configuring and building a development application host, including certificate setup, host file
/// configuration, launch settings, and project resource integration.
/// </summary>
/// <remarks>Implementations of this interface provide a fluent API for preparing a development environment for
/// distributed applications. Methods allow for incremental setup of certificates, host entries, launch settings, and
/// project resources before building the final application model. This interface is typically used in scenarios where
/// automated or programmatic setup of development infrastructure is required.</remarks>
public interface IDevAppHost
{
    /// <summary>
    /// Ensures that the development application host is properly set up and ready for use. Optionally forces
    /// regeneration of the development certificate.
    /// </summary>
    /// <param name="forceCertificateRegeneration">If <see langword="true"/>, forces the regeneration of the development certificate even if one already exists; if
    /// <see langword="false"/>, uses the existing certificate if available. If <see langword="null"/>, the default
    /// behavior is applied.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance representing the configured development application host.</returns>
    IDevAppHost EnsureSetup(bool? forceCertificateRegeneration = false);

    /// <summary>
    /// Ensures that launch settings are configured for the specified application and returns a host instance
    /// representing the setup.
    /// </summary>
    /// <param name="name">The name of the application for which to configure launch settings. Cannot be null or empty.</param>
    /// <param name="launchSettingsFilePath">The file path to the launch settings configuration file. Must refer to an existing file; cannot be null or
    /// empty.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance representing the application with launch settings configured.</returns>
    IDevAppHost EnsureLaunchSettingsSetup(string name, string launchSettingsFilePath);

    /// <summary>
    /// Creates a new application host instance configured with a project resource built using the specified resource
    /// builder and name.
    /// </summary>
    /// <param name="resourceBuilder">A delegate that receives an <see cref="IDistributedApplicationBuilder"/> and returns a resource builder for the
    /// project resource to be added. Cannot be null.</param>
    /// <param name="name">The name to assign to the project resource. Must be non-empty and unique within the application host.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance that includes the configured project resource.</returns>
    IDevAppHost WithProjectInstance(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name);

    /// <summary>
    /// Builds and returns a configured distributed application instance based on the current settings.
    /// </summary>
    /// <returns>A <see cref="DistributedApplication"/> representing the finalized distributed application configuration.</returns>
    DistributedApplication Build();
}

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
        EnsureLaunchSettingsSetup("devdash", DevAppHostLaunchSettingsPath());
        EnsureHostsSetup();

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
    /// Ensures that the launch settings file is configured with the required environment variables for the specified
    /// application host.
    /// </summary>
    /// <remarks>This method updates the specified launch settings file with environment variables necessary
    /// for ASP.NET Core and related services. The caller must ensure that the provided file path is accessible and that
    /// the configuration contains the required values.</remarks>
    /// <param name="name">The name of the application host for which to set up launch settings. This value is used to determine
    /// environment variable values and endpoint configuration.</param>
    /// <param name="launchSettingsFilePath">The file path to the launch settings JSON file to be updated. Must refer to a valid file location.</param>
    /// <returns>An <see cref="IDevAppHost"/> instance representing the configured application host.</returns>
    /// <exception cref="InvalidDataException">Thrown if the required 'LaunchSettingsProfile' configuration value is missing.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the required 'Environment' configuration value is missing.</exception>
    public IDevAppHost EnsureLaunchSettingsSetup(string name, string launchSettingsFilePath)
    {
        LaunchSettingsHelper.AddOrUpdateEnvironmentVariables(launchSettingsFilePath,
            DevAppHostConfiguration["LaunchSettingsProfile"] ?? throw new InvalidDataException("missing LaunchSettingsProfile"),
            new Dictionary<string, string>
            {
                { "ASPNETCORE_Kestrel__Certificates__Default__Path", DnsConfiguration.CertPathOrDie() },
                { "ASPNETCORE_Kestrel__Certificates__Default__KeyPath", DnsConfiguration.KeyPathOrDie() },
                { "ASPNETCORE_URLS", DnsConfiguration.GetAspNetCoreUrl(name) },
                { "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", DnsConfiguration.GetUrl(name, DnsConfiguration.Ports[$"{name}-oltp"]) },
                { "ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL", DnsConfiguration.GetUrl(name, DnsConfiguration.Ports[$"{name}-services"]) },
                { "ASPIRE_ENVIRONMENT", DevAppHostConfiguration["Environment"] ?? throw new InvalidOperationException() },
                { "DOTNET_ENVIRONMENT", DevAppHostConfiguration["Environment"]?? throw new InvalidOperationException() },
                { "CustomDomain__Fqdn", DnsConfiguration.GetUrl(name) },
                { "CustomDomain__Port", DnsConfiguration.Ports[name].ToString() }
            });

        return this;
    }

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

    public IDevAppHost EnsureSetup(DnsConfiguration DnsConfiguration, bool? forceCertificateRegeneration = false)
    {
        // Check if running with elevated privileges on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsRunningAsAdministrator())
        {
            throw new InvalidOperationException("need admin rights");
        }

        if (NeedsHostsFileUpdate(DnsConfiguration))
        {
            UpdateHostsFile(DnsConfiguration);
        }

        // Ensure mkcert is installed and certificates are generated
        EnsureCertSetup(forceCertificateRegeneration);

        return this;

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

    public string GetCertificatePath()
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
        return Path.Combine(certsDir, $"{DnsConfiguration.Domain}.pem");
    }

    public string GetKeyPath()
    {
        var certsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DnsConfiguration.CertificatesDirectory);
        return Path.Combine(certsDir, $"{DnsConfiguration.Domain}-key.pem");
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

    public IDevAppHost WithProjectInstance(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name)
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

public static class LaunchSettingsHelper
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
