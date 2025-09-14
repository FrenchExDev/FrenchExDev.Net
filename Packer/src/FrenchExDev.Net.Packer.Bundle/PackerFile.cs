#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PackerFile
{
    [JsonPropertyName("builders")] public required List<PackerBuilder>? Builders { get; set; }

    [JsonPropertyName("description")]
    [MaxLength(255)]
    public required string? Description { get; set; }

    [JsonPropertyName("provisioners")] public required List<Provisioner>? Provisioners { get; set; }

    [JsonPropertyName("post-processors")] public required List<PostProcessor>? PostProcessors { get; set; }

    [JsonPropertyName("variables")] public required Dictionary<string, string>? Variables { get; set; }
}