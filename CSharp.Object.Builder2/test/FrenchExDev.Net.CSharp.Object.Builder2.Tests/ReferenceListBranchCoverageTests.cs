using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReferenceListBranchCoverageTests
{
    [Fact]
    public void Indexer_Set_ShouldUpdateValue()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj1 = new SimpleObject { Value = "original" };
        var obj2 = new SimpleObject { Value = "updated" };
        refList.Add(obj1);

        refList[0] = obj2;

        refList[0].ShouldBe(obj2);
    }

    [Fact]
    public void Contains_WithUnresolvedReference_ShouldReturnFalse()
    {
        var refList = new ReferenceList<SimpleObject>();
        var unresolved = new Reference<SimpleObject>();
        refList.Add(unresolved);
        var obj = new SimpleObject { Value = "test" };

        refList.Contains(obj).ShouldBeFalse();
    }

    [Fact]
    public void IndexOf_WithUnresolvedReference_ShouldNotMatch()
    {
        var refList = new ReferenceList<SimpleObject>();
        var unresolved = new Reference<SimpleObject>();
        refList.Add(unresolved);
        var obj = new SimpleObject { Value = "test" };

        Should.Throw<InvalidOperationException>(() => refList.IndexOf(obj));
    }

    [Fact]
    public void Remove_WithUnresolvedReference_ShouldNotMatch()
    {
        var refList = new ReferenceList<SimpleObject>();
        var unresolved = new Reference<SimpleObject>();
        refList.Add(unresolved);
        var obj = new SimpleObject { Value = "test" };

        refList.Remove(obj).ShouldBeFalse();
    }
}

