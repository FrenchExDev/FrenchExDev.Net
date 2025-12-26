namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant box remove command.
/// </summary>
public sealed class VagrantBoxRemoveCommand : VagrantBoxCommand
{
    /// <summary>The name of the box to remove.</summary>
    public required string Name { get; init; }

    /// <summary>Remove all available versions of the box.</summary>
    public bool All { get; init; }

    /// <summary>The specific version of the box to remove.</summary>
    public string? BoxVersion { get; init; }

    /// <summary>Destroy without confirmation.</summary>
    public bool Force { get; init; }

    /// <summary>The specific provider of the box to remove.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "remove" };
        if (All)
            args.Add("--all");
        if (!string.IsNullOrWhiteSpace(BoxVersion))
        {
            args.Add("--box-version");
            args.Add(BoxVersion);
        }
        if (Force)
            args.Add("--force");
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        args.Add(Name);
        return args;
    }
}
