namespace FrenchExDev.Net.Vagrant;

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
