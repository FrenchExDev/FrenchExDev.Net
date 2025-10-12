using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxOutdatedCommandBuilderTests : CommandBuilderTester<BoxOutdatedCommandBuilder, BoxOutdatedCommand>
{
    [Fact]
    public void Build_Default_DoesNotIncludeOptions()
    {
        Valid(
            builder => { builder.WorkingDirectory("foo"); },
            cmd =>
            {
                cmd.Provider.ShouldBeNull();
                cmd.Global.ShouldBeNull();
                cmd.Insecure.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "box", "outdated" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithProvider_IncludesProvider()
    {
        Valid(
            builder => builder.Provider("virtualbox").WorkingDirectory("foo"),
            cmd => { cmd.Provider.ShouldBe("virtualbox"); },
            args =>
            {
                args.ShouldContain("--provider");
                args.ShouldContain("virtualbox");
            }
        );
    }

    [Fact]
    public void Build_WithEmptyProvider_FailsValidation()
    {
        Invalid(
            builder => builder.Provider("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("Options");
                var failure = failures["Options"].First();
                failure.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)failure.Value).Message.ShouldContain("--provider cannot be empty");
            }
        );
    }

    [Fact]
    public void Build_WithGlobalTrue_IncludesGlobalFlag()
    {
        Valid(
            builder => builder.Global().WorkingDirectory("foo"),
            cmd => { cmd.Global.ShouldBe(true); },
            args => { args.ShouldContain("--global"); }
        );
    }

    [Fact]
    public void Build_WithGlobalFalse_DoesNotIncludeGlobalFlag()
    {
        Valid(
            builder => builder.Global(false).WorkingDirectory("foo"),
            cmd => { cmd.Global.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "box", "outdated" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithInsecureTrue_IncludesInsecureFlag()
    {
        Valid(
            builder => builder.Insecure().WorkingDirectory("foo"),
            cmd => { cmd.Insecure.ShouldBe(true); },
            args => { args.ShouldContain("--insecure"); }
        );
    }
}
