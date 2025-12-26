namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantCloudBoxCommandBuilderTests : VagrantCommandTest<VagrantCloudBoxCommandBuilder, VagrantCloudBoxCommand>
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.SubCommand("show"));
        cmd.SubCommand.ShouldBe("show");
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.SubCommand("create"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("box");
        args.ShouldContain("create");
    }
}
