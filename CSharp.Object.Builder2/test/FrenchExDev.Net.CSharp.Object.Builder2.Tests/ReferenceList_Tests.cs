using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReferenceList_Tests
{
    [Fact]
    public void ReferenceList_Add_Instance_And_Contains_Works()
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
    public void ReferenceList_IndexOf_And_Remove()
    {
        var list = new ReferenceList<string>();
        list.Add("x");
        list.Add("y");
        list.IndexOf("y").ShouldBe(1);
        list.Remove("x").ShouldBeTrue();
        list.Count.ShouldBe(1);
    }

    [Fact]
    public void ReferenceList_Select_And_Where()
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
