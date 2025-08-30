namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class SuccessObjectBuildResult<TClass> : IObjectBuildResult<TClass>
{
    /// <summary>
    /// Get the resulting object from the build operation, or <see langword="null"/> if the build failed.
    /// </summary>
    public TClass Result { get; init; }

    /// </summary>
    public IEnumerable<Exception>? Exceptions { get; init; }

    public SuccessObjectBuildResult(TClass result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a successful build result with the specified result value.
    /// </summary>
    /// <param name="result">The result value to associate with the successful build.</param>
    /// <returns>A <see cref="SuccessObjectBuildResult{TClass}"/> instance representing a successful build, containing the specified
    /// result value.</returns>
    public static SuccessObjectBuildResult<TClass> Success(TClass result)
    {
        return new SuccessObjectBuildResult<TClass>(result);
    }
}
