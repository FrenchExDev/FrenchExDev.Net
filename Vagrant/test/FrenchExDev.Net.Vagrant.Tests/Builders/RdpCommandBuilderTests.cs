using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class RdpCommandBuilderTests : CommandBuilderTester<RdpCommandBuilder, RdpCommand>
{
    [Fact]
    public void Build_WithWorkingDirectory_Succeeds()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.WorkingDirectory.ShouldBe("foo"); },
            args => { args.ShouldBe(new[] { "rdp" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithoutWorkingDirectory_FailsValidation()
    {
        Invalid(
            builder => { },
            failures =>
            {
                failures.Keys.ShouldContain(nameof(RdpCommandBuilder.WorkingDirectory));
                var f = failures[nameof(RdpCommandBuilder.WorkingDirectory)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Working directory must be set and not empty or whitespace.");
            }
        );
    }
}
