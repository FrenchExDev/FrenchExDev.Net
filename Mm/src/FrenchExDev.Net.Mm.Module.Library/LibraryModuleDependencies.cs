using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library;

/// <summary>
/// Represents a collection of library module dependencies, where each dependency is associated with a  unique module
/// identifier and a factory function for creating the module instance.
/// </summary>
/// <remarks>This class provides functionality to retrieve the types of the library modules and to load the 
/// dependency modules in the next round of processing.</remarks>
/// <param name="dependencies"></param>
internal class LibraryModuleDependencies(Dictionary<ModuleId, Func<ILibraryModule>> dependencies)
{
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
    /// Loads the dependency modules during the next round of module loading.
    /// </summary>
    /// <param name="libraryModuleLoader">The module loader responsible for managing the loading of library modules.</param>
    /// <param name="configurationManager">The configuration manager providing configuration settings for the modules.</param>
    /// <param name="hostEnvironment">The host environment that provides information about the application's environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task LoadDependencyModulesOnNextRound(
        ILibraryModuleLoader libraryModuleLoader,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var module in dependencies) libraryModuleLoader.LoadNextRound(module.Key, module.Value);

        return Task.CompletedTask;
    }
}
