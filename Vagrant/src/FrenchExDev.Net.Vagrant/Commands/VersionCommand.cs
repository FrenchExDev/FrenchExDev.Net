namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record VersionCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "version" };
        return args;
    }
}
