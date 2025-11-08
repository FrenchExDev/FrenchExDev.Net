using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Coverage;

public class Additional_Coverage2_Tests
{
    [Fact]
    public void Given_ReferenceList_When_ContainsReferenceAndIndexerSet_Then_Behaves()
    {
        var rl = new ReferenceList<string>();
        var r = new Reference<string>().Resolve("one");
        rl.Add(r);
        rl.Contains(r).ShouldBeTrue();

        // indexer setter
        rl[0] = "two";
        rl[0].ShouldBe("two");
    }

    [Fact]
    public void Given_Visited_WithSameBuilderNotValidated_When_Build_Then_Returns_UnresolvedReference()
    {
        var b = new SimpleBuilder().Value("v");
        var visited = new VisitedObjectDictionary();
        // simulate a visited entry with the same builder in NotValidated state
        visited[b.Id] = b;

        var result = b.Build(visited);
        result.ShouldBeAssignableTo<Reference<string>>();
        var reference = (Reference<string>)result;
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Given_FailingBuilderInList_When_BuildFailures_Then_ReturnsFailures()
    {
        var list = new BuilderList<string, FailBuilder>();
        list.New(b => { });
        var fails = list.BuildFailures();
        fails.Count.ShouldBe(1);
        fails[0].Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Given_ValidateCalledTwice_When_NoFailures_Then_NoDuplicates()
    {
        var b = new SimpleBuilder();
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        b.Validate(visited, failures);
        failures.Count.ShouldBe(0);

        // second call should be no-op
        b.Validate(visited, failures);
        failures.Count.ShouldBe(0);
    }
}
