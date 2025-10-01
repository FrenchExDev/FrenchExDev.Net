#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

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
public class PackerBundleBuilder : AbstractBuilder<PackerBundle>
{
    /// <summary>
    /// List of directories to include in the bundle.
    /// </summary>
    private readonly List<string> _directories = [];
    /// <summary>
    /// Dictionary of named scripts to include in the bundle.
    /// </summary>
    private readonly ShellScriptBuildersDictionary _scriptsBuilders = [];
    /// <summary>
    /// List of plugins to include in the bundle.
    /// </summary>
    private readonly List<string> _plugins = [];
    /// <summary>
    /// Builder for the Packer file configuration.
    /// </summary>
    private readonly PackerFileBuilder _packerFileBuilder = new();
    /// <summary>
    /// Builder for the HTTP directory configuration.
    /// </summary>
    private readonly HttpDirectoryBuilder _httpDirectory = new();
    /// <summary>
    /// Builder for the Vagrant directory configuration.
    /// </summary>
    private readonly VagrantDirectoryBuilder _vagrantDirectory = new();

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
    /// Adds multiple shell script builders to the bundle using the specified mapping of script names to builder
    /// actions.
    /// </summary>
    /// <remarks>Each entry in <paramref name="builderBody"/> results in a new shell script builder being
    /// created and configured. This method enables fluent configuration of multiple scripts within the
    /// bundle.</remarks>
    /// <param name="builderBody">A dictionary that maps script names to actions which configure a <see cref="ShellScriptBuilder"/> instance for
    /// each script. Each action is invoked with a new builder for its corresponding script name.</param>
    /// <returns>The current <see cref="PackerBundleBuilder"/> instance to allow method chaining.</returns>
    public PackerBundleBuilder Script(Dictionary<string, Action<ShellScriptBuilder>> builderBody)
    {
        foreach (var kvp in builderBody)
        {
            var builder = new ShellScriptBuilder();
            kvp.Value(builder);
            _scriptsBuilders.Add(kvp.Key, builder);
        }
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
    /// Performs validation logic for the current object, recording visited objects and any validation failures
    /// encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// or circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a specific validation error found
    /// during processing.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        _packerFileBuilder.Validate(visitedCollector, failures);
        _httpDirectory.Validate(visitedCollector, failures);
        _vagrantDirectory.Validate(visitedCollector, failures);

        foreach (var scriptBuilder in _scriptsBuilders.Values)
        {
            scriptBuilder.Validate(visitedCollector, failures);
        }
    }

    /// <summary>
    /// Creates and returns a new instance of the <see cref="PackerBundle"/> class populated with the current
    /// configuration and built components.
    /// </summary>
    /// <remarks>The returned <see cref="PackerBundle"/> reflects the current state of the builder and its
    /// associated components. Subsequent changes to the builder will not affect the previously instantiated
    /// bundle.</remarks>
    /// <returns>A <see cref="PackerBundle"/> instance containing the configured packer file, directories, scripts, and plugins.</returns>
    protected override PackerBundle Instantiate()
    {
        return new PackerBundle()
        {
            PackerFile = _packerFileBuilder.Build().Success<PackerFile>(),
            HttpDirectory = _httpDirectory.Build().Success<HttpDirectory>(),
            VagrantDirectory = _vagrantDirectory.Build().Success<VagrantDirectory>(),
            Directories = [.. _directories],
            Scripts = _scriptsBuilders.Build(),
            Plugins = [.. _plugins]
        };
    }
}