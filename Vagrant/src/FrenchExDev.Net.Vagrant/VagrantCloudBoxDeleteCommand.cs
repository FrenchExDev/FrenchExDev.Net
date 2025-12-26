namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud box delete command.
/// </summary>
public sealed class VagrantCloudBoxDeleteCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Name { get; init; }

    /// <summary>Force deletion without confirmation.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "box", "delete" };
        if (Force)
            args.Add("--force");
        args.Add(Name);
        return args;
    }
}
