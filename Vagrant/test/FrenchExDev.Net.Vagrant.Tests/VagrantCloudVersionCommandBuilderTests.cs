namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantCloudVersionCommandBuilderTests : VagrantCommandTest<VagrantCloudVersionCommandBuilder, VagrantCloudVersionCommand>
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.SubCommand("create"));
        cmd.SubCommand.ShouldBe("create");
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.SubCommand("release"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("version");
        args.ShouldContain("release");
    }
}
