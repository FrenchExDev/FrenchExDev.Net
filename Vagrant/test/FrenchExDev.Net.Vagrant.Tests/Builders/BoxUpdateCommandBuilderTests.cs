using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxUpdateCommandBuilderTests : CommandBuilderTester<BoxUpdateCommandBuilder, BoxUpdateCommand>
{
    [Fact]
    public void Build_Default_HasNoNameOrProvider()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBeNull(); cmd.Provider.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "box", "update" }.ToList()); }
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
    public void Build_WithProvider_IncludesProviderInCommand()
    {
        Valid(
            builder => builder.Provider("virtualbox").WorkingDirectory("foo"),
            cmd => { cmd.Provider.ShouldBe("virtualbox"); },
            args => { args.ShouldBe(new[] { "box", "update" }.ToList()); }
        );
    }

    [Fact]
    public void Name_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Name(""),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxUpdateCommand.Name));
                var f = failures[nameof(BoxUpdateCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--box cannot be empty");
            }
        );
    }

    [Fact]
    public void Provider_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Provider("") ,
            failures =>
            {
                failures.Keys.ShouldContain(nameof(BoxUpdateCommand.Provider));
                var f = failures[nameof(BoxUpdateCommand.Provider)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--provider cannot be empty");
            }
        );
    }
}
