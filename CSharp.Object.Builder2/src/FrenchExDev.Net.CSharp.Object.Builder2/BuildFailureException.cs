namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Exception thrown when a build operation fails due to validation errors.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BuildFailureException"/> wraps the <see cref="IFailureCollector"/> that contains
/// all validation failures that caused the build to fail. This exception is typically:
/// </para>
/// <list type="bullet">
///   <item><description>Created internally by <see cref="AbstractBuilder{TClass}"/> when validation fails</description></item>
///   <item><description>Stored in the <c>Result</c> of a failed build</description></item>
///   <item><description>Thrown by <see cref="AbstractBuilder{TClass}.BuildOrThrow"/> wrapped in an <see cref="AggregateException"/></description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// try
/// {
///     var person = builder.BuildOrThrow();
/// }
/// catch (AggregateException ex)
/// {
///     // BuildOrThrow wraps failures in AggregateException
///     foreach (var inner in ex.InnerExceptions)
///     {
///         Console.WriteLine(inner.Message);
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IFailureCollector"/>
/// <seealso cref="FailuresDictionary"/>
public class BuildFailureException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildFailureException"/> class with the specified failures.
    /// </summary>
    /// <param name="failures">The collector containing all validation failures that caused the build to fail.</param>
    public BuildFailureException(IFailureCollector failures) 
    { 
        Failures = failures; 
    }

    /// <summary>
    /// Gets the collection of failures that caused the build to fail.
    /// </summary>
    /// <value>An <see cref="IFailureCollector"/> containing all recorded validation failures.</value>
    public IFailureCollector Failures { get; }
}
