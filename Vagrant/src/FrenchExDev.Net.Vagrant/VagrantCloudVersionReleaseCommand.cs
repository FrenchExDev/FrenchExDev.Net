namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud version release command.
/// </summary>
public sealed class VagrantCloudVersionReleaseCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The version number.</summary>
    public required string Version { get; init; }

    /// <summary>Force release without confirmation.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "version", "release" };
        if (Force)
            args.Add("--force");
        args.Add(Box);
        args.Add(Version);
        return args;
    }
}
