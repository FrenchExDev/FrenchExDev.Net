namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator;

public record AgentInfo(string Id, Uri Url, DateTimeOffset RegisteredAt, DateTimeOffset LastHeartbeat, string Status);
