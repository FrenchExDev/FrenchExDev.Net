namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud version create command.
/// </summary>
public sealed class VagrantCloudVersionCreateCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Box { get; init; }

    /// <summary>The version number.</summary>
    public required string Version { get; init; }

    /// <summary>Description of this version.</summary>
    public string? Description { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "version", "create" };
        if (!string.IsNullOrWhiteSpace(Description))
        {
            args.Add("--description");
            args.Add(Description);
        }
        args.Add(Box);
        args.Add(Version);
        return args;
    }
}
