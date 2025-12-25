namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Provides static validation assertion methods for use in builder validation logic.
/// </summary>
public static class ValidationAssertions
{
    /// <summary>
    /// Checks whether the specified string value is not empty or consists solely of whitespace characters,
    /// and records a failure if the condition is not met.
    /// </summary>
    public static void AssertNotEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (value is null) return;
        if (string.IsNullOrWhiteSpace(value)) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that each string in the specified list is not null, empty, or consists only of white-space characters.
    /// </summary>
    public static void AssertNotEmptyOrWhitespace(List<string>? values, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (values is null) return;
        foreach (var item in values) AssertNotEmptyOrWhitespace(item, name, failures, exceptionBuilder);
    }

    /// <summary>
    /// Asserts that the specified string is not null, empty, or consists only of white-space characters.
    /// </summary>
    public static void AssertNotNullOrEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (string.IsNullOrWhiteSpace(value)) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Checks that the specified value is not null and records a failure if the value is null.
    /// </summary>
    public static void AssertNotNull(object? value, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (value is null) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Validates that the specified collection is not null and not empty.
    /// If the collection contains string elements, validates that none are null, empty, or consist only of white-space characters.
    /// </summary>
    public static void AssertNotNullNotEmptyCollection<T>(List<T>? list, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (list is null) { failures.AddFailure(name, Failure.FromException(exceptionBuilder(name))); return; }
        if (list.Count == 0) { failures.AddFailure(name, Failure.FromException(exceptionBuilder(name))); return; }
        foreach (var item in list)
            if (item is string str && string.IsNullOrWhiteSpace(str))
                failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }

    /// <summary>
    /// Evaluates the specified predicate and, if it returns true, records a failure.
    /// </summary>
    public static void Assert(Func<bool> predicate, string name, IFailureCollector failures, Func<string, Exception> exceptionBuilder)
    {
        if (predicate()) failures.AddFailure(name, Failure.FromException(exceptionBuilder(name)));
    }
}
