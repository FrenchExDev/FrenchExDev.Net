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
/// Provides a fluent builder for configuring and constructing instances of <see cref="PackerBundle"/>.
/// </summary>
/// <remarks>
/// Use this builder to specify directories, plugins, scripts, and configure the Packer file, HTTP directory, and Vagrant directory for a Packer bundle.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="PackerBundle"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new PackerBundleBuilder()
///     .Directory("/tmp/build")
///     .Plugin("packer-plugin-virtualbox")
///     .Script("setup", new ShellScriptBuilder().AddLine("echo Hello").Build())
///     .PackerFile(fileBuilder => fileBuilder.Description("Alpine build"))
///     .HttpDirectory(httpBuilder => httpBuilder.AddFile("file.txt", new File()))
///     .VagrantDirectory(vagrantBuilder => vagrantBuilder.AddFile("Vagrantfile", new File()));
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class PackerBundleBuilder : AbstractObjectBuilder<PackerBundle, PackerBundleBuilder>
{
    /// <summary>
    /// List of directories to include in the bundle.
    /// </summary>
    private readonly List<string> _directories = new();
    /// <summary>
    /// Dictionary of named scripts to include in the bundle.
    /// </summary>
    private readonly ShellScriptBuildersDictionary _scriptsBuilders = new();
    /// <summary>
    /// List of plugins to include in the bundle.
    /// </summary>
    private readonly List<string> _plugins = new();
    /// <summary>
    /// Builder for the Packer file configuration.
    /// </summary>
    private PackerFileBuilder _packerFileBuilder { get; } = new();
    /// <summary>
    /// Builder for the HTTP directory configuration.
    /// </summary>
    private HttpDirectoryBuilder _httpDirectory { get; } = new();
    /// <summary>
    /// Builder for the Vagrant directory configuration.
    /// </summary>
    private VagrantDirectoryBuilder _vagrantDirectory { get; } = new();

    /// <summary>
    /// Adds a directory to the bundle.
    /// </summary>
    /// <param name="dir">Directory path.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Directory("/tmp/build")</remarks>
    public PackerBundleBuilder Directory(string dir)
    {
        _directories.Add(dir);
        return this;
    }

    /// <summary>
    /// Adds a plugin to the bundle.
    /// </summary>
    /// <param name="plugin">Plugin name.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Plugin("packer-plugin-virtualbox")</remarks>
    public PackerBundleBuilder Plugin(string plugin)
    {
        _plugins.Add(plugin);
        return this;
    }

    /// <summary>
    /// Adds a named script to the bundle using a builder action.
    /// </summary>
    /// <param name="name">Script name.</param>
    /// <param name="builderBody">Action to configure the script builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Script("setup", builder => builder.AddLine("echo Hello"))</remarks>
    public PackerBundleBuilder Script(string name, Action<ShellScriptBuilder> builderBody)
    {
        var builder = new ShellScriptBuilder();
        builderBody(builder);
        _scriptsBuilders.Add(name, builder);
        return this;
    }

    /// <summary>
    /// Removes a named script from the bundle.
    /// </summary>
    /// <param name="script">Script name.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: ScriptRemove("setup")</remarks>
    public PackerBundleBuilder ScriptRemove(string script)
    {
        _scriptsBuilders.Remove(script);
        return this;
    }

    /// <summary>
    /// Configures the Vagrant directory using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the Vagrant directory builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: VagrantDirectory(builder => builder.AddFile("Vagrantfile", new File()))</remarks>
    public PackerBundleBuilder VagrantDirectory(Action<VagrantDirectoryBuilder> builderBody)
    {
        builderBody(_vagrantDirectory);
        return this;
    }

    /// <summary>
    /// Configures the HTTP directory using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the HTTP directory builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: HttpDirectory(builder => builder.AddFile("file.txt", new File()))</remarks>
    public PackerBundleBuilder HttpDirectory(Action<HttpDirectoryBuilder> builderBody)
    {
        builderBody(_httpDirectory);
        return this;
    }

    /// <summary>
    /// Configures the Packer file using a builder action.
    /// </summary>
    /// <param name="builderBody">Action to configure the Packer file builder.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: PackerFile(builder => builder.Description("Alpine build"))</remarks>
    public PackerBundleBuilder PackerFile(Action<PackerFileBuilder> builderBody)
    {
        builderBody(_packerFileBuilder);
        return this;
    }

    /// <summary>
    /// Builds a new instance of a <see cref="PackerBundle"/> by aggregating results from its component builders and
    /// directories.
    /// </summary>
    /// <remarks>If any component builder fails, its exceptions are added to the <paramref name="exceptions"/>
    /// dictionary, and the method returns a failure result. The <paramref name="visited"/> list helps avoid processing
    /// the same object multiple times, which is useful in scenarios with cyclic references.</remarks>
    /// <param name="exceptions">A dictionary used to collect exceptions encountered during the build process. Exceptions from failed component
    /// builds are added to this dictionary.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process. Used to prevent redundant processing
    /// and handle cyclic dependencies.</param>
    /// <returns>An <see cref="IObjectBuildResult{PackerBundle}"/> representing the outcome of the build operation. Returns a
    /// successful result containing the constructed <see cref="PackerBundle"/> if all components build successfully;
    /// otherwise, returns a failure result with collected exceptions.</returns>
    protected override IObjectBuildResult<PackerBundle> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        var packerFileBuildResult = _packerFileBuilder.Build();
        if (packerFileBuildResult is FailureObjectBuildResult<PackerFile, PackerFileBuilder> failure)
            exceptions.Add(nameof(_packerFileBuilder), failure.Exceptions);

        var httpDirectoryBuildResult = _httpDirectory.Build(visited);
        if (httpDirectoryBuildResult is FailureObjectBuildResult<HttpDirectory, HttpDirectoryBuilder> httpFailure)
            exceptions.Add(nameof(_httpDirectory), httpFailure.Exceptions);

        var vagrantDirectoryBuildResult = _vagrantDirectory.Build(visited);
        if (vagrantDirectoryBuildResult is FailureObjectBuildResult<VagrantDirectory, VagrantDirectoryBuilder> vagrantFailure)
            exceptions.Add(nameof(_vagrantDirectory), vagrantFailure.Exceptions);

        var scripts = new Dictionary<string, IScript>();
        foreach (var k in _scriptsBuilders)
        {
            var list = k.Value.Build(visited);
            if (list is FailureObjectBuildResult<ShellScript, ShellScriptBuilder> failureList)
            {
                exceptions.Add(nameof(_scriptsBuilders), failureList.Exceptions);
                continue;
            }

            scripts[k.Key] = list.Success();
        }

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        return Success(new PackerBundle()
        {
            PackerFile = packerFileBuildResult.Success(),
            HttpDirectory = httpDirectoryBuildResult.Success(),
            VagrantDirectory = vagrantDirectoryBuildResult.Success(),
            Directories = _directories,
            Scripts = scripts,
            Plugins = _plugins
        });
    }
}