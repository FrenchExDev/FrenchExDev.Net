using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxAddCommandBuilderTests : CommandBuilderTester<BoxAddCommandBuilder, BoxAddCommand>
{
    [Fact]
    public void Build_WithUrlOrPath_SetsNameAndArguments()
    {
        Valid((b) =>
        {
            b
            .UrlOrPath("ubuntu.box")
            .WorkingDirectory("foo");
        }, (cmd) =>
        {
            cmd.NameOrUrlOrPath.ShouldBe("ubuntu.box");

        }, (args) =>
        {
            args.ShouldContain("box");
            args.ShouldContain("add");
            args.ShouldContain("ubuntu.box");
        });
    }

    [Fact]
    public void Build_WithoutUrlOrPath_FailsValidation()
    {
        Invalid((b) => { }, (failures) =>
        {
            failures.Keys.ShouldContain(nameof(BoxAddCommand.NameOrUrlOrPath));
            var failure = failures[nameof(BoxAddCommand.NameOrUrlOrPath)].First();
            failure.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)failure.Value).Message.ShouldContain("Missing required parameter 'source'");
        });
    }

    [Fact]
    public void Build_WithAllOptions_IncludesAllArguments()
    {
        Valid((b) =>
        {
            b
            .UrlOrPath("mybox")
            .Force()
            .Provider("virtualbox")
            .Checksum("abc123")
            .ChecksumType("sha256")
            .BoxVersion("1.2.3")
            .Architecture("x64")
            .Insecure()
            .CaCert("cacert.pem")
            .CaPath("/etc/ssl")
            .Cert("cert.pem")
            .LocationTrusted()
            .WorkingDirectory("foo");
        }, (cmd) =>
        {

            cmd.Force.ShouldBe(true);
            cmd.Provider.ShouldBe("virtualbox");
            cmd.Checksum.ShouldBe("abc123");
            cmd.ChecksumType.ShouldBe("sha256");
            cmd.BoxVersion.ShouldBe("1.2.3");
            cmd.Architecture.ShouldBe("x64");
            cmd.Insecure.ShouldBe(true);
            cmd.CaCert.ShouldBe("cacert.pem");
            cmd.CaPath.ShouldBe("/etc/ssl");
            cmd.Cert.ShouldBe("cert.pem");
            cmd.LocationTrusted.ShouldBe(true);
            cmd.WorkingDirectory.ShouldBe("foo");
        }, (args) =>
        {
            args.ShouldContain("--force");
            args.ShouldContain("--insecure");
            args.ShouldContain("--location-trusted");
            args.ShouldContain("--cacert"); args.ShouldContain("cacert.pem");
            args.ShouldContain("--capath"); args.ShouldContain("/etc/ssl");
            args.ShouldContain("--cert"); args.ShouldContain("cert.pem");
            args.ShouldContain("--architecture"); args.ShouldContain("x64");
            args.ShouldContain("--provider"); args.ShouldContain("virtualbox");
            args.ShouldContain("--box-version"); args.ShouldContain("1.2.3");
            args.ShouldContain("--checksum"); args.ShouldContain("abc123");
            args.ShouldContain("--checksum-type"); args.ShouldContain("sha256");
            args.ShouldContain("mybox");
        });
    }

    [Fact]
    public void ChecksumType_WithoutChecksum_Fails()
    {
        Invalid((b) => b.UrlOrPath("box").ChecksumType("sha256"), (failures) =>
        {
            failures.Keys.ShouldContain(nameof(BoxAddCommand.ChecksumType));
            var failure = failures[nameof(BoxAddCommand.ChecksumType)].First();
            failure.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)failure.Value).Message.ShouldContain("--checksum-type requires --checksum");
        });
    }

    [Fact]
    public void ChecksumType_Unsupported_Fails()
    {
        Invalid((b) => b.UrlOrPath("box").ChecksumType("badtype").Checksum("abc"), (failures) =>
        {
            failures.Keys.ShouldContain(nameof(BoxAddCommand.ChecksumType));
            var failure = failures[nameof(BoxAddCommand.ChecksumType)].First();
            failure.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)failure.Value).Message.ShouldContain("Unsupported checksum type");
        });
    }

    [Fact]
    public void Provider_Empty_Fails()
    {
        Invalid((b) => b.UrlOrPath("box").Provider(""), (failures) =>
        {
            failures.Keys.ShouldContain(nameof(BoxAddCommand.Provider));
            var failure = failures[nameof(BoxAddCommand.Provider)].First();
            failure.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)failure.Value).Message.ShouldContain("--provider cannot be empty");
        });
    }
}
