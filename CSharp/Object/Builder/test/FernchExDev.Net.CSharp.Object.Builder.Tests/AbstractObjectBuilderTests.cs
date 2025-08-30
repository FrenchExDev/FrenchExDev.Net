using FernchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FernchExDev.Net.CSharp.Object.Builder.Tests;

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
    #region Test classes

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

        protected override IObjectBuildResult<Person> BuildInternal(VisitedObjectsList visited)
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
        protected override IObjectBuildResult<Address> BuildInternal(VisitedObjectsList visited)
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
    public void Can_Build_Complete_Person_Async()
    {
        var p1b = new PersonBuilder()
            .Name("foo")
            .Age(30)
            .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
            .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));

        var p1 = p1b.Build();

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
    public void Cannot_Build_Complete_Person_Async()
    {
        var p1b = new PersonBuilder()
            .Name("foo")
            .Age(0)
            ;
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
