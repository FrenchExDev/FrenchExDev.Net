namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record ShareCommand : VagrantCommandBase
{
    public string? Service { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "share" };
        if (!string.IsNullOrWhiteSpace(Service)) args.Add(Service!);
        return args;
    }
}
