namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant snapshot restore command.
/// </summary>
public sealed class VagrantSnapshotRestoreCommand : VagrantSnapshotCommand
{
    /// <summary>The name of the snapshot to restore.</summary>
    public required string Name { get; init; }

    /// <summary>Optional VM name or ID.</summary>
    public string? VmName { get; init; }

    /// <summary>Disable provisioning.</summary>
    public bool NoProvision { get; init; }

    /// <summary>Do not start the VM after restore.</summary>
    public bool NoStart { get; init; }

    /// <summary>Force provisioners to run.</summary>
    public bool Provision { get; init; }

    /// <summary>Run only the given provisioners.</summary>
    public IReadOnlyList<string>? ProvisionWith { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "restore" };
        if (NoProvision)
            args.Add("--no-provision");
        if (NoStart)
            args.Add("--no-start");
        if (Provision)
            args.Add("--provision");
        if (ProvisionWith is { Count: > 0 })
        {
            args.Add("--provision-with");
            args.Add(string.Join(",", ProvisionWith));
        }
        if (!string.IsNullOrWhiteSpace(VmName))
            args.Add(VmName);
        args.Add(Name);
        return args;
    }
}
