namespace FrenchexDev.VirtualBox.Net;

/// <summary>
/// Represents a version identifier for VirtualBox, including major, minor, patch, and release number components.
/// </summary>
/// <param name="Major">The major version number of VirtualBox. Typically indicates significant changes or milestones.</param>
/// <param name="Minor">The minor version number of VirtualBox. Used for incremental feature updates.</param>
/// <param name="Patch">The patch version number of VirtualBox. Specifies maintenance or bug fix releases.</param>
/// <param name="ReleaseNumber">The release number for the VirtualBox version. Distinguishes builds or revisions within the same version.</param>
public record VirtualBoxVersionRecord(string Major, string Minor, string Patch, string ReleaseNumber)
{
    /// <summary>
    /// Returns a string representation of the version in the format 'Major.Minor.Patch', excluding any release or
    /// pre-release information.
    /// </summary>
    /// <returns>A string containing the major, minor, and patch components of the version, separated by periods.</returns>
    public string ToStringWithoutRelease()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}