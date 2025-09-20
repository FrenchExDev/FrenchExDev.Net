#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Represents a configuration for provisioning resources, including the provisioner type, associated scripts, override
/// settings, and an optional pause before execution.
/// </summary>
/// <remarks>Use this class to specify the details required for a provisioning operation. The provisioner type
/// determines the provisioning strategy, while scripts provide custom actions to execute. Override settings allow
/// customization of the provisioner's behavior, and the pause option enables delaying the start of provisioning if
/// needed.</remarks>
public class Provisioner
{
    /// <summary>
    /// Initializes a new instance of the Provisioner class with the specified type, scripts, override settings, and
    /// optional pause before execution.
    /// </summary>
    /// <param name="type">The type of provisioner to use. Can be null to indicate a default or unspecified type.</param>
    /// <param name="scripts">A list of script file paths to execute during provisioning. Can be null if no scripts are required.</param>
    /// <param name="override">An optional override configuration that customizes the provisioner's behavior. Can be null if no override is
    /// needed.</param>
    /// <param name="pauseBefore">An optional duration or marker indicating a pause before provisioning begins. Can be null if no pause is
    /// required.</param>
    public Provisioner(string? type, List<string>? scripts, ProvisionerOverride? @override, string? pauseBefore)
    {
        Type = type;
        Scripts = scripts;
        Override = @override;
        PauseBefore = pauseBefore;
    }

    /// <summary>
    /// Gets or sets the type identifier for the object as represented in the JSON payload.
    /// </summary>
    /// <remarks>The value corresponds to the "type" property in the serialized JSON. This property is
    /// typically used to distinguish between different object kinds or schemas when deserializing or processing
    /// data.</remarks>
    [JsonPropertyName("type")] public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the collection of script contents associated with this object.
    /// </summary>
    /// <remarks>Each string in the collection represents the full content of a script. The order of scripts
    /// in the list may affect processing if scripts are executed sequentially.</remarks>
    [JsonPropertyName("scripts")] public List<string>? Scripts { get; set; }

    /// <summary>
    /// Gets or sets the override configuration for the provisioner.
    /// </summary>
    /// <remarks>Use this property to specify custom settings that should take precedence over the default
    /// provisioner configuration. If not set, the provisioner will use its standard configuration values.</remarks>
    [JsonPropertyName("override")] public ProvisionerOverride? Override { get; set; }

    /// <summary>
    /// Gets or sets an optional pause duration or marker before provisioning begins.
    /// </summary>
    [JsonPropertyName("pause_before")] public string? PauseBefore { get; set; }
}