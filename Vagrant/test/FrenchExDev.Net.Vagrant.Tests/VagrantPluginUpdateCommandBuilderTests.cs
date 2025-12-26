using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantPluginUpdateCommandBuilderTests : VagrantCommandTest<VagrantPluginUpdateCommandBuilder, VagrantPluginUpdateCommand>
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Name("vagrant-vbguest").Local());

        cmd.Name.ShouldBe("vagrant-vbguest");
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.Name("vagrant-vbguest").Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("update");
        args.ShouldContain("--local");
        args.ShouldContain("vagrant-vbguest");
    }
}
