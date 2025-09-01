namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class SuccessObjectBuildResult<TClass> : IObjectBuildResult<TClass>
{
    /// <summary>
    /// Get the resulting object from the build operation, or <see langword="null"/> if the build failed.
    /// </summary>
    public TClass Result { get; init; }

    /// <summary>
    /// Enumerates any exceptions that occurred during the build process, or <see langword="null"/> if no exceptions were encountered.
    /// </summary>
    public IEnumerable<Exception>? Exceptions { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessObjectBuildResult{TClass}"/> class  with the specified
    /// result.
    /// </summary>
    /// <param name="result">The result object representing the successful outcome of the operation.</param>
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
