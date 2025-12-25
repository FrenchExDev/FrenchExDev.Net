namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Provides static validation assertion methods for use in builder validation logic.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ValidationAssertions"/> contains a set of commonly used validation checks that can be called
/// from <see cref="AbstractBuilder{TClass}.ValidateInternal"/> implementations.
/// </para>
/// <para>
/// All methods follow the same pattern: check a condition and add a failure to the collector if the check fails.
/// The <paramref name="exceptionBuilder"/> parameter allows customizing the exception type created for each failure.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// protected override void ValidateInternal(VisitedObjectDictionary visited, IFailureCollector failures)
/// {
///     ValidationAssertions.AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures,
///         n => new ArgumentException($"{n} is required"));
///     
///     ValidationAssertions.Assert(() => Age &lt; 0, nameof(Age), failures,
///         n => new ArgumentOutOfRangeException(n, "Age cannot be negative"));
/// }
/// </code>
/// </example>
/// <seealso cref="AbstractBuilder{TClass}"/>
/// <seealso cref="IFailureCollector"/>
public static class ValidationAssertions
{
    /// <summary>
    /// Validates that a string is not empty or whitespace. Null values pass this validation.
    /// </summary>
    /// <param name="value">The string value to validate. <see langword="null"/> is allowed.</param>
    /// <param name="name">The name of the member being validated, used for error reporting.</param>
    /// <param name="failures">The collector to add the failure to if validation fails.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for the failure, receiving the member name.</param>
    /// <remarks>
    /// Use this method when a property is optional (can be null) but if provided, must have content.
    /// For required strings that cannot be null, use <see cref="AssertNotNullOrEmptyOrWhitespace"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Description is optional but if provided, cannot be empty
    /// AssertNotEmptyOrWhitespace(Description, nameof(Description), failures,
    ///     n => new ArgumentException($"{n} cannot be empty if provided"));
    /// </code>
    /// </example>
    public static void AssertNotEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (value is null) return;
        if (string.IsNullOrWhiteSpace(value)) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that each string in a list is not empty or whitespace. Null lists pass this validation.
    /// </summary>
    /// <param name="values">The list of strings to validate. <see langword="null"/> is allowed.</param>
    /// <param name="name">The name of the member being validated.</param>
    /// <param name="failures">The collector to add failures to. One failure is added per invalid string.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for each failure.</param>
    /// <remarks>
    /// This method validates each individual string in the list. If you need to ensure the list itself
    /// is not null or empty, use <see cref="AssertNotNullNotEmptyCollection{T}"/> instead.
    /// </remarks>
    public static void AssertNotEmptyOrWhitespace(List<string>? values, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (values is null) return;
        foreach (var item in values) AssertNotEmptyOrWhitespace(item, name, failures, exceptionBuilder);
    }

    /// <summary>
    /// Validates that a string is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="name">The name of the member being validated.</param>
    /// <param name="failures">The collector to add the failure to if validation fails.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for the failure.</param>
    /// <remarks>
    /// This is the most common string validation for required fields.
    /// It fails if the value is <see langword="null"/>, empty (<c>""</c>), or contains only whitespace.
    /// </remarks>
    /// <example>
    /// <code>
    /// AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures,
    ///     n => new ArgumentException($"{n} is required"));
    /// </code>
    /// </example>
    public static void AssertNotNullOrEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (string.IsNullOrWhiteSpace(value)) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that a value is not null.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The name of the member being validated.</param>
    /// <param name="failures">The collector to add the failure to if validation fails.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for the failure.</param>
    /// <example>
    /// <code>
    /// AssertNotNull(Address, nameof(Address), failures,
    ///     n => new ArgumentNullException(n));
    /// </code>
    /// </example>
    public static void AssertNotNull(object? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (value is null) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that a collection is not null, not empty, and (for string collections) contains no empty/whitespace strings.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to validate.</param>
    /// <param name="name">The name of the member being validated.</param>
    /// <param name="failures">The collector to add failures to.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for each failure.</param>
    /// <remarks>
    /// <para>
    /// This method performs three checks:
    /// </para>
    /// <list type="number">
    ///   <item><description>The list is not <see langword="null"/></description></item>
    ///   <item><description>The list has at least one element</description></item>
    ///   <item><description>If the list contains strings, none are empty or whitespace</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// AssertNotNullNotEmptyCollection(Tags, nameof(Tags), failures,
    ///     n => new ArgumentException($"{n} must contain at least one valid tag"));
    /// </code>
    /// </example>
    public static void AssertNotNullNotEmptyCollection<T>(List<T>? list, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (list is null) { failures.AddFailure(name, Failure.FromException(exceptionBuilder(name))); return; }
        if (list.Count == 0) { failures.AddFailure(name, Failure.FromException(exceptionBuilder(name))); return; }
        foreach (var item in list)
            if (item is string str && string.IsNullOrWhiteSpace(str))
                failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that a custom predicate returns false. If the predicate returns true, a failure is recorded.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> if validation should fail.</param>
    /// <param name="name">The name of the member being validated.</param>
    /// <param name="failures">The collector to add the failure to if validation fails.</param>
    /// <param name="exceptionBuilder">A function that creates the exception for the failure.</param>
    /// <remarks>
    /// <para>
    /// The predicate should return <see langword="true"/> when the validation <em>fails</em>.
    /// This allows for natural reading: "Assert that [condition] is a failure".
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assert that having a negative age is a failure
    /// Assert(() => Age &lt; 0, nameof(Age), failures,
    ///     n => new ArgumentOutOfRangeException(n, "Age cannot be negative"));
    /// 
    /// // Assert that having end date before start date is a failure
    /// Assert(() => EndDate &lt; StartDate, nameof(EndDate), failures,
    ///     n => new ArgumentException("End date must be after start date"));
    /// </code>
    /// </example>
    public static void Assert(Func<bool> predicate, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (predicate()) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }
}
