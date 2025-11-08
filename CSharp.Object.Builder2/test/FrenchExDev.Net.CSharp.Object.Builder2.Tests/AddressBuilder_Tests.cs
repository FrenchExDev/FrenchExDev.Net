using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class AddressBuilder_Tests
{
    [Fact]
    public void AddressBuilder_Validate_Fails_When_Missing_Street_Or_City()
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
    public void AddressBuilder_Build_Fails_When_Missing_Values()
    {
        var b = new AddressBuilder();
        var result = b.Build();
        result.IsFailure().ShouldBeTrue();
    }
}
