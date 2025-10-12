using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record GlobalStatusCommand : VagrantCommandBase
{
    public bool? Prune { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "global-status" };
        if (Prune == true) args.Add("--prune");
        return args;
    }
}
