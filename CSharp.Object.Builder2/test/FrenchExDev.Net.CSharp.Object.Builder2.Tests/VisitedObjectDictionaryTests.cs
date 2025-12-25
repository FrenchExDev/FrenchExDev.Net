using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class VisitedObjectDictionaryTests
{
    [Fact]
    public void IsVisited_WhenNotVisited_ShouldReturnFalse()
    {
        var visited = new VisitedObjectDictionary();
        var id = Guid.NewGuid();

        visited.IsVisited(id).ShouldBeFalse();
    }

    [Fact]
    public void IsVisited_WhenVisited_ShouldReturnTrue()
    {
        var visited = new VisitedObjectDictionary();
        var id = Guid.NewGuid();

        visited.MarkVisited(id, new object());

        visited.IsVisited(id).ShouldBeTrue();
    }

    [Fact]
    public void TryGet_WhenVisited_ShouldReturnObject()
    {
        var visited = new VisitedObjectDictionary();
        var id = Guid.NewGuid();
        var obj = new SimpleObject { Value = "test" };

        visited.MarkVisited(id, obj);

        visited.TryGet(id, out var result).ShouldBeTrue();
        result.ShouldBe(obj);
    }

    [Fact]
    public void TryGet_WhenNotVisited_ShouldReturnFalse()
    {
        var visited = new VisitedObjectDictionary();

        visited.TryGet(Guid.NewGuid(), out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }
}

