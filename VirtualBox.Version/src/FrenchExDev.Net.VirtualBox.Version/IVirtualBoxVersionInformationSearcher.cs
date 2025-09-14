namespace FrenchexDev.VirtualBox.Net;

/// <summary>
/// Defines a contract for searching VirtualBox version information based on specified filters.
/// </summary>
/// <remarks>Implementations of this interface provide asynchronous search capabilities for VirtualBox version
/// details. The search operation can be customized using filtering criteria and supports cancellation via a
/// cancellation token.</remarks>
public interface IVirtualBoxVersionInformationSearcher
{
    /// <summary>
    /// Asynchronously searches for VirtualBox version information that matches the specified filters.
    /// </summary>
    /// <param name="filters">The criteria used to filter VirtualBox version information results. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of VirtualBoxVersionInfos
    /// objects matching the specified filters. The list will be empty if no matches are found.</returns>
    Task<List<VirtualBoxVersionInfos>> SearchAsync(
        VirtualBoxVersionInformationSearchingFilters filters, CancellationToken cancellationToken = default);
}