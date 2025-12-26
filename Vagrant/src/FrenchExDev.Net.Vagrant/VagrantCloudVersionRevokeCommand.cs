namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud version revoke command.
/// </summary>
public sealed class VagrantCloudVersionRevokeCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The version number.</summary>
    public required string Version { get; init; }

    /// <summary>Force revocation without confirmation.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "version", "revoke" };
        if (Force)
            args.Add("--force");
        args.Add(Box);
        args.Add(Version);
        return args;
    }
}
