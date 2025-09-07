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
}
