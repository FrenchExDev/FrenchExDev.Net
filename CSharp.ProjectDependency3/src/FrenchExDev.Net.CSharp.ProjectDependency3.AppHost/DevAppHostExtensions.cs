using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class DevAppHostExtensions
{
    public static IDistributedApplicationBuilder EnsureSetup(this IDistributedApplicationBuilder builder, DnsConfiguration dnsConfiguration, ILogger logger)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(DevAppHost)))
        {
            return builder;
        }

        var da = new DevAppHost(logger);
        builder.Services.AddSingleton<IDevAppHost>(da);
        da.EnsureSetup(dnsConfiguration);
        return builder;
    }
}

