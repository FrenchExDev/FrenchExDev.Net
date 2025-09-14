#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PostProcessor
{
    [JsonIgnore] public string? Name { get; set; }
    [JsonPropertyName("type")] public required string? Type { get; set; }

    [JsonPropertyName("compression_level")]
    public required int? CompressionLevel { get; set; }

    [JsonPropertyName("keep_input_artifact")]
    public required bool KeepInputArtifact { get; set; }

    [JsonPropertyName("output")] public required string? Output { get; set; }

    [JsonPropertyName("vagrantfile_template")]
    public required string? VagrantfileTemplate { get; set; }
}