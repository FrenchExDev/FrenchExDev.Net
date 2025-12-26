namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud provider upload command.
/// </summary>
public sealed class VagrantCloudProviderUploadCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The provider name.</summary>
    public required string Provider { get; init; }

    /// <summary>The version of the box.</summary>
    public required string Version { get; init; }

    /// <summary>Path to the box file to upload.</summary>
    public required string BoxFile { get; init; }

    /// <summary>Use direct upload instead of the default method.</summary>
    public bool Direct { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "provider", "upload" };
        if (Direct)
            args.Add("--direct");
        args.Add(Box);
        args.Add(Provider);
        args.Add(Version);
        args.Add(BoxFile);
        return args;
    }
}
