# FrenchExDev.Net.Vagrant

A strongly-typed .NET library for building and executing [Vagrant](https://www.vagrantup.com/) CLI commands. This library provides a fluent builder API with validation, making it easy to construct type-safe Vagrant commands in your .NET applications.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Features

- 🔷 **Strongly-typed commands** - Full IntelliSense support for all Vagrant CLI options
- 🏗️ **Fluent builder pattern** - Chain methods to construct commands naturally
- ✅ **Built-in validation** - Catches missing required parameters at build time
- 🧪 **Test-friendly architecture** - Dedicated testing helpers for unit tests
- 🔧 **Process execution utilities** - Easy integration with `System.Diagnostics.Process`
- 🌍 **Environment variable support** - Configure Vagrant environment per command

## Installation

```bash
# Main library
dotnet add package FrenchExDev.Net.Vagrant

# Testing helpers (for unit tests)
dotnet add package FrenchExDev.Net.Vagrant.Testing
```

## Quick Start

### Basic Command Execution

```csharp
using FrenchExDev.Net.Vagrant;

// Create a command using object initializer
var upCommand = new VagrantUpCommand
{
    Provider = "virtualbox",
    NoProvision = true,
    WorkingDirectory = "/path/to/vagrantfile"
};

// Execute the command
var process = upCommand.ToProcess(
    onStdOut: line => Console.WriteLine(line),
    onStdErr: line => Console.Error.WriteLine(line)
);

bool success = await process.WaitAsync();
```

### Using Builders (Recommended)

```csharp
using FrenchExDev.Net.Vagrant;

// Build commands with fluent API and validation
var builder = new VagrantUpCommandBuilder()
    .VmName("web")
    .Provider("virtualbox")
    .NoProvision()
    .WorkingDirectory("/path/to/project");

var result = builder.Build();
if (result.IsSuccess)
{
    var command = result.Value;
    var args = command.ToArguments();
    // ["up", "--provider", "virtualbox", "--no-provision", "web"]
}
```

### Command Line Generation

```csharp
var command = new VagrantSshCommand
{
    VmName = "web",
    Command = "ls -la /var/log"
};

// Generate shell-safe command string
string cmdLine = command.ToCommandLine();
// "vagrant ssh --command \"ls -la /var/log\" web"

// Get arguments as a list
IReadOnlyList<string> args = command.ToArguments();
// ["ssh", "--command", "ls -la /var/log", "web"]

// Create ProcessStartInfo directly
ProcessStartInfo psi = command.ToProcessStartInfo("/working/dir");
```

## Supported Commands

### Box Commands

| Command | Class | Builder |
|---------|-------|---------|
| `vagrant box add` | `VagrantBoxAddCommand` | `VagrantBoxAddCommandBuilder` |
| `vagrant box list` | `VagrantBoxListCommand` | `VagrantBoxListCommandBuilder` |
| `vagrant box outdated` | `VagrantBoxOutdatedCommand` | `VagrantBoxOutdatedCommandBuilder` |
| `vagrant box prune` | `VagrantBoxPruneCommand` | `VagrantBoxPruneCommandBuilder` |
| `vagrant box remove` | `VagrantBoxRemoveCommand` | `VagrantBoxRemoveCommandBuilder` |
| `vagrant box repackage` | `VagrantBoxRepackageCommand` | `VagrantBoxRepackageCommandBuilder` |
| `vagrant box update` | `VagrantBoxUpdateCommand` | `VagrantBoxUpdateCommandBuilder` |

### Cloud Commands

| Command | Class | Builder |
|---------|-------|---------|
| `vagrant cloud auth` | `VagrantCloudAuthCommand` | `VagrantCloudAuthCommandBuilder` |
| `vagrant cloud box` | `VagrantCloudBoxCommand` | `VagrantCloudBoxCommandBuilder` |
| `vagrant cloud provider` | `VagrantCloudProviderCommand` | `VagrantCloudProviderCommandBuilder` |
| `vagrant cloud search` | `VagrantCloudSearchCommand` | `VagrantCloudSearchCommandBuilder` |
| `vagrant cloud version` | `VagrantCloudVersionCommand` | `VagrantCloudVersionCommandBuilder` |

### Plugin Commands

| Command | Class | Builder |
|---------|-------|---------|
| `vagrant plugin install` | `VagrantPluginInstallCommand` | `VagrantPluginInstallCommandBuilder` |
| `vagrant plugin license` | `VagrantPluginLicenseCommand` | `VagrantPluginLicenseCommandBuilder` |
| `vagrant plugin list` | `VagrantPluginListCommand` | `VagrantPluginListCommandBuilder` |
| `vagrant plugin repair` | `VagrantPluginRepairCommand` | `VagrantPluginRepairCommandBuilder` |
| `vagrant plugin uninstall` | `VagrantPluginUninstallCommand` | `VagrantPluginUninstallCommandBuilder` |
| `vagrant plugin update` | `VagrantPluginUpdateCommand` | `VagrantPluginUpdateCommandBuilder` |

### Snapshot Commands

| Command | Class | Builder |
|---------|-------|---------|
| `vagrant snapshot delete` | `VagrantSnapshotDeleteCommand` | `VagrantSnapshotDeleteCommandBuilder` |
| `vagrant snapshot list` | `VagrantSnapshotListCommand` | `VagrantSnapshotListCommandBuilder` |
| `vagrant snapshot pop` | `VagrantSnapshotPopCommand` | `VagrantSnapshotPopCommandBuilder` |
| `vagrant snapshot push` | `VagrantSnapshotPushCommand` | `VagrantSnapshotPushCommandBuilder` |
| `vagrant snapshot restore` | `VagrantSnapshotRestoreCommand` | `VagrantSnapshotRestoreCommandBuilder` |
| `vagrant snapshot save` | `VagrantSnapshotSaveCommand` | `VagrantSnapshotSaveCommandBuilder` |

### Top-Level Commands

| Command | Class | Builder |
|---------|-------|---------|
| `vagrant destroy` | `VagrantDestroyCommand` | `VagrantDestroyCommandBuilder` |
| `vagrant global-status` | `VagrantGlobalStatusCommand` | `VagrantGlobalStatusCommandBuilder` |
| `vagrant halt` | `VagrantHaltCommand` | `VagrantHaltCommandBuilder` |
| `vagrant init` | `VagrantInitCommand` | `VagrantInitCommandBuilder` |
| `vagrant package` | `VagrantPackageCommand` | `VagrantPackageCommandBuilder` |
| `vagrant port` | `VagrantPortCommand` | `VagrantPortCommandBuilder` |
| `vagrant powershell` | `VagrantPowerShellCommand` | `VagrantPowerShellCommandBuilder` |
| `vagrant provision` | `VagrantProvisionCommand` | `VagrantProvisionCommandBuilder` |
| `vagrant rdp` | `VagrantRdpCommand` | `VagrantRdpCommandBuilder` |
| `vagrant reload` | `VagrantReloadCommand` | `VagrantReloadCommandBuilder` |
| `vagrant resume` | `VagrantResumeCommand` | `VagrantResumeCommandBuilder` |
| `vagrant ssh` | `VagrantSshCommand` | `VagrantSshCommandBuilder` |
| `vagrant ssh-config` | `VagrantSshConfigCommand` | `VagrantSshConfigCommandBuilder` |
| `vagrant status` | `VagrantStatusCommand` | `VagrantStatusCommandBuilder` |
| `vagrant suspend` | `VagrantSuspendCommand` | `VagrantSuspendCommandBuilder` |
| `vagrant up` | `VagrantUpCommand` | `VagrantUpCommandBuilder` |
| `vagrant validate` | `VagrantValidateCommand` | `VagrantValidateCommandBuilder` |
| `vagrant version` | `VagrantVersionCommand` | `VagrantVersionCommandBuilder` |
| `vagrant winrm` | `VagrantWinrmCommand` | `VagrantWinrmCommandBuilder` |
| `vagrant winrm-config` | `VagrantWinrmConfigCommand` | `VagrantWinrmConfigCommandBuilder` |

## Command Examples

### Box Management

```csharp
// Add a box
var addCmd = new VagrantBoxAddCommand
{
    Box = "ubuntu/focal64",
    Provider = "virtualbox",
    Force = true
};

// List boxes with info
var listCmd = new VagrantBoxListCommand
{
    BoxInfo = true,
    MachineReadable = true
};

// Remove a box
var removeCmd = new VagrantBoxRemoveCommand
{
    Name = "ubuntu/focal64",
    Provider = "virtualbox",
    Force = true
};
```

### VM Lifecycle

```csharp
// Initialize a new Vagrantfile
var initCmd = new VagrantInitCommand
{
    BoxName = "hashicorp/bionic64",
    Minimal = true,
    WorkingDirectory = "/my/project"
};

// Start VM with specific provider
var upCmd = new VagrantUpCommand
{
    VmName = "web",
    Provider = "virtualbox",
    Provision = true,
    ProvisionWith = ["shell", "puppet"]
};

// Gracefully stop VM
var haltCmd = new VagrantHaltCommand
{
    VmName = "web",
    Force = false
};

// Destroy VM
var destroyCmd = new VagrantDestroyCommand
{
    VmName = "web",
    Force = true,
    Graceful = true
};
```

### SSH & Remote Execution

```csharp
// SSH into VM
var sshCmd = new VagrantSshCommand
{
    VmName = "web",
    Command = "cat /etc/hosts",
    Tty = true
};

// PowerShell (Windows guests)
var psCmd = new VagrantPowerShellCommand
{
    VmName = "win-server",
    Command = "Get-Process",
    Elevated = true
};

// WinRM (Windows guests)
var winrmCmd = new VagrantWinrmCommand
{
    VmName = "win-server",
    Command = "hostname",
    Shell = "powershell",
    Elevated = true
};
```

### Snapshots

```csharp
// Save a snapshot
var saveCmd = new VagrantSnapshotSaveCommand
{
    Name = "before-update",
    VmName = "web",
    Force = true
};

// Restore a snapshot
var restoreCmd = new VagrantSnapshotRestoreCommand
{
    Name = "before-update",
    VmName = "web",
    NoProvision = true
};

// Push/Pop workflow
var pushCmd = new VagrantSnapshotPushCommand { VmName = "web" };
var popCmd = new VagrantSnapshotPopCommand
{
    VmName = "web",
    NoDelete = true,
    Provision = true
};
```

### Cloud Operations

```csharp
// Search for boxes
var searchCmd = new VagrantCloudSearchCommand
{
    Query = "ubuntu",
    Provider = "virtualbox",
    Json = true,
    Limit = 10,
    SortBy = "downloads",
    Order = "desc"
};
```

### Plugin Management

```csharp
// Install a plugin
var installCmd = new VagrantPluginInstallCommand
{
    Name = "vagrant-vbguest",
    PluginVersion = "0.31.0",
    Verbose = true
};

// List plugins
var listCmd = new VagrantPluginListCommand
{
    Local = true
};
```

## Environment Variables

All commands support setting environment variables that will be passed to the Vagrant process:

```csharp
var cmd = new VagrantUpCommand
{
    VmName = "web",
    Provider = "virtualbox"
};

// Set environment variables
cmd.Env("VAGRANT_LOG", "debug");
cmd.Env("VAGRANT_HOME", "/custom/vagrant/home");

// Environment variables are included in ProcessStartInfo
var psi = cmd.ToProcessStartInfo();
// psi.Environment contains the configured variables
```

## Testing Infrastructure

The `FrenchExDev.Net.Vagrant.Testing` package provides test helpers for validating command builders in your unit tests.

### Test Helpers

Each command has a corresponding test helper class:

```csharp
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

public class MyVagrantTests
{
    [Fact]
    public void Valid_BoxAdd_BuildsSuccessfully()
    {
        // Use Valid() to build and assert success
        var cmd = VagrantBoxAddCommandTest.Valid(b => b
            .Box("ubuntu/focal64")
            .Provider("virtualbox")
            .Force());

        cmd.Box.ShouldBe("ubuntu/focal64");
        cmd.Provider.ShouldBe("virtualbox");
        cmd.Force.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_BoxAdd_WithoutBox_Fails()
    {
        // Use Invalid() to assert build failure
        VagrantBoxAddCommandTest.Invalid(b => b
            .Provider("virtualbox"));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArgs()
    {
        var cmd = VagrantUpCommandTest.Valid(b => b
            .VmName("web")
            .Provider("virtualbox")
            .NoProvision());

        var args = cmd.ToArguments();
        
        args.ShouldContain("up");
        args.ShouldContain("--provider");
        args.ShouldContain("virtualbox");
        args.ShouldContain("--no-provision");
        args.ShouldContain("web");
    }
}
```

### Available Test Helpers

| Test Helper | For Builder |
|-------------|-------------|
| `VagrantBoxAddCommandTest` | `VagrantBoxAddCommandBuilder` |
| `VagrantBoxListCommandTest` | `VagrantBoxListCommandBuilder` |
| `VagrantBoxOutdatedCommandTest` | `VagrantBoxOutdatedCommandBuilder` |
| `VagrantBoxPruneCommandTest` | `VagrantBoxPruneCommandBuilder` |
| `VagrantBoxRemoveCommandTest` | `VagrantBoxRemoveCommandBuilder` |
| `VagrantBoxRepackageCommandTest` | `VagrantBoxRepackageCommandBuilder` |
| `VagrantBoxUpdateCommandTest` | `VagrantBoxUpdateCommandBuilder` |
| `VagrantCloudAuthCommandTest` | `VagrantCloudAuthCommandBuilder` |
| `VagrantCloudBoxCommandTest` | `VagrantCloudBoxCommandBuilder` |
| `VagrantCloudProviderCommandTest` | `VagrantCloudProviderCommandBuilder` |
| `VagrantCloudSearchCommandTest` | `VagrantCloudSearchCommandBuilder` |
| `VagrantCloudVersionCommandTest` | `VagrantCloudVersionCommandBuilder` |
| `VagrantPluginInstallCommandTest` | `VagrantPluginInstallCommandBuilder` |
| `VagrantPluginLicenseCommandTest` | `VagrantPluginLicenseCommandBuilder` |
| `VagrantPluginListCommandTest` | `VagrantPluginListCommandBuilder` |
| `VagrantPluginRepairCommandTest` | `VagrantPluginRepairCommandBuilder` |
| `VagrantPluginUninstallCommandTest` | `VagrantPluginUninstallCommandBuilder` |
| `VagrantPluginUpdateCommandTest` | `VagrantPluginUpdateCommandBuilder` |
| `VagrantSnapshotDeleteCommandTest` | `VagrantSnapshotDeleteCommandBuilder` |
| `VagrantSnapshotListCommandTest` | `VagrantSnapshotListCommandBuilder` |
| `VagrantSnapshotPopCommandTest` | `VagrantSnapshotPopCommandBuilder` |
| `VagrantSnapshotPushCommandTest` | `VagrantSnapshotPushCommandBuilder` |
| `VagrantSnapshotRestoreCommandTest` | `VagrantSnapshotRestoreCommandBuilder` |
| `VagrantSnapshotSaveCommandTest` | `VagrantSnapshotSaveCommandBuilder` |
| `VagrantDestroyCommandTest` | `VagrantDestroyCommandBuilder` |
| `VagrantGlobalStatusCommandTest` | `VagrantGlobalStatusCommandBuilder` |
| `VagrantHaltCommandTest` | `VagrantHaltCommandBuilder` |
| `VagrantInitCommandTest` | `VagrantInitCommandBuilder` |
| `VagrantPackageCommandTest` | `VagrantPackageCommandBuilder` |
| `VagrantPortCommandTest` | `VagrantPortCommandBuilder` |
| `VagrantPowerShellCommandTest` | `VagrantPowerShellCommandBuilder` |
| `VagrantProvisionCommandTest` | `VagrantProvisionCommandBuilder` |
| `VagrantRdpCommandTest` | `VagrantRdpCommandBuilder` |
| `VagrantReloadCommandTest` | `VagrantReloadCommandBuilder` |
| `VagrantResumeCommandTest` | `VagrantResumeCommandBuilder` |
| `VagrantSshCommandTest` | `VagrantSshCommandBuilder` |
| `VagrantSshConfigCommandTest` | `VagrantSshConfigCommandBuilder` |
| `VagrantStatusCommandTest` | `VagrantStatusCommandBuilder` |
| `VagrantSuspendCommandTest` | `VagrantSuspendCommandBuilder` |
| `VagrantUpCommandTest` | `VagrantUpCommandBuilder` |
| `VagrantValidateCommandTest` | `VagrantValidateCommandBuilder` |
| `VagrantVersionCommandTest` | `VagrantVersionCommandBuilder` |
| `VagrantWinrmCommandTest` | `VagrantWinrmCommandBuilder` |
| `VagrantWinrmConfigCommandTest` | `VagrantWinrmConfigCommandBuilder` |

## Architecture

### Project Structure

```
Vagrant/
├── src/
│   ├── FrenchExDev.Net.Vagrant/           # Main library
│   │   ├── Code.cs                        # Command classes and interfaces
│   │   └── Builders.cs                    # Fluent builder classes
│   └── FrenchExDev.Net.Vagrant.Testing/   # Testing helpers
│       └── Code.cs                        # Test helper classes
└── test/
    └── FrenchExDev.Net.Vagrant.Tests/     # Unit tests
        └── UnitTest1.cs
```

### Key Interfaces

#### `IVagrantCommand`

The core interface implemented by all Vagrant commands:

```csharp
public interface IVagrantCommand
{
    /// <summary>Returns the executable file name (defaults to "vagrant").</summary>
    string Executable => "vagrant";

    /// <summary>Returns the ordered list of command line arguments.</summary>
    IReadOnlyList<string> ToArguments();

    /// <summary>Creates a configured ProcessStartInfo ready to start.</summary>
    ProcessStartInfo ToProcessStartInfo(string? workingDirectory = null);

    /// <summary>Optional working directory hint.</summary>
    string? WorkingDirectory { get; }

    /// <summary>Optional environment variables to inject.</summary>
    IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
}
```

### Command Hierarchy

```
IVagrantCommand
├── VagrantBoxCommand (abstract)
│   ├── VagrantBoxAddCommand
│   ├── VagrantBoxListCommand
│   ├── VagrantBoxOutdatedCommand
│   ├── VagrantBoxPruneCommand
│   ├── VagrantBoxRemoveCommand
│   ├── VagrantBoxRepackageCommand
│   └── VagrantBoxUpdateCommand
├── VagrantCloudCommand (abstract)
│   ├── VagrantCloudAuthCommand
│   ├── VagrantCloudBoxCommand
│   ├── VagrantCloudProviderCommand
│   ├── VagrantCloudSearchCommand
│   └── VagrantCloudVersionCommand
├── VagrantPluginCommand (abstract)
│   ├── VagrantPluginInstallCommand
│   ├── VagrantPluginLicenseCommand
│   ├── VagrantPluginListCommand
│   ├── VagrantPluginRepairCommand
│   ├── VagrantPluginUninstallCommand
│   └── VagrantPluginUpdateCommand
├── VagrantSnapshotCommand (abstract)
│   ├── VagrantSnapshotDeleteCommand
│   ├── VagrantSnapshotListCommand
│   ├── VagrantSnapshotPopCommand
│   ├── VagrantSnapshotPushCommand
│   ├── VagrantSnapshotRestoreCommand
│   └── VagrantSnapshotSaveCommand
└── Top-Level Commands
    ├── VagrantDestroyCommand
    ├── VagrantGlobalStatusCommand
    ├── VagrantHaltCommand
    ├── VagrantInitCommand
    ├── VagrantPackageCommand
    ├── VagrantPortCommand
    ├── VagrantPowerShellCommand
    ├── VagrantProvisionCommand
    ├── VagrantRdpCommand
    ├── VagrantReloadCommand
    ├── VagrantResumeCommand
    ├── VagrantSshCommand
    ├── VagrantSshConfigCommand
    ├── VagrantStatusCommand
    ├── VagrantSuspendCommand
    ├── VagrantUpCommand
    ├── VagrantValidateCommand
    ├── VagrantVersionCommand
    ├── VagrantWinrmCommand
    └── VagrantWinrmConfigCommand
```

## Dependencies

- **FrenchExDev.Net.CSharp.Object.Builder** - Abstract builder pattern implementation
- **FrenchExDev.Net.CSharp.Object.Result** - Result type for validation outcomes
- **FrenchExDev.Net.CSharp.Object.Builder.Testing** - Testing extensions (for Testing package)

## Requirements

- .NET 10.0 or later
- Vagrant CLI installed and available in PATH (for command execution)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Related Projects

- [FrenchExDev.Net.Packer](../Packer/README.md) - Similar library for HashiCorp Packer CLI
- [FrenchExDev.Net.CSharp.Object.Builder](../CSharp.Object.Builder/README.md) - Abstract builder pattern library
```
