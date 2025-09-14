#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Specifies options for controlling master connection behavior in SSH operations.
/// </summary>
/// <remarks>Use this enumeration to select how SSH master connections are managed, such as enabling, disabling,
/// or prompting for master connection usage. The available values correspond to common SSH control master modes,
/// allowing configuration of automatic or interactive connection handling.</remarks>
public enum ControlMasterEnum
{
    Yes,
    No,
    Ask,
    Auto,
    AutoAsk
}