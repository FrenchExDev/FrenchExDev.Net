namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud version update command.
/// </summary>
public sealed class VagrantCloudVersionUpdateCommand : VagrantCloudCommand
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
        var args = new List<string> { "cloud", "version", "update" };
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
