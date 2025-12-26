namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant plugin repair command.
/// </summary>
public sealed class VagrantPluginRepairCommand : VagrantPluginCommand
{
    /// <summary>Repair plugins in local project only.</summary>
    public bool Local { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugin", "repair" };
        if (Local)
            args.Add("--local");
        return args;
    }
}
