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
    /// <summary>
    /// Initializes a new instance of the PackerFile class with the specified builders, description, provisioners,
    /// post-processors, and variables.
    /// </summary>
    /// <param name="builders">A list of builder configurations that define how images are created. Can be null if no builders are specified.</param>
    /// <param name="description">An optional description of the Packer file. Can be null if no description is provided.</param>
    /// <param name="provisioners">A list of provisioner configurations that specify how resources are provisioned during image creation. Can be
    /// null if no provisioners are specified.</param>
    /// <param name="postProcessors">A list of post-processor configurations that define actions to perform after image creation. Can be null if no
    /// post-processors are specified.</param>
    /// <param name="variables">A dictionary of variable names and values used for parameterizing the Packer file. Can be null if no variables
    /// are required.</param>
    public PackerFile(List<PackerBuilder>? builders, string? description, List<Provisioner>? provisioners, List<PostProcessor>? postProcessors, Dictionary<string, string>? variables)
    {
        Builders = builders;
        Description = description;
        Provisioners = provisioners;
        PostProcessors = postProcessors;
        Variables = variables;
    }

    [JsonPropertyName("builders")] public List<PackerBuilder>? Builders { get; set; }

    [JsonPropertyName("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [JsonPropertyName("provisioners")] public List<Provisioner>? Provisioners { get; set; }

    [JsonPropertyName("post-processors")] public List<PostProcessor>? PostProcessors { get; set; }

    [JsonPropertyName("variables")] public Dictionary<string, string>? Variables { get; set; }
}