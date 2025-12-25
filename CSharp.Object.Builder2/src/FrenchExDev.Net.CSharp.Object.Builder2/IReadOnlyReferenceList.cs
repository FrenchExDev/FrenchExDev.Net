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
