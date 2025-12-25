namespace FrenchExDev.Net.CSharp.Object.Builder2;

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
