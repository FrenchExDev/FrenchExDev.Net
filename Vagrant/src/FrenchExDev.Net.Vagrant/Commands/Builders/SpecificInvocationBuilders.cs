using FrenchExDev.Net.Vagrant.Commands.Invocation;

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
}

public sealed class BoxAddInvocationBuilder : VagrantInvocationBuilderBase<BoxAddInvocationBuilder>
{
    public BoxAddInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("add")); return this; }
    public BoxAddInvocationBuilder Source(string src) => Param("source", src);
    public BoxAddInvocationBuilder Name(string name) => Option("name", name);
    public BoxAddInvocationBuilder Checksum(string checksum) => Option("checksum", checksum);
    public BoxAddInvocationBuilder ChecksumType(string type) => Option("checksum-type", type);
    public BoxAddInvocationBuilder BoxVersion(string v) => Option("box-version", v);
    public BoxAddInvocationBuilder Force() => Flag("force");
    public BoxAddInvocationBuilder Clean() => Flag("clean");
}

public sealed class BoxRemoveInvocationBuilder : VagrantInvocationBuilderBase<BoxRemoveInvocationBuilder>
{
    public BoxRemoveInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("remove")); return this; }
    public BoxRemoveInvocationBuilder Name(string n) => Param("name", n);
    public BoxRemoveInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxRemoveInvocationBuilder BoxVersion(string v) => Option("box-version", v);
    public BoxRemoveInvocationBuilder All() => Flag("all");
    public BoxRemoveInvocationBuilder Force() => Flag("force");
}

public sealed class BoxListInvocationBuilder : VagrantInvocationBuilderBase<BoxListInvocationBuilder>
{
    public BoxListInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("list")); return this; }
    public BoxListInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxListInvocationBuilder Force() => Flag("force");
}

public sealed class BoxOutdatedInvocationBuilder : VagrantInvocationBuilderBase<BoxOutdatedInvocationBuilder>
{
    public BoxOutdatedInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("outdated")); return this; }
    public BoxOutdatedInvocationBuilder Provider(string provider) => Option("provider", provider);
    public BoxOutdatedInvocationBuilder Global() => Flag("global");
    public BoxOutdatedInvocationBuilder Insecure() => Flag("insecure");
}

public sealed class BoxPruneInvocationBuilder : VagrantInvocationBuilderBase<BoxPruneInvocationBuilder>
{
    public BoxPruneInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("prune")); return this; }
    public BoxPruneInvocationBuilder DryRun() => Flag("dry-run");
    public BoxPruneInvocationBuilder KeepActiveProvider() => Flag("keep-active-provider");
    public BoxPruneInvocationBuilder Force() => Flag("force");
}

public sealed class BoxRepackageInvocationBuilder : VagrantInvocationBuilderBase<BoxRepackageInvocationBuilder>
{
    public BoxRepackageInvocationBuilder UseDefaultTree() { var tree = VagrantCommandTree.Build(); Command(tree.FindLeaf("repackage")); return this; }
    public BoxRepackageInvocationBuilder Name(string name) => Param("name", name);
    public BoxRepackageInvocationBuilder ProviderParam(string provider) => Param("provider", provider);
    public BoxRepackageInvocationBuilder Version(string version) => Param("version", version);
    public BoxRepackageInvocationBuilder Output(string output) => Option("output", output);
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
    public SshInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("ssh"));
    public SshInvocationBuilder Machine(string value) => Param("machine", value);
    public SshInvocationBuilder CommandParam(string value) => Param("command", value);
    public SshInvocationBuilder CommandOption(string value) => Option("command", value);
    public SshInvocationBuilder Plain() => Flag("plain");
}

public sealed class SshConfigInvocationBuilder : VagrantInvocationBuilderBase<SshConfigInvocationBuilder>
{
    public SshConfigInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("ssh-config"));
    public SshConfigInvocationBuilder Machine(string value) => Param("machine", value);
    public SshConfigInvocationBuilder Host(string value) => Option("host", value);
}

public sealed class ProvisionInvocationBuilder : VagrantInvocationBuilderBase<ProvisionInvocationBuilder>
{
    public ProvisionInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("provision"));
    public ProvisionInvocationBuilder Machine(string value) => Param("machine", value);
    public ProvisionInvocationBuilder ProvisionWith(string namesCsv) => Option("provision-with", namesCsv);
}

public sealed class ReloadInvocationBuilder : VagrantInvocationBuilderBase<ReloadInvocationBuilder>
{
    public ReloadInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("reload"));
    public ReloadInvocationBuilder Machine(params string[] names) => Param("machine", names);
    public ReloadInvocationBuilder Provider(string provider) => Option("provider", provider);
    public ReloadInvocationBuilder Provision() => Flag("provision");
    public ReloadInvocationBuilder NoProvision() => Flag("no-provision");
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
    public SnapshotSaveInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("save"));
    public SnapshotSaveInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotSaveInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class SnapshotRestoreInvocationBuilder : VagrantInvocationBuilderBase<SnapshotRestoreInvocationBuilder>
{
    public SnapshotRestoreInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("restore"));
    public SnapshotRestoreInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotRestoreInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class SnapshotListInvocationBuilder : VagrantInvocationBuilderBase<SnapshotListInvocationBuilder>
{
    public SnapshotListInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("list"));
    public SnapshotListInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SnapshotDeleteInvocationBuilder : VagrantInvocationBuilderBase<SnapshotDeleteInvocationBuilder>
{
    public SnapshotDeleteInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("delete"));
    public SnapshotDeleteInvocationBuilder Machine(string name) => Param("machine", name);
    public SnapshotDeleteInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class SnapshotPushInvocationBuilder : VagrantInvocationBuilderBase<SnapshotPushInvocationBuilder>
{
    public SnapshotPushInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("push"));
    public SnapshotPushInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class SnapshotPopInvocationBuilder : VagrantInvocationBuilderBase<SnapshotPopInvocationBuilder>
{
    public SnapshotPopInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("pop"));
    public SnapshotPopInvocationBuilder Machine(string name) => Param("machine", name);
}

public sealed class PluginInstallInvocationBuilder : VagrantInvocationBuilderBase<PluginInstallInvocationBuilder>
{
    public PluginInstallInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("install"));
    public PluginInstallInvocationBuilder Name(string name) => Param("name", name);
    public PluginInstallInvocationBuilder Version(string version) => Option("plugin-version", version);
    public PluginInstallInvocationBuilder Local() => Flag("local");
}

public sealed class PluginUninstallInvocationBuilder : VagrantInvocationBuilderBase<PluginUninstallInvocationBuilder>
{
    public PluginUninstallInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("uninstall"));
    public PluginUninstallInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class PluginUpdateInvocationBuilder : VagrantInvocationBuilderBase<PluginUpdateInvocationBuilder>
{
    public PluginUpdateInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("update"));
    public PluginUpdateInvocationBuilder Name(string name) => Param("name", name);
}

public sealed class PluginListInvocationBuilder : VagrantInvocationBuilderBase<PluginListInvocationBuilder>
{
    public PluginListInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("list"));
}

public sealed class PluginExpungeInvocationBuilder : VagrantInvocationBuilderBase<PluginExpungeInvocationBuilder>
{
    public PluginExpungeInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("expunge"));
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
    public BoxUpdateInvocationBuilder UseDefaultTree() => Command(VagrantCommandTree.Build().FindLeaf("update"));
    public BoxUpdateInvocationBuilder Box(string name) => Option("box", name);
    public BoxUpdateInvocationBuilder Provider(string provider) => Option("provider", provider);
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
}
