namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud provider delete command.
/// </summary>
public sealed class VagrantCloudProviderDeleteCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The provider name.</summary>
    public required string Provider { get; init; }

    /// <summary>The version of the box.</summary>
    public required string Version { get; init; }

    /// <summary>Force deletion without confirmation.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "provider", "delete" };
        if (Force)
            args.Add("--force");
        args.Add(Box);
        args.Add(Provider);
        args.Add(Version);
        return args;
    }
}
