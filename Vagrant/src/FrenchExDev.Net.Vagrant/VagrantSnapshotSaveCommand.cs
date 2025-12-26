namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant snapshot save command.
/// </summary>
public sealed class VagrantSnapshotSaveCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to save.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Replace an existing snapshot with the same name.</summary>
    public bool Force { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "save" };
        if (Force)
            args.Add("--force");
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}
