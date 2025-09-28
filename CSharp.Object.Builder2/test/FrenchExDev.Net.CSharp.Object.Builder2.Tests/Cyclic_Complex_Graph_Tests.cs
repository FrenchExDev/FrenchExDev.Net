using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Contains unit tests for verifying the construction of cyclic object graphs using builder patterns.
/// </summary>
/// <remarks>This class is intended for use with the xUnit testing framework. The tests demonstrate scenarios
/// where objects reference each other in a cyclic manner, ensuring that builder implementations correctly handle such
/// relationships. The class is not intended for production use and should be used only in the context of automated
/// testing.</remarks>
public class Cyclic_Complex_Graph_Tests
{
    /// <summary>
    /// Verifies that a Reference<string> instance can resolve and return a specified value.
    /// </summary>
    /// <remarks>This test ensures that the Resolve method correctly sets the value and that the Resolved
    /// method retrieves the expected result. Use this test to confirm the basic value resolution functionality of the
    /// Reference<T> class.</remarks>
    [Fact]
    public void Reference_Can_Resolve_Value()
    {
        var sut = new Reference<string>();
        sut.Resolve("Hello");
        sut.Resolved().ShouldBe("Hello");
    }

    /// <summary>
    /// Verifies that the object builder can construct a cyclic object graph, where objects reference each other in a
    /// loop, without errors or loss of data integrity.
    /// </summary>
    /// <remarks>This test ensures that the builder supports scenarios in which objects have mutual or
    /// circular references, such as a person whose contact is another person who, in turn, references the original
    /// person. Cyclic graphs are common in complex domain models and must be handled correctly to avoid issues such as
    /// stack overflows or incomplete object construction.</remarks>
    [Fact]
    public void Can_Build_Cyclic_Object_Graph()
    {
        var janeSmithBuilder = new PersonBuilder()
                .Name("Jane Smith")
                .Age(28)
                .Address(new AddressBuilder()
                    .Street("123 Main St")
                    .City("Anytown"));

        var johnDoeBuilder = new PersonBuilder()
            .Name("John Doe")
            .Age(30)
            .Contact(janeSmithBuilder)
            .Address(new AddressBuilder()
                .Street("456 Elm St")
                .City("Othertown"))
            .KnownPerson(new PersonBuilder()
                .Name("Alice Johnson")
                .Age(35)
                .KnownPerson(janeSmithBuilder)
                .Contact(janeSmithBuilder)
                .Address(new AddressBuilder()
                    .Street("789 Oak St")
                    .City("Sometown")))
            .KnownPerson(janeSmithBuilder);

        janeSmithBuilder.Contact(johnDoeBuilder);
        janeSmithBuilder.KnownPerson(johnDoeBuilder);

        var result = johnDoeBuilder.Build();

        result.ShouldBeAssignableTo<SuccessResult<Person>>();
        var johnDoe = result.Success<Person>();
        johnDoe.ShouldBeAssignableTo<Person>();
        johnDoe.Contact.ShouldBeAssignableTo<Person>();

        // Assert main properties
        johnDoe.Name.ShouldBe("John Doe");
        johnDoe.Age.ShouldBe(30);

        // Assert addresses
        johnDoe.Addresses.Count().ShouldBe(1);
        var address = johnDoe.Addresses.First();
        address.ShouldBeAssignableTo<Address>();
        address.Street.ShouldBe("456 Elm St");
        address.City.ShouldBe("Othertown");

        // Assert known persons
        johnDoe.KnownPersons.Count().ShouldBe(2);
        var aliceJohndson = johnDoe.KnownPersons.First();
        var janeSmith = johnDoe.KnownPersons.Last();
        aliceJohndson.Name.ShouldBe("Alice Johnson");
        aliceJohndson.Age.ShouldBe(35);
        janeSmith.Name.ShouldBe("Jane Smith");
        janeSmith.Age.ShouldBe(28);

        // Assert contact
        var contact = johnDoe.Contact;
        contact.Name.ShouldBe("Jane Smith");
        contact.Age.ShouldBe(28);
        contact.Addresses.Count().ShouldBe(1);
        contact.Addresses.First().Street.ShouldBe("123 Main St");
        contact.Addresses.First().City.ShouldBe("Anytown");

        // Assert cyclic reference: Jane's contact is John
        contact.Contact.ShouldBe(johnDoe);
    }
}
