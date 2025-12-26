namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantPluginUpdateCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin to update (optional, updates all if not specified).</summary>
    public string? Name { get; init; }

    /// <summary>Update plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "update" };
        if (Local)
            args.Add("--local");
        if (!string.IsNullOrWhiteSpace(Name))
            args.Add(Name);
        return args;
    }
}
