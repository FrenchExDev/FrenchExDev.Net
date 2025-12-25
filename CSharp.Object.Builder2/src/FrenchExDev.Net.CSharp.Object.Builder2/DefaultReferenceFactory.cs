namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Default implementation of IReferenceFactory.
/// </summary>
public class DefaultReferenceFactory : IReferenceFactory
{
    public static readonly DefaultReferenceFactory Instance = new();
    
    public Reference<TClass> Create<TClass>() where TClass : class => new();
    public Reference<TClass> Create<TClass>(TClass? existing) where TClass : class => new(existing);
}
