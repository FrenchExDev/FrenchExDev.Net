using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxPruneCommand : BoxCommand
{
    public bool? DryRun { get; internal set; }
    public bool? KeepActiveProvider { get; internal set; }
    public bool? Force { get; internal set; }

    public override IReadOnlyList<string> ToArguments() => new List<string> { "box", "prune" };
}
