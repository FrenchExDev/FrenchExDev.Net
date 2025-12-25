namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class PersonBuilder : AbstractBuilder<Person>
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public AddressBuilder? AddressBuilder { get; set; }
    public BuilderList<Person, PersonBuilder> Friends { get; } = [];

    protected override Person Instantiate() => new()
    {
        Name = Name!,
        Age = Age,
        Email = Email,
        Address = AddressBuilder?.Reference().ResolvedOrNull(),
        Friends = [.. Friends.AsReferenceList().AsEnumerable()]
    };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
        Assert(() => Age < 0, nameof(Age), failures, n => new ArgumentOutOfRangeException(n, "Age cannot be negative"));
        AddressBuilder?.Validate(visitedCollector, failures);
        ValidateListInternal(Friends, nameof(Friends), visitedCollector, failures);
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        if (AddressBuilder is not null)
            BuildChild(AddressBuilder, visitedCollector);
        BuildList<PersonBuilder, Person>(Friends, visitedCollector);
    }

    public PersonBuilder WithName(string name) { Name = name; return this; }
    public PersonBuilder WithAge(int age) { Age = age; return this; }
    public PersonBuilder WithEmail(string email) { Email = email; return this; }
    public PersonBuilder WithAddress(Action<AddressBuilder> configure)
    {
        AddressBuilder = new AddressBuilder();
        configure(AddressBuilder);
        return this;
    }
    public PersonBuilder WithFriend(Action<PersonBuilder> configure) { Friends.New(configure); return this; }
}
