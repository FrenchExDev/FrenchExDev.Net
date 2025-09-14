using FrenchExDev.Net.Ssh.Config.Testing;
using Shouldly;

namespace FrenchExDev.Net.Ssh.Config.Tests;

public class SshConfigFileTests
{
    [Theory]
    [InlineData(AddressFamilyEnum.All)]
    [InlineData(AddressFamilyEnum.Inet)]
    [InlineData(AddressFamilyEnum.Inet6)]
    public void Can_Set_AddressFamily(AddressFamilyEnum addressFamilyEnum)
    {
        SshConfigTesting.FileTestter.Valid(
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
    }
}
