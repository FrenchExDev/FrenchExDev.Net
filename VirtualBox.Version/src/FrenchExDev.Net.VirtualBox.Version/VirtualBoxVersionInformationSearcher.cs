namespace FrenchexDev.VirtualBox.Net;

/// <summary>
/// Provides functionality to search for VirtualBox version information and retrieve associated checksums from the
/// official VirtualBox download site.
/// </summary>
/// <remarks>This class is intended for use in scenarios where accurate VirtualBox version and checksum data is
/// required, such as validating downloads or automating deployment processes. Instances of this class are thread-safe
/// for concurrent use. Implements the IVirtualBoxVersionInformationSearcher interface.</remarks>
public sealed class VirtualBoxVersionInformationSearcher : IVirtualBoxVersionInformationSearcher
{
    /// <summary>
    /// Represents the URL pattern used to retrieve SHA256 checksum files for VirtualBox releases.
    /// </summary>
    /// <remarks>Replace the #VERSION# placeholder in the pattern with the desired VirtualBox version to
    /// construct the full URL for the corresponding SHA256SUMS file.</remarks>
    private static readonly string ShaChecksumsUrlPattern = "https://www.virtualbox.org/download/hashes/#VERSION#/SHA256SUMS";

    /// <summary>
    /// Stores the HttpClient instance used for making HTTP requests to retrieve version information.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the VirtualBoxVersionInformationSearcher class, optionally using a specified
    /// HttpClient for HTTP requests.
    /// </summary>
    /// <remarks>Providing a custom HttpClient allows for advanced configuration, such as custom handlers,
    /// timeouts, or sharing the client across multiple components. If not specified, the class will manage its own
    /// HttpClient instance.</remarks>
    /// <param name="httpClient">The HttpClient instance to use for sending HTTP requests. If null, a new HttpClient will be created and used
    /// internally.</param>
    public VirtualBoxVersionInformationSearcher(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Asynchronously searches for VirtualBox version information matching the specified filters.
    /// </summary>
    /// <param name="filters">The filters to apply when searching for VirtualBox version information. The <c>ExactVersion</c> property must be
    /// specified and non-empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="VirtualBoxVersionInfos"/> objects matching the search criteria. The list will be empty if no matching
    /// information is found.</returns>
    /// <exception cref="InvalidDataException">Thrown if <paramref name="filters"/>.ExactVersion is null or empty.</exception>
    public async Task<List<VirtualBoxVersionInfos>> SearchAsync(VirtualBoxVersionInformationSearchingFilters filters, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filters.ExactVersion))
            throw new InvalidDataException(nameof(filters.ExactVersion));

        var results = new List<VirtualBoxVersionInfos>();

        var uriVersionChecksums = new Uri(ShaChecksumsUrlPattern.Replace("#VERSION#", filters.ExactVersion));

        var getVersionChecksumsResponseMessage = await _httpClient.GetAsync(uriVersionChecksums, cancellationToken);
        var versionChecksumsInfosHtml = await getVersionChecksumsResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        var versionChecksumsInfosHtmlSplit = versionChecksumsInfosHtml.Split("\n");

        foreach (var versionChecksumInfoHtml in versionChecksumsInfosHtmlSplit)
        {
            var versionChecksumInfosHtmlLineSplit = versionChecksumInfoHtml.Split(" ");
            if (versionChecksumInfosHtmlLineSplit.Length > 0 && !versionChecksumInfosHtmlLineSplit[1].StartsWith("*VBoxGuestAdditions_")) continue;

            var searchResult = new VirtualBoxVersionInfos(
                Version: filters.ExactVersion,
                AdditionsIsoSha256: versionChecksumInfosHtmlLineSplit[0]
            );

            results.Add(searchResult);
            break;
        }

        return results;
    }
}