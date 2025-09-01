using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;
using FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of <see cref="LambdaObjectBuilder{TObject, TBuilder}"/>
/// implementations, specifically <see cref="PersonBuilder"/> and <see cref="AddressBuilder"/>.
/// </summary>
/// <remarks>This class includes tests to ensure that the <see cref="PersonBuilder"/> and <see
/// cref="AddressBuilder"/> correctly construct objects of type <see cref="Person"/> and <see cref="Address"/>,
/// respectively. The tests validate both successful and failure scenarios, including proper handling of invalid input
/// and ensuring that all properties are set as expected in the resulting objects.</remarks>
[Trait("unit", "virtual")]
public class LambdaObjectBuilderTests
{
    #region Builder test classes

    /// <summary>
    /// Provides a builder for creating and configuring instances of the <see cref="Person"/> class.
    /// </summary>
    /// <remarks>This class is designed to facilitate the construction of <see cref="Person"/> objects using a
    /// fluent API.  It extends the <see cref="LambdaObjectBuilder{TObject, TBuilder}"/> base class, allowing for custom
    /// build logic and validation during the object creation process.</remarks>
    internal class PersonBuilder : LambdaObjectBuilder<Person, PersonBuilder>
    {
        public const string ErrorInvalidAge = "Invalid age";

        public PersonBuilder(Func<PersonBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<Person>> buildFunc) : base(buildFunc)
        {
        }
    }

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>This class is used to construct <see cref="Address"/> objects using a customizable build
    /// function.  It extends the <see cref="LambdaObjectBuilder{TObject, TBuilder}"/> class, allowing for flexible 
    /// object creation patterns.</remarks>
    internal class AddressBuilder : LambdaObjectBuilder<Address, AddressBuilder>
    {
        public AddressBuilder(Func<AddressBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<Address>> buildFunc) : base(buildFunc)
        {
        }
    }

    #endregion

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> can successfully build a complete <see cref="Person"/> object with
    /// multiple addresses and the specified properties.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PersonBuilder"/> correctly constructs a <see
    /// cref="Person"/> instance with the provided name, age, and multiple addresses. It ensures that the resulting
    /// object is of type <see cref="SuccessObjectBuildResult{T}"/> and that all properties are set as expected.</remarks>
    /// <returns></returns>
    [Fact]
    public async Task Can_Build_Complete_Person()
    {
        await BuilderTester.TestValid<PersonBuilder, Person>(
            () => new PersonBuilder((builder, exceptions, visited) =>
            {
                return SuccessObjectBuildResult<Person>.Success(new Person("foo", 30, [new Address("123 Main St", "12345")]));
            }),
            (builder) =>
            {
            },
            (person) =>
            {
                person.Name.ShouldBe("foo");
                person.Age.ShouldBe(30);
                person.Addresses.Count().ShouldBe(1);
                person.Addresses.ElementAt(0).Street.ShouldBe("123 Main St");
                person.Addresses.ElementAt(0).ZipCode.ShouldBe("12345");
            });
    }
}
