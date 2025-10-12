namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PluginInstallCommand : PluginCommand
{
    public string? Name { get; init; }
    public bool? Force { get; init; }

    public string? PluginVersion { get; init; }
    public bool? Local { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "install" };
        if (Force == true) args.Add("--force");
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name);
        if (!string.IsNullOrWhiteSpace(PluginVersion)) { args.Add("--plugin-version"); args.Add(PluginVersion!); }
        return args;
    }
}
