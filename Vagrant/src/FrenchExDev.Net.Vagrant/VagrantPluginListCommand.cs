namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant plugin list command.
/// </summary>
public sealed class VagrantPluginListCommand : VagrantPluginCommand
{
    /// <summary>List plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "list" };
        if (Local)
            args.Add("--local");
        return args;
    }
}
