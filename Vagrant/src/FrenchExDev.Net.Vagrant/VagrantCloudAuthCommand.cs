namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud auth subcommands (login, logout, whoami).
/// For type-safe usage, prefer <see cref="VagrantCloudAuthLoginCommand"/>, 
/// <see cref="VagrantCloudAuthLogoutCommand"/>, or <see cref="VagrantCloudAuthWhoamiCommand"/>.
/// </summary>
public sealed class VagrantCloudAuthCommand : VagrantCloudCommand
{
    /// <summary>The auth subcommand (login, logout, whoami).</summary>
    public required string SubCommand { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "auth", SubCommand];
    }
}
