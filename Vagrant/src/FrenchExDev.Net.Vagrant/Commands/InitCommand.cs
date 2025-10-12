using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record InitCommand : VagrantCommandBase
{
    public string? NameOrUrl { get; init; }
    public string? Output { get; internal set; }
    public bool? Minimal { get; internal set; }
    public bool? Force { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "init" };
        if (!string.IsNullOrWhiteSpace(NameOrUrl)) args.Add(NameOrUrl!);
        return args;
    }
}
