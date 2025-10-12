using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class ReloadCommandBuilderTests : CommandBuilderTester<ReloadCommandBuilder, ReloadCommand>
{
    [Fact]
    public void Build_Default_NoFlagsOrProvider()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Provision.ShouldBeNull(); cmd.NoProvision.ShouldBeNull(); cmd.Provider.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "reload" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithProvisionTrue_IncludesProvision()
    {
        Valid(
            builder => builder.Provision().WorkingDirectory("foo"),
            cmd => { cmd.Provision.ShouldBe(true); },
            args => { args.ShouldContain("--provision"); }
        );
    }

    [Fact]
    public void Build_WithNoProvisionTrue_IncludesNoProvision()
    {
        Valid(
            builder => builder.NoProvision().WorkingDirectory("foo"),
            cmd => { cmd.NoProvision.ShouldBe(true); },
            args => { args.ShouldContain("--no-provision"); }
        );
    }

    [Fact]
    public void Build_WithBothProvisionAndNoProvision_FailsValidation()
    {
        Invalid(
            builder => builder.Provision().NoProvision().WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("_provision");
                var f = failures["_provision"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("mutually exclusive");
            }
        );
    }

    [Fact]
    public void Provider_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Provider("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("_provider");
                var f = failures["_provider"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--provider cannot be empty");
            }
        );
    }

    [Fact]
    public void Build_WithProvider_IncludesProvider()
    {
        Valid(
            builder => builder.Provider("virtualbox").WorkingDirectory("foo"),
            cmd => { cmd.Provider.ShouldBe("virtualbox"); },
            args => { args.ShouldContain("--provider"); args.ShouldContain("virtualbox"); }
        );
    }
}
