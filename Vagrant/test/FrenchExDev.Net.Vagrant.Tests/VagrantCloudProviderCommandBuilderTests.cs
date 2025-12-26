namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantCloudProviderCommandBuilderTests : VagrantCommandTest<VagrantCloudProviderCommandBuilder, VagrantCloudProviderCommand>
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
        var cmd = Valid(b => b.SubCommand("upload"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("provider");
        args.ShouldContain("upload");
    }
}
