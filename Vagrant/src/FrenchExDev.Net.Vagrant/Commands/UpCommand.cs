namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record UpCommand : VagrantCommandBase
{
    public bool? Provision { get; init; }
    public string? Provider { get; init; }
    public bool? Parallel { get; internal set; }
    public bool? DestroyOnError { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "up" };
        if (Provision.HasValue) args.Add(Provision == true ? "--provision" : "--no-provision");
        if (!string.IsNullOrWhiteSpace(Provider)) { args.Add("--provider"); args.Add(Provider!); }
        return args;
    }
}
