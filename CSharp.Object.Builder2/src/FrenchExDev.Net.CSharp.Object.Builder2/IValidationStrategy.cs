namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for custom validation strategies that can be applied to builders.
/// </summary>
/// <typeparam name="TBuilder">The type of builder being validated.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IValidationStrategy{TBuilder}"/> follows the Strategy pattern and Open/Closed Principle,
/// allowing new validation logic to be added without modifying existing builder code.
/// </para>
/// <para>
/// Implementations can provide domain-specific validation rules that are applied during
/// the build process.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class PersonValidationStrategy : IValidationStrategy&lt;PersonBuilder&gt;
/// {
///     public void Validate(PersonBuilder builder, IVisitedTracker visited, IFailureCollector failures)
///     {
///         if (builder.Age &lt; 0 || builder.Age &gt; 150)
///         {
///             failures.AddFailure(nameof(builder.Age), 
///                 Failure.FromMessage("Age must be between 0 and 150"));
///         }
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IValidatable"/>
/// <seealso cref="IFailureCollector"/>
public interface IValidationStrategy<in TBuilder>
{
    /// <summary>
    /// Validates the specified builder and records any failures in the collector.
    /// </summary>
    /// <param name="builder">The builder instance to validate.</param>
    /// <param name="visited">Tracker for visited objects to handle circular references.</param>
    /// <param name="failures">Collector for recording validation failures.</param>
    void Validate(TBuilder builder, IVisitedTracker visited, IFailureCollector failures);
}
