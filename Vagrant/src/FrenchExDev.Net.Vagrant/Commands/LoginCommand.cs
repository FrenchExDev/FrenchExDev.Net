using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record LoginCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "login" };
}
