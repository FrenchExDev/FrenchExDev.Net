using System.Text.Json;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;

public record DnsConfiguration
{
    public string? VizHost { get; init; }
    public string? CertPath { get; set; }
    public string? KeyPath { get; set; }
    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public string VizHostOrDie() => VizHost ?? throw new InvalidOperationException("VizPath is not initialized.");
    public string Domain { get; init; } = "pd3i1.com";
    public string GatewayHost { get; init; } = "gateway";
    public string OrchestratorHost { get; init; } = "orchestrator";
    public string WorkerHostTemplate { get; init; } = "worker-agent";
    public string DashboardHost { get; init; } = "devdash";
    public string ApiHost { get; init; } = "api";
    public PortConfiguration Ports { get; init; } = new();
    public int WorkerCount { get; init; } = 1;
    public bool EnableHttps { get; init; } = true;
    public string CertificatesDirectory { get; init; } = ".aspire-certs";
    public string GetGatewayFqdn() => $"{GatewayHost}.{Domain}";
    public string GetOrchestratorFqdn() => $"{OrchestratorHost}.{Domain}";
    public string GetWorkerFqdn(int index) => $"{WorkerHostTemplate}-{index}.{Domain}";
    public string GetDashboardFqdn() => $"{DashboardHost}.{Domain}";
    public string GetVizFqdn() => $"{VizHost}.{Domain}";
    public string GetApiHostFqdn() => $"{ApiHost}.{Domain}";
    public string GetGatewayUrl() => $"{(EnableHttps ? "https" : "http")}://{GetGatewayFqdn()}:{Ports.Gateway}";
    public string GetOrchestratorUrl() => $"{(EnableHttps ? "https" : "http")}://{GetOrchestratorFqdn()}:{Ports.Orchestrator}";
    public string GetWorkerUrl(int index) => $"{(EnableHttps ? "https" : "http")}://{GetWorkerFqdn(index)}:{Ports.WorkerBase + index - 1}";
    public string GetDashboardUrl() => $"{(EnableHttps ? "https" : "http")}://{GetDashboardFqdn()}:{Ports.Dashboard}";
    public string GetVizUrl() => $"{(EnableHttps ? "https" : "http")}://{GetVizFqdn()}:{Ports.Viz}";
    public string GetApiHostUrl() => $"{(EnableHttps ? "https" : "http")}://{GetApiHostFqdn()}:{Ports.Api}";

    public IEnumerable<string> GetAllHosts()
    {
        yield return GetGatewayFqdn();
        yield return GetVizFqdn();
        yield return GetOrchestratorFqdn();
        yield return GetDashboardFqdn();
        yield return GetApiHostFqdn();
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

    /// <summary>
    /// Calculates a hash of the DNS configuration to detect changes.
    /// Uses the record's built-in GetHashCode for consistent structural equality.
    /// Excludes CertPath and KeyPath as they are mutable and not part of configuration identity.
    /// </summary>
    public string CalculateConfigurationHash()
    {
        // Create a copy with nulled certificate paths for consistent hashing
        var configForHashing = this with { CertPath = null, KeyPath = null };

        // Use the record's built-in GetHashCode which considers all init properties
        var hashCode = configForHashing.GetHashCode();

        // Convert to hex string for readability and storage
        return hashCode.ToString("X8");
    }

    /// <summary>
    /// Serializes the configuration to JSON
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Deserializes a configuration from JSON
    /// </summary>
    public static DnsConfiguration? FromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<DnsConfiguration>(json);
        }
        catch
        {
            return null;
        }
    }
}

public record PortConfiguration
{
    public int Gateway { get; init; } = 443;
    public int Api { get; init; } = 5060;
    public int Viz { get; init; } = 5070;
    public int Orchestrator { get; init; } = 5080;
    public int WorkerBase { get; init; } = 5090;
    public int Dashboard { get; init; } = 18888;
}
