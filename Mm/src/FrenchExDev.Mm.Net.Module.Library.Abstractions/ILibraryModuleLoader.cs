using FrenchExDev.Mm.Net.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Mm.Net.Module.Library.Abstractions;

/// <summary>
/// Logic for loading and managing library modules in an application.
/// </summary>
/// <remarks>This interface provides functionality to load library modules asynchronously during application
/// startup  and to register additional modules dynamically at runtime. It is designed to support modular application 
/// architectures where components can be loaded and configured independently.</remarks>
public interface ILibraryModuleLoader
{
    /// <summary>
    /// Method for loading library modules asynchronously during application startup.
    /// </summary>
    /// <param name="libraryModules">A dictionary of library module identifiers and their corresponding factory functions.</param>
    /// <param name="servicesCollection">The service collection to register services with.</param>
    /// <param name="configurationManager">The configuration manager to use for loading configuration settings.</param>
    /// <param name="hostEnvironment">The host environment information.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task LoadAsync(
        Dictionary<ModuleId, Func<ILibraryModule>> libraryModules,
        IServiceCollection servicesCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Loads the next round of data for the specified module.
    /// </summary>
    /// <remarks>The <paramref name="libraryModule"/> function is invoked to retrieve the library module
    /// instance, which is used to load the data for the specified module. Ensure that the provided <paramref
    /// name="record"/> corresponds to a valid module identifier.</remarks>
    /// <param name="record">The identifier of the module for which the next round of data should be loaded.</param>
    /// <param name="libraryModule">A function that provides an instance of the library module to be used during the loading process.</param>
    void LoadNextRound(ModuleId record, Func<ILibraryModule> libraryModule);
}
