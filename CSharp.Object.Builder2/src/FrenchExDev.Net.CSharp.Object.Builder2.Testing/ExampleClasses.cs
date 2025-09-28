namespace FrenchExDev.Net.CSharp.Object.Builder2.Testing;

/// <summary>
/// Represents a person with a name, age, contact, addresses, and known persons.
/// </summary>
/// <remarks>
/// This class is constructed via <see cref="PersonBuilder"/> and supports cyclic references through <see cref="PersonReference"/> and <see cref="AddressReference"/>.
/// </remarks>
public class Person
{
    /// <summary>
    /// Gets or sets the name of the person.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the age of the person.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Reference to the contact person, resolved via <see cref="PersonReference"/>.
    /// </summary>
    protected Reference<Person> _contact;

    /// <summary>
    /// List of address references associated with this person.
    /// </summary>
    protected ReferenceList<Address> _addresses;

    /// <summary>
    /// List of known person references associated with this person.
    /// </summary>
    protected ReferenceList<Person> _knownPersons;

    /// <summary>
    /// Gets the contact person, resolving the reference. Throws if not resolved.
    /// </summary>
    public Person Contact => _contact.Resolved();

    /// <summary>
    /// Gets the addresses associated with this person, resolving each reference.
    /// </summary>
    public IEnumerable<Address> Addresses => _addresses.AsEnumerable();

    /// <summary>
    /// Gets the known persons associated with this person, resolving each reference.
    /// </summary>
    public IEnumerable<Person> KnownPersons => _knownPersons.AsEnumerable();

    /// <summary>
    /// Initializes a new instance of <see cref="Person"/> with the specified properties and references.
    /// </summary>
    /// <param name="name">The name of the person.</param>
    /// <param name="age">The age of the person.</param>
    /// <param name="addresses">The list of address references.</param>
    /// <param name="knownPersons">The list of known person references.</param>
    /// <param name="contact">The contact person reference.</param>
    public Person(string name, int age, ReferenceList<Address> addresses, ReferenceList<Person> knownPersons, Reference<Person> contact)
    {
        Name = name;
        Age = age;
        _contact = contact;
        _addresses = addresses;
        _knownPersons = knownPersons;
    }
}

/// <summary>
/// Represents a physical address with a street and city.
/// </summary>
/// <remarks>
/// This class is constructed via <see cref="AddressBuilder"/> and is used as part of a <see cref="Person"/>.
/// </remarks>
public class Address(string street, string city)
{
    /// <summary>
    /// Gets or sets the street name of the address.
    /// </summary>
    public string Street { get; set; } = street;
    /// <summary>
    /// Gets or sets the city name of the address.
    /// </summary>
    public string City { get; set; } = city;
}

/// <summary>
/// Builder for constructing <see cref="Person"/> instances with validation and cyclic reference management.
/// </summary>
/// <remarks>
/// This builder supports fluent configuration of a <see cref="Person"/> object, including name, age, addresses, known persons, and contact.
/// It manages cyclic references using <see cref="PersonReference"/> and <see cref="VisitedObjectDictionary"/>.
/// <para>Example (success):</para>
/// <code>
/// var jane = new PersonBuilder().Name("Jane").Age(28).Address(new AddressBuilder().Street("1 Rue").City("Paris"));
/// var john = new PersonBuilder().Name("John").Age(30).Contact(jane).Address(new AddressBuilder().Street("2 Rue").City("Lyon"));
/// jane.Contact(john);
/// var result = john.Build().Success&lt;Person&gt;();
/// </code>
/// <para>Example (failure):</para>
/// <code>
/// var builder = new PersonBuilder();
/// var result = builder.Build().Failures();
/// </code>
/// </remarks>
public class PersonBuilder : AbstractBuilder<Person>
{
    /// <summary>
    /// Stores the name for the <see cref="Person"/> being built.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Sets the name of the person.
    /// </summary>
    /// <param name="name">The name to assign. Can be null or whitespace, which will cause validation failure.</param>
    /// <returns>The current <see cref="PersonBuilder"/> instance.</returns>
    public PersonBuilder Name(string? name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Stores the age for the <see cref="Person"/> being built.
    /// </summary>
    private int? _age;
    /// <summary>
    /// Sets the age of the person.
    /// </summary>
    /// <param name="age">The age to assign. Must be non-negative; null or negative will cause validation failure.</param>
    /// <returns>The current <see cref="PersonBuilder"/> instance.</returns>
    public PersonBuilder Age(int? age)
    {
        _age = age;
        return this;
    }

    /// <summary>
    /// Stores the contact builder for the <see cref="Person"/> being built.
    /// </summary>
    private PersonBuilder? _contact;
    /// <summary>
    /// Sets the contact person for this person.
    /// </summary>
    /// <param name="contact">A <see cref="PersonBuilder"/> representing the contact. Null will cause validation failure.</param>
    /// <returns>The current <see cref="PersonBuilder"/> instance.</returns>
    public PersonBuilder Contact(PersonBuilder? contact)
    {
        _contact = contact;
        return this;
    }

    /// <summary>
    /// Stores the list of known persons for the <see cref="Person"/> being built.
    /// </summary>
    private readonly BuilderList<Person, PersonBuilder> _knownPersons = [];
    /// <summary>
    /// Adds a known person to this person.
    /// </summary>
    /// <param name="personBuilder">A <see cref="PersonBuilder"/> representing a known person.</param>
    /// <returns>The current <see cref="PersonBuilder"/> instance.</returns>
    public PersonBuilder KnownPerson(PersonBuilder personBuilder)
    {
        _knownPersons.Add(personBuilder);
        return this;
    }

    /// <summary>
    /// Stores the list of addresses for the <see cref="Person"/> being built.
    /// </summary>
    private readonly BuilderList<Address, AddressBuilder> _addresses = [];
    /// <summary>
    /// Adds an address to this person.
    /// </summary>
    /// <param name="addressBuilder">An <see cref="AddressBuilder"/> representing an address.</param>
    /// <returns>The current <see cref="PersonBuilder"/> instance.</returns>
    public PersonBuilder Address(AddressBuilder addressBuilder)
    {
        _addresses.Add(addressBuilder);
        return this;
    }

    public class PersonBuilderException : Exception
    {
        public PersonBuilderException()
        {
        }

        public PersonBuilderException(string? message) : base(message)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the name is null, empty, or whitespace.
    /// </summary>
    public class NameCannotBeNullOrEmptyOrWhitespaceException : PersonBuilderException
    {
        public NameCannotBeNullOrEmptyOrWhitespaceException() : base("Name cannot be null or empty or whitespace") { }
    }

    /// <summary>
    /// Exception thrown when the age is null or negative.
    /// </summary>
    public class AgeMustBeNonNegativeException : PersonBuilderException
    {
        public AgeMustBeNonNegativeException() : base("Age must be a non-negative integer") { }
    }

    /// <summary>
    /// Exception thrown when no address is provided.
    /// </summary>
    public class AtLeastOneAddressMustBeProvidedException : PersonBuilderException
    {
        public AtLeastOneAddressMustBeProvidedException() : base("At least one address must be provided") { }
    }

    /// <summary>
    /// Exception thrown when no contact is provided.
    /// </summary>
    public class MustHaveContactException : PersonBuilderException
    {
        public MustHaveContactException() : base("Person must have a contact") { }
    }

    /// <summary>
    /// Exception thrown when no known person is provided.
    /// </summary>
    public class MustKnowAtLeastOnePersonException : PersonBuilderException
    {
        public MustKnowAtLeastOnePersonException() : base("Person must know at least one other person") { }
    }

    /// <summary>
    /// Validates the builder's state and collects failures for invalid configuration.
    /// </summary>
    /// <param name="visitedCollector">Dictionary for tracking visited objects to prevent cycles.</param>
    /// <param name="failures">Dictionary for collecting validation failures.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(_name), failures, (s) => new NameCannotBeNullOrEmptyOrWhitespaceException());

        if (_addresses.Count == 0)
        {
            failures.Failure(nameof(_addresses), new AtLeastOneAddressMustBeProvidedException());
        }

        if (_age is null || _age < 0)
        {
            failures.Failure(nameof(_age), new AgeMustBeNonNegativeException());
        }

        if (_contact is not null)
        {
            var contactFailures = new FailuresDictionary();
            _contact.Validate(visitedCollector, contactFailures);
            if (contactFailures.Count > 0)
            {
                failures.Failure(nameof(_contact), contactFailures);
            }
        }
        else
        {
            failures.Failure(nameof(_contact), new MustHaveContactException());
        }

        if (_knownPersons.Count == 0)
        {
            failures.Failure(nameof(_knownPersons), new MustKnowAtLeastOnePersonException());
        }

        foreach (var person in _knownPersons)
        {
            var personFailures = new FailuresDictionary();
            person.Validate(visitedCollector, personFailures);
            if (personFailures.Count > 0)
            {
                failures.Failure(nameof(person), personFailures);
            }
        }
    }

    /// <summary>
    /// Instantiates a <see cref="Person"/> using the builder's configuration. Throws if validation fails.
    /// </summary>
    /// <returns>A new <see cref="Person"/> instance.</returns>
    /// <exception cref="NameCannotBeNullOrEmptyOrWhitespaceException">If name is invalid.</exception>
    /// <exception cref="AtLeastOneAddressMustBeProvidedException">If no address is provided.</exception>
    /// <exception cref="AgeMustBeNonNegativeException">If age is invalid.</exception>
    /// <exception cref="MustHaveContactException">If contact is missing.</exception>
    protected override Person Instantiate()
    {
        if (_name is null) throw new NameCannotBeNullOrEmptyOrWhitespaceException();
        if (_addresses.Count == 0) throw new AtLeastOneAddressMustBeProvidedException();
        if (_age is null) throw new AgeMustBeNonNegativeException();
        if (_contact is null) throw new MustHaveContactException();

        return new Person(
            _name,
            _age.Value,
            _addresses.AsReferenceList(),
            _knownPersons.AsReferenceList(),
            _contact.Reference());
    }

    /// <summary>
    /// Builds all nested builders (contact, known persons, addresses) to resolve cyclic references.
    /// </summary>
    /// <param name="visitedCollector">Dictionary for tracking visited objects.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        _contact?.Build(visitedCollector);
        foreach (var person in _knownPersons)
        {
            person.Build(visitedCollector);
        }
        foreach (var address in _addresses)
        {
            address.Build(visitedCollector);
        }
    }
}

public class AddressBuilder : AbstractBuilder<Address>
{
    private string? _street;
    public AddressBuilder Street(string? street)
    {
        _street = street;
        return this;
    }

    private string? _city;
    public AddressBuilder City(string? city)
    {
        _city = city;
        return this;
    }

    internal class StreetCannotBeNullOrEmptyException : Exception
    {
        public StreetCannotBeNullOrEmptyException() : base("Street cannot be null or empty") { }
    }

    internal class CityCannotBeNullOrEmptyException : Exception
    {
        public CityCannotBeNullOrEmptyException() : base("City cannot be null or empty") { }
    }

    protected new void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_street, nameof(_street), failures, (s) => new StreetCannotBeNullOrEmptyException());
        AssertNotNullOrEmptyOrWhitespace(_city, nameof(_city), failures, (s) => new CityCannotBeNullOrEmptyException());
    }

    protected override Address Instantiate()
    {
        if (_street is null) throw new StreetCannotBeNullOrEmptyException();
        if (_city is null) throw new CityCannotBeNullOrEmptyException();

        return new Address(_street, _city);
    }
}