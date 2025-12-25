namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Builder that counts validation calls for concurrency testing.
/// </summary>
public class CountingValidationBuilder : AbstractBuilder<SimpleObject>
{
    private readonly Action _onValidate;
    
    public CountingValidationBuilder(Action onValidate) => _onValidate = onValidate;
    
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        Thread.Sleep(50); // Simulate slow validation
        _onValidate();
    }
}
