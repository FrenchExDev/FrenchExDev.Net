using FrenchExDev.Net.Mm.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

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
        LoadableLibraryModules libraryModules,
        IServiceCollection servicesCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );
}
