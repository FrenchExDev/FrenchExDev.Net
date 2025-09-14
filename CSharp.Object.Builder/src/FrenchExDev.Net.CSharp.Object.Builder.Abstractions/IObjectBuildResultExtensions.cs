namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Extension methods for <see cref="IObjectBuildResult{TClass}"/> to simplify success and failure result handling.
/// </summary>
public static class IObjectBuildResultExtensions
{
    /// <summary>
    /// Returns a list of failed object build results of the specified type from the provided collection.
    /// </summary>
    /// <remarks>Use this method to retrieve only the failed build results from a collection of object build
    /// results, enabling further analysis or error handling.</remarks>
    /// <typeparam name="TClass">The type of object being built. Must be a reference type.</typeparam>
    /// <typeparam name="TBuilder">The type of builder used to construct objects of type <typeparamref name="TClass"/>.</typeparam>
    /// <param name="results">The collection of object build results to filter for failures.</param>
    /// <returns>A list of <see cref="FailureObjectBuildResult{TClass, TBuilder}"/> instances representing failed build results.
    /// The list will be empty if no failures are found.</returns>
    public static List<FailureObjectBuildResult<TClass, TBuilder>> Failures<TClass, TBuilder>(this List<IObjectBuildResult<TClass>> results)
        where TClass : class
        where TBuilder : IObjectBuilder<TClass>
    {
        return results.OfType<FailureObjectBuildResult<TClass, TBuilder>>().ToList();
    }

    /// <summary>
    /// Returns a list of successful object build results from the specified collection.
    /// </summary>
    /// <typeparam name="TClass">The type of object being built.</typeparam>
    /// <typeparam name="TBuilder">The type of builder used to construct objects of type <typeparamref name="TClass"/>.</typeparam>
    /// <param name="results">The collection of object build results to filter for successful results. Cannot be null.</param>
    /// <returns>A list containing all successful object build results in the collection. If no successful results are found, the
    /// list will be empty.</returns>
    public static List<SuccessObjectBuildResult<TClass>> Successes<TClass, TBuilder>(this List<IObjectBuildResult<TClass>> results)
        where TClass : class
        where TBuilder : IObjectBuilder<TClass>
    {
        return results.OfType<SuccessObjectBuildResult<TClass>>().ToList();
    }

    /// <summary>
    /// Returns a list of successfully built objects from the specified collection of object build results.
    /// </summary>
    /// <remarks>Only results of type <see cref="SuccessObjectBuildResult{TClass}"/> are included. Failed or
    /// incomplete build results are excluded from the returned list.</remarks>
    /// <typeparam name="TClass">The type of object produced by the builder.</typeparam>
    /// <typeparam name="TBuilder">The type of builder used to construct objects of type <typeparamref name="TClass"/>.</typeparam>
    /// <param name="results">The collection of object build results to filter for successful results. Cannot be null.</param>
    /// <returns>A list containing the result objects from all successful build results. The list will be empty if there are no
    /// successful results.</returns>
    public static List<TClass> SuccessesResults<TClass, TBuilder>(this List<IObjectBuildResult<TClass>> results)
        where TClass : class
        where TBuilder : IObjectBuilder<TClass>
    {
        return results.OfType<SuccessObjectBuildResult<TClass>>().Select(x => x.Result).ToList();
    }

    /// <summary>
    /// Retrieves the successful result from an <see cref="IObjectBuildResult{TClass}"/>.
    /// Throws an exception if the result is not a success.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <param name="result">The build result to extract from.</param>
    /// <returns>The built object if the result is successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is not a success.</exception>
    public static TClass Success<TClass>(this IObjectBuildResult<TClass> result) where TClass : class
    {
        return result switch
        {
            SuccessObjectBuildResult<TClass> success => success.Result,
            _ => throw new InvalidObjectBuildResultException((IObjectBuildResult<object>)result)
        };
    }

    /// <summary>
    /// Retrieves the failure result from an <see cref="IObjectBuildResult{TClass}"/>.
    /// Throws an exception if the result is not a failure.
    /// </summary>
    /// <typeparam name="TClass">The type of the built object.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder used.</typeparam>
    /// <param name="result">The build result to extract from.</param>
    /// <returns>The failure result if the build failed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is not a failure.</exception>
    public static FailureObjectBuildResult<TClass, TBuilder> Failure<TClass, TBuilder>(this IObjectBuildResult<TClass> result)
        where TClass : class
        where TBuilder : IObjectBuilder<TClass>
    {
        return result switch
        {
            FailureObjectBuildResult<TClass, TBuilder> failure => failure,
            _ => throw new InvalidOperationException("Not a failure object")
        };
    }
}
