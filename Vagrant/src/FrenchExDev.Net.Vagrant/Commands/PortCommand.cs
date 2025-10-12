namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PortCommand : VagrantCommandBase
{
    public bool? Private { get; init; }
    public bool? Public { get; init; }
    public string? Machine { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "port" };
        if (Private == true) args.Add("--private");
        if (Public == true) args.Add("--public");
        if (!string.IsNullOrWhiteSpace(Machine)) { args.Add("--machine"); args.Add(Machine!); }
        return args;
    }
}
