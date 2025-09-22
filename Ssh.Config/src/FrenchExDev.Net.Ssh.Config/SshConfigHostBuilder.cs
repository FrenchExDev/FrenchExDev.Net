#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;


#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Builder for <see cref="SshConfigHost"/> objects, providing a fluent API to configure SSH host options.
/// </summary>
/// <remarks>
/// This builder supports all major SSH configuration options, including authentication, forwarding, connection
/// settings, and advanced features. It validates required fields and aggregates errors for robust configuration.
///
/// Example usage:
/// <code>
/// var host = new SshConfigHostBuilder()
///     .Name("myhost")
///     .HostName("example.com")
///     .Port(22)
///     .User("alice")
///     .Compression()
///     .ForwardAgent()
///     .IdentityFile("~/.ssh/id_rsa")
///     .Build().Success();
/// </code>
///
/// Remarks:
/// - All methods are chainable for fluent configuration.
/// - Validation is performed during build; missing required fields (e.g., Name) will result in a failure result.
/// - Optional fields can be omitted; only set what is needed for your SSH scenario.
/// - Use <c>Build()</c> to produce the final <see cref="SshConfigHost"/> or inspect errors.
/// </remarks>
public class SshConfigHostBuilder : AbstractBuilder<SshConfigHost>
{
    /// <summary>Stores the logical SSH host name.</summary>
    private string? _name;
    /// <summary>Stores the address family (IPv4, IPv6, or both).</summary>
    private AddressFamilyEnum? _addressFamily;
    /// <summary>Stores the batch mode option.</summary>
    private string? _batchMode;
    /// <summary>Stores the bind address for outgoing connections.</summary>
    private string? _bindAddress;
    /// <summary>Stores the challenge-response authentication option.</summary>
    private string? _challengeResponseAuthentication;
    /// <summary>Stores the host IP checking option.</summary>
    private string? _checkHostIp;
    /// <summary>Stores the encryption cipher name.</summary>
    private string? _cipher;
    /// <summary>Stores the list of supported ciphers.</summary>
    private List<string>? _ciphers;
    /// <summary>Indicates whether to clear all port forwardings.</summary>
    private bool? _clearAllForwardings;
    /// <summary>Indicates whether compression is enabled.</summary>
    private bool? _compression;
    /// <summary>Stores the compression level.</summary>
    private int? _compressionLevel;
    /// <summary>Stores the number of connection attempts.</summary>
    private int? _connectionAttempts;
    /// <summary>Stores the connection timeout in seconds.</summary>
    private int? _connectionTimeout;
    /// <summary>Stores the control master mode for connection sharing.</summary>
    private ControlMasterEnum? _controlMaster;
    /// <summary>Stores the control socket path for master connections.</summary>
    private string? _controlPath;
    /// <summary>Stores the dynamic forward specification for SOCKS proxying.</summary>
    private string? _dynamicForward;
    /// <summary>Indicates whether SSH key signing is enabled.</summary>
    private bool? _enableSshKeysign;
    /// <summary>Stores the escape character for SSH protocol automation.</summary>
    private string? _escapeChar;
    /// <summary>Indicates whether to exit on forward failure.</summary>
    private bool? _exitOnForwardFailure;
    /// <summary>Indicates whether SSH agent forwarding is enabled.</summary>
    private bool? _forwardAgent;
    /// <summary>Indicates whether X11 forwarding is enabled.</summary>
    private bool? _forwardX11;
    /// <summary>Indicates whether trusted X11 forwarding is enabled.</summary>
    private bool? _forwardX11Trusted;
    /// <summary>Stores the gateway ports specification for remote forwarding.</summary>
    private string? _gatewayPorts;
    /// <summary>Stores the global known hosts file path.</summary>
    private string? _globalKnownHostsFile;
    /// <summary>Indicates whether GSSAPI authentication is enabled.</summary>
    private bool? _gssApiAuthentication;
    /// <summary>Indicates whether GSSAPI key exchange is enabled.</summary>
    private bool? _gssApiKeyExchange;
    /// <summary>Stores the GSSAPI client identity.</summary>
    private string? _gssApiClientIdentity;
    /// <summary>Indicates whether GSSAPI delegate credentials are enabled.</summary>
    private bool? _gssApiDelegateCredentials;
    /// <summary>Indicates whether GSSAPI renewal forces rekeying.</summary>
    private bool? _gssApiRenewalForcesRekey;
    /// <summary>Indicates whether GSSAPI trust DNS is enabled.</summary>
    private bool? _gssApiTrustDns;
    /// <summary>Indicates whether known hosts hashing is enabled.</summary>
    private bool? _hashKnownHosts;
    /// <summary>Indicates whether host-based authentication is enabled.</summary>
    private bool? _hostbasedAuthentication;
    /// <summary>Stores the host key algorithms for authentication.</summary>
    private string? _hostKeyAlgorithms;
    /// <summary>Stores the host key alias.</summary>
    private string? _hostKeyAlias;
    /// <summary>Stores the real host name or IP address.</summary>
    private string? _hostName;
    /// <summary>Indicates whether only specified identity files are used for authentication.</summary>
    private bool? _identitiesOnly;
    /// <summary>Stores the primary identity file path.</summary>
    private string? _identityFile;
    /// <summary>Indicates whether keyboard-interactive authentication is enabled.</summary>
    private bool? _kbdInteractiveAuthentication;
    /// <summary>Stores the devices for keyboard-interactive authentication.</summary>
    private string? _kbdInteractiveDevices;
    /// <summary>Stores the local command to execute after authentication.</summary>
    private string? _localCommand;
    /// <summary>Stores the local port forwarding specification.</summary>
    private string? _localForward;
    /// <summary>Stores the logging level for SSH messages.</summary>
    private string? _logLevel;
    /// <summary>Stores the MAC algorithms for data integrity.</summary>
    private string? _macs;
    /// <summary>Indicates whether host authentication for localhost is disabled.</summary>
    private bool? _noHostAuthenticationForLocalhost;
    /// <summary>Stores the number of password prompts for authentication.</summary>
    private int? _numberOfPasswordPrompts;
    /// <summary>Indicates whether password authentication is enabled.</summary>
    private bool? _passwordAuthentication;
    /// <summary>Indicates whether local command execution is permitted after login.</summary>
    private bool? _permitLocalCommand;
    /// <summary>Stores the port number for the SSH connection.</summary>
    private int? _port;
    /// <summary>Stores the preferred authentication methods.</summary>
    private List<string>? _preferredAuthentication;
    /// <summary>Stores the SSH protocol version.</summary>
    private int? _protocol;
    /// <summary>Stores the proxy command for connecting through a proxy.</summary>
    private string? _proxyCommand;
    /// <summary>Indicates whether public key authentication is enabled.</summary>
    private bool? _pubkeyAuthentication;
    /// <summary>Stores the rekey limit for the connection.</summary>
    private string? _rekeyLimit;
    /// <summary>Stores the value for removing specific port forwardings.</summary>
    private string? _removeForward;
    /// <summary>Indicates whether rhosts RSA authentication is enabled.</summary>
    private bool? _rHostsRsaAuthentication;
    /// <summary>Indicates whether RSA authentication is enabled.</summary>
    private bool? _rsaAuthentication;
    /// <summary>Indicates whether sending environment variables is enabled.</summary>
    private bool? _sendEnv;
    /// <summary>Stores the maximum number of server alive messages.</summary>
    private int? _serverAliveCountMax;
    /// <summary>Stores the interval in seconds between server alive messages.</summary>
    private int? _serverAliveInterval;
    /// <summary>Stores the smartcard device for authentication.</summary>
    private string? _smartcardDevice;
    /// <summary>Stores the strict host key checking level.</summary>
    private YesNoAskEnum? _strictHostKeyChecking;
    /// <summary>Indicates whether TCP keepalive messages are enabled.</summary>
    private bool? _tcpKeepAlive;
    /// <summary>Stores the tunneling mode for the connection.</summary>
    private TunnelEnum? _tunnel;
    /// <summary>Stores the tunnel device specification.</summary>
    private string? _tunnelDevice;
    /// <summary>Indicates whether privileged ports are used for the connection.</summary>
    private bool? _usePrivilegedPort;
    /// <summary>Stores the user name for authentication.</summary>
    private string? _user;
    /// <summary>Stores the user known hosts file path.</summary>
    private string? _userKnownHostsFile;
    /// <summary>Indicates whether host key DNS verification is enabled.</summary>
    private bool? _verifyHostKeyDns;
    /// <summary>Indicates whether visual host key authentication is enabled.</summary>
    private bool? _visualHostKey;
    /// <summary>Stores the X11 authentication location.</summary>
    private string? _xAuthLocation;

    /// <summary>
    /// Sets the logical SSH host name (required).
    /// </summary>
    /// <param name="name">The host name (e.g., "myhost").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is the identifier used in SSH config files. Must be unique per host entry.
    /// </remarks>
    public SshConfigHostBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the address family for the host (IPv4, IPv6, or both).
    /// </summary>
    /// <param name="addressFamily">The address family enum value.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Use <see cref="AddressFamilyEnum.Inet"/> for IPv4, <see cref="AddressFamilyEnum.Inet6"/> for IPv6, or <see cref="AddressFamilyEnum.All"/> for both.
    /// </remarks>
    public SshConfigHostBuilder AddressFamily(AddressFamilyEnum? addressFamily)
    {
        _addressFamily = addressFamily;
        return this;
    }

    /// <summary>
    /// Sets the batch mode option ("yes" or "no").
    /// </summary>
    /// <param name="batchMode">Batch mode value.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, disables password/passphrase querying.
    /// </remarks>
    public SshConfigHostBuilder BatchMode(string? batchMode)
    {
        _batchMode = batchMode;
        return this;
    }

    /// <summary>
    /// Sets the bind address for the host.
    /// </summary>
    /// <param name="bindAddress">The bind address (e.g., "192.168.1.1").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the local address to bind to for outgoing connections.
    /// </remarks>
    public SshConfigHostBuilder BindAddress(string? bindAddress)
    {
        _bindAddress = bindAddress;
        return this;
    }

    /// <summary>
    /// Enables or disables challenge-response authentication.
    /// </summary>
    /// <param name="challengeResponseAuthentication">The challenge-response authentication option ("yes" or "no").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is typically used for keyboard-interactive authentication methods.
    /// </remarks>
    public SshConfigHostBuilder ChallengeResponseAuthentication(string? challengeResponseAuthentication)
    {
        _challengeResponseAuthentication = challengeResponseAuthentication;
        return this;
    }

    /// <summary>
    /// Enables or disables host IP address checking.
    /// </summary>
    /// <param name="checkHostIp">The host IP checking option ("yes" or "no").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// If enabled, SSH will verify that the server's IP address matches the one in the known_hosts file.
    /// </remarks>
    public SshConfigHostBuilder CheckHostIp(string? checkHostIp)
    {
        _checkHostIp = checkHostIp;
        return this;
    }

    /// <summary>
    /// Sets the encryption cipher for the connection.
    /// </summary>
    /// <param name="cipher">The cipher name (e.g., "aes256-ctr").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the symmetric encryption algorithm used to protect the session.
    /// </remarks>
    public SshConfigHostBuilder Cipher(string? cipher)
    {
        _cipher = cipher;
        return this;
    }

    /// <summary>
    /// Sets the list of supported ciphers for the connection.
    /// </summary>
    /// <param name="ciphers">The list of cipher names (e.g., "aes256-ctr,aes192-ctr").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This overrides the default list of ciphers and sets a custom list.
    /// </remarks>
    public SshConfigHostBuilder Ciphers(List<string>? ciphers)
    {
        _ciphers = ciphers;
        return this;
    }

    /// <summary>
    /// Clears all existing port forwardings.
    /// </summary>
    /// <param name="yes">True to clear all forwardings, false otherwise.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is useful to ensure no stale or conflicting forwardings are active.
    /// </remarks>
    public SshConfigHostBuilder ClearAllForwardings(bool? yes = true)
    {
        _clearAllForwardings = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables compression for the connection.
    /// </summary>
    /// <param name="with">True to enable compression, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Compression can reduce the amount of data sent over the network, but may decrease CPU performance.
    /// </remarks>
    public SshConfigHostBuilder Compression(bool? with = true)
    {
        _compression = with;
        return this;
    }

    /// <summary>
    /// Sets the compression level (1-9) for the connection.
    /// </summary>
    /// <param name="cL">The compression level.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Higher levels provide more compression but require more CPU resources.
    /// </remarks>
    public SshConfigHostBuilder CompressionLevel(int? cL)
    {
        _compressionLevel = cL;
        return this;
    }

    /// <summary>
    /// Sets the number of connection attempts before giving up.
    /// </summary>
    /// <param name="cA">The number of connection attempts.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// If the first connection attempt fails, SSH will retry until this limit is reached.
    /// </remarks>
    public SshConfigHostBuilder ConnectionAttempts(int? cA)
    {
        _connectionAttempts = cA;
        return this;
    }

    /// <summary>
    /// Sets the connection timeout in seconds.
    /// </summary>
    /// <param name="cT">The connection timeout.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// If the server does not respond within this time, the connection attempt will be aborted.
    /// </remarks>
    public SshConfigHostBuilder ConnectionTimeout(int? cT)
    {
        _connectionTimeout = cT;
        return this;
    }

    /// <summary>
    /// Enables or disables control master mode for connection sharing.
    /// </summary>
    /// <param name="enum">The control master mode enum value.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Control master mode allows multiple SSH sessions to share a single network connection.
    /// </remarks>
    public SshConfigHostBuilder ControlMaster(ControlMasterEnum @enum)
    {
        _controlMaster = @enum;
        return this;
    }

    /// <summary>
    /// Sets the path for the control socket used in control master mode.
    /// </summary>
    /// <param name="cP">The control path.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the Unix socket or tcp:host:port used for communication between the master and slave
    /// connections.
    /// </remarks>
    public SshConfigHostBuilder ControlPath(string? cP)
    {
        _controlPath = cP;
        return this;
    }

    /// <summary>
    /// Sets the dynamic forward option for SOCKS proxying.
    /// </summary>
    /// <param name="dF">The dynamic forward specification (e.g., "8080").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This enables dynamic port forwarding using a local SOCKS proxy.
    /// </remarks>
    public SshConfigHostBuilder DynamicForward(string? dF)
    {
        _dynamicForward = dF;
        return this;
    }

    /// <summary>
    /// Enables or disables SSH key signing.
    /// </summary>
    /// <param name="yes">True to enable SSH key signing, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// SSH key signing allows the use of SSH keys for host authentication.
    /// </remarks>
    public SshConfigHostBuilder EnableSshKeysign(bool? yes = true)
    {
        _enableSshKeysign = yes;
        return this;
    }

    /// <summary>
    /// Sets the escape character for SSH protocol automation.
    /// </summary>
    /// <param name="escapeChar">The escape character (e.g., "~").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// The escape character is used to delimit commands sent to the SSH client.
    /// </remarks>
    public SshConfigHostBuilder EscapeChar(string? escapeChar)
    {
        _escapeChar = escapeChar;
        return this;
    }

    /// <summary>
    /// Enables or disables exit on forward failure.
    /// </summary>
    /// <param name="yes">True to enable exit on forward failure, false otherwise.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// If enabled, the SSH client will disconnect if a requested port forwarding fails.
    /// </remarks>
    public SshConfigHostBuilder ExitOnForwardFailure(bool? yes = true)
    {
        _exitOnForwardFailure = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables SSH agent forwarding.
    /// </summary>
    /// <param name="yes">True to enable agent forwarding, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Agent forwarding allows the remote host to use the local SSH agent for authentication.
    /// </remarks>
    public SshConfigHostBuilder ForwardAgent(bool? yes = true)
    {
        _forwardAgent = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables X11 forwarding.
    /// </summary>
    /// <param name="yes">True to enable X11 forwarding, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// X11 forwarding allows remote graphical applications to be displayed locally.
    /// </remarks>
    public SshConfigHostBuilder ForwardX11(bool? yes = true)
    {
        _forwardX11 = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables trusted X11 forwarding.
    /// </summary>
    /// <param name="yes">True to enable trusted X11 forwarding, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Trusted X11 forwarding disables access control and allows any X client to connect to the local X server.
    /// </remarks>
    public SshConfigHostBuilder ForwardX11Trusted(bool? yes = true)
    {
        _forwardX11Trusted = yes;
        return this;
    }

    /// <summary>
    /// Sets the gateway ports option for remote port forwarding.
    /// </summary>
    /// <param name="gP">The gateway ports specification (e.g., "y").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This controls which hosts are allowed to connect to forwarded ports.
    /// </remarks>
    public SshConfigHostBuilder GatewayPorts(string? gP)
    {
        _gatewayPorts = gP;
        return this;
    }

    /// <summary>
    /// Sets the global known hosts file for host key verification.
    /// </summary>
    /// <param name="file">The file path (e.g., "/etc/ssh/ssh_known_hosts").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies an additional file to search for known host keys.
    /// </remarks>
    public SshConfigHostBuilder GlobalKnownHostsFile(string? file)
    {
        _globalKnownHostsFile = file;
        return this;
    }

    /// <summary>
    /// Enables or disables GSSAPI authentication.
    /// </summary>
    /// <param name="yes">True to enable GSSAPI authentication, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// GSSAPI authentication allows using Kerberos tickets for SSH authentication.
    /// </remarks>
    public SshConfigHostBuilder GssApiAuthentication(bool? yes = true)
    {
        _gssApiAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables GSSAPI key exchange.
    /// </summary>
    /// <param name="yes">True to enable GSSAPI key exchange, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// GSSAPI key exchange allows using Kerberos tickets to secure the key exchange.
    /// </remarks>
    public SshConfigHostBuilder GssApiKeyExchange(bool? yes = true)
    {
        _gssApiKeyExchange = yes;
        return this;
    }

    /// <summary>
    /// Sets the GSSAPI client identity for authentication.
    /// </summary>
    /// <param name="i">The client identity (e.g., "user@EXAMPLE.COM").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the principal name used for GSSAPI authentication.
    /// </remarks>
    public SshConfigHostBuilder GssApiClientIdentity(string? i)
    {
        _gssApiClientIdentity = i;
        return this;
    }

    /// <summary>
    /// Enables or disables GSSAPI delegate credentials.
    /// </summary>
    /// <param name="yes">True to enable delegation, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Delegation allows the remote server to use the client's Kerberos credentials.
    /// </remarks>
    public SshConfigHostBuilder GssApiDelegateCredentials(bool? yes = true)
    {
        _gssApiDelegateCredentials = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables GSSAPI renewal forces rekeying.
    /// </summary>
    /// <param name="yes">True to enable renewal forces rekeying, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This forces a rekeying of the session whenever the GSSAPI ticket is renewed.
    /// </remarks>
    public SshConfigHostBuilder GssApiRenewalForcesRekey(bool? yes = true)
    {
        _gssApiRenewalForcesRekey = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables GSSAPI trust DNS.
    /// </summary>
    /// <param name="yes">True to enable DNS trust, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, the GSSAPI authentication will use the DNS to canonicalize the host name.
    /// </remarks>
    public SshConfigHostBuilder GssApiTrustDns(bool? yes = true)
    {
        _gssApiTrustDns = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables hash known hosts.
    /// </summary>
    /// <param name="yes">True to enable hash known hosts, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, the host key hashes are used and the actual keys are not stored in the known_hosts file.
    /// </remarks>
    public SshConfigHostBuilder HashKnownHosts(bool? yes = true)
    {
        _hashKnownHosts = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables host-based authentication.
    /// </summary>
    /// <param name="yes">True to enable host-based authentication, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Host-based authentication uses the client's IP address and host name for authentication.
    /// </remarks>
    public SshConfigHostBuilder HostbasedAuthentication(bool? yes = true)
    {
        _hostbasedAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Sets the host key algorithms for host authentication.
    /// </summary>
    /// <param name="a">The algorithm names (e.g., "rsa-sha2-256,rsa-sha2-512").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the public key algorithms accepted for host authentication.
    /// </remarks>
    public SshConfigHostBuilder HostKeyAlgorithms(string? a)
    {
        _hostKeyAlgorithms = a;
        return this;
    }

    /// <summary>
    /// Sets the host key alias for the host.
    /// </summary>
    /// <param name="h">The host key alias.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies an alternative name for the host key, useful for host key rotation.
    /// </remarks>
    public SshConfigHostBuilder HostKeyAlias(string? h)
    {
        _hostKeyAlias = h;
        return this;
    }

    /// <summary>
    /// Sets the real host name or IP address of the SSH server.
    /// </summary>
    /// <param name="hN">The host name (e.g., "example.com").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is the address the SSH client will connect to. It can be a domain name or an IP address.
    /// </remarks>
    public SshConfigHostBuilder HostName(string? hN)
    {
        _hostName = hN;
        return this;
    }

    /// <summary>
    /// Requires that only the specified identity file is used for authentication.
    /// </summary>
    /// <param name="yes">True to enforce, false to relax.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, only the identity file(s) explicitly specified are used, ignoring any agent or default files.
    /// </remarks>
    public SshConfigHostBuilder IdentitiesOnly(bool? yes = true)
    {
        _identitiesOnly = yes;
        return this;
    }

    /// <summary>
    /// Sets the primary identity file for public key authentication.
    /// </summary>
    /// <param name="iF">The identity file path (e.g., "~/.ssh/id_rsa").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the file containing the private key for public key authentication.
    /// </remarks>
    public SshConfigHostBuilder IdentityFile(string? iF)
    {
        _identityFile = iF;
        return this;
    }

    /// <summary>
    /// Enables or disables keyboard-interactive authentication.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Keyboard-interactive authentication prompts the user for a password or passphrase.
    /// </remarks>
    public SshConfigHostBuilder KbdInteractiveAuthentication(bool? yes = true)
    {
        _kbdInteractiveAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Sets the devices for keyboard-interactive authentication.
    /// </summary>
    /// <param name="d">The devices (e.g., "pam").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the authentication methods used in keyboard-interactive mode.
    /// </remarks>
    public SshConfigHostBuilder KbdInteractiveDevices(string? d)
    {
        _kbdInteractiveDevices = d;
        return this;
    }

    /// <summary>
    /// Sets the local command to be executed after successful authentication.
    /// </summary>
    /// <param name="c">The command (e.g., "echo 'Welcome!'").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// The command is executed on the remote server after the SSH session is established.
    /// </remarks>
    public SshConfigHostBuilder LocalCommand(string? c)
    {
        _localCommand = c;
        return this;
    }

    /// <summary>
    /// Sets the local port forwarding specification.
    /// </summary>
    /// <param name="f">The forwarding specification (e.g., "8080:remote.com:80").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This forwards a local port to a remote host and port through the SSH tunnel.
    /// </remarks>
    public SshConfigHostBuilder LocalForward(string? f)
    {
        _localForward = f;
        return this;
    }

    /// <summary>
    /// Sets the logging level for SSH messages.
    /// </summary>
    /// <param name="lL">The log level (e.g., "INFO").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This controls the verbosity of the SSH client's log messages.
    /// </remarks>
    public SshConfigHostBuilder LogLevel(string? lL)
    {
        _logLevel = lL;
        return this;
    }

    /// <summary>
    /// Sets the message authentication codes (MACs) for data integrity.
    /// </summary>
    /// <param name="macs">The MACs (e.g., "hmac-sha2-256,hmac-sha2-512").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the algorithms used to verify the integrity and authenticity of the data.
    /// </remarks>
    public SshConfigHostBuilder Macs(string? macs)
    {
        _macs = macs;
        return this;
    }

    /// <summary>
    /// Disables host authentication for the localhost.
    /// </summary>
    /// <param name="yes">True to disable, false to enable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This allows connections to localhost without host key verification.
    /// </remarks>
    public SshConfigHostBuilder NoHostAuthenticationForLocalhost(bool yes = true)
    {
        _noHostAuthenticationForLocalhost = yes;
        return this;
    }

    /// <summary>
    /// Sets the number of password prompts for authentication.
    /// </summary>
    /// <param name="a">The number of attempts.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This controls how many times the user is prompted for a password before the authentication fails.
    /// </remarks>
    public SshConfigHostBuilder NumberOfPasswordAttempts(int? a)
    {
        _numberOfPasswordPrompts = a;
        return this;
    }

    /// <summary>
    /// Enables or disables password authentication.
    /// </summary>
    /// <param name="yes">True to enable password authentication, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Password authentication requires the user to enter a password for authentication.
    /// </remarks>
    public SshConfigHostBuilder PasswordAuthentication(bool? yes = true)
    {
        _passwordAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables local command execution after successful login.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, the specified local command is executed each time this host configuration is used.
    /// </remarks>
    public SshConfigHostBuilder PermitLocalCommand(bool? yes = true)
    {
        _permitLocalCommand = yes;
        return this;
    }

    /// <summary>
    /// Sets the port number for the SSH connection.
    /// </summary>
    /// <param name="port">The port number (e.g., 22).</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the port on the SSH server to connect to. The default SSH port is 22.
    /// </remarks>
    public SshConfigHostBuilder Port(int? port)
    {
        _port = port;
        return this;
    }

    /// <summary>
    /// Adds a preferred authentication method.
    /// </summary>
    /// <param name="a">The authentication method (e.g., "publickey").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Multiple authentication methods can be specified; SSH will try them in order.
    /// </remarks>
    public SshConfigHostBuilder PreferredAuthentication(string a)
    {
        _preferredAuthentication ??= new List<string>();
        _preferredAuthentication.Add(a);
        return this;
    }

    /// <summary>
    /// Sets the SSH protocol version to use.
    /// </summary>
    /// <param name="p">The protocol version (e.g., 2).</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Only protocol version 2 is currently supported.
    /// </remarks>
    public SshConfigHostBuilder Protocol(int? p)
    {
        _protocol = p;
        return this;
    }

    /// <summary>
    /// Sets the proxy command for connecting through a proxy.
    /// </summary>
    /// <param name="pC">The proxy command (e.g., "nc -x proxy:port %h %p").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the command used to connect to the SSH server through a proxy.
    /// </remarks>
    public SshConfigHostBuilder ProxyCommand(string? pC)
    {
        _proxyCommand = pC;
        return this;
    }

    /// <summary>
    /// Enables or disables public key authentication.
    /// </summary>
    /// <param name="yes">True to enable public key authentication, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Public key authentication uses a private key for authentication, providing better security than passwords.
    /// </remarks>
    public SshConfigHostBuilder PubkeyAuthentication(bool? yes = true)
    {
        _pubkeyAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Sets the rekey limit for the connection.
    /// </summary>
    /// <param name="rL">The rekey limit (e.g., "1G").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the amount of data that can be sent before a key re-exchange is required.
    /// </remarks>
    public SshConfigHostBuilder RekeyLimit(string? rL)
    {
        _rekeyLimit = rL;
        return this;
    }

    /// <summary>
    /// Sets the value for removing specific port forwardings.
    /// </summary>
    /// <param name="value">The value indicating which forwardings to remove.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This can be used to selectively remove certain port forwardings that were previously added.
    /// </remarks>
    public SshConfigHostBuilder RemoveForward(string? value)
    {
        _removeForward = value;
        return this;
    }

    /// <summary>
    /// Enables or disables rhosts RSA authentication.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Rhosts RSA authentication uses the client's host credentials for authentication.
    /// </remarks>
    public SshConfigHostBuilder RHostRsaAuthentication(bool? yes = true)
    {
        _rHostsRsaAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables RSA authentication.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// RSA authentication uses the RSA algorithm for public key authentication.
    /// </remarks>
    public SshConfigHostBuilder RsaAuthentication(bool? yes = true)
    {
        _rsaAuthentication = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables the sending of environment variables to the remote session.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This allows specifying which environment variables should be available in the remote SSH session.
    /// </remarks>
    public SshConfigHostBuilder SendEnv(bool? yes = true)
    {
        _sendEnv = yes;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of server alive messages that can be sent without receiving a response.
    /// </summary>
    /// <param name="c">The maximum count.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is used to detect and terminate unresponsive connections.
    /// </remarks>
    public SshConfigHostBuilder ServerAliveCountMax(int? c)
    {
        _serverAliveCountMax = c;
        return this;
    }

    /// <summary>
    /// Sets the interval in seconds between sending server alive messages.
    /// </summary>
    /// <param name="i">The interval.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This configures how often the client sends keepalive messages to the server.
    /// </remarks>
    public SshConfigHostBuilder ServerAliveInterval(int? i)
    {
        _serverAliveInterval = i;
        return this;
    }

    /// <summary>
    /// Sets the smartcard device for authentication.
    /// </summary>
    /// <param name="s">The smartcard device specification.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This is used to specify the smartcard reader and the card to be used for authentication.
    /// </remarks>
    public SshConfigHostBuilder SmardcardDevice(string? s)
    {
        _smartcardDevice = s;
        return this;
    }

    /// <summary>
    /// Enables or disables strict host key checking.
    /// </summary>
    /// <param name="enum">The strictness level (yes, no, ask).</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Strict host key checking controls how the client verifies the server's host key.
    /// </remarks>
    public SshConfigHostBuilder StrictHostKeyChecking(YesNoAskEnum @enum)
    {
        _strictHostKeyChecking = @enum;
        return this;
    }

    /// <summary>
    /// Enables or disables TCP keepalive messages.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// TCP keepalive messages are used to keep the connection open and detect dead peers.
    /// </remarks>
    public SshConfigHostBuilder TcpKeepAlive(bool? yes = true)
    {
        _tcpKeepAlive = yes;
        return this;
    }

    /// <summary>
    /// Enables or disables tunneling (VPN-like) features.
    /// </summary>
    /// <param name="enum">The tunneling mode (if, remote, local, no).</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Tunneling allows forwarding of arbitrary traffic over the SSH connection.
    /// </remarks>
    public SshConfigHostBuilder Tunnel(TunnelEnum @enum)
    {
        _tunnel = @enum;
        return this;
    }

    /// <summary>
    /// Sets the tunnel device specification.
    /// </summary>
    /// <param name="tD">The tunnel device specification.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the local or remote device to be used for tunneling.
    /// </remarks>
    public SshConfigHostBuilder TunnelDevice(string? tD)
    {
        _tunnelDevice = tD;
        return this;
    }

    /// <summary>
    /// Enables or disables the use of privileged ports.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Privileged ports are those below 1024, which normally require root permissions to use.
    /// </remarks>
    public SshConfigHostBuilder UsePrivilegedPort(bool? yes = true)
    {
        _usePrivilegedPort = yes;
        return this;
    }

    /// <summary>
    /// Sets the user name for authentication.
    /// </summary>
    /// <param name="u">The user name (e.g., "alice").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the user account on the remote SSH server to authenticate as.
    /// </remarks>
    public SshConfigHostBuilder User(string? u)
    {
        _user = u;
        return this;
    }

    /// <summary>
    /// Sets the user known hosts file for host key verification.
    /// </summary>
    /// <param name="h">The file path (e.g., "~/.ssh/known_hosts").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the file where the client's known host keys are stored.
    /// </remarks>
    public SshConfigHostBuilder UserKnownHostsFile(string? h)
    {
        _userKnownHostsFile = h;
        return this;
    }

    /// <summary>
    /// Enables or disables verification of host key DNS records.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// When enabled, the client will verify the server's host key using DNS records.
    /// </remarks>
    public SshConfigHostBuilder VerifyHostKeyDns(bool? yes = true)
    {
        _verifyHostKeyDns = yes;
        return this;
    }


    /// <summary>
    /// Enables or disables the use of visual host key authentication.
    /// </summary>
    /// <param name="yes">True to enable, false to disable.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// Visual host key authentication provides a visual display of the host key fingerprint for verification.
    /// </remarks>
    public SshConfigHostBuilder VisualHostKey(bool? yes = true)
    {
        _visualHostKey = yes;
        return this;
    }

    /// <summary>
    /// Sets the X11 authentication location.
    /// </summary>
    /// <param name="l">The X11 auth location (e.g., "/tmp/.X11-unix/X0").</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>
    /// This specifies the location of the X11 authentication socket.
    /// </remarks>
    public SshConfigHostBuilder XAuthLocation(string? l)
    {
        _xAuthLocation = l;
        return this;
    }

    /// <summary>
    /// Validates the SSH configuration host properties and records any validation failures.
    /// </summary>
    /// <remarks>This method checks that required SSH configuration properties are not null, empty, or consist
    /// solely of whitespace. Any validation errors encountered are added to the failures dictionary for further
    /// processing. This method is intended to be called as part of a larger validation workflow and does not throw
    /// exceptions directly for validation errors.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary that collects validation failures, mapping property names to the corresponding exception describing
    /// the failure.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(SshConfigHost.Name), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_batchMode, nameof(SshConfigHost.BatchMode), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_bindAddress, nameof(SshConfigHost.BindAddress), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_challengeResponseAuthentication, nameof(SshConfigHost.ChallengeResponseAuthentication), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_checkHostIp, nameof(SshConfigHost.CheckHostIp), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_cipher, nameof(SshConfigHost.Cipher), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_ciphers ?? [], nameof(SshConfigHost.Ciphers), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_controlPath, nameof(SshConfigHost.ControlPath), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_dynamicForward, nameof(SshConfigHost.DynamicForward), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_escapeChar, nameof(SshConfigHost.EscapeChar), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_gatewayPorts, nameof(SshConfigHost.GatewayPorts), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_globalKnownHostsFile, nameof(SshConfigHost.GlobalKnownHostsFile), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_gssApiClientIdentity, nameof(SshConfigHost.GssApiClientIdentity), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_hostKeyAlgorithms, nameof(SshConfigHost.HostKeyAlgorithms), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_hostKeyAlgorithms, nameof(SshConfigHost.HostKeyAlias), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_hostName, nameof(SshConfigHost.HostName), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_identityFile, nameof(SshConfigHost.IdentityFile), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_kbdInteractiveDevices, nameof(SshConfigHost.KbdInteractiveDevices), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_localCommand, nameof(SshConfigHost.LocalCommand), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_localForward, nameof(SshConfigHost.LocalForward), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_logLevel, nameof(SshConfigHost.LogLevel), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_macs, nameof(SshConfigHost.Macs), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_preferredAuthentication ?? [], nameof(SshConfigHost.PreferredAuthentications), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_proxyCommand, nameof(SshConfigHost.ProxyCommand), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_rekeyLimit, nameof(SshConfigHost.RekeyLimit), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_removeForward, nameof(SshConfigHost.RemoveForward), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_smartcardDevice, nameof(SshConfigHost.SmartcardDevice), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_tunnelDevice, nameof(SshConfigHost.TunnelDevice), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_user, nameof(SshConfigHost.User), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_userKnownHostsFile, nameof(SshConfigHost.UserKnownHostsFile), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
        AssertNotEmptyOrWhitespace(_xAuthLocation, nameof(SshConfigHost.XAuthLocation), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
    }

    /// <summary>
    /// Creates a new instance of <see cref="SshConfigHost"/> populated with the current configuration values.
    /// </summary>
    /// <remarks>All properties of the returned <see cref="SshConfigHost"/> are set based on the corresponding
    /// values in this instance. This method is typically used to materialize a configuration object for use in SSH
    /// connection scenarios.</remarks>
    /// <returns>A <see cref="SshConfigHost"/> object containing the SSH configuration settings specified by this instance.</returns>
    /// <exception cref="MissingMemberException">Thrown if the host name is not specified.</exception>
    protected override SshConfigHost Instantiate() => new SshConfigHost
    {
        Name = _name ?? throw new MissingMemberException(nameof(_name)),
        AddressFamily = _addressFamily,
        BatchMode = _batchMode,
        BindAddress = _bindAddress,
        ChallengeResponseAuthentication = _challengeResponseAuthentication,
        CheckHostIp = _checkHostIp,
        Cipher = _cipher,
        Ciphers = _ciphers,
        ClearAllForwardings = _clearAllForwardings,
        Compression = _compression,
        CompressionLevel = _compressionLevel,
        ConnectionAttempts = _connectionAttempts,
        ConnectTimeout = _connectionTimeout,
        ControlMaster = _controlMaster,
        ControlPath = _controlPath,
        DynamicForward = _dynamicForward,
        EnableSshKeysign = _enableSshKeysign,
        EscapeChar = _escapeChar,
        ExitOnForwardFailure = _exitOnForwardFailure,
        ForwardAgent = _forwardAgent,
        ForwardX11 = _forwardX11,
        ForwardX11Trusted = _forwardX11Trusted,
        GatewayPorts = _gatewayPorts,
        GlobalKnownHostsFile = _globalKnownHostsFile,
        GssApiAuthentication = _gssApiAuthentication,
        GssApiKeyExchange = _gssApiKeyExchange,
        GssApiClientIdentity = _gssApiClientIdentity,
        GssApiDelegateCredentials = _gssApiDelegateCredentials,
        GssApiRenewalForcesRekey = _gssApiRenewalForcesRekey,
        GssApiTrustDns = _gssApiTrustDns,
        HashKnownHosts = _hashKnownHosts,
        HostbasedAuthentication = _hostbasedAuthentication,
        HostKeyAlgorithms = _hostKeyAlgorithms,
        HostKeyAlias = _hostKeyAlias,
        HostName = _hostName,
        IdentitiesOnly = _identitiesOnly,
        IdentityFile = _identityFile,
        KbdInteractiveAuthentication = _kbdInteractiveAuthentication,
        KbdInteractiveDevices = _kbdInteractiveDevices,
        LocalCommand = _localCommand,
        LocalForward = _localForward,
        LogLevel = _logLevel,
        Macs = _macs,
        NoHostAuthenticationForLocalhost = _noHostAuthenticationForLocalhost,
        NumberOfPasswordPrompts = _numberOfPasswordPrompts,
        PasswordAuthentication = _passwordAuthentication,
        PermitLocalCommand = _permitLocalCommand,
        Port = _port,
        PreferredAuthentications = _preferredAuthentication,
        Protocol = _protocol,
        ProxyCommand = _proxyCommand,
        PubkeyAuthentication = _pubkeyAuthentication,
        RekeyLimit = _rekeyLimit,
        RemoveForward = _removeForward,
        RhostsRsaAuthentication = _rHostsRsaAuthentication,
        RsaAuthentication = _rsaAuthentication,
        SendEnv = _sendEnv,
        ServerAliveCountMax = _serverAliveCountMax,
        ServerAliveInterval = _serverAliveInterval,
        SmartcardDevice = _smartcardDevice,
        StrictHostKeyChecking = _strictHostKeyChecking,
        TcpKeeAlive = _tcpKeepAlive,
        Tunnel = _tunnel,
        TunnelDevice = _tunnelDevice,
        UsePrivilegedPort = _usePrivilegedPort,
        User = _user,
        UserKnownHostsFile = _userKnownHostsFile,
        VerifyHostKeyDns = _verifyHostKeyDns,
        VisualHostKey = _visualHostKey,
        XAuthLocation = _xAuthLocation
    };
}