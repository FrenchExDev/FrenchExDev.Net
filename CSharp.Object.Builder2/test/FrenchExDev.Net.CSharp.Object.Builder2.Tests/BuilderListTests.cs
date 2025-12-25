using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class BuilderListTests
{
    [Fact]
    public void New_ShouldAddConfiguredBuilder()
    {
        var list = new BuilderList<SimpleObject, SimpleObjectBuilder>();

        list.New(b => b.WithValue("test"));

        list.Count.ShouldBe(1);
        list[0].Value.ShouldBe("test");
    }

    [Fact]
    public void New_ShouldReturnListForChaining()
    {
        var list = new BuilderList<SimpleObject, SimpleObjectBuilder>();

        var result = list
            .New(b => b.WithValue("a"))
            .New(b => b.WithValue("b"))
            .New(b => b.WithValue("c"));

        result.ShouldBeSameAs(list);
        list.Count.ShouldBe(3);
    }

    [Fact]
    public void AsReferenceList_ShouldReturnReferences()
    {
        var list = new BuilderList<SimpleObject, SimpleObjectBuilder>();
        list.New(b => b.WithValue("test"));

        var refList = list.AsReferenceList();

        refList.Count.ShouldBe(1);
    }

    [Fact]
    public void BuildSuccess_ShouldReturnBuiltInstances()
    {
        var list = new BuilderList<SimpleObject, SimpleObjectBuilder>();
        list.New(b => b.WithValue("a"));
        list.New(b => b.WithValue("b"));

        var results = list.BuildSuccess();

        results.Count.ShouldBe(2);
        results[0].Value.ShouldBe("a");
        results[1].Value.ShouldBe("b");
    }

    [Fact]
    public void ValidateFailures_ShouldReturnOnlyFailedValidations()
    {
        var list = new BuilderList<Person, PersonBuilder>();
        list.New(b => b.WithName("Valid").WithAge(25)); // Valid
        list.New(b => b.WithAge(30)); // Invalid - missing name

        var failures = list.ValidateFailures();

        failures.Count.ShouldBe(1);
    }
}

