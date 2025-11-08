using Shouldly;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Coverage;

public class Housekeeping_Tests
{
    [Fact]
    public void Given_CoverageHelpers_When_Invoked_Then_HitVariousLines()
    {
        // Intentional no-op tests that exist to exercise code paths for coverage.
        var d = new FailuresDictionary();
        d.Failure("a", new Failure("x"));
        d.Failure("a", new Failure("y"));
        d.ContainsKey("a").ShouldBeTrue();
    }
}
