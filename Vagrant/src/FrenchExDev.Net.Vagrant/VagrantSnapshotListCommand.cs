namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantSnapshotListCommand : VagrantSnapshotCommand
{
    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "list" };
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        return args;
    }
}
