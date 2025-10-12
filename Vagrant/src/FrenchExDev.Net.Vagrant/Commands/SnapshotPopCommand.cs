namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SnapshotPopCommand : SnapshotCommand
{
    public override IReadOnlyList<string> ToArguments() => ["snapshot", "pop"];
}