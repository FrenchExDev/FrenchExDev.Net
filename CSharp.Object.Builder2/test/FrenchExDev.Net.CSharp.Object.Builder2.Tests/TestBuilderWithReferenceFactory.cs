namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Builder that uses the single-parameter constructor with reference factory.
/// </summary>
public class TestBuilderWithReferenceFactory : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    
    public TestBuilderWithReferenceFactory(IReferenceFactory referenceFactory)
        : base(referenceFactory) { }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
}
