#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;


#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides a builder for constructing an SSH configuration file composed of multiple host entries.
/// </summary>
/// <remarks>Use this builder to programmatically define SSH host configurations and generate a complete SSH
/// config file. Host entries can be added using the <see cref="Host"/> method, which allows customization of each
/// host's settings. This class is typically used in scenarios where SSH configuration needs to be generated or modified
/// dynamically within an application.</remarks>
public class SshConfigFileBuilder : AbstractObjectBuilder<SshConfigFile, SshConfigFileBuilder>
{
    /// <summary>
    /// Holds the list of host builders used to construct individual SSH host configurations.
    /// </summary>
    private readonly List<SshConfigHostBuilder> _hostBuilders = [];

    /// <summary>
    /// Adds a new host entry to the SSH configuration using the specified configuration action.
    /// </summary>
    /// <remarks>Each call to this method creates a new host entry in the SSH configuration. This method
    /// supports a fluent interface for building multiple host entries.</remarks>
    /// <param name="body">An action that configures the host entry by operating on an <see cref="SshConfigHostBuilder"/> instance. Cannot
    /// be null.</param>
    /// <returns>The current <see cref="SshConfigFileBuilder"/> instance, enabling method chaining.</returns>
    public SshConfigFileBuilder Host(Action<SshConfigHostBuilder> body)
    {
        var builder = new SshConfigHostBuilder();
        body(builder);
        _hostBuilders.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds an instance of <see cref="SshConfigFile"/> by aggregating results from host builders, collecting any
    /// build errors encountered.
    /// </summary>
    /// <param name="exceptions">A dictionary used to collect exceptions that occur during the build process. Existing entries may be augmented
    /// with additional errors.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process to prevent redundant processing or
    /// circular references.</param>
    /// <returns>An <see cref="IObjectBuildResult{SshConfigFile}"/> representing either a successful build containing the
    /// constructed <see cref="SshConfigFile"/>, or a failure result containing the collected exceptions.</returns>
    protected override IObjectBuildResult<SshConfigFile> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        var buildErrors = _hostBuilders.Select(x => x.Build(visited)).ToList();

        var errors = buildErrors.OfType<FailureObjectBuildResult<SshConfigFile, SshConfigFileBuilder>>()
            .SelectMany(x => x.Exceptions)
            .ToList();

        if (errors.Count == 0)
        {
            return Success(new SshConfigFile()
            {
                Hosts = [.. buildErrors.OfType<SuccessObjectBuildResult<SshConfigHost>>().Select(x => x.Result)]
            });
        }

        foreach (KeyValuePair<string, List<Exception>> error in errors)
            exceptions.Add(error.Key, error.Value);

        return Failure(exceptions, visited);
    }
}