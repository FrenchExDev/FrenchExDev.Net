namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantPluginLicenseCommand : VagrantPluginCommand
{
    /// <summary>The name of the plugin.</summary>
    public required string Name { get; init; }

    /// <summary>The path to the license file.</summary>
    public required string LicenseFile { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        return ["plugin", "license", Name, LicenseFile];
    }
}
