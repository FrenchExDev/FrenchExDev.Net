using System.Collections.Concurrent;
using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;

public interface IOrchestratorRegistry
{
    string Register(string url);
    bool Unregister(string id);
    void UpdateHeartbeat(string id);
    void UpdateStatus(string id, OrchestratorStatus status);
    OrchestratorRegistration? Get(string id);
    IReadOnlyCollection<OrchestratorRegistration> GetAll();
}

public class OrchestratorRegistry : IOrchestratorRegistry
{
    private readonly ConcurrentDictionary<string, OrchestratorRegistration> _orchestrators = new();
    private readonly ILogger<OrchestratorRegistry> _logger;

    public event EventHandler<OrchestratorRegistration>? OrchestratorRegistered;
    public event EventHandler<OrchestratorRegistration>? OrchestratorUnregistered;
    public event EventHandler<OrchestratorRegistration>? OrchestratorStatusChanged;

    public OrchestratorRegistry(ILogger<OrchestratorRegistry> logger)
    {
        _logger = logger;
    }

    public string Register(string url)
    {
        var id = Guid.NewGuid().ToString("N");
        var registration = new OrchestratorRegistration(
 id,
   url,
    DateTime.UtcNow,
            DateTime.UtcNow,
   OrchestratorStatus.Starting);

        _orchestrators[id] = registration;
    _logger.LogInformation("Orchestrator {Id} registered at {Url}", id, url);
        OrchestratorRegistered?.Invoke(this, registration);
        return id;
    }

    public bool Unregister(string id)
    {
        if (_orchestrators.TryRemove(id, out var registration))
 {
            _logger.LogInformation("Orchestrator {Id} unregistered", id);
            OrchestratorUnregistered?.Invoke(this, registration);
       return true;
        }
 return false;
    }

    public void UpdateHeartbeat(string id)
    {
        if (_orchestrators.TryGetValue(id, out var existing))
        {
            var updated = existing with { LastHeartbeat = DateTime.UtcNow };
            _orchestrators[id] = updated;
     }
    }

    public void UpdateStatus(string id, OrchestratorStatus status)
    {
      if (_orchestrators.TryGetValue(id, out var existing))
   {
 var updated = existing with { Status = status, LastHeartbeat = DateTime.UtcNow };
 _orchestrators[id] = updated;
            _logger.LogInformation("Orchestrator {Id} status changed to {Status}", id, status);
    OrchestratorStatusChanged?.Invoke(this, updated);
        }
 }

    public OrchestratorRegistration? Get(string id)
    {
        _orchestrators.TryGetValue(id, out var registration);
        return registration;
    }

    public IReadOnlyCollection<OrchestratorRegistration> GetAll()
    {
      return _orchestrators.Values.ToList();
    }
}
