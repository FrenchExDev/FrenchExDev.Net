#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Extension builder for converting a string to an AlpineVersion instance.
/// </summary>
public static class AlpineVersionBuilder
{
    /// <summary>
    /// Converts a version string to an <see cref="AlpineVersion"/> instance.
    /// </summary>
    /// <param name="versionString">Version string to convert.</param>
    /// <returns>Parsed <see cref="AlpineVersion"/>.</returns>
    public static AlpineVersion AsAlpineVersion(this string versionString)
    {
        return AlpineVersion.From(versionString);
    }
}