using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class SshCommandBuilderTests : CommandBuilderTester<SshCommandBuilder, SshCommand>
{
    [Fact]
    public void Build_WithCommand_SetsCommandAndArguments()
    {
        Valid(b => b.Command("ls -la").WorkingDirectory("foo"), (cmd) =>
        {
            cmd.Command.ShouldBe("ls -la");
        }, (args) =>
        {
            args.ShouldContain("-c");
            args.ShouldContain("ls -la");
        });
    }

    [Fact]
    public void Build_WithoutCommand_FailsValidation()
    {
        Invalid(b => b.WorkingDirectory("foo"), failures =>
        {
            failures.Keys.ShouldContain("_command");
            var f = failures["_command"].First();
            f.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)f.Value).Message.ShouldContain("--command cannot be empty");
        });
    }

    [Fact]
    public void Build_WithWhitespaceCommand_FailsValidation()
    {
        Invalid(b => b.Command("   ").WorkingDirectory("foo"), failures =>
        {
            failures.Keys.ShouldContain("_command");
            var f = failures["_command"].First();
            f.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)f.Value).Message.ShouldContain("--command cannot be empty");
        });
    }

    [Fact]
    public void Build_WithCommand_ButNoWorkingDirectory_FailsWorkingDirectoryValidation()
    {
        Invalid(b => b.Command("ls -la"), failures =>
        {
            failures.Keys.ShouldContain(nameof(SshCommandBuilder.WorkingDirectory));
            var f = failures[nameof(SshCommandBuilder.WorkingDirectory)].First();
            f.Value.ShouldBeOfType<InvalidDataException>();
            ((InvalidDataException)f.Value).Message.ShouldContain("Working directory must be set");
        });
    }
}
