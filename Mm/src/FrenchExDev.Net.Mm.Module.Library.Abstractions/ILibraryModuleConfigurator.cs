using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

/// <summary>
/// Logic for configuring a library module
/// </summary>
public interface ILibraryModuleConfigurator
{

    /// <summary>
    /// Method for configuring a library module.
    /// </summary>
    /// <param name="libraryModule">Reference to the library module to configure.</param>
    /// <param name="libraryModuleLoader">Reference to the library module loader.</param>
    /// <param name="configurationManager">Reference to the configuration manager.</param>
    /// <param name="serviceCollection">Reference to the service collection.</param>
    /// <param name="hostEnvironment">Reference to the host environment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ConfigureAsync(
        ILibraryModule libraryModule,
        ILibraryModuleLoader libraryModuleLoader,
        IConfigurationManager configurationManager,
        IServiceCollection serviceCollection,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );
}
