namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Builder that exposes CreateFailureCollector for testing.
/// </summary>
public class FailureCollectorTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    public FailuresDictionary TestCreateFailureCollector() => CreateFailureCollector();
}
