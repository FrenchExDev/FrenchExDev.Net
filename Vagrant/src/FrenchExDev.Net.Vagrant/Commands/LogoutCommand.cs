using System.Collections.Generic;

namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record LogoutCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "logout" };
}
