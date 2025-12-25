using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ValidationAssertionsTests
{
    [Fact]
    public void AssertNotEmptyOrWhitespace_WithNull_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace((string?)null, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_WithValidString_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace("valid", "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_WithEmptyString_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace("", "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_WithWhitespace_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace("   ", "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_List_WithNull_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace((List<string>?)null, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_List_WithValidStrings_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace(new List<string> { "a", "b" }, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_List_WithEmptyString_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace(new List<string> { "valid", "" }, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotNullOrEmptyOrWhitespace_WithValidString_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullOrEmptyOrWhitespace("valid", "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotNullOrEmptyOrWhitespace_WithNull_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullOrEmptyOrWhitespace(null, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotNull_WithValue_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNull(new object(), "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotNull_WithNull_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNull(null, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_WithValidList_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection(new List<int> { 1, 2 }, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_WithNull_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection<int>(null, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_WithEmptyList_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection(new List<int>(), "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_StringList_WithEmptyString_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection(new List<string> { "valid", "" }, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void Assert_WhenPredicateReturnsFalse_ShouldPass()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.Assert(() => false, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeFalse();
    }

    [Fact]
    public void Assert_WhenPredicateReturnsTrue_ShouldFail()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.Assert(() => true, "field", failures, n => new Exception(n));
        failures.HasFailures.ShouldBeTrue();
    }
}

