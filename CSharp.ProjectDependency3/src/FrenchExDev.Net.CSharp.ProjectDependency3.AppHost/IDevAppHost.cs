using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;

public interface IDevAppHost
{
    void EnsureMkcertSetup(DnsConfiguration config);
    void EnsureMkcertSetup(DnsConfiguration config, bool force);
    void EnsureSetup(DnsConfiguration dnsConfig);
    void EnsureSetup(DnsConfiguration dnsConfig, bool forceCertificateRegeneration);
    bool ExecuteCommand(string command, string arguments, out string output);
    string GetCertificatePath(DnsConfiguration config);
    string GetKeyPath(DnsConfiguration config);
    string GetConfigurationFilePath(DnsConfiguration config);
    bool IsRunningAsAdministrator();
    bool NeedsHostsFileUpdate(DnsConfiguration config);
    bool NeedsCertificateRegeneration(DnsConfiguration config);
    bool RestartAsAdministrator();
    void SaveConfiguration(DnsConfiguration config);
    DnsConfiguration? LoadSavedConfiguration(DnsConfiguration config);
    void UpdateHostsFile(DnsConfiguration config);
}