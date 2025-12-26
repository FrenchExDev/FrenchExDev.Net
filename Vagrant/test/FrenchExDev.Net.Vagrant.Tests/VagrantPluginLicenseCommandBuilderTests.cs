using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantPluginLicenseCommandBuilderTests : VagrantCommandTest<VagrantPluginLicenseCommandBuilder, VagrantPluginLicenseCommand>
{
    [Fact]
    public void Valid_WithRequiredFields_BuildsSuccessfully()
    {
        var cmd = Valid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("/path/to/license.lic"));

        cmd.Name.ShouldBe("vagrant-vmware-desktop");
        cmd.LicenseFile.ShouldBe("/path/to/license.lic");
    }

    [Fact]
    public void Invalid_WithoutName_Fails()
    {
        Invalid(b => b.LicenseFile("/path/to/license.lic"));
    }

    [Fact]
    public void Invalid_WithoutLicenseFile_Fails()
    {
        Invalid(b => b.Name("vagrant-vmware-desktop"));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b
            .Name("vagrant-vmware-desktop")
            .LicenseFile("/path/to/license.lic"));

        var args = cmd.ToArguments();
        args.ShouldContain("plugin");
        args.ShouldContain("license");
        args.ShouldContain("vagrant-vmware-desktop");
        args.ShouldContain("/path/to/license.lic");
    }
}
