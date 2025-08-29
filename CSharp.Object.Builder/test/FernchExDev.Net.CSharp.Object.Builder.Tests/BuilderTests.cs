using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FernchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Tests for the <see cref="AbstractAsyncBuilder{TClass}"/> class.
/// </summary>
public class BuilderTests
{
    #region Test classes

    /// <summary>
    /// Represents a person with a name, age, and a collection of addresses.
    /// </summary>
    internal class Person
    {
        public string Name { get; }

        public int Age { get; }

        public IEnumerable<Address> Addresses { get; }

        public Person(string name, int age, IEnumerable<Address> addresses)
        {
            Name = name;
            Age = age;
            Addresses = addresses;
        }
    }

    /// <summary>
    /// A builder for constructing instances of <see cref="Person"/>.
    /// </summary>
    internal class PersonBuilder : AbstractAsyncBuilder<Person>
    {
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
        protected override async Task<IBuildResult<Person>> BuildInternalAsync(Dictionary<object, object> visited, CancellationToken cancellationToken = default)
        {
            var addresses = new List<Address>();

            foreach (var addressBuilder in _addresses)
            {
                var addressBuildResult = await addressBuilder.BuildAsync(visited);
                switch (addressBuildResult)
                {
                    case SuccessResult<Address> successResult:
                        addresses.Add(successResult.Result);
                        break;
                    case AsyncFailureResult<Address, AddressBuilder> failureResult:
                        return new AsyncFailureResult<Person, PersonBuilder>(this, failureResult.Exceptions, visited);
                }
            }

            if (string.IsNullOrWhiteSpace(_name))
            {
                return AsyncFailureResult<PersonBuilder>([
                    new BasicAsyncBuildException<Person, PersonBuilder>("Invalid name", this, visited),
                    new BasicAsyncBuildException<Person, PersonBuilder>("Failed to build person", this, visited)
                ], visited);
            }

            if (_age == 0)
            {
                return AsyncFailureResult<PersonBuilder>([
                    new BasicAsyncBuildException<Person, PersonBuilder>("Invalid age", this, visited),
                    new BasicAsyncBuildException<Person, PersonBuilder>("Failed to build person", this, visited)
                ], visited);
            }

            return new SuccessResult<Person>(new Person(_name!, _age!.Value, addresses));
        }
    }

    internal class Address
    {
        public string? Street { get; set; }
        public string? ZipCode { get; set; }

        public Address(string street, string zipCode)
        {
            Street = street;
            ZipCode = zipCode;
        }
    }

    internal class AddressBuilder : AbstractAsyncBuilder<Address>
    {
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
        protected override Task<IBuildResult<Address>> BuildInternalAsync(Dictionary<object, object> visited, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_street))
            {
                return Task.FromResult<IBuildResult<Address>>(AsyncFailureResult<AddressBuilder>([new Exception("Invalid street")], visited));
            }

            if (string.IsNullOrEmpty(_zipCode))
            {
                return Task.FromResult<IBuildResult<Address>>(AsyncFailureResult<AddressBuilder>([new Exception("Invalid zip code")], visited));
            }

            return Task.FromResult<IBuildResult<Address>>(new SuccessResult<Address>(new Address(_street, _zipCode)));
        }
    }

    #endregion

    /// <summary>
    /// Tests whether a <see cref="PersonBuilder"/> can successfully build a complete <see cref="Person"/> object with
    /// multiple addresses and the specified properties.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PersonBuilder"/> correctly constructs a <see
    /// cref="Person"/> instance with the provided name, age, and multiple addresses. It ensures that the resulting
    /// object is of type <see cref="SuccessResult{T}"/> and that all properties are set as expected.</remarks>
    /// <returns></returns>
    [Fact]
    public async Task Can_Build_Complete_Person_Async()
    {
        var p1b = new PersonBuilder()
            .Name("foo")
            .Age(30)
            .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
            .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
        ;

        var p1 = await p1b.BuildAsync();

        p1.ShouldBeAssignableTo<SuccessResult<Person>>();

        var person = ((SuccessResult<Person>)p1).Result;

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
            .Address(ab => ab.Street("123 Main St").ZipCode("12345"))
            .Address(ab => ab.Street("456 Elm St").ZipCode("67890"));
        ;

        var p1 = await p1b.BuildAsync();

        p1.ShouldBeAssignableTo<AsyncFailureResult<Person, PersonBuilder>>();

        var failure = (AsyncFailureResult<Person, PersonBuilder>)p1;

        failure.ShouldNotBeNull();
        failure.Exceptions.ElementAt(0).Message.ShouldBe("Invalid age");
        failure.Exceptions.ElementAt(1).Message.ShouldBe("Failed to build person");
        failure.Builder.ShouldNotBeNull();
        failure.Builder.ShouldBeAssignableTo<PersonBuilder>();
    }
}
