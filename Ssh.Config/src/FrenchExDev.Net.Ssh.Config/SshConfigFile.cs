#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Represents the contents of an SSH configuration file, including all defined host entries.
/// </summary>
/// <remarks>Use this class to access and manage the collection of SSH host configurations parsed from an SSH
/// config file. Each host entry provides connection details and options for a specific SSH target.</remarks>
public class SshConfigFile
{
    /// <summary>
    /// Gets the collection of SSH host configurations for this instance.
    /// </summary>
    /// <remarks>Each entry in the collection represents a distinct SSH host with its associated connection
    /// settings. The property is required and must be initialized during object construction.</remarks>
    public required List<SshConfigHost> Hosts { get; init; }
}