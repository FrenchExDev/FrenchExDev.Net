using FrenchExDev.Net.Vagrant.Commands.Invocation;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public sealed class UpInvocationBuilder : VagrantInvocationBuilderBase<UpInvocationBuilder>
{
    public UpInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("up"));
    public UpInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public UpInvocationBuilder Provider(string provider) => Option("provider", provider);
    public UpInvocationBuilder Parallel() => Flag("parallel");
    public UpInvocationBuilder DestroyOnError() => Flag("destroy-on-error");
    public UpInvocationBuilder Color(string? value = null) { if (value is null) Flag("color"); else Option("color", value); return this; }
    public UpInvocationBuilder Provision() => Flag("provision");
    public UpInvocationBuilder NoProvision() => Flag("no-provision");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return; // short circuit
        var hasProvision = _options.ContainsKey("provision");
        var hasNoProvision = _options.ContainsKey("no-provision");
        if (hasProvision && hasNoProvision)
            failures.Failure("Options", new InvalidDataException("Options --provision and --no-provision are mutually exclusive."));
        if (_options.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
        if (_options.TryGetValue("color", out var colorVal) && colorVal is not null &&
            !(string.Equals(colorVal, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(colorVal, "false", StringComparison.OrdinalIgnoreCase)))
            failures.Failure("Options", new InvalidDataException("--color value must be 'true' or 'false' when specified"));
        if (_parameters.TryGetValue("machine", out var mvals) && mvals.Any(v => string.IsNullOrWhiteSpace(v)))
            failures.Failure("Parameters", new InvalidDataException("Machine names cannot be empty"));
    }
}

public sealed class BoxAddInvocationBuilder : VagrantInvocationBuilderBase<BoxAddInvocationBuilder>
{
    public BoxAddInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","add"));
    public BoxAddInvocationBuilder Source(string src) => Param("source", src);
    public BoxAddInvocationBuilder Name(string name) => Option("name", name);
    public BoxAddInvocationBuilder Checksum(string checksum) => Option("checksum", checksum);
    public BoxAddInvocationBuilder ChecksumType(string type) => Option("checksum-type", type);
    public BoxAddInvocationBuilder BoxVersion(string v) => Option("box-version", v);
    public BoxAddInvocationBuilder Force() => Flag("force");
    public BoxAddInvocationBuilder Clean() => Flag("clean");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.TryGetValue("checksum", out var checksum) && string.IsNullOrWhiteSpace(checksum))
            failures.Failure("Options", new InvalidDataException("--checksum cannot be empty"));
        if (_options.TryGetValue("checksum-type", out var cstype))
        {
            if (string.IsNullOrWhiteSpace(cstype)) failures.Failure("Options", new InvalidDataException("--checksum-type cannot be empty"));
            else
            {
                var allowed = new[] {"sha1","sha256","sha512","md5"};
                if (!allowed.Contains(cstype, StringComparer.OrdinalIgnoreCase))
                    failures.Failure("Options", new InvalidDataException("Unsupported checksum type"));
                if (!_options.ContainsKey("checksum"))
                    failures.Failure("Options", new InvalidDataException("--checksum-type requires --checksum"));
            }
        }
        if (_options.TryGetValue("box-version", out var bv) && string.IsNullOrWhiteSpace(bv))
            failures.Failure("Options", new InvalidDataException("--box-version cannot be empty"));
        if (_parameters.TryGetValue("source", out var srcVals) && srcVals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Source parameter cannot be empty"));
    }
}

public sealed class BoxRemoveInvocationBuilder : VagrantInvocationBuilderBase<BoxRemoveInvocationBuilder>
{
    public BoxRemoveInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","remove"));
    public BoxRemoveInvocationBuilder Name(string n) => Param("name", n);
    public BoxRemoveInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxRemoveInvocationBuilder BoxVersion(string v) => Option("box-version", v);
    public BoxRemoveInvocationBuilder All() => Flag("all");
    public BoxRemoveInvocationBuilder Force() => Flag("force");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.ContainsKey("all") && _options.ContainsKey("box-version"))
            failures.Failure("Options", new InvalidDataException("--all cannot be combined with --box-version"));
        if (_options.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
        if (_options.TryGetValue("box-version", out var ver) && string.IsNullOrWhiteSpace(ver))
            failures.Failure("Options", new InvalidDataException("--box-version cannot be empty"));
        if (_parameters.TryGetValue("name", out var nvals) && nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Name parameter cannot be empty"));
    }
}

public sealed class BoxListInvocationBuilder : VagrantInvocationBuilderBase<BoxListInvocationBuilder>
{
    public BoxListInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","list"));
    public BoxListInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxListInvocationBuilder Force() => Flag("force");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
    }
}

public sealed class BoxOutdatedInvocationBuilder : VagrantInvocationBuilderBase<BoxOutdatedInvocationBuilder>
{
    public BoxOutdatedInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","outdated"));
    public BoxOutdatedInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxOutdatedInvocationBuilder Global() => Flag("global");
    public BoxOutdatedInvocationBuilder Insecure() => Flag("insecure");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
    }
}

public sealed class BoxPruneInvocationBuilder : VagrantInvocationBuilderBase<BoxPruneInvocationBuilder>
{
    public BoxPruneInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","prune"));
    public BoxPruneInvocationBuilder DryRun() => Flag("dry-run");
    public BoxPruneInvocationBuilder KeepActiveProvider() => Flag("keep-active-provider");
    public BoxPruneInvocationBuilder Force() => Flag("force");
}

public sealed class BoxRepackageInvocationBuilder : VagrantInvocationBuilderBase<BoxRepackageInvocationBuilder>
{
    public BoxRepackageInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","repackage"));
    public BoxRepackageInvocationBuilder Name(string name) => Param("name", name);
    public BoxRepackageInvocationBuilder ProviderParam(string provider) => Param("provider", provider);
    public BoxRepackageInvocationBuilder Version(string version) => Param("version", version);
    public BoxRepackageInvocationBuilder Output(string output) => Option("output", output);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        string[] required = ["name","provider","version"];
        foreach (var r in required)
        {
            if (!_parameters.TryGetValue(r, out var vals) || vals.Length == 0 || vals.Any(string.IsNullOrWhiteSpace))
                failures.Failure("Parameters", new InvalidDataException($"Parameter '{r}' is required and cannot be empty"));
        }
        if (_options.TryGetValue("output", out var outv) && string.IsNullOrWhiteSpace(outv))
            failures.Failure("Options", new InvalidDataException("--output cannot be empty"));
    }
}

public sealed class InitInvocationBuilder : VagrantInvocationBuilderBase<InitInvocationBuilder>
{
    public InitInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("init"));
    public InitInvocationBuilder BoxName(string value) => Param("box-name", value);
    public InitInvocationBuilder BoxUrl(string value) => Param("box-url", value);
    public InitInvocationBuilder Box(string value) => Option("box", value);
    public InitInvocationBuilder Output(string file) => Option("output", file);
    public InitInvocationBuilder Minimal() => Flag("minimal");
    public InitInvocationBuilder Force() => Flag("force");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        foreach (var opt in new[]{"box","output"})
            if (_options.TryGetValue(opt, out var val) && string.IsNullOrWhiteSpace(val))
                failures.Failure("Options", new InvalidDataException($"--{opt} cannot be empty"));
    }
}

public sealed class HaltInvocationBuilder : VagrantInvocationBuilderBase<HaltInvocationBuilder>
{
    public HaltInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("halt"));
    public HaltInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public HaltInvocationBuilder Force() => Flag("force");
}

public sealed class DestroyInvocationBuilder : VagrantInvocationBuilderBase<DestroyInvocationBuilder>
{
    public DestroyInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("destroy"));
    public DestroyInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public DestroyInvocationBuilder Force() => Flag("force");
    public DestroyInvocationBuilder Graceful() => Flag("graceful");
}

public sealed class StatusInvocationBuilder : VagrantInvocationBuilderBase<StatusInvocationBuilder>
{
    public StatusInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("status"));
    public StatusInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public StatusInvocationBuilder MachineReadable() => Flag("machine-readable");
}

public sealed class SshInvocationBuilder : VagrantInvocationBuilderBase<SshInvocationBuilder>
{
    private bool _commandParamUsed;
    private bool _commandOptionUsed;
    public SshInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("ssh"));
    public SshInvocationBuilder Machine(string value) => Param("machine", value);
    public SshInvocationBuilder CommandParam(string value) { _commandParamUsed = true; return Param("command", value); }
    public SshInvocationBuilder CommandOption(string value) { _commandOptionUsed = true; return Option("command", value); }
    public SshInvocationBuilder Plain() => Flag("plain");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_commandParamUsed && _commandOptionUsed)
            failures.Failure("Options", new InvalidDataException("Command specified both as parameter and option"));
        if (_options.TryGetValue("command", out var cmdOpt) && string.IsNullOrWhiteSpace(cmdOpt))
            failures.Failure("Options", new InvalidDataException("--command cannot be empty"));
        if (_parameters.TryGetValue("command", out var cmdParamVals) && cmdParamVals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("command parameter cannot be empty"));
    }
}

public sealed class SshConfigInvocationBuilder : VagrantInvocationBuilderBase<SshConfigInvocationBuilder>
{
    public SshConfigInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("ssh-config"));
    public SshConfigInvocationBuilder Machine(string value) => Param("machine", value);
    public SshConfigInvocationBuilder Host(string value) => Option("host", value);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.TryGetValue("host", out var host) && string.IsNullOrWhiteSpace(host))
            failures.Failure("Options", new InvalidDataException("--host cannot be empty"));
    }
}

public sealed class ProvisionInvocationBuilder : VagrantInvocationBuilderBase<ProvisionInvocationBuilder>
{
    public ProvisionInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("provision"));
    public ProvisionInvocationBuilder Machine(string value) => Param("machine", value);
    public ProvisionInvocationBuilder ProvisionWith(string namesCsv) => Option("provision-with", namesCsv);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.TryGetValue("provision-with", out var pw) && string.IsNullOrWhiteSpace(pw))
            failures.Failure("Options", new InvalidDataException("--provision-with cannot be empty"));
    }
}

public sealed class ReloadInvocationBuilder : VagrantInvocationBuilderBase<ReloadInvocationBuilder>
{
    public ReloadInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("reload"));
    public ReloadInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public ReloadInvocationBuilder Provider(string provider) => Option("provider", provider);
    public ReloadInvocationBuilder Provision() => Flag("provision");
    public ReloadInvocationBuilder NoProvision() => Flag("no-provision");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (_options.ContainsKey("provision") && _options.ContainsKey("no-provision"))
            failures.Failure("Options", new InvalidDataException("Options --provision and --no-provision are mutually exclusive."));
        if (_options.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
    }
}

public sealed class ValidateInvocationBuilder : VagrantInvocationBuilderBase<ValidateInvocationBuilder>
{
    public ValidateInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("validate"));
}

public sealed class GlobalStatusInvocationBuilder : VagrantInvocationBuilderBase<GlobalStatusInvocationBuilder>
{
    public GlobalStatusInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("global-status"));
    public GlobalStatusInvocationBuilder Prune() => Flag("prune");
}

public sealed class PackageInvocationBuilder : VagrantInvocationBuilderBase<PackageInvocationBuilder>
{
    public PackageInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("package"));
    public PackageInvocationBuilder Output(string file) => Option("output", file);
    public PackageInvocationBuilder Base(string name) => Option("base", name);
    public PackageInvocationBuilder Include(string file) => Option("include", file);
    public PackageInvocationBuilder Vagrantfile(string file) => Option("vagrantfile", file);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        foreach (var opt in new[]{"output","base","include","vagrantfile"})
            if (_options.TryGetValue(opt, out var val) && string.IsNullOrWhiteSpace(val))
                failures.Failure("Options", new InvalidDataException($"--{opt} cannot be empty"));
    }
}

public sealed class PortInvocationBuilder : VagrantInvocationBuilderBase<PortInvocationBuilder>
{
    public PortInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("port"));
    public PortInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SuspendInvocationBuilder : VagrantInvocationBuilderBase<SuspendInvocationBuilder>
{
    public SuspendInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("suspend"));
    public SuspendInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class ResumeInvocationBuilder : VagrantInvocationBuilderBase<ResumeInvocationBuilder>
{
    public ResumeInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("resume"));
    public ResumeInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SnapshotSaveInvocationBuilder : VagrantInvocationBuilderBase<SnapshotSaveInvocationBuilder>
{
    public SnapshotSaveInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","save"));
    public SnapshotSaveInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotSaveInvocationBuilder Name(string name) => Param("name", name);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (!_parameters.TryGetValue("name", out var nvals) || nvals.Length == 0 || nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
    }
}

public sealed class SnapshotRestoreInvocationBuilder : VagrantInvocationBuilderBase<SnapshotRestoreInvocationBuilder>
{
    public SnapshotRestoreInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","restore"));
    public SnapshotRestoreInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotRestoreInvocationBuilder Name(string name) => Param("name", name);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (!_parameters.TryGetValue("name", out var nvals) || nvals.Length == 0 || nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
    }
}

public sealed class SnapshotListInvocationBuilder : VagrantInvocationBuilderBase<SnapshotListInvocationBuilder>
{
    public SnapshotListInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","list"));
    public SnapshotListInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SnapshotDeleteInvocationBuilder : VagrantInvocationBuilderBase<SnapshotDeleteInvocationBuilder>
{
    public SnapshotDeleteInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","delete"));
    public SnapshotDeleteInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotDeleteInvocationBuilder Name(string name) => Param("name", name);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (!_parameters.TryGetValue("name", out var nvals) || nvals.Length == 0 || nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
    }
}

public sealed class SnapshotPushInvocationBuilder : VagrantInvocationBuilderBase<SnapshotPushInvocationBuilder>
{
    public SnapshotPushInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","push"));
    public SnapshotPushInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SnapshotPopInvocationBuilder : VagrantInvocationBuilderBase<SnapshotPopInvocationBuilder>
{
    public SnapshotPopInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("snapshot","pop"));
    public SnapshotPopInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class PluginInstallInvocationBuilder : VagrantInvocationBuilderBase<PluginInstallInvocationBuilder>
{
    public PluginInstallInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("plugin","install"));
    public PluginInstallInvocationBuilder Name(string name) => Param("name", name);
    public PluginInstallInvocationBuilder Version(string version) => Option("plugin-version", version);
    public PluginInstallInvocationBuilder Local() => Flag("local");
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (!_parameters.TryGetValue("name", out var nvals) || nvals.Length == 0 || nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
        if (_options.TryGetValue("plugin-version", out var pv) && string.IsNullOrWhiteSpace(pv))
            failures.Failure("Options", new InvalidDataException("--plugin-version cannot be empty"));
    }
}

public sealed class PluginUninstallInvocationBuilder : VagrantInvocationBuilderBase<PluginUninstallInvocationBuilder>
{
    public PluginUninstallInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("plugin","uninstall"));
    public PluginUninstallInvocationBuilder Name(string name) => Param("name", name);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        if (!_parameters.TryGetValue("name", out var nvals) || nvals.Length == 0 || nvals.Any(string.IsNullOrWhiteSpace))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
    }
}

public sealed class PluginUpdateInvocationBuilder : VagrantInvocationBuilderBase<PluginUpdateInvocationBuilder>
{
    public PluginUpdateInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("plugin","update"));
    public PluginUpdateInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class PluginListInvocationBuilder : VagrantInvocationBuilderBase<PluginListInvocationBuilder>
{
    public PluginListInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("plugin","list"));
}

public sealed class PluginExpungeInvocationBuilder : VagrantInvocationBuilderBase<PluginExpungeInvocationBuilder>
{
    public PluginExpungeInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("plugin","expunge"));
    public PluginExpungeInvocationBuilder Force() => Flag("force");
    public PluginExpungeInvocationBuilder Reinstall() => Flag("reinstall");
}

public sealed class LoginInvocationBuilder : VagrantInvocationBuilderBase<LoginInvocationBuilder>
{
    public LoginInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("login"));
}

public sealed class LogoutInvocationBuilder : VagrantInvocationBuilderBase<LogoutInvocationBuilder>
{
    public LogoutInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("logout"));
}

public sealed class RSyncInvocationBuilder : VagrantInvocationBuilderBase<RSyncInvocationBuilder>
{
    public RSyncInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("rsync"));
    public RSyncInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class RSyncAutoInvocationBuilder : VagrantInvocationBuilderBase<RSyncAutoInvocationBuilder>
{
    public RSyncAutoInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("rsync-auto"));
    public RSyncAutoInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class BoxUpdateInvocationBuilder : VagrantInvocationBuilderBase<BoxUpdateInvocationBuilder>
{
    public BoxUpdateInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeafPath("box","update"));
    public BoxUpdateInvocationBuilder Box(string name) => Option("box", name);
    public BoxUpdateInvocationBuilder Provider(string provider) => Option("provider", provider);
    protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        base.ValidateInternal(visited, failures);
        if (failures.Count > 0) return;
        foreach (var opt in new[]{"box","provider"})
            if (_options.TryGetValue(opt, out var v) && string.IsNullOrWhiteSpace(v))
                failures.Failure("Options", new InvalidDataException($"--{opt} cannot be empty"));
    }
}

internal static class CommandTreeExtensions
{
    public static LeafCommandNode FindLeaf(this CommandGroupNode root, string leafName)
    {
        if (root.Children.TryGetValue(leafName, out var child) && child is LeafCommandNode l) return l;
        foreach (var c in root.Children.Values)
        {
            if (c is CommandGroupNode g && g.Children.TryGetValue(leafName, out var inside) && inside is LeafCommandNode il) return il;
        }
        throw new InvalidOperationException($"Leaf '{leafName}' not found");
    }

    public static LeafCommandNode FindLeafPath(this CommandGroupNode root, params string[] path)
    {
        if (path.Length == 0) throw new ArgumentException("Path empty", nameof(path));
        ICommandNode current = root;
        for (int i = 0; i < path.Length; i++)
        {
            var seg = path[i];
            if (current is not CommandGroupNode grp)
                throw new InvalidOperationException($"Segment '{seg}' expects a group node in path '{string.Join(" ", path)}'.");
            if (!grp.Children.TryGetValue(seg, out var next))
                throw new InvalidOperationException($"Segment '{seg}' not found in path '{string.Join(" ", path)}'.");
            current = next;
        }
        if (current is LeafCommandNode leaf) return leaf;
        throw new InvalidOperationException($"Path '{string.Join(" ", path)}' does not end with a leaf command.");
    }
}
