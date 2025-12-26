using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.Packer.Tests;

#region PackerBuildCommand Tests

public class PackerBuildCommandTests
{
    [Fact]
    public void ToArguments_Returns_Build_And_TemplatePath()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.pkr.hcl",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var args = cmd.ToArguments();

        args.ShouldContain("build");
        args.ShouldContain("template.pkr.hcl");
    }

    [Fact]
    public void ToArguments_With_All_Flags_Includes_All_Options()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = true,
            TimestampUi = true,
            Force = true,
            Debug = true,
            MachineReadable = true,
            ParallelBuilds = 4,
            OnError = OnErrorStrategy.Abort
        };

        var args = cmd.ToArguments();

        args.ShouldContain("-color=false");
        args.ShouldContain("-timestamp-ui");
        args.ShouldContain("-force");
        args.ShouldContain("-debug");
        args.ShouldContain("-machine-readable");
        args.ShouldContain("-parallel-builds");
        args.ShouldContain("4");
        args.ShouldContain("-on-error=abort");
    }

    [Fact]
    public void ToArguments_OnError_Ask_Adds_Flag()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Ask
        };

        var args = cmd.ToArguments();

        args.ShouldContain("-on-error=ask");
    }

    [Fact]
    public void ToArguments_OnError_Cleanup_No_Flag()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var args = cmd.ToArguments();

        args.ShouldNotContain("-on-error=cleanup");
        args.ShouldNotContain("-on-error=abort");
        args.ShouldNotContain("-on-error=ask");
    }

    [Fact]
    public void Var_Adds_Variable_To_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Var("key1", "value1").Var("key2", "value2");
        var args = cmd.ToArguments();

        args.ShouldContain("-var");
        args.ShouldContain("key1=value1");
        args.ShouldContain("key2=value2");
    }

    [Fact]
    public void VarFile_Adds_VarFile_To_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.VarFile("vars.pkrvars.hcl");
        var args = cmd.ToArguments();

        args.ShouldContain("-var-file=vars.pkrvars.hcl");
    }

    [Fact]
    public void Only_Adds_Only_Builders_To_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Only("virtualbox-iso").Only("qemu");
        var args = cmd.ToArguments();

        args.ShouldContain("-only=virtualbox-iso,qemu");
    }

    [Fact]
    public void Only_Does_Not_Add_Duplicates()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Only("virtualbox-iso").Only("virtualbox-iso");
        var args = cmd.ToArguments();

        args.ShouldContain("-only=virtualbox-iso");
    }

    [Fact]
    public void Except_Adds_Except_Builders_To_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Except("qemu").Except("docker");
        var args = cmd.ToArguments();

        args.ShouldContain("-except=qemu,docker");
    }

    [Fact]
    public void Except_Does_Not_Add_Duplicates()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Except("qemu").Except("qemu");
        var args = cmd.ToArguments();

        args.ShouldContain("-except=qemu");
    }

    [Fact]
    public void Env_Sets_Environment_Variable()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Env("PACKER_LOG", "1");
        
        var packerCmd = (IPackerCommand)cmd;
        packerCmd.EnvironmentVariables.ShouldContainKey("PACKER_LOG");
        packerCmd.EnvironmentVariables["PACKER_LOG"].ShouldBe("1");
    }

    [Fact]
    public void ParallelBuilds_Zero_Does_Not_Add_Flag()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            ParallelBuilds = 0,
            OnError = OnErrorStrategy.Cleanup
        };

        var args = cmd.ToArguments();

        args.ShouldNotContain("-parallel-builds");
    }
}

#endregion

#region PackerBuildCommandBuilder Tests

public class PackerBuildCommandBuilderTests
{
    [Fact]
    public void Build_Creates_Command_With_All_Properties()
    {
        var builder = new PackerBuildCommandBuilder()
            .WithDebug(true)
            .WithDisableColor(true)
            .WithForce(true)
            .WithMachineReadable(true)
            .WithOnError(OnErrorStrategy.Abort)
            .WithParallelBuilds(2)
            .WithTemplatePath("template.pkr.hcl")
            .WithTimestampUi(true)
            .WithWorkingDirectory("/tmp");

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var cmd = result.Value.Resolved();
        cmd.Debug.ShouldBeTrue();
        cmd.DisableColor.ShouldBeTrue();
        cmd.Force.ShouldBeTrue();
        cmd.MachineReadable.ShouldBeTrue();
        cmd.OnError.ShouldBe(OnErrorStrategy.Abort);
        cmd.ParallelBuilds.ShouldBe(2);
        cmd.TemplatePath.ShouldBe("template.pkr.hcl");
        cmd.TimestampUi.ShouldBeTrue();
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }
}

#endregion

#region PackerInstallPluginCommand Tests

public class PackerInstallPluginCommandTests
{
    [Fact]
    public void ToArguments_Returns_Basic_Arguments()
    {
        var cmd = new PackerInstallPluginCommand
        {
            PluginIdentifier = "hashicorp/amazon",
            Version = "1.0.0",
            Force = false
        };

        var args = cmd.ToArguments();

        args.ShouldContain("plugins");
        args.ShouldContain("install");
        args.ShouldContain("hashicorp/amazon");
        args.ShouldContain("1.0.0");
    }

    [Fact]
    public void ToArguments_With_Force_Adds_Flag()
    {
        var cmd = new PackerInstallPluginCommand
        {
            PluginIdentifier = "hashicorp/amazon",
            Version = "1.0.0",
            Force = true
        };

        var args = cmd.ToArguments();

        args.ShouldContain("-force");
    }

    [Fact]
    public void ToArguments_With_DestinationDir_Adds_Path()
    {
        var cmd = new PackerInstallPluginCommand
        {
            PluginIdentifier = "hashicorp/amazon",
            Version = "1.0.0",
            Force = false,
            DestinationDir = "/plugins"
        };

        var args = cmd.ToArguments();

        args.ShouldContain("-path");
        args.ShouldContain("/plugins");
    }

    [Fact]
    public void ToArguments_With_SourceUri_Adds_Source()
    {
        var cmd = new PackerInstallPluginCommand
        {
            PluginIdentifier = "hashicorp/amazon",
            Version = "1.0.0",
            Force = false,
            SourceUri = "https://example.com/plugin"
        };

        var args = cmd.ToArguments();

        args.ShouldContain("-source");
        args.ShouldContain("https://example.com/plugin");
    }

    [Fact]
    public void Env_Sets_Environment_Variable()
    {
        var cmd = new PackerInstallPluginCommand
        {
            PluginIdentifier = "hashicorp/amazon",
            Version = "1.0.0",
            Force = false
        };

        cmd.Env("TEST_VAR", "test_value");
        
        var packerCmd = (IPackerCommand)cmd;
        packerCmd.EnvironmentVariables.ShouldContainKey("TEST_VAR");
        packerCmd.EnvironmentVariables["TEST_VAR"].ShouldBe("test_value");
    }
}

#endregion

#region PackerInstallPluginCommandBuilder Tests

public class PackerInstallPluginCommandBuilderTests
{
    [Fact]
    public void Build_Creates_Command_With_All_Properties()
    {
        var builder = new PackerInstallPluginCommandBuilder()
            .WithDestinationDir("/plugins")
            .WithPluginIdentifier("hashicorp/amazon")
            .WithForce(true)
            .WithSourceUri("https://example.com")
            .WithVersion("1.0.0")
            .WithWorkingDirectory("/tmp");

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var cmd = result.Value.Resolved();
        cmd.DestinationDir.ShouldBe("/plugins");
        cmd.PluginIdentifier.ShouldBe("hashicorp/amazon");
        cmd.Force.ShouldBeTrue();
        cmd.SourceUri.ShouldBe("https://example.com");
        cmd.Version.ShouldBe("1.0.0");
        cmd.WorkingDirectory.ShouldBe("/tmp");
    }
}

#endregion

#region IPackerCommand Interface Tests

public class IPackerCommandTests
{
    [Fact]
    public void Executable_Returns_Packer()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var packerCmd = (IPackerCommand)cmd;
        packerCmd.Executable.ShouldBe("packer");
    }

    [Fact]
    public void ToProcessStartInfo_Creates_Valid_ProcessStartInfo()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup,
            WorkingDirectory = "/tmp"
        };

        cmd.Env("TEST_ENV", "value");
        
        var packerCmd = (IPackerCommand)cmd;
        var psi = packerCmd.ToProcessStartInfo();

        psi.FileName.ShouldBe("packer");
        psi.WorkingDirectory.ShouldBe("/tmp");
        psi.Environment.ShouldContainKey("TEST_ENV");
        psi.ArgumentList.ShouldContain("build");
        psi.ArgumentList.ShouldContain("template.json");
    }

    [Fact]
    public void ToProcessStartInfo_Uses_Provided_WorkingDirectory()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var packerCmd = (IPackerCommand)cmd;
        var psi = packerCmd.ToProcessStartInfo("/custom/path");

        psi.WorkingDirectory.ShouldBe("/custom/path");
    }

    [Fact]
    public void ToProcessStartInfo_Uses_CurrentDirectory_When_No_WorkingDirectory()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var packerCmd = (IPackerCommand)cmd;
        var psi = packerCmd.ToProcessStartInfo();

        psi.WorkingDirectory.ShouldBe(Environment.CurrentDirectory);
    }
}

#endregion

#region PackerCommandExtensions Tests

public class PackerCommandExtensionsTests
{
    [Fact]
    public void ToCommandLine_Returns_Executable_And_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldStartWith("packer");
        commandLine.ShouldContain("build");
        commandLine.ShouldContain("template.json");
    }

    [Fact]
    public void ToCommandLine_Quotes_Arguments_With_Spaces()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "path with spaces/template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldContain("\"path with spaces/template.json\"");
    }

    [Fact]
    public void ToCommandLine_Escapes_Quotes_In_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Var("key", "value with \"quotes\"");
        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldContain("\\\"quotes\\\"");
    }

    [Fact]
    public void ToCommandLine_Quotes_Empty_Arguments()
    {
        var cmd = new PackerBuildCommand
        {
            TemplatePath = "template.json",
            DisableColor = false,
            TimestampUi = false,
            Force = false,
            Debug = false,
            MachineReadable = false,
            OnError = OnErrorStrategy.Cleanup
        };

        cmd.Var("key", "");
        var commandLine = cmd.ToCommandLine();

        // Empty value results in key= (the value is empty but included)
        commandLine.ShouldContain("key=");
    }
}

#endregion

#region OnErrorStrategy Enum Tests

public class OnErrorStrategyTests
{
    [Fact]
    public void OnErrorStrategy_Has_Expected_Values()
    {
        OnErrorStrategy.Cleanup.ShouldBe((OnErrorStrategy)0);
        OnErrorStrategy.Abort.ShouldBe((OnErrorStrategy)1);
        OnErrorStrategy.Ask.ShouldBe((OnErrorStrategy)2);
    }
}

#endregion
