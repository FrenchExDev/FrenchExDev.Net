namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;

public record OrchestratorRegistration(
    string Id,
    string Url,
    DateTime RegisteredAt,
    DateTime LastHeartbeat,
    OrchestratorStatus Status);

public enum OrchestratorStatus
{
    Starting,
    Running,
    Stopping,
    Stopped,
    Failed
}

public record AgentRegistration(
    string Id,
  string OrchestratorId,
    string Url,
    DateTime RegisteredAt,
    DateTime LastHeartbeat,
    AgentStatus Status);

public enum AgentStatus
{
    Starting,
    Idle,
    Busy,
    Stopping,
    Stopped,
    Failed
}

public record RegisterOrchestratorRequest(string Url);
public record RegisterAgentRequest(string OrchestratorId, string Url);
public record HeartbeatRequest(string Id);
