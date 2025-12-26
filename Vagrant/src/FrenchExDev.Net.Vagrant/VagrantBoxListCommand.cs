namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantBoxListCommand : VagrantBoxCommand
{
    /// <summary>Display additional box info.</summary>
    public bool BoxInfo { get; init; }

    /// <summary>Enable machine readable output.</summary>
    public bool MachineReadable { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "list" };
        if (BoxInfo)
            args.Add("--box-info");
        if (MachineReadable)
            args.Add("--machine-readable");
        return args;
    }
}
