namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantSnapshotPushCommandBuilderTests : VagrantCommandTest<VagrantSnapshotPushCommandBuilder, VagrantSnapshotPushCommand>
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
        args.ShouldContain("push");
        args.ShouldContain("web");
    }
}
