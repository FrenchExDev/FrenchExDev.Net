namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud version subcommands (create, delete, release, revoke, update).
/// For type-safe usage, prefer specific command classes like <see cref="VagrantCloudVersionCreateCommand"/>.
/// </summary>
public sealed class VagrantCloudVersionCommand : VagrantCloudCommand
{
    /// <summary>The version subcommand (create, delete, release, revoke, update).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "version", SubCommand];
    }
}
