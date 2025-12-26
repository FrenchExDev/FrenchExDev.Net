namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantPluginRepairCommandBuilderTests : VagrantCommandTest<VagrantPluginRepairCommandBuilder, VagrantPluginRepairCommand>
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithLocal_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Local());
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("repair");
        args.ShouldContain("--local");
    }
}
