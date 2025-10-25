using FrenchExDev.Net.CSharp.ProjectDependency3.AppHost;

public interface IDevAppHost
{
    void EnsureMkcertSetup(DnsConfiguration config);
    void EnsureSetup(DnsConfiguration dnsConfig);
    bool ExecuteCommand(string command, string arguments, out string output);
    string GetCertificatePath(DnsConfiguration config);
    string GetKeyPath(DnsConfiguration config);
    bool IsRunningAsAdministrator();
    bool NeedsHostsFileUpdate(DnsConfiguration config);
    bool RestartAsAdministrator();
    void UpdateHostsFile(DnsConfiguration config);
}