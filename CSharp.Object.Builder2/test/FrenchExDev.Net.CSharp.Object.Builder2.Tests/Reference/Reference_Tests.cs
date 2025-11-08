using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Reference;

public class Reference_Tests
{
    [Fact]
    public void Given_NewReference_When_Unresolved_Then_IsResolved_False_And_ResolvedOrNull_Null()
    {
        var r = new Reference<string>();
        r.IsResolved.ShouldBeFalse();
        r.ResolvedOrNull().ShouldBeNull();
    }

    [Fact]
    public void Given_ResolvedReference_When_Resolved_Then_Returns_Value()
    {
        var r = new Reference<string>();
        r.Resolve("hello");
        r.IsResolved.ShouldBeTrue();
        r.ResolvedOrNull().ShouldBe("hello");
        r.Resolved().ShouldBe("hello");
    }

    [Fact]
    public void Given_UnresolvedReference_When_ResolvedCalled_Then_Throws_NotResolvedException()
    {
        var r = new Reference<object>();
        Should.Throw<NotResolvedException>(() => r.Resolved());
    }

    [Fact]
    public void Given_RecordCtor_When_PassedValue_Then_IsResolvedTrue()
    {
        var r = new Reference<string>("hello");
        r.IsResolved.ShouldBeTrue();
        r.Instance.ShouldBe("hello");
        r.Resolved().ShouldBe("hello");
    }
}
