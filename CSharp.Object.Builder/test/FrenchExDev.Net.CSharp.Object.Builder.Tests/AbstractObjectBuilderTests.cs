using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;
using FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Provides unit tests for verifying the behavior of object builders derived from  <see
/// cref="AbstractObjectBuilder{TObject, TBuilder}"/>.
/// </summary>
/// <remarks>This class contains nested test-specific builder implementations, such as <c>PersonBuilder</c>  and
/// <c>AddressBuilder</c>, which are used to validate the functionality of the abstract object  builder pattern. These
/// builders demonstrate how to implement and use the <c>BuildInternal</c>  method to construct objects while handling
/// validation and error reporting.</remarks>
public class AbstractObjectBuilderTests
{
    #region Builder test classes

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Person"/> class.
    /// </summary>
    /// <remarks>
    /// This builder allows for the step-by-step construction of a <see cref="Person"/> object, 
    /// including setting the name, age, and associated addresses. It validates the input data during the build process
    /// and returns either a successfully constructed <see cref="Person"/> or a failure result with detailed
    /// exceptions.
    /// </remarks>
    internal class PersonBuilder : AbstractObjectBuilder<Person, PersonBuilder>
    {
        /// <summary>
        /// Error message for invalid name.
        /// </summary>
        public const string ErrorInvalidName = "Invalid name";

        /// <summary>
        /// Error message for invalid age.
        /// </summary>
        public const string ErrorInvalidAge = "Invalid age";

        /// <summary>
        /// Error message for a general build failure.
        /// </summary>
        public const string BuildError = "Failed to build person";

        /// <summary>
        /// Stores the name of the person being built.
        /// </summary>
        private string? _name;

        /// <summary>
        /// Stores the age of the person being built.
        /// </summary>
        private int? _age;

        /// <summary>
        /// Stores the list of address builders for the person.
        /// </summary>
        private List<AddressBuilder> _addresses = new();

        /// <summary>
        /// Stores the list of people known by this person.
        /// </summary>
        private List<PersonBuilder> _people = new();

        /// <summary>
        /// Sets the name of the person.
        /// </summary>
        public PersonBuilder Name(string? name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the age of the person.
        /// </summary>
        public PersonBuilder Age(int? age)
        {
            _age = age;
            return this;
        }

        /// <summary>
        /// Adds an address to the person using an address builder action.
        /// </summary>
        public PersonBuilder Address(Action<AddressBuilder> addressBuilderAction)
        {
            var addressBuilder = new AddressBuilder();
            addressBuilderAction(addressBuilder);
            _addresses.Add(addressBuilder);
            return this;
        }

        /// <summary>
        /// Adds a new person to the builder and allows configuration of that person using the specified action.
        /// </summary>
        /// <remarks>Use this method to define relationships or properties for additional people within a
        /// fluent builder pattern. The method enables chaining multiple calls to build complex person graphs.</remarks>
        /// <param name="body">An action that receives a <see cref="PersonBuilder"/> to configure the new person. Cannot be null.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Knows(Action<PersonBuilder> body)
        {
            var builder = new PersonBuilder();
            body(builder);
            _people.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds a relationship indicating that the current person knows the specified person.
        /// </summary>
        /// <param name="builder">The builder representing the person to be added as someone known by the current person. Cannot be null.</param>
        /// <returns>The current <see cref="PersonBuilder"/> instance to allow method chaining.</returns>
        public PersonBuilder Knows(PersonBuilder builder)
        {
            _people.Add(builder);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="Person"/> object using the configured properties and child builders.
        /// </summary>
        /// <remarks>
        /// The method validates the configured properties, such as <c>_name</c> and <c>_age</c>,
        /// and ensures that all child builders for <see cref="Address"/> objects produce successful results. If any
        /// validation fails or a child builder returns a failure, the method returns a failure result with the relevant
        /// exceptions.
        /// </remarks>
        /// <param name="exceptions">A collection to which any exceptions encountered during the build process are added.</param>
        /// <param name="visited">A list of objects that have already been visited during the build process to prevent circular references.</param>
        /// <returns>A <see cref="SuccessObjectBuildResult{T}"/> containing the constructed <see cref="Person"/> object if the
        /// build is successful; otherwise, a <see cref="FailureObjectBuildResult{T, TBuilder}"/> containing the
        /// encountered exceptions.</returns>
        protected override IObjectBuildResult<Person> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
        {
            // next people variable will be used to collect built people
            // built people can be built successfully, fail to build, or be a build reference
            // we need to handle each case appropriately
            // if any people fail to build, we collect the exceptions
            // if any people are build references, we register an action to add them once built
            var people = BuildList<Person, PersonBuilder>(nameof(_people), _people, exceptions, visited);

            // Validate name
            if (string.IsNullOrWhiteSpace(_name))
            {
                exceptions.Add(nameof(_name), new BasicObjectBuildException<Person, PersonBuilder>(new MemberName(nameof(_name)), ErrorInvalidName, this, visited));
            }

            // Validate age
            if (_age is null || _age == 0)
            {
                exceptions.Add(nameof(_age), new BasicObjectBuildException<Person, PersonBuilder>(new MemberName(nameof(_age)), ErrorInvalidAge, this, visited));
            }

            // Return failure if any exceptions were collected
            if (exceptions.Any())
            {
                return Failure(exceptions, visited);
            }

            // Ensure required fields are not null
            ArgumentNullException.ThrowIfNull(_name);
            ArgumentNullException.ThrowIfNull(_age);

            // Return a successful build result with the constructed Person
            return Success(new Person(_name, _age.Value, [], people));
        }
    }

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>
    /// This builder allows for the step-by-step construction of an <see cref="Address"/> object by
    /// setting its properties, such as <see cref="Street"/> and <see cref="ZipCode"/>.  The builder validates the
    /// required fields during the build process and returns either a  successful result or a failure result with
    /// appropriate error messages.
    /// </remarks>
    internal class AddressBuilder : AbstractObjectBuilder<Address, AddressBuilder>
    {
        /// <summary>
        /// Error message for invalid street.
        /// </summary>
        public const string ErrorInvalidStreet = "Invalid street";
        /// <summary>
        /// Error message for invalid zip code.
        /// </summary>
        public const string ErrorInvalidZipCode = "Invalid zip code";
        /// <summary>
        /// Stores the street value for the address being built.
        /// </summary>
        private string? _street;
        /// <summary>
        /// Stores the zip code value for the address being built.
        /// </summary>
        private string? _zipCode;

        /// <summary>
        /// Sets the street value for the address.
        /// </summary>
        public AddressBuilder Street(string street)
        {
            _street = street;
            return this;
        }
        /// <summary>
        /// Sets the zip code value for the address.
        /// </summary>
        public AddressBuilder ZipCode(string zipCode)
        {
            _zipCode = zipCode;
            return this;
        }

        /// <summary>
        /// Builds an <see cref="Address"/> object based on the current state of the builder.
        /// </summary>
        /// <param name="exceptions">A list to which any exceptions encountered during the build process are added.</param>
        /// <param name="visited">A list of objects visited during the build process to prevent circular references.</param>
        /// <returns>A successful build result containing the constructed <see cref="Address"/> object if the required fields are
        /// valid; otherwise, a failure result with the appropriate error message.</returns>
        protected override IObjectBuildResult<Address> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
        {
            // Validate street and zip code
            if (string.IsNullOrEmpty(_street))
            {
                exceptions.Add(nameof(_street), new BasicObjectBuildException<Address, AddressBuilder>(nameof(_street).ToMemberName(), ErrorInvalidStreet, this, visited));
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                exceptions.Add(nameof(_zipCode), new BasicObjectBuildException<Address, AddressBuilder>(nameof(_zipCode).ToMemberName(), ErrorInvalidZipCode, this, visited));
            }

            // Return failure if any exceptions were collected
            if (exceptions.Any())
            {
                return Failure(exceptions, visited);
            }

            // Ensure required fields are not null
            ArgumentNullException.ThrowIfNull(_street);
            ArgumentNullException.ThrowIfNull(_zipCode);

            // Return a successful build result with the constructed Address
            return Success(new Address(_street, _zipCode));
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
    public async Task Can_Build_Complete_Person_With_Cyclic_Reference() => await BuilderTester.TestValid<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder) =>
            {
                builder.Name("foo")
                       .Age(30)
                       .Knows(k => k.Name("bar").Age(20).Knows(k => k.References(builder)));
            }, asserts: (person) =>
            {
                person.Name.ShouldBe("foo");
                person.Age.ShouldBe(30);
                person.Knows.Count().ShouldBe(1);
                var knownPerson = person.Knows.ElementAt(0);
                knownPerson.Name.ShouldBe("bar");
                knownPerson.Age.ShouldBe(20);
                knownPerson.Knows.Count().ShouldBe(1);
                knownPerson.Knows.ElementAt(0).ShouldBe(person);
            });

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> correctly handles the case where an invalid age is provided,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void Cannot_Build_Complete_Invalid_Person() => BuilderTester.Invalid<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder) =>
            {
                builder.Name("foo");
            }, assert: (buildResult) =>
            {
                var failure = (FailureObjectBuildResult<Person, PersonBuilder>)buildResult;
                failure.Exceptions.Count().ShouldBe(1);
                failure.Exceptions["_age"].ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
            });
}
