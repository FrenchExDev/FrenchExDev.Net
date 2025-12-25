namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Builder that simulates circular reference during build.
/// </summary>
public class CircularRefTestBuilder : AbstractBuilder<SimpleObject>
{
    public string? Name { get; set; }
    
    protected override SimpleObject Instantiate() => new() { Value = Name ?? string.Empty };
    
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // Try to build self again - simulates circular reference
        Build(visitedCollector);
    }
    
    public CircularRefTestBuilder WithName(string name) { Name = name; return this; }
}
