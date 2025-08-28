using FrenchExDev.Mm.Net.Abstractions;
using FrenchExDev.Mm.Net.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Mm.Net.Module.Library;

/// <summary>
/// Provides functionality to load and configure library modules in a .NET application.
/// </summary>
/// <remarks>
/// Responsible for managing the lifecycle of library modules, including loading, configuration, and dependency resolution.
/// Modules are loaded in rounds to ensure dependencies are resolved before configuration. Supports deferred loading for modules with dependencies.
/// </remarks>
public class LibraryModuleLoader : ILibraryModuleLoader
{
    /// <summary>
    /// Configurator for individual library modules.
    /// </summary>
    private readonly ILibraryModuleConfigurator _LibraryModuleConfigurator;

    /// <summary>
    /// Configurator for mediator services between modules.
    /// </summary>
    private readonly ILibraryModuleMediatorConfigurator _LibraryModuleMediatorConfigurator;

    /// <summary>
    /// Stores loaded modules, keyed by their ModuleId.
    /// </summary>
    private readonly Dictionary<ModuleId, ILibraryModule> _loaded = new();

    /// <summary>
    /// Logger for module loader operations.
    /// </summary>
    private readonly ILogger<LibraryModuleLoader> _logger;

    /// <summary>
    /// Stores modules to be loaded in the next round, keyed by ModuleId.
    /// </summary>
    private Dictionary<ModuleId, Func<ILibraryModule>> _loadNextRound = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryModuleLoader"/> class.
    /// </summary>
    /// <param name="LibraryModuleConfigurator">Module configurator instance.</param>
    /// <param name="LibraryModuleMediatorConfigurator">Mediator configurator instance.</param>
    /// <param name="logger">Logger instance.</param>
    public LibraryModuleLoader(
        ILibraryModuleConfigurator LibraryModuleConfigurator,
        ILibraryModuleMediatorConfigurator LibraryModuleMediatorConfigurator,
        ILogger<LibraryModuleLoader> logger
    )
    {
        _LibraryModuleConfigurator = LibraryModuleConfigurator;
        _LibraryModuleMediatorConfigurator = LibraryModuleMediatorConfigurator;
        _logger = logger;
    }

    /// <summary>
    /// Loads and configures library modules asynchronously, resolving dependencies in rounds.
    /// </summary>
    /// <param name="LibraryModules">Dictionary of module factories keyed by ModuleId.</param>
    /// <param name="servicesCollection">Service collection for dependency injection.</param>
    /// <param name="configurationManager">Configuration manager instance.</param>
    /// <param name="hostEnvironment">Host environment information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LoadAsync(
        Dictionary<ModuleId, Func<ILibraryModule>> LibraryModules,
        IServiceCollection servicesCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        var LibraryModulesToLoad = LibraryModules;

        // Loop over modules to load, handling dependencies in rounds.
        do
        {
            var modulesToLoad = new Dictionary<ModuleId, ILibraryModule>();
            foreach (var module in LibraryModulesToLoad)
            {
                if (HasBeenAlreadyLoaded(module.Key) && modulesToLoad.ContainsKey(module.Key)) continue;

                var moduleToLoad = module.Value();

                modulesToLoad.Add(module.Key, moduleToLoad);
            }

            foreach (var LibraryModule in modulesToLoad)
            {
                await _LibraryModuleConfigurator.ConfigureAsync(
                    LibraryModule.Value,
                    this,
                    configurationManager,
                    servicesCollection,
                    hostEnvironment,
                    cancellationToken
                );

                _loaded.Add(LibraryModule.Key, LibraryModule.Value);
            }

            LibraryModulesToLoad = _loadNextRound;
            _loadNextRound = new Dictionary<ModuleId, Func<ILibraryModule>>();
        } while (LibraryModulesToLoad.Count > 0);

        await _LibraryModuleMediatorConfigurator.ConfigureMediatorServicesAsync(servicesCollection,
            _loaded.Values.ToList());
    }

    /// <summary>
    /// Registers a module to be loaded in the next round if it has not already been loaded.
    /// </summary>
    /// <param name="record">The module identifier.</param>
    /// <param name="LibraryModule">Factory function for the module.</param>
    public void LoadNextRound(ModuleId record, Func<ILibraryModule> LibraryModule)
    {
        if (!HasBeenAlreadyLoaded(record))
            _loadNextRound.TryAdd(record, LibraryModule);
    }

    /// <summary>
    /// Checks if a module has already been loaded.
    /// </summary>
    /// <param name="record">The module identifier.</param>
    /// <returns>True if the module is already loaded; otherwise, false.</returns>
    public bool HasBeenAlreadyLoaded(ModuleId record)
    {
        return _loaded.ContainsKey(record);
    }
}
