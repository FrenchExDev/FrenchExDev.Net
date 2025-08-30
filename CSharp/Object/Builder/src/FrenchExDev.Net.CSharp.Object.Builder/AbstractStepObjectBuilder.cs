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
    /// Holds whether this step is the final step in the building process.
    /// </summary>
    public virtual bool IsFinalStep { get; set; }

    /// <summary>
    /// Holds the result of the building process for this step.
    /// </summary>
    public virtual Task<TClass> Result => _result is not null ? Task.FromResult(_result) : throw new InvalidOperationException("Result not available");

    /// <summary>
    /// Step logic to build part of the object using the provided intermediate state and visited objects.
    /// </summary>
    /// <param name="intermediate"></param>
    /// <param name="visited"></param>
    public abstract void Build(IntermediateObjectsList intermediate, VisitedObjectsList visited);

    public void Set(TClass result)
    {
        _result = result;
    }
}
