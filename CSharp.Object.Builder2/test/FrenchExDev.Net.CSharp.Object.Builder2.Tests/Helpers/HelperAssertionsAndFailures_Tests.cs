using Shouldly;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Helpers;

public class HelperAssertionsAndFailures_Tests
{
    [Fact]
    public void Given_HelperBuilder_NullInputs_When_InvokeAssertHelpers_Then_NoFailures()
    {
        var b = new HelperBuilder();
        var failures = new FailuresDictionary();

        b.CallAssertNotEmptyOrWhitespace_String(null, "n", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(0);

        failures = new FailuresDictionary();
        b.CallAssertNotEmptyOrWhitespace_List(null, "lst", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(0);
    }

    [Fact]
    public void Given_HelperBuilder_InvalidInputs_When_InvokeAssertHelpers_Then_FailuresAdded()
    {
        var b = new HelperBuilder();
        var failures = new FailuresDictionary();

        b.CallAssertNotNullOrEmptyOrWhitespace("", "n", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        b.CallAssertNotNull(null, "obj", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        b.CallAssertNotNullNotEmptyCollection(new List<string> { " ", "ok" }, "col", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);

        failures = new FailuresDictionary();
        b.CallAssert(() => true, "p", failures, s => new System.Exception(s));
        failures.Count.ShouldBe(1);
    }

    [Fact]
    public void Given_FailuresDictionary_When_AddingMultipleFailures_Then_Accumulates()
    {
        var fd = new FailuresDictionary();
        fd.Failure("a", new Failure("x"));
        fd.Failure("a", new Failure("y"));
        fd.ContainsKey("a").ShouldBeTrue();
        fd["a"].Count.ShouldBe(2);
    }

    [Fact]
    public void Given_BuilderList_When_Built_Then_ReferencesResolve()
    {
        var list = new BuilderList<string, SimpleBuilder>();
        list.New(b => b.Value("one"));
        list.New(b => b.Value("two"));

        var refs = list.AsReferenceList();
        refs.Any().ShouldBeFalse();

        var built = list.BuildSuccess();
        built.Count.ShouldBe(2);

        // after building, the references should resolve
        refs.Any().ShouldBeTrue();
        refs.ElementAt(0).ShouldBe("one");
        refs.IndexOf("two").ShouldBe(1);
    }
}
