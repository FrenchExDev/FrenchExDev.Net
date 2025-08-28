using FrenchExDev.Mm.Net.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Mm.Net.Module.Library.Abstractions;

/// <summary>
/// Represents a library module in a modular monolith application.
/// Provides mechanisms for configuration, dependency management, and service registration.
/// </summary>
/// <remarks>
/// A library module is a class library that can use MediatR, Microsoft Dependency Injection, and Microsoft Configuration
/// to help create a module for a modular monolith app.
/// </remarks>
public interface ILibraryModule : IModule
{
    /// <summary>
    /// Gets all types from this library module.
    /// Typically used to register MediatR handlers or other types for reflection-based registration.
    /// </summary>
    /// <returns>A task that returns an enumerable of types defined in the module.</returns>
    Task<IEnumerable<Type>> GetClassModulesTypesAsync();

    /// <summary>
    /// Configures dependencies for this module.
    /// Called to allow the module to register its dependencies or request other modules to be loaded.
    /// </summary>
    /// <param name="libraryModuleLoader">The module loader responsible for managing module lifecycles.</param>
    /// <param name="configurationManager">The configuration manager for accessing configuration settings.</param>
    /// <param name="hostEnvironment">The current host environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConfigureDependenciesAsync(
        ILibraryModuleLoader libraryModuleLoader,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Configures the module's configuration.
    /// Called to allow the module to set up or validate its configuration settings.
    /// </summary>
    /// <param name="configurationManager">The configuration manager for accessing configuration settings.</param>
    /// <param name="hostEnvironment">The current host environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConfigureConfigurationAsync(
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Configures services for this module.
    /// Called to allow the module to register its services with the dependency injection container.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add services to.</param>
    /// <param name="configurationManager">The configuration manager for accessing configuration settings.</param>
    /// <param name="hostEnvironment">The current host environment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConfigureServicesAsync(
        IServiceCollection serviceCollection,
        IConfigurationManager configurationManager,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    );
}
