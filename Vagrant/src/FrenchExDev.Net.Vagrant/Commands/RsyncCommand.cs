namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record RsyncCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "rsync" };
}
