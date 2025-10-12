namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PluginExpungeCommand : PluginCommand
{
    public bool? Force { get; init; }
    public bool? Reinstall { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "expunge" };
        if (Force == true) args.Add("--force");
        if (Reinstall == true) args.Add("--reinstall");
        return args;
    }
}
