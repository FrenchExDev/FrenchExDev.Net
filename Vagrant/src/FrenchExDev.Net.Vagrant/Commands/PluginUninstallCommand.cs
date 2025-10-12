namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PluginUninstallCommand : PluginCommand
{
    public string? Name { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "uninstall" };
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name!);
        return args;
    }
}
