namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant box add command.
/// </summary>
public sealed class VagrantBoxAddCommand : VagrantBoxCommand
{
    /// <summary>The name, url, or path of the box to add.</summary>
    public required string Box { get; init; }

    /// <summary>Constrain version of the added box.</summary>
    public string? BoxVersion { get; init; }

    /// <summary>CA certificate for SSL download.</summary>
    public string? CaCert { get; init; }

    /// <summary>CA certificate directory for SSL download.</summary>
    public string? CaPath { get; init; }

    /// <summary>Client certificate for SSL download.</summary>
    public string? Cert { get; init; }

    /// <summary>Checksum for the downloaded box.</summary>
    public string? Checksum { get; init; }

    /// <summary>Checksum type (e.g., md5, sha1, sha256, sha384, sha512).</summary>
    public string? ChecksumType { get; init; }

    /// <summary>Clean any temporary download files.</summary>
    public bool Clean { get; init; }

    /// <summary>Overwrite an existing box if it exists.</summary>
    public bool Force { get; init; }

    /// <summary>Do not validate SSL certificates.</summary>
    public bool Insecure { get; init; }

    /// <summary>Trust 'Location' header from HTTP redirects and use the same credentials for subsequent URLs.</summary>
    public bool LocationTrusted { get; init; }

    /// <summary>Name of the box (required for direct box file paths).</summary>
    public string? Name { get; init; }

    /// <summary>Provider for the box.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "add" };
        if (!string.IsNullOrWhiteSpace(BoxVersion))
        {
            args.Add("--box-version");
            args.Add(BoxVersion);
        }
        if (!string.IsNullOrWhiteSpace(CaCert))
        {
            args.Add("--cacert");
            args.Add(CaCert);
        }
        if (!string.IsNullOrWhiteSpace(CaPath))
        {
            args.Add("--capath");
            args.Add(CaPath);
        }
        if (!string.IsNullOrWhiteSpace(Cert))
        {
            args.Add("--cert");
            args.Add(Cert);
        }
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
        if (Clean)
            args.Add("--clean");
        if (Force)
            args.Add("--force");
        if (Insecure)
            args.Add("--insecure");
        if (LocationTrusted)
            args.Add("--location-trusted");
        if (!string.IsNullOrWhiteSpace(Name))
        {
            args.Add("--name");
            args.Add(Name);
        }
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        args.Add(Box);
        return args;
    }
}
