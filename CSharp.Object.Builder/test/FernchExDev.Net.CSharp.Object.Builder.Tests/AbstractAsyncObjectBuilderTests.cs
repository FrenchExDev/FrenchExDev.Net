using FernchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FernchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Tests for the <see cref="AbstractAsyncObjectBuilder{TClass}"/> class.
/// </summary>
[Trait("unit", "virtual")]
public class AbstractAsyncObjectBuilderTests
{
    #region Test classes

    /// <summary>
    /// A builder for constructing instances of <see cref="Person"/>.
    /// </summary>
    internal class PersonBuilder : AbstractAsyncObjectBuilder<Person>
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
        /// Implements the internal build logic for constructing a <see cref="Person"/> instance.
        /// </summary>
        /// <param name="visited"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<IObjectBuildResult<Person>> BuildInternalAsync(VisitedObjectsList visited, CancellationToken cancellationToken = default)
        {
            var addresses = new List<Address>();

            foreach (var addressBuilder in _addresses)
            {
                var addressBuildResult = await addressBuilder.BuildAsync(visited);
                switch (addressBuildResult)
                {
                    case SuccessObjectBuildResult<Address> successResult:
                        addresses.Add(successResult.Result);
                        break;
                    case FailureAsyncObjectBuildResult<Address, AddressBuilder> failureResult:
                        return new FailureAsyncObjectBuildResult<Person, PersonBuilder>(this, failureResult.Exceptions, visited);
                }
            }

            if (string.IsNullOrWhiteSpace(_name))
            {
                return AsyncFailureResult<PersonBuilder>([
                    new BasicAsyncObjectBuildException<Person, PersonBuilder>(ErrorInvalidName, this, visited),
                    new BasicAsyncObjectBuildException<Person, PersonBuilder>(BuildError, this, visited)
                ], visited);
            }

            if (_age == 0)
            {
                return AsyncFailureResult<PersonBuilder>([
                    new BasicAsyncObjectBuildException<Person, PersonBuilder>(ErrorInvalidAge, this, visited),
                    new BasicAsyncObjectBuildException<Person, PersonBuilder>(BuildError, this, visited)
                ], visited);
            }

            return new SuccessObjectBuildResult<Person>(new Person(_name!, _age!.Value, addresses));
        }
    }

    /// <summary>
    /// Provides a builder for constructing instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>This builder allows for the step-by-step construction of an <see cref="Address"/> object by
    /// setting its properties, such as <see cref="Street"/> and <see cref="ZipCode"/>.  The builder validates the
    /// provided values during the build process and ensures that all required fields are set before creating the <see
    /// cref="Address"/> instance.</remarks>
    internal class AddressBuilder : AbstractAsyncObjectBuilder<Address>
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
        /// Simulates the internal build logic for constructing an <see cref="Address"/> instance.
        /// </summary>
        /// <param name="visited"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<IObjectBuildResult<Address>> BuildInternalAsync(VisitedObjectsList visited, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_street))
            {
                return Task.FromResult<IObjectBuildResult<Address>>(AsyncFailureResult<AddressBuilder>([new Exception(ErrorInvalidStreet)], visited));
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                return Task.FromResult<IObjectBuildResult<Address>>(AsyncFailureResult<AddressBuilder>([new Exception(ERrorInvalidZipCode)], visited));
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
        var p1b = new PersonBuilder()
            .Name("foo")
            .Age(30)
            .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
            .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));

        var p1 = await p1b.BuildAsync();

        p1.ShouldBeAssignableTo<SuccessObjectBuildResult<Person>>();

        var person = ((SuccessObjectBuildResult<Person>)p1).Result;

        person.Name.ShouldBe("foo");
        person.Age.ShouldBe(30);
        person.Addresses.Count().ShouldBe(2);
        person.Addresses.ElementAt(0).Street.ShouldBe("123 Main St");
        person.Addresses.ElementAt(0).ZipCode.ShouldBe("12345");
        person.Addresses.ElementAt(1).Street.ShouldBe("456 Elm St");
        person.Addresses.ElementAt(1).ZipCode.ShouldBe("67890");
    }

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> correctly handles the case where an invalid age is provided,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Cannot_Build_Complete_Person_Async()
    {
        var p1b = new PersonBuilder()
            .Name("foo")
            .Age(0)
            ;
        var p1 = await p1b.BuildAsync();

        p1.ShouldBeAssignableTo<FailureAsyncObjectBuildResult<Person, PersonBuilder>>();

        var failure = (FailureAsyncObjectBuildResult<Person, PersonBuilder>)p1;

        failure.ShouldNotBeNull();
        failure.Exceptions.ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
        failure.Exceptions.ElementAt(1).Message.ShouldBe(PersonBuilder.BuildError);
        failure.Builder.ShouldNotBeNull();
        failure.Builder.ShouldBeAssignableTo<PersonBuilder>();
    }
}
