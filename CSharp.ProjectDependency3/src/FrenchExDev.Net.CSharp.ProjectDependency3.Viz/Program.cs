using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var registryApiUrl = builder.Configuration["RegistryApiUrl"] ?? throw new InvalidDataException("Missing RegistryApiurl");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Per-user session & local storage
        builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddBlazoredLocalStorage();

        // App services with Registry API URL for service discovery
        builder.Services.AddScoped<Services.ISessionManager, Services.SessionManager>();
        builder.Services.AddScoped<Services.IAnalysisService>(sp =>
            new Services.AnalysisService(
                sp.GetRequiredService<Services.ISessionManager>(),
                sp.GetRequiredService<ILocalStorageService>(),
                registryApiUrl));

        await builder.Build().RunAsync();
    }
}
