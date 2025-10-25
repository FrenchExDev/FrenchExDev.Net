namespace FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;

public class DnsConfiguration
{
    public string VizHost { get; set; }
    public string CertPath { get; set; }
    public string KeyPath { get; set; }

    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public string VizHostOrDie() => VizHost ?? throw new InvalidOperationException("VizPath is not initialized.");
    public string Suffix { get; set; } = "pd3i1.com";
    public string OrchestratorHost { get; set; } = "orchestrator";
    public string WorkerHostPrefix { get; set; } = "worker-agent";
    public string DashboardHost { get; set; } = "dashboard";
    public PortConfiguration Ports { get; set; } = new();
    public int WorkerCount { get; set; } = 1;
    public bool EnableHttps { get; set; } = true;
    public string CertificatesDirectory { get; set; } = ".aspire-certs";

    public string GetOrchestratorFqdn() => $"{OrchestratorHost}.{Suffix}";
    public string GetWorkerFqdn(int index) => $"{WorkerHostPrefix}-{index}.{Suffix}";
    public string GetDashboardFqdn() => $"{DashboardHost}.{Suffix}";
    public string GetVizFqdn() => $"{VizHost}.{Suffix}";

    public string GetOrchestratorUrl() => $"{(EnableHttps ? "https" : "http")}://{GetOrchestratorFqdn()}:{Ports.Orchestrator}";
    public string GetWorkerUrl(int index) => $"{(EnableHttps ? "https" : "http")}://{GetWorkerFqdn(index)}:{Ports.WorkerBase + index - 1}";
    public string GetDashboardUrl() => $"{(EnableHttps ? "https" : "http")}://{GetDashboardFqdn()}:{Ports.Dashboard}";
    public string GetVizUrl() => $"{(EnableHttps ? "https" : "http")}://{GetVizFqdn()}:{Ports.Viz}";

    public IEnumerable<string> GetAllHosts()
    {
        yield return GetVizFqdn();
        yield return GetOrchestratorFqdn();
        yield return GetDashboardFqdn();
        for (int i = 1; i <= WorkerCount; i++)
        {
            yield return GetWorkerFqdn(i);
        }
    }

    public IEnumerable<string> GetHostsFileEntries()
    {
        foreach (var host in GetAllHosts())
        {
            yield return $"127.0.0.1 {host}";
        }
    }

}

public class PortConfiguration
{
    public int Viz { get; set; } = 5070;
    public int Orchestrator { get; set; } = 5080;
    public int WorkerBase { get; set; } = 5090;
    public int Dashboard { get; set; } = 18888;
}
