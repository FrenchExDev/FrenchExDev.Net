using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
await app.RunAsync();

[ApiController]
public class IndexController : ControllerBase
{
    [HttpGet("/")] public IActionResult Index() => Ok("hello");
}