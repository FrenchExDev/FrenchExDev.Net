using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.VirtualBox.Version.Mm;

/// <summary>
/// Provides module integration for VirtualBox version discovery and information services within the application
/// framework.
/// </summary>
/// <remarks>This module registers services for discovering VirtualBox system versions and searching for version
/// information. It is intended to be used as part of the application's modular architecture, enabling
/// VirtualBox-related functionality through dependency injection.</remarks>
public class VirtualBoxVersionModule : LibraryModule
{
    /// <summary>
    /// Retrieves the unique identifier for the associated module.
    /// </summary>
    /// <returns>A <see cref="ModuleId"/> representing the unique identifier of the module.</returns>
    protected override ModuleId GetModuleId() => new ModuleId(Guid.Parse("29316fe1-c828-44f9-b88d-154fa2de2cc7"));

    /// <summary>
    /// Retrieves information about the VirtualBox version module.
    /// </summary>
    /// <returns>An <see cref="IModuleInformation"/> instance containing details about the VirtualBox version module. The
    /// returned object indicates a successful information retrieval.</returns>
    protected override IModuleInformation GetModuleInformation() => new BasicModuleInformationBuilder()
        .DisplayName("VirtualBox Version Module")
        .BuildSuccess()
        ;

    /// <summary>
    /// Retrieves the version information for the module using a major, minor, and patch format.
    /// </summary>
    /// <returns>An <see cref="IModuleVersion"/> instance representing version 1.0.0 of the module.</returns>
    protected override IModuleVersion GetModuleVersion() => new MajorMinorPatchModuleVersion(1, 0, 0);

    /// <summary>
    /// Configures services required for VirtualBox version discovery and information searching within the application's
    /// dependency injection container.
    /// </summary>
    /// <remarks>This method registers services for VirtualBox system version discovery and version
    /// information searching as transient dependencies. It should be called during application startup to ensure
    /// required services are available for VirtualBox integration.</remarks>
    /// <param name="serviceCollection">The service collection to which VirtualBox-related services will be added. Must not be null.</param>
    /// <param name="configurationManager">The configuration manager used to access application configuration settings. Must not be null.</param>
    /// <param name="hostEnvironment">The host environment that provides information about the application's runtime environment. Must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation. Optional.</param>
    /// <returns>A task that represents the asynchronous operation of configuring services.</returns>
    public override Task ConfigureServicesAsync(IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
    {
        serviceCollection
            .AddTransient<IVirtualBoxSystemVersionDiscoverer, VirtualBoxSystemVersionDiscoverer>()
            .AddTransient<IVirtualBoxVersionInformationSearcher, VirtualBoxVersionInformationSearcher>()
            ;

        return Task.CompletedTask;
    }
}
