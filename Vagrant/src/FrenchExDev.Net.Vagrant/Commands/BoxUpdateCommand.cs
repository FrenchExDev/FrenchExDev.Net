using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxUpdateCommand : BoxCommand
{
    public string? Name { get; init; }
    public string? Provider { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "update" };
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name!);
        return args;
    }
}
