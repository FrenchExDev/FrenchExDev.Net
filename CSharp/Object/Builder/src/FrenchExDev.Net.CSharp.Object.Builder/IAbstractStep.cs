namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Defines a step in a process that can produce a result of the specified type.
/// </summary>
/// <typeparam name="TClass">The type of the result produced by the step.</typeparam>
public interface IAbstractStep<TClass>
{
    /// <summary>
    /// Returns whether the operation has produced a result.
    /// </summary>
    /// <returns></returns>
    bool HasResult();

    /// <summary>
    /// Returns the result of the operation.
    /// </summary>
    /// <returns></returns>
    TClass Result();
}
