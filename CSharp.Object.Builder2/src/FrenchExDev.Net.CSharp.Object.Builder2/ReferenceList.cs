namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Read-only list of object references.
/// </summary>
public interface IReadOnlyReferenceList<TClass> : IReadOnlyList<TClass> where TClass : class
{
    IEnumerable<TClass> AsEnumerable();
    bool Contains(TClass instance);
    bool Contains(Reference<TClass> reference);
}

/// <summary>
/// Mutable list of object references.
/// </summary>
public interface IReferenceList<TClass> : IList<TClass>, IReadOnlyReferenceList<TClass> where TClass : class
{
    IQueryable<TClass> Queryable { get; }
    TClass ElementAt(int index);
    void Add(Reference<TClass> reference);
    new bool Contains(Reference<TClass> reference);
}

/// <summary>
/// List of references. Use AsEnumerable() with standard LINQ methods for querying.
/// </summary>
public class ReferenceList<TClass> : IReferenceList<TClass> where TClass : class
{
    private readonly List<Reference<TClass>> _references;

    public ReferenceList(IEnumerable<Reference<TClass>> references) { _references = references.ToList() ?? throw new ArgumentNullException(nameof(references)); }
    public ReferenceList() { _references = []; }

    public IEnumerable<TClass> AsEnumerable()
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) yield return r.Instance;
    }

    public void Add(Reference<TClass> reference) => _references.Add(reference);
    public void Add(TClass instance) => _references.Add(new Reference<TClass>().Resolve(instance));
    public bool Contains(Reference<TClass> reference) => _references.Contains(reference);

    public bool Contains(TClass instance)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance == instance) return true;
        return false;
    }

    public IQueryable<TClass> Queryable => AsEnumerable().AsQueryable();
    public TClass ElementAt(int index) => _references[index].Resolved();

    public int IndexOf(TClass item)
    {
        for (int i = 0; i < _references.Count; i++) { var r = _references[i]; if (r.IsResolved && r.Instance == item) return i; }
        throw new InvalidOperationException("Item not found");
    }

    public void Insert(int index, TClass item) => _references.Insert(index, new Reference<TClass>().Resolve(item));
    public void RemoveAt(int index) => _references.RemoveAt(index);
    public void Clear() => _references.Clear();

    public void CopyTo(TClass[] array, int arrayIndex)
    {
        var i = arrayIndex;
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) array[i++] = r.Instance;
    }

    public bool Remove(TClass item)
    {
        for (int i = 0; i < _references.Count; i++)
        {
            var r = _references[i];
            if (r.IsResolved && r.Instance == item) { _references.RemoveAt(i); return true; }
        }
        return false;
    }

    public IEnumerator<TClass> GetEnumerator() => AsEnumerable().GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _references.Count;
    public bool IsReadOnly => false;

    public TClass this[int index]
    {
        get => _references[index].Resolved();
        set => _references[index] = new Reference<TClass>().Resolve(value);
    }
}
