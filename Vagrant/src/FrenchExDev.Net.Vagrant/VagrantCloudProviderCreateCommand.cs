namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud provider create command.
/// </summary>
public sealed class VagrantCloudProviderCreateCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The provider name (e.g., virtualbox, vmware_desktop).</summary>
    public required string Provider { get; init; }

    /// <summary>The version of the box.</summary>
    public required string Version { get; init; }

    /// <summary>Checksum of the box file.</summary>
    public string? Checksum { get; init; }

    /// <summary>Checksum type (e.g., sha256).</summary>
    public string? ChecksumType { get; init; }

    /// <summary>URL to the box file for external hosting.</summary>
    public string? Url { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "provider", "create" };
        if (!string.IsNullOrWhiteSpace(Checksum))
        {
            args.Add("--checksum");
            args.Add(Checksum);
        }
        if (!string.IsNullOrWhiteSpace(ChecksumType))
        {
            args.Add("--checksum-type");
            args.Add(ChecksumType);
        }
        if (!string.IsNullOrWhiteSpace(Url))
        {
            args.Add("--url");
            args.Add(Url);
        }
        args.Add(Box);
        args.Add(Provider);
        args.Add(Version);
        return args;
    }
}
