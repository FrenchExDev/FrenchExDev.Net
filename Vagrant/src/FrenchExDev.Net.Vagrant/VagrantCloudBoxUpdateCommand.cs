namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud box update command.
/// </summary>
public sealed class VagrantCloudBoxUpdateCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Name { get; init; }

    /// <summary>Short description of the box.</summary>
    public string? Short { get; init; }

    /// <summary>Full description of the box (Markdown supported).</summary>
    public string? Description { get; init; }

    /// <summary>Make the box private.</summary>
    public bool Private { get; init; }

    /// <summary>Make the box public.</summary>
    public bool NoPrivate { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "box", "update" };
        if (!string.IsNullOrWhiteSpace(Short))
        {
            args.Add("--short-description");
            args.Add(Short);
        }
        if (!string.IsNullOrWhiteSpace(Description))
        {
            args.Add("--description");
            args.Add(Description);
        }
        if (Private)
            args.Add("--private");
        if (NoPrivate)
            args.Add("--no-private");
        args.Add(Name);
        return args;
    }
}
