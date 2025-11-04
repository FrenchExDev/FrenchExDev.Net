using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.Aspire.DevAppHost;

public static class DevAppHostExtensions
{
    public static IDistributedApplicationBuilder EnsureSetup(this IDistributedApplicationBuilder builder, ILogger logger)
    {
        var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? DnsConfiguration.Default();

        return builder.EnsureSetup(dnsConfig, logger);
    }

    public static IDistributedApplicationBuilder EnsureSetup(this IDistributedApplicationBuilder builder, DnsConfiguration dnsConfiguration, ILogger logger, bool forceCertificateRegeneration = false)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(DevAppHost)))
        {
            return builder;
        }

        var da = new DevAppHost2(builder, logger);
        builder.Services.AddSingleton<IDevAppHost2>(da);
        da.EnsureSetup2();
        return builder;
    }
    public static IResourceBuilder<T> WithDevSetup<T>(this IResourceBuilder<T> builder, string name, DnsConfiguration dnsConfiguration) where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        builder
            .WithHttpsEndpoint(port: dnsConfiguration.Subdomains.First(x => x.Value.Domain == name).Value.Port, name: "https")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfiguration.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfiguration.KeyPathOrDie())
            .WithUrl(name);

        return builder;
    }
    public static IResourceBuilder<T> WithDevSetup<T>(this IResourceBuilder<T> builder, string url, int port, DnsConfiguration dnsConfiguration) where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        builder
            .WithHttpsEndpoint(port: port, name: "https")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfiguration.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfiguration.KeyPathOrDie())
            .WithUrl(url);

        return builder;
    }
}

