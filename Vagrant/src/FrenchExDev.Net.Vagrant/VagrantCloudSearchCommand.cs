namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud search command.
/// </summary>
public sealed class VagrantCloudSearchCommand : VagrantCloudCommand
{
    /// <summary>The search query.</summary>
    public string? Query { get; init; }

    /// <summary>Format results in JSON.</summary>
    public bool Json { get; init; }

    /// <summary>Maximum number of results to display.</summary>
    public int? Limit { get; init; }

    /// <summary>Page number to display.</summary>
    public int? Page { get; init; }

    /// <summary>Filter results to a specific provider.</summary>
    public string? Provider { get; init; }

    /// <summary>Display short results (just box names).</summary>
    public bool Short { get; init; }

    /// <summary>Field to sort results by (e.g., downloads, created, updated).</summary>
    public string? SortBy { get; init; }

    /// <summary>Order of results (asc or desc).</summary>
    public string? Order { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "search" };
        if (Json)
            args.Add("--json");
        if (Limit.HasValue)
        {
            args.Add("--limit");
            args.Add(Limit.Value.ToString());
        }
        if (Page.HasValue)
        {
            args.Add("--page");
            args.Add(Page.Value.ToString());
        }
        if (!string.IsNullOrWhiteSpace(Provider))
        {
            args.Add("--provider");
            args.Add(Provider);
        }
        if (Short)
            args.Add("--short");
        if (!string.IsNullOrWhiteSpace(SortBy))
        {
            args.Add("--sort-by");
            args.Add(SortBy);
        }
        if (!string.IsNullOrWhiteSpace(Order))
        {
            args.Add("--order");
            args.Add(Order);
        }
        if (!string.IsNullOrWhiteSpace(Query))
            args.Add(Query);
        return args;
    }
}
