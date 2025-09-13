using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.Alpine.Version.Testing;
using Shouldly;

namespace FrenchExDev.Alpine.Version.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of the AlpineVersionSearcher when searching for stable Alpine Linux
/// versions.
/// </summary>
/// <remarks>These tests require internet access to perform live queries against Alpine Linux version
/// repositories. The tests focus on ensuring that the searcher correctly identifies stable versions and matches
/// expected architecture and flavor criteria.</remarks>
[Trait("unit", "virtual")]
[Trait("needs-internet", "true")]
public class AlpineVersionSearcherTests
{
    /// <summary>
    /// Verifies that the AlpineVersionSearcher can successfully find stable Alpine Linux versions matching the
    /// specified version, flavor, and architecture.
    /// </summary>
    /// <remarks>This test ensures that the searcher returns at least one result matching the specified
    /// criteria and that the result's properties correspond to the provided version, flavor, and
    /// architecture.</remarks>
    /// <param name="searchingVersion">The exact version string of Alpine Linux to search for. Must correspond to a stable release version.</param>
    /// <param name="flavor">The flavor of Alpine Linux to filter the search results by.</param>
    /// <param name="architecture">The architecture to filter the search results by.</param>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Theory]
    [InlineData("3.18.0", AlpineFlavors.Virt, AlpineArchitectures.x86_64)]
    [InlineData("3.19.0", AlpineFlavors.Virt, AlpineArchitectures.x86_64)]
    [InlineData("3.20.0", AlpineFlavors.Virt, AlpineArchitectures.x86_64)]
    [InlineData("3.21.0", AlpineFlavors.Virt, AlpineArchitectures.x86_64)]
    [InlineData("3.22.0", AlpineFlavors.Virt, AlpineArchitectures.x86_64)]
    public async Task Can_Search_Stable(string searchingVersion, AlpineFlavors flavor, AlpineArchitectures architecture)
    {
        await AlpineVersionSearcherTester.ValidAsync(
            searcher: new AlpineVersionSearcher(new HttpClient()),
            filterBuilder: (filtersBuilder) =>
                filtersBuilder
                    .WithExactVersion(searchingVersion)
                    .WithFlavors([flavor])
                    .WithArchs([architecture]),
            assert: (result) =>
            {
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
                result.ElementAt(0).Version.ToString().ShouldBe(searchingVersion);
                result.ElementAt(0).Flavor.ShouldBeEquivalentTo(AlpineFlavors.Virt.ToString().ToLowerInvariant());
                result.ElementAt(0).Architecture.ShouldBeEquivalentTo(AlpineArchitectures.x86_64.ToString().ToLowerInvariant());
            });
    }
}
