namespace FrenchExDev.Net.Aspire.DevAppHost;

/// <summary>
/// Defines methods for managing development application host setup, certificate management, DNS configuration, and
/// administrative operations required for local development environments.
/// </summary>
/// <remarks>The IDevAppHost interface provides a set of operations to automate and manage the setup of local
/// development environments, including handling SSL certificate generation, DNS configuration, and hosts file updates.
/// Implementations of this interface are responsible for ensuring that the necessary prerequisites for secure and
/// reliable local development are met, and for providing utilities to check and update the environment as needed.
/// Methods may require elevated permissions depending on the operation (such as updating the hosts file or restarting
/// with administrative rights).</remarks>
public interface IDevAppHost
{
    /// <summary>
    /// Ensures that mkcert is properly set up for the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration for which mkcert should be set up. Cannot be null.</param>
    /// <param name="force">If set to <see langword="true"/>, forces reinstallation or reconfiguration of mkcert even if it is already set
    /// up; if <see langword="false"/>, skips setup if mkcert is already configured. If null, the default behavior is to
    /// force setup.</param>
    void EnsureMkcertSetup(DnsConfiguration config, bool? force = true);

    /// <summary>
    /// Ensures that the DNS and certificate setup is complete and up to date according to the specified configuration.
    /// </summary>
    /// <param name="dnsConfig">The DNS configuration to apply. Specifies the desired DNS records and related settings required for setup.</param>
    /// <param name="forceCertificateRegeneration">true to force regeneration of certificates even if they are already valid; otherwise, false to only generate
    /// certificates if necessary.</param>
    void EnsureSetup(DnsConfiguration dnsConfig, bool? forceCertificateRegeneration = false);

    /// <summary>
    /// Retrieves the file system path to the certificate associated with the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration for which to obtain the certificate path. Cannot be null.</param>
    /// <returns>A string containing the full file system path to the certificate. Returns null if no certificate is associated
    /// with the provided configuration.</returns>
    string GetCertificatePath(DnsConfiguration config);

    /// <summary>
    /// Retrieves the key path associated with the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration for which to obtain the key path. Cannot be null.</param>
    /// <returns>A string representing the key path for the provided DNS configuration.</returns>
    string GetKeyPath(DnsConfiguration config);

    /// <summary>
    /// Gets the file path to the configuration file associated with the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration for which to retrieve the configuration file path. Cannot be null.</param>
    /// <returns>The full file path to the configuration file for the specified DNS configuration.</returns>
    string GetConfigurationFilePath(DnsConfiguration config);

    /// <summary>
    /// Determines whether the current process is running with administrator privileges.
    /// </summary>
    /// <remarks>This method can be used to check if elevated permissions are available before performing
    /// operations that require administrative rights. The result may vary depending on the operating system and user
    /// account control (UAC) settings.</remarks>
    /// <returns>true if the process is running as an administrator; otherwise, false.</returns>
    bool IsRunningAsAdministrator();

    /// <summary>
    /// Determines whether the hosts file requires an update based on the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration to evaluate for potential changes that may necessitate updating the hosts file. Cannot be
    /// null.</param>
    /// <returns>true if the hosts file needs to be updated to reflect the provided DNS configuration; otherwise, false.</returns>
    bool NeedsHostsFileUpdate(DnsConfiguration config);

    /// <summary>
    /// Determines whether the current certificate must be regenerated based on the specified DNS configuration.
    /// </summary>
    /// <param name="config">The DNS configuration to evaluate for changes that may require certificate regeneration. Cannot be null.</param>
    /// <returns>true if certificate regeneration is required due to changes in the DNS configuration; otherwise, false.</returns>
    bool NeedsCertificateRegeneration(DnsConfiguration config);

    /// <summary>
    /// Restarts the current application with elevated administrator privileges.
    /// </summary>
    /// <remarks>Use this method when the application requires administrator rights to perform certain
    /// operations. The current process will typically exit after initiating the restart. The method may return false if
    /// the restart cannot be performed, such as when the user declines the elevation prompt or if the operating system
    /// does not support elevation.</remarks>
    /// <returns>true if the restart process was successfully initiated; otherwise, false.</returns>
    bool RestartAsAdministrator();

    /// <summary>
    /// Saves the specified DNS configuration settings.
    /// </summary>
    /// <param name="config">The DNS configuration to be saved. Cannot be null.</param>
    void SaveConfiguration(DnsConfiguration config);

    /// <summary>
    /// Loads the previously saved DNS configuration that matches the specified configuration parameters.
    /// </summary>
    /// <param name="config">The DNS configuration parameters to use when searching for a saved configuration. Cannot be null.</param>
    /// <returns>A <see cref="DnsConfiguration"/> object representing the saved configuration if found; otherwise, <see
    /// langword="null"/>.</returns>
    DnsConfiguration? LoadSavedConfiguration(DnsConfiguration config);

    /// <summary>
    /// Updates the system hosts file using the specified DNS configuration.
    /// </summary>
    /// <remarks>This method modifies the system hosts file, which may require elevated permissions. Changes
    /// take effect immediately and may impact network name resolution on the system.</remarks>
    /// <param name="config">The DNS configuration to apply to the hosts file. Cannot be null.</param>
    void UpdateHostsFile(DnsConfiguration config);
}

