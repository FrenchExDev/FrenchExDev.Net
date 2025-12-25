using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that can be built.
/// </summary>
public interface IBuildable<TClass> where TClass : class
{
    BuildStatus BuildStatus { get; }
    Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null);
}

/// <summary>
/// Defines a contract for objects that can be validated.
/// </summary>
public interface IValidatable
{
    ValidationStatus ValidationStatus { get; }
    void Validate(VisitedObjectDictionary visitedCollector, IFailureCollector failures);
}

/// <summary>
/// Defines a contract for objects that can provide a reference to themselves.
/// </summary>
public interface IReferenceable<TClass> where TClass : class
{
    Reference<TClass> Reference();
}

/// <summary>
/// Defines a contract for objects that have a unique identifier.
/// </summary>
public interface IIdentifiable
{
    Guid Id { get; }
}

/// <summary>
/// Defines a contract for collecting validation/build failures.
/// </summary>
public interface IFailureCollector
{
    IFailureCollector AddFailure(string memberName, Failure failure);
    bool HasFailures { get; }
    int FailureCount { get; }
}

/// <summary>
/// Defines a contract for creating references.
/// </summary>
public interface IReferenceFactory
{
    Reference<TClass> Create<TClass>() where TClass : class;
    Reference<TClass> Create<TClass>(TClass? existing) where TClass : class;
}

/// <summary>
/// Defines the complete builder contract.
/// </summary>
public interface IBuilder<TClass> : IBuildable<TClass>, IValidatable, IReferenceable<TClass>, IIdentifiable
    where TClass : class
{
    TClass? Result { get; }
}
