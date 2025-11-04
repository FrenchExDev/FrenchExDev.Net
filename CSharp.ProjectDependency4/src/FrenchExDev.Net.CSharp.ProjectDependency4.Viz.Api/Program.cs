using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var certPath = builder.Configuration["Kestrel:Certificates:Default:Path"];
var keyPath = builder.Configuration["Kestrel:Certificates:Default:KeyPath"];

if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath) && File.Exists(certPath) && File.Exists(keyPath))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
            httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
        });
    });
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();

await app.RunAsync();

