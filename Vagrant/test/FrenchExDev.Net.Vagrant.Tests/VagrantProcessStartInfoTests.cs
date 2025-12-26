using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantProcessStartInfoTests
{
    [Fact]
    public void ToProcessStartInfo_SetsCorrectExecutable()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();

        psi.FileName.ShouldBe("vagrant");
    }

    [Fact]
    public void ToProcessStartInfo_SetsWorkingDirectory()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => b.WorkingDirectory("/tmp/vagrant"));
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();

        psi.WorkingDirectory.ShouldBe("/tmp/vagrant");
    }

    [Fact]
    public void ToProcessStartInfo_SetsWorkingDirectoryFromParameter()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo("/custom/path");

        psi.WorkingDirectory.ShouldBe("/custom/path");
    }

    [Fact]
    public void ToProcessStartInfo_IncludesArguments()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => b.Provider("virtualbox").NoProvision());
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();

        psi.ArgumentList.ShouldContain("up");
        psi.ArgumentList.ShouldContain("--provider");
        psi.ArgumentList.ShouldContain("virtualbox");
        psi.ArgumentList.ShouldContain("--no-provision");
    }

    [Fact]
    public void ToProcessStartInfo_SetsRedirection()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();

        psi.RedirectStandardOutput.ShouldBeTrue();
        psi.RedirectStandardError.ShouldBeTrue();
        psi.RedirectStandardInput.ShouldBeTrue();
        psi.UseShellExecute.ShouldBeFalse();
    }
}
