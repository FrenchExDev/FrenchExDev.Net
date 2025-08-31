using FernchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FernchExDev.Net.CSharp.Object.Builder.Tests;

[Trait("unit", "virtual")]
public class LambdaObjectBuilderTests
{
    internal class PersonBuilder : LambdaObjectBuilder<Person, PersonBuilder>
    {
        public const string ErrorInvalidAge = "Invalid age";

        public PersonBuilder(Func<PersonBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<Person>> buildFunc) : base(buildFunc)
        {
        }
    }

    internal class AddressBuilder : LambdaObjectBuilder<Address, AddressBuilder>
    {
        public AddressBuilder(Func<AddressBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<Address>> buildFunc) : base(buildFunc)
        {
        }
    }

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> can successfully build a complete <see cref="Person"/> object with
    /// multiple addresses and the specified properties.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PersonBuilder"/> correctly constructs a <see
    /// cref="Person"/> instance with the provided name, age, and multiple addresses. It ensures that the resulting
    /// object is of type <see cref="SuccessObjectBuildResult{T}"/> and that all properties are set as expected.</remarks>
    /// <returns></returns>
    [Fact]
    public void Can_Build_Complete_Person_Async()
    {
        var p1b = new PersonBuilder((builder, exceptions, visited) =>
        {
            return SuccessObjectBuildResult<Person>.Success(new Person("foo", 30, [new Address("123 Main St", "12345")]));
        });

        var p1 = p1b.Build();

        p1.ShouldBeAssignableTo<SuccessObjectBuildResult<Person>>();

        var person = ((SuccessObjectBuildResult<Person>)p1).Result;

        person.Name.ShouldBe("foo");
        person.Age.ShouldBe(30);
        person.Addresses.Count().ShouldBe(1);
        person.Addresses.ElementAt(0).Street.ShouldBe("123 Main St");
        person.Addresses.ElementAt(0).ZipCode.ShouldBe("12345");
    }

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> correctly handles the case where an invalid age is provided,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void Cannot_Build_Incomplete_Person_Async()
    {
        var p1b = new PersonBuilder((builder, exceptions, visited) =>
        {
            exceptions.Add(new Exception(PersonBuilder.ErrorInvalidAge));

            return new FailureObjectBuildResult<Person, PersonBuilder>(builder, exceptions, visited);
        });

        var p1 = p1b.Build();

        p1.ShouldBeAssignableTo<FailureObjectBuildResult<Person, PersonBuilder>>();

        var failure = (FailureObjectBuildResult<Person, PersonBuilder>)p1;

        failure.ShouldNotBeNull();
        failure.Exceptions.Count().ShouldBeEquivalentTo(1);
        failure.Exceptions.ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
        failure.Builder.ShouldNotBeNull();
        failure.Builder.ShouldBeAssignableTo<PersonBuilder>();
    }
}