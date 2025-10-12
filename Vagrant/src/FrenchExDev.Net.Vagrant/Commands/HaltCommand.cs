using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record HaltCommand : VagrantCommandBase
{
    public bool? Force { get; init; }
    public string? Name { get; internal set; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "halt" };
        if (Force == true) args.Add("-f");
        return args;
    }
}
