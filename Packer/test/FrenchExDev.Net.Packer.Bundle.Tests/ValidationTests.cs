using FrenchExDev.Net.CSharp.Object.Builder;
using Shouldly;

namespace FrenchExDev.Net.Packer.Bundle.Tests;

/// <summary>
/// Tests for ValidateInternal methods in Packer.Bundle builders to ensure validation logic is covered.
/// Now that ValidateInternal uses 'override' instead of 'new', polymorphic dispatch works correctly.
/// </summary>
public class ValidationTests
{
    #region PackerBuilderBuilder Validation Tests

    [Fact]
    public void PackerBuilderBuilder_Validate_Reports_All_Missing_Fields()
    {
        var builder = new PackerBuilderBuilder();
        // All fields missing

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // Should report many missing fields
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PackerBuilderBuilder_Validate_Passes_With_All_Required_Fields()
    {
        var builder = CreateFullyConfiguredPackerBuilderBuilder();

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    [Fact]
    public void PackerBuilderBuilder_Build_Succeeds_With_All_Fields()
    {
        var builder = CreateFullyConfiguredPackerBuilderBuilder();

        var result = builder.Build();

        result.IsSuccess<PackerBuilder>().ShouldBeTrue();
    }

    #endregion

    #region ProvisionerBuilder Validation Tests

    [Fact]
    public void ProvisionerBuilder_Validate_Reports_Missing_Type()
    {
        var builder = new ProvisionerBuilder()
            .AddScript("script.sh");
        // Missing Type

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("_type").ShouldBeTrue();
    }

    [Fact]
    public void ProvisionerBuilder_Validate_Passes_With_Type()
    {
        var builder = new ProvisionerBuilder()
            .Type("shell")
            .AddScript("script.sh");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    [Fact]
    public void ProvisionerBuilder_Build_Fails_Without_Type()
    {
        var builder = new ProvisionerBuilder()
            .AddScript("script.sh");

        var result = builder.Build();

        result.IsFailure().ShouldBeTrue();
    }

    #endregion

    #region PostProcessorBuilder Validation Tests

    [Fact]
    public void PostProcessorBuilder_Validate_Reports_Missing_CompressionLevel()
    {
        // CompressionLevel uses AssertNotNull which fails on null
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .Output("output.box")
            .VagrantfileTemplate("template.tpl");
        // Missing CompressionLevel

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("CompressionLevel").ShouldBeTrue();
    }

    [Fact]
    public void PostProcessorBuilder_Validate_Reports_Empty_Output()
    {
        // AssertNotEmptyOrWhitespace fails on empty string (not null)
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .CompressionLevel(6)
            .Output("")  // Empty string triggers failure
            .VagrantfileTemplate("template.tpl");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("Output").ShouldBeTrue();
    }

    [Fact]
    public void PostProcessorBuilder_Validate_Reports_Empty_Type()
    {
        var builder = new PostProcessorBuilder()
            .Type("")  // Empty string triggers failure
            .CompressionLevel(6)
            .Output("output.box")
            .VagrantfileTemplate("template.tpl");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("Type").ShouldBeTrue();
    }

    [Fact]
    public void PostProcessorBuilder_Validate_Reports_Empty_VagrantfileTemplate()
    {
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .CompressionLevel(6)
            .Output("output.box")
            .VagrantfileTemplate("");  // Empty string triggers failure

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("VagrantfileTemplate").ShouldBeTrue();
    }

    [Fact]
    public void PostProcessorBuilder_Validate_Passes_With_All_Required_Fields()
    {
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .CompressionLevel(6)
            .Output("output.box")
            .VagrantfileTemplate("template.tpl");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    #endregion

    #region VirtualBoxIsoProvisionerOverrideBuilder Validation Tests

    [Fact]
    public void VirtualBoxIsoProvisionerOverrideBuilder_Validate_Does_Not_Fail_On_Null()
    {
        // AssertNotEmptyOrWhitespace returns early on null without failure
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder();
        // ExecuteCommand is null

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // Null doesn't trigger failure with AssertNotEmptyOrWhitespace
        failures.Count.ShouldBe(0);
    }

    [Fact]
    public void VirtualBoxIsoProvisionerOverrideBuilder_Validate_Reports_Empty_ExecuteCommand()
    {
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
            .ExecuteCommand("");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("ExecuteCommand").ShouldBeTrue();
    }

    [Fact]
    public void VirtualBoxIsoProvisionerOverrideBuilder_Validate_Reports_Whitespace_ExecuteCommand()
    {
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
            .ExecuteCommand("   ");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("ExecuteCommand").ShouldBeTrue();
    }

    [Fact]
    public void VirtualBoxIsoProvisionerOverrideBuilder_Validate_Passes_With_ExecuteCommand()
    {
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
            .ExecuteCommand("echo test");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    #endregion

    #region VagrantDirectoryBuilder Validation Tests

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Reports_Empty_Extension()
    {
        var fileWithEmptyExtension = new File(new List<string>(), "\n", "test", "", "/path");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("test.txt", fileWithEmptyExtension);

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("Extension").ShouldBeTrue();
    }

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Reports_Empty_Path()
    {
        var fileWithEmptyPath = new File(new List<string>(), "\n", "test", ".txt", "");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("test.txt", fileWithEmptyPath);

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("Path").ShouldBeTrue();
    }

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Reports_Empty_NewLine()
    {
        var fileWithEmptyNewLine = new File(new List<string>(), "", "test", ".txt", "/path");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("test.txt", fileWithEmptyNewLine);

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.ContainsKey("NewLine").ShouldBeTrue();
    }

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Reports_Multiple_Issues()
    {
        var fileWithMultipleIssues = new File(new List<string>(), "", "test", "", "");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("test.txt", fileWithMultipleIssues);

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // Should report all three issues
        failures.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Passes_With_Valid_File()
    {
        var validFile = new File(new List<string>(), "\n", "Vagrantfile", ".rb", "/vagrant");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("Vagrantfile", validFile);

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    [Fact]
    public void VagrantDirectoryBuilder_Validate_Passes_With_No_Files()
    {
        var builder = new VagrantDirectoryBuilder();

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    #endregion

    #region PackerFileBuilder Validation Tests

    [Fact]
    public void PackerFileBuilder_Validate_Propagates_Builder_Failures()
    {
        var builder = new PackerFileBuilder()
            .Builder(b => { /* Empty builder - will have validation failures */ })
            .Description("Test");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // PackerBuilderBuilder validation failures should be collected
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PackerFileBuilder_Validate_Propagates_Provisioner_Failures()
    {
        var builder = new PackerFileBuilder()
            .Provisioner(p => { /* Empty provisioner - missing type */ })
            .Description("Test");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // ProvisionerBuilder validation failures should be collected
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PackerFileBuilder_Validate_Propagates_PostProcessor_Failures()
    {
        var builder = new PackerFileBuilder()
            .PostProcessor(pp => { /* Empty post-processor - missing CompressionLevel */ })
            .Description("Test");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // PostProcessorBuilder validation failures should be collected (CompressionLevel is null)
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PackerFileBuilder_Validate_Passes_With_No_Nested_Builders()
    {
        var builder = new PackerFileBuilder()
            .Description("Test");

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    #endregion

    #region PackerBundleBuilder Validation Tests

    [Fact]
    public void PackerBundleBuilder_Validate_Propagates_Script_Failures()
    {
        var builder = new PackerBundleBuilder()
            .Script("invalid", sb => { /* No name set - will throw in Instantiate */ });

        // The ShellScriptBuilder throws InvalidDataException in Instantiate when name is missing
        // This causes Build() to throw an exception
        Should.Throw<InvalidDataException>(() => builder.Build());
    }

    [Fact]
    public void PackerBundleBuilder_Validate_Passes_With_Valid_Configuration()
    {
        var builder = new PackerBundleBuilder()
            .Script("valid", sb => sb.Name("valid.sh").AddLine("echo test"))
            .Directory("/tmp")
            .Plugin("plugin");

        var result = builder.Build();
        result.IsSuccess<PackerBundle>().ShouldBeTrue();
    }

    [Fact]
    public void PackerBundleBuilder_Validate_Passes_With_Empty_Config()
    {
        var builder = new PackerBundleBuilder();

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        failures.Count.ShouldBe(0);
    }

    #endregion

    #region ShellScriptBuilder Validation Tests

    [Fact]
    public void ShellScriptBuilder_Build_Fails_Without_Name()
    {
        var builder = new ShellScriptBuilder()
            .AddLine("echo test");

        Should.Throw<InvalidDataException>(() => builder.BuildSuccess());
    }

    [Fact]
    public void ShellScriptBuilder_Build_Succeeds_With_Name()
    {
        var builder = new ShellScriptBuilder()
            .Name("test.sh")
            .AddLine("echo test");

        var script = builder.BuildSuccess();
        script.Name.ShouldBe("test.sh");
    }

    #endregion

    #region Integration Validation Tests

    [Fact]
    public void Full_PackerFile_Validation_With_Invalid_Nested_Builders()
    {
        var builder = new PackerFileBuilder()
            .Description("Test")
            .Builder(b => b.Type("virtualbox-iso")) // Missing many required fields
            .Provisioner(p => p.AddScript("script.sh")) // Missing type
            .PostProcessor(pp => pp.Type("vagrant")); // Missing CompressionLevel

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        builder.Validate(visited, failures);

        // Should have failures from nested builders
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Validation_Only_Runs_Once_Per_Builder()
    {
        var builder = new ProvisionerBuilder().Type("shell");
        var visited = new VisitedObjectDictionary();

        // First validation
        var failures1 = new FailuresDictionary();
        builder.Validate(visited, failures1);

        // Second validation should be skipped (status is Validated)
        var failures2 = new FailuresDictionary();
        builder.Validate(visited, failures2);

        // Both should be empty since builder is valid
        failures1.Count.ShouldBe(0);
        failures2.Count.ShouldBe(0);
    }

    [Fact]
    public void Build_Uses_Validation()
    {
        // Build internally calls Validate, and with override working, validation is exercised
        var builder = new ProvisionerBuilder(); // Missing type

        var result = builder.Build();

        result.IsFailure().ShouldBeTrue();
    }

    #endregion

    #region Helper Methods

    private static PackerBuilderBuilder CreateFullyConfiguredPackerBuilderBuilder()
    {
        return new PackerBuilderBuilder()
            .AddBootCommand("<wait>")
            .BootWait("10s")
            .Communicator("ssh")
            .DiskSize("20000")
            .Format("ova")
            .GuestOsType("Linux_64")
            .HttpDirectory("http")
            .IsoChecksum("sha256:abc123")
            .AddIsoUrl("http://example.com/iso.iso")
            .GuestAdditionUrl("http://example.com/ga.iso")
            .GuestAdditionSha256("abc123")
            .GuestAdditionPath("/tmp/ga.iso")
            .GuestAdditionMode("upload")
            .VirtualBoxVersionFile("VBoxVersion.txt")
            .ShutdownCommand("poweroff")
            .SshPassword("password")
            .SshTimeout("10m")
            .SshUsername("root")
            .Type("virtualbox-iso")
            .ModifyVm("--memory", "1024")
            .VmName("test-vm")
            .HardDriveInterface("sata");
    }

    #endregion
}
