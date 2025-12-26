namespace FrenchExDev.Net.Vagrant;

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
