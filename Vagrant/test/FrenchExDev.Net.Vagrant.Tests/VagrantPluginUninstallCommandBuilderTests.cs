using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantPluginUninstallCommandBuilderTests : VagrantCommandTest<VagrantPluginUninstallCommandBuilder, VagrantPluginUninstallCommand>
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Name("vagrant-vbguest"));
        cmd.Name.ShouldBe("vagrant-vbguest");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Name("vagrant-vbguest").Local());

        cmd.Name.ShouldBe("vagrant-vbguest");
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.Name("vagrant-vbguest").Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("uninstall");
        args.ShouldContain("--local");
        args.ShouldContain("vagrant-vbguest");
    }
}
