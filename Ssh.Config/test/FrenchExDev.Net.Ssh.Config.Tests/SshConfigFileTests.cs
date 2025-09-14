using FrenchExDev.Net.Ssh.Config.Testing;
using Shouldly;

namespace FrenchExDev.Net.Ssh.Config.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of SSH configuration file handling, specifically related to address
/// family settings and other host configuration options.
/// </summary>
/// <remarks>
/// This test class uses parameterized tests to ensure that SSH configuration files correctly process different host options.
/// The tests validate that the configuration builder and resulting file objects reflect the expected values for each scenario.
/// </remarks>
public class SshConfigFileTests
{
    /// <summary>
    /// Verifies that the AddressFamily property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(AddressFamilyEnum.All)]
    [InlineData(AddressFamilyEnum.Inet)]
    [InlineData(AddressFamilyEnum.Inet6)]
    public void Can_Set_AddressFamily(AddressFamilyEnum addressFamilyEnum) => SshConfigTesting.FileTestter.Valid(
            body: builder =>
            {
                builder.Host(hostBuilder =>
                {
                    hostBuilder.Name("foo");
                    hostBuilder.AddressFamily(addressFamilyEnum);
                });
            },
            assert: file =>
            {
                file.Hosts.Count.ShouldBe(1);
                file.Hosts[0].AddressFamily.ShouldBe(addressFamilyEnum);
            });

    /// <summary>
    /// Verifies that the HostName property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("example.com")]
    [InlineData("192.168.1.1")]
    public void Can_Set_HostName(string hostName) => SshConfigTesting.FileTestter.Valid(
        body: builder =>
        {
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("foo");
                hostBuilder.HostName(hostName);
            });
        },
        assert: file =>
        {
            file.Hosts.Count.ShouldBe(1);
            file.Hosts[0].HostName.ShouldBe(hostName);
        });

    /// <summary>
    /// Verifies that the Port property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(22)]
    [InlineData(2222)]
    public void Can_Set_Port(int port) => SshConfigTesting.FileTestter.Valid(
        body: builder =>
        {
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("foo");
                hostBuilder.Port(port);
            });
        },
        assert: file =>
        {
            file.Hosts.Count.ShouldBe(1);
            file.Hosts[0].Port.ShouldBe(port);
        });

    /// <summary>
    /// Verifies that the User property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("alice")]
    [InlineData("bob")]
    public void Can_Set_User(string user) => SshConfigTesting.FileTestter.Valid(
        body: builder =>
        {
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("foo");
                hostBuilder.User(user);
            });
        },
        assert: file =>
        {
            file.Hosts.Count.ShouldBe(1);
            file.Hosts[0].User.ShouldBe(user);
        });

    /// <summary>
    /// Verifies that the Compression property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_Compression(bool compression) => SshConfigTesting.FileTestter.Valid(
        body: builder =>
        {
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("foo");
                hostBuilder.Compression(compression);
            });
        },
        assert: file =>
        {
            file.Hosts.Count.ShouldBe(1);
            file.Hosts[0].Compression.ShouldBe(compression);
        });

    /// <summary>
    /// Verifies that multiple hosts can be configured in a single SSH config file.
    /// </summary>
    [Fact]
    public void Can_Configure_Multiple_Hosts() => SshConfigTesting.FileTestter.Valid(
        body: builder =>
        {
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("foo");
                hostBuilder.Port(22);
            });
            builder.Host(hostBuilder =>
            {
                hostBuilder.Name("bar");
                hostBuilder.Port(2222);
            });
        },
        assert: file =>
        {
            file.Hosts.Count.ShouldBe(2);
            file.Hosts[0].Name.ShouldBe("foo");
            file.Hosts[0].Port.ShouldBe(22);
            file.Hosts[1].Name.ShouldBe("bar");
            file.Hosts[1].Port.ShouldBe(2222);
        });

    /// <summary>
    /// Verifies that the BatchMode property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_BatchMode(string batchMode) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.BatchMode(batchMode); }),
        assert: file => file.Hosts[0].BatchMode.ShouldBe(batchMode));

    /// <summary>
    /// Verifies that the BindAddress property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("0.0.0.0")]
    public void Can_Set_BindAddress(string bindAddress) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.BindAddress(bindAddress); }),
        assert: file => file.Hosts[0].BindAddress.ShouldBe(bindAddress));

    /// <summary>
    /// Verifies that the ChallengeResponseAuthentication property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_ChallengeResponseAuthentication(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ChallengeResponseAuthentication(value); }),
        assert: file => file.Hosts[0].ChallengeResponseAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the CheckHostIp property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_CheckHostIp(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.CheckHostIp(value); }),
        assert: file => file.Hosts[0].CheckHostIp.ShouldBe(value));

    /// <summary>
    /// Verifies that the Cipher property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("aes256-ctr")]
    [InlineData("chacha20-poly1305@openssh.com")]
    public void Can_Set_Cipher(string cipher) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.Cipher(cipher); }),
        assert: file => file.Hosts[0].Cipher.ShouldBe(cipher));

    /// <summary>
    /// Verifies that the Ciphers property of a host configuration can be set to the specified values.
    /// </summary>
    [Theory]
    [InlineData(["aes256-ctr", "aes192-ctr"])]
    [InlineData(["chacha20-poly1305@openssh.com"])]
    public void Can_Set_Ciphers(params string[] ciphers) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.Ciphers([.. ciphers]); }),
        assert: file => file.Hosts[0].Ciphers.ShouldBe(ciphers));

    /// <summary>
    /// Verifies that the ClearAllForwardings property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ClearAllForwardings(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ClearAllForwardings(value); }),
        assert: file => file.Hosts[0].ClearAllForwardings.ShouldBe(value));

    /// <summary>
    /// Verifies that the CompressionLevel property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    public void Can_Set_CompressionLevel(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.CompressionLevel(value); }),
        assert: file => file.Hosts[0].CompressionLevel.ShouldBe(value));

    /// <summary>
    /// Verifies that the ConnectionAttempts property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Can_Set_ConnectionAttempts(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ConnectionAttempts(value); }),
        assert: file => file.Hosts[0].ConnectionAttempts.ShouldBe(value));

    /// <summary>
    /// Verifies that the ConnectTimeout property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(10)]
    [InlineData(60)]
    public void Can_Set_ConnectTimeout(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ConnectionTimeout(value); }),
        assert: file => file.Hosts[0].ConnectTimeout.ShouldBe(value));

    /// <summary>
    /// Verifies that the ControlMaster property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(ControlMasterEnum.No)]
    public void Can_Set_ControlMaster(ControlMasterEnum value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ControlMaster(value); }),
        assert: file => file.Hosts[0].ControlMaster.ShouldBe(value));

    /// <summary>
    /// Verifies that the ControlPath property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("/tmp/cm_socket")]
    [InlineData("~/.ssh/cm_socket")]
    public void Can_Set_ControlPath(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ControlPath(value); }),
        assert: file => file.Hosts[0].ControlPath.ShouldBe(value));

    /// <summary>
    /// Verifies that the DynamicForward property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("8080")]
    [InlineData("1080")]
    public void Can_Set_DynamicForward(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.DynamicForward(value); }),
        assert: file => file.Hosts[0].DynamicForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the EnableSshKeysign property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_EnableSshKeysign(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.EnableSshKeysign(value); }),
        assert: file => file.Hosts[0].EnableSshKeysign.ShouldBe(value));

    /// <summary>
    /// Verifies that the EscapeChar property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("~")]
    [InlineData("^")]
    public void Can_Set_EscapeChar(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.EscapeChar(value); }),
        assert: file => file.Hosts[0].EscapeChar.ShouldBe(value));

    /// <summary>
    /// Verifies that the ExitOnForwardFailure property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ExitOnForwardFailure(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ExitOnForwardFailure(value); }),
        assert: file => file.Hosts[0].ExitOnForwardFailure.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardAgent property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardAgent(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ForwardAgent(value); }),
        assert: file => file.Hosts[0].ForwardAgent.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardX11 property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardX11(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ForwardX11(value); }),
        assert: file => file.Hosts[0].ForwardX11.ShouldBe(value));

    /// <summary>
    /// Verifies that the ForwardX11Trusted property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_ForwardX11Trusted(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ForwardX11Trusted(value); }),
        assert: file => file.Hosts[0].ForwardX11Trusted.ShouldBe(value));

    /// <summary>
    /// Verifies that the GatewayPorts property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    public void Can_Set_GatewayPorts(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GatewayPorts(value); }),
        assert: file => file.Hosts[0].GatewayPorts.ShouldBe(value));

    /// <summary>
    /// Verifies that the GlobalKnownHostsFile property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("/etc/ssh/ssh_known_hosts")]
    [InlineData("/tmp/known_hosts")]
    public void Can_Set_GlobalKnownHostsFile(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GlobalKnownHostsFile(value); }),
        assert: file => file.Hosts[0].GlobalKnownHostsFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiAuthentication(value); }),
        assert: file => file.Hosts[0].GssApiAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiKeyExchange property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiKeyExchange(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiKeyExchange(value); }),
        assert: file => file.Hosts[0].GssApiKeyExchange.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiClientIdentity property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("user@EXAMPLE.COM")]
    [InlineData("other@EXAMPLE.COM")]
    public void Can_Set_GssApiClientIdentity(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiClientIdentity(value); }),
        assert: file => file.Hosts[0].GssApiClientIdentity.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiDelegateCredentials property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiDelegateCredentials(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiDelegateCredentials(value); }),
        assert: file => file.Hosts[0].GssApiDelegateCredentials.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiRenewalForcesRekey property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiRenewalForcesRekey(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiRenewalForcesRekey(value); }),
        assert: file => file.Hosts[0].GssApiRenewalForcesRekey.ShouldBe(value));

    /// <summary>
    /// Verifies that the GssApiTrustDns property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_GssApiTrustDns(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.GssApiTrustDns(value); }),
        assert: file => file.Hosts[0].GssApiTrustDns.ShouldBe(value));

    /// <summary>
    /// Verifies that the HashKnownHosts property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_HashKnownHosts(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.HashKnownHosts(value); }),
        assert: file => file.Hosts[0].HashKnownHosts.ShouldBe(value));

    /// <summary>
    /// Verifies that the HostbasedAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_HostbasedAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.HostbasedAuthentication(value); }),
        assert: file => file.Hosts[0].HostbasedAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the HostKeyAlgorithms property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("rsa-sha2-256")]
    [InlineData("ecdsa-sha2-nistp256")]
    public void Can_Set_HostKeyAlgorithms(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.HostKeyAlgorithms(value); }),
        assert: file => file.Hosts[0].HostKeyAlgorithms.ShouldBe(value));

    /// <summary>
    /// Verifies that the HostKeyAlias property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("myhostalias")]
    [InlineData("otheralias")]
    public void Can_Set_HostKeyAlias(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.HostKeyAlias(value); }),
        assert: file => file.Hosts[0].HostKeyAlias.ShouldBe(value));

    /// <summary>
    /// Verifies that the IdentitiesOnly property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_IdentitiesOnly(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.IdentitiesOnly(value); }),
        assert: file => file.Hosts[0].IdentitiesOnly.ShouldBe(value));

    /// <summary>
    /// Verifies that the IdentityFile property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("~/.ssh/id_rsa")]
    [InlineData("/tmp/id_ed25519")]
    public void Can_Set_IdentityFile(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.IdentityFile(value); }),
        assert: file => file.Hosts[0].IdentityFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the KbdInteractiveAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_KbdInteractiveAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.KbdInteractiveAuthentication(value); }),
        assert: file => file.Hosts[0].KbdInteractiveAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the KbdInteractiveDevices property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("pam")]
    [InlineData("keyboard-interactive")]
    public void Can_Set_KbdInteractiveDevices(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.KbdInteractiveDevices(value); }),
        assert: file => file.Hosts[0].KbdInteractiveDevices.ShouldBe(value));

    /// <summary>
    /// Verifies that the LocalCommand property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("echo 'Welcome!'")]
    [InlineData("ls -la")]
    public void Can_Set_LocalCommand(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.LocalCommand(value); }),
        assert: file => file.Hosts[0].LocalCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the LocalForward property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("8080:remote.com:80")]
    [InlineData("2222:other.com:22")]
    public void Can_Set_LocalForward(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.LocalForward(value); }),
        assert: file => file.Hosts[0].LocalForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the LogLevel property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("INFO")]
    [InlineData("DEBUG")]
    public void Can_Set_LogLevel(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.LogLevel(value); }),
        assert: file => file.Hosts[0].LogLevel.ShouldBe(value));

    /// <summary>
    /// Verifies that the Macs property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("hmac-sha2-256")]
    [InlineData("hmac-sha2-512")]
    public void Can_Set_Macs(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.Macs(value); }),
        assert: file => file.Hosts[0].Macs.ShouldBe(value));

    /// <summary>
    /// Verifies that the NoHostAuthenticationForLocalhost property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_NoHostAuthenticationForLocalhost(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.NoHostAuthenticationForLocalhost(value); }),
        assert: file => file.Hosts[0].NoHostAuthenticationForLocalhost.ShouldBe(value));

    /// <summary>
    /// Verifies that the NumberOfPasswordPrompts property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Can_Set_NumberOfPasswordPrompts(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.NumberOfPasswordAttempts(value); }),
        assert: file => file.Hosts[0].NumberOfPasswordPrompts.ShouldBe(value));

    /// <summary>
    /// Verifies that the PasswordAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PasswordAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.PasswordAuthentication(value); }),
        assert: file => file.Hosts[0].PasswordAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the PermitLocalCommand property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PermitLocalCommand(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.PermitLocalCommand(value); }),
        assert: file => file.Hosts[0].PermitLocalCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the PreferredAuthentications property of a host configuration can be set to the specified values.
    /// </summary>
    [Theory]
    [InlineData("publickey", "password")]
    [InlineData("keyboard-interactive")]
    public void Can_Set_PreferredAuthentications(params string[] value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); foreach (var v in value) hb.PreferredAuthentication(v); }),
        assert: file => file.Hosts[0].PreferredAuthentications.ShouldBe(value));

    /// <summary>
    /// Verifies that the Protocol property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    public void Can_Set_Protocol(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.Protocol(value); }),
        assert: file => file.Hosts[0].Protocol.ShouldBe(value));

    /// <summary>
    /// Verifies that the ProxyCommand property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("nc -x proxy:port %h %p")]
    [InlineData("socat TCP:host:port")]
    public void Can_Set_ProxyCommand(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ProxyCommand(value); }),
        assert: file => file.Hosts[0].ProxyCommand.ShouldBe(value));

    /// <summary>
    /// Verifies that the PubkeyAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_PubkeyAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.PubkeyAuthentication(value); }),
        assert: file => file.Hosts[0].PubkeyAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the RekeyLimit property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("1G")]
    [InlineData("500M")]
    public void Can_Set_RekeyLimit(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.RekeyLimit(value); }),
        assert: file => file.Hosts[0].RekeyLimit.ShouldBe(value));

    /// <summary>
    /// Verifies that the RemoveForward property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("8080")]
    [InlineData("2222")]
    public void Can_Set_RemoveForward(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.RemoveForward(value); }),
        assert: file => file.Hosts[0].RemoveForward.ShouldBe(value));

    /// <summary>
    /// Verifies that the RhostsRsaAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_RhostsRsaAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.RHostRsaAuthentication(value); }),
        assert: file => file.Hosts[0].RhostsRsaAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the RsaAuthentication property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_RsaAuthentication(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.RsaAuthentication(value); }),
        assert: file => file.Hosts[0].RsaAuthentication.ShouldBe(value));

    /// <summary>
    /// Verifies that the SendEnv property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_SendEnv(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.SendEnv(value); }),
        assert: file => file.Hosts[0].SendEnv.ShouldBe(value));

    /// <summary>
    /// Verifies that the ServerAliveCountMax property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Can_Set_ServerAliveCountMax(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ServerAliveCountMax(value); }),
        assert: file => file.Hosts[0].ServerAliveCountMax.ShouldBe(value));

    /// <summary>
    /// Verifies that the ServerAliveInterval property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(60)]
    [InlineData(120)]
    public void Can_Set_ServerAliveInterval(int value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.ServerAliveInterval(value); }),
        assert: file => file.Hosts[0].ServerAliveInterval.ShouldBe(value));

    /// <summary>
    /// Verifies that the SmartcardDevice property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("/dev/smartcard")]
    [InlineData("/tmp/smartcard")]
    public void Can_Set_SmartcardDevice(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.SmardcardDevice(value); }),
        assert: file => file.Hosts[0].SmartcardDevice.ShouldBe(value));

    /// <summary>
    /// Verifies that the StrictHostKeyChecking property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(YesNoAskEnum.No)]
    public void Can_Set_StrictHostKeyChecking(YesNoAskEnum value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.StrictHostKeyChecking(value); }),
        assert: file => file.Hosts[0].StrictHostKeyChecking.ShouldBe(value));

    /// <summary>
    /// Verifies that the TcpKeeAlive property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_TcpKeeAlive(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.TcpKeepAlive(value); }),
        assert: file => file.Hosts[0].TcpKeeAlive.ShouldBe(value));

    /// <summary>
    /// Verifies that the Tunnel property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData(TunnelEnum.No)]
    public void Can_Set_Tunnel(TunnelEnum value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.Tunnel(value); }),
        assert: file => file.Hosts[0].Tunnel.ShouldBe(value));

    /// <summary>
    /// Verifies that the TunnelDevice property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("tun0")]
    [InlineData("tun1")]
    public void Can_Set_TunnelDevice(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.TunnelDevice(value); }),
        assert: file => file.Hosts[0].TunnelDevice.ShouldBe(value));

    /// <summary>
    /// Verifies that the UsePrivilegedPort property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_UsePrivilegedPort(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.UsePrivilegedPort(value); }),
        assert: file => file.Hosts[0].UsePrivilegedPort.ShouldBe(value));

    /// <summary>
    /// Verifies that the UserKnownHostsFile property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("~/.ssh/known_hosts")]
    [InlineData("/tmp/known_hosts")]
    public void Can_Set_UserKnownHostsFile(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.UserKnownHostsFile(value); }),
        assert: file => file.Hosts[0].UserKnownHostsFile.ShouldBe(value));

    /// <summary>
    /// Verifies that the VerifyHostKeyDns property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_VerifyHostKeyDns(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.VerifyHostKeyDns(value); }),
        assert: file => file.Hosts[0].VerifyHostKeyDns.ShouldBe(value));

    /// <summary>
    /// Verifies that the VisualHostKey property of a host configuration can be enabled or disabled.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Can_Set_VisualHostKey(bool value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.VisualHostKey(value); }),
        assert: file => file.Hosts[0].VisualHostKey.ShouldBe(value));

    /// <summary>
    /// Verifies that the XAuthLocation property of a host configuration can be set to the specified value.
    /// </summary>
    [Theory]
    [InlineData("/usr/bin/xauth")]
    [InlineData("/tmp/xauth")]
    public void Can_Set_XAuthLocation(string value) => SshConfigTesting.FileTestter.Valid(
        body: builder => builder.Host(hb => { hb.Name("foo"); hb.XAuthLocation(value); }),
        assert: file => file.Hosts[0].XAuthLocation.ShouldBe(value));
}