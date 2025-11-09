using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace FrenchExDev.Net.CSharp.Aspire.Dev.WebApplication;

/// <summary>
/// Provides extension methods for configuring application behavior related to server certificates and HTTPS setup.
/// </summary>
/// <remarks>This static class contains methods that assist in setting up certificate-based HTTPS for ASP.NET Core
/// applications. The methods are designed to work with configuration-driven certificate paths and integrate with
/// Kestrel server options without interfering with endpoint configuration managed by external tools or environment
/// variables.</remarks>
public static class CodeExtensions
{
    /// <summary>
    /// Configures the application's Kestrel server to use a certificate for HTTPS if certificate and key paths are
    /// specified in the configuration.
    /// </summary>
    /// <remarks>This method sets up the server certificate for HTTPS using the paths specified in the
    /// 'Kestrel:Certificates:Default:Path' and 'Kestrel:Certificates:Default:KeyPath' configuration values. It does not
    /// modify endpoint configuration, allowing external tools or environment variables such as Aspire or
    /// ASPNETCORE_URLS to control endpoints. If the certificate or key files are missing or the configuration values
    /// are not set, no changes are made.</remarks>
    /// <param name="app">The WebApplicationBuilder instance to configure for certificate-based HTTPS.</param>
    public static void EnsureCertificateSetup(this WebApplicationBuilder app)
    {
        var certPath = app.Configuration["Kestrel:Certificates:Default:Path"];
        var keyPath = app.Configuration["Kestrel:Certificates:Default:KeyPath"];

        // Only configure the certificate, let Aspire/ASPNETCORE_URLS handle the endpoints
        if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(keyPath) || !File.Exists(certPath) || !File.Exists(keyPath))
        {
            return;
        }

        app.WebHost.ConfigureKestrel(serverOptions =>
        {
            // ? Configure HTTPS defaults without overriding Aspire's endpoint configuration
            serverOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                var pemCert = X509Certificate2.CreateFromPemFile(certPath, keyPath);
                var pfxBytes = pemCert.Export(X509ContentType.Pfx);
                var cert = new X509Certificate2(
                 pfxBytes,
                 (string?)null,
                 X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);
                httpsOptions.ServerCertificate = cert;
                httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
            });
        });
    }
}
