namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud auth whoami command.
/// </summary>
public sealed class VagrantCloudAuthWhoamiCommand : VagrantCloudCommand
{
    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["cloud", "auth", "whoami"];
    }
}
