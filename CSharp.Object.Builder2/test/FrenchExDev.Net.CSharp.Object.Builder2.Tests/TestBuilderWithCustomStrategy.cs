namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Builder that allows setting custom reference factory and sync strategy for testing constructors.
/// </summary>
public class TestBuilderWithCustomStrategy : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    
    public TestBuilderWithCustomStrategy(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy)
        : base(referenceFactory, syncStrategy) { }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
}
