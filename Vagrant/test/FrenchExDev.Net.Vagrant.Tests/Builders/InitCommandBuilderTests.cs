using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class InitCommandBuilderTests : CommandBuilderTester<InitCommandBuilder, InitCommand>
{
    [Fact]
    public void Build_Default_NoNameOrOptions()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd =>
            {
                cmd.NameOrUrl.ShouldBeNull();
                cmd.Output.ShouldBeNull();
                cmd.Minimal.ShouldBeNull();
                cmd.Force.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "init" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithNameOrUrl_IncludesName()
    {
        Valid(
            builder => builder.NameOrUrl("ubuntu").WorkingDirectory("foo"),
            cmd => { cmd.NameOrUrl.ShouldBe("ubuntu"); },
            args => { args.ShouldContain("ubuntu"); }
        );
    }

    [Fact]
    public void Build_WithOutput_IncludesOutputProperty()
    {
        Valid(
            builder => builder.Output("Vagrantfile").WorkingDirectory("foo"),
            cmd => { cmd.Output.ShouldBe("Vagrantfile"); },
            args => { args.ShouldBe(new[] { "init" }.ToList()); }
        );
    }

    [Fact]
    public void Minimal_True_SetsProperty()
    {
        Valid(
            builder => builder.Minimal().WorkingDirectory("foo"),
            cmd => { cmd.Minimal.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "init" }.ToList()); }
        );
    }

    [Fact]
    public void Force_True_SetsProperty()
    {
        Valid(
            builder => builder.Force().WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "init" }.ToList()); }
        );
    }

    [Fact]
    public void NameOrUrl_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.NameOrUrl("") ,
            failures =>
            {
                failures.Keys.ShouldContain(nameof(InitCommand.NameOrUrl));
                var f = failures[nameof(InitCommand.NameOrUrl)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--box cannot be empty");
            }
        );
    }

    [Fact]
    public void Output_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Output("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(InitCommand.Output));
                var f = failures[nameof(InitCommand.Output)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--output cannot be empty");
            }
        );
    }
}
