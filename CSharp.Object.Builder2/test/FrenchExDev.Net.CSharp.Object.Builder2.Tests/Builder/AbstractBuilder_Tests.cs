using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Builder;

public class AbstractBuilder_Tests
{
    [Fact]
    public void Given_BuilderWithExisting_When_Build_Then_ReturnsSuccessResult()
    {
        var pb = new PublicBuilder();
        pb.Existing("exists");
        var r = pb.Build();
        r.IsSuccess<string>().ShouldBeTrue();
        r.Success<string>().ShouldBe("exists");
    }

    [Fact]
    public void Given_FailingBuilder_When_BuildSuccess_Then_ThrowsAggregateException()
    {
        var fb = new FailingBuilder();
        Should.Throw<AggregateException>(() => fb.BuildSuccess());
    }

    [Fact]
    public void Given_VisitedContainsSameBuilder_NotValidated_When_Build_Then_Returns_UnresolvedReference()
    {
        var b = new SimpleBuilder().Value("v");
        var visited = new VisitedObjectDictionary();
        visited[b.Id] = b;
        var result = b.Build(visited);
        result.ShouldBeAssignableTo<Reference<string>>();
        var reference = (Reference<string>)result;
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Given_ValidateCalledTwice_When_NoFailures_Then_NoDuplicates()
    {
        var b = new SimpleBuilder();
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        b.Validate(visited, failures);
        failures.Count.ShouldBe(0);
        b.Validate(visited, failures);
        failures.Count.ShouldBe(0);
    }
}
