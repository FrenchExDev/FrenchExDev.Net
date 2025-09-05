using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.Alpine.Version.Testing;
using Shouldly;

namespace FrenchExDev.Alpine.Version.Tests;

[Trait("unit", "virtual")]
[Trait("needs-internet", "true")]
public class AlpineVersionSearcherTests
{
    [Theory]
    [InlineData("3.18.0")]
    [InlineData("3.19.0")]
    [InlineData("3.20.0")]
    [InlineData("3.21.0")]
    [InlineData("3.22.0")]
    public async Task Can_Search_Stable(string searchingVersion)
    {
        await AlpineVersionSearcherTester.TestValidAsync(
            searcher: new AlpineVersionSearcher(new HttpClient()),
            filterBuilder: (filtersBuilder) =>
                filtersBuilder
                    .WithExactVersion(searchingVersion)
                    .WithFlavors([AlpineFlavors.Virt])
                    .WithArchs([AlpineArchitectures.x86_64]),
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
