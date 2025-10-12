namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record ReloadCommand : VagrantCommandBase
{
    public bool? Provision { get; init; }
    public bool? NoProvision { get; init; }
    public string? Provider { get; init; }
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "reload" };
        if (!string.IsNullOrWhiteSpace(Provider)) { args.Add("--provider"); args.Add(Provider!); }
        if (Provision == true) args.Add("--provision");
        if (NoProvision == true) args.Add("--no-provision");
        return args;
    }
}
