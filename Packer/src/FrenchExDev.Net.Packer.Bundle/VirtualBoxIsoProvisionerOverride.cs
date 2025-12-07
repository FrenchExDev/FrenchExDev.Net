#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Represents an override for the ISO provisioner in VirtualBox, allowing customization of the command executed during
/// provisioning.
/// </summary>
public class VirtualBoxIsoProvisionerOverride
{
    /// <summary>
    /// Initializes a new instance of the VirtualBoxIsoProvisionerOverride class with the specified command to execute
    /// during provisioning.
    /// </summary>
    /// <param name="executeCommand">The command to execute as part of the ISO provisioning process. Can be null to indicate that no command should
    /// be executed.</param>
    public VirtualBoxIsoProvisionerOverride(string? executeCommand)
    {
        ExecuteCommand = executeCommand;
    }

    /// <summary>
    /// Gets or sets the shell command to execute as part of the operation.
    /// </summary>
    [JsonPropertyName("execute_command")] public string? ExecuteCommand { get; set; }
}