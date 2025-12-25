namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for creating references.
/// </summary>
public interface IReferenceFactory
{
    Reference<TClass> Create<TClass>() where TClass : class;
    Reference<TClass> Create<TClass>(TClass? existing) where TClass : class;
}
