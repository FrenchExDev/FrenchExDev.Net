using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FrenchExDev.Net.Mm.Module.Library;

public class HostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = "UnitTest";
    public string ApplicationName { get; set; } = "UnitTest";
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(AppContext.BaseDirectory);
}
