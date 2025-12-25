using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class DefaultReferenceFactoryTests
{
    [Fact]
    public void Create_ShouldReturnUnresolvedReference()
    {
        var reference = DefaultReferenceFactory.Instance.Create<SimpleObject>();
        reference.ShouldNotBeNull();
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Create_WithExisting_ShouldReturnResolvedReference()
    {
        var existing = new SimpleObject { Value = "test" };
        var reference = DefaultReferenceFactory.Instance.Create(existing);
        reference.IsResolved.ShouldBeTrue();
        reference.Resolved().ShouldBe(existing);
    }

    [Fact]
    public void Create_WithNull_ShouldReturnUnresolvedReference()
    {
        var reference = DefaultReferenceFactory.Instance.Create<SimpleObject>(null);
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        var a = DefaultReferenceFactory.Instance;
        var b = DefaultReferenceFactory.Instance;
        a.ShouldBeSameAs(b);
    }
}

