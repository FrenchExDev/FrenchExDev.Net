using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class AdditionalInvalidBuilderTests
{
    // Box Commands - Invalid tests for commands with required fields
    [Fact]
    public void VagrantBoxAddCommand_Invalid_WithWhitespaceBox()
    {
        (new VagrantCommandTest<VagrantBoxAddCommandBuilder, VagrantBoxAddCommand>()).Invalid(b => b.Box("   "));
    }

    [Fact]
    public void VagrantBoxRemoveCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantBoxRemoveCommandBuilder, VagrantBoxRemoveCommand>()).Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyProvider()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithEmptyCartouche()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche(""));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("   ")
            .Provider("virtualbox")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceProvider()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("   ")
            .Cartouche("1.0.0"));
    }

    [Fact]
    public void VagrantBoxRepackageCommand_Invalid_WithWhitespaceCartouche()
    {
        (new VagrantCommandTest<VagrantBoxRepackageCommandBuilder, VagrantBoxRepackageCommand>()).Invalid(b => b
            .Name("ubuntu/focal64")
            .Provider("virtualbox")
            .Cartouche("   "));
    }

    // Cloud Commands - Invalid tests
    [Fact]
    public void VagrantCloudAuthCommand_Invalid_WithWhitespaceSubCommand()
    {
        (new VagrantCommandTest<VagrantCloudAuthCommandBuilder, VagrantCloudAuthCommand>()).Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithoutSubCommand()
    {
        (new VagrantCommandTest<VagrantCloudBoxCommandBuilder, VagrantCloudBoxCommand>()).Invalid(b => { });
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithEmptySubCommand()
    {
        (new VagrantCommandTest<VagrantCloudBoxCommandBuilder, VagrantCloudBoxCommand>()).Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudBoxCommand_Invalid_WithWhitespaceSubCommand()
    {
        (new VagrantCommandTest<VagrantCloudBoxCommandBuilder, VagrantCloudBoxCommand>()).Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudProviderCommand_Invalid_WithEmptySubCommand()
    {
        (new VagrantCommandTest<VagrantCloudProviderCommandBuilder, VagrantCloudProviderCommand>()).Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudProviderCommand_Invalid_WithWhitespaceSubCommand()
    {
        (new VagrantCommandTest<VagrantCloudProviderCommandBuilder, VagrantCloudProviderCommand>()).Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void VagrantCloudVersionCommand_Invalid_WithEmptySubCommand()
    {
        (new VagrantCommandTest<VagrantCloudVersionCommandBuilder, VagrantCloudVersionCommand>()).Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void VagrantCloudVersionCommand_Invalid_WithWhitespaceSubCommand()
    {
        (new VagrantCommandTest<VagrantCloudVersionCommandBuilder, VagrantCloudVersionCommand>()).Invalid(b => b.SubCommand("   "));
    }

    // Plugin Commands - Invalid tests
    [Fact]
    public void VagrantPluginInstallCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantPluginInstallCommandBuilder, VagrantPluginInstallCommand>()).Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantPluginInstallCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantPluginInstallCommandBuilder, VagrantPluginInstallCommand>()).Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantPluginLicenseCommandBuilder, VagrantPluginLicenseCommand>()).Invalid(b => b
            .Name("")
            .LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantPluginLicenseCommandBuilder, VagrantPluginLicenseCommand>()).Invalid(b => b
            .Name("   ")
            .LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithEmptyLicenseFile()
    {
        (new VagrantCommandTest<VagrantPluginLicenseCommandBuilder, VagrantPluginLicenseCommand>()).Invalid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile(""));
    }

    [Fact]
    public void VagrantPluginLicenseCommand_Invalid_WithWhitespaceLicenseFile()
    {
        (new VagrantCommandTest<VagrantPluginLicenseCommandBuilder, VagrantPluginLicenseCommand>()).Invalid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("   "));
    }

    [Fact]
    public void VagrantPluginUninstallCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantPluginUninstallCommandBuilder, VagrantPluginUninstallCommand>()).Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantPluginUninstallCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantPluginUninstallCommandBuilder, VagrantPluginUninstallCommand>()).Invalid(b => b.Name("   "));
    }

    // Snapshot Commands - Invalid tests
    [Fact]
    public void VagrantSnapshotDeleteCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantSnapshotDeleteCommandBuilder, VagrantSnapshotDeleteCommand>()).Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotDeleteCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantSnapshotDeleteCommandBuilder, VagrantSnapshotDeleteCommand>()).Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantSnapshotRestoreCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantSnapshotRestoreCommandBuilder, VagrantSnapshotRestoreCommand>()).Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotRestoreCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantSnapshotRestoreCommandBuilder, VagrantSnapshotRestoreCommand>()).Invalid(b => b.Name("   "));
    }

    [Fact]
    public void VagrantSnapshotSaveCommand_Invalid_WithEmptyName()
    {
        (new VagrantCommandTest<VagrantSnapshotSaveCommandBuilder, VagrantSnapshotSaveCommand>()).Invalid(b => b.Name(""));
    }

    [Fact]
    public void VagrantSnapshotSaveCommand_Invalid_WithWhitespaceName()
    {
        (new VagrantCommandTest<VagrantSnapshotSaveCommandBuilder, VagrantSnapshotSaveCommand>()).Invalid(b => b.Name("   "));
    }
}
