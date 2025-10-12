namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record RdpCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "rdp" };
}
