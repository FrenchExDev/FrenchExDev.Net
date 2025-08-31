using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library;

/// <summary>
/// Represents a collection of library module dependencies, where each dependency is associated with a  unique module
/// identifier and a factory function for creating the module instance.
/// </summary>
/// <remarks>This class provides functionality to retrieve the types of the library modules and to load the 
/// dependency modules in the next round of processing.</remarks>
/// <param name="dependencies"></param>
internal class LibraryModuleDependencies(LoadableLibraryModules dependencies)
{
    /// <summary>
    /// Returns the collection of library module dependencies.
    /// </summary>
    public LoadableLibraryModules Dependencies => dependencies;

    /// <summary>
    /// Retrieves a collection of distinct types representing the library modules.
    /// </summary>
    /// <remarks>The returned collection contains unique types derived from the library modules'
    /// dependencies.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Type"/> objects, where each type corresponds to a distinct library
    /// module.</returns>
    public IEnumerable<Type> GetLibraryModulesTypesAsync()
    {
        return dependencies.Select(x => x.Key.GetType()).Distinct();
    }

    /// <summary>
    /// Asynchronously loads the dependencies of the current library module.
    /// </summary>
    /// <remarks>This method iterates through the dependencies of the current library module and invokes the 
    /// <see cref="ILibraryModuleLoader.LoadAsync"/> method for each dependency. It ensures that all dependencies  are
    /// loaded asynchronously and integrates them into the provided service collection.</remarks>
    /// <param name="libraryModuleLoader">The loader responsible for loading library modules.</param>
    /// <param name="serviceCollection">The service collection to which services can be added during the loading process.</param>
    /// <param name="configurationManager">The configuration manager used to access configuration settings.</param>
    /// <param name="hostEnvironment">The host environment providing information about the application's environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    public async Task LoadAsync(
        ILibraryModuleLoader libraryModuleLoader, 
        IServiceCollection serviceCollection,
        IConfigurationManager configurationManager, 
        IHostEnvironment hostEnvironment, 
        CancellationToken cancellationToken
    )
    {
        foreach (var dependency in Dependencies)
        {
            await libraryModuleLoader.LoadAsync(
                dependencies,
                serviceCollection,
                configurationManager,
                hostEnvironment,
                cancellationToken);
        }
    }
}
