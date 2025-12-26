namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantSnapshotListCommandBuilderTests : VagrantCommandTest<VagrantSnapshotListCommandBuilder, VagrantSnapshotListCommand>
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithVmName_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.VmName("web"));
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("list");
        args.ShouldContain("web");
    }
}
