namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantGlobalStatusCommandBuilderTests : VagrantCommandTest<VagrantGlobalStatusCommandBuilder, VagrantGlobalStatusCommand>
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithPrune_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Prune());
        cmd.Prune.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.Prune());

        var args = cmd.ToArguments();
        args.ShouldContain("global-status");
        args.ShouldContain("--prune");
    }
}
