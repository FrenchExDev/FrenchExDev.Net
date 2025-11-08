using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class Internal_Reflection_Tests
{
    [Fact]
    public void Can_Construct_Nested_Exception_Types_Via_Reflection()
    {
        // PersonBuilder nested exceptions
        var pbType = typeof(PersonBuilder);
        var nestedNames = new[] { "MustHaveContactException", "MustKnowAtLeastOnePersonException", "NameCannotBeNullOrEmptyOrWhitespaceException", "AgeMustBeNonNegativeException", "AtLeastOneAddressMustBeProvidedException" };
        foreach (var name in nestedNames)
        {
            var nt = pbType.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic);
            nt.ShouldNotBeNull();
            var inst = Activator.CreateInstance(nt!, nonPublic: true);
            inst.ShouldBeAssignableTo<Exception>();
        }

        // AddressBuilder internal exceptions
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
    public void AddressBuilder_Private_ValidateInternal_Adds_Failures_For_Missing_Fields()
    {
        var ab = new AddressBuilder();
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();

        var mi = typeof(AddressBuilder).GetMethod("ValidateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();

        // both null
        mi!.Invoke(ab, new object[] { visited, failures });
        failures.Count.ShouldBeGreaterThan(0);

        // street only
        failures = new FailuresDictionary();
        var ab2 = new AddressBuilder().Street("S");
        mi.Invoke(ab2, new object[] { new VisitedObjectDictionary(), failures });
        failures.Count.ShouldBeGreaterThan(0);

        // city only
        failures = new FailuresDictionary();
        var ab3 = new AddressBuilder().City("C");
        mi.Invoke(ab3, new object[] { new VisitedObjectDictionary(), failures });
        failures.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PersonBuilder_Private_ValidateInternal_Reports_All_Missing_Properties()
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
