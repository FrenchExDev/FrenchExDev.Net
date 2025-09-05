using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.Alpine.Version.Testing;
using Shouldly;
using static FrenchExDev.Net.Alpine.Version.Testing.AlpineVersionSearcherTester;

namespace FrenchExDev.Alpine.Version.Tests;

/// <summary>
/// Unit tests for comparing Alpine Linux version strings using various relational operators.
/// Verifies that the comparison logic for major, minor, and patch versions behaves as expected.
/// </summary>
[Trait("unit", "virtual")]
public class AlpineVersionComparerTests
{
    /// <summary>
    /// Tests the comparison of two Alpine version strings using the specified operator.
    /// </summary>
    /// <param name="left">The left-hand Alpine version string to compare.</param>
    /// <param name="operator">The comparison operator to use (e.g., Equal, NotEqual, LessThan).</param>
    /// <param name="right">The right-hand Alpine version string to compare.</param>
    /// <param name="expected">The expected result of the comparison (true if the comparison should succeed, false otherwise).</param>
    [Theory]
    [InlineData("3.18.0", Operator.Equal, "3.18.0", true)]
    [InlineData("3.19.0", Operator.Equal, "3.19.0", true)]
    [InlineData("3.20.0", Operator.Equal, "3.20.0", true)]
    [InlineData("3.21.0", Operator.Equal, "3.21.0", true)]
    [InlineData("3.22.0", Operator.Equal, "3.22.0", true)]

    [InlineData("3.18.0", Operator.Equal, "3.19.0", false)]
    [InlineData("3.19.0", Operator.Equal, "3.20.0", false)]
    [InlineData("3.20.0", Operator.Equal, "3.21.0", false)]
    [InlineData("3.21.0", Operator.Equal, "3.22.0", false)]
    [InlineData("3.22.0", Operator.Equal, "3.23.0", false)]

    [InlineData("3.18.0", Operator.NotEqual, "3.19.0", true)]
    [InlineData("3.19.0", Operator.NotEqual, "3.20.0", true)]
    [InlineData("3.20.0", Operator.NotEqual, "3.21.0", true)]
    [InlineData("3.21.0", Operator.NotEqual, "3.22.0", true)]
    [InlineData("3.22.0", Operator.NotEqual, "3.23.0", true)]

    [InlineData("3.18.0", Operator.LessThan, "3.19.0", true)]
    [InlineData("3.19.0", Operator.LessThan, "3.20.0", true)]
    [InlineData("3.20.0", Operator.LessThan, "3.21.0", true)]
    [InlineData("3.21.0", Operator.LessThan, "3.22.0", true)]
    [InlineData("3.22.0", Operator.LessThan, "3.23.0", true)]

    [InlineData("3.18.0", Operator.LessThanOrEqual, "3.19.0", true)]
    [InlineData("3.19.0", Operator.LessThanOrEqual, "3.20.0", true)]
    [InlineData("3.20.0", Operator.LessThanOrEqual, "3.21.0", true)]
    [InlineData("3.21.0", Operator.LessThanOrEqual, "3.22.0", true)]
    [InlineData("3.22.0", Operator.LessThanOrEqual, "3.23.0", true)]

    [InlineData("3.18.0", Operator.GreaterThan, "3.19.0", false)]
    [InlineData("3.19.0", Operator.GreaterThan, "3.20.0", false)]
    [InlineData("3.20.0", Operator.GreaterThan, "3.21.0", false)]
    [InlineData("3.21.0", Operator.GreaterThan, "3.22.0", false)]
    [InlineData("3.22.0", Operator.GreaterThan, "3.23.0", false)]

    [InlineData("3.18.0", Operator.GreaterThanOrEqual, "3.19.0", false)]
    [InlineData("3.19.0", Operator.GreaterThanOrEqual, "3.20.0", false)]
    [InlineData("3.20.0", Operator.GreaterThanOrEqual, "3.21.0", false)]
    [InlineData("3.21.0", Operator.GreaterThanOrEqual, "3.22.0", false)]
    [InlineData("3.22.0", Operator.GreaterThanOrEqual, "3.23.0", false)]
    public void Can_Compare(string left, Operator @operator, string right, bool expected)
    {
        AlpineVersionSearcherTester.Compare(left.AsAlpineVersion(), @operator, right.AsAlpineVersion()).ShouldBeEquivalentTo(expected);
    }
}