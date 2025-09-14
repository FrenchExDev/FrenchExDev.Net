#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class VirtualBoxIsoProvisionerOverride
{
    [JsonPropertyName("execute_command")] public required string? ExecuteCommand { get; set; }
}