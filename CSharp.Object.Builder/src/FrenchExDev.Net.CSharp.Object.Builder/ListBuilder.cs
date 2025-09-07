using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Builder class for constructing lists of objects using individual item builders.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TItemBuilder"></typeparam>
public class ListBuilder<TItem, TItemBuilder>
    where TItem : class
    where TItemBuilder : IObjectBuilder<TItem>, new()
{
    private readonly List<TItemBuilder> _items = new();
    public ListBuilder<TItem, TItemBuilder> Add(TItemBuilder item)
    {
        _items.Add(item);
        return this;
    }

    public ListBuilder<TItem, TItemBuilder> Add(Action<TItemBuilder> body)
    {
        var builder = new TItemBuilder();
        body(builder);
        _items.Add(builder);
        return this;
    }

    public ListBuilder<TItem, TItemBuilder> AddRange(IEnumerable<TItemBuilder> items)
    {
        _items.AddRange(items);
        return this;
    }

    public List<TItem> Build()
    {
        return _items.Select(x => x.Build().Success()).ToList();
    }
}
