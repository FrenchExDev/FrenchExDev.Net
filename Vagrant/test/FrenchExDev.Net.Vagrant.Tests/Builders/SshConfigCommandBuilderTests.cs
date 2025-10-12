using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class SshConfigCommandBuilderTests : CommandBuilderTester<SshConfigCommandBuilder, SshConfigCommand>
{
    [Fact]
    public void Build_Default_NoOptions()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.MachineName.ShouldBeNull(); cmd.ExtraArgument.ShouldBeNull(); cmd.Host.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "ssh-config" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithMachine_IncludesMachineName()
    {
        Valid(
            builder => builder.Machine("default").WorkingDirectory("foo"),
            cmd => { cmd.MachineName.ShouldBe("default"); },
            args => { args.ShouldContain("default"); }
        );
    }

    [Fact]
    public void Build_WithExtraArg_IncludesExtraArg()
    {
        Valid(
            builder => builder.ExtraArg("--foo").WorkingDirectory("foo"),
            cmd => { cmd.ExtraArgument.ShouldBe("--foo"); },
            args => { args.ShouldContain("--foo"); }
        );
    }

    [Fact]
    public void Build_WithHost_IncludesHostOption()
    {
        Valid(
            builder => builder.Host("example.com").WorkingDirectory("foo"),
            cmd => { cmd.Host.ShouldBe("example.com"); },
            args => { args.ShouldContain("--host"); args.ShouldContain("example.com"); }
        );
    }

    [Fact]
    public void Machine_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Machine("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SshConfigCommand.MachineName));
                var f = failures[nameof(SshConfigCommand.MachineName)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Machine name cannot be empty");
            }
        );
    }

    [Fact]
    public void Host_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Host("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SshConfigCommand.Host));
                var f = failures[nameof(SshConfigCommand.Host)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--host cannot be set and empty");
            }
        );
    }
}
