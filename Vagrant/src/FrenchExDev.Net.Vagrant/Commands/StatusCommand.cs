namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record StatusCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "status" };
        if (MachineReadable == true) args.Add("--machine-readable");
        return args;
    }
}
