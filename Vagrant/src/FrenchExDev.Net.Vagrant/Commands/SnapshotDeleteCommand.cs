namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SnapshotDeleteCommand : SnapshotCommand
{
    public string? Name { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "snapshot", "delete" };
        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name!);
        return args;
    }
}
