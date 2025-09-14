using FrenchexDev.VirtualBox.Net;
using FrenchExDev.Net.VirtualBox.Version.Testing;
using Shouldly;

namespace FrenchExDev.Net.VirtualBox.Version.Tests;

/// <summary>
/// Contains unit tests for verifying the retrieval and validation of VirtualBox version information, including checks
/// for specific releases and system-installed versions.
/// </summary>
/// <remarks>These tests require either an active internet connection or a local VirtualBox installation,
/// depending on the scenario. They ensure that version discovery and checksum validation behave as expected under
/// typical usage conditions.</remarks>
[Trait("test", "unit")]
[Trait("unit", "virtualbox.version.information.searcher")]
public class VirtualBoxVersionInformationSearcherTests
{
    /// <summary>
    /// Verifies that version information for a specified VirtualBox release can be retrieved and matches the expected
    /// checksum.
    /// </summary>
    /// <remarks>This test requires an active internet connection to retrieve version information from remote
    /// sources.</remarks>
    /// <param name="version">The VirtualBox version string to search for. Must be a valid version identifier.</param>
    /// <param name="checksum">The expected SHA-256 checksum of the Additions ISO for the specified version.</param>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Theory(Skip = "virtualbox.org does not provide checksums anymore")]
    [Trait("needs", "internet")]
    [InlineData("7.1.4", "80c91d35742f68217cf47b13e5b50d53f54c22c485bacce41ad7fdc321649e61")]
    public async Task Can_Get_VersionInformation(string version, string checksum)
    {
        await VirtualBoxVersionInformationSearcherTester.ValidAsync(
            () => new VirtualBoxVersionInformationSearchingFilters(version),
            results =>
            {
                results.Count.ShouldBeEquivalentTo(1);
                results.First().AdditionsIsoSha256.ShouldBeEquivalentTo(checksum);
                results.First().Version.ShouldBeEquivalentTo(version);
            });
    }

    /// <summary>
    /// Verifies that the system's VirtualBox version can be discovered and that the result is not null or empty.
    /// </summary>
    /// <remarks>This test requires VirtualBox to be installed on the system. It uses the
    /// VirtualBoxSystemVersionDiscoverer to retrieve the version information asynchronously and asserts that the
    /// discovered version string is valid.</remarks>
    /// <returns></returns>
    [Fact]
    [Trait("needs", "virtualbox")]
    public async Task Can_Get_System_VirtualBoxVersion_Test()
    {
        var searcher = new VirtualBoxSystemVersionDiscoverer();

        var results = await searcher.DiscoverAsync(CancellationToken.None);

        results.ToStringWithoutRelease().ShouldNotBeNullOrEmpty();
    }
}