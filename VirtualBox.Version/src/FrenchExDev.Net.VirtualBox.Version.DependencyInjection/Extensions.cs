using FrenchexDev.VirtualBox.Net;
using Microsoft.Extensions.DependencyInjection;

namespace FrenchExDev.Net.VirtualBox.Version.DependencyInjection;

/// <summary>
/// Provides extension methods for registering VirtualBox version discovery and information services with the dependency
/// injection container.
/// </summary>
/// <remarks>This class contains extension methods for IServiceCollection that add services related to VirtualBox
/// system version discovery and information searching. Use these methods to enable VirtualBox version-related
/// functionality in your application's dependency injection setup.</remarks>
public static class Extensions
{
    public static IServiceCollection AddVirtualBoxVersion(this IServiceCollection services)
    {
        services.AddTransient<IVirtualBoxSystemVersionDiscoverer, VirtualBoxSystemVersionDiscoverer>();
        services.AddTransient<IVirtualBoxVersionInformationSearcher, VirtualBoxVersionInformationSearcher>();
        return services;
    }
}
