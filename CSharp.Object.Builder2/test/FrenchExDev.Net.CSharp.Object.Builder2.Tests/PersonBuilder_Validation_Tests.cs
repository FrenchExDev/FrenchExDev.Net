using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class PersonBuilder_Validation_Tests
{
    [Fact]
    public void PersonBuilder_Build_Returns_Failures_When_Missing_Required_Fields()
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
    public void PersonBuilder_Contact_And_KnownPerson_Invalid_Are_Reported_In_Failures()
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
