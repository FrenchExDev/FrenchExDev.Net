using Shouldly;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Collections;

public class ReferenceList_Basic_Tests
{
    [Fact]
    public void Given_EmptyList_When_Adding_Instance_Then_ContainsAndIndexerWork()
    {
        var list = new ReferenceList<string>();
        list.Any().ShouldBeFalse();

        list.Add("a");
        list.Any().ShouldBeTrue();
        list.Contains("a").ShouldBeTrue();
        list[0].ShouldBe("a");
        list.ElementAt(0).ShouldBe("a");
    }

    [Fact]
    public void Given_ListWithItems_When_IndexOfAndRemove_Then_BehaviorIsExpected()
    {
        var list = new ReferenceList<string>();
        list.Add("x");
        list.Add("y");
        list.IndexOf("y").ShouldBe(1);
        list.Remove("x").ShouldBeTrue();
        list.Count.ShouldBe(1);
    }

    [Fact]
    public void Given_ListWithNumbers_When_SelectAndWhere_Then_FilteringWorks()
    {
        var list = new ReferenceList<string>();
        list.Add("1");
        list.Add("2");
        var evens = list.Where(s => int.Parse(s) % 2 == 0).ToList();
        evens.Count.ShouldBe(1);
        evens[0].ShouldBe("2");
        list.All(s => int.Parse(s) > 0).ShouldBeTrue();
    }
}
