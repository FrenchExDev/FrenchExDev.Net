namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantSnapshotDeleteCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to delete.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "delete" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}
