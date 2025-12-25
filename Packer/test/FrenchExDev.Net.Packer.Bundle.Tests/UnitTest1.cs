using FrenchExDev.Net.CSharp.Object.Builder2;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace FrenchExDev.Net.Packer.Bundle.Tests;

#region ShellScript Tests

public class ShellScriptTests
{
    [Fact]
    public void ShellScript_Has_Required_Properties()
    {
        var script = new ShellScript { Name = "test.sh" };
        
        script.Name.ShouldBe("test.sh");
        script.Lines.ShouldBeEmpty();
        script.NewLine.ShouldBe("\n");
    }

    [Fact]
    public void ShellScript_Lines_Can_Be_Modified()
    {
        var script = new ShellScript { Name = "test.sh" };
        script.Lines.Add("echo hello");
        script.Lines.Add("echo world");
        
        script.Lines.Count.ShouldBe(2);
        script.Lines[0].ShouldBe("echo hello");
    }

    [Fact]
    public void ShellScript_NewLine_Can_Be_Changed()
    {
        var script = new ShellScript { Name = "test.sh" };
        script.NewLine = "\r\n";
        
        script.NewLine.ShouldBe("\r\n");
    }
}

#endregion

#region ShellScriptBuilder Tests

public class ShellScriptBuilderTests
{
    [Fact]
    public void Build_Creates_ShellScript_With_Name()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh");
        
        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var script = result.Value.Resolved();
        script.Name.ShouldBe("setup.sh");
    }

    [Fact]
    public void AddLine_Adds_Single_Line()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh")
            .AddLine("echo hello");
        
        var script = builder.Build().Value.Resolved();
        
        script.Lines.ShouldContain("echo hello");
    }

    [Fact]
    public void AddLines_Array_Adds_Multiple_Lines()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh")
            .AddLines(new[] { "echo hello", "echo world" });

        var script = builder.Build().Value.Resolved();

        script.Lines.ShouldContain("echo hello");
        script.Lines.ShouldContain("echo world");
    }

    [Fact]
    public void AddLines_String_Splits_By_EOL()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh")
            .AddLines("echo hello\necho world", "\n");

        var script = builder.Build().Value.Resolved();

        script.Lines.ShouldContain("echo hello");
        script.Lines.ShouldContain("echo world");
    }

    [Fact]
    public void Set_Adds_Set_Command_As_First_Line()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh")
            .Set("-e")
            .AddLine("echo hello");

        var script = builder.Build().Value.Resolved();

        script.Lines[0].ShouldBe("set -e");
        script.Lines[1].ShouldBe("echo hello");
    }

    [Fact]
    public void SetNewLine_Changes_NewLine_Character()
    {
        var builder = new ShellScriptBuilder()
            .Name("setup.sh")
            .SetNewLine("\r\n");

        var script = builder.Build().Value.Resolved();

        script.NewLine.ShouldBe("\r\n");
    }
}

#endregion

#region ShellScriptBuildersDictionary Tests

public class ShellScriptBuildersDictionaryTests
{
    [Fact]
    public void Build_Creates_ScriptDictionary()
    {
        var dict = new ShellScriptBuildersDictionary();
        var builder = new ShellScriptBuilder().Name("test.sh").AddLine("echo test");
        dict.Add("test", builder);
        
        var result = dict.Build();
        
        result.ShouldContainKey("test");
        result["test"].ShouldBeOfType<ShellScript>();
    }

    [Fact]
    public void Build_With_Multiple_Scripts()
    {
        var dict = new ShellScriptBuildersDictionary();
        dict.Add("script1", new ShellScriptBuilder().Name("s1.sh"));
        dict.Add("script2", new ShellScriptBuilder().Name("s2.sh"));
        
        var result = dict.Build();
        
        result.Count.ShouldBe(2);
        result.ShouldContainKey("script1");
        result.ShouldContainKey("script2");
    }
}

#endregion

#region File Tests

public class FileTests
{
    [Fact]
    public void File_Constructor_Sets_Properties()
    {
        var file = new File(new List<string> { "line1" }, "\n", "test", ".txt", "/path");
        
        file.Lines.ShouldContain("line1");
        file.NewLine.ShouldBe("\n");
        file.Name.ShouldBe("test");
        file.Extension.ShouldBe(".txt");
        file.Path.ShouldBe("/path");
    }

    [Fact]
    public void AddLine_Adds_Single_Line()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.AddLine("test line");
        
        file.Lines.ShouldContain("test line");
    }

    [Fact]
    public void AddLines_Array_Adds_Multiple_Lines()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.AddLines(new[] { "line1", "line2" });
        
        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void AddLines_List_Adds_Multiple_Lines()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.AddLines(new List<string> { "line1", "line2" });
        
        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void AddLines_String_Splits_By_EOL()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.AddLines("line1\nline2", "\n");
        
        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void SetNewLine_Changes_NewLine()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.SetNewLine("\r\n");
        
        file.NewLine.ShouldBe("\r\n");
    }

    [Fact]
    public void AppendLine_Is_Alias_For_AddLine()
    {
        var file = new File(new List<string>(), "\n", "", "", "");
        file.AppendLine("test");
        
        file.Lines.ShouldContain("test");
    }
}

#endregion

#region FileBuilder Tests

public class FileBuilderTests
{
    [Fact]
    public void Build_Creates_File_With_Defaults()
    {
        var builder = new FileBuilder();
        var file = builder.Build().Value.Resolved();
        
        file.NewLine.ShouldBe("\n");
        file.Name.ShouldBeEmpty();
        file.Extension.ShouldBeEmpty();
        file.Path.ShouldBeEmpty();
    }

    [Fact]
    public void AddLine_Adds_To_File()
    {
        var builder = new FileBuilder()
            .AddLine("test line");
        
        var file = builder.Build().Value.Resolved();
        
        file.Lines.ShouldContain("test line");
    }

    [Fact]
    public void AddLines_Array_Adds_Multiple()
    {
        var builder = new FileBuilder()
            .AddLines(new[] { "line1", "line2" });

        var file = builder.Build().Value.Resolved();

        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void AddLines_List_Adds_Multiple()
    {
        var builder = new FileBuilder()
            .AddLines(new List<string> { "line1", "line2" });

        var file = builder.Build().Value.Resolved();

        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void AddLines_String_Splits()
    {
        var builder = new FileBuilder()
            .AddLines("line1;line2", ";");

        var file = builder.Build().Value.Resolved();

        file.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void SetNewLine_Changes_NewLine()
    {
        var builder = new FileBuilder()
            .SetNewLine("\r\n");

        var file = builder.Build().Value.Resolved();

        file.NewLine.ShouldBe("\r\n");
    }

    [Fact]
    public void AppendLine_Is_Alias_For_AddLine()
    {
        var builder = new FileBuilder()
            .AppendLine("test");

        var file = builder.Build().Value.Resolved();

        file.Lines.ShouldContain("test");
    }
}

#endregion

#region HttpDirectory Tests

public class HttpDirectoryTests
{
    [Fact]
    public void HttpDirectory_Has_Empty_Files_By_Default()
    {
        var dir = new HttpDirectory();
        
        dir.Files.ShouldBeEmpty();
    }

    [Fact]
    public void Files_Can_Be_Added()
    {
        var dir = new HttpDirectory();
        var file = new File(new List<string>(), "\n", "test", ".txt", "/");
        dir.Files.Add("test.txt", file);
        
        dir.Files.ShouldContainKey("test.txt");
    }
}

#endregion

#region HttpDirectoryBuilder Tests

public class HttpDirectoryBuilderTests
{
    [Fact]
    public void Build_Creates_Empty_Directory()
    {
        var builder = new HttpDirectoryBuilder();
        var dir = builder.Build().Value.Resolved();
        
        dir.Files.ShouldBeEmpty();
    }

    [Fact]
    public void AddFile_Adds_File_To_Directory()
    {
        var file = new File(new List<string>(), "\n", "test", ".txt", "/");
        var builder = new HttpDirectoryBuilder()
            .AddFile("test.txt", file);
        
        var dir = builder.Build().Value.Resolved();
        
        dir.Files.ShouldContainKey("test.txt");
    }

    [Fact]
    public void RemoveFile_Removes_File_From_Directory()
    {
        var file = new File(new List<string>(), "\n", "test", ".txt", "/");
        var builder = new HttpDirectoryBuilder()
            .AddFile("test.txt", file)
            .RemoveFile("test.txt");

        var dir = builder.Build().Value.Resolved();

        dir.Files.ShouldNotContainKey("test.txt");
    }
}

#endregion

#region VagrantDirectory Tests

public class VagrantDirectoryTests
{
    [Fact]
    public void VagrantDirectory_Has_Empty_Files_By_Default()
    {
        var dir = new VagrantDirectory();
        
        dir.Files.ShouldBeEmpty();
    }
}

#endregion

#region VagrantDirectoryBuilder Tests

public class VagrantDirectoryBuilderTests
{
    [Fact]
    public void Build_Creates_Empty_Directory()
    {
        var builder = new VagrantDirectoryBuilder();
        var dir = builder.Build().Value.Resolved();
        
        dir.Files.ShouldBeEmpty();
    }

    [Fact]
    public void AddFile_Adds_File_To_Directory()
    {
        // File must have valid extension and path to pass validation
        var file = new File(new List<string>(), "\n", "Vagrantfile", ".rb", "/vagrant");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("Vagrantfile", file);

        var dir = builder.Build().Value.Resolved();

        dir.Files.ShouldContainKey("Vagrantfile");
    }

    [Fact]
    public void RemoveFile_Removes_File_From_Directory()
    {
        var file = new File(new List<string>(), "\n", "Vagrantfile", ".rb", "/vagrant");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("Vagrantfile", file)
            .RemoveFile("Vagrantfile");

        var dir = builder.Build().Value.Resolved();

        dir.Files.ShouldNotContainKey("Vagrantfile");
    }
}

#endregion

#region PackerFile Tests

public class PackerFileTests
{
    [Fact]
    public void PackerFile_Constructor_Sets_Properties()
    {
        var builders = new List<PackerBuilder>();
        var provisioners = new List<Provisioner>();
        var postProcessors = new List<PostProcessor>();
        var variables = new Dictionary<string, string> { { "key", "value" } };
        
        var packerFile = new PackerFile(builders, "Test description", provisioners, postProcessors, variables);
        
        packerFile.Builders.ShouldBeSameAs(builders);
        packerFile.Description.ShouldBe("Test description");
        packerFile.Provisioners.ShouldBeSameAs(provisioners);
        packerFile.PostProcessors.ShouldBeSameAs(postProcessors);
        packerFile.Variables.ShouldBeSameAs(variables);
    }
}

#endregion

#region PackerFileBuilder Tests

public class PackerFileBuilderTests
{
    [Fact]
    public void Build_Creates_PackerFile_With_Description()
    {
        var builder = new PackerFileBuilder()
            .Description("Test build");
        
        var result = builder.Build();
        
        result.IsSuccess.ShouldBeTrue();
        var pf = result.Value.Resolved();
        pf.Description.ShouldBe("Test build");
    }

    [Fact]
    public void Builder_Adds_PackerBuilder()
    {
        // Must provide required fields for PackerBuilderBuilder validation
        var builder = new PackerFileBuilder()
            .Builder(b => b
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
                .VmName("test-vm")
                .HardDriveInterface("sata")
                .ModifyVm("--cpus", "1"));

        var pf = builder.Build().Value.Resolved();

        pf.Builders.ShouldNotBeNull();
        pf.Builders!.Count.ShouldBe(1);
    }

    [Fact]
    public void Provisioner_Adds_Provisioner()
    {
        var builder = new PackerFileBuilder()
            .Provisioner(p => p.Type("shell"));

        var pf = builder.Build().Value.Resolved();

        pf.Provisioners.ShouldNotBeNull();
        pf.Provisioners!.Count.ShouldBe(1);
    }

    [Fact]
    public void PostProcessor_Adds_PostProcessor()
    {
        // PostProcessor validation requires CompressionLevel
        var builder = new PackerFileBuilder()
            .PostProcessor(pp => pp.Type("vagrant").CompressionLevel(6));

        var pf = builder.Build().Value.Resolved();

        pf.PostProcessors.ShouldNotBeNull();
        pf.PostProcessors!.Count.ShouldBe(1);
    }

    [Fact]
    public void Variable_Adds_Variable()
    {
        var builder = new PackerFileBuilder()
            .Variable("os_version", "1.0");

        var pf = builder.Build().Value.Resolved();

        pf.Variables.ShouldContainKeyAndValue("os_version", "1.0");
    }

    [Fact]
    public void ChangeVariable_Updates_Variable()
    {
        var builder = new PackerFileBuilder()
            .Variable("os_version", "1.0")
            .ChangeVariable("os_version", "2.0");

        var pf = builder.Build().Value.Resolved();

        pf.Variables.ShouldContainKeyAndValue("os_version", "2.0");
    }

    [Fact]
    public void GetBuilder_Returns_Matching_Builder()
    {
        var builder = new PackerFileBuilder()
            .Builder(b => b.Type("virtualbox-iso").VmName("test"));
        
        var found = builder.GetBuilder(b => true);
        
        found.ShouldNotBeNull();
    }

    [Fact]
    public void GetBuilder_Returns_Null_When_Not_Found()
    {
        var builder = new PackerFileBuilder();
        
        var found = builder.GetBuilder(b => false);
        
        found.ShouldBeNull();
    }

    [Fact]
    public void GetProvisioner_Returns_Matching_Provisioner()
    {
        var builder = new PackerFileBuilder()
            .Provisioner(p => p.Type("shell"));
        
        var found = builder.GetProvisioner(p => true);
        
        found.ShouldNotBeNull();
    }

    [Fact]
    public void GetPostProcessor_Returns_Matching_PostProcessor()
    {
        var builder = new PackerFileBuilder()
            .PostProcessor(pp => pp.Type("vagrant").CompressionLevel(6));
        
        var found = builder.GetPostProcessor(pp => true);
        
        found.ShouldNotBeNull();
    }

    [Fact]
    public void UpdateProvisioner_Updates_Existing_Provisioner()
    {
        var builder = new PackerFileBuilder()
            .Provisioner(p => p.Type("shell"))
            .UpdateProvisioner(p => true, p => p.AddScript("test.sh"));

        var pf = builder.Build().Value.Resolved();

        pf.Provisioners![0].Scripts.ShouldContain("test.sh");
    }

    [Fact]
    public void UpdatePostProcessor_Updates_Existing_PostProcessor()
    {
        var builder = new PackerFileBuilder()
            .PostProcessor(pp => pp.Type("vagrant").CompressionLevel(6))
            .UpdatePostProcessor(pp => true, pp => pp.Output("output.box"));

        var pf = builder.Build().Value.Resolved();

        pf.PostProcessors![0].Output.ShouldBe("output.box");
    }
}

#endregion

#region PackerBuilderBuilder Tests

public class PackerBuilderBuilderTests
{
    // Helper to create a fully configured builder with one VboxManage command
    private static PackerBuilderBuilder CreateBaseBuilder() =>
        new PackerBuilderBuilder()
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
            .VmName("test-vm")
            .HardDriveInterface("sata")
            .ModifyVm("--cpus", "1"); // Add at least one VboxManage command

    [Fact]
    public void Build_Creates_PackerBuilder_With_All_Properties()
    {
        var builder = CreateBaseBuilder()
            .Headless(true)
            .KeepRegistered(false)
            .HardDriveDiscard(true);
        
        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var pb = result.Value.Resolved();
        pb.Type.ShouldBe("virtualbox-iso");
        pb.VmName.ShouldBe("test-vm");
        pb.Headless.ShouldBe(true);
        pb.HardDriveDiscard.ShouldBe(true);
    }

    [Fact]
    public void ModifyVm_Adds_VBoxManage_Command()
    {
        var builder = CreateBaseBuilder()
            .ModifyVm("--memory", "1024");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage.ShouldNotBeNull();
        // Should have at least 2 commands (base + this one)
        pb.VboxManage!.Count.ShouldBeGreaterThanOrEqualTo(2);
        pb.VboxManage.Any(cmd => cmd.Contains("modifyvm") && cmd.Contains("--memory")).ShouldBeTrue();
    }

    [Fact]
    public void ModifyVmIf_Adds_Command_When_Condition_True()
    {
        var builder = CreateBaseBuilder()
            .ModifyVmIf(() => true, "--memory", "1024");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage.ShouldNotBeNull();
        pb.VboxManage!.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void ModifyVmIf_Does_Not_Add_When_Condition_False()
    {
        var builder = CreateBaseBuilder()
            .ModifyVmIf(() => false, "--memory", "1024");

        var pb = builder.Build().Value.Resolved();

        // Should only have the base command from CreateBaseBuilder
        pb.VboxManage.ShouldNotBeNull();
        pb.VboxManage!.Count.ShouldBe(1);
        pb.VboxManage.Any(cmd => cmd.Contains("--memory")).ShouldBeFalse();
    }

    [Fact]
    public void ModifyStorageController_Adds_Storagectl_Command()
    {
        var builder = CreateBaseBuilder()
            .ModifyStorageController("SATA Controller", "--add", "sata");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage.ShouldNotBeNull();
        pb.VboxManage!.Any(cmd => cmd.Contains("storagectl")).ShouldBeTrue();
    }

    [Fact]
    public void ModifyStorageControllerIf_Conditional()
    {
        var builder = CreateBaseBuilder()
            .ModifyStorageControllerIf(() => true, "SATA", "--add", "sata")
            .ModifyStorageControllerIf(() => false, "IDE", "--add", "ide");

        var pb = builder.Build().Value.Resolved();

        // Should have base command + one storagectl (not the IDE one)
        pb.VboxManage!.Count.ShouldBe(2);
    }

    [Fact]
    public void ModifyStorageAttach_Adds_Storageattach_Command()
    {
        var builder = CreateBaseBuilder()
            .ModifyStorageAttach("SATA Controller", 0, "--type", "hdd");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage.ShouldNotBeNull();
        pb.VboxManage!.Any(cmd => cmd.Contains("storageattach")).ShouldBeTrue();
    }

    [Fact]
    public void ModifyStorageAttachIf_Conditional()
    {
        var builder = CreateBaseBuilder()
            .ModifyStorageAttachIf(() => true, "SATA", 0, "--type", "hdd")
            .ModifyStorageAttachIf(() => false, "IDE", 0, "--type", "dvd");
        
        var pb = builder.Build().Value.Resolved();

        // base + one storageattach
        pb.VboxManage!.Count.ShouldBe(2);
    }

    [Fact]
    public void AddVBoxManage_Adds_Generic_Command()
    {
        var builder = CreateBaseBuilder()
            .AddVBoxManage("setproperty", "machinefolder", "/vms", "default");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage.ShouldNotBeNull();
        pb.VboxManage!.Any(cmd => cmd.Contains("setproperty")).ShouldBeTrue();
    }

    [Fact]
    public void AddVBoxManageIf_Conditional()
    {
        var builder = CreateBaseBuilder()
            .AddVBoxManageIf(() => true, "setproperty", "a", "b", "c")
            .AddVBoxManageIf(() => false, "setproperty", "x", "y", "z");

        var pb = builder.Build().Value.Resolved();

        // base + one setproperty
        pb.VboxManage!.Count.ShouldBe(2);
    }

    [Fact]
    public void ModifyProperty_Adds_Setproperty_Command()
    {
        var builder = CreateBaseBuilder()
            .ModifyProperty("machinefolder", "/vms");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage!.Any(cmd => cmd.Contains("setproperty") && cmd.Contains("machinefolder")).ShouldBeTrue();
    }

    [Fact]
    public void SetExtraData_Adds_Setextradata_Command()
    {
        var builder = CreateBaseBuilder()
            .SetExtraData("VBoxInternal/Key", "Value");

        var pb = builder.Build().Value.Resolved();

        pb.VboxManage!.Any(cmd => cmd.Contains("setextradata")).ShouldBeTrue();
    }
}

#endregion

#region Provisioner Tests

public class ProvisionerTests
{
    [Fact]
    public void Provisioner_Constructor_Sets_All_Properties()
    {
        var scripts = new List<string> { "script.sh" };
        var @override = new ProvisionerOverride { VirtualBoxIso = null };
        
        var provisioner = new Provisioner("shell", scripts, @override, "10s");
        
        provisioner.Type.ShouldBe("shell");
        provisioner.Scripts.ShouldBeSameAs(scripts);
        provisioner.Override.ShouldBeSameAs(@override);
        provisioner.PauseBefore.ShouldBe("10s");
    }
}

#endregion

#region ProvisionerBuilder Tests

public class ProvisionerBuilderTests
{
    [Fact]
    public void Build_Creates_Provisioner()
    {
        var builder = new ProvisionerBuilder()
            .Type("shell")
            .AddScript("setup.sh")
            .PauseBefore("5s");
        
        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var p = result.Value.Resolved();
        p.Type.ShouldBe("shell");
        p.Scripts!.ShouldContain("setup.sh");
        p.PauseBefore.ShouldBe("5s");
    }

    [Fact]
    public void BeforeScript_Inserts_Script_Before_Existing()
    {
        var builder = new ProvisionerBuilder()
            .Type("shell")
            .AddScript("second.sh")
            .BeforeScript("second.sh", "first.sh");

        var p = builder.Build().Value.Resolved();

        p.Scripts![0].ShouldBe("first.sh");
        p.Scripts[1].ShouldBe("second.sh");
    }

    [Fact]
    public void BeforeScript_Does_Nothing_When_Target_Not_Found()
    {
        var builder = new ProvisionerBuilder()
            .Type("shell")
            .AddScript("existing.sh")
            .BeforeScript("nonexistent.sh", "new.sh");

        var p = builder.Build().Value.Resolved();
        p.Scripts!.Count.ShouldBe(1);
    }

    [Fact]
    public void Override_Sets_Override()
    {
        var @override = new ProvisionerOverride { VirtualBoxIso = new VirtualBoxIsoProvisionerOverride("cmd") };
        var builder = new ProvisionerBuilder()
            .Type("shell")
            .Override(@override);
        
        var p = builder.Build().Value.Resolved();
        
        p.Override.ShouldBeSameAs(@override);
    }
}

#endregion

#region PostProcessor Tests

public class PostProcessorTests
{
    [Fact]
    public void PostProcessor_Has_Required_Properties()
    {
        var pp = new PostProcessor
        {
            Type = "vagrant",
            CompressionLevel = 6,
            KeepInputArtifact = true,
            Output = "output.box",
            VagrantfileTemplate = "template.tpl"
        };
        
        pp.Type.ShouldBe("vagrant");
        pp.CompressionLevel.ShouldBe(6);
        pp.KeepInputArtifact.ShouldBeTrue();
        pp.Output.ShouldBe("output.box");
        pp.VagrantfileTemplate.ShouldBe("template.tpl");
    }
}

#endregion

#region PostProcessorBuilder Tests

public class PostProcessorBuilderTests
{
    [Fact]
    public void Build_Creates_PostProcessor()
    {
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .CompressionLevel(6)
            .KeepInputArtefact(true)
            .Output("output.box")
            .VagrantfileTemplate("template.tpl");
        
        var result = builder.Build();
        
        result.IsSuccess.ShouldBeTrue();
        var pp = result.Value.Resolved();
        pp.Type.ShouldBe("vagrant");
        pp.CompressionLevel.ShouldBe(6);
        pp.KeepInputArtifact.ShouldBeTrue();
        pp.Output.ShouldBe("output.box");
    }

    [Fact]
    public void KeepInputArtefact_Defaults_To_False()
    {
        // Must provide all required fields for validation to pass
        var builder = new PostProcessorBuilder()
            .Type("vagrant")
            .CompressionLevel(6)
            .Output("output.box")
            .VagrantfileTemplate("template.tpl");

        var pp = builder.Build().Value.Resolved();

        pp.KeepInputArtifact.ShouldBeFalse();
    }
}

#endregion

#region ProvisionerOverride Tests

public class ProvisionerOverrideTests
{
    [Fact]
    public void ProvisionerOverride_Has_VirtualBoxIso_Property()
    {
        var vboxOverride = new VirtualBoxIsoProvisionerOverride("echo test");
        var @override = new ProvisionerOverride { VirtualBoxIso = vboxOverride };
        
        @override.VirtualBoxIso.ShouldBeSameAs(vboxOverride);
    }
}

#endregion

#region ProvisionerOverrideBuilder Tests

public class ProvisionerOverrideBuilderTests
{
    [Fact]
    public void Build_Creates_ProvisionerOverride()
    {
        var vboxOverride = new VirtualBoxIsoProvisionerOverride("cmd");
        var builder = new ProvisionerOverrideBuilder()
            .VirtualBoxIso(vboxOverride);
        
        var result = builder.Build();
        result.IsSuccess.ShouldBeTrue();
        var po = result.Value.Resolved();
        po.VirtualBoxIso.ShouldBeSameAs(vboxOverride);
    }

    [Fact]
    public void Build_With_Null_VirtualBoxIso()
    {
        var builder = new ProvisionerOverrideBuilder()
            .VirtualBoxIso(null);
        
        var po = builder.Build().Value.Resolved();
        
        po.VirtualBoxIso.ShouldBeNull();
    }
}

#endregion

#region VirtualBoxIsoProvisionerOverride Tests

public class VirtualBoxIsoProvisionerOverrideTests
{
    [Fact]
    public void Constructor_Sets_ExecuteCommand()
    {
        var @override = new VirtualBoxIsoProvisionerOverride("echo hello");
        
        @override.ExecuteCommand.ShouldBe("echo hello");
    }
}

#endregion

#region VirtualBoxIsoProvisionerOverrideBuilder Tests

public class VirtualBoxIsoProvisionerOverrideBuilderTests
{
    [Fact]
    public void Build_Creates_Override()
    {
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
            .ExecuteCommand("echo test");
        
        var result = builder.Build();
        result.IsSuccess.ShouldBeTrue();
        var vo = result.Value.Resolved();
        vo.ExecuteCommand.ShouldBe("echo test");
    }

    [Fact]
    public void Build_With_Null_Command()
    {
        var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
            .ExecuteCommand(null);
        
        var vo = builder.Build().Value.Resolved();
        
        vo.ExecuteCommand.ShouldBeNull();
    }
}

#endregion

#region PackerBundle Tests

public class PackerBundleTests
{
    [Fact]
    public void PackerBundle_Has_Required_Properties()
    {
        var packerFile = new PackerFile(null, null, null, null, null);
        var httpDir = new HttpDirectory();
        var vagrantDir = new VagrantDirectory();
        var dirs = new DirectoryList();
        var scripts = new ScriptDictionary();
        var plugins = new PluginList();
        
        var bundle = new PackerBundle
        {
            PackerFile = packerFile,
            HttpDirectory = httpDir,
            VagrantDirectory = vagrantDir,
            Directories = dirs,
            Scripts = scripts,
            Plugins = plugins
        };
        
        bundle.PackerFile.ShouldBeSameAs(packerFile);
        bundle.HttpDirectory.ShouldBeSameAs(httpDir);
        bundle.VagrantDirectory.ShouldBeSameAs(vagrantDir);
        bundle.Directories.ShouldBeSameAs(dirs);
        bundle.Scripts.ShouldBeSameAs(scripts);
        bundle.Plugins.ShouldBeSameAs(plugins);
    }
}

#endregion

#region PackerBundleBuilder Tests

public class PackerBundleBuilderTests
{
    [Fact]
    public void Build_Creates_PackerBundle()
    {
        var builder = new PackerBundleBuilder()
            .Directory("/tmp")
            .Plugin("packer-plugin-virtualbox")
            .Script("setup", sb => sb.Name("setup.sh").AddLine("echo setup"));
        
        var result = builder.Build();
        
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Directory_Adds_Directory()
    {
        var builder = new PackerBundleBuilder()
            .Directory("/tmp/build");
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.Directories.ShouldContain("/tmp/build");
    }

    [Fact]
    public void Plugin_Adds_Plugin()
    {
        var builder = new PackerBundleBuilder()
            .Plugin("packer-plugin-virtualbox");
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.Plugins.ShouldContain("packer-plugin-virtualbox");
    }

    [Fact]
    public void Script_Adds_Script()
    {
        var builder = new PackerBundleBuilder()
            .Script("setup", sb => sb.Name("setup.sh"));
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.Scripts.ShouldContainKey("setup");
    }

    [Fact]
    public void Script_Dictionary_Adds_Multiple_Scripts()
    {
        var builder = new PackerBundleBuilder()
            .Script(new Dictionary<string, Action<ShellScriptBuilder>>
            {
                { "script1", sb => sb.Name("s1.sh") },
                { "script2", sb => sb.Name("s2.sh") }
            });
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.Scripts.Count.ShouldBe(2);
    }

    [Fact]
    public void ScriptRemove_Removes_Script()
    {
        var builder = new PackerBundleBuilder()
            .Script("setup", sb => sb.Name("setup.sh"))
            .ScriptRemove("setup");
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.Scripts.ShouldNotContainKey("setup");
    }

    [Fact]
    public void VagrantDirectory_Configures_VagrantDirectory()
    {
        // File must have valid extension, path, and newline
        var file = new File(new List<string>(), "\n", "Vagrantfile", ".rb", "/vagrant");
        var builder = new PackerBundleBuilder()
            .VagrantDirectory(vd => vd.AddFile("Vagrantfile", file));
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.VagrantDirectory.Files.ShouldContainKey("Vagrantfile");
    }

    [Fact]
    public void HttpDirectory_Configures_HttpDirectory()
    {
        var file = new File(new List<string>(), "\n", "answers", ".txt", "/http");
        var builder = new PackerBundleBuilder()
            .HttpDirectory(hd => hd.AddFile("answers.txt", file));
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.HttpDirectory.Files.ShouldContainKey("answers.txt");
    }

    [Fact]
    public void PackerFile_Configures_PackerFile()
    {
        var builder = new PackerBundleBuilder()
            .PackerFile(pf => pf.Description("Test bundle"));
        
        var bundle = builder.Build().Value.Resolved();
        
        bundle.PackerFile.Description.ShouldBe("Test bundle");
    }
}

#endregion

#region PackerBundleExtensions Tests

public class PackerBundleExtensionsTests
{
    [Fact]
    public void Provisioning_Adds_Provisioner_To_PackerFile()
    {
        var bundle = new PackerBundle
        {
            PackerFile = new PackerFile(null, null, null, null, null),
            HttpDirectory = new HttpDirectory(),
            VagrantDirectory = new VagrantDirectory(),
            Directories = new DirectoryList(),
            Scripts = new ScriptDictionary { { "setup", new ShellScript { Name = "setup.sh" } } },
            Plugins = new PluginList()
        };
        
        bundle.Provisioning((pb, scripts) =>
        {
            pb.Type("shell");
            pb.AddScript(((ShellScript)scripts["setup"]).Name);
        });
        
        bundle.PackerFile.Provisioners.ShouldNotBeNull();
        bundle.PackerFile.Provisioners!.Count.ShouldBe(1);
        bundle.PackerFile.Provisioners[0].Scripts.ShouldContain("setup.sh");
    }

    [Fact]
    public void Provisioning_Returns_Same_Bundle()
    {
        var bundle = new PackerBundle
        {
            PackerFile = new PackerFile(null, null, null, null, null),
            HttpDirectory = new HttpDirectory(),
            VagrantDirectory = new VagrantDirectory(),
            Directories = new DirectoryList(),
            Scripts = new ScriptDictionary(),
            Plugins = new PluginList()
        };
        
        var result = bundle.Provisioning((pb, _) => pb.Type("shell"));
        
        result.ShouldBeSameAs(bundle);
    }
}

#endregion

#region PackerFileExtensions Tests

public class PackerFileExtensionsTests
{
    [Fact]
    public void Serialize_Returns_Json_String()
    {
        var packerFile = new PackerFile(null, "Test", null, null, null);
        
        var json = packerFile.Serialize();
        
        json.ShouldContain("\"description\"");
        json.ShouldContain("Test");
    }

    [Fact]
    public void Serialize_Uses_Indented_Format()
    {
        var packerFile = new PackerFile(null, "Test", null, null, null);
        
        var json = packerFile.Serialize();
        
        json.ShouldContain("\n");
    }
}

#endregion

#region Collection Types Tests

public class CollectionTypesTests
{
    [Fact]
    public void DirectoryList_Default_Constructor()
    {
        var list = new DirectoryList();
        
        list.ShouldBeEmpty();
    }

    [Fact]
    public void DirectoryList_Collection_Constructor()
    {
        var list = new DirectoryList(new[] { "/dir1", "/dir2" });
        
        list.Count.ShouldBe(2);
    }

    [Fact]
    public void ScriptDictionary_Default_Constructor()
    {
        var dict = new ScriptDictionary();
        
        dict.ShouldBeEmpty();
    }

    [Fact]
    public void ScriptDictionary_Dictionary_Constructor()
    {
        var source = new Dictionary<string, IScript>
        {
            { "test", new ShellScript { Name = "test.sh" } }
        };
        var dict = new ScriptDictionary(source);
        
        dict.Count.ShouldBe(1);
    }

    [Fact]
    public void PluginList_Default_Constructor()
    {
        var list = new PluginList();
        
        list.ShouldBeEmpty();
    }

    [Fact]
    public void PluginList_Collection_Constructor()
    {
        var list = new PluginList(new[] { "plugin1", "plugin2" });
        
        list.Count.ShouldBe(2);
    }
}

#endregion

#region WorkingDirectory Tests

public class WorkingDirectoryTests
{
    [Fact]
    public void WorkingDirectory_Record_Has_Directory_Property()
    {
        var wd = new WorkingDirectory("/tmp/work");
        
        wd.Directory.ShouldBe("/tmp/work");
    }
}

#endregion

#region PackerBundleWritingContext Tests

public class PackerBundleWritingContextTests
{
    [Fact]
    public void PackerBundleWritingContext_Record_Has_Path_Property()
    {
        var ctx = new PackerBundleWritingContext("/tmp/output");
        
        ctx.Path.ShouldBe("/tmp/output");
    }
}

#endregion

#region PackerBundleWriter Tests

public class PackerBundleWriterTests
{
    [Fact]
    public async Task WriteAsync_Creates_Files_And_Directories()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "packer_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        try
        {
            var httpFile = new File(new List<string> { "line1", "line2" }, "\n", "answers", ".txt", "/");
            var vagrantFile = new File(new List<string> { "Vagrant.configure('2')" }, "\n", "Vagrantfile", "", "/");
            var script = new ShellScript { Name = "setup.sh" };
            script.Lines.Add("echo hello");
            
            var bundle = new PackerBundle
            {
                PackerFile = new PackerFile(null, "Test", null, null, null),
                HttpDirectory = new HttpDirectory { Files = { { "answers.txt", httpFile } } },
                VagrantDirectory = new VagrantDirectory { Files = { { "Vagrantfile", vagrantFile } } },
                Directories = new DirectoryList { "scripts" },
                Scripts = new ScriptDictionary { { "scripts/setup.sh", script } },
                Plugins = new PluginList()
            };
            
            var context = new PackerBundleWritingContext(tempDir);
            var writer = new PackerBundleWriter();
            
            // Act
            await writer.WriteAsync(bundle, context);
            
            // Assert
            System.IO.File.Exists(Path.Combine(tempDir, "alpine.json")).ShouldBeTrue();
            Directory.Exists(Path.Combine(tempDir, "http")).ShouldBeTrue();
            System.IO.File.Exists(Path.Combine(tempDir, "http", "answers.txt")).ShouldBeTrue();
            Directory.Exists(Path.Combine(tempDir, "vagrant")).ShouldBeTrue();
            System.IO.File.Exists(Path.Combine(tempDir, "vagrant", "Vagrantfile")).ShouldBeTrue();
            Directory.Exists(Path.Combine(tempDir, "scripts")).ShouldBeTrue();
            System.IO.File.Exists(Path.Combine(tempDir, "scripts", "setup.sh")).ShouldBeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public async Task WriteAsync_Handles_Empty_Bundle()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "packer_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        try
        {
            var bundle = new PackerBundle
            {
                PackerFile = new PackerFile(null, null, null, null, null),
                HttpDirectory = new HttpDirectory(),
                VagrantDirectory = new VagrantDirectory(),
                Directories = new DirectoryList(),
                Scripts = new ScriptDictionary(),
                Plugins = new PluginList()
            };
            
            var context = new PackerBundleWritingContext(tempDir);
            var writer = new PackerBundleWriter();
            
            await writer.WriteAsync(bundle, context);
            
            System.IO.File.Exists(Path.Combine(tempDir, "alpine.json")).ShouldBeTrue();
            Directory.Exists(Path.Combine(tempDir, "http")).ShouldBeTrue();
            Directory.Exists(Path.Combine(tempDir, "vagrant")).ShouldBeTrue();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public async Task WriteAsync_Handles_NonFile_Types_In_Directories()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "packer_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        try
        {
            // Create a mock IFile that is not a File instance
            var mockFile = new MockFile();
            
            var bundle = new PackerBundle
            {
                PackerFile = new PackerFile(null, null, null, null, null),
                HttpDirectory = new HttpDirectory { Files = { { "mock.txt", mockFile } } },
                VagrantDirectory = new VagrantDirectory { Files = { { "mock2.txt", mockFile } } },
                Directories = new DirectoryList(),
                Scripts = new ScriptDictionary { { "script.sh", new MockScript() } },
                Plugins = new PluginList()
            };
            
            var context = new PackerBundleWritingContext(tempDir);
            var writer = new PackerBundleWriter();
            
            // Act - should not throw
            await writer.WriteAsync(bundle, context);
            
            // Assert - mock files should be skipped
            System.IO.File.Exists(Path.Combine(tempDir, "http", "mock.txt")).ShouldBeFalse();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}

// Mock classes for testing
public class MockFile : IFile
{
    public string Name => "mock";
    public string Extension => ".txt";
    public string Path => "/";
    public string NewLine => "\n";
}

public class MockScript : IScript
{
    public string NewLine => "\n";
}

#endregion

#region VagrantDirectoryBuilder Validation Tests

public class VagrantDirectoryBuilderValidationTests
{
    [Fact]
    public void ValidateInternal_Validates_Empty_Extension()
    {
        // Now that ValidateInternal uses 'override', validation is properly called
        var file = new File(new List<string>(), "\n", "test", "", "/path");
        var builder = new VagrantDirectoryBuilder()
            .AddFile("test.txt", file);
        
        // Build should fail because ValidateInternal validates empty extension
        var result = builder.Build();
        result.IsFailure().ShouldBeTrue();
    }
}

#endregion

#region PostProcessor Name Property Tests

public class PostProcessorNameTests
{
    [Fact]
    public void PostProcessor_Name_Property_Can_Be_Set()
    {
        var pp = new PostProcessor
        {
            Name = "test-processor",
            Type = "vagrant",
            CompressionLevel = 6,
            KeepInputArtifact = false,
            Output = "output.box",
            VagrantfileTemplate = "template.tpl"
        };
        
        pp.Name.ShouldBe("test-processor");
    }
}

#endregion
