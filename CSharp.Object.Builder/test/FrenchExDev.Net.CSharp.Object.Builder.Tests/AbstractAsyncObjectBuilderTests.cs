using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;
using FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Provides unit tests for verifying the behavior of asynchronous object builders, such as <see cref="PersonBuilder"/>
/// and <see cref="AddressBuilder"/>.
/// </summary>
/// <remarks>This class contains tests to ensure that asynchronous object builders correctly construct instances
/// of their respective types, handle validation errors, and manage dependencies between objects. The tests cover both
/// successful and failure scenarios, validating that the builders produce the expected results or errors under various
/// conditions.</remarks>
[Trait("unit", "virtual")]
public class AbstractAsyncObjectBuilderTests
{
    #region Builder test classes

    /// <summary>
    /// A builder for constructing instances of <see cref="Person"/>.
    /// </summary>
    internal class PersonBuilder : AbstractAsyncObjectBuilder<Person, PersonBuilder>
    {
        public const string ErrorInvalidName = "Invalid name";
        public const string ErrorInvalidAge = "Invalid age";
        public const string BuildError = "Failed to build person";
        public const string ErrorInvalidAddress = "Invalid person";

        private string? _name;
        private int? _age;
        private List<AddressBuilder> _addresses = new();
        private List<PersonBuilder> _people = new();

        /// <summary>
        /// Sets the name of the person being built.
        /// </summary>
        /// <param name="name">The name to assign to the person. Can be null to indicate that no name is specified.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Name(string? name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the age for the person being built.
        /// </summary>
        /// <param name="age">The age of the person, or <see langword="null"/> to leave the age unspecified.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Age(int? age)
        {
            _age = age;
            return this;
        }

        /// <summary>
        /// Adds an address to the person being built using the specified configuration action.
        /// </summary>
        /// <remarks>Multiple calls to this method will add multiple addresses to the person. The provided
        /// action is invoked to configure each address before it is added.</remarks>
        /// <param name="body">An action that configures the address by operating on an <see cref="AddressBuilder"/> instance. Cannot be
        /// null.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Address(Action<AddressBuilder> body)
        {
            var builder = new AddressBuilder();
            body(builder);
            _addresses.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a new person to the builder and allows configuration of that person using the specified action.
        /// </summary>
        /// <remarks>Use this method to define relationships or properties for additional people within a
        /// fluent builder pattern. The provided action is invoked with a new <see cref="PersonBuilder"/> instance
        /// representing the person to be added.</remarks>
        /// <param name="body">An action that receives a <see cref="PersonBuilder"/> to configure the details of the person being added.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Knows(Action<PersonBuilder> body)
        {
            var builder = new PersonBuilder();
            body(builder);
            _people.Add(builder);
            return this;
        }

        /// <summary>
        /// Implements the internal build logic for constructing a <see cref="Person"/> instance.
        /// </summary>
        /// <param name="visited"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<IObjectBuildResult<Person>> BuildInternalAsync(ExceptionBuildDictionary exceptions, VisitedObjectsList visited, CancellationToken cancellationToken = default)
        {
            var addresses = await BuildListAsync<Address, AddressBuilder>(nameof(_addresses), _addresses, exceptions, visited, cancellationToken);
            var people = await BuildListAsync<Person, PersonBuilder>(nameof(_people), _people, exceptions, visited, cancellationToken);

            // Validate name
            if (string.IsNullOrWhiteSpace(_name))
            {
                exceptions.Add(nameof(_name), new BasicAsyncObjectBuildException<Person, PersonBuilder>(nameof(_name).ToMemberName(), ErrorInvalidName, this, visited));
            }

            // Validate age
            if (_age is null || _age == 0)
            {
                exceptions.Add(nameof(_age), new BasicAsyncObjectBuildException<Person, PersonBuilder>(nameof(_age).ToMemberName(), ErrorInvalidAge, this, visited));
            }

            // Return failure if any exceptions were collected
            if (exceptions.Any())
            {
                return AsyncFailureResult(exceptions, visited);
            }

            // Ensure required fields are not null
            ArgumentNullException.ThrowIfNull(_name);
            ArgumentNullException.ThrowIfNull(_age);

            return new SuccessObjectBuildResult<Person>(new Person(_name, _age.Value, addresses, people));
        }
    }

    /// <summary>
    /// Provides a builder for constructing instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>This builder allows for the step-by-step construction of an <see cref="Address"/> object by
    /// setting its properties, such as <see cref="Street"/> and <see cref="ZipCode"/>.  The builder validates the
    /// provided values during the build process and ensures that all required fields are set before creating the <see
    /// cref="Address"/> instance.</remarks>
    internal class AddressBuilder : AbstractAsyncObjectBuilder<Address, AddressBuilder>
    {
        public const string ErrorInvalidStreet = "Invalid street";
        public const string ErrorInvalidZipCode = "Invalid zip code";

        private string? _street;
        private string? _zipCode;
        public AddressBuilder Street(string street)
        {
            _street = street;
            return this;
        }
        public AddressBuilder ZipCode(string zipCode)
        {
            _zipCode = zipCode;
            return this;
        }

        /// <summary>
        /// Simulates the internal build logic for constructing an <see cref="Address"/> instance.
        /// </summary>
        /// <param name="visited"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<IObjectBuildResult<Address>> BuildInternalAsync(ExceptionBuildDictionary exceptions, VisitedObjectsList visited, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_street))
            {
                exceptions.Add(new MemberName(nameof(_street)), new InvalidDataException(ErrorInvalidStreet));
                return Task.FromResult<IObjectBuildResult<Address>>(
                    AsyncFailureResult(exceptions, visited));
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                return Task.FromResult<IObjectBuildResult<Address>>(
                    AsyncFailureResult(exceptions, visited));
            }

            return Task.FromResult<IObjectBuildResult<Address>>(new SuccessObjectBuildResult<Address>(new Address(_street, _zipCode)));
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
    public async Task Can_Build_Complete_Person_Async()
    {
        await BuilderTester.ValidAsync<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder, cancellationToken) =>
            {
                builder.Name("foo")
                       .Age(30)
                       .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
                       .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
                return Task.CompletedTask;
            }, asserts: (person) =>
            {
                person.Name.ShouldBe("foo");
                person.Age.ShouldBe(30);
                person.Addresses.Count().ShouldBe(2);
                person.Addresses.ElementAt(0).Street.ShouldBe("123 Main St");
                person.Addresses.ElementAt(0).ZipCode.ShouldBe("12345");
                person.Addresses.ElementAt(1).Street.ShouldBe("456 Elm St");
                person.Addresses.ElementAt(1).ZipCode.ShouldBe("67890");
            });
    }

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> correctly handles the case where an invalid age is provided,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Cannot_Build_Complete_Person_Async()
    {
        await BuilderTester.InvalidAsync<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder, cancellationToken) =>
            {
                builder.Name("foo")
                       .Age(0);
                return Task.CompletedTask;
            },
            assert: (failure) =>
            {
                failure.ShouldNotBeNull();
                failure.Exceptions["_age"].ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
                failure.Exceptions.Count.ShouldBeEquivalentTo(1);
                failure.Builder.ShouldNotBeNull();
                failure.Builder.ShouldBeAssignableTo<PersonBuilder>();
            });
    }
}
