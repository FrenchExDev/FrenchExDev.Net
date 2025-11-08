using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Builders;

public class PersonBuilder_Tests
{
    [Fact]
    public void Given_PersonBuilderMissingProperties_When_ValidateInternalInvoked_Then_ReportsMissing()
    {
        var pb = new PersonBuilder();
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        var mi = typeof(PersonBuilder).GetMethod("ValidateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();
        mi!.Invoke(pb, new object[] { visited, failures });
        failures.Count.ShouldBeGreaterThanOrEqualTo(4);
        failures.Keys.ShouldContain("_name");
        failures.Keys.ShouldContain("_addresses");
        failures.Keys.ShouldContain("_age");
        failures.Keys.ShouldContain("_contact");
        failures.Keys.ShouldContain("_knownPersons");
    }

    [Fact]
    public void Given_PersonBuilderNestedExceptions_When_Reflecting_Then_CanConstruct()
    {
        var pbType = typeof(PersonBuilder);
        var nestedNames = new[] { "MustHaveContactException", "MustKnowAtLeastOnePersonException", "NameCannotBeNullOrEmptyOrWhitespaceException", "AgeMustBeNonNegativeException", "AtLeastOneAddressMustBeProvidedException" };
        foreach (var name in nestedNames)
        {
            var nt = pbType.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic);
            nt.ShouldNotBeNull();
            var inst = Activator.CreateInstance(nt!, nonPublic: true);
            inst.ShouldBeAssignableTo<Exception>();
        }
    }

    [Fact]
    public void Given_PersonBuilderBuild_When_MissingRequiredFields_Then_BuildReturnsFailures()
    {
        var builder = new PersonBuilder();
        var result = builder.Build();
        result.IsFailure().ShouldBeTrue();
        var failures = result.Failures();
        failures.Keys.ShouldContain("_name");
        failures.Keys.ShouldContain("_addresses");
        failures.Keys.ShouldContain("_age");
        failures.Keys.ShouldContain("_contact");
        failures.Keys.ShouldContain("_knownPersons");
    }

    [Fact]
    public void Given_PersonBuilder_WithInvalidContactAndKnownPerson_When_Build_Then_NestedFailuresReported()
    {
        var builder = new PersonBuilder()
            .Name("Main")
            .Age(40)
            .Address(new AddressBuilder().Street("S").City("C"))
            // contact is invalid (no fields set)
            .Contact(new PersonBuilder())
            // add a known person that is invalid
            .KnownPerson(new PersonBuilder());

        var result = builder.Build();
        result.IsFailure().ShouldBeTrue();
        var failures = result.Failures();
        // contact should have nested failures
        failures.Keys.ShouldContain("_contact");
        // the known person validation added failures under the loop name 'person'
        failures.Keys.ShouldContain("person");
    }
}
