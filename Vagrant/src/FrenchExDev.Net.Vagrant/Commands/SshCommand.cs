namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SshCommand : VagrantCommandBase
{
    public string? Command { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "ssh" };
        if (!string.IsNullOrWhiteSpace(Command)) { args.Add("-c"); args.Add(Command!); }
        return args;
    }
}
