using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;
using Blazored.LocalStorage;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Get orchestrator URL from environment or use default
        var orchestratorUrl = builder.Configuration["OrchestratorUrl"] ?? "http://localhost:5080";
        
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Per-user session & local storage
        builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddBlazoredLocalStorage();

        // App services with orchestrator URL
        builder.Services.AddScoped<Services.ISessionManager, Services.SessionManager>();
        builder.Services.AddScoped<Services.IAnalysisService>(sp => 
            new Services.AnalysisService(
                sp.GetRequiredService<Services.ISessionManager>(),
                sp.GetRequiredService<ILocalStorageService>(),
                orchestratorUrl));

        await builder.Build().RunAsync();
    }
}
