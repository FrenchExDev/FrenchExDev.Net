using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class Reference_Tests
{
    [Fact]
    public void Reference_Can_Resolve_And_ResolvedOrNull()
    {
        var r = new Reference<string>();
        r.IsResolved.ShouldBeFalse();
        r.ResolvedOrNull().ShouldBeNull();

        r.Resolve("hello");
        r.IsResolved.ShouldBeTrue();
        r.ResolvedOrNull().ShouldBe("hello");
        r.Resolved().ShouldBe("hello");
    }

    [Fact]
    public void Reference_Resolved_Throws_When_NotResolved()
    {
        var r = new Reference<object>();
        Should.Throw<NotResolvedException>(() => r.Resolved());
    }
}
