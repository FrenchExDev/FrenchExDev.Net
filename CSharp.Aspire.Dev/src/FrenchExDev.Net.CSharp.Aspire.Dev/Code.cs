using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.Aspire.Dev;

public record DnsConfiguration2
{
    public string Domain { get; set; }

    public string GetInstanceUrl(string appName, int instance)
    {
        return $"https://{appName}-instance{instance}.{Domain}";
    }
}

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
    /// Gets or sets the name of the dashboard associated with this instance.
    /// </summary>
    public string DashboardAppName { get; set; }

    /// <summary>
    /// Gets or sets the password used to access the SSL certificate for secure connections.
    /// </summary>
    /// <remarks>If the SSL certificate is password-protected, this property must be set to the correct
    /// password before establishing a secure connection. The password should be stored and handled securely to prevent
    /// unauthorized access.</remarks>
    public string SslPassword { get; set; }

    /// <summary>
    /// Gets or sets the name of the SSL certificate generator to use for secure connections.
    /// </summary>
    public string SslGenerator { get; set; }

    /// <summary>
    /// Gets or sets the directory path where certificate files are stored.
    /// </summary>
    public string CertificatesDirectory { get; set; }

    /// <summary>
    /// Gets or sets the file system path to the certificate used for authentication.
    /// </summary>
    public string CertPath { get; set; }

    /// <summary>
    /// Gets or sets the file system path to the private key associated with the certificate.
    /// </summary>
    public string KeyPath { get; set; }

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
    public string Domain { get; set; }

    /// <summary>
    /// Gets the list of domain names associated with the current instance.
    /// </summary>
    public string[] Domains { get; set; }

    /// <summary>
    /// Gets the collection of named ports and their corresponding port numbers for the current instance.
    /// </summary>
    /// <remarks>The dictionary maps port names to their assigned integer values. Modifications to this
    /// collection affect the available ports for the instance. This property is protected set; it can only be modified
    /// within derived classes.</remarks>
    public Dictionary<string, int> Ports { get; set; }

    /// <summary>
    /// Gets or sets the collection of ASP.NET Core endpoint URLs mapped by their names.
    /// </summary>
    /// <remarks>Each entry in the dictionary represents a named endpoint and its corresponding URL. This
    /// property can be used to configure or retrieve the URLs for different ASP.NET Core services within the
    /// application.</remarks>
    public Dictionary<string, string> AspNetCoreUrls { get; set; }

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
    /// Ensures that the specified launch settings are configured for the given application and returns a host instance
    /// representing the setup.
    /// </summary>
    /// <param name="appName">The name of the application for which to configure launch settings. Cannot be null or empty.</param>
    /// <param name="launchSettingName">The name of the launch setting to apply. Must correspond to a valid setting in the launch settings file.</param>
    /// <param name="launchSettingsFilePath">The full path to the launch settings file. Must be a valid file path and the file must exist.</param>
    /// <returns>An instance of <see cref="IDevAppHost"/> representing the application host configured with the specified launch
    /// settings.</returns>
    IDevAppHost EnsureLaunchSettingsSetup(string appName, string launchSettingName, string launchSettingsFilePath);

    /// <summary>
    /// Adds a project resource to the distributed application host using the specified resource builder and
    /// configuration.
    /// </summary>
    /// <remarks>Use this method to programmatically add and configure a project resource as part of the
    /// distributed application. The resource name must be unique within the application to avoid conflicts.</remarks>
    /// <param name="resourceBuilder">A delegate that creates a resource builder for the project resource, using the provided distributed application
    /// builder.</param>
    /// <param name="name">The unique name to assign to the project resource within the application. Cannot be null or empty.</param>
    /// <param name="configuration">An optional delegate to further configure the project resource builder before the resource is added.</param>
    /// <returns>An updated distributed application host instance that includes the configured project resource.</returns>
    IDevAppHost WithProjectInstance(Func<IDistributedApplicationBuilder, IResourceBuilder<ProjectResource>> resourceBuilder, string name, Action<IResourceBuilder<ProjectResource>>? configuration = null, int? replicas = 1);

    /// <summary>
    /// Builds and returns a configured distributed application instance based on the current settings.
    /// </summary>
    /// <returns>A <see cref="DistributedApplication"/> representing the finalized distributed application configuration.</returns>
    DistributedApplication Build();
}
