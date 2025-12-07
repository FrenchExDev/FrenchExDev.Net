using System.Diagnostics;
using System.Text;

namespace FrenchExDev.Net.Vagrant;

#region Core Interface and Base

/// <summary>
/// Common contract for a Vagrant CLI command (e.g. up, halt, ssh).
/// Implementations provide an executable name (vagrant) and ordered argument list.
/// </summary>
public interface IVagrantCommand
{
    /// <summary>Returns the executable file name (defaults to "vagrant").</summary>
    string Executable => "vagrant";

    /// <summary>Returns the ordered list of command line arguments.</summary>
    IReadOnlyList<string> ToArguments();

    /// <summary>Creates a configured <see cref="ProcessStartInfo"/> ready to start.</summary>
    ProcessStartInfo ToProcessStartInfo(string? workingDirectory = null)
    {
        var psi = new ProcessStartInfo(Executable)
        {
            WorkingDirectory = workingDirectory ?? WorkingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            ArgumentList = { }
        };
        foreach (var arg in ToArguments()) psi.ArgumentList.Add(arg);
        foreach (var kv in EnvironmentVariables)
        {
            psi.Environment[kv.Key] = kv.Value;
        }
        return psi;
    }

    /// <summary>Optional working directory hint (used when building a process start info).</summary>
    string? WorkingDirectory { get; }

    /// <summary>Optional environment variables to inject when launching the CLI.</summary>
    IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
}

#endregion

#region Abstract Command Groups

/// <summary>
/// Abstract base for vagrant box subcommands (add, list, outdated, prune, remove, repackage, update).
/// </summary>
public abstract class VagrantBoxCommand : IVagrantCommand
{
    protected readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public void Env(string key, string value) => _env[key] = value;

    /// <inheritdoc/>
    public abstract IReadOnlyList<string> ToArguments();
}

/// <summary>
/// Abstract base for vagrant cloud subcommands (auth, box, provider, search, version).
/// </summary>
public abstract class VagrantCloudCommand : IVagrantCommand
{
    protected readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public void Env(string key, string value) => _env[key] = value;

    /// <inheritdoc/>
    public abstract IReadOnlyList<string> ToArguments();
}

/// <summary>
/// Abstract base for vagrant plugin subcommands (install, license, list, repair, uninstall, update).
/// </summary>
public abstract class VagrantPluginCommand : IVagrantCommand
{
    protected readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public void Env(string key, string value) => _env[key] = value;

    /// <inheritdoc/>
    public abstract IReadOnlyList<string> ToArguments();
}

/// <summary>
/// Abstract base for vagrant snapshot subcommands (delete, list, pop, push, restore, save).
/// </summary>
public abstract class VagrantSnapshotCommand : IVagrantCommand
{
    protected readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public void Env(string key, string value) => _env[key] = value;

    /// <inheritdoc/>
    public abstract IReadOnlyList<string> ToArguments();
}

#endregion

#region Box Commands

/// <summary>
/// Represents the vagrant box add command.
/// </summary>
public sealed class VagrantBoxAddCommand : VagrantBoxCommand
{
    /// <summary>The name, url, or path of the box to add.</summary>
    public required string Box { get; init; }

    /// <summary>Constrain version of the added box.</summary>
    public string? BoxVersion { get; init; }

    /// <summary>CA certificate for SSL download.</summary>
    public string? CaCert { get; init; }

    /// <summary>CA certificate directory for SSL download.</summary>
    public string? CaPath { get; init; }

    /// <summary>Client certificate for SSL download.</summary>
    public string? Cert { get; init; }

    /// <summary>Checksum for the downloaded box.</summary>
    public string? Checksum { get; init; }

    /// <summary>Checksum type (e.g., md5, sha1, sha256, sha384, sha512).</summary>
    public string? ChecksumType { get; init; }

    /// <summary>Clean any temporary download files.</summary>
    public bool Clean { get; init; }

    /// <summary>Overwrite an existing box if it exists.</summary>
    public bool Force { get; init; }

    /// <summary>Do not validate SSL certificates.</summary>
    public bool Insecure { get; init; }

    /// <summary>Trust 'Location' header from HTTP redirects and use the same credentials for subsequent URLs.</summary>
    public bool LocationTrusted { get; init; }

    /// <summary>Name of the box (required for direct box file paths).</summary>
    public string? Name { get; init; }

    /// <summary>Provider for the box.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "add" };
        if (!string.IsNullOrWhiteSpace(BoxVersion))
        {
            args.Add("--box-version");
            args.Add(BoxVersion);
        }
        if (!string.IsNullOrWhiteSpace(CaCert))
        {
            args.Add("--cacert");
            args.Add(CaCert);
        }
        if (!string.IsNullOrWhiteSpace(CaPath))
        {
            args.Add("--capath");
            args.Add(CaPath);
        }
        if (!string.IsNullOrWhiteSpace(Cert))
        {
            args.Add("--cert");
            args.Add(Cert);
        }
        if (!string.IsNullOrWhiteSpace(Checksum))
        {
            args.Add("--checksum");
            args.Add(Checksum);
        }
        if (!string.IsNullOrWhiteSpace(ChecksumType))
        {
            args.Add("--checksum-type");
            args.Add(ChecksumType);
        }
        if (Clean)
            args.Add("--clean");
        if (Force)
            args.Add("--force");
        if (Insecure)
            args.Add("--insecure");
        if (LocationTrusted)
            args.Add("--location-trusted");
        if (!string.IsNullOrWhiteSpace(Name))
        {
            args.Add("--name");
            args.Add(Name);
        }
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        args.Add(Box);
        return args;
    }
}

/// <summary>
/// Represents the vagrant box list command.
/// </summary>
// TODO: Implement properties for --box-info, --machine-readable
public sealed class VagrantBoxListCommand : VagrantBoxCommand
{
    /// <summary>Display additional box info.</summary>
    public bool BoxInfo { get; init; }

    /// <summary>Enable machine readable output.</summary>
    public bool MachineReadable { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "list" };
        if (BoxInfo)
            args.Add("--box-info");
        if (MachineReadable)
            args.Add("--machine-readable");
        return args;
    }
}

/// <summary>
/// Represents the vagrant box outdated command.
/// </summary>
public sealed class VagrantBoxOutdatedCommand : VagrantBoxCommand
{
    /// <summary>Force check for latest box version.</summary>
    public bool Force { get; init; }

    /// <summary>Check all boxes installed.</summary>
    public bool Global { get; init; }

    /// <summary>Enable machine readable output.</summary>
    public bool MachineReadable { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "outdated" };
        if (Force)
            args.Add("--force");
        if (Global)
            args.Add("--global");
        if (MachineReadable)
            args.Add("--machine-readable");
        return args;
    }
}

/// <summary>
/// Represents the vagrant box prune command.
/// </summary>
public sealed class VagrantBoxPruneCommand : VagrantBoxCommand
{
    /// <summary>Only print the boxes that would be removed.</summary>
    public bool DryRun { get; init; }

    /// <summary>Destroy without confirmation.</summary>
    public bool Force { get; init; }

    /// <summary>Keep boxes still actively in use.</summary>
    public bool KeepActiveBoxes { get; init; }

    /// <summary>The specific box name to prune.</summary>
    public string? Name { get; init; }

    /// <summary>The specific provider to prune.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "prune" };
        if (DryRun)
            args.Add("--dry-run");
        if (Force)
            args.Add("--force");
        if (KeepActiveBoxes)
            args.Add("--keep-active-boxes");
        if (!string.IsNullOrWhiteSpace(Name))
        {
            args.Add("--name");
            args.Add(Name);
        }
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        return args;
    }
}

/// <summary>
/// Represents the vagrant box remove command.
/// </summary>
public sealed class VagrantBoxRemoveCommand : VagrantBoxCommand
{
    /// <summary>The name of the box to remove.</summary>
    public required string Name { get; init; }

    /// <summary>Remove all available versions of the box.</summary>
    public bool All { get; init; }

    /// <summary>The specific version of the box to remove.</summary>
    public string? BoxVersion { get; init; }

    /// <summary>Destroy without confirmation.</summary>
    public bool Force { get; init; }

    /// <summary>The specific provider of the box to remove.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "remove" };
        if (All)
            args.Add("--all");
        if (!string.IsNullOrWhiteSpace(BoxVersion))
        {
            args.Add("--box-version");
            args.Add(BoxVersion);
        }
        if (Force)
            args.Add("--force");
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        args.Add(Name);
        return args;
    }
}

/// <summary>
/// Represents the vagrant box repackage command.
/// </summary>
// TODO: Implement full options support
public sealed class VagrantBoxRepackageCommand : VagrantBoxCommand
{
    /// <summary>The name of the box to repackage.</summary>
    public required string Name { get; init; }

    /// <summary>The provider of the box.</summary>
    public required string Provider { get; init; }

    /// <summary>A cartouche is a carved tablet or drawing representing a scroll with twisted ends.</summary>
    /// <remarks>It is used in the context of packaging and distributing Vagrant boxes.</remarks>
    public required string Cartouche { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "repackage", Name, Provider, Cartouche };
        return args;
    }
}

/// <summary>
/// Represents the vagrant box update command.
/// </summary>
public sealed class VagrantBoxUpdateCommand : VagrantBoxCommand
{
    /// <summary>Update a specific box.</summary>
    public string? Box { get; init; }

    /// <summary>CA certificate for SSL download.</summary>
    public string? CaCert { get; init; }

    /// <summary>CA certificate directory for SSL download.</summary>
    public string? CaPath { get; init; }

    /// <summary>Client certificate for SSL download.</summary>
    public string? Cert { get; init; }

    /// <summary>Force check for latest box version.</summary>
    public bool Force { get; init; }

    /// <summary>Do not validate SSL certificates.</summary>
    public bool Insecure { get; init; }

    /// <summary>Update box for a specific provider.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "update" };
        if (!string.IsNullOrWhiteSpace(Box))
        {
            args.Add("--box");
            args.Add(Box);
        }
        if (!string.IsNullOrWhiteSpace(CaCert))
        {
            args.Add("--cacert");
            args.Add(CaCert);
        }
        if (!string.IsNullOrWhiteSpace(CaPath))
        {
            args.Add("--capath");
            args.Add(CaPath);
        }
        if (!string.IsNullOrWhiteSpace(Cert))
        {
            args.Add("--cert");
            args.Add(Cert);
        }
        if (Force)
            args.Add("--force");
        if (Insecure)
            args.Add("--insecure");
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        return args;
    }
}

#endregion

#region Cloud Commands

/// <summary>
/// Represents the vagrant cloud auth subcommands.
/// </summary>
// TODO: Implement login, logout, whoami subcommands with their respective options
public sealed class VagrantCloudAuthCommand : VagrantCloudCommand
{
    /// <summary>The auth subcommand (login, logout, whoami).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "auth", SubCommand };
        // TODO: Add options based on subcommand
        return args;
    }
}

/// <summary>
/// Represents the vagrant cloud box subcommands.
/// </summary>
// TODO: Implement create, delete, show, update subcommands with their respective options
public sealed class VagrantCloudBoxCommand : VagrantCloudCommand
{
    /// <summary>The box subcommand (create, delete, show, update).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "box", SubCommand };
        // TODO: Add options based on subcommand
        return args;
    }
}

/// <summary>
/// Represents the vagrant cloud provider subcommands.
/// </summary>
// TODO: Implement create, delete, update, upload subcommands with their respective options
public sealed class VagrantCloudProviderCommand : VagrantCloudCommand
{
    /// <summary>The provider subcommand (create, delete, update, upload).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "provider", SubCommand };
        // TODO: Add options based on subcommand
        return args;
    }
}

/// <summary>
/// Represents the vagrant cloud search command.
/// </summary>
public sealed class VagrantCloudSearchCommand : VagrantCloudCommand
{
    /// <summary>The search query.</summary>
    public string? Query { get; init; }

    /// <summary>Format results in JSON.</summary>
    public bool Json { get; init; }

    /// <summary>Maximum number of results to display.</summary>
    public int? Limit { get; init; }

    /// <summary>Page number to display.</summary>
    public int? Page { get; init; }

    /// <summary>Filter results to a specific provider.</summary>
    public string? Provider { get; init; }

    /// <summary>Display short results (just box names).</summary>
    public bool Short { get; init; }

    /// <summary>Field to sort results by (e.g., downloads, created, updated).</summary>
    public string? SortBy { get; init; }

    /// <summary>Order of results (asc or desc).</summary>
    public string? Order { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "search" };
        if (Json)
            args.Add("--json");
        if (Limit.HasValue)
        {
            args.Add("--limit");
            args.Add(Limit.Value.ToString());
        }
        if (Page.HasValue)
        {
            args.Add("--page");
            args.Add(Page.Value.ToString());
        }
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        if (Short)
            args.Add("--short");
        if (!string.IsNullOrWhiteSpace(SortBy))
        {
            args.Add("--sort-by");
            args.Add(SortBy);
        }
        if (!string.IsNullOrWhiteSpace(Order))
        {
            args.Add("--order");
            args.Add(Order);
        }
        if (!string.IsNullOrWhiteSpace(Query))
            args.Add(Query);
        return args;
    }
}

/// <summary>
/// Represents the vagrant cloud version subcommands.
/// </summary>
// TODO: Implement create, delete, release, revoke, update subcommands with their respective options
public sealed class VagrantCloudVersionCommand : VagrantCloudCommand
{
    /// <summary>The version subcommand (create, delete, release, revoke, update).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "version", SubCommand };
        // TODO: Add options based on subcommand
        return args;
    }
}

#endregion

#region Plugin Commands

/// <summary>
/// Represents the vagrant plugin install command.
/// </summary>
public sealed class VagrantPluginInstallCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin to install.</summary>
    public required string Name { get; init; }

    /// <summary>The name of the entry point file for loading the plugin.</summary>
    public string? EntryPoint { get; init; }

    /// <summary>Install plugin for local project only.</summary>
    public bool Local { get; init; }

    /// <summary>Clear plugin sources before install.</summary>
    public bool PluginCleanSources { get; init; }

    /// <summary>Custom plugin source URL.</summary>
    public string? PluginSource { get; init; }

    /// <summary>The specific version of the plugin to install.</summary>
    public string? PluginVersion { get; init; }

    /// <summary>Enable verbose output for plugin installation.</summary>
    public bool Verbose { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "install" };
        if (!string.IsNullOrWhiteSpace(EntryPoint))
        {
            args.Add("--entry-point");
            args.Add(EntryPoint);
        }
        if (Local)
            args.Add("--local");
        if (PluginCleanSources)
            args.Add("--plugin-clean-sources");
        if (!string.IsNullOrWhiteSpace(PluginSource))
        {
            args.Add("--plugin-source");
            args.Add(PluginSource);
        }
        if (!string.IsNullOrWhiteSpace(PluginVersion))
        {
            args.Add("--plugin-version");
            args.Add(PluginVersion);
        }
        if (Verbose)
            args.Add("--verbose");
        args.Add(Name);
        return args;
    }
}

/// <summary>
/// Represents the vagrant plugin license command.
/// </summary>
// TODO: Implement full options support
public sealed class VagrantPluginLicenseCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin.</summary>
    public required string Name { get; init; }

    /// <summary>The path to the license file.</summary>
    public required string LicenseFile { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["plugin", "license", Name, LicenseFile];
    }
}

/// <summary>
/// Represents the vagrant plugin list command.
/// </summary>
public sealed class VagrantPluginListCommand : VagrantPluginCommand
{
    /// <summary>List plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "list" };
        if (Local)
            args.Add("--local");
        return args;
    }
}

/// <summary>
/// Represents the vagrant plugin repair command.
/// </summary>
public sealed class VagrantPluginRepairCommand : VagrantPluginCommand
{
    /// <summary>Repair plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "repair" };
        if (Local)
            args.Add("--local");
        return args;
    }
}

/// <summary>
/// Represents the vagrant plugin uninstall command.
/// </summary>
public sealed class VagrantPluginUninstallCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin to uninstall.</summary>
    public required string Name { get; init; }

    /// <summary>Uninstall plugin from local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "uninstall" };
        if (Local)
            args.Add("--local");
        args.Add(Name);
        return args;
    }
}

/// <summary>
/// Represents the vagrant plugin update command.
/// </summary>
// TODO: Implement properties for --local
public sealed class VagrantPluginUpdateCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin to update (optional, updates all if not specified).</summary>
    public string? Name { get; init; }

    /// <summary>Update plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "update" };
        if (Local)
            args.Add("--local");
        if (!string.IsNullOrWhiteSpace(Name))
            args.Add(Name);
        return args;
    }
}

#endregion

#region Snapshot Commands

/// <summary>
/// Represents the vagrant snapshot delete command.
/// </summary>
// TODO: Implement full options support
public sealed class VagrantSnapshotDeleteCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to delete.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "delete" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}

/// <summary>
/// Represents the vagrant snapshot list command.
/// </summary>
// TODO: Implement full options support
public sealed class VagrantSnapshotListCommand : VagrantSnapshotCommand
{
    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "list" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant snapshot pop command.
/// </summary>
public sealed class VagrantSnapshotPopCommand : VagrantSnapshotCommand
{
    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Do not delete the snapshot after restoring.</summary>
    public bool NoDelete { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Do not start the VM after restore.</summary>
    public bool NoStart { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "pop" };
        if (NoDelete)
            args.Add("--no-delete");
        if (NoProvision)
            args.Add("--no-provision");
        if (NoStart)
            args.Add("--no-start");
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant snapshot push command.
/// </summary>
// TODO: Implement full options support
public sealed class VagrantSnapshotPushCommand : VagrantSnapshotCommand
{
    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "push" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant snapshot restore command.
/// </summary>
public sealed class VagrantSnapshotRestoreCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to restore.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Do not start the VM after restore.</summary>
    public bool NoStart { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "restore" };
        if (NoProvision)
            args.Add("--no-provision");
        if (NoStart)
            args.Add("--no-start");
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}

/// <summary>
/// Represents the vagrant snapshot save command.
/// </summary>
public sealed class VagrantSnapshotSaveCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to save.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Replace an existing snapshot with the same name.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "save" };
        if (Force)
            args.Add("--force");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}

#endregion

#region Top-Level Commands

/// <summary>
/// Represents the vagrant destroy command.
/// </summary>
public sealed class VagrantDestroyCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Destroy without confirmation.</summary>
    public bool Force { get; init; }

    /// <summary>Gracefully power off of VM.</summary>
    public bool Graceful { get; init; }

    /// <summary>Enable or disable parallelism if provider supports it.</summary>
    public bool Parallel { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantDestroyCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "destroy" };
        if (Force)
            args.Add("--force");
        if (Graceful)
            args.Add("--graceful");
        if (Parallel)
            args.Add("--parallel");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant global-status command.
/// </summary>
// TODO: Implement properties for --prune
public sealed class VagrantGlobalStatusCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Prune invalid entries.</summary>
    public bool Prune { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantGlobalStatusCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "global-status" };
        if (Prune)
            args.Add("--prune");
        return args;
    }
}

/// <summary>
/// Represents the vagrant halt command.
/// </summary>
public sealed class VagrantHaltCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Force shutdown (power off) the VM.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantHaltCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "halt" };
        if (Force)
            args.Add("--force");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant init command.
/// </summary>
// TODO: Implement properties for --box-version, --force, --minimal, --output, --template
public sealed class VagrantInitCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The name of the box to use for initialization.</summary>
    public string? BoxName { get; init; }

    /// <summary>The URL of the box.</summary>
    public string? BoxUrl { get; init; }

    /// <summary>Constrain version of the box.</summary>
    public string? BoxVersion { get; init; }

    /// <summary>Overwrite an existing Vagrantfile.</summary>
    public bool Force { get; init; }

    /// <summary>Create a minimal Vagrantfile (no comments or helpers).</summary>
    public bool Minimal { get; init; }

    /// <summary>Output path for the generated Vagrantfile.</summary>
    public string? Output { get; init; }

    /// <summary>Path to a custom Vagrantfile template.</summary>
    public string? Template { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantInitCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "init" };
        if (!string.IsNullOrWhiteSpace(BoxVersion))
        {
            args.Add("--box-version");
            args.Add(BoxVersion);
        }
        if (Force)
            args.Add("--force");
        if (Minimal)
            args.Add("--minimal");
        if (!string.IsNullOrWhiteSpace(Output))
        {
            args.Add("--output");
            args.Add(Output);
        }
        if (!string.IsNullOrWhiteSpace(Template))
        {
            args.Add("--template");
            args.Add(Template);
        }
        if (!string.IsNullOrWhiteSpace(BoxName))
            args.Add(BoxName);
        if (!string.IsNullOrWhiteSpace(BoxUrl))
            args.Add(BoxUrl);
        return args;
    }
}

/// <summary>
/// Represents the vagrant package command.
/// </summary>
// TODO: Implement properties for --base, --include, --output, --vagrantfile
public sealed class VagrantPackageCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Name of a VM in VirtualBox to package as a base box.</summary>
    public string? Base { get; init; }

    /// <summary>Comma-separated list of additional files to package.</summary>
    public IReadOnlyList<string>? Include { get; init; }

    /// <summary>Name of the output box file.</summary>
    public string? Output { get; init; }

    /// <summary>Path to a Vagrantfile to include in the box.</summary>
    public string? Vagrantfile { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantPackageCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "package" };
        if (!string.IsNullOrWhiteSpace(Base))
        {
            args.Add("--base");
            args.Add(Base);
        }
        if (Include is { Count: > 0 })
        {
            args.Add("--include");
            args.Add(string.Join(",", Include));
        }
        if (!string.IsNullOrWhiteSpace(Output))
        {
            args.Add("--output");
            args.Add(Output);
        }
        if (!string.IsNullOrWhiteSpace(Vagrantfile))
        {
            args.Add("--vagrantfile");
            args.Add(Vagrantfile);
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant port command.
/// </summary>
// TODO: Implement properties for --guest, --machine-readable
public sealed class VagrantPortCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Output the host port for the given guest port.</summary>
    public int? Guest { get; init; }

    /// <summary>Enable machine readable output.</summary>
    public bool MachineReadable { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantPortCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "port" };
        if (Guest.HasValue)
        {
            args.Add("--guest");
            args.Add(Guest.Value.ToString());
        }
        if (MachineReadable)
            args.Add("--machine-readable");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant powershell command.
/// </summary>
public sealed class VagrantPowerShellCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Command to execute on the guest.</summary>
    public string? Command { get; init; }

    /// <summary>Run as an elevated (administrator) command.</summary>
    public bool Elevated { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantPowerShellCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "powershell" };
        if (!string.IsNullOrWhiteSpace(Command))
        {
            args.Add("--command");
            args.Add(Command);
        }
        if (Elevated)
            args.Add("--elevated");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant provision command.
/// </summary>
public sealed class VagrantProvisionCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantProvisionCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "provision" };
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant rdp command.
/// </summary>
public sealed class VagrantRdpCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Extra arguments to pass to the RDP client.</summary>
    public string? ExtraArgs { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantRdpCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "rdp" };
        if (!string.IsNullOrWhiteSpace(ExtraArgs))
        {
            args.Add("--");
            args.Add(ExtraArgs);
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant reload command.
/// </summary>
public sealed class VagrantReloadCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Force shutdown before reload.</summary>
    public bool Force { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantReloadCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "reload" };
        if (Force)
            args.Add("--force");
        if (NoProvision)
            args.Add("--no-provision");
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant resume command.
/// </summary>
public sealed class VagrantResumeCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantResumeCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "resume" };
        if (NoProvision)
            args.Add("--no-provision");
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant ssh command.
/// </summary>
public sealed class VagrantSshCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Command to execute on the guest.</summary>
    public string? Command { get; init; }

    /// <summary>Extra arguments to pass to the SSH client.</summary>
    public string? ExtraArgs { get; init; }

    /// <summary>Plain mode: don't authenticate with Vagrant.</summary>
    public bool Plain { get; init; }

    /// <summary>Allocate a TTY.</summary>
    public bool Tty { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantSshCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "ssh" };
        if (!string.IsNullOrWhiteSpace(Command))
        {
            args.Add("--command");
            args.Add(Command);
        }
        if (Plain)
            args.Add("--plain");
        if (Tty)
            args.Add("--tty");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        if (!string.IsNullOrWhiteSpace(ExtraArgs))
        {
            args.Add("--");
            args.Add(ExtraArgs);
        }
        return args;
    }
}

/// <summary>
/// Represents the vagrant ssh-config command.
/// </summary>
// TODO: Implement properties for --host
public sealed class VagrantSshConfigCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Name the host for the config.</summary>
    public string? Host { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantSshConfigCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "ssh-config" };
        if (!string.IsNullOrWhiteSpace(Host))
        {
            args.Add("--host");
            args.Add(Host);
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant up command.
/// </summary>
public sealed class VagrantUpCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Destroy machine on any error.</summary>
    public bool DestroyOnError { get; init; }

    /// <summary>Install provider if missing.</summary>
    public bool InstallProvider { get; init; }

    /// <summary>Do not destroy machine on error.</summary>
    public bool NoDestroyOnError { get; init; }

    /// <summary>Do not install provider if missing.</summary>
    public bool NoInstallProvider { get; init; }

    /// <summary>Disable parallelism.</summary>
    public bool NoParallel { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Enable parallelism.</summary>
    public bool Parallel { get; init; }

    /// <summary>Bring machine up with the given provider.</summary>
    public string? Provider { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantUpCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "up" };
        if (DestroyOnError)
            args.Add("--destroy-on-error");
        if (InstallProvider)
            args.Add("--install-provider");
        if (NoDestroyOnError)
            args.Add("--no-destroy-on-error");
        if (NoInstallProvider)
            args.Add("--no-install-provider");
        if (NoParallel)
            args.Add("--no-parallel");
        if (NoProvision)
            args.Add("--no-provision");
        if (Parallel)
            args.Add("--parallel");
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant winrm command.
/// </summary>
public sealed class VagrantWinrmCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Command to execute on the guest.</summary>
    public string? Command { get; init; }

    /// <summary>Run as an elevated (administrator) command.</summary>
    public bool Elevated { get; init; }

    /// <summary>The shell to use (e.g., cmd, powershell).</summary>
    public string? Shell { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantWinrmCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "winrm" };
        if (!string.IsNullOrWhiteSpace(Command))
        {
            args.Add("--command");
            args.Add(Command);
        }
        if (Elevated)
            args.Add("--elevated");
        if (!string.IsNullOrWhiteSpace(Shell))
        {
            args.Add("--shell");
            args.Add(Shell);
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant winrm-config command.
/// </summary>
public sealed class VagrantWinrmConfigCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Name the host for the config.</summary>
    public string? Host { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantWinrmConfigCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "winrm-config" };
        if (!string.IsNullOrWhiteSpace(Host))
        {
            args.Add("--host");
            args.Add(Host);
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant status command.
/// </summary>
public sealed class VagrantStatusCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantStatusCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "status" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant suspend command.
/// </summary>
public sealed class VagrantSuspendCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantSuspendCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "suspend" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}

/// <summary>
/// Represents the vagrant validate command.
/// </summary>
public sealed class VagrantValidateCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantValidateCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        return ["validate"];
    }
}

/// <summary>
/// Represents the vagrant version command.
/// </summary>
public sealed class VagrantVersionCommand : IVagrantCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string? WorkingDirectory { get; init; }

    /// <inheritdoc/>
    IReadOnlyDictionary<string, string> IVagrantCommand.EnvironmentVariables => _env;

    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public VagrantVersionCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <inheritdoc/>
    public IReadOnlyList<string> ToArguments()
    {
        return ["version"];
    }
}

#endregion

#region Extensions

/// <summary>
/// Utility helpers for <see cref="IVagrantCommand"/>.
/// </summary>
public static class VagrantCommandExtensions
{
    /// <summary>Renders a shell-safe single string (quoted where needed).</summary>
    public static string ToCommandLine(this IVagrantCommand command)
    {
        var sb = new StringBuilder(command.Executable);
        foreach (var a in command.ToArguments())
        {
            sb.Append(' ');
            sb.Append(NeedsQuoting(a) ? Quote(a) : a);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the specified text requires quoting based on its content.
    /// </summary>
    private static bool NeedsQuoting(string text) => string.IsNullOrEmpty(text) || text.Any(char.IsWhiteSpace) || text.Contains('"');

    /// <summary>
    /// Encloses the specified string in double quotes and escapes any internal double quote characters.
    /// </summary>
    private static string Quote(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }

    /// <summary>
    /// Starts a new process using the specified vagrant command and attaches optional handlers for standard output and error data.
    /// </summary>
    public static Process ToProcess(this IVagrantCommand vagrantCommand, Action<string?>? onStdOut = null, Action<string?>? onStdErr = null)
    {
        var process = new Process();

        process.StartInfo = vagrantCommand.ToProcessStartInfo();

        if (onStdErr is not null)
        {
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                {
                    onStdErr(e.Data);
                }
            };
        }

        if (onStdOut is not null)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                {
                    onStdOut(e.Data);
                }
            };
        }

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        return process;
    }

    /// <summary>
    /// Asynchronously waits for the associated process to exit.
    /// </summary>
    public static async Task<bool> WaitAsync(this Process process)
    {
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }
}

#endregion
