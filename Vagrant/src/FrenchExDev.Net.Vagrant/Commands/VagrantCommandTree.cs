using System.Linq;

namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Provides a composed in-memory representation of Vagrant's command hierarchy.
/// This focuses on structure & option inheritance, not execution.
/// </summary>
public static class VagrantCommandTree
{
    public static CommandGroupNode Build()
    {
        // Root implicit 'vagrant'
        var root = new CommandGroupNode("vagrant", "Vagrant CLI root")
            .WithOption(new CommandOption("color", "Enable/disable color", true, ValueOptional: true, ValueName: "auto|true|false"))
            .WithOption(new CommandOption("machine-readable", "Output machine-readable logs"))
            .WithOption(new CommandOption("debug", "Enable debug output"))
            .WithOption(new CommandOption("version", "Show version"))
            .WithOption(new CommandOption("help", "Show help", Aliases: new[] { "h" }));

        // Local inline builder pattern used directly in each ToCommandLine call
        static string BuildInline(LeafCommandNode l)
        {
            return l.BuildCommandLine();
        }

        // box group
        var box = root.AddChild(new CommandGroupNode("box", "Manage boxes", root)
            .WithOption(new CommandOption("provider", "Limit to a provider", true, ValueName: "name"))
            .WithOption(new CommandOption("force", "Force some box operations"))
        );

        box.AddChild(new LeafCommandNode("add", "Add a box", box)
            .WithParameter(new CommandParameter("source", "Box source (name, URL, or path)", Required: true))
            .WithOption(new CommandOption("clean", "Clean temp files after add"))
            .WithOption(new CommandOption("checksum", "Checksum for downloaded box", true, ValueName: "value"))
            .WithOption(new CommandOption("checksum-type", "Type of checksum", true, ValueName: "md5|sha1|sha256"))
            .WithOption(new CommandOption("box-version", "Constrain version", true, ValueName: "semver"))
            .WithOption(new CommandOption("name", "Override box name", true, ValueName: "name"))
            .WithOption(new CommandOption("force", "Override existing box"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("remove", "Remove a box", box)
            .WithParameter(new CommandParameter("name", "Name of the box", Required: true))
            .WithOption(new CommandOption("provider", "Provider of box", true, ValueName: "name"))
            .WithOption(new CommandOption("box-version", "Specific version", true, ValueName: "semver"))
            .WithOption(new CommandOption("all", "Remove all versions"))
            .WithOption(new CommandOption("force", "Remove even if in use"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("list", "List boxes", box)
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("outdated", "Check outdated boxes", box)
            .WithOption(new CommandOption("global", "Check all boxes"))
            .WithOption(new CommandOption("insecure", "Skip SSL verifications"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("prune", "Prune old box versions", box)
            .WithOption(new CommandOption("dry-run", "Only report would-be deletions"))
            .WithOption(new CommandOption("keep-active-provider", "Keep boxes for active provider"))
            .WithOption(new CommandOption("force", "Remove without confirmation"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("repackage", "Repackage a box", box)
            .WithParameter(new CommandParameter("name", "Box name", Required: true))
            .WithParameter(new CommandParameter("provider", "Provider name", Required: true))
            .WithParameter(new CommandParameter("version", "Box version", Required: true))
            .WithOption(new CommandOption("output", "Output path", true, ValueName: "file"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        box.AddChild(new LeafCommandNode("update", "Update a box", box)
            .WithOption(new CommandOption("box", "Box name to update", true, ValueName: "name"))
            .WithOption(new CommandOption("provider", "Provider filter", true, ValueName: "name"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // init (leaf at root)
        root.AddChild(new LeafCommandNode("init", "Initialize a Vagrant environment", root)
            .WithParameter(new CommandParameter("box-name", "Optional box name", Required: false))
            .WithParameter(new CommandParameter("box-url", "Optional box url", Required: false))
            .WithOption(new CommandOption("box", "Base box name", true, ValueName: "name"))
            .WithOption(new CommandOption("output", "Path to Vagrantfile", true, ValueName: "file"))
            .WithOption(new CommandOption("minimal", "Generate minimal Vagrantfile"))
            .WithOption(new CommandOption("force", "Overwrite existing Vagrantfile"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // up
        root.AddChild(new LeafCommandNode("up", "Create and configure guest", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false, IsVariadic: true))
            .WithOption(new CommandOption("provider", "Specific provider", true, ValueName: "name"))
            .WithOption(new CommandOption("provision", "Enable provisioning"))
            .WithOption(new CommandOption("no-provision", "Disable provisioning"))
            .WithOption(new CommandOption("destroy-on-error", "Auto-destroy on error"))
            .WithOption(new CommandOption("parallel", "Parallel bring up"))
            .WithOption(new CommandOption("color", "Override color", true, ValueOptional: true, ValueName: "auto|true|false"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // halt
        root.AddChild(new LeafCommandNode("halt", "Halt the VM", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false, IsVariadic: true))
            .WithOption(new CommandOption("force", "Force shutdown"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // destroy
        root.AddChild(new LeafCommandNode("destroy", "Destroy the VM", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false, IsVariadic: true))
            .WithOption(new CommandOption("force", "Skip confirmation"))
            .WithOption(new CommandOption("graceful", "Graceful shutdown if possible"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // status
        root.AddChild(new LeafCommandNode("status", "Show VM status", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false, IsVariadic: true))
            .WithOption(new CommandOption("machine-readable", "Machine readable output"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // ssh
        root.AddChild(new LeafCommandNode("ssh", "SSH into machine", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .WithParameter(new CommandParameter("command", "Command to execute after login", Required: false))
            .WithOption(new CommandOption("command", "Run single command", true, ValueName: "cmd"))
            .WithOption(new CommandOption("plain", "Plain mode (no colors)"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // ssh-config
        root.AddChild(new LeafCommandNode("ssh-config", "Output OpenSSH configuration", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .WithOption(new CommandOption("host", "Override Host header name", true, ValueName: "name"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // provision
        root.AddChild(new LeafCommandNode("provision", "Run provisioners", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false))
            .WithOption(new CommandOption("provision-with", "Limit to certain provisioners", true, ValueName: "names"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // reload
        root.AddChild(new LeafCommandNode("reload", "Halt then up", root)
            .WithParameter(new CommandParameter("machine", "Specific machine(s)", Required: false, IsVariadic: true))
            .WithOption(new CommandOption("provision", "Enable provisioning"))
            .WithOption(new CommandOption("no-provision", "Disable provisioning"))
            .WithOption(new CommandOption("provider", "Specific provider", true, ValueName: "name"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // validate
        root.AddChild(new LeafCommandNode("validate", "Validate Vagrantfile", root)
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // Additional common commands
        root.AddChild(new LeafCommandNode("global-status", "List all active Vagrant environments", root)
            .WithOption(new CommandOption("prune", "Prune invalid entries"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        root.AddChild(new LeafCommandNode("package", "Package environment into a box", root)
            .WithOption(new CommandOption("output", "Output .box file", true, ValueName: "file"))
            .WithOption(new CommandOption("base", "Base VM name", true, ValueName: "name"))
            .WithOption(new CommandOption("include", "Additional files", true, ValueName: "file"))
            .WithOption(new CommandOption("vagrantfile", "Custom Vagrantfile", true, ValueName: "file"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        root.AddChild(new LeafCommandNode("port", "Display network port mappings", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        root.AddChild(new LeafCommandNode("suspend", "Suspend the machine", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );
        root.AddChild(new LeafCommandNode("resume", "Resume a suspended machine", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0))
        );

        // snapshot group
        var snapshot = root.AddChild(new CommandGroupNode("snapshot", "Snapshot management", root));
        snapshot.AddChild(new LeafCommandNode("save", "Save a snapshot", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .WithParameter(new CommandParameter("name", "Snapshot name", Required: true))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        snapshot.AddChild(new LeafCommandNode("restore", "Restore snapshot", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .WithParameter(new CommandParameter("name", "Snapshot name", Required: true))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        snapshot.AddChild(new LeafCommandNode("list", "List snapshots", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        snapshot.AddChild(new LeafCommandNode("delete", "Delete snapshot", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .WithParameter(new CommandParameter("name", "Snapshot name", Required: true))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        snapshot.AddChild(new LeafCommandNode("push", "Push current state onto stack", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        snapshot.AddChild(new LeafCommandNode("pop", "Pop state from stack", snapshot)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));

        // plugin group
        var plugin = root.AddChild(new CommandGroupNode("plugin", "Plugin management", root));
        plugin.AddChild(new LeafCommandNode("install", "Install a plugin", plugin)
            .WithParameter(new CommandParameter("name", "Plugin name", Required: true))
            .WithOption(new CommandOption("plugin-version", "Specific version", true, ValueName: "semver"))
            .WithOption(new CommandOption("local", "Local file install"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        plugin.AddChild(new LeafCommandNode("uninstall", "Uninstall a plugin", plugin)
            .WithParameter(new CommandParameter("name", "Plugin name", Required: true))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        plugin.AddChild(new LeafCommandNode("update", "Update plugins", plugin)
            .WithParameter(new CommandParameter("name", "Plugin name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        plugin.AddChild(new LeafCommandNode("list", "List plugins", plugin)
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        plugin.AddChild(new LeafCommandNode("expunge", "Remove all plugins", plugin)
            .WithOption(new CommandOption("force", "Force removal"))
            .WithOption(new CommandOption("reinstall", "Reinstall after cleanup"))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));

        root.AddChild(new LeafCommandNode("login", "Log in to Vagrant Cloud", root)
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        root.AddChild(new LeafCommandNode("logout", "Log out of Vagrant Cloud", root)
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));

        root.AddChild(new LeafCommandNode("rsync", "Rsync files to guest", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));
        root.AddChild(new LeafCommandNode("rsync-auto", "Continually sync changes", root)
            .WithParameter(new CommandParameter("machine", "Machine name", Required: false))
            .ToCommandLine(BuildInline)
            .WithHandler(_ => Task.FromResult(0)));

        return root;
    }
}
