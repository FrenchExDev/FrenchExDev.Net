namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxListCommand : BoxCommand
{
    public bool? Local { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "list" };
        if (Local == true) args.Add("--local");
        return args;
    }
}
