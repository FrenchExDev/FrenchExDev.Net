namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud provider subcommands (create, delete, update, upload).
/// For type-safe usage, prefer specific command classes like <see cref="VagrantCloudProviderCreateCommand"/>.
/// </summary>
public sealed class VagrantCloudProviderCommand : VagrantCloudCommand
{
    /// <summary>The provider subcommand (create, delete, update, upload).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "provider", SubCommand];
    }
}
