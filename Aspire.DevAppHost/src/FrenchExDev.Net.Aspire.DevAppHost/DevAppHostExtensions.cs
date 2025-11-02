using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.Aspire.DevAppHost;

/// <summary>
/// Provides extension methods for configuring and setting up development application hosting scenarios using
/// DevAppHost2, including logger creation, DNS configuration, and resource setup for distributed applications.
/// </summary>
/// <remarks>These extension methods are intended to simplify the setup of development environments for
/// distributed applications by providing streamlined configuration of logging, DNS, and HTTPS endpoints. The methods
/// are designed for use with the distributed application builder and resource builder interfaces, enabling consistent
/// and repeatable development setups.</remarks>
public static class DevAppHost2Extensions
{
    /// <summary>
    /// Ensures that the distributed application builder is configured for development scenarios.
    /// </summary>
    /// <param name="builder">The distributed application builder to configure for development.</param>
    /// <param name="appHostLogger">The logger used to record application host events during setup.</param>
    /// <returns>The same distributed application builder instance, configured for development use.</returns>
    public static IDistributedApplicationBuilder EnsureDevSetup2(this IDistributedApplicationBuilder builder, ILogger appHostLogger)
    {
        var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? DnsConfiguration.Default();
        return builder.EnsureSetup2(dnsConfig, appHostLogger);
    }

    /// <summary>
    /// Ensures that the development application host is configured for the distributed application builder, adding it
    /// if not already present.
    /// </summary>
    /// <remarks>If the development application host has already been added to the builder's services, this
    /// method performs no action and returns the builder unchanged.</remarks>
    /// <param name="builder">The distributed application builder to configure.</param>
    /// <param name="dnsConfiguration">The DNS configuration to use for the development application host setup.</param>
    /// <param name="logger">The logger used to record diagnostic and setup information.</param>
    /// <param name="forceCertificateRegeneration">true to force regeneration of development certificates during setup; otherwise, false.</param>
    /// <returns>The same distributed application builder instance, with the development application host ensured.</returns>
    public static IDistributedApplicationBuilder EnsureSetup2(this IDistributedApplicationBuilder builder, DnsConfiguration dnsConfiguration, ILogger logger, bool forceCertificateRegeneration = false)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IDevAppHost2)))
        {
            return builder;
        }
        new DevAppHost2(builder, logger).EnsureSetup2();
        return builder;
    }

    public static IResourceBuilder<T> WithDevSetup2<T>(this IResourceBuilder<T> builder, string url, int port, DnsConfiguration dnsConfiguration) where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        builder
            .WithHttpsEndpoint(port: port, name: "https")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", dnsConfiguration.CertPathOrDie())
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__KeyPath", dnsConfiguration.KeyPathOrDie())
            .WithUrl(url);

        return builder;
    }
}

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

        var da = new DevAppHost(logger);
        builder.Services.AddSingleton<IDevAppHost>(da);
        da.EnsureSetup(dnsConfiguration, forceCertificateRegeneration);
        return builder;
    }
    public static IResourceBuilder<T> WithDevSetup<T>(this IResourceBuilder<T> builder, string name, DnsConfiguration dnsConfiguration) where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        builder
            .WithHttpsEndpoint(port: dnsConfiguration.Domains.First(x => x.Value.Domain == name).Value.Port, name: "https")
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

