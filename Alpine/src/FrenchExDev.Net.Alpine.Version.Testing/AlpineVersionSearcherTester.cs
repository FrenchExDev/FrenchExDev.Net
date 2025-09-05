namespace FrenchExDev.Net.Alpine.Version.Testing;

/// <summary>
/// Provides utility methods for testing the AlpineVersionSearcher with custom filters and assertions.
/// </summary>
/// <remarks>
/// This static class allows you to build search filters using <see cref="AlpineVersionSearchingFiltersBuilder"/>,
/// execute a search asynchronously, and assert the results using a provided delegate.
/// </remarks>
public static class AlpineVersionSearcherTester
{
    /// <summary>
    /// Runs an asynchronous test for AlpineVersionSearcher using the specified filter builder and assertion.
    /// </summary>
    /// <param name="filterBuilder">Action to configure the filter builder.</param>
    /// <param name="assert">Action to assert the search results.</param>
    /// <param name="cancellationToken">Optional cancellation token for the search operation.</param>
    public static async Task TestValidAsync(
        IAlpineVersionSearcher searcher,
        Action<AlpineVersionSearchingFiltersBuilder> filterBuilder,
        Action<AlpineVersionList> assert,
        CancellationToken cancellationToken = default
    )
    {
        var result = await searcher.SearchAsync(filterBuilder, cancellationToken);
        assert(result);
    }

    /// <summary>
    /// Specifies the comparison operators that can be used in conditional expressions.
    /// </summary>
    /// <remarks>This enumeration defines a set of operators commonly used for comparing values.  It can be
    /// used in scenarios such as filtering, sorting, or evaluating conditions.</remarks>
    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    /// <summary>
    /// Compares two <see cref="AlpineVersion"/> instances using the specified comparison operator.
    /// </summary>
    /// <param name="left">The first <see cref="AlpineVersion"/> instance to compare.</param>
    /// <param name="operator">The <see cref="Operator"/> that specifies the type of comparison to perform. Supported operators include <see
    /// cref="Operator.Equal"/>, <see cref="Operator.NotEqual"/>,  <see cref="Operator.GreaterThan"/>, <see
    /// cref="Operator.GreaterThanOrEqual"/>,  <see cref="Operator.LessThan"/>, and <see
    /// cref="Operator.LessThanOrEqual"/>.</param>
    /// <param name="right">The second <see cref="AlpineVersion"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the comparison between <paramref name="left"/> and <paramref name="right"/>  evaluates
    /// to true based on the specified <paramref name="operator"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if the specified <paramref name="operator"/> is not a supported comparison operator.</exception>
    public static bool Compare(AlpineVersion left, Operator @operator, AlpineVersion right)
    {
        return (@operator) switch
        {
            Operator.Equal => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) == 0,
            Operator.NotEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) != 0,
            Operator.GreaterThan => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) > 0,
            Operator.GreaterThanOrEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) >= 0,
            Operator.LessThan => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) < 0,
            Operator.LessThanOrEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) <= 0,
            _ => throw new NotSupportedException($"Unsupported operator: {@operator}")
        };
    }
}
