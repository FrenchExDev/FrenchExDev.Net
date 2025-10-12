using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record DestroyCommand : VagrantCommandBase
{
    public bool? Force { get; init; }
    public bool? Graceful { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "destroy" };
        if (Force == true) args.Add("-f");
        if (Graceful == true) args.Add("--graceful");
        return args;
    }
}
