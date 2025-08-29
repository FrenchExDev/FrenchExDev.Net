using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FrenchExDev.Net.Mm.Module.Library.Infrastructure;

/// <summary>
/// Logic to configure MediatR for multiple <see cref="ILibraryModule"/>s.
/// </summary>
public class LibraryModuleMediatorConfigurator : ILibraryModuleMediatorConfigurator
{
    /// <summary>
    /// Semaphore to ensure thread-safe configuration.
    /// </summary>
    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    /// <summary>
    /// Indicates whether the configuration has already been applied.
    /// </summary>
    /// <remarks>This field is used internally to track if the configuration process has been
    /// completed.</remarks>
    private bool _alreadyConfigured;

    /// <summary>
    /// Configures MediatR services for the specified service collection using the provided library modules.
    /// </summary>
    /// <remarks>This method ensures that MediatR services are configured only once, even if called multiple
    /// times concurrently.</remarks>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to which MediatR services will be added.</param>
    /// <param name="LibraryModules">A collection of library modules whose assemblies will be scanned to register MediatR handlers.</param>
    /// <returns></returns>
    public async Task ConfigureMediatorServicesAsync(
        IServiceCollection serviceCollection,
        IList<ILibraryModule> LibraryModules
    )
    {
        if (_alreadyConfigured) return;

        await _semaphoreSlim.WaitAsync();

        serviceCollection.AddMediatR(configuration =>
        {
            foreach (var type in LibraryModules) configuration.RegisterServicesFromAssemblyContaining(type.GetType());
        });

        _alreadyConfigured = true;

        _semaphoreSlim.Release();
    }
}