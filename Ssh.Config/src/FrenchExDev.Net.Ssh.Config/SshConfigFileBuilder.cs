#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;


#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides a builder for constructing an SSH configuration file composed of multiple host entries.
/// </summary>
/// <remarks>Use this builder to programmatically define SSH host configurations and generate a complete SSH
/// config file. Host entries can be added using the <see cref="Host"/> method, which allows customization of each
/// host's settings. This class is typically used in scenarios where SSH configuration needs to be generated or modified
/// dynamically within an application.</remarks>
public class SshConfigFileBuilder : AbstractBuilder<SshConfigFile>
{
    /// <summary>
    /// Holds the list of host builders used to construct individual SSH host configurations.
    /// </summary>
    private readonly BuilderList<SshConfigHost, SshConfigHostBuilder> _hostBuilders = [];

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
    /// Creates a new instance of the <see cref="SshConfigFile"/> class with its hosts initialized from the current host
    /// builders.
    /// </summary>
    /// <returns>A new <see cref="SshConfigFile"/> object containing the hosts defined by the host builders.</returns>
    protected override SshConfigFile Instantiate()
    {
        return new SshConfigFile()
        {
            Hosts = _hostBuilders.BuildSuccess()
        };
    }

    /// <summary>
    /// Performs validation on the collection of host builders, recording any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a specific validation error found
    /// during the process.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        ValidateListInternal(_hostBuilders, nameof(_hostBuilders), visitedCollector, failures);
    }
}