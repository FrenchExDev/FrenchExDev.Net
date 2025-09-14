using FrenchexDev.VirtualBox.Net;

namespace FrenchExDev.Net.VirtualBox.Version.Testing;

/// <summary>
/// Provides helper methods for testing VirtualBox version information search functionality.
/// </summary>
/// <remarks>This class is intended for use in unit tests to verify the behavior of VirtualBox version information
/// searching operations. All methods are static and designed to facilitate test scenarios by abstracting common setup
/// and assertion patterns.</remarks>
public static class VirtualBoxVersionInformationSearcherTester
{
    /// <summary>
    /// Executes an asynchronous search for VirtualBox version information using the specified filter, and applies the
    /// provided assertion action to the search results.
    /// </summary>
    /// <remarks>This method is intended for scenarios such as automated testing, where custom filters and
    /// assertions are applied to VirtualBox version information search results. The search is performed asynchronously,
    /// and the assertion action is invoked after the results are retrieved.</remarks>
    /// <param name="filterBody">A delegate that returns the filters to apply when searching for VirtualBox version information. The returned
    /// filters determine which version information records are retrieved.</param>
    /// <param name="assertBody">An action to perform on the list of VirtualBox version information results. Typically used to assert conditions
    /// or validate the search results.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task ValidAsync(Func<VirtualBoxVersionInformationSearchingFilters> filterBody, Action<List<VirtualBoxVersionInfos>> assertBody)
    {
        var filters = filterBody();
        var searcher = new VirtualBoxVersionInformationSearcher();
        List<VirtualBoxVersionInfos> results = await searcher.SearchAsync(filters, CancellationToken.None);
        assertBody(results);
    }
}
