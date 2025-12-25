using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReferenceTests
{
    [Fact]
    public void NewReference_ShouldBeUnresolved()
    {
        var reference = new Reference<SimpleObject>();

        reference.IsResolved.ShouldBeFalse();
        reference.Instance.ShouldBeNull();
    }

    [Fact]
    public void Resolve_ShouldSetInstance()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        reference.Resolve(obj);

        reference.IsResolved.ShouldBeTrue();
        reference.Instance.ShouldBe(obj);
    }

    [Fact]
    public void Resolved_WhenResolved_ShouldReturnInstance()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        reference.Resolve(obj);

        var result = reference.Resolved();

        result.ShouldBe(obj);
    }

    [Fact]
    public void Resolved_WhenNotResolved_ShouldThrow()
    {
        var reference = new Reference<SimpleObject>();

        Should.Throw<ReferenceNotResolvedException>(() => reference.Resolved());
    }

    [Fact]
    public void ResolvedOrNull_WhenResolved_ShouldReturnInstance()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        reference.Resolve(obj);

        var result = reference.ResolvedOrNull();

        result.ShouldBe(obj);
    }

    [Fact]
    public void ResolvedOrNull_WhenNotResolved_ShouldReturnNull()
    {
        var reference = new Reference<SimpleObject>();

        var result = reference.ResolvedOrNull();

        result.ShouldBeNull();
    }

    [Fact]
    public void OnResolve_ShouldCallCallbackWhenResolved()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        var callbackCalled = false;

        reference.OnResolve(_ => callbackCalled = true);
        reference.Resolve(obj);

        callbackCalled.ShouldBeTrue();
    }

    [Fact]
    public void OnResolve_RegisteredAfterResolve_ShouldNotBeCalled()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        reference.Resolve(obj);

        var callbackCalled = false;
        reference.OnResolve(_ => callbackCalled = true);

        callbackCalled.ShouldBeFalse();
    }

    [Fact]
    public void Resolve_CalledTwice_ShouldOnlyResolveOnce()
    {
        var reference = new Reference<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };

        reference.Resolve(obj1);
        reference.Resolve(obj2);

        reference.Resolved().Value.ShouldBe("first");
    }

    [Fact]
    public void Constructor_WithExisting_ShouldBeResolved()
    {
        var obj = new SimpleObject { Value = "test" };

        var reference = new Reference<SimpleObject>(obj);

        reference.IsResolved.ShouldBeTrue();
        reference.Resolved().ShouldBe(obj);
    }
}

