namespace FrenchExDev.Net.CSharp.Object.Builder.Testing;

/// <summary>
/// Provides assertion methods for testing <see cref="IBuilder{TClass}"/> implementations and build results.
/// </summary>
/// <remarks>
/// Use these methods in unit tests to verify that builders produce expected results, handle validation correctly,
/// and manage references properly. All assertion methods throw <see cref="BuilderAssertException"/> when the assertion fails.
/// </remarks>
public static class BuilderAssert
{
    #region IResult Assertions

    /// <summary>
    /// Asserts that the specified result represents a successful build operation.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the result is not successful.</exception>
    public static void IsSuccess<TClass>(IResult result, string? message = null) where TClass : class
    {
        if (result is not SuccessResult<TClass>)
        {
            var failureInfo = result is FailureResult f ? FormatFailures(f.Failures) : string.Empty;
            throw new BuilderAssertException(message ?? $"Expected result to be SuccessResult<{typeof(TClass).Name}>, but was {result.GetType().Name}.{failureInfo}");
        }
    }

    /// <summary>
    /// Asserts that the specified result represents a failed build operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the result is not a failure.</exception>
    public static void IsFailure(IResult result, string? message = null)
    {
        if (result is not FailureResult)
        {
            throw new BuilderAssertException(message ?? $"Expected result to be FailureResult, but was {result.GetType().Name}.");
        }
    }

    /// <summary>
    /// Asserts that the specified result is successful and contains the expected object.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the result is not successful or the object does not match.</exception>
    public static void HasObject<TClass>(IResult result, TClass expected, string? message = null) where TClass : class
    {
        IsSuccess<TClass>(result, message);
        var success = (SuccessResult<TClass>)result;
        if (!Equals(success.Object, expected))
        {
            throw new BuilderAssertException(message ?? $"Expected result object to be '{expected}', but was '{success.Object}'.");
        }
    }

    /// <summary>
    /// Asserts that the specified result is successful and the object satisfies the given predicate.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="predicate">A function to test the result object.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the result is not successful or the predicate returns false.</exception>
    public static void HasObjectMatching<TClass>(IResult result, Func<TClass, bool> predicate, string? message = null) where TClass : class
    {
        IsSuccess<TClass>(result, message);
        var success = (SuccessResult<TClass>)result;
        if (!predicate(success.Object))
        {
            throw new BuilderAssertException(message ?? $"Result object '{success.Object}' did not match the expected predicate.");
        }
    }

    /// <summary>
    /// Asserts that the specified result contains a failure with the given member name.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="memberName">The member name to look for in the failures dictionary.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the result does not contain the specified failure.</exception>
    public static void HasFailureForMember(IResult result, string memberName, string? message = null)
    {
        IsFailure(result, message);
        var failure = (FailureResult)result;
        if (!failure.Failures.ContainsKey(memberName))
        {
            throw new BuilderAssertException(message ?? $"Expected result to contain failure for member '{memberName}', but it was not found. Available members: {string.Join(", ", failure.Failures.Keys)}");
        }
    }

    /// <summary>
    /// Asserts that the specified result contains a failure count matching the expected value.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="expectedCount">The expected number of failure entries.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the failure count does not match.</exception>
    public static void HasFailureCount(IResult result, int expectedCount, string? message = null)
    {
        IsFailure(result, message);
        var failure = (FailureResult)result;
        if (failure.Failures.Count != expectedCount)
        {
            throw new BuilderAssertException(message ?? $"Expected {expectedCount} failure entries, but found {failure.Failures.Count}.");
        }
    }

    #endregion

    #region Reference Assertions

    /// <summary>
    /// Asserts that the specified reference is resolved.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the reference is not resolved.</exception>
    public static void IsResolved<TClass>(Reference<TClass> reference, string? message = null) where TClass : class
    {
        if (!reference.IsResolved)
        {
            throw new BuilderAssertException(message ?? $"Expected Reference<{typeof(TClass).Name}> to be resolved, but it was not.");
        }
    }

    /// <summary>
    /// Asserts that the specified reference is not resolved.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the reference is resolved.</exception>
    public static void IsNotResolved<TClass>(Reference<TClass> reference, string? message = null) where TClass : class
    {
        if (reference.IsResolved)
        {
            throw new BuilderAssertException(message ?? $"Expected Reference<{typeof(TClass).Name}> to not be resolved, but it was.");
        }
    }

    /// <summary>
    /// Asserts that the specified reference is resolved and contains the expected instance.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="expected">The expected instance.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the reference is not resolved or the instance does not match.</exception>
    public static void HasInstance<TClass>(Reference<TClass> reference, TClass expected, string? message = null) where TClass : class
    {
        IsResolved(reference, message);
        if (!ReferenceEquals(reference.Instance, expected))
        {
            throw new BuilderAssertException(message ?? $"Expected reference instance to be '{expected}', but was '{reference.Instance}'.");
        }
    }

    #endregion

    #region Builder Assertions

    /// <summary>
    /// Asserts that the builder has the specified build status.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="expectedStatus">The expected build status.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the build status does not match.</exception>
    public static void HasBuildStatus<TClass>(IBuilder<TClass> builder, BuildStatus expectedStatus, string? message = null) where TClass : class
    {
        if (builder.BuildStatus != expectedStatus)
        {
            throw new BuilderAssertException(message ?? $"Expected BuildStatus to be {expectedStatus}, but was {builder.BuildStatus}.");
        }
    }

    /// <summary>
    /// Asserts that the builder has the specified validation status.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="expectedStatus">The expected validation status.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the validation status does not match.</exception>
    public static void HasValidationStatus<TClass>(IBuilder<TClass> builder, ValidationStatus expectedStatus, string? message = null) where TClass : class
    {
        if (builder.ValidationStatus != expectedStatus)
        {
            throw new BuilderAssertException(message ?? $"Expected ValidationStatus to be {expectedStatus}, but was {builder.ValidationStatus}.");
        }
    }

    /// <summary>
    /// Asserts that the builder produces a successful build result.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The built object.</returns>
    /// <exception cref="BuilderAssertException">Thrown when the build fails.</exception>
    public static TClass BuildsSuccessfully<TClass>(IBuilder<TClass> builder, string? message = null) where TClass : class
    {
        var result = builder.Build();
        IsSuccess<TClass>(result, message);
        return ((SuccessResult<TClass>)result).Object;
    }

    /// <summary>
    /// Asserts that the builder produces a failure result.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary.</returns>
    /// <exception cref="BuilderAssertException">Thrown when the build succeeds.</exception>
    public static FailuresDictionary BuildsFailing<TClass>(IBuilder<TClass> builder, string? message = null) where TClass : class
    {
        var result = builder.Build();
        IsFailure(result, message);
        return ((FailureResult)result).Failures;
    }

    #endregion

    #region FailuresDictionary Assertions

    /// <summary>
    /// Asserts that the failures dictionary contains the specified member.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="memberName">The member name to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the member is not found.</exception>
    public static void ContainsMember(FailuresDictionary failures, string memberName, string? message = null)
    {
        if (!failures.ContainsKey(memberName))
        {
            throw new BuilderAssertException(message ?? $"Expected failures to contain member '{memberName}', but it was not found. Available members: {string.Join(", ", failures.Keys)}");
        }
    }

    /// <summary>
    /// Asserts that the failures dictionary contains the specified number of entries.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="expectedCount">The expected number of entries.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the count does not match.</exception>
    public static void HasCount(FailuresDictionary failures, int expectedCount, string? message = null)
    {
        if (failures.Count != expectedCount)
        {
            throw new BuilderAssertException(message ?? $"Expected failures to contain {expectedCount} entries, but found {failures.Count}.");
        }
    }

    /// <summary>
    /// Asserts that the failures dictionary is empty.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the dictionary is not empty.</exception>
    public static void IsEmpty(FailuresDictionary failures, string? message = null)
    {
        if (failures.Count > 0)
        {
            throw new BuilderAssertException(message ?? $"Expected failures to be empty, but found {failures.Count} entries: {string.Join(", ", failures.Keys)}");
        }
    }

    /// <summary>
    /// Asserts that the failures dictionary is not empty.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the dictionary is empty.</exception>
    public static void IsNotEmpty(FailuresDictionary failures, string? message = null)
    {
        if (failures.Count == 0)
        {
            throw new BuilderAssertException(message ?? "Expected failures to not be empty, but it was.");
        }
    }

    /// <summary>
    /// Asserts that the failures dictionary contains an exception of the specified type for the given member.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="memberName">The member name to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception of the specified type.</returns>
    /// <exception cref="BuilderAssertException">Thrown when the exception is not found.</exception>
    public static TException ContainsException<TException>(FailuresDictionary failures, string memberName, string? message = null) where TException : Exception
    {
        ContainsMember(failures, memberName, message);
        var exception = failures[memberName]
            .Select(f => f.Value)
            .OfType<TException>()
            .FirstOrDefault();

        if (exception is null)
        {
            throw new BuilderAssertException(message ?? $"Expected failures for member '{memberName}' to contain exception of type {typeof(TException).Name}, but none was found.");
        }

        return exception;
    }

    #endregion

    #region ReferenceList Assertions

    /// <summary>
    /// Asserts that the reference list contains the specified number of items.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="expectedCount">The expected number of items.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the count does not match.</exception>
    public static void HasCount<TClass>(IReferenceList<TClass> list, int expectedCount, string? message = null) where TClass : class
    {
        if (list.Count != expectedCount)
        {
            throw new BuilderAssertException(message ?? $"Expected reference list to contain {expectedCount} items, but found {list.Count}.");
        }
    }

    /// <summary>
    /// Asserts that the reference list is empty.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the list is not empty.</exception>
    public static void IsEmpty<TClass>(IReferenceList<TClass> list, string? message = null) where TClass : class
    {
        if (list.Count > 0)
        {
            throw new BuilderAssertException(message ?? $"Expected reference list to be empty, but found {list.Count} items.");
        }
    }

    /// <summary>
    /// Asserts that the reference list contains the specified item.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="item">The item to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="BuilderAssertException">Thrown when the item is not found.</exception>
    public static void Contains<TClass>(IReferenceList<TClass> list, TClass item, string? message = null) where TClass : class
    {
        if (!list.Contains(item))
        {
            throw new BuilderAssertException(message ?? $"Expected reference list to contain item '{item}', but it was not found.");
        }
    }

    #endregion

    #region Helpers

    private static string FormatFailures(FailuresDictionary failures)
    {
        if (failures.Count == 0) return string.Empty;
        var entries = failures.Select(kvp => $"{kvp.Key}: [{string.Join(", ", kvp.Value.Select(f => f.Value?.ToString() ?? "null"))}]");
        return $" Failures: {string.Join("; ", entries)}";
    }

    #endregion
}

/// <summary>
/// Exception thrown when a builder assertion fails.
/// </summary>
public class BuilderAssertException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderAssertException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the assertion failure.</param>
    public BuilderAssertException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderAssertException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the assertion failure.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BuilderAssertException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Provides extension methods for fluent builder assertions in tests.
/// </summary>
public static class BuilderAssertExtensions
{
    #region IResult Extensions

    /// <summary>
    /// Asserts that the result represents a successful build operation.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static IResult ShouldBeSuccess<TClass>(this IResult result, string? message = null) where TClass : class
    {
        BuilderAssert.IsSuccess<TClass>(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result represents a failed build operation.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static IResult ShouldBeFailure(this IResult result, string? message = null)
    {
        BuilderAssert.IsFailure(result, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result is successful and returns the built object.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The built object.</returns>
    public static TClass ShouldHaveObject<TClass>(this IResult result, string? message = null) where TClass : class
    {
        BuilderAssert.IsSuccess<TClass>(result, message);
        return ((SuccessResult<TClass>)result).Object;
    }

    /// <summary>
    /// Asserts that the result is successful and the object satisfies the given predicate.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The result to verify.</param>
    /// <param name="predicate">A function to test the result object.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static IResult ShouldHaveObjectMatching<TClass>(this IResult result, Func<TClass, bool> predicate, string? message = null) where TClass : class
    {
        BuilderAssert.HasObjectMatching(result, predicate, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result contains a failure for the specified member.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="memberName">The member name to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The result for further chaining.</returns>
    public static IResult ShouldHaveFailureForMember(this IResult result, string memberName, string? message = null)
    {
        BuilderAssert.HasFailureForMember(result, memberName, message);
        return result;
    }

    /// <summary>
    /// Asserts that the result is a failure and returns the failures dictionary.
    /// </summary>
    /// <param name="result">The result to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary.</returns>
    public static FailuresDictionary ShouldHaveFailures(this IResult result, string? message = null)
    {
        BuilderAssert.IsFailure(result, message);
        return ((FailureResult)result).Failures;
    }

    #endregion

    #region Reference Extensions

    /// <summary>
    /// Asserts that the reference is resolved.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The reference for further chaining.</returns>
    public static Reference<TClass> ShouldBeResolved<TClass>(this Reference<TClass> reference, string? message = null) where TClass : class
    {
        BuilderAssert.IsResolved(reference, message);
        return reference;
    }

    /// <summary>
    /// Asserts that the reference is not resolved.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The reference for further chaining.</returns>
    public static Reference<TClass> ShouldNotBeResolved<TClass>(this Reference<TClass> reference, string? message = null) where TClass : class
    {
        BuilderAssert.IsNotResolved(reference, message);
        return reference;
    }

    /// <summary>
    /// Asserts that the reference is resolved and returns the instance.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced object.</typeparam>
    /// <param name="reference">The reference to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The resolved instance.</returns>
    public static TClass ShouldHaveInstance<TClass>(this Reference<TClass> reference, string? message = null) where TClass : class
    {
        BuilderAssert.IsResolved(reference, message);
        return reference.Instance!;
    }

    #endregion

    #region Builder Extensions

    /// <summary>
    /// Asserts that the builder has the specified build status.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="expectedStatus">The expected build status.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The builder for further chaining.</returns>
    public static IBuilder<TClass> ShouldHaveBuildStatus<TClass>(this IBuilder<TClass> builder, BuildStatus expectedStatus, string? message = null) where TClass : class
    {
        BuilderAssert.HasBuildStatus(builder, expectedStatus, message);
        return builder;
    }

    /// <summary>
    /// Asserts that the builder has the specified validation status.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to verify.</param>
    /// <param name="expectedStatus">The expected validation status.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The builder for further chaining.</returns>
    public static IBuilder<TClass> ShouldHaveValidationStatus<TClass>(this IBuilder<TClass> builder, ValidationStatus expectedStatus, string? message = null) where TClass : class
    {
        BuilderAssert.HasValidationStatus(builder, expectedStatus, message);
        return builder;
    }

    /// <summary>
    /// Builds and asserts the build is successful, returning the built object.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to build and verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The built object.</returns>
    public static TClass ShouldBuildSuccessfully<TClass>(this IBuilder<TClass> builder, string? message = null) where TClass : class
    {
        return BuilderAssert.BuildsSuccessfully(builder, message);
    }

    /// <summary>
    /// Builds and asserts the build fails, returning the failures dictionary.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="builder">The builder to build and verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary.</returns>
    public static FailuresDictionary ShouldBuildFailing<TClass>(this IBuilder<TClass> builder, string? message = null) where TClass : class
    {
        return BuilderAssert.BuildsFailing(builder, message);
    }

    #endregion

    #region FailuresDictionary Extensions

    /// <summary>
    /// Asserts that the failures dictionary contains the specified member.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="memberName">The member name to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary for further chaining.</returns>
    public static FailuresDictionary ShouldContainMember(this FailuresDictionary failures, string memberName, string? message = null)
    {
        BuilderAssert.ContainsMember(failures, memberName, message);
        return failures;
    }

    /// <summary>
    /// Asserts that the failures dictionary contains the specified number of entries.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="expectedCount">The expected number of entries.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary for further chaining.</returns>
    public static FailuresDictionary ShouldHaveCount(this FailuresDictionary failures, int expectedCount, string? message = null)
    {
        BuilderAssert.HasCount(failures, expectedCount, message);
        return failures;
    }

    /// <summary>
    /// Asserts that the failures dictionary is empty.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary for further chaining.</returns>
    public static FailuresDictionary ShouldBeEmpty(this FailuresDictionary failures, string? message = null)
    {
        BuilderAssert.IsEmpty(failures, message);
        return failures;
    }

    /// <summary>
    /// Asserts that the failures dictionary is not empty.
    /// </summary>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The failures dictionary for further chaining.</returns>
    public static FailuresDictionary ShouldNotBeEmpty(this FailuresDictionary failures, string? message = null)
    {
        BuilderAssert.IsNotEmpty(failures, message);
        return failures;
    }

    /// <summary>
    /// Asserts that the failures dictionary contains an exception of the specified type for the given member.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="failures">The failures dictionary to verify.</param>
    /// <param name="memberName">The member name to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The exception of the specified type.</returns>
    public static TException ShouldContainException<TException>(this FailuresDictionary failures, string memberName, string? message = null) where TException : Exception
    {
        return BuilderAssert.ContainsException<TException>(failures, memberName, message);
    }

    #endregion

    #region ReferenceList Extensions

    /// <summary>
    /// Asserts that the reference list contains the specified number of items.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="expectedCount">The expected number of items.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The reference list for further chaining.</returns>
    public static IReferenceList<TClass> ShouldHaveCount<TClass>(this IReferenceList<TClass> list, int expectedCount, string? message = null) where TClass : class
    {
        BuilderAssert.HasCount(list, expectedCount, message);
        return list;
    }

    /// <summary>
    /// Asserts that the reference list is empty.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The reference list for further chaining.</returns>
    public static IReferenceList<TClass> ShouldBeEmpty<TClass>(this IReferenceList<TClass> list, string? message = null) where TClass : class
    {
        BuilderAssert.IsEmpty(list, message);
        return list;
    }

    /// <summary>
    /// Asserts that the reference list contains the specified item.
    /// </summary>
    /// <typeparam name="TClass">The type of the referenced objects.</typeparam>
    /// <param name="list">The reference list to verify.</param>
    /// <param name="item">The item to look for.</param>
    /// <param name="message">Optional custom message to include in the exception if the assertion fails.</param>
    /// <returns>The reference list for further chaining.</returns>
    public static IReferenceList<TClass> ShouldContain<TClass>(this IReferenceList<TClass> list, TClass item, string? message = null) where TClass : class
    {
        BuilderAssert.Contains(list, item, message);
        return list;
    }

    #endregion
}
