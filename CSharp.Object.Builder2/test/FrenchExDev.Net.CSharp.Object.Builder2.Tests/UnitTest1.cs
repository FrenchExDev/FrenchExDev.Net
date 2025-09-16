using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public static class Extensions
{
    public static TClass Success<TClass>(this IBuildResult result) where TClass : class
    {
        return result switch
        {
            SuccessBuildResult<TClass> success => success.Object,
            _ => throw new InvalidOperationException(),
        };
    }

    public static FailuresDictionary Failures<TClass>(this IBuildResult result) where TClass : class
    {
        return result switch
        {
            FailureBuildResult failure => failure.Failures,
            _ => throw new InvalidOperationException(),
        };
    }
}

public interface IBuildResult
{

}

public class SuccessBuildResult<TClass>(TClass instance) : IBuildResult where TClass : class
{
    public TClass Object { get; } = instance;
}

public class FailuresDictionary : Dictionary<string, List<object>>
{
    /// <summary>
    /// Adds an entry to the dictionary that associates the specified member name with the given exception to be thrown
    /// for invalid data.
    /// </summary>
    /// <param name="memberName">The name of the member to associate with the exception. Cannot be null.</param>
    /// <param name="exception">The exception to be thrown when the specified member contains invalid data. Cannot be null.</param>
    /// <returns>The current <see cref="ExceptionBuildDictionary"/> instance with the new association added.</returns>
    public FailuresDictionary Failure<T>(string memberName, T failure) where T : class
    {
        var list = this.GetValueOrDefault(memberName);
        if (list is null)
        {
            list = [];
            this[memberName] = list;
        }
        list.Add(failure);
        return this;
    }

}

public class FailureBuildResult(FailuresDictionary failures) : IBuildResult
{
    public FailuresDictionary Failures { get; } = failures;
}

public class VisitedObjectDictionary : Dictionary<Guid, object>
{

}

public interface IBuilder<TClass, TReference> where TClass : class where TReference : class, IReference<TClass>
{
    TReference Reference();
    Guid Id { get; }
    IBuildResult? Result { get; }
    IBuildResult? Build(VisitedObjectDictionary? visitedCollector = null);
    void OnBuilt(Action<TClass> hook);
}

public interface IReference<TClass> : IBuildResult where TClass : class
{
    TClass? Instance { get; }
    bool IsResolved => Instance is not null;
    void Resolve(TClass instance);
}

public abstract class AbstractReference<TClass> : IReference<TClass> where TClass : class
{
    public TClass? Instance { get; protected set; }
    public bool IsResolved => Instance is not null;
    public void Resolve(TClass instance)
    {
        Instance = instance;
    }
}

public abstract class AbstractBuilder<TClass, TReference> : IBuilder<TClass, TReference> where TClass : class where TReference : class, IReference<TClass>, new()
{
    private readonly List<Action<TClass>> _hooks = [];
    protected TReference _reference = new();
    public Guid Id { get; } = Guid.NewGuid();
    public TReference Reference() => _reference;
    public IBuildResult? Result { get; protected set; }
    public IBuildResult GetResult() => Result ?? throw new InvalidOperationException("You must call Build() before accessing the Result.");
    public IBuildResult Build(VisitedObjectDictionary? visitedCollector = null)
    {
        if (visitedCollector is not null && visitedCollector.ContainsKey(Id))
        {
            return Reference();
        }

        visitedCollector ??= [];

        visitedCollector.Add(Id, this);

        var failuresCollector = new FailuresDictionary();

        Validate(visitedCollector, failuresCollector);

        BuildInternal(visitedCollector);

        Result = failuresCollector.Count > 0 ? Failure(failuresCollector) : Success(Instantiate());

        if (Result is SuccessBuildResult<TClass> success)
            _reference.Resolve(success.Object);

        return Result;
    }

    protected abstract void BuildInternal(VisitedObjectDictionary visitedCollector);

    protected void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (visitedCollector.ContainsKey(Id))
        {
            return;
        }

        visitedCollector[this.Id] = this;

        ValidateInternal(visitedCollector, failures);
    }

    protected abstract void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures);

    protected abstract TClass Instantiate();

    protected static SuccessBuildResult<TClass> Success(TClass instance)
    {
        return new SuccessBuildResult<TClass>(instance);
    }

    protected static FailureBuildResult Failure(FailuresDictionary failures)
    {
        return new FailureBuildResult(failures);
    }

    public void OnBuilt(Action<TClass> hook)
    {
        _hooks.Add(hook);
    }
}

public class UnitTest1
{
    internal class PersonReference : AbstractReference<Person> { }

    internal class AddressReference : AbstractReference<Address> { }

    internal class Person(string name, int age, List<AddressReference> addresses, List<PersonReference> knownPersons, PersonReference contact)
    {
        public string Name { get; set; } = name;
        public int Age { get; set; } = age;
        public PersonReference Contact { get; protected set; } = contact;
        public List<AddressReference> Addresses { get; protected set; } = addresses;
        public List<PersonReference> KnownPersons { get; protected set; } = knownPersons;
    }

    internal record Address(string Street, string City);

    internal class PersonBuilder : AbstractBuilder<Person, PersonReference>
    {
        private string? _name;
        public PersonBuilder Name(string? name)
        {
            _name = name;
            return this;
        }

        private int? _age;
        public PersonBuilder Age(int? age)
        {
            _age = age;
            return this;
        }

        private PersonBuilder? _contact;
        public PersonBuilder Contact(PersonBuilder? contact)
        {
            _contact = contact;
            return this;
        }

        private readonly List<PersonBuilder> _knownPersons = [];
        public PersonBuilder KnownPerson(PersonBuilder personBuilder)
        {
            _knownPersons.Add(personBuilder);
            return this;
        }

        private readonly List<AddressBuilder> _addresses = [];
        public PersonBuilder Address(AddressBuilder addressBuilder)
        {
            _addresses.Add(addressBuilder);
            return this;
        }

        internal class NameCannotBeNullOrEmptyOrWhitespaceException : Exception
        {
            public NameCannotBeNullOrEmptyOrWhitespaceException() : base("Name cannot be null or empty or whitespace") { }
        }

        internal class AgeMustBeNonNegativeException : Exception
        {
            public AgeMustBeNonNegativeException() : base("Age must be a non-negative integer") { }
        }

        internal class AtLeastOneAddressMustBeProvidedException : Exception
        {
            public AtLeastOneAddressMustBeProvidedException() : base("At least one address must be provided") { }
        }

        internal class MustHaveContactException : Exception
        {
            public MustHaveContactException() : base("Person must have a contact") { }
        }

        protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                failures.Failure(nameof(_name), new NameCannotBeNullOrEmptyOrWhitespaceException());
            }

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

        protected override Person Instantiate()
        {
            if (_name is null) throw new NameCannotBeNullOrEmptyOrWhitespaceException();
            if (_addresses.Count == 0) throw new AtLeastOneAddressMustBeProvidedException();
            if (_age is null) throw new AgeMustBeNonNegativeException();
            if (_contact is null) throw new MustHaveContactException();

            var contactReference = _contact.Reference();
            _contact.OnBuilt(contactReference.Resolve);

            var knownPersonsReferences = new List<PersonReference>();
            foreach (var person in _knownPersons)
            {
                var personReference = person.Reference();
                person.OnBuilt(personReference.Resolve);
                knownPersonsReferences.Add(personReference);
            }

            var addressesReferences = new List<AddressReference>();
            foreach (var address in _addresses)
            {
                var addressReference = address.Reference();
                address.OnBuilt(addressReference.Resolve);
                addressesReferences.Add(addressReference);
            }

            return new Person(
                _name,
                _age.Value,
                addressesReferences,
                knownPersonsReferences,
                contactReference);
        }

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

    internal class AddressBuilder : AbstractBuilder<Address, AddressReference>
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

        protected override void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
        {
            if (string.IsNullOrWhiteSpace(_street))
            {
                failures.Failure(nameof(_street), new StreetCannotBeNullOrEmptyException());
            }

            if (string.IsNullOrWhiteSpace(_city))
            {
                failures.Failure(nameof(_city), new CityCannotBeNullOrEmptyException());
            }
        }

        protected override Address Instantiate()
        {
            ArgumentNullException.ThrowIfNull(_street, nameof(_street));
            ArgumentNullException.ThrowIfNull(_city, nameof(_city));
            return new Address(_street, _city);
        }

        protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
        {
            // No nested builders to build
        }
    }

    [Fact]
    public void Test1()
    {
        var janeSmith = new PersonBuilder()
                .Name("Jane Smith")
                .Age(28)
                .Address(new AddressBuilder()
                    .Street("123 Main St")
                    .City("Anytown"));

        var johnDoe = new PersonBuilder()
            .Name("John Doe")
            .Age(30)
            .Contact(janeSmith)
            .Address(new AddressBuilder()
                .Street("456 Elm St")
                .City("Othertown"))
            .KnownPerson(new PersonBuilder()
                .Name("Alice Johnson")
                .Age(35)
                .Contact(janeSmith)
                .Address(new AddressBuilder()
                    .Street("789 Oak St")
                    .City("Sometown")))
            .KnownPerson(janeSmith);

        janeSmith.Contact(johnDoe);

        var result = johnDoe.Build();

        result.ShouldBeAssignableTo<SuccessBuildResult<Person>>();

        var person = result.Success<Person>();

        person.ShouldBeAssignableTo<Person>();

        person.Contact.IsResolved.ShouldBeTrue();
    }
}
