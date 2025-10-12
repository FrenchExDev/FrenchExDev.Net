using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record HelpCommand : VagrantCommandBase
{
    public string? Topic { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "--help" };
        if (!string.IsNullOrWhiteSpace(Topic)) args.Add(Topic!);
        return args;
    }
}
