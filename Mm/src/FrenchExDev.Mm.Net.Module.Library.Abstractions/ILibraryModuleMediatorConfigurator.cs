using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FrenchExDev.Mm.Net.Module.Library.Abstractions;

/// <summary>
/// Defines a contract for configuring mediator services for a collection of library modules.
/// </summary>
/// <remarks>This interface is intended to be implemented by classes that configure mediator-related services for
/// a set of library modules. Implementations should ensure that the provided service collection is updated with the
/// necessary mediator services to enable communication between the modules.</remarks>
public interface ILibraryModuleMediatorConfigurator
{
    /// <summary>
    /// Configures mediator services by registering the necessary dependencies for the specified library modules.
    /// </summary>
    /// <param name="serviceCollection">The collection of service descriptors to which the mediator services will be added.</param>
    /// <param name="libraryModule">A list of library modules that define the mediator-related configurations to be applied.</param>
    /// <returns>A task that represents the asynchronous operation of configuring mediator services.</returns>
    Task ConfigureMediatorServicesAsync(
        IServiceCollection serviceCollection,
        IList<ILibraryModule> libraryModule
    );
}