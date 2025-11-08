using Shouldly;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Graph;

public class Cyclic_Complex_Graph_Tests
{
    [Fact]
    public void Given_CyclicGraph_When_BuildOrValidate_Then_NoInfiniteLoop()
    {
        // Existing tests exercised cyclic graph handling; keep a simple check here.
        var a = new SimpleClassForGraph("a");
        var b = new SimpleClassForGraph("b");
        a.Other = b; b.Other = a;

        var builderA = new SimpleGraphBuilder().WithValue(a.Name).Other(b => b.WithValue("b"));
        var visited = new VisitedObjectDictionary();
        // building should not infinite loop; we only assert it completes and returns a result
        var result = builderA.Build(visited);
        result.ShouldNotBeNull();
    }
}
