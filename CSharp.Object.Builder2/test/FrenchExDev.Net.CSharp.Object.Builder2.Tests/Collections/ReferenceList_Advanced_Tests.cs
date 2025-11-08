using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Collections;

public class ReferenceList_Advanced_Tests
{
    private class Simple { public string Name { get; set; } = string.Empty; }

    [Fact]
    public void Given_UnresolvedReference_When_ElementAt_Then_Throws_NotResolvedException()
    {
        var rl = new ReferenceList<Simple>();
        var r = new Reference<Simple>();
        rl.Add(r);
        Should.Throw<NotResolvedException>(() => rl.ElementAt(0));
    }

    [Fact]
    public void Given_ItemNotFound_When_IndexOf_Then_Throws_InvalidOperationException()
    {
        var rl = new ReferenceList<Simple>();
        rl.Add(new Simple());
        Should.Throw<InvalidOperationException>(() => rl.IndexOf(new Simple()));
    }

    [Fact]
    public void Given_ResolvedItems_When_CopyTo_Then_CopiesValues()
    {
        var rl = new ReferenceList<string>();
        rl.Add("a");
        rl.Add("b");
        var arr = new string[2];
        rl.CopyTo(arr, 0);
        arr[0].ShouldBe("a");
        arr[1].ShouldBe("b");
    }

    [Fact]
    public void Given_List_When_InsertRemoveClear_Then_BehavesAsExpected()
    {
        var rl = new ReferenceList<string>();
        rl.Add("one");
        rl.Add("three");
        rl.Insert(1, "two");
        rl.Count.ShouldBe(3);
        rl[1].ShouldBe("two");

        rl.RemoveAt(1);
        rl.Count.ShouldBe(2);

        var removed = rl.Remove("nonexistent");
        removed.ShouldBeFalse();

        var removed2 = rl.Remove("one");
        removed2.ShouldBeTrue();
        rl.Clear();
        rl.Count.ShouldBe(0);
    }

    [Fact]
    public void Given_List_When_AnyAllWhereSelectQueryable_Then_LINQWorks()
    {
        var rl = new ReferenceList<string>();
        rl.Add("1");
        rl.Add("2");
        rl.Any().ShouldBeTrue();
        rl.Any(s => s == "2").ShouldBeTrue();
        rl.All(s => int.Parse(s) > 0).ShouldBeTrue();
        var evens = rl.Where(s => int.Parse(s) % 2 == 0).ToList();
        evens.Count.ShouldBe(1);
        var q = rl.Queryable.Where(s => s == "1").ToList();
        q.Count.ShouldBe(1);
        var mapped = rl.Select(s => s + "x").ToList();
        mapped[0].ShouldBe("1x");
    }
}
