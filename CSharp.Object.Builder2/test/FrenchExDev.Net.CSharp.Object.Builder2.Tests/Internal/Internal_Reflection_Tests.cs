using Shouldly;
using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Internal;

public class Internal_Reflection_Tests
{
    [Fact]
    public void Given_BuilderNestedExceptions_When_Reflecting_Then_CanConstruct()
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

        var abType = typeof(AddressBuilder);
        var streetEx = abType.GetNestedType("StreetCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        var cityEx = abType.GetNestedType("CityCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        streetEx.ShouldNotBeNull(); cityEx.ShouldNotBeNull();
        var sInst = Activator.CreateInstance(streetEx!, nonPublic: true);
        var cInst = Activator.CreateInstance(cityEx!, nonPublic: true);
        sInst.ShouldBeAssignableTo<Exception>();
        cInst.ShouldBeAssignableTo<Exception>();
    }

    [Fact]
    public void Given_AddressBuilderPrivateValidate_When_Invoke_Then_AddsFailures()
    {
        var ab = new AddressBuilder();
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();

        var mi = typeof(AddressBuilder).GetMethod("ValidateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();

        mi!.Invoke(ab, new object[] { visited, failures });
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Given_PersonBuilderPrivateValidate_When_Invoke_Then_ReportsMissingProperties()
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
}
