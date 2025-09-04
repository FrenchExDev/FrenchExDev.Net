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
        protected override IObjectBuildResult<Person> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
        {
            // Build all addresses and collect their results
            var addresses = new List<Address>();
            foreach (var addressBuilder in _addresses)
            {
                var addressBuildResult = addressBuilder.Build(visited);
                switch (addressBuildResult)
                {
                    case SuccessObjectBuildResult<Address> successResult:
                        addresses.Add(successResult.Result);
                        break;
                    case FailureObjectBuildResult<Address, AddressBuilder> failureResult:
                        exceptions.Add(new FailureObjectBuildResultException<Address, AddressBuilder>(new FailureObjectBuildResult<Address, AddressBuilder>(addressBuilder, failureResult.Exceptions, visited), "exceptions"));
                        break;
                }
            }

            // Validate name and age
            if (string.IsNullOrWhiteSpace(_name))
            {
                exceptions.Add(new BasicObjectBuildException<Person, PersonBuilder>(ErrorInvalidName, this, visited));
            }

            if (_age == 0)
            {
                exceptions.Add(new BasicObjectBuildException<Person, PersonBuilder>(ErrorInvalidAge, this, visited));
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
            return Success(new Person(_name, _age.Value, addresses));
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
        protected override IObjectBuildResult<Address> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
        {
            // Validate street and zip code
            if (string.IsNullOrEmpty(_street))
            {
                exceptions.Add(new BasicObjectBuildException<Address, AddressBuilder>(ErrorInvalidStreet, this, visited));
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                exceptions.Add(new BasicObjectBuildException<Address, AddressBuilder>(ErrorInvalidZipCode, this, visited));
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
    public async Task Can_Build_Complete_Person() => await BuilderTester.TestValid<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder) =>
            {
                builder.Name("foo")
                       .Age(30)
                       .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
                       .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
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

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> correctly handles the case where an invalid age is provided,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void Cannot_Build_Complete_Person() => BuilderTester.TestInvalid<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder(),
            body: (builder) =>
            {
                builder.Name("foo")
                       .Age(0) // invalid age
                       .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
                       .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
            }, assert: (buildResult) =>
            {
                var failure = (FailureObjectBuildResult<Person, PersonBuilder>)buildResult;
                failure.Exceptions.Count().ShouldBe(1);
                failure.Exceptions.ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
            });
}
