using FrenchExDev.Net.CSharp.Aspire.Dev.WebApplication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.EnsureCertificateSetup();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();


[ApiController]
public class IndexController : ControllerBase
{
    [HttpGet("/")] public IActionResult Index() => Ok("hello");
}