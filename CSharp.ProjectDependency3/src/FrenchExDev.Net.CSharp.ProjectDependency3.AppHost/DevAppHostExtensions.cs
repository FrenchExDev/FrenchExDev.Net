using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;
using Microsoft.Extensions.DependencyInjection;

public static class DevAppHostExtensions
{
    public static IDistributedApplicationBuilder EnsureSetup(this IDistributedApplicationBuilder builder, DnsConfiguration dnsConfiguration)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(DevAppHost)))
        {
            return builder;
        }

        var da = new DevAppHost();
        builder.Services.AddSingleton<IDevAppHost>(da);
        da.EnsureSetup(dnsConfiguration);
        return builder;
    }
}

