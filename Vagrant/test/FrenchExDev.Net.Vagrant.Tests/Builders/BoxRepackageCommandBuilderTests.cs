using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxRepackageCommandBuilderTests : CommandBuilderTester<BoxRepackageCommandBuilder, BoxRepackageCommand>
{
    [Fact]
    public void Build_WithRequiredParameters_Succeeds()
    {
        Valid(
            builder => builder.Name("mybox").Provider("virtualbox").BoxVersion("1.0").WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Name.ShouldBe("mybox");
                cmd.Provider.ShouldBe("virtualbox");
                cmd.BoxVersion.ShouldBe("1.0");
            },
            args =>
            {
                args.ShouldContain("box");
                args.ShouldContain("repackage");
                args.ShouldContain("mybox");
            }
        );
    }

    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => builder.Provider("virtualbox").BoxVersion("1.0").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRepackageCommand.Name));
                var f = failures[nameof(BoxRepackageCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Parameter 'name' is required");
            }
        );
    }

    [Fact]
    public void Build_MissingProvider_FailsValidation()
    {
        Invalid(
            builder => builder.Name("mybox").BoxVersion("1.0").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRepackageCommand.Provider));
                var f = failures[nameof(BoxRepackageCommand.Provider)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Parameter 'provider' is required");
            }
        );
    }

    [Fact]
    public void Build_MissingVersion_FailsValidation()
    {
        Invalid(
            builder => builder.Name("mybox").Provider("virtualbox").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRepackageCommand.Version));
                var f = failures[nameof(BoxRepackageCommand.Version)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Parameter 'version' is required");
            }
        );
    }

    [Fact]
    public void Output_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Name("mybox").Provider("virtualbox").BoxVersion("1.0").Output("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("_output");
                var f = failures["_output"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--output cannot be empty");
            }
        );
    }
}
