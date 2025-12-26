namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud auth logout command.
/// </summary>
public sealed class VagrantCloudAuthLogoutCommand : VagrantCloudCommand
{
    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "auth", "logout"];
    }
}
