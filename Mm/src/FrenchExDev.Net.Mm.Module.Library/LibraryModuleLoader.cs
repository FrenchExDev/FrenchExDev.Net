using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.Mm.Module.Library;

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
    private readonly LoadedModules _loaded = new();

    /// <summary>
    /// Logger for module loader operations.
    /// </summary>
    private readonly ILogger<LibraryModuleLoader> _logger;

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
    /// Returns the collection of modules that have been loaded.
    /// </summary>
    public LoadedModules LoadedModules => _loaded;

    /// <summary>
    /// Loads and configures library modules asynchronously, resolving dependencies in rounds.
    /// </summary>
    /// <param name="libraryModules">Dictionary of module factories keyed by ModuleId.</param>
    /// <param name="servicesCollection">Service collection for dependency injection.</param>
    /// <param name="configurationManager">Configuration manager instance.</param>
    /// <param name="hostEnvironment">Host environment information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LoadAsync(
        LoadableLibraryModules libraryModules,
        IServiceCollection servicesCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var module in libraryModules)
        {
            _logger.LogDebug("Preparing to load module {ModuleId}", module.Key);
            var moduleId = module.Key;
            if (_loaded.ContainsKey(moduleId))
            {
                _logger.LogWarning("Module {ModuleId} has already been loaded. Skipping.", moduleId);
                continue;
            }

            var moduleInstance = module.Value();
            _logger.LogDebug("Module {ModuleId} instantiated successfully.", moduleId);
            _loaded.Add(moduleId, moduleInstance);

            _logger.LogDebug("Module {ModuleId} added to loaded modules.", moduleId);

            await _LibraryModuleConfigurator.ConfigureAsync(
                    moduleInstance,
                    this,
                    configurationManager,
                    servicesCollection,
                    hostEnvironment,
                    cancellationToken
                );

        }

        await _LibraryModuleMediatorConfigurator.ConfigureMediatorServicesAsync(servicesCollection,
            _loaded.Values.ToList());
    }
}
