#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

public record AlpineVersionArchFlavorRecord(
    string Version,
    string Architecture,
    string Flavor,
    string Url,
    string Sha256,
    string Sha512)
{
    public bool VersionContainsDots()
    {
        return Version.Contains(".");
    }
}