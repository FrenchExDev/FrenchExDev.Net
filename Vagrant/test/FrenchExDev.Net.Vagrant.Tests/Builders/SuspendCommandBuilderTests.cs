using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class SuspendCommandBuilderTests : CommandBuilderTester<SuspendCommandBuilder, SuspendCommand>
{
    [Fact]
    public void Build_Default_NoName()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "suspend" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithName_Succeeds()
    {
        Valid(
            builder => builder.Name("vm1").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("vm1"); },
            args => { args.ShouldBe(new[] { "suspend" }.ToList()); }
        );
    }

    [Fact]
    public void Name_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Name("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SuspendCommand.Name));
                var f = failures[nameof(SuspendCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("If specified, 'name' cannot be empty or whitespace");
            }
        );
    }

    [Fact]
    public void Name_Whitespace_FailsValidation()
    {
        Invalid(
            builder => builder.Name("   ").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SuspendCommand.Name));
                var f = failures[nameof(SuspendCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("If specified, 'name' cannot be empty or whitespace");
            }
        );
    }

    [Fact]
    public void Build_WithoutWorkingDirectory_FailsValidation()
    {
        Invalid(
            builder => { },
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SuspendCommandBuilder.WorkingDirectory));
                var f = failures[nameof(SuspendCommandBuilder.WorkingDirectory)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Working directory must be set");
            }
        );
    }
}
