namespace FrenchExDev.Net.Aspire.DevAppHost;

public record PortConfiguration
{
    public int Api { get; init; } = 5060;
    public int Viz { get; init; } = 5070;
    public int Orchestrator { get; init; } = 5080;
    public int WorkerBase { get; init; } = 5090;
    public int Dashboard { get; init; } = 18888;
}

