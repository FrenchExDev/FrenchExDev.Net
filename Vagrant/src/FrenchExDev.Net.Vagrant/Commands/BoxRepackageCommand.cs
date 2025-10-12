using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxRepackageCommand : BoxCommand
{
    public string? Name { get; init; }
    public string? Provider { get; internal set; }
    public string? BoxVersion { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "repackage" };
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name!);
        return args;
    }
}
