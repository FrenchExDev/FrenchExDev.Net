using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Builders;

public class AddressBuilder_Tests
{
    [Fact]
    public void Given_AddressBuilderMissingFields_When_ValidateInternalInvoked_Then_AddsFailures()
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
    public void Given_AddressInstantiate_When_MissingValues_Then_Throws_SpecificException()
    {
        var ab = new AddressBuilder();
        var mi = typeof(AddressBuilder).GetMethod("Instantiate", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();

        var streetType = typeof(AddressBuilder).GetNestedType("StreetCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        var cityType = typeof(AddressBuilder).GetNestedType("CityCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        streetType.ShouldNotBeNull(); cityType.ShouldNotBeNull();

        var ex1 = Should.Throw<Exception>(() => { try { mi!.Invoke(ab, null); } catch (TargetInvocationException tie) { throw tie.InnerException!; } });
        ex1.GetType().ShouldBe(streetType);

        var ab2 = new AddressBuilder().Street("S");
        var ex2 = Should.Throw<Exception>(() => { try { mi!.Invoke(ab2, null); } catch (TargetInvocationException tie) { throw tie.InnerException!; } });
        ex2.GetType().ShouldBe(cityType);
    }

    [Fact]
    public void Given_AddressBuilder_When_MissingFields_Then_ValidateReportsFailures()
    {
        var b1 = new AddressBuilder().Street(null).City("City");
        var failures1 = new FailuresDictionary();
        b1.Validate(new VisitedObjectDictionary(), failures1);
        failures1.Count.ShouldBe(1);

        var b2 = new AddressBuilder().Street("Street").City(null);
        var failures2 = new FailuresDictionary();
        b2.Validate(new VisitedObjectDictionary(), failures2);
        failures2.Count.ShouldBe(1);
    }

    [Fact]
    public void Given_AddressBuilder_When_MissingValues_Then_BuildFails()
    {
        var b = new AddressBuilder();
        var result = b.Build();
        result.IsFailure().ShouldBeTrue();
    }
}
