using FenchExDev.Net.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

[Feature(nameof(IBuilder<object>), TestKind.Unit, IntegrationKind.Isolated)]
[Trait(Internet.Test, Internet.Offline)]
[Trait(Kind.Test, Kind.Unit)]
public class ProtectedHelpers_Tests
{
    [Fact]
    public void Given_ProtectedAsserts_When_Invalid_Then_AddsFailures()
    {
        var pb = new PublicBuilder();
        var failures = new FailuresDictionary();

        pb.CallAssertNotEmptyOrWhitespace_String("", "n", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        pb.CallAssertNotEmptyOrWhitespace_List(new List<string> { " ", "ok" }, "lst", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        pb.CallAssertNotNullNotEmptyCollection<string>(null, "col", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        pb.CallAssert(() => true, "p", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);
    }

    [Fact]
    public void Given_Builder_When_GetResultNotBuilt_Then_ThrowsInvalidOperation()
    {
        var pb = new PublicBuilder();
        Should.Throw<InvalidOperationException>(() => pb.GetResult());
    }

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
    public void Given_BuilderList_When_CallBuildList_Then_BuildersAreInvoked()
    {
        var list = new BuilderList<string, PublicBuilder>();
        list.New(b => b.WithValue("a"));
        list.New(b => b.WithValue("b"));

        var caller = new PublicBuilder();
        var visited = new VisitedObjectDictionary();
        caller.CallBuildList(list, visited);

        // ensure builders built
        var built = list.BuildSuccess();
        built[0].ShouldBe("a");
        built[1].ShouldBe("b");
    }

    [Fact]
    public void Given_ValidateListInternal_When_SomeBuildersInvalid_Then_AggregatesFailures()
    {
        var list = new BuilderList<string, PublicBuilder>();
        list.New(b => b.MakeInvalid());
        var caller = new PublicBuilder();
        var failures = new FailuresDictionary();
        caller.CallValidateListInternal(list, "prop", new VisitedObjectDictionary(), failures);
        failures.Count.ShouldBeGreaterThan(0);
    }
}
