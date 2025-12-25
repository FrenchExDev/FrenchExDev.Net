using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReferenceListTests
{
    [Fact]
    public void AsEnumerable_ShouldYieldOnlyResolvedReferences()
    {
        var refList = new ReferenceList<SimpleObject>();
        var resolved = new Reference<SimpleObject>().Resolve(new SimpleObject { Value = "resolved" });
        var unresolved = new Reference<SimpleObject>();

        refList.Add(resolved);
        refList.Add(unresolved);

        var items = refList.AsEnumerable().ToList();
        items.Count.ShouldBe(1);
        items[0].Value.ShouldBe("resolved");
    }

    [Fact]
    public void Add_WithInstance_ShouldWrapInReference()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        refList.Add(obj);

        refList.Count.ShouldBe(1);
        refList.AsEnumerable().First().ShouldBe(obj);
    }

    [Fact]
    public void Indexer_ShouldReturnResolvedValue()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        refList[0].ShouldBe(obj);
    }

    [Fact]
    public void Contains_WithInstance_ShouldReturnTrueWhenFound()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        refList.Contains(obj).ShouldBeTrue();
    }

    [Fact]
    public void Contains_WithInstance_ShouldReturnFalseWhenNotFound()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        refList.Contains(obj).ShouldBeFalse();
    }

    [Fact]
    public void Contains_WithReference_ShouldWork()
    {
        var refList = new ReferenceList<SimpleObject>();
        var reference = new Reference<SimpleObject>().Resolve(new SimpleObject { Value = "test" });
        refList.Add(reference);

        refList.Contains(reference).ShouldBeTrue();
    }

    [Fact]
    public void ElementAt_ShouldReturnResolvedInstance()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        refList.ElementAt(0).ShouldBe(obj);
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        refList.Add(obj1);
        refList.Add(obj2);

        refList.IndexOf(obj2).ShouldBe(1);
    }

    [Fact]
    public void IndexOf_WhenNotFound_ShouldThrow()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        Should.Throw<InvalidOperationException>(() => refList.IndexOf(obj));
    }

    [Fact]
    public void Insert_ShouldInsertAtCorrectIndex()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        refList.Add(obj1);

        refList.Insert(0, obj2);

        refList.Count.ShouldBe(2);
        refList[0].ShouldBe(obj2);
    }

    [Fact]
    public void RemoveAt_ShouldRemoveAtIndex()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        refList.Add(obj1);
        refList.Add(obj2);

        refList.RemoveAt(0);

        refList.Count.ShouldBe(1);
        refList[0].ShouldBe(obj2);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var refList = new ReferenceList<SimpleObject>();
        refList.Add(new SimpleObject { Value = "test" });
        refList.Add(new SimpleObject { Value = "test2" });

        refList.Clear();

        refList.Count.ShouldBe(0);
    }

    [Fact]
    public void CopyTo_ShouldCopyResolvedInstances()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        var array = new SimpleObject[1];
        refList.CopyTo(array, 0);

        array[0].ShouldBe(obj);
    }

    [Fact]
    public void Remove_ShouldRemoveInstance()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        var result = refList.Remove(obj);

        result.ShouldBeTrue();
        refList.Count.ShouldBe(0);
    }

    [Fact]
    public void Remove_WhenNotFound_ShouldReturnFalse()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        var result = refList.Remove(obj);

        result.ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_ShouldEnumerateResolvedInstances()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        var items = refList.ToList();

        items.Count.ShouldBe(1);
        items[0].ShouldBe(obj);
    }

    [Fact]
    public void Queryable_ShouldReturnQueryable()
    {
        var refList = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        refList.Add(obj);

        var queryable = refList.Queryable;

        queryable.ShouldNotBeNull();
        queryable.Count().ShouldBe(1);
    }

    [Fact]
    public void IsReadOnly_ShouldBeFalse()
    {
        var refList = new ReferenceList<SimpleObject>();

        refList.IsReadOnly.ShouldBeFalse();
    }
}

