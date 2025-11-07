using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var certPath = builder.Configuration["Kestrel:Certificates:Default:Path"];
var keyPath = builder.Configuration["Kestrel:Certificates:Default:KeyPath"];

// Only configure the certificate, let Aspire/ASPNETCORE_URLS handle the endpoints
if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath) && File.Exists(certPath) && File.Exists(keyPath))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        // ? Configure HTTPS defaults without overriding Aspire's endpoint configuration
        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            httpsOptions.ServerCertificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
            httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
        });
    });
}

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();

[Controller]
public class TestController : ControllerBase
{
    [Route("/test")]
    public string Get() => "Hello from ProjectDependency4.Viz!";
}