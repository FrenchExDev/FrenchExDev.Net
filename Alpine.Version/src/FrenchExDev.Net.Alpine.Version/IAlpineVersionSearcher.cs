#region Licensing

// Copyright Stéphâne Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Defines a contract for searching Alpine Linux versions, architectures, and flavors from the official Alpine repository.
/// </summary>
/// <remarks>
/// Implementations should provide logic to filter Alpine versions by version, architecture, flavor, and release candidate status.
/// </remarks>
public interface IAlpineVersionSearcher
{
    /// <summary>
    /// Searches for Alpine Linux versions matching the provided filters.
    /// </summary>
    /// <remarks>
    /// This method should query the Alpine Linux repository, apply the specified filters, and return matching results.
    /// <para>Example usage:
    /// <code>
    /// var filters = new AlpineVersionSearchingFiltersBuilder()
    ///     .WithMinimumVersion("3.18")
    ///     .WithArch(AlpineArchitectures.x86_64)
    ///     .Build();
    /// var results = await searcher.SearchAsync(filters);
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="searchFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="cancellationToken">Optional cancellation token for async operations.</param>
    /// <returns>List of matching <see cref="AlpineVersionArchFlavorRecord"/> results.</returns>
    Task<AlpineVersionList> SearchAsync(
        AlpineVersionSearchingFilters searchFilters,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Searches for Alpine versions based on the specified filtering criteria.
    /// </summary>
    /// <remarks>This method performs an asynchronous search operation. Ensure that the <paramref
    /// name="configureSearchFilter"/>  delegate is properly configured to define the desired search criteria. The
    /// operation can be canceled by passing  a cancellation token.</remarks>
    /// <param name="configureSearchFilter">A delegate that configures the search filters using an <see cref="AlpineVersionSearchingFiltersBuilder"/>. The
    /// delegate must define the filtering criteria for the search.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AlpineVersionList"/> 
    /// with the versions that match the specified filters. If no versions match, the result will be an empty list.</returns>
    Task<AlpineVersionList> SearchAsync(
        Action<AlpineVersionSearchingFiltersBuilder> configureSearchFilter,
        CancellationToken cancellationToken = default
    );
}
