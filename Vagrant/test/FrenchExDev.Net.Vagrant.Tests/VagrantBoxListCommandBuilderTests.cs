namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantBoxListCommandBuilderTests : VagrantCommandTest<VagrantBoxListCommandBuilder, VagrantBoxListCommand>
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
        var cmd = Valid(b => b
            .BoxInfo()
            .MachineReadable()
            .WorkingDirectory("/tmp"));

        cmd.BoxInfo.ShouldBeTrue();
        cmd.MachineReadable.ShouldBeTrue();
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.BoxInfo().MachineReadable());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("list");
        args.ShouldContain("--box-info");
        args.ShouldContain("--machine-readable");
    }
}
