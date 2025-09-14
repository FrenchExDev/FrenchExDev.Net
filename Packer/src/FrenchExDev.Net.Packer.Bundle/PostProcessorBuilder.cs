#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;


#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="PostProcessor"/> for Packer post-processing steps.
/// </summary>
/// <remarks>
/// Use this builder to specify required and optional settings for a Packer post-processor, such as compression level, output file, and Vagrantfile template.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="PostProcessor"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new PostProcessorBuilder()
///     .Type("vagrant")
///     .CompressionLevel(6)
///     .KeepInputArtefact(true)
///     .Output("output.box")
///     .VagrantfileTemplate("template.tpl");
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class PostProcessorBuilder : DeconstructedAbstractObjectBuilder<PostProcessor, PostProcessorBuilder>
{
    /// <summary>
    /// Compression level for the post-processed artifact (e.g., 1-9 for gzip).
    /// </summary>
    /// <remarks>Higher values result in better compression but slower processing.</remarks>
    private int? _compressionLevel;

    /// <summary>
    /// Whether to keep the input artifact after post-processing.
    /// </summary>
    /// <remarks>Set to true to retain the original artifact. Default is false.</remarks>
    private bool? _keepInputArtefact;

    /// <summary>
    /// Output file path for the post-processed artifact.
    /// </summary>
    /// <remarks>Example: "output.box".</remarks>
    private string? _output;

    /// <summary>
    /// Type of post-processor (e.g., "vagrant", "compress").
    /// </summary>
    /// <remarks>Example: "vagrant" for Vagrant box post-processing.</remarks>
    private string? _type;

    /// <summary>
    /// Path to the Vagrantfile template to use in the post-processing step.
    /// </summary>
    /// <remarks>Example: "template.tpl".</remarks>
    private string? _vagrantfileTemplate;

    /// <summary>
    /// Sets the type of post-processor.
    /// </summary>
    /// <param name="value">Type string (e.g., "vagrant").</param>
    /// <returns>The builder instance.</returns>
    public PostProcessorBuilder Type(string? value)
    {
        _type = value;
        return this;
    }

    /// <summary>
    /// Sets the compression level for the post-processed artifact.
    /// </summary>
    /// <param name="value">Compression level (e.g., 1-9).</param>
    /// <returns>The builder instance.</returns>
    public PostProcessorBuilder CompressionLevel(int value)
    {
        _compressionLevel = value;
        return this;
    }

    /// <summary>
    /// Sets whether to keep the input artifact after post-processing.
    /// </summary>
    /// <param name="value">True to keep the input artifact; false to discard.</param>
    /// <returns>The builder instance.</returns>
    public PostProcessorBuilder KeepInputArtefact(bool value = true)
    {
        _keepInputArtefact = value;
        return this;
    }

    /// <summary>
    /// Sets the output file path for the post-processed artifact.
    /// </summary>
    /// <param name="value">Output file path (e.g., "output.box").</param>
    /// <returns>The builder instance.</returns>
    public PostProcessorBuilder Output(string? value)
    {
        _output = value;
        return this;
    }

    /// <summary>
    /// Sets the path to the Vagrantfile template.
    /// </summary>
    /// <param name="vagrantfileTemplate">Template file path (e.g., "template.tpl").</param>
    /// <returns>The builder instance.</returns>
    public PostProcessorBuilder VagrantfileTemplate(string? vagrantfileTemplate)
    {
        _vagrantfileTemplate = vagrantfileTemplate;
        return this;
    }

    /// <summary>
    /// Instantiates a <see cref="PostProcessor"/> object using the configured properties.
    /// </summary>
    /// <returns>The constructed <see cref="PostProcessor"/> instance.</returns>
    /// <remarks>All required properties must be set before instantiation.</remarks>
    protected override PostProcessor Instantiate()
    {
        return new PostProcessor
        {
            Type = _type,
            CompressionLevel = _compressionLevel,
            KeepInputArtifact = _keepInputArtefact ?? false,
            Output = _output,
            VagrantfileTemplate = _vagrantfileTemplate
        };
    }

    /// <summary>
    /// Validates the builder configuration and adds any validation errors to the exception dictionary.
    /// </summary>
    /// <param name="exceptions">Dictionary to collect validation exceptions.</param>
    /// <param name="visited">List of visited objects for cyclic dependency detection.</param>
    /// <remarks>
    /// All required properties are validated before building. If any required property is missing or invalid, the build will fail and exceptions will be collected.
    /// </remarks>
    protected override void Validate(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        AssertNotEmptyOrWhitespace(_output, nameof(PostProcessor.Output), exceptions);
        AssertNotEmptyOrWhitespace(_vagrantfileTemplate, nameof(PostProcessor.VagrantfileTemplate), exceptions);
        AssertNotEmptyOrWhitespace(_type, nameof(PostProcessor.Type), exceptions);
        AssertNotNull(_compressionLevel, nameof(PostProcessor.CompressionLevel), exceptions);
    }
}