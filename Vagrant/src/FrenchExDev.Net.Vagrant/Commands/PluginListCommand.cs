namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PluginListCommand : PluginCommand
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "plugin", "list" };
}
