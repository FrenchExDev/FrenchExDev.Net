#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to serialize an <see cref="SshConfigHost"/> instance into a list of SSH configuration file
/// lines for a single host entry.
/// </summary>
/// <remarks>Use <see cref="Write"/> to generate the SSH configuration lines corresponding to the properties set
/// on the provided <see cref="SshConfigHost"/>. Only properties with non-null values are included in the output. This
/// class is intended for scenarios where programmatic generation or modification of SSH config files is
/// required.</remarks>
public class SshConfigHostWriter : IWriter<SshConfigHost>
{
    /// <summary>
    /// Generates a list of SSH configuration lines representing the specified host settings.
    /// </summary>
    /// <remarks>The returned list can be written directly to an SSH config file or used for inspection. Only
    /// properties of the host that are not null are included in the output. The first line always specifies the host
    /// name using the 'Host' directive.</remarks>
    /// <param name="subject">The host configuration to serialize. Must not be null. Each property of the host is translated into its
    /// corresponding SSH configuration directive if set.</param>
    /// <returns>A list of strings, each representing a line in the SSH configuration for the specified host. The list will
    /// contain only the directives corresponding to properties that are set on the host.</returns>
    public List<string> Write(SshConfigHost subject)
    {
        // Initialize the output list with the Host directive (required for SSH config)
        var sb = new List<string>()
        {
            "Host " + subject.Name
        };

        // For each property, check if it is set (not null) and add the corresponding SSH config line.
        // Only non-null properties are serialized, matching SSH config file conventions.

        if (subject.AddressFamily is not null)
            // AddressFamily: Specifies IPv4/IPv6/both
            sb.Add("\tAddressFamily " + new AddressFamilyStringBuilder().Build(subject.AddressFamily.Value));

        if (subject.BatchMode is not null)
            // BatchMode: Enables/disables batch mode
            sb.Add("\tBachMode " + subject.BatchMode);

        if (subject.BindAddress is not null)
            // BindAddress: Local address for outgoing connections
            sb.Add("\tBindAddress " + subject.BindAddress);

        if (subject.ChallengeResponseAuthentication is not null)
            // ChallengeResponseAuthentication: Enables/disables challenge-response
            sb.Add("\tChallengeResponseAuthentication " + subject.ChallengeResponseAuthentication);

        if (subject.CheckHostIp is not null)
            // CheckHostIp: Enables/disables host IP checking
            sb.Add("\tCheckHostIp " + subject.CheckHostIp);

        if (subject.Cipher is not null)
            // Cipher: Specifies encryption cipher
            sb.Add("\tCipher " + subject.Cipher);

        if (subject.Ciphers is not null && subject.Ciphers.Count > 0)
            // Ciphers: List of allowed ciphers, joined by comma
            sb.Add("\tCiphers " + string.Join(",", subject.Ciphers));

        if (subject.ClearAllForwardings is not null)
            // ClearAllForwardings: Clears all port forwardings
            sb.Add("\tClearAllForwardings " + subject.ClearAllForwardings);

        if (subject.Compression is not null)
            // Compression: Enables/disables compression (yes/no)
            sb.Add("\tCompression " + (subject.Compression.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.CompressionLevel is not null)
            // CompressionLevel: Sets compression level
            sb.Add("\tCompressionLevel " + subject.CompressionLevel);

        if (subject.ConnectionAttempts is not null)
            // ConnectionAttempts: Number of connection attempts
            sb.Add("\tConnectionAttempts " + subject.ConnectionAttempts);

        if (subject.ConnectTimeout is not null)
            // ConnectTimeout: Timeout in seconds
            sb.Add("\tConnectTimeout " + subject.ConnectTimeout);

        if (subject.ControlMaster is not null)
            // ControlMaster: Master connection mode
            sb.Add("\tControlMaster " + new ControlMasterEnumStringBuilder().Build(subject.ControlMaster.Value));

        if (subject.ControlPath is not null)
            // ControlPath: Path for control socket
            sb.Add("\tControlPath " + subject.ControlPath);

        if (subject.DynamicForward is not null)
            // DynamicForward: Dynamic port forwarding (SOCKS proxy)
            sb.Add("\tDynamicForward " + subject.DynamicForward);

        if (subject.EnableSshKeysign is not null)
            // EnableSshKeysign: Enables/disables SSH key signing
            sb.Add("\tEnableSshKeysign " + (subject.EnableSshKeysign.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.EscapeChar is not null)
            // EscapeChar: Escape character for SSH session
            sb.Add("\tEscapeChar " + subject.EscapeChar);

        if (subject.ExitOnForwardFailure is not null)
            // ExitOnForwardFailure: Disconnect if forwarding fails
            sb.Add("\tExitOnForwardFailure " + (subject.ExitOnForwardFailure.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.ForwardAgent is not null)
            // ForwardAgent: Enables/disables agent forwarding
            sb.Add("\tForwardAgent " + (subject.ForwardAgent.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.ForwardX11 is not null)
            // ForwardX11: Enables/disables X11 forwarding
            sb.Add("\tForwardX11 " + (subject.ForwardX11.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.ForwardX11Trusted is not null)
            // ForwardX11Trusted: Enables/disables trusted X11 forwarding
            sb.Add("\tForwardX11Trusted " + (subject.ForwardX11Trusted.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.GatewayPorts is not null)
            // GatewayPorts: Controls remote port forwarding
            sb.Add("\tGatewayPorts " + subject.GatewayPorts);

        if (subject.GlobalKnownHostsFile is not null)
            // GlobalKnownHostsFile: Path to global known hosts file
            sb.Add("\tGlobalKnownHostsFile " + subject.GlobalKnownHostsFile);

        if (subject.GssApiAuthentication is not null)
            // GSSAPIAuthentication: Enables/disables GSSAPI authentication
            sb.Add("\tGSSAPIAuthentication " + (subject.GssApiAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.GssApiKeyExchange is not null)
            // GSSAPIKeyExchange: Enables/disables GSSAPI key exchange
            sb.Add("\tGSSAPIKeyExchange " + subject.GssApiKeyExchange);

        if (subject.GssApiClientIdentity is not null)
            // GSSAPIClientIdentity: Specifies GSSAPI client identity
            sb.Add("\tGSSAPIClientIdentity " + subject.GssApiClientIdentity);

        if (subject.GssApiDelegateCredentials is not null)
            // GSSAPIDelegateCredentials: Enables delegation of credentials
            sb.Add("\tGSSAPIDelegateCredentials " + subject.GssApiDelegateCredentials);

        if (subject.GssApiRenewalForcesRekey is not null)
            // GSSAPIRenewalForcesRekey: Forces rekeying on renewal
            sb.Add("\tGSSAPIRenewalForcesRekey " + subject.GssApiRenewalForcesRekey);

        if (subject.GssApiTrustDns is not null)
            // GSSAPITrustDns: Enables trust of DNS for GSSAPI
            sb.Add("\tGSSAPITrustDns " + subject.GssApiTrustDns);

        if (subject.HashKnownHosts is not null)
            // HashKnownHosts: Enables/disables hashing of known hosts
            sb.Add("\tHashKnownHosts " + (subject.HashKnownHosts.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.HostbasedAuthentication is not null)
            // HostbasedAuthentication: Enables/disables host-based authentication
            sb.Add("\tHostbasedAuthentication " + (subject.HostbasedAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.HostKeyAlgorithms is not null)
            // HostKeyAlgorithms: Specifies allowed host key algorithms
            sb.Add("\tHostKeyAlgorithms " + subject.HostKeyAlgorithms);

        if (subject.HostKeyAlias is not null)
            // HostKeyAlias: Specifies alias for host key
            sb.Add("\tHostKeyAlias " + subject.HostKeyAlias);

        if (subject.HostName is not null)
            // HostName: Actual hostname or IP address
            sb.Add("\tHostName " + subject.HostName);

        if (subject.IdentitiesOnly is not null)
            // IdentitiesOnly: Restricts authentication to specified identity files
            sb.Add("\tIdentitiesOnly " + (subject.IdentitiesOnly.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.IdentityFile is not null)
            // IdentityFile: Path to identity file
            sb.Add("\tIdentityFile " + subject.IdentityFile);

        if (subject.KbdInteractiveAuthentication is not null)
            // KbdInteractiveAuthentication: Enables keyboard-interactive authentication
            sb.Add("\tKbdInteractiveAuthentication " + subject.KbdInteractiveAuthentication);

        if (subject.KbdInteractiveDevices is not null)
            // KbdInteractiveDevices: Allowed keyboard-interactive devices
            sb.Add("\tKbdInteractiveDevices " + subject.KbdInteractiveDevices);

        if (subject.LocalCommand is not null)
            // LocalCommand: Command to execute after connecting
            sb.Add("\tLocalCommand " + subject.LocalCommand);

        if (subject.LocalForward is not null)
            // LocalForward: Local port forwarding rule
            sb.Add("\tLocalForward " + subject.LocalForward);

        if (subject.LogLevel is not null)
            // LogLevel: Logging level for SSH session
            sb.Add("\tLogLevel " + subject.LogLevel);

        if (subject.Macs is not null)
            // MACs: Allowed message authentication codes
            sb.Add("\tMACs " + subject.Macs);

        if (subject.NoHostAuthenticationForLocalhost is not null)
            // NoHostAuthenticationForLocalhost: Disables host authentication for localhost
            sb.Add("\tNoHostAuthenticationForLocalhost " + (subject.NoHostAuthenticationForLocalhost.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.NumberOfPasswordPrompts is not null)
            // NumberOfPasswordPrompts: Number of password prompts before giving up
            sb.Add("\tNumberOfPasswordPrompts " + subject.NumberOfPasswordPrompts);

        if (subject.PasswordAuthentication is not null)
            // PasswordAuthentication: Enables/disables password authentication
            sb.Add("\tPasswordAuthentication " + (subject.PasswordAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.PermitLocalCommand is not null)
            // PermitLocalCommand: Enables/disables local command execution
            sb.Add("\tPermitLocalCommand " + (subject.PermitLocalCommand.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.Port is not null)
            // Port: Port to connect to on remote host
            sb.Add("\tPort " + subject.Port);

        if (subject.PreferredAuthentications is not null && subject.PreferredAuthentications.Count > 0)
            // PreferredAuthentications: List of preferred authentication methods
            sb.Add("\tPreferredAuthentications " + string.Join(",", subject.PreferredAuthentications));

        if (subject.Protocol is not null)
            // Protocol: SSH protocol version
            sb.Add("\tProtocol " + subject.Protocol);

        if (subject.ProxyCommand is not null)
            // ProxyCommand: Command to use for proxying connection
            sb.Add("\tProxyCommand " + subject.ProxyCommand);

        if (subject.PubkeyAuthentication is not null)
            // PubkeyAuthentication: Enables/disables public key authentication
            sb.Add("\tPubkeyAuthentication " + (subject.PubkeyAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.RekeyLimit is not null)
            // RekeyLimit: Data limit before key re-exchange
            sb.Add("\tRekeyLimit " + subject.RekeyLimit);

        if (subject.RemoveForward is not null)
            // RemoveForward: Forwarding rule to remove
            sb.Add("\tRemoveForward " + subject.RemoveForward);

        if (subject.RhostsRsaAuthentication is not null)
            // RHostsRsaAuthentication: Enables/disables RhostsRSA authentication
            sb.Add("\tRHostsRsaAuthentication " + (subject.RhostsRsaAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.RsaAuthentication is not null)
            // RsaAuthentication: Enables/disables RSA authentication
            sb.Add("\tRsaAuthentication " + (subject.RsaAuthentication.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.SendEnv is not null)
            // SendEnv: Enables/disables sending environment variables
            sb.Add("\tSendEnd " + (subject.SendEnv.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.ServerAliveInterval is not null)
            // ServerAliveInterval: Interval for server alive messages
            sb.Add("\tServerAliveInterval " + subject.ServerAliveInterval);

        if (subject.ServerAliveCountMax is not null)
            // ServerAliveCountMax: Maximum number of server alive messages
            sb.Add("\tServerAliveCountMax " + subject.ServerAliveCountMax);

        if (subject.SmartcardDevice is not null)
            // SmartcardDevice: Device for smartcard authentication
            sb.Add("\tSmartcardDevice " + subject.SmartcardDevice);

        if (subject.StrictHostKeyChecking is not null)
            // StrictHostKeyChecking: Controls strict host key checking behavior
            sb.Add("\tStrictHostKeyChecking " + new YesNoAskEnumStringBuilder().Build(subject.StrictHostKeyChecking.Value));

        if (subject.TcpKeeAlive is not null)
            // TcpKeepAlive: Enables/disables TCP keep-alive messages
            sb.Add("\tTcpKeepAlive " + (subject.TcpKeeAlive.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.Tunnel is not null)
            // Tunnel: Controls tunneling behavior
            sb.Add("\tTunnel " + new TunnelEnumStringBuilder().Build(subject.Tunnel.Value));

        if (subject.TunnelDevice is not null)
            // TunnelDevice: Specifies tunnel device
            sb.Add("\tTunnelDevices " + subject.TunnelDevice);

        if (subject.UsePrivilegedPort is not null)
            // UsePrivilegedPort: Enables/disables use of privileged port
            sb.Add("\tUsePrivilegedPort " + (subject.UsePrivilegedPort.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.User is not null)
            // User: Username for authentication
            sb.Add("\tUser " + subject.User);

        if (subject.UserKnownHostsFile is not null)
            // UserKnownHostsFile: Path to user's known hosts file
            sb.Add("\tUserKnownHostsFile " + subject.UserKnownHostsFile);

        if (subject.VerifyHostKeyDns is not null)
            // VerifyHostKeyDns: Enables/disables DNS-based host key verification
            sb.Add("\tVerifyHostKeyDns " + (subject.VerifyHostKeyDns.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.VisualHostKey is not null)
            // VisualHostKey: Enables/disables visual host key display
            sb.Add("\tVisualHostKey " + (subject.VisualHostKey.Value ? SshConfigHostStrings.YesString : SshConfigHostStrings.NoString));

        if (subject.XAuthLocation is not null)
            // XAuthLocation: Location of XAuth binary for X11 forwarding
            sb.Add("\tXAuthLocation " + subject.XAuthLocation);

        // Return the list of SSH config lines for the host entry
        return sb;
    }
}