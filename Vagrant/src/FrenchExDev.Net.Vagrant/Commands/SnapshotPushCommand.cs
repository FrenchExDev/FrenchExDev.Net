namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SnapshotPushCommand : SnapshotCommand
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "snapshot", "push" };
}

