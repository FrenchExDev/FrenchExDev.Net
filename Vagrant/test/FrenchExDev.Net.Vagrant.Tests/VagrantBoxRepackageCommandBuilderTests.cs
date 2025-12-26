namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantBoxRepackageCommandBuilderTests : VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>
{
    [Fact]
    public void Valid_WithRequiredFields_BuildsSuccessfully()
    {
        var cmd = Valid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));

        cmd.Name.ShouldBe("ubuntu/focal64");
        cmd.Provider.ShouldBe("virtualbox");
        cmd.Cartouche.ShouldBe("1.0.0");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        Invalid(b => b
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void Invalid_WithoutProvider_Fails()
    {
        Invalid(b => b
            .Name("ubuntu/focal64")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void Invalid_WithoutCartouche_Fails()
    {
        Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox"));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("repackage");
        args.ShouldContain("ubuntu/focal64");
        args.ShouldContain("virtualbox");
        args.ShouldContain("1.0.0");
    }
}
