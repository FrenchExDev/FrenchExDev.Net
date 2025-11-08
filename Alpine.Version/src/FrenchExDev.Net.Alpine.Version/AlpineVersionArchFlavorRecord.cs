#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Represents a record containing metadata for a specific Alpine Linux release, including its version, architecture,
/// flavor, download URL, and cryptographic hashes.
/// </summary>
/// <param name="Version">The version string of the Alpine Linux release. Typically follows semantic versioning (e.g., "3.18.2").</param>
/// <param name="Architecture">The target system architecture for the release, such as "x86_64" or "arm64".</param>
/// <param name="Flavor">The distribution flavor of the release, indicating the variant or build type (e.g., "standard", "minimal").</param>
/// <param name="Url">The URL from which the Alpine Linux release can be downloaded.</param>
/// <param name="Sha256">The SHA-256 hash of the release file, used to verify its integrity.</param>
/// <param name="Sha512">The SHA-512 hash of the release file, used to verify its integrity.</param>
public record AlpineVersionArchFlavorRecord(
    string Version,
    string Architecture,
    string Flavor,
    string Url,
    string Sha256,
    string Sha512)
{
    /// <summary>
    /// Determines whether the version string contains one or more dot ('.') characters.
    /// </summary>
    /// <returns>true if the version string contains at least one dot; otherwise, false.</returns>
    public bool VersionContainsDots() => Version.Contains(".");
}