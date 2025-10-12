namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SnapshotListCommand : SnapshotCommand
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "snapshot", "list" };
}
