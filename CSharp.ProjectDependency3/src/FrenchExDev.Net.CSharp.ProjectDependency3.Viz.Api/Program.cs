using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            ;
    });
});

var app = builder.Build();

// Add a simple root endpoint for testing
app.MapGet("/", () => Results.Ok(new { 
    status = "Running", 
    message = "Viz API is operational",
    timestamp = DateTime.UtcNow 
}));

// Map controllers BEFORE MapDefaultEndpoints
app.MapControllers();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseWebSockets();

await app.RunAsync();