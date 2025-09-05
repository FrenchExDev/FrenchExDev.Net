# FrenchExDev.Net.Alpine.Version.Testing

## Overview

This project provides infrastructure, utilities, and documentation for testing the `FrenchExDev.Net.Alpine.Version` library. It is designed to support and extend the main test suite found in the `FrenchExDev.Alpine.Version.Tests` project.

## Purpose

- Supplies shared test helpers, mocks, and base classes for Alpine version-related tests.
- Documents recommended testing patterns and usage for contributors.
- Ensures consistency and reliability across all test scenarios for Alpine version search and filtering.

## Remarks

- The main unit and integration tests are located in the `FrenchExDev.Alpine.Version.Tests` project.
- Use this project to add reusable components or documentation for Alpine version testing.
- All test code should target `.NET 9` and follow best practices for asynchronous and parallel test execution.

## Example: Using Tests from `.Tests` Project

Below is an example of how to write a unit test for the `AlpineVersionSearcher` using the builder and filter classes. This pattern is used in the `FrenchExDev.Net.Alpine.Version.Tests` project:

```csharp
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
````

Another example demonstrates how to test version comparison logic:

```csharp
[InlineData("3.18.0", Operator.GreaterThanOrEqual, "3.19.0", false)]
[InlineData("3.19.0", Operator.GreaterThanOrEqual, "3.20.0", false)]
[InlineData("3.20.0", Operator.GreaterThanOrEqual, "3.21.0", false)]
[InlineData("3.21.0", Operator.GreaterThanOrEqual, "3.22.0", false)]
[InlineData("3.22.0", Operator.GreaterThanOrEqual, "3.23.0", false)]
public void Can_Compare(string left, Operator @operator, string right, bool expected)
{
    AlpineVersionSearcherTester.Compare(left.AsAlpineVersion(), @operator, right.AsAlpineVersion()).ShouldBeEquivalentTo(expected);
}
```