namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PluginUpdateCommand : PluginCommand
{
    public required string Name { get; init; }
    public override IReadOnlyList<string> ToArguments()
    {
        var sb = new List<string>
        {
            "plugin",
            "update",
            Name
        };

        return sb;
    }
}