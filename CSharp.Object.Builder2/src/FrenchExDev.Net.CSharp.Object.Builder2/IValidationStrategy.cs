namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for validation strategies.
/// Follows Open/Closed Principle - new validation strategies can be added without modifying existing code.
/// </summary>
/// <typeparam name="TBuilder">The type of builder being validated.</typeparam>
public interface IValidationStrategy<in TBuilder>
{
    /// <summary>
    /// Validates the builder and collects failures.
    /// </summary>
    void Validate(TBuilder builder, IVisitedTracker visited, IFailureCollector failures);
}
