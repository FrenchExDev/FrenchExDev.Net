#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class Provisioner
{
    [JsonPropertyName("type")] public required string? Type { get; set; }
    [JsonPropertyName("scripts")] public required List<string>? Scripts { get; set; }
    [JsonPropertyName("override")] public required ProvisionerOverride? Override { get; set; }
    [JsonPropertyName("pause_before")] public required string? PauseBefore { get; set; }
}