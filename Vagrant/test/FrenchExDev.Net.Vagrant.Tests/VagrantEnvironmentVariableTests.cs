namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantEnvironmentVariableTests
{
    [Fact]
    public void Env_SetsEnvironmentVariable_OnBoxAddCommand()
    {
        var cmd = new VagrantBoxAddCommand { Box = "test" };
        cmd.Env("VAGRANT_HOME", "/custom/home");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["VAGRANT_HOME"].ShouldBe("/custom/home");
    }

    [Fact]
    public void Env_SetsEnvironmentVariable_OnCloudSearchCommand()
    {
        var cmd = new VagrantCloudSearchCommand { };
        cmd.Env("VAGRANT_CLOUD_TOKEN", "mytoken");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["VAGRANT_CLOUD_TOKEN"].ShouldBe("mytoken");
    }

    [Fact]
    public void Env_SetsEnvironmentVariable_OnPluginInstallCommand()
    {
        var cmd = new VagrantPluginInstallCommand { Name = "test" };
        cmd.Env("GEM_HOME", "/custom/gems");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["GEM_HOME"].ShouldBe("/custom/gems");
    }

    [Fact]
    public void Env_SetsEnvironmentVariable_OnSnapshotSaveCommand()
    {
        var cmd = new VagrantSnapshotSaveCommand { Name = "test" };
        cmd.Env("VAGRANT_LOG", "debug");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["VAGRANT_LOG"].ShouldBe("debug");
    }

    [Fact]
    public void Env_SetsEnvironmentVariable_OnDestroyCommand()
    {
        var cmd = new VagrantDestroyCommand { }.Env("VAGRANT_LOG", "info");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["VAGRANT_LOG"].ShouldBe("info");
    }

    [Fact]
    public void Env_SetsEnvironmentVariable_OnUpCommand()
    {
        var cmd = new VagrantUpCommand { }.Env("VAGRANT_HOME", "/test");

        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        psi.Environment["VAGRANT_HOME"].ShouldBe("/test");
    }
}
