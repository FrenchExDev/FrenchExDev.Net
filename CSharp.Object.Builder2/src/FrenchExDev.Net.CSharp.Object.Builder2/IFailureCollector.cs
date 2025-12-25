namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for collecting validation and build failures.
/// </summary>
/// <remarks>
/// <para>
/// Implementations of <see cref="IFailureCollector"/> are used throughout the validation process
/// to accumulate failures without interrupting the validation flow. This allows all validation
/// errors to be collected and reported together.
/// </para>
/// </remarks>
/// <seealso cref="FailuresDictionary"/>
/// <seealso cref="Failure"/>
public interface IFailureCollector
{
    /// <summary>
    /// Adds a failure associated with a specific member.
    /// </summary>
    /// <param name="memberName">The name of the member (property, field, parameter) that has the failure.</param>
    /// <param name="failure">The <see cref="Failure"/> to add.</param>
    /// <returns>The current collector instance for method chaining.</returns>
    IFailureCollector AddFailure(string memberName, Failure failure);

    /// <summary>
    /// Gets a value indicating whether any failures have been recorded.
    /// </summary>
    /// <value><see langword="true"/> if at least one failure exists; otherwise, <see langword="false"/>.</value>
    bool HasFailures { get; }

    /// <summary>
    /// Gets the number of distinct members that have recorded failures.
    /// </summary>
    int FailureCount { get; }
}
