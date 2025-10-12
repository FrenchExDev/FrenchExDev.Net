namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SuspendCommand : VagrantCommandBase
{
    public required string? Name { get; init; }
    public override IReadOnlyList<string> ToArguments() => new List<string> { "suspend" };
}
