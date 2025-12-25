namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// A dictionary that maps member names to their associated validation failures.
/// Implements <see cref="IFailureCollector"/> for collecting failures during validation.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="FailuresDictionary"/> extends <see cref="Dictionary{TKey, TValue}"/> where keys are member names
/// (property names, field names, etc.) and values are lists of <see cref="Failure"/> objects.
/// </para>
/// <para>
/// This class is used throughout the builder validation process to collect and organize validation errors
/// by the member that caused them.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var failures = new FailuresDictionary();
/// 
/// // Add failures using the fluent API
/// failures
///     .Failure("Name", new ArgumentException("Name is required"))
///     .Failure("Age", "Age must be positive")
///     .Failure("Age", new ArgumentOutOfRangeException("Age"));
/// 
/// // Check for failures
/// if (failures.HasFailures)
/// {
///     foreach (var kvp in failures)
///     {
///         Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} error(s)");
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IFailureCollector"/>
/// <seealso cref="Failure"/>
public class FailuresDictionary : Dictionary<string, List<Failure>>, IFailureCollector
{
    /// <summary>
    /// Adds a failure for the specified member name.
    /// </summary>
    /// <param name="memberName">The name of the member (property, field, etc.) that has the failure.</param>
    /// <param name="failure">The <see cref="Failure"/> to add.</param>
    /// <returns>The current <see cref="IFailureCollector"/> instance for method chaining.</returns>
    /// <remarks>
    /// If no failures exist for the specified member, a new list is created.
    /// Multiple failures can be added for the same member.
    /// </remarks>
    public IFailureCollector AddFailure(string memberName, Failure failure)
    {
        if (!TryGetValue(memberName, out var list)) 
        { 
            list = []; 
            this[memberName] = list; 
        }
        list.Add(failure);
        return this;
    }

    /// <summary>
    /// Adds a failure for the specified member name using fluent syntax.
    /// </summary>
    /// <param name="memberName">The name of the member that has the failure.</param>
    /// <param name="failure">The <see cref="Failure"/> to add. Can be implicitly converted from <see cref="Exception"/> or <see cref="string"/>.</param>
    /// <returns>The current <see cref="FailuresDictionary"/> instance for method chaining.</returns>
    /// <example>
    /// <code>
    /// var failures = new FailuresDictionary()
    ///     .Failure("Name", "Name is required")
    ///     .Failure("Email", new FormatException("Invalid email format"))
    ///     .Failure("Age", Failure.FromMessage("Must be positive"));
    /// </code>
    /// </example>
    public FailuresDictionary Failure(string memberName, Failure failure)
    {
        AddFailure(memberName, failure);
        return this;
    }

    /// <summary>
    /// Gets a value indicating whether this dictionary contains any failures.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if there is at least one member with failures; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasFailures => Count > 0;

    /// <summary>
    /// Gets the number of members that have failures.
    /// </summary>
    /// <value>The count of distinct members with recorded failures.</value>
    /// <remarks>
    /// This returns the number of keys in the dictionary, not the total number of individual failures.
    /// To get the total failure count across all members, iterate through the values and sum their counts.
    /// </remarks>
    public int FailureCount => Count;
}
