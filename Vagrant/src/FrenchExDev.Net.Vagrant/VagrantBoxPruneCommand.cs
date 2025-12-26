namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant box prune command.
/// </summary>
public sealed class VagrantBoxPruneCommand : VagrantBoxCommand
{
    /// <summary>Only print the boxes that would be removed.</summary>
    public bool DryRun { get; init; }

    /// <summary>Destroy without confirmation.</summary>
    public bool Force { get; init; }

    /// <summary>Keep boxes still actively in use.</summary>
    public bool KeepActiveBoxes { get; init; }

    /// <summary>The specific box name to prune.</summary>
    public string? Name { get; init; }

    /// <summary>The specific provider to prune.</summary>
    public string? Provider { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "prune" };
        if (DryRun)
            args.Add("--dry-run");
        if (Force)
            args.Add("--force");
        if (KeepActiveBoxes)
            args.Add("--keep-active-boxes");
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
        return args;
    }
}
