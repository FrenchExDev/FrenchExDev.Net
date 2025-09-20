#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to parse SSH configuration host entries from text lines and construct corresponding <see cref="SshConfigHost"/> objects.
/// </summary>
/// <remarks>
/// This class uses a set of predefined regular expressions to identify and extract SSH host configuration options from input lines. It maps each recognized configuration directive to a builder action, enabling flexible and extensible parsing of SSH config files. Thread safety is not guaranteed; concurrent access should be externally synchronized if required.
/// </remarks>
public class SshConfigHostReader : IReader<SshConfigHost>
{
    /// <summary>
    /// Compiled regex to match the "Host" directive and capture its value.
    /// Example: "Host myhost" will capture "myhost".
    /// </summary>
    public static readonly Regex Name = new("Host (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "AddressFamily" directive and capture its value.
    /// Example: "AddressFamily inet" will capture "inet".
    /// </summary>
    public static readonly Regex AddressFamily = new("AddressFamily (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "BatchMode" directive and capture its value.
    /// Example: "BatchMode yes" will capture "yes".
    /// </summary>
    public static readonly Regex BatchMode = new("BatchMode (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "BindAdress" directive and capture its value.
    /// Example: "BindAdress 192.168.1.1" will capture "192.168.1.1".
    /// </summary>
    public static readonly Regex BindAddress = new("BindAdress (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ChallengeResponseAuthentication" directive and capture its value.
    /// Example: "ChallengeResponseAuthentication no" will capture "no".
    /// </summary>
    public static readonly Regex ChallengeResponseAuthentication = new("ChallengeResponseAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "CheckHostIp" directive and capture its value.
    /// Example: "CheckHostIp yes" will capture "yes".
    /// </summary>
    public static readonly Regex CheckHostIp = new("CheckHostIp (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Cipher" directive and capture its value.
    /// Example: "Cipher aes256-ctr" will capture "aes256-ctr".
    /// </summary>
    public static readonly Regex Cipher = new("Cipher (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Ciphers" directive and capture its value.
    /// Example: "Ciphers aes256-ctr aes192-ctr" will capture "aes256-ctr aes192-ctr".
    /// </summary>
    public static readonly Regex Ciphers = new("Ciphers (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ClearAllForwardings" directive and capture its value.
    /// Example: "ClearAllForwardings yes" will capture "yes".
    /// </summary>
    public static readonly Regex ClearAllForwardings = new("ClearAllForwardings (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Compression" directive and capture its value.
    /// Example: "Compression yes" will capture "yes".
    /// </summary>
    public static readonly Regex Compression = new("Compression (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "CompressionLevel" directive and capture its value.
    /// Example: "CompressionLevel 6" will capture "6".
    /// </summary>
    public static readonly Regex CompressionLevel = new("CompressionLevel (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ConnectionAttempts" directive and capture its value.
    /// Example: "ConnectionAttempts 3" will capture "3".
    /// </summary>
    public static readonly Regex ConnectionAttempts = new("ConnectionAttempts (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ConnectTimeout" directive and capture its value.
    /// Example: "ConnectTimeout 10" will capture "10".
    /// </summary>
    public static readonly Regex ConnectTimeout = new("ConnectTimeout (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ControlMaster" directive and capture its value.
    /// Example: "ControlMaster auto" will capture "auto".
    /// </summary>
    public static readonly Regex ControlMaster = new("ControlMaster (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ControlPath" directive and capture its value.
    /// Example: "ControlPath ~/.ssh/cm_socket" will capture "~/.ssh/cm_socket".
    /// </summary>
    public static readonly Regex ControlPath = new("ControlPath (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "DynamicForward" directive and capture its value.
    /// Example: "DynamicForward 8080" will capture "8080".
    /// </summary>
    public static readonly Regex DynamicForward = new("DynamicForward (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "EnableSshKeysign" directive and capture its value.
    /// Example: "EnableSshKeysign yes" will capture "yes".
    /// </summary>
    public static readonly Regex EnableSshKeysign = new("EnableSshKeysign (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "EscapeChar" directive and capture its value.
    /// Example: "EscapeChar ~" will capture "~".
    /// </summary>
    public static readonly Regex EscapeChar = new("EscapeChar (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ExitOnForwardFailure" directive and capture its value.
    /// Example: "ExitOnForwardFailure yes" will capture "yes".
    /// </summary>
    public static readonly Regex ExitOnForwardFailure = new("ExitOnForwardFailure (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ForwardAgent" directive and capture its value.
    /// Example: "ForwardAgent yes" will capture "yes".
    /// </summary>
    public static readonly Regex ForwardAgent = new("ForwardAgent (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ForwardX11" directive and capture its value.
    /// Example: "ForwardX11 yes" will capture "yes".
    /// </summary>
    public static readonly Regex ForwardX11 = new("ForwardX11 (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ForwardX11Trusted" directive and capture its value.
    /// Example: "ForwardX11Trusted yes" will capture "yes".
    /// </summary>
    public static readonly Regex ForwardX11Trusted = new("ForwardX11Trusted (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GatewayPorts" directive and capture its value.
    /// Example: "GatewayPorts yes" will capture "yes".
    /// </summary>
    public static readonly Regex GatewayPorts = new("GatewayPorts (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GlobalKnownHostsFile" directive and capture its value.
    /// Example: "GlobalKnownHostsFile /etc/ssh/ssh_known_hosts" will capture "/etc/ssh/ssh_known_hosts".
    /// </summary>
    public static readonly Regex GlobalKnownHostsFilePorts = new("GlobalKnownHostsFile (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPIAuthentication" directive and capture its value.
    /// Example: "GSSAPIAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex GssApiAuthentication = new("GSSAPIAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPIKeyExchange" directive and capture its value.
    /// Example: "GSSAPIKeyExchange yes" will capture "yes".
    /// </summary>
    public static readonly Regex GssApiKeyExchange = new("GSSAPIKeyExchange (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPIClientIdentity" directive and capture its value.
    /// Example: "GSSAPIClientIdentity user@EXAMPLE.COM" will capture "user@EXAMPLE.COM".
    /// </summary>
    public static readonly Regex GssApiClientIdentity = new("GSSAPIClientIdentity (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPIDelegateCredentials" directive and capture its value.
    /// Example: "GSSAPIDelegateCredentials yes" will capture "yes".
    /// </summary>
    public static readonly Regex GssApiDelegateCredentials = new("GSSAPIDelegateCredentials (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPIRenewalForcesRekey" directive and capture its value.
    /// Example: "GSSAPIRenewalForcesRekey yes" will capture "yes".
    /// </summary>
    public static readonly Regex GssApiRenewalForcesRekey = new("GSSAPIRenewalForcesRekey (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "GSSAPITrustDns" directive and capture its value.
    /// Example: "GSSAPITrustDns yes" will capture "yes".
    /// </summary>
    public static readonly Regex GssApiTrustDns = new("GSSAPITrustDns (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "HashKnownHosts" directive and capture its value.
    /// Example: "HashKnownHosts yes" will capture "yes".
    /// </summary>
    public static readonly Regex HashKnownHosts = new("HashKnownHosts (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "HostbasedAuthentication" directive and capture its value.
    /// Example: "HostbasedAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex HostbasedAuthentication = new("HostbasedAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "HostKeyAlgorithms" directive and capture its value.
    /// Example: "HostKeyAlgorithms rsa-sha2-256" will capture "rsa-sha2-256".
    /// </summary>
    public static readonly Regex HostKeyAlgorithms = new("HostKeyAlgorithms (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "HostKeyAlias" directive and capture its value.
    /// Example: "HostKeyAlias myhostalias" will capture "myhostalias".
    /// </summary>
    public static readonly Regex HostKeyAlias = new("HostKeyAlias (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "HostName" directive and capture its value.
    /// Example: "HostName example.com" will capture "example.com".
    /// </summary>
    public static readonly Regex HostName = new("HostName (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "IdentitiesOnly" directive and capture its value.
    /// Example: "IdentitiesOnly yes" will capture "yes".
    /// </summary>
    public static readonly Regex IdentitiesOnly = new("IdentitiesOnly (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "IdentityFile" directive and capture its value.
    /// Example: "IdentityFile ~/.ssh/id_rsa" will capture "~/.ssh/id_rsa".
    /// </summary>
    public static readonly Regex IdentityFile = new("IdentityFile (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "KbdInteractiveAuthentication" directive and capture its value.
    /// Example: "KbdInteractiveAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex KbdInteractiveAuthentication = new("KbdInteractiveAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "KbdInteractiveDevices" directive and capture its value.
    /// Example: "KbdInteractiveDevices pam" will capture "pam".
    /// </summary>
    public static readonly Regex KbdInteractiveDevices = new("KbdInteractiveDevices (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "LocalCommand" directive and capture its value.
    /// Example: "LocalCommand echo 'Welcome!'" will capture "echo 'Welcome!'".
    /// </summary>
    public static readonly Regex LocalCommand = new("LocalCommand (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "LocalForward" directive and capture its value.
    /// Example: "LocalForward 8080:remote.com:80" will capture "8080:remote.com:80".
    /// </summary>
    public static readonly Regex LocalForward = new("LocalForward (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "LogLevel" directive and capture its value.
    /// Example: "LogLevel INFO" will capture "INFO".
    /// </summary>
    public static readonly Regex LogLevel = new("LogLevel (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "MACs" directive and capture its value.
    /// Example: "MACs hmac-sha2-256" will capture "hmac-sha2-256".
    /// </summary>
    public static readonly Regex Macs = new("MACs (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "NoHostAuthenticationForLocalhost" directive and capture its value.
    /// Example: "NoHostAuthenticationForLocalhost yes" will capture "yes".
    /// </summary>
    public static readonly Regex NoHostAuthenticationForLocalhost = new("NoHostAuthenticationForLocalhost (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "NumberOfPasswordPrompts" directive and capture its value.
    /// Example: "NumberOfPasswordPrompts 2" will capture "2".
    /// </summary>
    public static readonly Regex NumberOfPasswordPrompts = new("NumberOfPasswordPrompts (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "PasswordAuthentication" directive and capture its value.
    /// Example: "PasswordAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex PasswordAuthentication = new("PasswordAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "PermitLocalCommand" directive and capture its value.
    /// Example: "PermitLocalCommand yes" will capture "yes".
    /// </summary>
    public static readonly Regex PermitLocalCommand = new("PermitLocalCommand (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Port" directive and capture its value.
    /// Example: "Port 22" will capture "22".
    /// </summary>
    public static readonly Regex Port = new("Port (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "PreferredAuthentications" directive and capture its value.
    /// Example: "PreferredAuthentications publickey" will capture "publickey".
    /// </summary>
    public static readonly Regex PreferredAuthentications = new("PreferredAuthentications (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Protocol" directive and capture its value.
    /// Example: "Protocol 2" will capture "2".
    /// </summary>
    public static readonly Regex Protocol = new("Protocol (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ProxyCommand" directive and capture its value.
    /// Example: "ProxyCommand nc -x proxy:port %h %p" will capture "nc -x proxy:port %h %p".
    /// </summary>
    public static readonly Regex ProxyCommand = new("ProxyCommand (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "PubkeyAuthentication" directive and capture its value.
    /// Example: "PubkeyAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex PubkeyAuthentication = new("PubkeyAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "RekeyLimit" directive and capture its value.
    /// Example: "RekeyLimit 1G" will capture "1G".
    /// </summary>
    public static readonly Regex RekeyLimit = new("RekeyLimit (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "RemoveForward" directive and capture its value.
    /// Example: "RemoveForward 8080" will capture "8080".
    /// </summary>
    public static readonly Regex RemoveForward = new("RemoveForward (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "RhostsRsaAuthentication" directive and capture its value.
    /// Example: "RhostsRsaAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex RhostsRsaAuthentication = new("RhostsRsaAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "RsaAuthentication" directive and capture its value.
    /// Example: "RsaAuthentication yes" will capture "yes".
    /// </summary>
    public static readonly Regex RsaAuthentication = new("RsaAuthentication (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "SendEnv" directive and capture its value.
    /// Example: "SendEnv yes" will capture "yes".
    /// </summary>
    public static readonly Regex SendEnv = new("SendEnv (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ServerAliveCountMax" directive and capture its value.
    /// Example: "ServerAliveCountMax 3" will capture "3".
    /// </summary>
    public static readonly Regex ServerAliveCountMax = new("ServerAliveCountMax (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "ServerAliveInterval" directive and capture its value.
    /// Example: "ServerAliveInterval 60" will capture "60".
    /// </summary>
    public static readonly Regex ServerAliveInterval = new("ServerAliveInterval (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "SmartcardDevice" directive and capture its value.
    /// Example: "SmartcardDevice /dev/smartcard" will capture "/dev/smartcard".
    /// </summary>
    public static readonly Regex SmartcardDevice = new("SmartcardDevice (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "StrictHostKeyChecking" directive and capture its value.
    /// Example: "StrictHostKeyChecking yes" will capture "yes".
    /// </summary>
    public static readonly Regex StrictHostKeyChecking = new("StrictHostKeyChecking (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "TcpKeeAlive" directive and capture its value.
    /// Example: "TcpKeeAlive yes" will capture "yes".
    /// </summary>
    public static readonly Regex TcpKeeAlive = new("TcpKeeAlive (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "Tunnel" directive and capture its value.
    /// Example: "Tunnel yes" will capture "yes".
    /// </summary>
    public static readonly Regex Tunnel = new("Tunnel (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "TunnelDevice" directive and capture its value.
    /// Example: "TunnelDevice tun0" will capture "tun0".
    /// </summary>
    public static readonly Regex TunnelDevice = new("TunnelDevice (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "UsePrivilegedPort" directive and capture its value.
    /// Example: "UsePrivilegedPort yes" will capture "yes".
    /// </summary>
    public static readonly Regex UsePrivilegedPort = new("UsePrivilegedPort (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "User" directive and capture its value.
    /// Example: "User alice" will capture "alice".
    /// </summary>
    public static readonly Regex User = new("User (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "UserKnownHostsFile" directive and capture its value.
    /// Example: "UserKnownHostsFile ~/.ssh/known_hosts" will capture "~/.ssh/known_hosts".
    /// </summary>
    public static readonly Regex UserKnownHostsFile = new("UserKnownHostsFile (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "VerifyHostKeyDns" directive and capture its value.
    /// Example: "VerifyHostKeyDns yes" will capture "yes".
    /// </summary>
    public static readonly Regex VerifyHostKeyDns = new("VerifyHostKeyDns (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "VisualHostKey" directive and capture its value.
    /// Example: "VisualHostKey yes" will capture "yes".
    /// </summary>
    public static readonly Regex VisualHostKey = new("VisualHostKey (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Compiled regex to match the "XAuthLocation" directive and capture its value.
    /// Example: "XAuthLocation /usr/bin/xauth" will capture "/usr/bin/xauth".
    /// </summary>
    public static readonly Regex XAuthLocation = new("XAuthLocation (.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Maps each SSH config directive regex to a builder action that sets the corresponding property on <see cref="SshConfigHostBuilder"/>.
    /// Example: The regex for "HostName" maps to an action that calls <c>builder.HostName(value)</c>.
    /// </summary>
    public static readonly IReadOnlyDictionary<Regex, Action<SshConfigHostBuilder, Match>> Instructions = new Dictionary<Regex, Action<SshConfigHostBuilder, Match>>()
    {
        { Name, (builder, s) => { builder.Name(s.Groups[1].Value); } },
        { AddressFamily, (builder, s) => { builder.AddressFamily(Enum.Parse<AddressFamilyEnum>(s.Groups[1].Value)); } },
        { BatchMode, (builder, s) => { builder.BatchMode(s.Groups[1].Value); } },
        { BindAddress, (builder, s) => { builder.BindAddress(s.Groups[1].Value); } },
        { ChallengeResponseAuthentication, (builder, s) => { builder.ChallengeResponseAuthentication(s.Groups[1].Value); } },
        { CheckHostIp, (builder, s) => { builder.CheckHostIp(s.Groups[1].Value); } },
        { Cipher, (builder, s) => { builder.Cipher(s.Groups[1].Value); } },
        { Ciphers, (builder, s) => { builder.Ciphers(s.Groups[1].Value.Split(" ").ToList()); } },
        { ClearAllForwardings, (builder, s) => { builder.ClearAllForwardings(s.Groups[1].Value == SshConfigHostStrings.YesString ? true : false); } },
        { Compression, (builder, s) => { builder.Compression(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { CompressionLevel, (builder, s) => { builder.CompressionLevel(int.Parse(s.Groups[1].Value)); } },
        { ConnectionAttempts, (builder, s) => { builder.ConnectionAttempts(int.Parse(s.Groups[1].Value)); } },
        { ConnectTimeout, (builder, s) => { builder.ConnectionTimeout(int.Parse(s.Groups[1].Value)); } },
        { ControlMaster, (builder, s) => { builder.ControlMaster(new ControlMasterEnumReader().Read(s.Groups[1].Value)); } },
        { ControlPath, (builder, s) => { builder.ControlPath(s.Groups[1].Value); } },
        { DynamicForward, (builder, s) => { builder.DynamicForward(s.Groups[1].Value); } },
        { EnableSshKeysign, (builder, s) => { builder.EnableSshKeysign(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { EscapeChar, (builder, s) => { builder.EscapeChar(s.Groups[1].Value); } },
        { ExitOnForwardFailure, (builder, s) => { builder.ExitOnForwardFailure(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { ForwardAgent, (builder, s) => { builder.ForwardAgent(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { ForwardX11, (builder, s) => { builder.ForwardX11(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { ForwardX11Trusted, (builder, s) => { builder.ForwardX11Trusted(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { GatewayPorts, (builder, s) => { builder.GatewayPorts(s.Groups[1].Value); } },
        { GlobalKnownHostsFilePorts, (builder, s) => { builder.GlobalKnownHostsFile(s.Groups[1].Value); } },
        { GssApiAuthentication, (builder, s) => { builder.GssApiAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { GssApiKeyExchange, (builder, s) => { builder.GssApiKeyExchange(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { GssApiClientIdentity, (builder, s) => { builder.GssApiClientIdentity(s.Groups[1].Value); } },
        { GssApiDelegateCredentials, (builder, s) => { builder.GssApiDelegateCredentials(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { GssApiRenewalForcesRekey, (builder, s) => { builder.GssApiRenewalForcesRekey(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { GssApiTrustDns, (builder, s) => { builder.GssApiTrustDns(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { HashKnownHosts, (builder, s) => { builder.HashKnownHosts(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { HostbasedAuthentication, (builder, s) => { builder.HostbasedAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { HostKeyAlgorithms, (builder, s) => { builder.HostKeyAlgorithms(s.Groups[1].Value); } },
        { HostKeyAlias, (builder, s) => { builder.HostKeyAlias(s.Groups[1].Value); } },
        { HostName, (builder, s) => { builder.HostName(s.Groups[1].Value); } },
        { IdentitiesOnly, (builder, s) => { builder.IdentitiesOnly(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { IdentityFile, (builder, s) => { builder.IdentityFile(s.Groups[1].Value); } },
        { KbdInteractiveAuthentication, (builder, s) => { builder.KbdInteractiveAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { KbdInteractiveDevices, (builder, s) => { builder.KbdInteractiveDevices(s.Groups[1].Value); } },
        { LocalCommand, (builder, s) => { builder.LocalCommand(s.Groups[1].Value); } },
        { LocalForward, (builder, s) => { builder.LocalForward(s.Groups[1].Value); } },
        { LogLevel, (builder, s) => { builder.LogLevel(s.Groups[1].Value); } },
        { Macs, (builder, s) => { builder.Macs(s.Groups[1].Value); } },
        { NoHostAuthenticationForLocalhost, (builder, s) => { builder.NoHostAuthenticationForLocalhost(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { NumberOfPasswordPrompts, (builder, s) => { builder.NumberOfPasswordAttempts(int.Parse(s.Groups[1].Value)); } },
        { PasswordAuthentication, (builder, s) => { builder.PasswordAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { PermitLocalCommand, (builder, s) => { builder.PermitLocalCommand(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { Port, (builder, s) => { builder.Port(int.Parse(s.Groups[1].Value)); } },
        { PreferredAuthentications, (builder, s) => { builder.PreferredAuthentication(s.Groups[1].Value); } },
        { Protocol, (builder, s) => { builder.Protocol(int.Parse(s.Groups[1].Value)); } },
        { ProxyCommand, (builder, s) => { builder.ProxyCommand(s.Groups[1].Value); } },
        { PubkeyAuthentication, (builder, s) => { builder.PubkeyAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { RekeyLimit, (builder, s) => { builder.RekeyLimit(s.Groups[1].Value); } },
        { RemoveForward, (builder, s) => { builder.RemoveForward(s.Groups[1].Value); } },
        { RhostsRsaAuthentication, (builder, s) => { builder.RHostRsaAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { RsaAuthentication, (builder, s) => { builder.RsaAuthentication(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { SendEnv, (builder, s) => { builder.SendEnv(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { ServerAliveCountMax, (builder, s) => { builder.ServerAliveCountMax(int.Parse(s.Groups[1].Value)); } },
        { ServerAliveInterval, (builder, s) => { builder.ServerAliveInterval(int.Parse(s.Groups[1].Value)); } },
        { SmartcardDevice, (builder, s) => { builder.SmardcardDevice(s.Groups[1].Value); } },
        { StrictHostKeyChecking, (builder, s) => { builder.StrictHostKeyChecking(new YesNoAskEnumReader().Read(s.Groups[1].Value)); } },
        { TcpKeeAlive, (builder, s) => { builder.TcpKeepAlive(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { Tunnel, (builder, s) => { builder.Tunnel(new TunnelEnumReader().Read(s.Groups[1].Value)); } },
        { TunnelDevice, (builder, s) => { builder.TunnelDevice(s.Groups[1].Value); } },
        { UsePrivilegedPort, (builder, s) => { builder.UsePrivilegedPort(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { User, (builder, s) => { builder.User(s.Groups[1].Value); } },
        { UserKnownHostsFile, (builder, s) => { builder.UserKnownHostsFile(s.Groups[1].Value); } },
        { VerifyHostKeyDns, (builder, s) => { builder.VerifyHostKeyDns(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { VisualHostKey, (builder, s) => { builder.VisualHostKey(s.Groups[1].Value == SshConfigHostStrings.YesString); } },
        { XAuthLocation, (builder, s) => { builder.XAuthLocation(s.Groups[1].Value); } }
    }.ToImmutableDictionary();

    /// <summary>
    /// Parses a collection of SSH configuration file lines and returns a corresponding <see cref="SshConfigHost"/>
    /// instance.
    /// </summary>
    /// <param name="lines">An enumerable collection of strings representing lines from an SSH configuration file. Each line should follow
    /// the standard SSH config syntax.</param>
    /// <returns>An <see cref="SshConfigHost"/> object that represents the parsed configuration. Returns <c>null</c> if the input
    /// does not yield a valid host configuration.</returns>
    public SshConfigHost Read(IEnumerable<string> lines)
    {
        return BuildFromLines(lines).Build().Success<SshConfigHost>();
    }

    /// <summary>
    /// Parses a collection of SSH configuration lines and builds an <see cref="SshConfigHostBuilder"/> representing the
    /// host configuration.
    /// </summary>
    /// <param name="lines">An enumerable collection of strings, each representing a line from an SSH configuration file. Lines should be
    /// formatted according to SSH config syntax.</param>
    /// <returns>An <see cref="SshConfigHostBuilder"/> populated with the configuration extracted from the provided lines.</returns>
    private SshConfigHostBuilder BuildFromLines(IEnumerable<string> lines)
    {
        var sshConfigHostBuilder = new SshConfigHostBuilder();

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimStart();

            foreach (var instructionReader in Instructions)
            {
                var matches = instructionReader.Key.Match(trimmedLine);
                if (matches.Success)
                {
                    instructionReader.Value(sshConfigHostBuilder, matches);
                    continue;
                }
            }
        }

        return sshConfigHostBuilder;
    }
}