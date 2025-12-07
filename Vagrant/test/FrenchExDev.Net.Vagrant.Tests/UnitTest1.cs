using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests;

#region Box Command Tests

public class VagrantBoxAddCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredBox_BuildsSuccessfully()
    {
        var cmd = VagrantBoxAddCommandTest.Valid(b => b.Box("ubuntu/focal64"));
        cmd.Box.ShouldBe("ubuntu/focal64");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxAddCommandTest.Valid(b => b
            .Box("ubuntu/focal64")
            .BoxVersion("1.0.0")
            .CaCert("/path/to/ca.crt")
            .CaPath("/path/to/ca")
            .Cert("/path/to/cert.crt")
            .Checksum("abc123")
            .ChecksumType("sha256")
            .Clean()
            .Force()
            .Insecure()
            .LocationTrusted()
            .Name("my-box")
            .Provider("virtualbox")
            .WorkingDirectory("/tmp"));

        cmd.Box.ShouldBe("ubuntu/focal64");
        cmd.BoxVersion.ShouldBe("1.0.0");
        cmd.CaCert.ShouldBe("/path/to/ca.crt");
        cmd.CaPath.ShouldBe("/path/to/ca");
        cmd.Cert.ShouldBe("/path/to/cert.crt");
        cmd.Checksum.ShouldBe("abc123");
        cmd.ChecksumType.ShouldBe("sha256");
        cmd.Clean.ShouldBeTrue();
        cmd.Force.ShouldBeTrue();
        cmd.Insecure.ShouldBeTrue();
        cmd.LocationTrusted.ShouldBeTrue();
        cmd.Name.ShouldBe("my-box");
        cmd.Provider.ShouldBe("virtualbox");
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void Invalid_WithoutBox_Fails()
    {
        VagrantBoxAddCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyBox_Fails()
    {
        VagrantBoxAddCommandTest.Invalid(b => b.Box(""));
    }

    [Fact]
    public void Invalid_WithWhitespaceBox_Fails()
    {
        VagrantBoxAddCommandTest.Invalid(b => b.Box("   "));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxAddCommandTest.Valid(b => b
            .Box("ubuntu/focal64")
            .Force()
            .Provider("virtualbox"));

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("add");
        args.ShouldContain("--force");
        args.ShouldContain("--provider");
        args.ShouldContain("virtualbox");
        args.ShouldContain("ubuntu/focal64");
    }
}

public class VagrantBoxListCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxListCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxListCommandTest.Valid(b => b
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
        var cmd = VagrantBoxListCommandTest.Valid(b => b.BoxInfo().MachineReadable());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("list");
        args.ShouldContain("--box-info");
        args.ShouldContain("--machine-readable");
    }
}

public class VagrantBoxOutdatedCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxOutdatedCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxOutdatedCommandTest.Valid(b => b
            .Force()
            .Global()
            .MachineReadable());

        cmd.Force.ShouldBeTrue();
        cmd.Global.ShouldBeTrue();
        cmd.MachineReadable.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxOutdatedCommandTest.Valid(b => b.Global().Force());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("outdated");
        args.ShouldContain("--global");
        args.ShouldContain("--force");
    }
}

public class VagrantBoxPruneCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxPruneCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxPruneCommandTest.Valid(b => b
            .DryRun()
            .Force()
            .KeepActiveBoxes()
            .Name("ubuntu/focal64")
            .Provider("virtualbox"));

        cmd.DryRun.ShouldBeTrue();
        cmd.Force.ShouldBeTrue();
        cmd.KeepActiveBoxes.ShouldBeTrue();
        cmd.Name.ShouldBe("ubuntu/focal64");
        cmd.Provider.ShouldBe("virtualbox");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxPruneCommandTest.Valid(b => b.DryRun().Force());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("prune");
        args.ShouldContain("--dry-run");
        args.ShouldContain("--force");
    }
}

public class VagrantBoxRemoveCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantBoxRemoveCommandTest.Valid(b => b.Name("ubuntu/focal64"));
        cmd.Name.ShouldBe("ubuntu/focal64");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxRemoveCommandTest.Valid(b => b
            .Name("ubuntu/focal64")
            .All()
            .BoxVersion("1.0.0")
            .Force()
            .Provider("virtualbox"));

        cmd.Name.ShouldBe("ubuntu/focal64");
        cmd.All.ShouldBeTrue();
        cmd.BoxVersion.ShouldBe("1.0.0");
        cmd.Force.ShouldBeTrue();
        cmd.Provider.ShouldBe("virtualbox");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantBoxRemoveCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyName_Fails()
    {
        VagrantBoxRemoveCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxRemoveCommandTest.Valid(b => b.Name("ubuntu/focal64").Force());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("remove");
        args.ShouldContain("--force");
        args.ShouldContain("ubuntu/focal64");
    }
}

public class VagrantBoxRepackageCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredFields_BuildsSuccessfully()
    {
        var cmd = VagrantBoxRepackageCommandTest.Valid(b => b
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
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void Invalid_WithoutProvider_Fails()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void Invalid_WithoutCartouche_Fails()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox"));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxRepackageCommandTest.Valid(b => b
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

public class VagrantBoxUpdateCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxUpdateCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantBoxUpdateCommandTest.Valid(b => b
            .Box("ubuntu/focal64")
            .CaCert("/path/to/ca.crt")
            .CaPath("/path/to/ca")
            .Cert("/path/to/cert.crt")
            .Force()
            .Insecure()
            .Provider("virtualbox"));

        cmd.Box.ShouldBe("ubuntu/focal64");
        cmd.CaCert.ShouldBe("/path/to/ca.crt");
        cmd.CaPath.ShouldBe("/path/to/ca");
        cmd.Cert.ShouldBe("/path/to/cert.crt");
        cmd.Force.ShouldBeTrue();
        cmd.Insecure.ShouldBeTrue();
        cmd.Provider.ShouldBe("virtualbox");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantBoxUpdateCommandTest.Valid(b => b.Box("ubuntu/focal64").Force());

        var args = cmd.ToArguments();
        args.ShouldContain("box");
        args.ShouldContain("update");
        args.ShouldContain("--box");
        args.ShouldContain("ubuntu/focal64");
        args.ShouldContain("--force");
    }
}

#endregion

#region Cloud Command Tests

public class VagrantCloudAuthCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = VagrantCloudAuthCommandTest.Valid(b => b.SubCommand("login"));
        cmd.SubCommand.ShouldBe("login");
    }

    [Theory]
    [InlineData("login")]
    [InlineData("logout")]
    [InlineData("whoami")]
    public void Valid_WithDifferentSubCommands_BuildsSuccessfully(string subCommand)
    {
        var cmd = VagrantCloudAuthCommandTest.Valid(b => b.SubCommand(subCommand));
        cmd.SubCommand.ShouldBe(subCommand);
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        VagrantCloudAuthCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptySubCommand_Fails()
    {
        VagrantCloudAuthCommandTest.Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void Invalid_WithWhitespaceSubCommand_Fails()
    {
        VagrantCloudAuthCommandTest.Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantCloudAuthCommandTest.Valid(b => b.SubCommand("login"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("auth");
        args.ShouldContain("login");
    }
}

public class VagrantCloudBoxCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = VagrantCloudBoxCommandTest.Valid(b => b.SubCommand("show"));
        cmd.SubCommand.ShouldBe("show");
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        VagrantCloudBoxCommandTest.Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantCloudBoxCommandTest.Valid(b => b.SubCommand("create"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("box");
        args.ShouldContain("create");
    }
}

public class VagrantCloudProviderCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = VagrantCloudProviderCommandTest.Valid(b => b.SubCommand("create"));
        cmd.SubCommand.ShouldBe("create");
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        VagrantCloudProviderCommandTest.Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantCloudProviderCommandTest.Valid(b => b.SubCommand("upload"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("provider");
        args.ShouldContain("upload");
    }
}

public class VagrantCloudSearchCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantCloudSearchCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantCloudSearchCommandTest.Valid(b => b
            .Query("ubuntu")
            .Json()
            .Limit(10)
            .Page(1)
            .Provider("virtualbox")
            .Short()
            .SortBy("downloads")
            .Order("desc"));

        cmd.Query.ShouldBe("ubuntu");
        cmd.Json.ShouldBeTrue();
        cmd.Limit.ShouldBe(10);
        cmd.Page.ShouldBe(1);
        cmd.Provider.ShouldBe("virtualbox");
        cmd.Short.ShouldBeTrue();
        cmd.SortBy.ShouldBe("downloads");
        cmd.Order.ShouldBe("desc");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantCloudSearchCommandTest.Valid(b => b.Query("ubuntu").Json().Limit(10));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("search");
        args.ShouldContain("--json");
        args.ShouldContain("--limit");
        args.ShouldContain("10");
        args.ShouldContain("ubuntu");
    }
}

public class VagrantCloudVersionCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = VagrantCloudVersionCommandTest.Valid(b => b.SubCommand("create"));
        cmd.SubCommand.ShouldBe("create");
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        VagrantCloudVersionCommandTest.Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantCloudVersionCommandTest.Valid(b => b.SubCommand("release"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("version");
        args.ShouldContain("release");
    }
}

#endregion

#region Plugin Command Tests

public class VagrantPluginInstallCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantPluginInstallCommandTest.Valid(b => b.Name("vagrant-vbguest"));
        cmd.Name.ShouldBe("vagrant-vbguest");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginInstallCommandTest.Valid(b => b
            .Name("vagrant-vbguest")
            .EntryPoint("entry.rb")
            .Local()
            .PluginCleanSources()
            .PluginSource("https://rubygems.org")
            .PluginVersion("1.0.0")
            .Verbose());

        cmd.Name.ShouldBe("vagrant-vbguest");
        cmd.EntryPoint.ShouldBe("entry.rb");
        cmd.Local.ShouldBeTrue();
        cmd.PluginCleanSources.ShouldBeTrue();
        cmd.PluginSource.ShouldBe("https://rubygems.org");
        cmd.PluginVersion.ShouldBe("1.0.0");
        cmd.Verbose.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantPluginInstallCommandTest.Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginInstallCommandTest.Valid(b => b.Name("vagrant-vbguest").Local().Verbose());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("install");
        args.ShouldContain("--local");
        args.ShouldContain("--verbose");
        args.ShouldContain("vagrant-vbguest");
    }
}

public class VagrantPluginLicenseCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredFields_BuildsSuccessfully()
    {
        var cmd = VagrantPluginLicenseCommandTest.Valid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("/path/to/license.lic"));

        cmd.Name.ShouldBe("vagrant-vmware-desktop");
        cmd.LicenseFile.ShouldBe("/path/to/license.lic");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b.LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void Invalid_WithoutLicenseFile_Fails()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b.Name("vagrant-vmware-desktop"));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginLicenseCommandTest.Valid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("/path/to/license.lic"));

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("license");
        args.ShouldContain("vagrant-vmware-desktop");
        args.ShouldContain("/path/to/license.lic");
    }
}

public class VagrantPluginListCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginListCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithLocal_BuildsSuccessfully()
    {
        var cmd = VagrantPluginListCommandTest.Valid(b => b.Local());
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginListCommandTest.Valid(b => b.Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("list");
        args.ShouldContain("--local");
    }
}

public class VagrantPluginRepairCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginRepairCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithLocal_BuildsSuccessfully()
    {
        var cmd = VagrantPluginRepairCommandTest.Valid(b => b.Local());
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginRepairCommandTest.Valid(b => b.Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("repair");
        args.ShouldContain("--local");
    }
}

public class VagrantPluginUninstallCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantPluginUninstallCommandTest.Valid(b => b.Name("vagrant-vbguest"));
        cmd.Name.ShouldBe("vagrant-vbguest");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginUninstallCommandTest.Valid(b => b.Name("vagrant-vbguest").Local());

        cmd.Name.ShouldBe("vagrant-vbguest");
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantPluginUninstallCommandTest.Invalid(b => { });
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginUninstallCommandTest.Valid(b => b.Name("vagrant-vbguest").Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("uninstall");
        args.ShouldContain("--local");
        args.ShouldContain("vagrant-vbguest");
    }
}

public class VagrantPluginUpdateCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginUpdateCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPluginUpdateCommandTest.Valid(b => b.Name("vagrant-vbguest").Local());

        cmd.Name.ShouldBe("vagrant-vbguest");
        cmd.Local.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPluginUpdateCommandTest.Valid(b => b.Name("vagrant-vbguest").Local());

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("update");
        args.ShouldContain("--local");
        args.ShouldContain("vagrant-vbguest");
    }
}

#endregion

#region Snapshot Command Tests

public class VagrantSnapshotDeleteCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotDeleteCommandTest.Valid(b => b.Name("snapshot1"));
        cmd.Name.ShouldBe("snapshot1");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotDeleteCommandTest.Valid(b => b
            .Name("snapshot1")
            .VmName("web"));

        cmd.Name.ShouldBe("snapshot1");
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantSnapshotDeleteCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyName_Fails()
    {
        VagrantSnapshotDeleteCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotDeleteCommandTest.Valid(b => b.Name("snapshot1").VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("delete");
        args.ShouldContain("web");
        args.ShouldContain("snapshot1");
    }
}

public class VagrantSnapshotListCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotListCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithVmName_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotListCommandTest.Valid(b => b.VmName("web"));
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotListCommandTest.Valid(b => b.VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("list");
        args.ShouldContain("web");
    }
}

public class VagrantSnapshotPopCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotPopCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotPopCommandTest.Valid(b => b
            .VmName("web")
            .NoDelete()
            .NoProvision()
            .NoStart()
            .Provision()
            .ProvisionWith("shell", "puppet"));

        cmd.VmName.ShouldBe("web");
        cmd.NoDelete.ShouldBeTrue();
        cmd.NoProvision.ShouldBeTrue();
        cmd.NoStart.ShouldBeTrue();
        cmd.Provision.ShouldBeTrue();
        cmd.ProvisionWith.ShouldContain("shell");
        cmd.ProvisionWith.ShouldContain("puppet");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotPopCommandTest.Valid(b => b.NoDelete().NoProvision());

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("pop");
        args.ShouldContain("--no-delete");
        args.ShouldContain("--no-provision");
    }
}

public class VagrantSnapshotPushCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotPushCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithVmName_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotPushCommandTest.Valid(b => b.VmName("web"));
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotPushCommandTest.Valid(b => b.VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("push");
        args.ShouldContain("web");
    }
}

public class VagrantSnapshotRestoreCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotRestoreCommandTest.Valid(b => b.Name("snapshot1"));
        cmd.Name.ShouldBe("snapshot1");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotRestoreCommandTest.Valid(b => b
            .Name("snapshot1")
            .VmName("web")
            .NoProvision()
            .NoStart()
            .Provision()
            .ProvisionWith("shell"));

        cmd.Name.ShouldBe("snapshot1");
        cmd.VmName.ShouldBe("web");
        cmd.NoProvision.ShouldBeTrue();
        cmd.NoStart.ShouldBeTrue();
        cmd.Provision.ShouldBeTrue();
        cmd.ProvisionWith.ShouldContain("shell");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantSnapshotRestoreCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyName_Fails()
    {
        VagrantSnapshotRestoreCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotRestoreCommandTest.Valid(b => b.Name("snapshot1").NoProvision());

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("restore");
        args.ShouldContain("--no-provision");
        args.ShouldContain("snapshot1");
    }
}

public class VagrantSnapshotSaveCommandBuilderTests
{
    [Fact]
    public void Valid_WithRequiredName_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotSaveCommandTest.Valid(b => b.Name("snapshot1"));
        cmd.Name.ShouldBe("snapshot1");
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSnapshotSaveCommandTest.Valid(b => b
            .Name("snapshot1")
            .VmName("web")
            .Force());

        cmd.Name.ShouldBe("snapshot1");
        cmd.VmName.ShouldBe("web");
        cmd.Force.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        VagrantSnapshotSaveCommandTest.Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptyName_Fails()
    {
        VagrantSnapshotSaveCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSnapshotSaveCommandTest.Valid(b => b.Name("snapshot1").Force());

        var args = cmd.ToArguments();
        args.ShouldContain("snapshot");
        args.ShouldContain("save");
        args.ShouldContain("--force");
        args.ShouldContain("snapshot1");
    }
}

#endregion

#region Top-Level Command Tests

public class VagrantPackageCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPackageCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPackageCommandTest.Valid(b => b
            .VmName("web")
            .Base("my-vm")
            .Include("file1.txt", "file2.txt")
            .Output("package.box")
            .Vagrantfile("/path/to/Vagrantfile"));

        cmd.VmName.ShouldBe("web");
        cmd.Base.ShouldBe("my-vm");
        cmd.Include.ShouldContain("file1.txt");
        cmd.Include.ShouldContain("file2.txt");
        cmd.Output.ShouldBe("package.box");
        cmd.Vagrantfile.ShouldBe("/path/to/Vagrantfile");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPackageCommandTest.Valid(b => b
            .Base("my-vm")
            .Output("package.box")
            .Include("file1.txt"));

        var args = cmd.ToArguments();
        args.ShouldContain("package");
        args.ShouldContain("--base");
        args.ShouldContain("my-vm");
        args.ShouldContain("--output");
        args.ShouldContain("package.box");
        args.ShouldContain("--include");
    }
}

public class VagrantPortCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPortCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPortCommandTest.Valid(b => b
            .VmName("web")
            .Guest(22)
            .MachineReadable());

        cmd.VmName.ShouldBe("web");
        cmd.Guest.ShouldBe(22);
        cmd.MachineReadable.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPortCommandTest.Valid(b => b.Guest(22).MachineReadable());

        var args = cmd.ToArguments();
        args.ShouldContain("port");
        args.ShouldContain("--guest");
        args.ShouldContain("22");
        args.ShouldContain("--machine-readable");
    }
}

public class VagrantPowerShellCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPowerShellCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantPowerShellCommandTest.Valid(b => b
            .VmName("web")
            .Command("Get-Process")
            .Elevated());

        cmd.VmName.ShouldBe("web");
        cmd.Command.ShouldBe("Get-Process");
        cmd.Elevated.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantPowerShellCommandTest.Valid(b => b.Command("Get-Process").Elevated());

        var args = cmd.ToArguments();
        args.ShouldContain("powershell");
        args.ShouldContain("--command");
        args.ShouldContain("Get-Process");
        args.ShouldContain("--elevated");
    }
}

public class VagrantProvisionCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantProvisionCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantProvisionCommandTest.Valid(b => b
            .VmName("web")
            .ProvisionWith("shell", "puppet"));

        cmd.VmName.ShouldBe("web");
        cmd.ProvisionWith.ShouldContain("shell");
        cmd.ProvisionWith.ShouldContain("puppet");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantProvisionCommandTest.Valid(b => b.ProvisionWith("shell"));

        var args = cmd.ToArguments();
        args.ShouldContain("provision");
        args.ShouldContain("--provision-with");
    }
}

public class VagrantRdpCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantRdpCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantRdpCommandTest.Valid(b => b
            .VmName("web")
            .ExtraArgs("/v:localhost"));

        cmd.VmName.ShouldBe("web");
        cmd.ExtraArgs.ShouldBe("/v:localhost");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantRdpCommandTest.Valid(b => b.ExtraArgs("/v:localhost"));

        var args = cmd.ToArguments();
        args.ShouldContain("rdp");
        args.ShouldContain("--");
        args.ShouldContain("/v:localhost");
    }
}

public class VagrantReloadCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantReloadCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantReloadCommandTest.Valid(b => b
            .VmName("web")
            .Force()
            .NoProvision()
            .Provision()
            .ProvisionWith("shell", "puppet"));

        cmd.VmName.ShouldBe("web");
        cmd.Force.ShouldBeTrue();
        cmd.NoProvision.ShouldBeTrue();
        cmd.Provision.ShouldBeTrue();
        cmd.ProvisionWith.ShouldContain("shell");
        cmd.ProvisionWith.ShouldContain("puppet");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantReloadCommandTest.Valid(b => b.Force().NoProvision());

        var args = cmd.ToArguments();
        args.ShouldContain("reload");
        args.ShouldContain("--force");
        args.ShouldContain("--no-provision");
    }
}

public class VagrantResumeCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantResumeCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantResumeCommandTest.Valid(b => b
            .VmName("web")
            .NoProvision()
            .Provision()
            .ProvisionWith("shell", "puppet"));

        cmd.VmName.ShouldBe("web");
        cmd.NoProvision.ShouldBeTrue();
        cmd.Provision.ShouldBeTrue();
        cmd.ProvisionWith.ShouldContain("shell");
        cmd.ProvisionWith.ShouldContain("puppet");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantResumeCommandTest.Valid(b => b.NoProvision().Provision());

        var args = cmd.ToArguments();
        args.ShouldContain("resume");
        args.ShouldContain("--no-provision");
        args.ShouldContain("--provision");
    }
}

public class VagrantSshConfigCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSshConfigCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSshConfigCommandTest.Valid(b => b
            .VmName("web")
            .Host("my-host"));

        cmd.VmName.ShouldBe("web");
        cmd.Host.ShouldBe("my-host");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSshConfigCommandTest.Valid(b => b.Host("my-host"));

        var args = cmd.ToArguments();
        args.ShouldContain("ssh-config");
        args.ShouldContain("--host");
        args.ShouldContain("my-host");
    }
}

public class VagrantSuspendCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSuspendCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithVmName_BuildsSuccessfully()
    {
        var cmd = VagrantSuspendCommandTest.Valid(b => b.VmName("web"));
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSuspendCommandTest.Valid(b => b.VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("suspend");
        args.ShouldContain("web");
    }
}

public class VagrantWinrmCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantWinrmCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantWinrmCommandTest.Valid(b => b
            .VmName("web")
            .Command("Get-Process")
            .Elevated()
            .Shell("powershell"));

        cmd.VmName.ShouldBe("web");
        cmd.Command.ShouldBe("Get-Process");
        cmd.Elevated.ShouldBeTrue();
        cmd.Shell.ShouldBe("powershell");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantWinrmCommandTest.Valid(b => b.Command("Get-Process").Elevated().Shell("powershell"));

        var args = cmd.ToArguments();
        args.ShouldContain("winrm");
        args.ShouldContain("--command");
        args.ShouldContain("Get-Process");
        args.ShouldContain("--elevated");
        args.ShouldContain("--shell");
        args.ShouldContain("powershell");
    }
}

public class VagrantWinrmConfigCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantWinrmConfigCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantWinrmConfigCommandTest.Valid(b => b
            .VmName("web")
            .Host("my-host"));

        cmd.VmName.ShouldBe("web");
        cmd.Host.ShouldBe("my-host");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantWinrmConfigCommandTest.Valid(b => b.Host("my-host"));

        var args = cmd.ToArguments();
        args.ShouldContain("winrm-config");
        args.ShouldContain("--host");
        args.ShouldContain("my-host");
    }
}

public class VagrantDestroyCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantDestroyCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantDestroyCommandTest.Valid(b => b
            .VmName("web")
            .Force()
            .Graceful()
            .Parallel());

        cmd.VmName.ShouldBe("web");
        cmd.Force.ShouldBeTrue();
        cmd.Graceful.ShouldBeTrue();
        cmd.Parallel.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantDestroyCommandTest.Valid(b => b.Force().Graceful());

        var args = cmd.ToArguments();
        args.ShouldContain("destroy");
        args.ShouldContain("--force");
        args.ShouldContain("--graceful");
    }
}

public class VagrantGlobalStatusCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantGlobalStatusCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithPrune_BuildsSuccessfully()
    {
        var cmd = VagrantGlobalStatusCommandTest.Valid(b => b.Prune());
        cmd.Prune.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantGlobalStatusCommandTest.Valid(b => b.Prune());

        var args = cmd.ToArguments();
        args.ShouldContain("global-status");
        args.ShouldContain("--prune");
    }
}

public class VagrantHaltCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantHaltCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantHaltCommandTest.Valid(b => b.VmName("web").Force());

        cmd.VmName.ShouldBe("web");
        cmd.Force.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantHaltCommandTest.Valid(b => b.Force());

        var args = cmd.ToArguments();
        args.ShouldContain("halt");
        args.ShouldContain("--force");
    }
}

public class VagrantInitCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantInitCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantInitCommandTest.Valid(b => b
            .BoxName("ubuntu/focal64")
            .BoxUrl("https://example.com/box.box")
            .BoxVersion("1.0.0")
            .Force()
            .Minimal()
            .Output("Vagrantfile")
            .Template("/path/to/template"));

        cmd.BoxName.ShouldBe("ubuntu/focal64");
        cmd.BoxUrl.ShouldBe("https://example.com/box.box");
        cmd.BoxVersion.ShouldBe("1.0.0");
        cmd.Force.ShouldBeTrue();
        cmd.Minimal.ShouldBeTrue();
        cmd.Output.ShouldBe("Vagrantfile");
        cmd.Template.ShouldBe("/path/to/template");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantInitCommandTest.Valid(b => b.BoxName("ubuntu/focal64").Minimal().Force());

        var args = cmd.ToArguments();
        args.ShouldContain("init");
        args.ShouldContain("--minimal");
        args.ShouldContain("--force");
        args.ShouldContain("ubuntu/focal64");
    }
}

public class VagrantUpCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantUpCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b
            .VmName("web")
            .DestroyOnError()
            .InstallProvider()
            .NoDestroyOnError()
            .NoInstallProvider()
            .NoParallel()
            .NoProvision()
            .Parallel()
            .Provider("virtualbox")
            .Provision()
            .ProvisionWith("shell", "puppet"));

        cmd.VmName.ShouldBe("web");
        cmd.DestroyOnError.ShouldBeTrue();
        cmd.InstallProvider.ShouldBeTrue();
        cmd.NoDestroyOnError.ShouldBeTrue();
        cmd.NoInstallProvider.ShouldBeTrue();
        cmd.NoParallel.ShouldBeTrue();
        cmd.NoProvision.ShouldBeTrue();
        cmd.Parallel.ShouldBeTrue();
        cmd.Provider.ShouldBe("virtualbox");
        cmd.Provision.ShouldBeTrue();
        cmd.ProvisionWith.ShouldContain("shell");
        cmd.ProvisionWith.ShouldContain("puppet");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b.Provider("virtualbox").NoProvision());

        var args = cmd.ToArguments();
        args.ShouldContain("up");
        args.ShouldContain("--provider");
        args.ShouldContain("virtualbox");
        args.ShouldContain("--no-provision");
    }
}

public class VagrantSshCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSshCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithAllOptions_BuildsSuccessfully()
    {
        var cmd = VagrantSshCommandTest.Valid(b => b
            .VmName("web")
            .Command("ls -la")
            .ExtraArgs("-o StrictHostKeyChecking=no")
            .Plain()
            .Tty());

        cmd.VmName.ShouldBe("web");
        cmd.Command.ShouldBe("ls -la");
        cmd.ExtraArgs.ShouldBe("-o StrictHostKeyChecking=no");
        cmd.Plain.ShouldBeTrue();
        cmd.Tty.ShouldBeTrue();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantSshCommandTest.Valid(b => b.Command("ls").Plain().Tty());

        var args = cmd.ToArguments();
        args.ShouldContain("ssh");
        args.ShouldContain("--command");
        args.ShouldContain("ls");
        args.ShouldContain("--plain");
        args.ShouldContain("--tty");
    }
}

public class VagrantStatusCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantStatusCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void Valid_WithVmName_BuildsSuccessfully()
    {
        var cmd = VagrantStatusCommandTest.Valid(b => b.VmName("web"));
        cmd.VmName.ShouldBe("web");
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantStatusCommandTest.Valid(b => b.VmName("web"));

        var args = cmd.ToArguments();
        args.ShouldContain("status");
        args.ShouldContain("web");
    }
}

public class VagrantVersionCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantVersionCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantVersionCommandTest.Valid(b => { });

        var args = cmd.ToArguments();
        args.ShouldContain("version");
    }
}

public class VagrantValidateCommandBuilderTests
{
    [Fact]
    public void Valid_WithNoOptions_BuildsSuccessfully()
    {
        var cmd = VagrantValidateCommandTest.Valid(b => { });
        cmd.ShouldNotBeNull();
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = VagrantValidateCommandTest.Valid(b => { });

        var args = cmd.ToArguments();
        args.ShouldContain("validate");
    }
}

#endregion

#region Additional Invalid Tests

public class AdditionalInvalidBuilderTests
{
    // Box Commands - Invalid tests for commands with required fields
    [Fact]
    public void VagrantBoxAddCommand_Invalid_WithWhitespaceBox()
    {
        VagrantBoxAddCommandTest.Invalid(b => b.Box("   "));
    }

    [Fact]
    public void VagrantBoxRemoveCommand_Invalid_WithWhitespaceName()
    {
        VagrantBoxRemoveCommandTest.Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyName()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyProvider()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyCartouche()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche(""));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceName()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("   ")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceProvider()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("   ")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceCartouche()
    {
        VagrantBoxRepackageCommandTest.Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche("   "));
    }

    // Cloud Commands - Invalid tests
    [Fact]
    public void VagrantCloudAuthCommand_Invalid_WithWhitespaceSubCommand()
    {
        VagrantCloudAuthCommandTest.Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithoutSubCommand()
    {
        VagrantCloudBoxCommandTest.Invalid(b => { });
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithEmptySubCommand()
    {
        VagrantCloudBoxCommandTest.Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithWhitespaceSubCommand()
    {
        VagrantCloudBoxCommandTest.Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudProviderCommand_Invalid_WithEmptySubCommand()
    {
        VagrantCloudProviderCommandTest.Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudProviderCommand_Invalid_WithWhitespaceSubCommand()
    {
        VagrantCloudProviderCommandTest.Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudVersionCommand_Invalid_WithEmptySubCommand()
    {
        VagrantCloudVersionCommandTest.Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudVersionCommand_Invalid_WithWhitespaceSubCommand()
    {
        VagrantCloudVersionCommandTest.Invalid(b => b.SubCommand("   "));
    }

    // Plugin Commands - Invalid tests
    [Fact]
    public void VagrantPluginInstallCommand_Invalid_WithEmptyName()
    {
        VagrantPluginInstallCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantPluginInstallCommand_Invalid_WithWhitespaceName()
    {
        VagrantPluginInstallCommandTest.Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithEmptyName()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b
            .Name("")
            .LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithWhitespaceName()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b
            .Name("   ")
            .LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithEmptyLicenseFile()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile(""));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithWhitespaceLicenseFile()
    {
        VagrantPluginLicenseCommandTest.Invalid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("   "));
    }

    [Fact]
    public void VagrantPluginUninstallCommand_Invalid_WithEmptyName()
    {
        VagrantPluginUninstallCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantPluginUninstallCommand_Invalid_WithWhitespaceName()
    {
        VagrantPluginUninstallCommandTest.Invalid(b => b.Name("   "));
    }

    // Snapshot Commands - Invalid tests
    [Fact]
    public void VagrantSnapshotDeleteCommand_Invalid_WithEmptyName()
    {
        VagrantSnapshotDeleteCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotDeleteCommand_Invalid_WithWhitespaceName()
    {
        VagrantSnapshotDeleteCommandTest.Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantSnapshotRestoreCommand_Invalid_WithEmptyName()
    {
        VagrantSnapshotRestoreCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotRestoreCommand_Invalid_WithWhitespaceName()
    {
        VagrantSnapshotRestoreCommandTest.Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantSnapshotSaveCommand_Invalid_WithEmptyName()
    {
        VagrantSnapshotSaveCommandTest.Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotSaveCommand_Invalid_WithWhitespaceName()
    {
        VagrantSnapshotSaveCommandTest.Invalid(b => b.Name("   "));
    }
}

#endregion

#region WorkingDirectory Builder Tests

public class WorkingDirectoryBuilderTests
{
    [Fact]
    public void VagrantBoxListCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxListCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantBoxOutdatedCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxOutdatedCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantBoxPruneCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxPruneCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantBoxRemoveCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxRemoveCommandTest.Valid(b => b.Name("test").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantBoxRepackageCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxRepackageCommandTest.Valid(b => b
            .Name("test")
            .Provider("virtualbox")
            .Cartouche("1.0.0")
            .WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantBoxUpdateCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantBoxUpdateCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantCloudAuthCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantCloudAuthCommandTest.Valid(b => b.SubCommand("login").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantCloudBoxCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantCloudBoxCommandTest.Valid(b => b.SubCommand("show").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantCloudProviderCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantCloudProviderCommandTest.Valid(b => b.SubCommand("create").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantCloudSearchCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantCloudSearchCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantCloudVersionCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantCloudVersionCommandTest.Valid(b => b.SubCommand("create").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantDestroyCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantDestroyCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantGlobalStatusCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantGlobalStatusCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantHaltCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantHaltCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantInitCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantInitCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPackageCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPackageCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginInstallCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginInstallCommandTest.Valid(b => b.Name("test").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginLicenseCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginLicenseCommandTest.Valid(b => b.Name("test").LicenseFile("/lic").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginListCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginListCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginRepairCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginRepairCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginUninstallCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginUninstallCommandTest.Valid(b => b.Name("test").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPluginUpdateCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPluginUpdateCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPortCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPortCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantPowerShellCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantPowerShellCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantProvisionCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantProvisionCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantRdpCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantRdpCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantReloadCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantReloadCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantResumeCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantResumeCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotDeleteCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotDeleteCommandTest.Valid(b => b.Name("snap").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotListCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotListCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotPopCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotPopCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotPushCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotPushCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotRestoreCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotRestoreCommandTest.Valid(b => b.Name("snap").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSnapshotSaveCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSnapshotSaveCommandTest.Valid(b => b.Name("snap").WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSshCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSshCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSshConfigCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSshConfigCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantStatusCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantStatusCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantSuspendCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantSuspendCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantUpCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantValidateCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantValidateCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantVersionCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantVersionCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantWinrmCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantWinrmCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }

    [Fact]
    public void VagrantWinrmConfigCommandBuilder_WorkingDirectory()
    {
        var cmd = VagrantWinrmConfigCommandTest.Valid(b => b.WorkingDirectory("/tmp"));
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }
}

#endregion

#region Command Line Generation Tests

public class VagrantCommandLineTests
{
    [Fact]
    public void ToCommandLine_GeneratesCorrectCommandLine()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b.VmName("web").Provider("virtualbox").NoProvision());

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldStartWith("vagrant");
        commandLine.ShouldContain("up");
        commandLine.ShouldContain("--provider");
        commandLine.ShouldContain("virtualbox");
        commandLine.ShouldContain("--no-provision");
        commandLine.ShouldContain("web");
    }

    [Fact]
    public void ToCommandLine_QuotesArgumentsWithSpaces()
    {
        var cmd = VagrantSshCommandTest.Valid(b => b.Command("ls -la /home"));

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldContain("\"ls -la /home\"");
    }

    [Fact]
    public void ToCommandLine_QuotesEmptyString()
    {
        var cmd = new VagrantSshCommand { Command = "" };
        var commandLine = cmd.ToCommandLine();
        commandLine.ShouldContain("ssh");
    }

    [Fact]
    public void ToCommandLine_EscapesQuotesInArguments()
    {
        var cmd = new VagrantSshCommand { Command = "echo \"hello\"" };
        var commandLine = cmd.ToCommandLine();
        commandLine.ShouldContain("\\\"hello\\\"");
    }
}

#endregion

#region ProcessStartInfo Tests

public class VagrantProcessStartInfoTests
{
    [Fact]
    public void ToProcessStartInfo_SetsCorrectExecutable()
    {
        var cmd = VagrantUpCommandTest.Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        
        psi.FileName.ShouldBe("vagrant");
    }

    [Fact]
    public void ToProcessStartInfo_SetsWorkingDirectory()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b.WorkingDirectory("/tmp/vagrant"));
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        
        psi.WorkingDirectory.ShouldBe("/tmp/vagrant");
    }

    [Fact]
    public void ToProcessStartInfo_SetsWorkingDirectoryFromParameter()
    {
        var cmd = VagrantUpCommandTest.Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo("/custom/path");
        
        psi.WorkingDirectory.ShouldBe("/custom/path");
    }

    [Fact]
    public void ToProcessStartInfo_IncludesArguments()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b.Provider("virtualbox").NoProvision());
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        
        psi.ArgumentList.ShouldContain("up");
        psi.ArgumentList.ShouldContain("--provider");
        psi.ArgumentList.ShouldContain("virtualbox");
        psi.ArgumentList.ShouldContain("--no-provision");
    }

    [Fact]
    public void ToProcessStartInfo_SetsRedirection()
    {
        var cmd = VagrantUpCommandTest.Valid(b => { });
        var psi = ((IVagrantCommand)cmd).ToProcessStartInfo();
        
        psi.RedirectStandardOutput.ShouldBeTrue();
        psi.RedirectStandardError.ShouldBeTrue();
        psi.RedirectStandardInput.ShouldBeTrue();
        psi.UseShellExecute.ShouldBeFalse();
    }
}

#endregion

#region Environment Variable Tests

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

#endregion
