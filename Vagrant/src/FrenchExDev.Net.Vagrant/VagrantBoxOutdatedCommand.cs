namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant box outdated command.
/// </summary>
public sealed class VagrantBoxOutdatedCommand : VagrantBoxCommand
{
    /// <summary>Force check for latest box version.</summary>
    public bool Force { get; init; }

    /// <summary>Check all boxes installed.</summary>
    public bool Global { get; init; }

    /// <summary>Enable machine readable output.</summary>
    public bool MachineReadable { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "outdated" };
        if (Force)
            args.Add("--force");
        if (Global)
            args.Add("--global");
        if (MachineReadable)
            args.Add("--machine-readable");
        return args;
    }
}
