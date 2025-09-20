using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Provides extension methods for working with collections of <see cref="SuccessObjectBuildResult{TClass}"/> instances.
/// </summary>
public static class SuccessObjectBuildResultExtensions
{
    /// <summary>
    /// Converts a collection of object build results into a list of successfully built objects.
    /// </summary>
    /// <remarks>Only results of type <see cref="SuccessObjectBuildResult{TClass}"/> are included in the
    /// output list. All other result types are ignored.</remarks>
    /// <typeparam name="TClass">The type of the objects contained in the build results.</typeparam>
    /// <param name="results">The collection of object build results to process.</param>
    /// <returns>A list of objects of type <typeparamref name="TClass"/> that were successfully built. The list will be empty if
    /// no successful results are found.</returns>
    public static List<TClass> ToResultList<TClass>(this IEnumerable<IBuildResult> results)
        where TClass : class
        => [.. results
            .OfType<SuccessBuildResult<TClass>>()
            .Select(x => x.Object)];
}
