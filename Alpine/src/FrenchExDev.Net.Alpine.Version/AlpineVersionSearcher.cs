#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

using System.Collections.Concurrent;
using System.Text.RegularExpressions;


#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Provides functionality to search for Alpine Linux versions, architectures, and flavors from the official Alpine repository.
/// </summary>
/// <remarks>
/// This class implements <see cref="IAlpineVersionSearcher"/> and allows filtering by version, architecture, flavor, and release candidate status.
/// It fetches and parses HTML from the Alpine Linux CDN, applies filters, and returns matching records with version, architecture, flavor, and SHA checksums.
/// </remarks>
public class AlpineVersionSearcher : IAlpineVersionSearcher
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of <see cref="AlpineVersionSearcher"/> with the specified HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for web requests.</param>
    public AlpineVersionSearcher(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Asynchronously searches for Alpine versions based on the specified search filters.
    /// </summary>
    /// <remarks>This method allows you to define custom search criteria by configuring an <see
    /// cref="AlpineVersionSearchingFiltersBuilder"/>. The search operation is performed asynchronously, and the results
    /// are returned as an <see cref="AlpineVersionList"/>.</remarks>
    /// <param name="configureSearchFilter">A delegate that configures the search filters using an <see cref="AlpineVersionSearchingFiltersBuilder"/>. The
    /// delegate must define the criteria for the search.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AlpineVersionList"/>
    /// with the search results.</returns>
    public Task<AlpineVersionList> SearchAsync(Action<AlpineVersionSearchingFiltersBuilder> configureSearchFilter, CancellationToken cancellationToken = default)
    {
        var builder = new AlpineVersionSearchingFiltersBuilder();
        configureSearchFilter(builder);
        var filters = builder.Build();
        return SearchAsync(filters, cancellationToken);
    }


    /// <summary>
    /// Searches for Alpine Linux versions matching the provided filters.
    /// </summary>
    /// <remarks>
    /// This method fetches the Alpine Linux repository index, parses available versions, architectures, and flavors,
    /// and applies the provided filters for version, architecture, flavor, and release candidate status.
    /// <para>
    /// Example usage:
    /// <code>
    /// var filters = new AlpineVersionSearchingFiltersBuilder()
    ///     .WithMinimumVersion("3.18")
    ///     .WithArchitectures(new[] { AlpineArchitectures.x86_64 })
    ///     .Build();
    /// var results = await searcher.SearchAsync(filters);
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="cancellationToken">Optional cancellation token for async operations.</param>
    /// <returns>List of matching <see cref="AlpineVersionArchFlavorRecord"/> results.</returns>
    public async Task<AlpineVersionList> SearchAsync(
        AlpineVersionSearchingFilters alpineVersionSearchingFilters,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(alpineVersionSearchingFilters);

        var getAlpineVersionsHtmlResponse =
            await _httpClient.GetAsync("https://dl-cdn.alpinelinux.org/alpine/", cancellationToken);

        var getAlpineVersionsHtml = await getAlpineVersionsHtmlResponse.Content.ReadAsStringAsync(cancellationToken);

        var exclude = new List<string> { "latest-stable/", "MIRRORS.txt", "last-updated", "../", ".", ".." };

        var includedArchitectures = alpineVersionSearchingFilters
                                        .Architectures
                                        ?.Select(x => x.ToString().ToLowerInvariant()).ToList()
                                    ?? [];

        var includedFlavors = alpineVersionSearchingFilters
                                  .Flavors
                                  ?.Select(x => x.ToString().ToLowerInvariant()).ToList()
                              ?? [];

        var regexUrls = new Regex("<a\\s+(?:[^>]*?\\s+)?href=([\"'])(.*?)\\1");

        var getAlpineVersionMatches = regexUrls.Matches(getAlpineVersionsHtml);

        if (getAlpineVersionMatches.Count == 0) return new AlpineVersionList();

        var minimalVersionObj = alpineVersionSearchingFilters.MinimumVersion;
        var maximumVersionObj = alpineVersionSearchingFilters.MaximumVersion;
        var exactVersionObj = alpineVersionSearchingFilters?.ExactVersion;

        var exactVersion = exactVersionObj is not null
            ? new AlpineVersion
            {
                Major = exactVersionObj.Major,
                Minor = exactVersionObj.Minor,
                Patch = exactVersionObj.Patch
            }
            : null;

        var minimalVersionMajorMinorObj = new AlpineVersion
        {
            Major = minimalVersionObj?.Major ?? "3",
            Minor = minimalVersionObj?.Minor ?? "0",
            Patch = string.Empty
        };

        var maximalVersionMajorMinorObj = new AlpineVersion
        {
            Major = maximumVersionObj?.Major ?? "edge",
            Minor = maximumVersionObj?.Minor ?? string.Empty,
            Patch = string.Empty
        };

        var listAlpineVersions = GetMatchingAlpineVersions(
            alpineVersionSearchingFilters,
            getAlpineVersionMatches,
            exclude,
            maximumVersionObj,
            maximalVersionMajorMinorObj,
            minimalVersionObj,
            minimalVersionMajorMinorObj,
            exactVersion
        ).ToList();

        var listAlpineVersionArchitectures = await QueryWebForAlpineVersionsForArchitecture(
            listAlpineVersions,
            regexUrls,
            exclude,
            includedArchitectures,
            cancellationToken
        );

        ArgumentNullException.ThrowIfNull(alpineVersionSearchingFilters);

        var listAlpineVersionArchFlavors = await QueryAlpineVersionsMatchingFilters(
            alpineVersionSearchingFilters,
            listAlpineVersionArchitectures,
            regexUrls,
            exclude,
            includedFlavors,
            cancellationToken
        );

        return new AlpineVersionList(listAlpineVersionArchFlavors
            .OrderBy(x => x.Version.AsAlpineVersion())
            .ToList());
    }

    /// <summary>
    /// Queries Alpine repository for available flavors matching the provided filters and architectures.
    /// </summary>
    /// <remarks>
    /// This method iterates over each architecture and version, fetches available flavors, and applies flavor and RC filters.
    /// Results are returned as a concurrent bag for thread safety during parallel execution.
    /// </remarks>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="listAlpineVersionArchitectures">List of version-architecture pairs to query.</param>
    /// <param name="regexUrls">Regex for extracting URLs from HTML.</param>
    /// <param name="exclude">List of entries to exclude from results.</param>
    /// <param name="includedFlavors">List of flavors to include.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Concurrent bag of matching <see cref="AlpineVersionArchFlavorRecord"/> results.</returns>
    private async Task<ConcurrentBag<AlpineVersionArchFlavorRecord>> QueryAlpineVersionsMatchingFilters(
        AlpineVersionSearchingFilters alpineVersionSearchingFilters,
        ConcurrentBag<AlpineVersionArchRecord> listAlpineVersionArchitectures,
        Regex regexUrls,
        List<string> exclude,
        List<string> includedFlavors,
        CancellationToken cancellationToken = default)
    {
        var results = new ConcurrentBag<AlpineVersionArchFlavorRecord>();
        var regexAlpineFlavorVersion = new Regex("alpine-(.*)-(.*)-(.*).iso");
        var regexAlpineVersion = new Regex("alpine-(.*)-(.*).iso");

        await Parallel.ForEachAsync(listAlpineVersionArchitectures,
            new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = 10 },
            async (alpineVersionArch, cT) =>
            {
                var alpineVersionObjectRoot = AlpineVersion.From(alpineVersionArch.Version);

                var getAlpineVersionArchFlavorsHtmlResponse =
                    await _httpClient.GetAsync(
                        $"https://dl-cdn.alpinelinux.org/alpine/{alpineVersionObjectRoot.ToMajorMinorUrl()}/releases/{alpineVersionArch.Architecture}/",
                        cT);

                var getAlpineVersionArchFlavorsHtml =
                    await getAlpineVersionArchFlavorsHtmlResponse.Content.ReadAsStringAsync(cT);
                var getAlpineVersionArchFlavorMatches = regexUrls.Matches(getAlpineVersionArchFlavorsHtml);

                if (getAlpineVersionArchFlavorMatches.Count <= 0) return;

                await Parallel.ForEachAsync(getAlpineVersionArchFlavorMatches,
                    new ParallelOptions { CancellationToken = cT, MaxDegreeOfParallelism = 10 },
                    async (match, token) =>
                    {
                        var rawFlavor = match.Groups[2].Value;
                        if (string.IsNullOrEmpty(rawFlavor)) return;
                        if (exclude.Contains(rawFlavor)) return;
                        if (!rawFlavor.EndsWith(".iso")) return;
                        if (rawFlavor.EndsWith('/')) return;
                        bool flowControl = await ProcessAlpineFlavorVersionAsync(alpineVersionSearchingFilters, includedFlavors, alpineVersionArch, results, regexAlpineFlavorVersion, regexAlpineVersion, alpineVersionObjectRoot, rawFlavor, cT);
                        if (!flowControl)
                        {
                            return;
                        }
                    });
            });

        return results;
    }

    /// <summary>
    /// Builds a new record for a specific Alpine version, architecture, and flavor, and adds it to the results.
    /// </summary>
    /// <remarks>
    /// This method parses the flavor and version from the match, applies flavor and RC filters, and fetches SHA checksums.
    /// </remarks>
    /// <param name="alpineFlavorVersionMatches">Regex matches for flavor-version pattern.</param>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="includedFlavors">List of flavors to include.</param>
    /// <param name="alpineVersionArch">Version-architecture pair.</param>
    /// <param name="results">Concurrent bag to add results to.</param>
    /// <param name="regexAlpineFlavorVersion">Regex for flavor-version extraction.</param>
    /// <param name="regexAlpineVersion">Regex for version extraction.</param>
    /// <param name="alpineVersionObjectRoot">Parsed Alpine version object.</param>
    /// <param name="rawFlavor">Raw flavor string from HTML.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    private async Task BuildNewFlavoredRecordAsync(
        MatchCollection alpineFlavorVersionMatches,
        AlpineVersionSearchingFilters alpineVersionSearchingFilters,
        List<string> includedFlavors,
        AlpineVersionArchRecord alpineVersionArch,
        ConcurrentBag<AlpineVersionArchFlavorRecord> results,
        Regex regexAlpineFlavorVersion,
        Regex regexAlpineVersion,
        AlpineVersion alpineVersionObjectRoot,
        string rawFlavor,
        CancellationToken cancellationToken = default
    )
    {
        var version = alpineFlavorVersionMatches[0].Groups[2].Value;
        var versionObject = AlpineVersion.From(version);
        var flavor = alpineFlavorVersionMatches[0].Groups[1].Value;

        if (!versionObject.IsEdge && includedFlavors.Count > 0 &&
            !includedFlavors.Contains(flavor))
            return;

        if (!alpineVersionSearchingFilters.Rc && versionObject.IsRcNumber)
            return;

        var diffWithExact = alpineVersionSearchingFilters.ExactVersion is not null
            ? AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.ExactVersion, versionObject)
            : -2;

        if (alpineVersionObjectRoot.IsEdge &&
            alpineVersionSearchingFilters.ExactVersion is not null) diffWithExact = 0;

        var diffWithMinimum = alpineVersionSearchingFilters.ExactVersion is null
            ? AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.MinimumVersion,
                versionObject)
            : -2;

        var diffWithMaximum = alpineVersionSearchingFilters.ExactVersion is null
            ? AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.MaximumVersion,
                versionObject)
            : -2;

        var shouldAdd = alpineVersionSearchingFilters.ExactVersion is not null
            ? diffWithExact == 0
            : alpineVersionSearchingFilters.MinimumVersion is not null && diffWithMinimum <= 0
              ||
              alpineVersionSearchingFilters.MaximumVersion is not null && diffWithMaximum >= 0
              ||
              alpineVersionSearchingFilters.ExactVersion is null
               && alpineVersionSearchingFilters.MinimumVersion is null
               && alpineVersionSearchingFilters.MaximumVersion is null;

        if (!shouldAdd)
        {
            return;
        }

        var shaRecords = await GetShaRecordsAsync(alpineVersionArch, rawFlavor, cancellationToken);

        results.Add(new AlpineVersionArchFlavorRecord(
            version,
            alpineVersionArch.Architecture,
            flavor,
            $"https://dl-cdn.alpinelinux.org/alpine/{alpineVersionObjectRoot.ToMajorMinorUrl()}/releases/{alpineVersionArch.Architecture}/{rawFlavor}",
            shaRecords.Sha256,
            shaRecords.Sha512
        ));
    }

    /// <summary>
    /// Builds a new record for a specific Alpine version and architecture (without flavor), and adds it to the results.
    /// </summary>
    /// <remarks>
    /// This method parses the version from the match, applies RC and version filters, and fetches SHA checksums.
    /// </remarks>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="includedFlavors">List of flavors to include.</param>
    /// <param name="alpineVersionArch">Version-architecture pair.</param>
    /// <param name="results">Concurrent bag to add results to.</param>
    /// <param name="regexAlpineFlavorVersion">Regex for flavor-version extraction.</param>
    /// <param name="regexAlpineVersion">Regex for version extraction.</param>
    /// <param name="alpineVersionObjectRoot">Parsed Alpine version object.</param>
    /// <param name="rawFlavor">Raw flavor string from HTML.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    private async Task BuildNewRecordAsync(
        AlpineVersionSearchingFilters alpineVersionSearchingFilters,
        List<string> includedFlavors,
        AlpineVersionArchRecord alpineVersionArch,
        ConcurrentBag<AlpineVersionArchFlavorRecord> results,
        Regex regexAlpineFlavorVersion,
        Regex regexAlpineVersion,
        AlpineVersion alpineVersionObjectRoot,
        string rawFlavor,
        CancellationToken cancellationToken = default
    )
    {
        var alpineVersionMatches = regexAlpineVersion.Matches(rawFlavor);

        if (alpineVersionMatches.Count <= 0)
        {
            return;
        }

        var version = alpineVersionMatches[0].Groups[0].Value;

        var versionObject = new AlpineVersion
        {
            Major = version.Split(".")[0],
            Minor = version.Split(".")[1],
            Patch = version.Split(".")[2]
        };

        if (!alpineVersionSearchingFilters.Rc && versionObject.IsRcNumber) return;

        var diffWithExact = alpineVersionSearchingFilters.ExactVersion is not null
            ? AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.ExactVersion, versionObject)
            : -2;

        var diffWithMinimum =
            AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.MinimumVersion,
                versionObject);

        var diffWithMaximum =
            AlpineVersion.MajorMinorPatchComparer.Compare(
                alpineVersionSearchingFilters.MaximumVersion,
                versionObject);

        var shouldAdd = alpineVersionSearchingFilters.ExactVersion is not null
            ? diffWithExact == 0
            : alpineVersionSearchingFilters.MinimumVersion is not null && diffWithMinimum <= 0
              ||
              alpineVersionSearchingFilters.MaximumVersion is not null && diffWithMaximum <= 0
              ||
              alpineVersionSearchingFilters.ExactVersion is null
               && alpineVersionSearchingFilters.MinimumVersion is null
               && alpineVersionSearchingFilters.MaximumVersion is null;

        if (!shouldAdd)
        {
            return;
        }

        var shaRecords = await GetShaRecordsAsync(alpineVersionArch, rawFlavor, cancellationToken);

        results.Add(new AlpineVersionArchFlavorRecord(
            alpineVersionArch.Version,
            alpineVersionArch.Architecture,
            string.Empty,
            $"https://dl-cdn.alpinelinux.org/alpine/{versionObject.ToMajorMinorUrl()}/releases/{alpineVersionArch.Architecture}/{rawFlavor}",
            shaRecords.Sha256,
            shaRecords.Sha512
        ));
    }

    /// <summary>
    /// Processes a flavor-version match and builds the corresponding record.
    /// </summary>
    /// <remarks>
    /// This method determines whether the match is for a flavored or non-flavored ISO and delegates to the appropriate builder method.
    /// </remarks>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="includedFlavors">List of flavors to include.</param>
    /// <param name="alpineVersionArch">Version-architecture pair.</param>
    /// <param name="results">Concurrent bag to add results to.</param>
    /// <param name="regexAlpineFlavorVersion">Regex for flavor-version extraction.</param>
    /// <param name="regexAlpineVersion">Regex for version extraction.</param>
    /// <param name="alpineVersionObjectRoot">Parsed Alpine version object.</param>
    /// <param name="rawFlavor">Raw flavor string from HTML.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if any results were added; otherwise, false.</returns>
    private async Task<bool> ProcessAlpineFlavorVersionAsync(
        AlpineVersionSearchingFilters alpineVersionSearchingFilters,
        List<string> includedFlavors,
        AlpineVersionArchRecord alpineVersionArch,
        ConcurrentBag<AlpineVersionArchFlavorRecord> results,
        Regex regexAlpineFlavorVersion,
        Regex regexAlpineVersion,
        AlpineVersion alpineVersionObjectRoot,
        string rawFlavor,
        CancellationToken cancellationToken = default
    )
    {
        var alpineFlavorVersionMatches = regexAlpineFlavorVersion.Matches(rawFlavor);
        if (alpineFlavorVersionMatches.Count > 0)
        {
            await BuildNewFlavoredRecordAsync(
                alpineFlavorVersionMatches,
                alpineVersionSearchingFilters,
                includedFlavors,
                alpineVersionArch,
                results,
                regexAlpineFlavorVersion,
                regexAlpineVersion,
                alpineVersionObjectRoot,
                rawFlavor,
                cancellationToken);
        }
        else
        {
            await BuildNewRecordAsync(alpineVersionSearchingFilters,
                includedFlavors,
                alpineVersionArch,
                results,
                regexAlpineFlavorVersion,
                regexAlpineVersion,
                alpineVersionObjectRoot,
                rawFlavor,
                cancellationToken);
        }

        return results.Any();
    }

    /// <summary>
    /// Queries the Alpine repository for available architectures for each version.
    /// </summary>
    /// <remarks>
    /// This method fetches the list of architectures for each version and applies architecture filters.
    /// Results are returned as a concurrent bag for thread safety during parallel execution.
    /// <para>
    /// Example usage:
    /// <code>
    /// var archRecords = await QueryWebForAlpineVersionsForArchitecture(versions, regex, exclude, ["x86_64"]);
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="listAlpineVersions">List of Alpine versions to query.</param>
    /// <param name="regexUrls">Regex for extracting URLs from HTML.</param>
    /// <param name="exclude">List of entries to exclude from results.</param>
    /// <param name="includedArchitectures">List of architectures to include.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Concurrent bag of <see cref="AlpineVersionArchRecord"/> results.</returns>
    private async Task<ConcurrentBag<AlpineVersionArchRecord>> QueryWebForAlpineVersionsForArchitecture(
        List<AlpineVersionRecord> listAlpineVersions,
        Regex regexUrls,
        List<string> exclude,
        List<string> includedArchitectures,
        CancellationToken cancellationToken = default)
    {
        var result = new ConcurrentBag<AlpineVersionArchRecord>();

        await Parallel.ForEachAsync(listAlpineVersions,
            new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = 10 },
            async (alpineVersion, ct) =>
            {
                var alpineVersionObj = AlpineVersion.From(alpineVersion.Version);
                var getAlpineVersionArchsHtmlResponse = await _httpClient.GetAsync(
                    $"https://dl-cdn.alpinelinux.org/alpine/{alpineVersionObj.ToMajorMinorUrl()}/releases/", ct);
                var getAlpineVersionArchsHtml = await getAlpineVersionArchsHtmlResponse.Content.ReadAsStringAsync(ct);
                var getAlpineVersionArchsMatches = regexUrls.Matches(getAlpineVersionArchsHtml);
                if (getAlpineVersionArchsMatches.Count > 0)
                    foreach (Match match in getAlpineVersionArchsMatches)
                    {
                        var rawArch = match.Groups[2].Value.Replace("/", "");
                        if (string.IsNullOrEmpty(rawArch)) continue;
                        if (exclude.Contains(rawArch)) continue;

                        if (rawArch.EndsWith("/")) rawArch = rawArch.TrimEnd('/');

                        if (includedArchitectures.Count > 0 && !includedArchitectures.Contains(rawArch)) continue;

                        var newAlpineVersion = new AlpineVersionArchRecord(alpineVersion.Version, rawArch);
                        result.Add(newAlpineVersion);
                    }
            });

        return result;
    }

    /// <summary>
    /// Gets a filtered list of Alpine versions matching the provided filters.
    /// </summary>
    /// <remarks>
    /// This static method parses the HTML matches for available versions and applies minimum, maximum, and exact version filters.
    /// </remarks>
    /// <param name="alpineVersionSearchingFilters">Filters for version, architecture, flavor, and RC status.</param>
    /// <param name="getAlpineVersionMatches">Regex matches for version URLs.</param>
    /// <param name="exclude">List of entries to exclude from results.</param>
    /// <param name="maximumVersionObj">Maximum version filter.</param>
    /// <param name="maximalVersionMajorMinorObj">Maximum version object for comparison.</param>
    /// <param name="minimalVersionObj">Minimum version filter.</param>
    /// <param name="minimalVersionMajorMinorObj">Minimum version object for comparison.</param>
    /// <param name="exactVersionObj">Exact version filter.</param>
    /// <returns>Enumerable of <see cref="AlpineVersionRecord"/> results.</returns>
    private static IEnumerable<AlpineVersionRecord> GetMatchingAlpineVersions(
        AlpineVersionSearchingFilters? alpineVersionSearchingFilters,
        MatchCollection getAlpineVersionMatches,
        List<string> exclude,
        AlpineVersion? maximumVersionObj,
        AlpineVersion maximalVersionMajorMinorObj,
        AlpineVersion? minimalVersionObj,
        AlpineVersion minimalVersionMajorMinorObj,
        AlpineVersion? exactVersionObj
    )
    {
        var results = new List<AlpineVersionRecord>();

        var exactVersionObjWithoutRc = exactVersionObj is not null
            ? new AlpineVersion
            {
                Major = exactVersionObj.Major,
                Minor = exactVersionObj.Minor,
                Patch = string.Empty
            }
            : null;

        foreach (Match match in getAlpineVersionMatches)
        {
            if (match.Groups.Count == 3 && exclude.Contains(match.Groups[2].Value)) continue;

            var rawVersion = match.Groups[2].Value.Replace("/", "");
            if (rawVersion[0] == 'v') rawVersion = rawVersion.Substring(1);

            var rawVersionInts = rawVersion.Split(".");

            var versionObject = new AlpineVersion
            {
                Major = rawVersionInts[0],
                Minor = rawVersionInts.Length > 1 ? rawVersionInts[1] : string.Empty,
                Patch = string.Empty //rawVersionInts.Length > 2 ? rawVersionInts[2] : string.Empty
            };

            var versionIsRc = versionObject.IsRcNumber;
            if (alpineVersionSearchingFilters?.Rc == false && versionIsRc)
                continue;

            var versionObjectComparedToExactVersion = exactVersionObj is not null
                ? AlpineVersion.MajorMinorPatchComparer.Compare(versionObject, exactVersionObjWithoutRc)
                : -2;
            if (versionObjectComparedToExactVersion != -2 && versionObjectComparedToExactVersion == 0)
            {
                results.Add(new AlpineVersionRecord(rawVersion));
                continue;
            }

            if (exactVersionObj is not null)
                continue;

            var versionObjectComparedToMaximumVersion = maximumVersionObj is not null
                ? AlpineVersion.MajorMinorPatchComparer.Compare(versionObject, maximalVersionMajorMinorObj)
                : -2;
            if (versionObjectComparedToMaximumVersion != -2 && versionObjectComparedToMaximumVersion > 0)
                continue;

            var versionObjectComparedMinimalVersion = minimalVersionObj is not null
                ? AlpineVersion.MajorMinorPatchComparer.Compare(versionObject, minimalVersionMajorMinorObj)
                : -2;
            if (versionObjectComparedMinimalVersion != -2 && versionObjectComparedMinimalVersion < 0)
                continue;

            results.Add(new AlpineVersionRecord(rawVersion));
        }

        return results;
    }

    /// <summary>
    /// Fetches SHA256 and SHA512 checksums for a given Alpine version, architecture, and flavor.
    /// </summary>
    /// <remarks>
    /// This method performs parallel HTTP requests to retrieve SHA256 and SHA512 checksums for the specified ISO file.
    /// </remarks>
    /// <param name="alpineVersionArch">Version-architecture pair.</param>
    /// <param name="rawFlavor">Raw flavor string from HTML.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>SHA256 and SHA512 checksums as a <c>ShaRecords</c> record.</returns>
    private async Task<ShaRecords> GetShaRecordsAsync(
        AlpineVersionArchRecord alpineVersionArch,
        string rawFlavor,
        CancellationToken cancellationToken = default
    )
    {
        var versionObject = AlpineVersion.From(alpineVersionArch.Version);
        var sha256Task = Task.Run(async () =>
        {
            var sha256Url =
                $"https://dl-cdn.alpinelinux.org/alpine/{versionObject.ToMajorMinorUrl()}/releases/{alpineVersionArch.Architecture}/{rawFlavor}.sha256";
            var sha256ResponseMessage = await _httpClient.GetAsync(sha256Url, cancellationToken);
            var sha256 = sha256ResponseMessage.IsSuccessStatusCode
                ? await sha256ResponseMessage.Content.ReadAsStringAsync(cancellationToken)
                : string.Empty;
            return sha256.Split(" ").First().Trim();
        }, cancellationToken);

        var sha512Task = Task.Run(async () =>
        {
            var sha512Url =
                $"https://dl-cdn.alpinelinux.org/alpine/{versionObject.ToMajorMinorUrl()}/releases/{alpineVersionArch.Architecture}/{rawFlavor}.sha512";
            var sha512ResponseMessage = await _httpClient.GetAsync(sha512Url, cancellationToken);
            var sha512 = sha512ResponseMessage.IsSuccessStatusCode
                ? await sha512ResponseMessage.Content.ReadAsStringAsync(cancellationToken)
                : string.Empty;
            return sha512.Split(" ").First().Trim();
        }, cancellationToken);

        await Task.WhenAll(sha256Task, sha512Task);

        var sha256 = sha256Task.Result;
        var sha512 = sha512Task.Result;

        if (string.IsNullOrEmpty(sha256) && string.IsNullOrEmpty(sha512))
            return new ShaRecords(string.Empty, string.Empty);

        return new ShaRecords(sha256, sha512);
    }

    private record ShaRecords(string Sha256, string Sha512);
}