using System.Collections.Concurrent;
using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;

public interface IAgentRegistry
{
    string Register(string orchestratorId, string url);
    bool Unregister(string id);
    void UpdateHeartbeat(string id);
    void UpdateStatus(string id, AgentStatus status);
    AgentRegistration? Get(string id);
    IReadOnlyCollection<AgentRegistration> GetAll();
    IReadOnlyCollection<AgentRegistration> GetByOrchestrator(string orchestratorId);
}

public class AgentRegistry : IAgentRegistry
{
    private readonly ConcurrentDictionary<string, AgentRegistration> _agents = new();
    private readonly ILogger<AgentRegistry> _logger;

    public event EventHandler<AgentRegistration>? AgentRegistered;
    public event EventHandler<AgentRegistration>? AgentUnregistered;
    public event EventHandler<AgentRegistration>? AgentStatusChanged;

    public AgentRegistry(ILogger<AgentRegistry> logger)
    {
        _logger = logger;
    }

    public string Register(string orchestratorId, string url)
    {
        var id = Guid.NewGuid().ToString("N");
        var registration = new AgentRegistration(
            id,
            orchestratorId,
            url,
            DateTime.UtcNow,
            DateTime.UtcNow,
            AgentStatus.Starting);

        _agents[id] = registration;
        _logger.LogInformation("Agent {Id} registered for orchestrator {OrchestratorId} at {Url}", id, orchestratorId, url);
        AgentRegistered?.Invoke(this, registration);
        return id;
    }

    public bool Unregister(string id)
    {
        if (_agents.TryRemove(id, out var registration))
        {
            _logger.LogInformation("Agent {Id} unregistered", id);
            AgentUnregistered?.Invoke(this, registration);
            return true;
        }
        return false;
    }

    public void UpdateHeartbeat(string id)
    {
        if (_agents.TryGetValue(id, out var existing))
        {
            var updated = existing with { LastHeartbeat = DateTime.UtcNow };
            _agents[id] = updated;
        }
    }

    public void UpdateStatus(string id, AgentStatus status)
    {
        if (_agents.TryGetValue(id, out var existing))
        {
            var updated = existing with { Status = status, LastHeartbeat = DateTime.UtcNow };
            _agents[id] = updated;
            _logger.LogInformation("Agent {Id} status changed to {Status}", id, status);
            AgentStatusChanged?.Invoke(this, updated);
        }
    }

    public AgentRegistration? Get(string id)
    {
        _agents.TryGetValue(id, out var registration);
        return registration;
    }

    public IReadOnlyCollection<AgentRegistration> GetAll()
    {
        return _agents.Values.ToList();
    }

    public IReadOnlyCollection<AgentRegistration> GetByOrchestrator(string orchestratorId)
    {
        return _agents.Values.Where(a => a.OrchestratorId == orchestratorId).ToList();
    }
}
