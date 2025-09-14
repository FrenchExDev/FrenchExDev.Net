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
public class PackerFileBuilder : AbstractObjectBuilder<PackerFile, PackerFileBuilder>
{
    /// <summary>
    /// List of builder configurations for the Packer file.
    /// </summary>
    private List<PackerBuilderBuilder>? _builders;
    /// <summary>
    /// Description of the Packer file.
    /// </summary>
    private string? _description;
    /// <summary>
    /// List of post-processor configurations for the Packer file.
    /// </summary>
    private List<PostProcessorBuilder>? _postProcessors;
    /// <summary>
    /// List of provisioner configurations for the Packer file.
    /// </summary>
    private List<ProvisionerBuilder>? _provisioners;
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
        _builders ??= new List<PackerBuilderBuilder>();
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
        _provisioners ??= new List<ProvisionerBuilder>();
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
        _postProcessors ??= new List<PostProcessorBuilder>();
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
    /// Builds a new instance of <see cref="PackerFile"/> by aggregating the results of configured builders, post-processors, and provisioners, while collecting any build exceptions encountered.
    /// </summary>
    /// <remarks>
    /// If any exceptions are recorded in <paramref name="exceptions"/>, the method returns a failure result and does not construct a <see cref="PackerFile"/>. The returned result includes all successfully built components and any variables defined for the packer file.
    /// </remarks>
    /// <param name="exceptions">A dictionary used to record exceptions that occur during the build process. Must not be null.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process to prevent redundant processing. Must not be null.</param>
    /// <returns>An <see cref="IObjectBuildResult{PackerFile}"/> containing the constructed <see cref="PackerFile"/> if the build succeeds; otherwise, a failure result with the collected exceptions.</returns>
    protected override IObjectBuildResult<PackerFile> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        var builders = BuildAndAssertNullOrWithoutFailuresAndReturnSuccesses<PackerBuilder, PackerBuilderBuilder>(_builders, nameof(_builders), exceptions, visited);
        var postProcessors = BuildAndAssertNullOrWithoutFailuresAndReturnSuccesses<PostProcessor, PostProcessorBuilder>(_postProcessors, nameof(_postProcessors), exceptions, visited);
        var provisioners = BuildAndAssertNullOrWithoutFailuresAndReturnSuccesses<Provisioner, ProvisionerBuilder>(_provisioners, nameof(_provisioners), exceptions, visited);

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        return Success(new PackerFile()
        {
            Builders = builders?.Select(x => x.Result).ToList() ?? [],
            Description = _description,
            PostProcessors = postProcessors?.Select(x => x.Result).ToList() ?? [],
            Provisioners = provisioners?.Select(x => x.Result).ToList() ?? [],
            Variables = _variables ?? new Dictionary<string, string>()
        });
    }
}