using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Infrastructure;

/// <summary>
/// Extension methods for <see cref="IObjectBuildResult{TClass}"/> to simplify success and failure result handling.
/// </summary>
public static class IObjectBuildResultExtensions
{
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
            _ => throw new InvalidOperationException("Not a success object")
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
