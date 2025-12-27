#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Represents the configuration settings for a single SSH host as defined in an SSH configuration file.
/// </summary>
/// <remarks>
/// Use this class to model and manipulate SSH host entries programmatically, such as when parsing or generating SSH configuration files.
/// Each property corresponds to a supported SSH configuration directive and may be null if not specified for the host.
/// This type does not perform validation or apply defaults; callers are responsible for ensuring property values are valid for their intended SSH client or server.
/// Thread safety is not guaranteed; synchronize access if instances are shared across threads.
///
/// Example:
/// <code>
/// var host = new SshConfigHost {
///     Name = "myhost",
///     HostName = "example.com",
///     Port = 22,
///     User = "alice",
///     Compression = true,
///     IdentityFile = "~/.ssh/id_rsa"
/// };
/// </code>
/// </remarks>
public class SshConfigHost
{
    /// <summary>
    /// The logical name of the SSH host entry (required).
    /// </summary>
    /// <remarks>
    /// This is the identifier used in SSH config files. Must be unique per host entry.
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    /// Specifies the address family (IPv4, IPv6, or both) for the host.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AddressFamilyEnum.Inet"/> for IPv4, <see cref="AddressFamilyEnum.Inet6"/> for IPv6, or <see cref="AddressFamilyEnum.All"/> for both.
    /// </remarks>
    public AddressFamilyEnum? AddressFamily { get; init; }

    /// <summary>
    /// Enables or disables batch mode ("yes" or "no").
    /// </summary>
    /// <remarks>
    /// When enabled, disables password/passphrase querying.
    /// </remarks>
    public string? BatchMode { get; init; }

    /// <summary>
    /// Specifies the local address to bind for outgoing connections.
    /// </summary>
    public string? BindAddress { get; init; }

    /// <summary>
    /// Enables or disables challenge-response authentication.
    /// </summary>
    public string? ChallengeResponseAuthentication { get; init; }

    /// <summary>
    /// Enables or disables host IP checking.
    /// </summary>
    public string? CheckHostIp { get; init; }

    /// <summary>
    /// Specifies the cipher to use for the connection.
    /// </summary>
    public string? Cipher { get; init; }

    /// <summary>
    /// Specifies a list of allowed ciphers.
    /// </summary>
    public List<string>? Ciphers { get; init; }

    /// <summary>
    /// Clears all forwarding rules for the host.
    /// </summary>
    public bool? ClearAllForwardings { get; init; }

    /// <summary>
    /// Enables or disables compression for the connection.
    /// </summary>
    public bool? Compression { get; init; }

    /// <summary>
    /// Sets the compression level (if compression is enabled).
    /// </summary>
    public int? CompressionLevel { get; init; }

    /// <summary>
    /// Specifies the number of connection attempts before giving up.
    /// </summary>
    public int? ConnectionAttempts { get; init; }

    /// <summary>
    /// Sets the connection timeout in seconds.
    /// </summary>
    public int? ConnectTimeout { get; init; }

    /// <summary>
    /// Controls master connection behavior for multiplexing SSH sessions.
    /// </summary>
    /// <remarks>
    /// See <see cref="ControlMasterEnum"/> for available options.
    /// </remarks>
    public ControlMasterEnum? ControlMaster { get; init; }

    /// <summary>
    /// Specifies the path for the control socket used in master connections.
    /// </summary>
    public string? ControlPath { get; init; }

    /// <summary>
    /// Specifies a dynamic port forwarding rule.
    /// </summary>
    public string? DynamicForward { get; init; }

    /// <summary>
    /// Enables SSH key signing for host authentication.
    /// </summary>
    public bool? EnableSshKeysign { get; init; }

    /// <summary>
    /// Sets the escape character for SSH sessions.
    /// </summary>
    public string? EscapeChar { get; init; }

    /// <summary>
    /// Exits if a forwarding rule fails.
    /// </summary>
    public bool? ExitOnForwardFailure { get; init; }

    /// <summary>
    /// Enables or disables forwarding of the authentication agent.
    /// </summary>
    public bool? ForwardAgent { get; init; }

    /// <summary>
    /// Enables or disables X11 forwarding.
    /// </summary>
    public bool? ForwardX11 { get; init; }

    /// <summary>
    /// Enables trusted X11 forwarding.
    /// </summary>
    public bool? ForwardX11Trusted { get; init; }

    /// <summary>
    /// Controls gateway ports for remote forwarding.
    /// </summary>
    public string? GatewayPorts { get; init; }

    /// <summary>
    /// Specifies the global known hosts file path.
    /// </summary>
    public string? GlobalKnownHostsFile { get; init; }

    /// <summary>
    /// Enables or disables GSSAPI authentication.
    /// </summary>
    public bool? GssApiAuthentication { get; init; }

    /// <summary>
    /// Enables or disables GSSAPI key exchange.
    /// </summary>
    public bool? GssApiKeyExchange { get; init; }

    /// <summary>
    /// Specifies the GSSAPI client identity.
    /// </summary>
    public string? GssApiClientIdentity { get; init; }

    /// <summary>
    /// Enables delegation of GSSAPI credentials.
    /// </summary>
    public bool? GssApiDelegateCredentials { get; init; }

    /// <summary>
    /// Forces rekeying on GSSAPI renewal.
    /// </summary>
    public bool? GssApiRenewalForcesRekey { get; init; }

    /// <summary>
    /// Enables trust of DNS for GSSAPI.
    /// </summary>
    public bool? GssApiTrustDns { get; init; }

    /// <summary>
    /// Enables hashing of known hosts files.
    /// </summary>
    public bool? HashKnownHosts { get; init; }

    /// <summary>
    /// Enables host-based authentication.
    /// </summary>
    public bool? HostbasedAuthentication { get; init; }

    /// <summary>
    /// Specifies allowed host key algorithms.
    /// </summary>
    public string? HostKeyAlgorithms { get; init; }

    /// <summary>
    /// Specifies an alias for the host key.
    /// </summary>
    public string? HostKeyAlias { get; init; }

    /// <summary>
    /// The actual hostname or IP address to connect to.
    /// </summary>
    /// <remarks>
    /// This is the remote address used for the SSH connection.
    /// </remarks>
    public string? HostName { get; init; }

    /// <summary>
    /// Restricts authentication to the specified identity files only.
    /// </summary>
    public bool? IdentitiesOnly { get; init; }

    /// <summary>
    /// Specifies the path to the identity file for authentication.
    /// </summary>
    public string? IdentityFile { get; init; }

    /// <summary>
    /// Enables keyboard-interactive authentication.
    /// </summary>
    public bool? KbdInteractiveAuthentication { get; init; }

    /// <summary>
    /// Specifies allowed keyboard-interactive devices.
    /// </summary>
    public string? KbdInteractiveDevices { get; init; }

    /// <summary>
    /// Specifies a local command to execute after connecting.
    /// </summary>
    public string? LocalCommand { get; init; }

    /// <summary>
    /// Specifies a local port forwarding rule.
    /// </summary>
    public string? LocalForward { get; init; }

    /// <summary>
    /// Sets the logging level for the SSH session.
    /// </summary>
    public string? LogLevel { get; init; }

    /// <summary>
    /// Specifies allowed MAC algorithms.
    /// </summary>
    public string? Macs { get; init; }

    /// <summary>
    /// Disables host authentication for localhost.
    /// </summary>
    public bool? NoHostAuthenticationForLocalhost { get; init; }

    /// <summary>
    /// Sets the number of password prompts before giving up.
    /// </summary>
    public int? NumberOfPasswordPrompts { get; init; }

    /// <summary>
    /// Enables or disables password authentication.
    /// </summary>
    public bool? PasswordAuthentication { get; init; }

    /// <summary>
    /// Enables or disables the local command permission.
    /// </summary>
    public bool? PermitLocalCommand { get; init; }

    /// <summary>
    /// Specifies the port to connect to on the remote host.
    /// </summary>
    /// <remarks>
    /// Default SSH port is 22.
    /// </remarks>
    public int? Port { get; init; }

    /// <summary>
    /// Specifies preferred authentication methods in order of preference.
    /// </summary>
    public List<string>? PreferredAuthentications { get; init; }

    /// <summary>
    /// Specifies the SSH protocol version to use.
    /// </summary>
    public int? Protocol { get; init; }

    /// <summary>
    /// Specifies a proxy command to use for the connection.
    /// </summary>
    public string? ProxyCommand { get; init; }

    /// <summary>
    /// Enables or disables public key authentication.
    /// </summary>
    public bool? PubkeyAuthentication { get; init; }

    /// <summary>
    /// Specifies the rekey limit for the session.
    /// </summary>
    public string? RekeyLimit { get; init; }

    /// <summary>
    /// Specifies a forwarding rule to remove.
    /// </summary>
    public string? RemoveForward { get; init; }

    /// <summary>
    /// Enables RhostsRSA authentication.
    /// </summary>
    public bool? RhostsRsaAuthentication { get; init; }

    /// <summary>
    /// Enables RSA authentication.
    /// </summary>
    public bool? RsaAuthentication { get; init; }

    /// <summary>
    /// Enables sending environment variables to the remote host.
    /// </summary>
    public bool? SendEnv { get; init; }

    /// <summary>
    /// Sets the maximum number of server alive messages.
    /// </summary>
    public int? ServerAliveCountMax { get; init; }

    /// <summary>
    /// Sets the interval (in seconds) for server alive messages.
    /// </summary>
    public int? ServerAliveInterval { get; init; }

    /// <summary>
    /// Specifies the smartcard device for authentication.
    /// </summary>
    public string? SmartcardDevice { get; init; }

    /// <summary>
    /// Controls strict host key checking behavior.
    /// </summary>
    /// <remarks>
    /// See <see cref="YesNoAskEnum"/> for available options.
    /// </remarks>
    public YesNoAskEnum? StrictHostKeyChecking { get; init; }

    /// <summary>
    /// Enables TCP keep-alive messages.
    /// </summary>
    public bool? TcpKeeAlive { get; init; }

    /// <summary>
    /// Controls tunneling behavior for the connection.
    /// </summary>
    /// <remarks>
    /// See <see cref="TunnelEnum"/> for available options.
    /// </remarks>
    public TunnelEnum? Tunnel { get; init; }

    /// <summary>
    /// Specifies the tunnel device for the connection.
    /// </summary>
    public string? TunnelDevice { get; init; }

    /// <summary>
    /// Enables use of a privileged port for the connection.
    /// </summary>
    public bool? UsePrivilegedPort { get; init; }

    /// <summary>
    /// Specifies the username for authentication.
    /// </summary>
    public string? User { get; init; }

    /// <summary>
    /// Specifies the path to the user's known hosts file.
    /// </summary>
    public string? UserKnownHostsFile { get; init; }

    /// <summary>
    /// Enables verification of host keys via DNS.
    /// </summary>
    public bool? VerifyHostKeyDns { get; init; }

    /// <summary>
    /// Enables visual host key display.
    /// </summary>
    public bool? VisualHostKey { get; init; }

    /// <summary>
    /// Specifies the location of the XAuth binary for X11 forwarding.
    /// </summary>
    public string? XAuthLocation { get; init; }
}