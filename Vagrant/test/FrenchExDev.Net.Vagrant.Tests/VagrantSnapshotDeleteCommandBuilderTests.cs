namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantSnapshotDeleteCommandBuilderTests : VagrantCommandTest<VagrantSnapshotDeleteCommandBuilder, VagrantSnapshotDeleteCommand>
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.Name("snapshot1"));
        cmd.Name.ShouldBe("snapshot1");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => b
            .Name("snapshot1")
            .VmName("web"));

        cmd.Name.ShouldBe("snapshot1");
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyName_Fails()
    {
        Invalid(b => b.Name(""));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.Name("snapshot1").VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("delete");
        args.ShouldContain("web");
        args.ShouldContain("snapshot1");
    }
}
