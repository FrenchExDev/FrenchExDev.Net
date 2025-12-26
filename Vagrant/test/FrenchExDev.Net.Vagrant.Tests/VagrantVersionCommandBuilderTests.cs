namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantVersionCommandBuilderTests : VagrantCommandTest<VagrantVersionCommandBuilder, VagrantVersionCommand>
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => { });

        var args = cmd.ToArguments();
        args.ShouldContain("version");
    }
}
