#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="PackerFile"/>.
/// </summary>
/// <remarks>
/// Use this builder to specify builders, provisioners, post-processors, and variables for a Packer file.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="PackerFile"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new PackerFileBuilder()
///     .Description("Alpine build")
///     .Builder(b => b.Format("ova").VmName("alpine-vm"))
///     .Provisioner(p => p.AddScript("setup.sh"))
///     .PostProcessor(pp => pp.Type("vagrant").Output("output.box"))
///     .Variable("os_version", "3.18.0");
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class PackerFileBuilder : AbstractBuilder<PackerFile>
{
    /// <summary>
    /// List of builder configurations for the Packer file.
    /// </summary>
    private BuilderList<PackerBuilder, PackerBuilderBuilder> _builders = [];
    /// <summary>
    /// Description of the Packer file.
    /// </summary>
    private string? _description;
    /// <summary>
    /// List of post-processor configurations for the Packer file.
    /// </summary>
    private BuilderList<PostProcessor, PostProcessorBuilder> _postProcessors = [];
    /// <summary>
    /// List of provisioner configurations for the Packer file.
    /// </summary>
    private BuilderList<Provisioner, ProvisionerBuilder> _provisioners = [];
    /// <summary>
    /// Dictionary of variables to be used in the Packer file.
    /// </summary>
    private Dictionary<string, string>? _variables;

    /// <summary>
    /// Adds a builder configuration to the Packer file.
    /// </summary>
    /// <param name="builder">Builder instance.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddBuilder(new PackerBuilderBuilder().Format("ova"))</remarks>
    public PackerFileBuilder AddBuilder(PackerBuilderBuilder builder)
    {
        _builders.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a builder configuration using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Builder(b => b.Format("ova").VmName("alpine-vm"))</remarks>
    public PackerFileBuilder Builder(Action<PackerBuilderBuilder> builderBody)
    {
        var builder = new PackerBuilderBuilder();
        builderBody(builder);
        AddBuilder(builder);
        return this;
    }

    /// <summary>
    /// Gets a builder configuration matching the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate to match the builder.</param>
    /// <returns>The matching builder instance, or null if not found.</returns>
    public PackerBuilderBuilder? GetBuilder(Func<PackerBuilderBuilder, bool> predicate)
    {
        return _builders?.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Sets the description for the Packer file.
    /// </summary>
    /// <param name="value">Description string.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Description("Alpine build")</remarks>
    public PackerFileBuilder Description(string? value)
    {
        _description = value;
        return this;
    }

    /// <summary>
    /// Adds a provisioner configuration to the Packer file.
    /// </summary>
    /// <param name="provisioner">Provisioner instance.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Provisioner(new ProvisionerBuilder().AddScript("setup.sh"))</remarks>
    public PackerFileBuilder Provisioner(ProvisionerBuilder provisioner)
    {
        _provisioners.Add(provisioner);
        return this;
    }

    /// <summary>
    /// Adds a provisioner configuration using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the provisioner builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Provisioner(p => p.AddScript("setup.sh"))</remarks>
    public PackerFileBuilder Provisioner(Action<ProvisionerBuilder> builderBody)
    {
        var builder = new ProvisionerBuilder();
        builderBody(builder);
        Provisioner(builder);
        return this;
    }

    /// <summary>
    /// Gets a provisioner configuration matching the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate to match the provisioner.</param>
    /// <returns>The matching provisioner instance, or null if not found.</returns>
    public ProvisionerBuilder? GetProvisioner(Func<ProvisionerBuilder, bool> predicate)
    {
        return _provisioners?.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Updates a provisioner configuration matching the specified predicate using an updater action.
    /// </summary>
    /// <param name="predicate">Predicate to match the provisioner.</param>
    /// <param name="updater">Action to update the provisioner.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: UpdateProvisioner(p => p.Name == "setup", p => p.AddScript("install.sh"))</remarks>
    public PackerFileBuilder UpdateProvisioner(Func<ProvisionerBuilder, bool> predicate,
        Action<ProvisionerBuilder> updater)
    {
        var provisioner = _provisioners?.FirstOrDefault(predicate);
        if (provisioner is not null) updater(provisioner);

        return this;
    }

    /// <summary>
    /// Adds a post-processor configuration to the Packer file.
    /// </summary>
    /// <param name="value">Post-processor instance.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: PostProcessor(new PostProcessorBuilder().Type("vagrant"))</remarks>
    public PackerFileBuilder PostProcessor(PostProcessorBuilder value)
    {
        _postProcessors.Add(value);
        return this;
    }

    /// <summary>
    /// Adds a post-processor configuration using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the post-processor builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: PostProcessor(pp => pp.Type("vagrant").Output("output.box"))</remarks>
    public PackerFileBuilder PostProcessor(Action<PostProcessorBuilder> builderBody)
    {
        var builder = new PostProcessorBuilder();
        builderBody(builder);
        PostProcessor(builder);

        return this;
    }

    /// <summary>
    /// Gets a post-processor configuration matching the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate to match the post-processor.</param>
    /// <returns>The matching post-processor instance, or null if not found.</returns>
    public PostProcessorBuilder? GetPostProcessor(Func<PostProcessorBuilder, bool> predicate)
    {
        return _postProcessors?.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Updates a post-processor configuration matching the specified predicate using an updater action.
    /// </summary>
    /// <param name="predicate">Predicate to match the post-processor.</param>
    /// <param name="updater">Action to update the post-processor.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: UpdatePostProcessor(pp => pp.Type == "vagrant", pp => pp.Output("new.box"))</remarks>
    public PackerFileBuilder UpdatePostProcessor(Func<PostProcessorBuilder, bool> predicate,
        Action<PostProcessorBuilder> updater)
    {
        var postProcessor = _postProcessors?.FirstOrDefault(predicate);
        if (postProcessor is not null) updater(postProcessor);

        return this;
    }

    /// <summary>
    /// Adds or updates a variable for the Packer file.
    /// </summary>
    /// <param name="key">Variable name.</param>
    /// <param name="value">Variable value.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Variable("os_version", "3.18.0")</remarks>
    public PackerFileBuilder Variable(string key, string value)
    {
        _variables ??= new Dictionary<string, string>();
        _variables[key] = value;
        return this;
    }

    /// <summary>
    /// Changes the value of an existing variable or adds a new one.
    /// </summary>
    /// <param name="key">Variable name.</param>
    /// <param name="value">Variable value.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: ChangeVariable("os_version", "3.19.0")</remarks>
    public PackerFileBuilder ChangeVariable(string key, string value)
    {
        return Variable(key, value);
    }

    /// <summary>
    /// Performs validation on the internal collections of builders, post-processors, and provisioners, recording any
    /// validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a specific issue found during the
    /// validation process.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        ValidateListInternal(_builders, nameof(_builders), visitedCollector, failures);
        ValidateListInternal(_postProcessors, nameof(_postProcessors), visitedCollector, failures);
        ValidateListInternal(_provisioners, nameof(_provisioners), visitedCollector, failures);
    }

    /// <summary>
    /// Creates and returns a new instance of the <see cref="PackerFile"/> class using the current configuration.
    /// </summary>
    /// <remarks>If no variables are defined, an empty dictionary is used. The returned <see
    /// cref="PackerFile"/> reflects the current state of the configuration and can be used for further processing or
    /// serialization.</remarks>
    /// <returns>A <see cref="PackerFile"/> object initialized with the current builders, description, provisioners,
    /// post-processors, and variables.</returns>
    protected override PackerFile Instantiate()
    {
        return new(_builders.BuildSuccess(), _description, _provisioners.BuildSuccess(), _postProcessors.BuildSuccess(), _variables ?? new Dictionary<string, string>());
    }

    /// <summary>
    /// Builds the internal object graph by processing builders, post-processors, and provisioners using the specified
    /// visited object dictionary.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This prevents
    /// redundant processing and helps manage object references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_builders, visitedCollector);
        BuildList(_postProcessors, visitedCollector);
        BuildList(_provisioners, visitedCollector);
    }
}