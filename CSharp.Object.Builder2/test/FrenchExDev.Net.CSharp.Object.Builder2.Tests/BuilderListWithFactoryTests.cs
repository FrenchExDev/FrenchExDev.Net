using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class BuilderListWithFactoryTests
{
    [Fact]
    public void New_ShouldUseFactory()
    {
        var factoryCallCount = 0;
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() =>
        {
            factoryCallCount++;
            return new SimpleObjectBuilder();
        });

        list.New(b => b.WithValue("test"));

        factoryCallCount.ShouldBe(1);
    }

    [Fact]
    public void Constructor_WithNullFactory_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(null!));
    }

    [Fact]
    public void New_ShouldReturnListForChaining()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());

        var result = list
            .New(b => b.WithValue("a"))
            .New(b => b.WithValue("b"));

        result.ShouldBeSameAs(list);
        list.Count.ShouldBe(2);
    }

    [Fact]
    public void AsReferenceList_ShouldReturnReferences()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("test"));

        var refList = list.AsReferenceList();

        refList.Count.ShouldBe(1);
    }

    [Fact]
    public void BuildSuccess_ShouldReturnBuiltInstances()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("a"));
        list.New(b => b.WithValue("b"));

        var results = list.BuildSuccess();

        results.Count.ShouldBe(2);
    }

    [Fact]
    public void ValidateFailures_ShouldReturnOnlyFailedValidations()
    {
        var list = new BuilderListWithFactory<Person, PersonBuilder>(() => new PersonBuilder());
        list.New(b => b.WithName("Valid").WithAge(25)); // Valid
        list.New(b => b.WithAge(30)); // Invalid - missing name

        var failures = list.ValidateFailures();

        failures.Count.ShouldBe(1);
    }
}

