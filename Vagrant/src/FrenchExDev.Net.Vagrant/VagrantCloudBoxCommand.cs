namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud box subcommands (create, delete, show, update).
/// For type-safe usage, prefer specific command classes like <see cref="VagrantCloudBoxCreateCommand"/>.
/// </summary>
public sealed class VagrantCloudBoxCommand : VagrantCloudCommand
{
    /// <summary>The box subcommand (create, delete, show, update).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "box", SubCommand];
    }
}
