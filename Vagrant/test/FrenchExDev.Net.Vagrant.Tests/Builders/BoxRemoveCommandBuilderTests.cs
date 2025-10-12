using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxRemoveCommandBuilderTests : CommandBuilderTester<BoxRemoveCommandBuilder, BoxRemoveCommand>
{
    [Fact]
    public void Build_Default_HasNoOptions()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Name.ShouldBeNull();
                cmd.Provider.ShouldBeNull();
                cmd.All.ShouldBeNull();
                cmd.Force.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "box", "remove" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithName_IncludesName()
    {
        Valid(
            builder => builder.Name("mybox").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("mybox"); },
            args => { args.ShouldContain("mybox"); }
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
    public void Provider_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Provider(""),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRemoveCommand.Provider));
                var f = failures[nameof(BoxRemoveCommand.Provider)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--provider cannot be empty");
            }
        );
    }

    [Fact]
    public void Name_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Name(""),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRemoveCommand.Name));
                var f = failures[nameof(BoxRemoveCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Name parameter cannot be empty");
            }
        );
    }

    [Fact]
    public void All_WithName_FailsValidation()
    {
        Invalid(
            builder => builder.Name("mybox").All(),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxRemoveCommand.All));
                var f = failures[nameof(BoxRemoveCommand.All)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--all cannot be combined with a name parameter");
            }
        );
    }

    [Fact]
    public void Build_WithAllAndForce_IncludesFlags()
    {
        Valid(
            builder => builder.All().Force().WorkingDirectory("foo"),
            cmd =>
            {
                cmd.All.ShouldBe(true);
                cmd.Force.ShouldBe(true);
            },
            args =>
            {
                args.ShouldContain("--all");
                args.ShouldContain("--force");
            }
        );
    }
}
