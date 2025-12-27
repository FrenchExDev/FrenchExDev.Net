using FrenchExDev.Net.Ssh.Config.Testing;
using Shouldly;

namespace FrenchExDev.Net.Ssh.Config.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of SSH host entry configuration using HostEntryTester.
/// </summary>
public class SshConfigHostTests
{
    /// <summary>
    /// Verifies that the Name property of a host entry can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    public void Can_Set_Name(string name) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name(name),
        assert: host => host.Name.ShouldBe(name));

    /// <summary>
    /// Verifies that the AddressFamily property can be set to each supported enum value.
    /// </summary>
    [Theory]
    [InlineData(AddressFamilyEnum.All)]
    [InlineData(AddressFamilyEnum.Inet)]
    [InlineData(AddressFamilyEnum.Inet6)]
    public void Can_Set_AddressFamily(AddressFamilyEnum addressFamilyEnum) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").AddressFamily(addressFamilyEnum),
        assert: host => host.AddressFamily.ShouldBe(addressFamilyEnum));

    /// <summary>
    /// Verifies that the BatchMode property can be set to "yes" or "no".
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_BatchMode(string batchMode) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").BatchMode(batchMode),
        assert: host => host.BatchMode.ShouldBe(batchMode));

    /// <summary>
    /// Verifies that the BindAddress property can be set to a specific IP address.
    /// </summary>
    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("0.0.0.0")]
    public void Can_Set_BindAddress(string bindAddress) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").BindAddress(bindAddress),
        assert: host => host.BindAddress.ShouldBe(bindAddress));

    /// <summary>
    /// Verifies that the ChallengeResponseAuthentication property can be set to "yes" or "no".
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_ChallengeResponseAuthentication(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ChallengeResponseAuthentication(value),
        assert: host => host.ChallengeResponseAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the CheckHostIp property can be set to "yes" or "no".
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_CheckHostIp(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").CheckHostIp(value),
        assert: host => host.CheckHostIp.ShouldBe(value));

    /// <summary>
    /// Verifies that the Cipher property can be set to a specific cipher string.
    /// </summary>
    [Theory]
    [InlineData("aes256-ctr")]
    [InlineData("chacha20-poly1305@openssh.com")]
    public void Can_Set_Cipher(string cipher) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").Cipher(cipher),
        assert: host => host.Cipher.ShouldBe(cipher));

    /// <summary>
    /// Verifies that the Ciphers property can be set to a list of cipher strings.
    /// </summary>
    [Theory]
    [InlineData(["aes256-ctr", "aes192-ctr"])]
    [InlineData(["chacha20-poly1305@openssh.com"])]
    public void Can_Set_Ciphers(params string[] ciphers) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").Ciphers([.. ciphers]),
        assert: host => host.Ciphers.ShouldBe(ciphers));

    /// <summary>
    /// Verifies that the ClearAllForwardings property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ClearAllForwardings(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ClearAllForwardings(value),
        assert: host => host.ClearAllForwardings.ShouldBe(value));

    /// <summary>
    /// Verifies that the Compression property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_Compression(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").Compression(value),
        assert: host => host.Compression.ShouldBe(value));

    /// <summary>
    /// Verifies that the CompressionLevel property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    public void Can_Set_CompressionLevel(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").CompressionLevel(value),
        assert: host => host.CompressionLevel.ShouldBe(value));

    /// <summary>
    /// Verifies that the ConnectionAttempts property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Can_Set_ConnectionAttempts(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ConnectionAttempts(value),
        assert: host => host.ConnectionAttempts.ShouldBe(value));

    /// <summary>
    /// Verifies that the ConnectTimeout property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(10)]
    [InlineData(60)]
    public void Can_Set_ConnectTimeout(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ConnectionTimeout(value),
        assert: host => host.ConnectTimeout.ShouldBe(value));

    /// <summary>
    /// Verifies that the ControlPath property can be set to a specific path string.
    /// </summary>
    [Theory]
    [InlineData("/tmp/cm_socket")]
    [InlineData("~/.ssh/cm_socket")]
    public void Can_Set_ControlPath(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ControlPath(value),
        assert: host => host.ControlPath.ShouldBe(value));

    /// <summary>
    /// Verifies that the ControlMaster property can be set to a ControlMasterEnum value.
    /// </summary>
    [Theory]
    [InlineData(ControlMasterEnum.No)]
    public void Can_Set_ControlMaster(ControlMasterEnum value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ControlMaster(value),
        assert: host => host.ControlMaster.ShouldBe(value));

    /// <summary>
    /// Verifies that the DynamicForward property can be set to a specific port string.
    /// </summary>
    [Theory]
    [InlineData("8080")]
    [InlineData("1080")]
    public void Can_Set_DynamicForward(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").DynamicForward(value),
        assert: host => host.DynamicForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the EnableSshKeysign property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_EnableSshKeysign(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").EnableSshKeysign(value),
        assert: host => host.EnableSshKeysign.ShouldBe(value));

    /// <summary>
    /// Verifies that the EscapeChar property can be set to a specific character.
    /// </summary>
    [Theory]
    [InlineData("~")]
    [InlineData("^")]
    public void Can_Set_EscapeChar(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").EscapeChar(value),
        assert: host => host.EscapeChar.ShouldBe(value));

    /// <summary>
    /// Verifies that the ExitOnForwardFailure property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ExitOnForwardFailure(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ExitOnForwardFailure(value),
        assert: host => host.ExitOnForwardFailure.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardAgent property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardAgent(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ForwardAgent(value),
        assert: host => host.ForwardAgent.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardX11 property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardX11(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ForwardX11(value),
        assert: host => host.ForwardX11.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardX11Trusted property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardX11Trusted(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ForwardX11Trusted(value),
        assert: host => host.ForwardX11Trusted.ShouldBe(value));

    /// <summary>
    /// Verifies that the GatewayPorts property can be set to a string value.
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_GatewayPorts(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GatewayPorts(value),
        assert: host => host.GatewayPorts.ShouldBe(value));

    /// <summary>
    /// Verifies that the GlobalKnownHostsFile property can be set to a file path.
    /// </summary>
    [Theory]
    [InlineData("/etc/ssh/ssh_known_hosts")]
    [InlineData("/tmp/known_hosts")]
    public void Can_Set_GlobalKnownHostsFile(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GlobalKnownHostsFile(value),
        assert: host => host.GlobalKnownHostsFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiAuthentication(value),
        assert: host => host.GssApiAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiKeyExchange property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiKeyExchange(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiKeyExchange(value),
        assert: host => host.GssApiKeyExchange.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiClientIdentity property can be set to a string value.
    /// </summary>
    [Theory]
    [InlineData("user@EXAMPLE.COM")]
    [InlineData("other@EXAMPLE.COM")]
    public void Can_Set_GssApiClientIdentity(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiClientIdentity(value),
        assert: host => host.GssApiClientIdentity.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiDelegateCredentials property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiDelegateCredentials(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiDelegateCredentials(value),
        assert: host => host.GssApiDelegateCredentials.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiRenewalForcesRekey property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiRenewalForcesRekey(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiRenewalForcesRekey(value),
        assert: host => host.GssApiRenewalForcesRekey.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiTrustDns property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiTrustDns(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").GssApiTrustDns(value),
        assert: host => host.GssApiTrustDns.ShouldBe(value));

    /// <summary>
    /// Verifies that the HashKnownHosts property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_HashKnownHosts(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").HashKnownHosts(value),
        assert: host => host.HashKnownHosts.ShouldBe(value));

    /// <summary>
    /// Verifies that the HostbasedAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_HostbasedAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").HostbasedAuthentication(value),
        assert: host => host.HostbasedAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the IdentityFile property can be set to a file path.
    /// </summary>
    [Theory]
    [InlineData("~/.ssh/id_rsa")]
    [InlineData("/tmp/id_ed25519")]
    public void Can_Set_IdentityFile(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").IdentityFile(value),
        assert: host => host.IdentityFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the KbdInteractiveAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_KbdInteractiveAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").KbdInteractiveAuthentication(value),
        assert: host => host.KbdInteractiveAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the KbdInteractiveDevices property can be set to a string value.
    /// </summary>
    [Theory]
    [InlineData("pam")]
    [InlineData("keyboard-interactive")]
    public void Can_Set_KbdInteractiveDevices(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").KbdInteractiveDevices(value),
        assert: host => host.KbdInteractiveDevices.ShouldBe(value));

    /// <summary>
    /// Verifies that the LocalCommand property can be set to a command string.
    /// </summary>
    [Theory]
    [InlineData("echo 'Welcome!'")]
    [InlineData("ls -la")]
    public void Can_Set_LocalCommand(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").LocalCommand(value),
        assert: host => host.LocalCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the LocalForward property can be set to a forwarding specification.
    /// </summary>
    [Theory]
    [InlineData("8080:remote.com:80")]
    [InlineData("2222:other.com:22")]
    public void Can_Set_LocalForward(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").LocalForward(value),
        assert: host => host.LocalForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the LogLevel property can be set to a log level string.
    /// </summary>
    [Theory]
    [InlineData("INFO")]
    [InlineData("DEBUG")]
    public void Can_Set_LogLevel(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").LogLevel(value),
        assert: host => host.LogLevel.ShouldBe(value));

    /// <summary>
    /// Verifies that the NumberOfPasswordPrompts property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Can_Set_NumberOfPasswordPrompts(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").NumberOfPasswordAttempts(value),
        assert: host => host.NumberOfPasswordPrompts.ShouldBe(value));

    /// <summary>
    /// Verifies that the PasswordAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PasswordAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").PasswordAuthentication(value),
        assert: host => host.PasswordAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the PermitLocalCommand property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PermitLocalCommand(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").PermitLocalCommand(value),
        assert: host => host.PermitLocalCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the PreferredAuthentications property can be set to a list of authentication methods.
    /// </summary>
    [Theory]
    [InlineData(["publickey", "password"])]
    [InlineData(["keyboard-interactive"])]
    public void Can_Set_PreferredAuthentications(params string[] value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => { builder.Name("foo"); foreach (var v in value) builder.PreferredAuthentication(v); },
        assert: host => host.PreferredAuthentications.ShouldBe(value));

    /// <summary>
    /// Verifies that the ProxyCommand property can be set to a command string.
    /// </summary>
    [Theory]
    [InlineData("nc -x proxy:port %h %p")]
    [InlineData("socat TCP:host:port")]
    public void Can_Set_ProxyCommand(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ProxyCommand(value),
        assert: host => host.ProxyCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the PubkeyAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PubkeyAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").PubkeyAuthentication(value),
        assert: host => host.PubkeyAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the RekeyLimit property can be set to a string value.
    /// </summary>
    [Theory]
    [InlineData("1G")]
    [InlineData("500M")]
    public void Can_Set_RekeyLimit(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").RekeyLimit(value),
        assert: host => host.RekeyLimit.ShouldBe(value));

    /// <summary>
    /// Verifies that the RemoveForward property can be set to a string value.
    /// </summary>
    [Theory]
    [InlineData("8080")]
    [InlineData("2222")]
    public void Can_Set_RemoveForward(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").RemoveForward(value),
        assert: host => host.RemoveForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the RhostsRsaAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_RhostsRsaAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").RHostRsaAuthentication(value),
        assert: host => host.RhostsRsaAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the RsaAuthentication property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_RsaAuthentication(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").RsaAuthentication(value),
        assert: host => host.RsaAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the SendEnv property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_SendEnv(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").SendEnv(value),
        assert: host => host.SendEnv.ShouldBe(value));

    /// <summary>
    /// Verifies that the ServerAliveCountMax property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Can_Set_ServerAliveCountMax(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ServerAliveCountMax(value),
        assert: host => host.ServerAliveCountMax.ShouldBe(value));

    /// <summary>
    /// Verifies that the ServerAliveInterval property can be set to an integer value.
    /// </summary>
    [Theory]
    [InlineData(60)]
    [InlineData(120)]
    public void Can_Set_ServerAliveInterval(int value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").ServerAliveInterval(value),
        assert: host => host.ServerAliveInterval.ShouldBe(value));

    /// <summary>
    /// Verifies that the SmartcardDevice property can be set to a device path.
    /// </summary>
    [Theory]
    [InlineData("/dev/smartcard")]
    [InlineData("/tmp/smartcard")]
    public void Can_Set_SmartcardDevice(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").SmardcardDevice(value),
        assert: host => host.SmartcardDevice.ShouldBe(value));

    /// <summary>
    /// Verifies that the TcpKeeAlive property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_TcpKeeAlive(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").TcpKeepAlive(value),
        assert: host => host.TcpKeeAlive.ShouldBe(value));

    /// <summary>
    /// Verifies that the TunnelDevice property can be set to a device string.
    /// </summary>
    [Theory]
    [InlineData("tun0")]
    [InlineData("tun1")]
    public void Can_Set_TunnelDevice(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").TunnelDevice(value),
        assert: host => host.TunnelDevice.ShouldBe(value));

    /// <summary>
    /// Verifies that the UsePrivilegedPort property can be set to true or false.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_UsePrivilegedPort(bool value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").UsePrivilegedPort(value),
        assert: host => host.UsePrivilegedPort.ShouldBe(value));

    /// <summary>
    /// Verifies that the UserKnownHostsFile property can be set to a file path.
    /// </summary>
    [Theory]
    [InlineData("~/.ssh/known_hosts")]
    [InlineData("/tmp/known_hosts")]
    public void Can_Set_UserKnownHostsFile(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").UserKnownHostsFile(value),
        assert: host => host.UserKnownHostsFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the XAuthLocation property can be set to a file path.
    /// </summary>
    [Theory]
    [InlineData("/usr/bin/xauth")]
    [InlineData("/tmp/xauth")]
    public void Can_Set_XAuthLocation(string value) => SshConfigTesting.HostEntryTester.Valid(
        body: builder => builder.Name("foo").XAuthLocation(value),
        assert: host => host.XAuthLocation.ShouldBe(value));
}
