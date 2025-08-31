using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library;

/// <summary>
/// Abstract base class for a library module in a modular monolith application.
/// Provides mechanisms for dependency management, configuration, and service registration.
/// </summary>
public abstract class LibraryModule(LoadableLibraryModules? dependencies = null) : ILibraryModule
{
    /// <summary>
    /// Manages the dependencies of this module.
    /// </summary>
    private readonly LibraryModuleDependencies _libraryModuleDependencies = new(dependencies ?? new LoadableLibraryModules());

    /// <summary>
    /// Gets the unique identifier of the module.
    /// </summary>
    public ModuleId ModuleId => GetModuleId();

    /// <summary>
    /// Returns the version of the module.
    /// </summary>
    public IModuleVersion ModuleVersion => GetModuleVersion();

    /// <summary>
    /// Returns information about the module.
    /// </summary>
    public IModuleInformation ModuleInformation => GetModuleInformation();

    /// <summary>
    /// Returns the types of all dependent library modules.
    /// </summary>
    /// <returns>A task that returns an enumerable of types.</returns>
    public Task<IEnumerable<Type>> GetLibraryModulesTypesAsync()
    {
        return Task.FromResult(_libraryModuleDependencies.GetLibraryModulesTypesAsync());
    }

    /// <summary>
    /// Configures the module's configuration. Override to provide custom configuration logic.
    /// </summary>
    /// <param name="configurationManager">The configuration manager.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task by default.</returns>
    public virtual Task ConfigureConfigurationAsync(
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Configures the module's services. Override to register services in the DI container.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configurationManager">The configuration manager.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task by default.</returns>
    public virtual Task ConfigureServicesAsync(
        IServiceCollection serviceCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Configures the dependencies of this module.
    /// </summary>
    /// <param name="libraryModuleLoader">The module loader.</param>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="configurationManager">The configuration manager.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task ConfigureDependenciesAsync(
        ILibraryModuleLoader libraryModuleLoader,
        IServiceCollection serviceCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        await _libraryModuleDependencies.LoadAsync(
            libraryModuleLoader,
            serviceCollection,
            configurationManager,
            hostEnvironment,
            cancellationToken);
    }

    /// <summary>
    /// Registers core services required for library modules in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ILibraryModuleLoader, LibraryModuleLoader>();
        services.AddScoped<ILibraryModuleConfigurator, LibraryModuleConfigurator>();
    }

    /// <summary>
    /// Returns the types of all classes in this module.
    /// Not implemented by default; must be overridden in derived classes.
    /// </summary>
    /// <returns>A task that returns an enumerable of types.</returns>
    public Task<IEnumerable<Type>> GetClassModulesTypesAsync()
    {
        return Task.FromResult(_libraryModuleDependencies.GetLibraryModulesTypesAsync());
    }

    /// <summary>
    /// Gets the unique identifier of the module. Must be implemented by derived classes.
    /// </summary>
    /// <returns>The module identifier.</returns>
    protected abstract ModuleId GetModuleId();

    /// <summary>
    /// Gets the version of the module. Must be implemented by derived classes.
    /// </summary>
    /// <returns>The module version.</returns>
    protected abstract IModuleVersion GetModuleVersion();

    /// <summary>
    /// Gets the information of the module. Must be implemented by derived classes.
    /// </summary>
    /// <returns></returns>
    protected abstract IModuleInformation GetModuleInformation();
}
