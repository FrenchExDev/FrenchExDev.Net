#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides constant string values representing common SSH configuration host options.
/// </summary>
/// <remarks>Use these constants when working with SSH configuration files or APIs that expect the string values
/// "yes" or "no" for host-related options. This helps ensure consistency and reduces the risk of typographical
/// errors.</remarks>
public static class SshConfigHostStrings
{
    public const string YesString = "yes";
    public const string NoString = "no";
}