namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxRemoveCommand : BoxCommand
{
    public string? Name { get; init; }
    public string? Provider { get; init; }
    public bool? All { get; internal set; }
    public bool? Force { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "remove" };
        if (!string.IsNullOrWhiteSpace(Provider)) { args.Add("--provider"); args.Add(Provider!); }
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name!);
        if (All is true) args.Add("--all");
        if (Force is true) args.Add("--force");
        return args;
    }
}
