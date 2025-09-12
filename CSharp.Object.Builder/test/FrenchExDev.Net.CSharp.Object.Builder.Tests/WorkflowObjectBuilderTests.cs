using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;
using FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Contains unit tests for the <see cref="PersonBuilder"/> class, which is used to construct <see cref="Person"/>
/// objects with specific configurations.
/// </summary>
/// <remarks>This class includes tests to verify the functionality of the <see cref="PersonBuilder"/> in both
/// valid and invalid scenarios. The tests ensure that the builder correctly constructs <see cref="Person"/> objects
/// when provided with valid data and appropriately handles errors when invalid data is supplied.</remarks>
[Trait("unit", "virtual")]
public class WorkflowObjectBuilderTests
{
    #region Builder test classes

    /// <summary>
    /// Provides a builder for creating and configuring instances of the <see cref="Person"/> class.
    /// </summary>
    /// <remarks>This class is a specialized implementation of <see cref="WorkflowObjectBuilder{TObject,
    /// TBuilder}"/> for the <see cref="Person"/> type. It enables a fluent API for constructing <see cref="Person"/>
    /// objects with specific configurations.</remarks>
    internal sealed class PersonBuilder : WorkflowObjectBuilder<Person, PersonBuilder>
    {
    }

    #endregion

    /// <summary>
    /// Tests whether a complete <see cref="Person"/> object can be successfully built using the <see
    /// cref="PersonBuilder"/>.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PersonBuilder"/> correctly constructs a <see
    /// cref="Person"/> object with the expected properties, including name, age, and addresses. The test ensures that
    /// all intermediate steps in the building process are executed as expected and that the resulting object meets the
    /// specified criteria.</remarks>
    /// <returns></returns>
    [Fact]
    public async Task Can_Build_Complete_Person() => await BuilderTester.ValidAsync<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: async (builder, cancellationToken) =>
            {
                builder
                    .Step(new LambdaStepObjectBuilder<Person>((step, exceptions, intermediates, cancellationToken) =>
                    {
                        intermediates["Name"] = "John Doe";
                        intermediates["Age"] = 30;
                        intermediates["Addresses"] = new List<Address> { new Address("123 Main St", "12345") };
                    }))
                    .Step(new LambdaStepObjectBuilder<Person>((step, exceptions, intermediates, cancellationToken) =>
                    {
                        var person = new Person(
                            name: intermediates.Get<string>("Name"),
                            age: intermediates.Get<int>("Age"),
                            addresses: intermediates.Get<IEnumerable<Address>>("Addresses"),
                            knows: Array.Empty<Person>()
                        );
                        step.Set(person);
                    }));
                await Task.CompletedTask;
            },
            asserts: (person) =>
            {
                person.ShouldNotBeNull();
                person.Name.ShouldBe("John Doe");
                person.Age.ShouldBe(30);
                person.Addresses.ShouldNotBeNull();
                person.Addresses.ShouldHaveSingleItem();
                person.Addresses.First().Street.ShouldBe("123 Main St");
                person.Addresses.First().ZipCode.ShouldBe("12345");
            });

    /// <summary>
    /// Tests that the <see cref="PersonBuilder"/> cannot build a complete <see cref="Person"/> object when
    /// invalid data is provided, such as an invalid age.
    /// </summary>
    /// <remarks>This test verifies that the builder correctly identifies and handles invalid input during the
    /// object-building process. Specifically, it ensures that an exception is raised when the age is less than or equal
    /// to zero, and that the failure contains the expected exception message and builder state.</remarks>
    /// <returns></returns>
    [Fact]
    public async Task Cannot_Build_Complete_Person() => await BuilderTester.InvalidAsync<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: async (builder, cancellationToken) =>
            {
                builder
                    .Step(new LambdaStepObjectBuilder<Person>((step, exceptions, intermediates, cancellationToken) =>
                    {
                        intermediates["Name"] = "John Doe";
                        intermediates["Age"] = 0; // Invalid age
                        intermediates["Addresses"] = new List<Address> { new Address("123 Main St", "12345") };
                    }))
                    .Step(new LambdaStepObjectBuilder<Person>((step, exceptions, intermediates, cancellationToken) =>
                    {
                        var age = intermediates.Get<int>("Age");
                        if (age <= 0)
                        {
                            exceptions.Add("age", new Exception("Invalid age provided."));
                            return;
                        }
                        var person = new Person(
                            name: intermediates.Get<string>("Name"),
                            age: age,
                            addresses: intermediates.Get<IEnumerable<Address>>("Addresses"),
                            knows: Array.Empty<Person>()
                        );
                        step.Set(person);
                    }));
                await Task.CompletedTask;
            },
            assert: (failure) =>
            {
                failure.ShouldNotBeNull();
                failure.Exceptions.Count().ShouldBe(1);
                failure.Exceptions["age"].ElementAt(0).Message.ShouldBe("Invalid age provided.");
                failure.Builder.ShouldNotBeNull();
            });
}
