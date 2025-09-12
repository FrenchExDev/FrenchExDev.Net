using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Abstract step in a multi-step object building process.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public abstract class AbstractStepObjectBuilder<TClass> : IStepObjectBuilder<TClass>
{
    /// <summary>
    /// Holds the result of the building process for this step.
    /// </summary>
    private TClass? _result;

    /// <summary>
    /// Holds the result of the building process for this step.
    /// </summary>
    public virtual TClass Result() => _result is not null ? _result : throw new InvalidOperationException("Result not available");

    /// <summary>
    /// Step logic to build part of the object using the provided intermediate state and visited objects.
    /// </summary>
    /// <param name="intermediate"></param>
    /// <param name="visited"></param>
    public abstract void Build(ExceptionBuildDictionary exceptions, IntermediateObjectDictionary intermediate, VisitedObjectsList visited);

    /// <summary>
    /// Determines whether the current instance contains a valid result of the specified type.
    /// </summary>
    /// <returns><see langword="true"/> if the result is not <see langword="null"/> and is of type <typeparamref name="TClass"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public bool HasResult()
    {
        return _result is not null && _result is TClass;
    }

    /// <summary>
    /// Sets the result of the step to the provided instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="result"></param>
    public void Set(TClass result)
    {
        _result = result;
    }
}
