namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxOutdatedCommand : BoxCommand
{
    public string? Provider { get; internal set; }
    public bool? Global { get; internal set; }
    public bool? Insecure { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var sb = new List<string> { "box", "outdated" };
        if (!string.IsNullOrWhiteSpace(Provider)) { sb.Add("--provider"); sb.Add(Provider!); }
        if (Global is true) sb.Add("--global");
        if (Insecure is true) sb.Add("--insecure");
        return sb;
    }
}
