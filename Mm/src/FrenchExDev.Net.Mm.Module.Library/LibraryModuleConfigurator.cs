using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library;

/// <summary>
/// Logic to configure a <see cref="ILibraryModule"/>.
/// </summary>
public class LibraryModuleConfigurator : ILibraryModuleConfigurator
{
    /// <summary>
    /// Configures the specified library module by setting up its dependencies, configuration, and services.
    /// </summary>
    /// <remarks>This method sequentially invokes the library module's configuration methods to set up its
    /// dependencies, configuration, and services. Ensure that all required parameters are provided and valid before
    /// calling this method.</remarks>
    /// <param name="libraryModule">The library module to configure. This module defines the configuration logic for dependencies, settings, and
    /// services.</param>
    /// <param name="libraryModuleLoader">The loader responsible for managing the lifecycle of library modules.</param>
    /// <param name="configurationManager">The configuration manager used to access and manage application settings.</param>
    /// <param name="serviceCollection">The service collection to which the library module's services will be added.</param>
    /// <param name="hostEnvironment">The host environment providing information about the application's runtime environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will terminate early if the token is canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ConfigureAsync(
        ILibraryModule libraryModule,
        ILibraryModuleLoader libraryModuleLoader,
        IConfigurationManager configurationManager,
        IServiceCollection serviceCollection,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        await libraryModule.ConfigureDependenciesAsync(
            libraryModuleLoader,
            serviceCollection,
            configurationManager,
            hostEnvironment,
            cancellationToken
        );

        await libraryModule.ConfigureConfigurationAsync(
            configurationManager,
            hostEnvironment,
            cancellationToken
        );

        await libraryModule.ConfigureServicesAsync(
            serviceCollection,
            configurationManager,
            hostEnvironment,
            cancellationToken
        );
    }
}
