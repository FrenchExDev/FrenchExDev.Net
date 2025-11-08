using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Graph;

public class SimpleClassForGraph
{
    public string Name { get; }
    public SimpleClassForGraph? Other { get; set; }
    public SimpleClassForGraph(string name) { Name = name; }
}

public class SimpleGraphBuilder : AbstractBuilder<SimpleClassForGraph>
{
    private string _value = string.Empty;
    private SimpleGraphBuilder? _other;

    public SimpleGraphBuilder WithValue(string v) { _value = v; return this; }
    public SimpleGraphBuilder Other(Action<SimpleGraphBuilder> body) { var b = new SimpleGraphBuilder(); body(b); _other = b; return this; }

    protected override SimpleClassForGraph Instantiate() => new SimpleClassForGraph(_value);

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        _other?.Build(visitedCollector);
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures) { }
}
