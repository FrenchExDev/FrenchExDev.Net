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
    /// <remarks>This builder allows for the step-by-step construction of a <see cref="Person"/> object, 
    /// including setting the name, age, and associated addresses. It validates the input data  during the build process
    /// and returns either a successfully constructed <see cref="Person"/>  or a failure result with detailed
    /// exceptions.</remarks>
    internal class PersonBuilder : AbstractObjectBuilder<Person, PersonBuilder>
    {
        public const string ErrorInvalidName = "Invalid name";
        public const string ErrorInvalidAge = "Invalid age";
        public const string BuildError = "Failed to build person";
        private string? _name;
        private int? _age;
        private List<AddressBuilder> _addresses = new();

        public PersonBuilder Name(string? name)
        {
            _name = name;
            return this;
        }

        public PersonBuilder Age(int? age)
        {
            _age = age;
            return this;
        }

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
        /// <remarks>The method validates the configured properties, such as <c>_name</c> and <c>_age</c>,
        /// and ensures that all child builders for <see cref="Address"/> objects produce successful results. If any
        /// validation fails or a child builder returns a failure, the method returns a failure result with the relevant
        /// exceptions.</remarks>
        /// <param name="exceptions">A collection to which any exceptions encountered during the build process are added.</param>
        /// <param name="visited">A list of objects that have already been visited during the build process to prevent circular references.</param>
        /// <returns>A <see cref="SuccessObjectBuildResult{T}"/> containing the constructed <see cref="Person"/> object if the
        /// build is successful; otherwise, a <see cref="FailureObjectBuildResult{T, TBuilder}"/> containing the
        /// encountered exceptions.</returns>
        protected override IObjectBuildResult<Person> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
        {
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
                        return new FailureObjectBuildResult<Person, PersonBuilder>(this, failureResult.Exceptions, visited);
                }
            }
            if (string.IsNullOrWhiteSpace(_name))
            {
                return FailureResult(ErrorInvalidName, visited);
            }
            if (_age == 0)
            {
                return FailureResult(ErrorInvalidAge, visited);
            }
            return new SuccessObjectBuildResult<Person>(new Person(_name!, _age!.Value, addresses));
        }
    }

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>This builder allows for the step-by-step construction of an <see cref="Address"/> object by
    /// setting its properties, such as <see cref="Street"/> and <see cref="ZipCode"/>.  The builder validates the
    /// required fields during the build process and returns either a  successful result or a failure result with
    /// appropriate error messages.</remarks>
    internal class AddressBuilder : AbstractObjectBuilder<Address, AddressBuilder>
    {
        public const string ErrorInvalidStreet = "Invalid street";
        public const string ERrorInvalidZipCode = "Invalid zip code";
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
        /// Builds an <see cref="Address"/> object based on the current state of the builder.
        /// </summary>
        /// <param name="exceptions">A list to which any exceptions encountered during the build process are added.</param>
        /// <param name="visited">A list of objects visited during the build process to prevent circular references.</param>
        /// <returns>A successful build result containing the constructed <see cref="Address"/> object if the required fields are
        /// valid; otherwise, a failure result with the appropriate error message.</returns>
        protected override IObjectBuildResult<Address> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
        {
            if (string.IsNullOrEmpty(_street))
            {
                return FailureResult(ErrorInvalidStreet, visited);
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                return FailureResult(ERrorInvalidZipCode, visited);
            }

            return new SuccessObjectBuildResult<Address>(new Address(_street, _zipCode));
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
            () => new PersonBuilder(),
            (builder) =>
            {
                builder.Name("foo")
                       .Age(30)
                       .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
                       .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
            }, (person) =>
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
    public void Cannot_Build_Complete_Person()
    {
        BuilderTester.TestInvalid<PersonBuilder, Person>(
            () => new PersonBuilder(),
            (builder) =>
            {
                builder.Name("foo")
                       .Age(0)
                       .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
                       .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
            }, (buildResult) =>
            {
                var failure = (FailureObjectBuildResult<Person, PersonBuilder>)buildResult;
                failure.Exceptions.Count().ShouldBe(1);
                failure.Exceptions.ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
            });
    }
}
