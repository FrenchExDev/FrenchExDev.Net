using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var janeSmith = new PersonBuilder()
                .Name("Jane Smith")
                .Age(28)
                .Address(new AddressBuilder()
                    .Street("123 Main St")
                    .City("Anytown"));

        var johnDoe = new PersonBuilder()
            .Name("John Doe")
            .Age(30)
            .Contact(janeSmith)
            .Address(new AddressBuilder()
                .Street("456 Elm St")
                .City("Othertown"))
            .KnownPerson(new PersonBuilder()
                .Name("Alice Johnson")
                .Age(35)
                .Contact(janeSmith)
                .Address(new AddressBuilder()
                    .Street("789 Oak St")
                    .City("Sometown")))
            .KnownPerson(janeSmith);

        janeSmith.Contact(johnDoe);

        var result = johnDoe.Build();

        result.ShouldBeAssignableTo<SuccessBuildResult<Person>>();

        var person = result.Success<Person>();

        person.ShouldBeAssignableTo<Person>();

        person.Contact.ShouldBeAssignableTo<Person>();
    }
}
