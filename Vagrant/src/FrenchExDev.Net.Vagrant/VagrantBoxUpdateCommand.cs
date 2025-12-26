namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant box update command.
/// </summary>
public sealed class VagrantBoxUpdateCommand : VagrantBoxCommand
{
    /// <summary>Update a specific box.</summary>
    public string? Box { get; init; }

    /// <summary>CA certificate for SSL download.</summary>
    public string? CaCert { get; init; }

    /// <summary>CA certificate directory for SSL download.</summary>
    public string? CaPath { get; init; }

    /// <summary>Client certificate for SSL download.</summary>
    public string? Cert { get; init; }

    /// <summary>Force check for latest box version.</summary>
    public bool Force { get; init; }

    /// <summary>Do not validate SSL certificates.</summary>
    public bool Insecure { get; init; }

    /// <summary>Update box for a specific provider.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "update" };
        if (!string.IsNullOrWhiteSpace(Box))
        {
            args.Add("--box");
            args.Add(Box);
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
        if (Force)
            args.Add("--force");
        if (Insecure)
            args.Add("--insecure");
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        return args;
    }
}
