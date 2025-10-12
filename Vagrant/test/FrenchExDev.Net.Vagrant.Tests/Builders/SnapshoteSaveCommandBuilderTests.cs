using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class SnapshoteSaveCommandBuilderTests : CommandBuilderTester<SnapshotSaveCommandBuilder, SnapshotSaveCommand>
{
    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => { },
            failures =>
            {
                failures.Keys.ShouldContain(nameof(SnapshotSaveCommand.Name));
                var f = failures[nameof(SnapshotSaveCommand.Name)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Missing required parameter 'name'");
            }
        );
    }

    [Fact]
    public void Build_WithName_SucceedsAndIncludesName()
    {
        Valid(
            builder => builder.Name("snap1").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("snap1"); },
            args => { args.ShouldContain("snap1"); }
        );
    }
}
