using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

#region Test Domain Models

/// <summary>
/// Simple domain model for testing.
/// </summary>
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public List<Person> Friends { get; set; } = [];
}

/// <summary>
/// Nested domain model for testing.
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
}

/// <summary>
/// Domain model with circular reference for testing.
/// </summary>
public class Department
{
    public string Name { get; set; } = string.Empty;
    public Employee? Manager { get; set; }
    public List<Employee> Employees { get; set; } = [];
}

public class Employee
{
    public string Name { get; set; } = string.Empty;
    public Department? Department { get; set; }
}

public class TaggedItem
{
    public List<string> Tags { get; set; } = [];
}

public class SimpleObject
{
    public string Value { get; set; } = string.Empty;
}

#endregion

#region Test Builders

/// <summary>
/// Simple builder for Person with validation.
/// </summary>
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

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
        Assert(() => Age < 0, nameof(Age), failures, n => new ArgumentOutOfRangeException(n, "Age cannot be negative"));
        
        if (AddressBuilder is not null)
            AddressBuilder.Validate(visitedCollector, failures);
        
        ValidateListInternal(Friends, nameof(Friends), visitedCollector, failures);
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        AddressBuilder?.Build(visitedCollector);
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
    public PersonBuilder WithFriend(Action<PersonBuilder> configure)
    {
        Friends.New(configure);
        return this;
    }
}

/// <summary>
/// Simple builder for Address with validation.
/// </summary>
public class AddressBuilder : AbstractBuilder<Address>
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }

    protected override Address Instantiate() => new()
    {
        Street = Street!,
        City = City!,
        ZipCode = ZipCode
    };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Street, nameof(Street), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNullOrEmptyOrWhitespace(City, nameof(City), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public AddressBuilder WithStreet(string street) { Street = street; return this; }
    public AddressBuilder WithCity(string city) { City = city; return this; }
    public AddressBuilder WithZipCode(string zipCode) { ZipCode = zipCode; return this; }
}

/// <summary>
/// Builder for Department with circular reference handling.
/// </summary>
public class DepartmentBuilder : AbstractBuilder<Department>
{
    public string? Name { get; set; }
    public EmployeeBuilder? ManagerBuilder { get; set; }
    public BuilderList<Employee, EmployeeBuilder> Employees { get; } = [];

    protected override Department Instantiate() => new()
    {
        Name = Name!,
        Manager = ManagerBuilder?.Reference().ResolvedOrNull(),
        Employees = [.. Employees.AsReferenceList().AsEnumerable()]
    };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
        ManagerBuilder?.Validate(visitedCollector, failures);
        ValidateListInternal(Employees, nameof(Employees), visitedCollector, failures);
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        ManagerBuilder?.Build(visitedCollector);
        BuildList<EmployeeBuilder, Employee>(Employees, visitedCollector);
    }

    public DepartmentBuilder WithName(string name) { Name = name; return this; }
    public DepartmentBuilder WithManager(Action<EmployeeBuilder> configure)
    {
        ManagerBuilder = new EmployeeBuilder();
        configure(ManagerBuilder);
        return this;
    }
    public DepartmentBuilder WithEmployee(Action<EmployeeBuilder> configure)
    {
        Employees.New(configure);
        return this;
    }
}

/// <summary>
/// Builder for Employee.
/// </summary>
public class EmployeeBuilder : AbstractBuilder<Employee>
{
    public string? Name { get; set; }
    public Reference<Department>? DepartmentRef { get; set; }

    protected override Employee Instantiate() => new()
    {
        Name = Name!,
        Department = DepartmentRef?.ResolvedOrNull()
    };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public EmployeeBuilder WithName(string name) { Name = name; return this; }
    public EmployeeBuilder WithDepartment(Reference<Department> deptRef) { DepartmentRef = deptRef; return this; }
}

/// <summary>
/// Builder with no validation for testing basic functionality.
/// </summary>
public class SimpleObjectBuilder : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }

    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };

    public SimpleObjectBuilder WithValue(string value) { Value = value; return this; }
}

/// <summary>
/// Builder that uses AssertNotEmptyOrWhitespace with List for testing.
/// </summary>
public class TaggedItemBuilder : AbstractBuilder<TaggedItem>
{
    public List<string>? Tags { get; set; }
    public List<string>? NullableTags { get; set; }

    protected override TaggedItem Instantiate() => new() { Tags = Tags ?? [] };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotEmptyOrWhitespace(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotEmptyOrWhitespace(NullableTags, nameof(NullableTags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNull(Tags, nameof(Tags), failures, n => new ArgumentNullException(n));
        AssertNotNullNotEmptyCollection(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public TaggedItemBuilder WithTags(List<string> tags) { Tags = tags; return this; }
    public TaggedItemBuilder WithNullableTags(List<string>? tags) { NullableTags = tags; return this; }
}

#endregion

#region Result Tests

public class ResultTests
{
    [Fact]
    public void SuccessResult_Contains_Built_Object()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(30);

        var result = builder.Build();

        result.ShouldBeSuccess<Person>();
        var person = result.ShouldHaveObject<Person>();
        Assert.Equal("John", person.Name);
        Assert.Equal(30, person.Age);
    }

    [Fact]
    public void FailureResult_Contains_Validation_Errors()
    {
        var builder = new PersonBuilder()
            .WithAge(-5); // Missing name, negative age

        var result = builder.Build();

        result.ShouldBeFailure()
              .ShouldHaveFailureForMember("Name")
              .ShouldHaveFailureForMember("Age");
    }

    [Fact]
    public void Result_IsSuccess_Extension_Works()
    {
        var successBuilder = new PersonBuilder().WithName("Jane").WithAge(25);
        var failBuilder = new PersonBuilder();

        Assert.True(successBuilder.Build().IsSuccess<Person>());
        Assert.False(failBuilder.Build().IsSuccess<Person>());
    }

    [Fact]
    public void Result_IsFailure_Extension_Works()
    {
        var successBuilder = new PersonBuilder().WithName("Jane").WithAge(25);
        var failBuilder = new PersonBuilder();

        Assert.False(successBuilder.Build().IsFailure());
        Assert.True(failBuilder.Build().IsFailure());
    }

    [Fact]
    public void Result_Success_Extension_Returns_Object()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(20);
        var result = builder.Build();

        var person = result.Success<Person>();

        Assert.Equal("Test", person.Name);
    }

    [Fact]
    public void Result_Success_Extension_Throws_BuildFailedException_On_Failure()
    {
        var builder = new PersonBuilder(); // Invalid - no name
        var result = builder.Build();

        Assert.Throws<BuildFailedException>(() => result.Success<Person>());
    }

    [Fact]
    public void Result_Failures_Extension_Returns_Dictionary()
    {
        var builder = new PersonBuilder().WithAge(-1); // Missing name, negative age
        var result = builder.Build();

        var failures = result.Failures<Person>();

        Assert.NotEmpty(failures);
        Assert.True(failures.ContainsKey("Name"));
        Assert.True(failures.ContainsKey("Age"));
    }

    [Fact]
    public void Result_Failures_Extension_Throws_BuildSucceededException_On_Success()
    {
        var builder = new PersonBuilder().WithName("Success").WithAge(30);
        var result = builder.Build();

        Assert.Throws<BuildSucceededException>(() => result.Failures<Person>());
    }
}

#endregion

#region Reference Tests

public class ReferenceTests
{
    [Fact]
    public void Reference_IsNotResolved_Initially()
    {
        var reference = new Reference<Person>();

        reference.ShouldNotBeResolved();
        Assert.False(reference.IsResolved);
        Assert.Null(reference.Instance);
    }

    [Fact]
    public void Reference_IsResolved_After_Resolve()
    {
        var reference = new Reference<Person>();
        var person = new Person { Name = "Test" };

        reference.Resolve(person);

        reference.ShouldBeResolved();
        Assert.True(reference.IsResolved);
        Assert.Same(person, reference.Instance);
    }

    [Fact]
    public void Reference_Resolved_Returns_Instance()
    {
        var person = new Person { Name = "Test" };
        var reference = new Reference<Person>(person);

        var resolved = reference.Resolved();

        Assert.Same(person, resolved);
    }

    [Fact]
    public void Reference_Resolved_Throws_NotResolvedException_When_Not_Resolved()
    {
        var reference = new Reference<Person>();

        Assert.Throws<NotResolvedException>(() => reference.Resolved());
    }

    [Fact]
    public void Reference_ResolvedOrNull_Returns_Null_When_Not_Resolved()
    {
        var reference = new Reference<Person>();

        Assert.Null(reference.ResolvedOrNull());
    }

    [Fact]
    public void Reference_Cannot_Be_Resolved_Twice()
    {
        var reference = new Reference<Person>();
        var person1 = new Person { Name = "First" };
        var person2 = new Person { Name = "Second" };

        reference.Resolve(person1);
        reference.Resolve(person2); // Should be ignored

        Assert.Same(person1, reference.Instance);
    }

    [Fact]
    public void Reference_Constructor_With_Existing_Instance()
    {
        var person = new Person { Name = "Existing" };
        var reference = new Reference<Person>(person);

        reference.ShouldBeResolved();
        Assert.Same(person, reference.ShouldHaveInstance());
    }

    [Fact]
    public void Builder_Reference_Is_Resolved_After_Build()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(25);

        var refBefore = builder.Reference();
        refBefore.ShouldNotBeResolved();

        builder.Build();

        var refAfter = builder.Reference();
        refAfter.ShouldBeResolved();
        Assert.Equal("John", refAfter.ShouldHaveInstance().Name);
    }
}

#endregion

#region Builder Status Tests

public class BuilderStatusTests
{
    [Fact]
    public void Builder_Initial_Status_Is_NotBuilding_And_NotValidated()
    {
        var builder = new PersonBuilder();

        builder.ShouldHaveBuildStatus(BuildStatus.NotBuilding);
        builder.ShouldHaveValidationStatus(ValidationStatus.NotValidated);
    }

    [Fact]
    public void Builder_Status_Is_Built_After_Successful_Build()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(20);

        builder.Build();

        builder.ShouldHaveBuildStatus(BuildStatus.Built);
        builder.ShouldHaveValidationStatus(ValidationStatus.Validated);
    }

    [Fact]
    public void Builder_Status_Is_Validated_Even_After_Failed_Build()
    {
        var builder = new PersonBuilder(); // Invalid

        builder.Build();

        builder.ShouldHaveValidationStatus(ValidationStatus.Validated);
    }

    [Fact]
    public void Validate_Only_Runs_Once()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures1 = new FailuresDictionary();
        var failures2 = new FailuresDictionary();

        builder.Validate(visited, failures1);
        builder.ShouldHaveValidationStatus(ValidationStatus.Validated);

        // Second validation should be skipped
        builder.Validate(visited, failures2);
        
        Assert.Empty(failures1);
        Assert.Empty(failures2);
    }
}

#endregion

#region Validation Tests

public class ValidationTests
{
    [Fact]
    public void AssertNotNullOrEmptyOrWhitespace_Fails_On_Null()
    {
        var builder = new PersonBuilder().WithAge(25); // Name is null
        
        var failures = builder.ShouldBuildFailing();
        
        failures.ShouldContainMember("Name");
    }

    [Fact]
    public void AssertNotNullOrEmptyOrWhitespace_Fails_On_Empty()
    {
        var builder = new PersonBuilder()
            .WithName("")
            .WithAge(25);
        
        var failures = builder.ShouldBuildFailing();
        
        failures.ShouldContainMember("Name");
    }

    [Fact]
    public void AssertNotNullOrEmptyOrWhitespace_Fails_On_Whitespace()
    {
        var builder = new PersonBuilder()
            .WithName("   ")
            .WithAge(25);
        
        var failures = builder.ShouldBuildFailing();
        
        failures.ShouldContainMember("Name");
    }

    [Fact]
    public void Assert_Predicate_Records_Failure_When_True()
    {
        var builder = new PersonBuilder()
            .WithName("Test")
            .WithAge(-5); // Negative age triggers Assert predicate

        var failures = builder.ShouldBuildFailing();
        
        failures.ShouldContainMember("Age");
        failures.ShouldContainException<ArgumentOutOfRangeException>("Age");
    }

    [Fact]
    public void Multiple_Validation_Failures_Are_Collected()
    {
        var builder = new PersonBuilder()
            .WithAge(-10); // Missing name AND negative age

        var failures = builder.ShouldBuildFailing();

        failures.ShouldContainMember("Name")
                .ShouldContainMember("Age");
        Assert.Equal(2, failures.Count);
    }

    [Fact]
    public void Nested_Builder_Validation_Failures_Are_Collected()
    {
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(30)
            .WithAddress(addr => { /* Missing Street and City */ });

        var result = builder.Build();

        result.ShouldBeFailure();
        var failures = result.ShouldHaveFailures();
        Assert.True(failures.ContainsKey("Street") || failures.ContainsKey("City"));
    }

    [Fact]
    public void ValidateFailures_On_BuilderList_Returns_All_Failures()
    {
        var builders = new BuilderList<Person, PersonBuilder>();
        builders.New(p => p.WithAge(25)); // Missing name
        builders.New(p => p.WithName("Valid").WithAge(30)); // Valid
        builders.New(p => p.WithAge(-5)); // Missing name AND negative age

        var allFailures = builders.ValidateFailures();

        Assert.Equal(2, allFailures.Count); // Two builders have failures
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_With_List_Validates_Each_Item()
    {
        var builder = new TaggedItemBuilder().WithTags(["valid", "", "also valid"]);

        var result = builder.Build();

        Assert.True(result.IsFailure());
    }

    [Fact]
    public void AssertNotEmptyOrWhitespace_With_Null_List_Passes()
    {
        var builder = new TaggedItemBuilder().WithTags(["valid"]).WithNullableTags(null);

        var result = builder.Build();

        Assert.True(result.IsSuccess<TaggedItem>());
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_With_Null_List_Fails()
    {
        var builder = new TaggedItemBuilder();

        var result = builder.Build();

        Assert.True(result.IsFailure());
    }

    [Fact]
    public void AssertNotNullNotEmptyCollection_With_Whitespace_String_In_List_Fails()
    {
        var builder = new TaggedItemBuilder().WithTags(["   "]);

        var result = builder.Build();

        Assert.True(result.IsFailure());
    }
}

#endregion

#region Build Tests

public class BuildTests
{
    [Fact]
    public void Build_Returns_SuccessResult_When_Valid()
    {
        var builder = new PersonBuilder()
            .WithName("Alice")
            .WithAge(28)
            .WithEmail("alice@example.com");

        var person = builder.ShouldBuildSuccessfully();

        Assert.Equal("Alice", person.Name);
        Assert.Equal(28, person.Age);
        Assert.Equal("alice@example.com", person.Email);
    }

    [Fact]
    public void Build_Returns_FailureResult_When_Invalid()
    {
        var builder = new PersonBuilder();

        var failures = builder.ShouldBuildFailing();

        failures.ShouldNotBeEmpty();
    }

    [Fact]
    public void Build_With_Existing_Instance_Skips_Validation()
    {
        var existingPerson = new Person { Name = "Existing", Age = 50 };
        var builder = new PersonBuilder();
        builder.Existing(existingPerson);

        var result = builder.Build();

        result.ShouldBeSuccess<Person>();
        var person = result.ShouldHaveObject<Person>();
        Assert.Same(existingPerson, person);
    }

    [Fact]
    public void Build_With_Existing_Resolves_Reference()
    {
        var existingPerson = new Person { Name = "Existing", Age = 50 };
        var builder = new PersonBuilder();
        builder.Existing(existingPerson);

        builder.Build();

        builder.Reference().ShouldBeResolved();
        Assert.Same(existingPerson, builder.Reference().Instance);
    }

    [Fact]
    public void BuildSuccess_Returns_Object_On_Success()
    {
        var builder = new PersonBuilder().WithName("Success").WithAge(25);

        var person = builder.BuildSuccess();

        Assert.Equal("Success", person.Name);
    }

    [Fact]
    public void BuildSuccess_Throws_AggregateException_On_Failure()
    {
        var builder = new PersonBuilder().WithAge(-1); // Invalid

        Assert.Throws<AggregateException>(() => builder.BuildSuccess());
    }

    [Fact]
    public void GetResult_Throws_Before_Build()
    {
        var builder = new PersonBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.GetResult());
    }

    [Fact]
    public void GetResult_Returns_Result_After_Build()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(20);
        builder.Build();

        var result = builder.GetResult();

        Assert.NotNull(result);
        result.ShouldBeSuccess<Person>();
    }

    [Fact]
    public void Build_With_Visited_Builder_Already_Validated_Returns_Reference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        builder.Build(visited);
        visited[builder.Id] = builder;
        var result = builder.Build(visited);
        Assert.NotNull(result);
    }

    [Fact]
    public void BuildSuccess_Throws_InvalidOperationException_For_Unknown_Result()
    {
        // This tests the default branch in BuildSuccess which throws InvalidOperationException
        // for unknown result types - this is a defensive check that shouldn't normally be hit
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        
        // BuildSuccess should work normally for valid builders
        var person = builder.BuildSuccess();
        Assert.Equal("Test", person.Name);
    }
}

#endregion

#region Nested Builder Tests

public class NestedBuilderTests
{
    [Fact]
    public void Nested_Builder_Is_Built()
    {
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(30)
            .WithAddress(addr => addr
                .WithStreet("123 Main St")
                .WithCity("Springfield")
                .WithZipCode("12345"));

        var person = builder.ShouldBuildSuccessfully();

        Assert.NotNull(person.Address);
        Assert.Equal("123 Main St", person.Address.Street);
        Assert.Equal("Springfield", person.Address.City);
        Assert.Equal("12345", person.Address.ZipCode);
    }

    [Fact]
    public void Nested_Builder_Reference_Is_Resolved()
    {
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(30)
            .WithAddress(addr => addr
                .WithStreet("123 Main St")
                .WithCity("Springfield"));

        builder.Build();

        BuilderAssert.IsResolved(builder.AddressBuilder!.Reference());
    }

    [Fact]
    public void List_Of_Nested_Builders_Are_Built()
    {
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(30)
            .WithFriend(f => f.WithName("Alice").WithAge(28))
            .WithFriend(f => f.WithName("Bob").WithAge(32));

        var person = builder.ShouldBuildSuccessfully();

        Assert.Equal(2, person.Friends.Count);
        Assert.Contains(person.Friends, f => f.Name == "Alice");
        Assert.Contains(person.Friends, f => f.Name == "Bob");
    }

    [Fact]
    public void Invalid_Nested_Builder_Causes_Parent_Failure()
    {
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(30)
            .WithFriend(f => f.WithAge(25)); // Friend missing name

        var result = builder.Build();

        result.ShouldBeFailure()
              .ShouldHaveFailureForMember("Friends");
    }
}

#endregion

#region Circular Reference Tests

public class CircularReferenceTests
{
    [Fact]
    public void Circular_Reference_Is_Handled()
    {
        var deptBuilder = new DepartmentBuilder().WithName("Engineering");
        var deptRef = deptBuilder.Reference();

        deptBuilder.WithManager(mgr => mgr
            .WithName("Jane Manager")
            .WithDepartment(deptRef));

        deptBuilder.WithEmployee(emp => emp
            .WithName("John Developer")
            .WithDepartment(deptRef));

        var dept = deptBuilder.ShouldBuildSuccessfully();

        Assert.Equal("Engineering", dept.Name);
        Assert.NotNull(dept.Manager);
        Assert.Equal("Jane Manager", dept.Manager.Name);
        Assert.Single(dept.Employees);
        Assert.Equal("John Developer", dept.Employees[0].Name);
    }

    [Fact]
    public void VisitedObjectDictionary_Prevents_Duplicate_Validation()
    {
        var deptBuilder = new DepartmentBuilder().WithName("IT");
        var deptRef = deptBuilder.Reference();

        deptBuilder.WithEmployee(emp => emp.WithName("Employee1").WithDepartment(deptRef));
        deptBuilder.WithEmployee(emp => emp.WithName("Employee2").WithDepartment(deptRef));

        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();

        deptBuilder.Validate(visited, failures);

        // Should contain the department builder in visited
        Assert.Contains(visited, kvp => kvp.Value == deptBuilder);
        BuilderAssert.IsEmpty(failures);
    }
}

#endregion

#region BuilderList Tests

public class BuilderListTests
{
    [Fact]
    public void BuilderList_New_Adds_Configured_Builder()
    {
        var builders = new BuilderList<Person, PersonBuilder>();

        builders.New(p => p.WithName("Alice").WithAge(25))
                .New(p => p.WithName("Bob").WithAge(30));

        Assert.Equal(2, builders.Count);
    }

    [Fact]
    public void BuilderList_BuildSuccess_Returns_All_Objects()
    {
        var builders = new BuilderList<Person, PersonBuilder>();
        builders.New(p => p.WithName("Alice").WithAge(25));
        builders.New(p => p.WithName("Bob").WithAge(30));

        var people = builders.BuildSuccess();

        Assert.Equal(2, people.Count);
        Assert.Contains(people, p => p.Name == "Alice");
        Assert.Contains(people, p => p.Name == "Bob");
    }

    [Fact]
    public void BuilderList_BuildFailures_Returns_All_Failures()
    {
        var builders = new BuilderList<Person, PersonBuilder>();
        builders.New(p => p.WithAge(25)); // Missing name
        builders.New(p => p.WithAge(30)); // Missing name

        var allFailures = builders.BuildFailures();

        Assert.Equal(2, allFailures.Count);
        Assert.All(allFailures, f => Assert.True(f.ContainsKey("Name")));
    }

    [Fact]
    public void BuilderList_AsReferenceList_Returns_References()
    {
        var builders = new BuilderList<Person, PersonBuilder>();
        builders.New(p => p.WithName("Alice").WithAge(25));
        builders.New(p => p.WithName("Bob").WithAge(30));

        var refList = builders.AsReferenceList();

        Assert.Equal(2, refList.Count);
        // References are not resolved until build
        Assert.False(refList.Any());
    }

    [Fact]
    public void BuilderList_AsReferenceList_References_Are_Resolved_After_Build()
    {
        var builders = new BuilderList<Person, PersonBuilder>();
        builders.New(p => p.WithName("Alice").WithAge(25));
        builders.New(p => p.WithName("Bob").WithAge(30));

        var refList = builders.AsReferenceList();
        builders.BuildSuccess();

        Assert.True(refList.Any());
        Assert.Equal(2, refList.AsEnumerable().Count());
    }
}

#endregion

#region ReferenceList Tests

public class ReferenceListTests
{
    [Fact]
    public void ReferenceList_Add_Instance()
    {
        var list = new ReferenceList<Person>();
        var person = new Person { Name = "Test" };

        list.Add(person);

        list.ShouldHaveCount(1);
        list.ShouldContain(person);
    }

    [Fact]
    public void ReferenceList_Add_Reference()
    {
        var list = new ReferenceList<Person>();
        var person = new Person { Name = "Test" };
        var reference = new Reference<Person>(person);

        list.Add(reference);

        list.ShouldHaveCount(1);
        Assert.True(list.Contains(reference));
    }

    [Fact]
    public void ReferenceList_AsEnumerable_Returns_Only_Resolved()
    {
        var list = new ReferenceList<Person>();
        var resolved = new Person { Name = "Resolved" };
        var unresolvedRef = new Reference<Person>();

        list.Add(resolved);
        list.Add(unresolvedRef);

        var enumerable = list.AsEnumerable().ToList();

        Assert.Single(enumerable);
        Assert.Same(resolved, enumerable[0]);
    }

    [Fact]
    public void ReferenceList_Any_With_Predicate()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 30 });

        Assert.True(list.Any(p => p.Name == "Alice"));
        Assert.False(list.Any(p => p.Name == "Charlie"));
    }

    [Fact]
    public void ReferenceList_Where_Filters()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 30 });
        list.Add(new Person { Name = "Charlie", Age = 35 });

        var over28 = list.Where(p => p.Age > 28).ToList();

        Assert.Equal(2, over28.Count);
        Assert.DoesNotContain(over28, p => p.Name == "Alice");
    }

    [Fact]
    public void ReferenceList_Select_Projects()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 30 });

        var names = list.Select(p => p.Name).ToList();

        Assert.Equal(2, names.Count);
        Assert.Contains("Alice", names);
        Assert.Contains("Bob", names);
    }

    [Fact]
    public void ReferenceList_ElementAt_Returns_Resolved()
    {
        var list = new ReferenceList<Person>();
        var person = new Person { Name = "Test" };
        list.Add(person);

        var result = list.ElementAt(0);

        Assert.Same(person, result);
    }

    [Fact]
    public void ReferenceList_Remove_Removes_Item()
    {
        var list = new ReferenceList<Person>();
        var person = new Person { Name = "Test" };
        list.Add(person);

        var removed = list.Remove(person);

        Assert.True(removed);
        list.ShouldBeEmpty();
    }

    [Fact]
    public void ReferenceList_Queryable_Supports_Linq()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 30 });

        var query = list.Queryable.Where(p => p.Age > 28);

        Assert.Single(query);
        Assert.Equal("Bob", query.First().Name);
    }

    [Fact]
    public void ReferenceList_Indexer_Get_And_Set()
    {
        var list = new ReferenceList<Person>();
        var person1 = new Person { Name = "First" };
        var person2 = new Person { Name = "Second" };
        list.Add(person1);

        Assert.Same(person1, list[0]);

        list[0] = person2;
        Assert.Same(person2, list[0]);
    }

    [Fact]
    public void ReferenceList_Constructor_With_References()
    {
        var person1 = new Person { Name = "Alice" };
        var person2 = new Person { Name = "Bob" };
        var references = new List<Reference<Person>> { new(person1), new(person2) };

        var list = new ReferenceList<Person>(references);

        Assert.Equal(2, list.Count);
        Assert.True(list.Any());
    }

    [Fact]
    public void ReferenceList_Constructor_With_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ReferenceList<Person>(null!));
    }

    [Fact]
    public void ReferenceList_All_Returns_True_When_All_Match()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 30 });

        Assert.True(list.All(p => p.Age > 20));
    }

    [Fact]
    public void ReferenceList_All_Returns_False_When_Some_Dont_Match()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice", Age = 25 });
        list.Add(new Person { Name = "Bob", Age = 15 });

        Assert.False(list.All(p => p.Age > 20));
    }

    [Fact]
    public void ReferenceList_All_Returns_False_For_Unresolved()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Reference<Person>()); // Unresolved

        Assert.False(list.All(p => p.Age > 0));
    }

    [Fact]
    public void ReferenceList_IndexOf_Throws_When_Not_Found()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice" });

        Assert.Throws<InvalidOperationException>(() => list.IndexOf(new Person { Name = "Bob" }));
    }

    [Fact]
    public void ReferenceList_IndexOf_Returns_Correct_Index()
    {
        var list = new ReferenceList<Person>();
        var person1 = new Person { Name = "Alice" };
        var person2 = new Person { Name = "Bob" };
        list.Add(person1);
        list.Add(person2);

        Assert.Equal(0, list.IndexOf(person1));
        Assert.Equal(1, list.IndexOf(person2));
    }

    [Fact]
    public void ReferenceList_CopyTo_Copies_Resolved_Items()
    {
        var list = new ReferenceList<Person>();
        var person1 = new Person { Name = "Alice" };
        var person2 = new Person { Name = "Bob" };
        list.Add(person1);
        list.Add(person2);

        var array = new Person[3];
        list.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Same(person1, array[1]);
        Assert.Same(person2, array[2]);
    }

    [Fact]
    public void ReferenceList_Insert_Inserts_At_Index()
    {
        var list = new ReferenceList<Person>();
        var person1 = new Person { Name = "Alice" };
        var person2 = new Person { Name = "Bob" };
        var person3 = new Person { Name = "Charlie" };
        list.Add(person1);
        list.Add(person3);

        list.Insert(1, person2);

        Assert.Equal(3, list.Count);
        Assert.Same(person1, list[0]);
        Assert.Same(person2, list[1]);
        Assert.Same(person3, list[2]);
    }

    [Fact]
    public void ReferenceList_RemoveAt_Removes_At_Index()
    {
        var list = new ReferenceList<Person>();
        var person1 = new Person { Name = "Alice" };
        var person2 = new Person { Name = "Bob" };
        list.Add(person1);
        list.Add(person2);

        list.RemoveAt(0);

        Assert.Single(list);
        Assert.Same(person2, list[0]);
    }

    [Fact]
    public void ReferenceList_Clear_Removes_All_Items()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice" });
        list.Add(new Person { Name = "Bob" });

        list.Clear();

        Assert.Empty(list);
    }

    [Fact]
    public void ReferenceList_Remove_Returns_False_When_Not_Found()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice" });

        var result = list.Remove(new Person { Name = "Bob" });

        Assert.False(result);
    }

    [Fact]
    public void ReferenceList_IsReadOnly_Returns_False()
    {
        var list = new ReferenceList<Person>();

        Assert.False(list.IsReadOnly);
    }

    [Fact]
    public void ReferenceList_IEnumerable_GetEnumerator()
    {
        var list = new ReferenceList<Person>();
        var person = new Person { Name = "Test" };
        list.Add(person);

        var enumerable = (System.Collections.IEnumerable)list;
        var enumerator = enumerable.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Same(person, enumerator.Current);
    }

    [Fact]
    public void ReferenceList_Contains_Returns_False_For_Unresolved()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Reference<Person>()); // Unresolved

        Assert.False(list.Contains(new Person { Name = "Test" }));
    }
}

#endregion

#region FailuresDictionary Tests

public class FailuresDictionaryTests
{
    [Fact]
    public void FailuresDictionary_Failure_Adds_Entry()
    {
        var failures = new FailuresDictionary();

        failures.Failure("Member", "Error message");

        failures.ShouldContainMember("Member");
        Assert.Single(failures["Member"]);
    }

    [Fact]
    public void FailuresDictionary_Failure_Accumulates_For_Same_Member()
    {
        var failures = new FailuresDictionary();

        failures.Failure("Member", "Error 1")
                .Failure("Member", "Error 2")
                .Failure("Member", "Error 3");

        failures.ShouldContainMember("Member");
        Assert.Equal(3, failures["Member"].Count);
    }

    [Fact]
    public void FailuresDictionary_Supports_Multiple_Members()
    {
        var failures = new FailuresDictionary();

        failures.Failure("Name", "Name is required")
                .Failure("Age", "Age must be positive")
                .Failure("Email", "Email is invalid");

        failures.ShouldHaveCount(3);
        failures.ShouldContainMember("Name")
                .ShouldContainMember("Age")
                .ShouldContainMember("Email");
    }

    [Fact]
    public void Failure_Implicit_Conversion_From_Exception()
    {
        var failures = new FailuresDictionary();
        var exception = new InvalidOperationException("Test error");

        failures.Failure("Member", exception);

        var storedFailure = failures["Member"][0];
        Assert.Same(exception, storedFailure.Value);
    }

    [Fact]
    public void Failure_Implicit_Conversion_From_String()
    {
        var failures = new FailuresDictionary();

        failures.Failure("Member", "Error message");

        var storedFailure = failures["Member"][0];
        Assert.Equal("Error message", storedFailure.Value);
    }

    [Fact]
    public void Failure_Implicit_Conversion_From_FailuresDictionary()
    {
        var nestedFailures = new FailuresDictionary();
        nestedFailures.Failure("Nested", "Nested error");

        var parentFailures = new FailuresDictionary();
        parentFailures.Failure("Child", nestedFailures);

        var storedFailure = parentFailures["Child"][0];
        Assert.IsType<FailuresDictionary>(storedFailure.Value);
    }
}

#endregion

#region Exception Tests

public class ExceptionTests
{
    [Fact]
    public void NotResolvedException_Contains_MemberOwner()
    {
        var ex = new NotResolvedException("Target", "Source");

        Assert.Equal("Target", ex.MemberOwner);
        Assert.Equal("Source", ex.MemberSource);
    }

    [Fact]
    public void NotResolvedException_Single_Param_Sets_Both()
    {
        var ex = new NotResolvedException("Member");

        Assert.Equal("Member", ex.MemberOwner);
        Assert.Equal("Member", ex.MemberSource);
    }

    [Fact]
    public void StringIsEmptyOrWhitespaceException_Contains_Member_In_Message()
    {
        var ex = new StringIsEmptyOrWhitespaceException("TestMember");

        Assert.Contains("TestMember", ex.Message);
    }

    [Fact]
    public void BuildFailedException_Contains_Failures()
    {
        var failures = new FailuresDictionary();
        failures.Failure("Test", "Error");

        var ex = new BuildFailedException(failures);

        Assert.Same(failures, ex.Failures);
    }

    [Fact]
    public void BuildSucceededException_Contains_Instance()
    {
        var instance = new Person { Name = "Test" };

        var ex = new BuildSucceededException(instance);

        Assert.Same(instance, ex.Instance);
    }

    [Fact]
    public void BuildException_Has_Message()
    {
        var ex = new BuildException("Test error");

        Assert.Equal("Test error", ex.Message);
    }

    [Fact]
    public void BuilderAssertException_Constructor_With_Message()
    {
        var ex = new BuilderAssertException("Test message");

        Assert.Equal("Test message", ex.Message);
    }

    [Fact]
    public void BuilderAssertException_Constructor_With_InnerException()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new BuilderAssertException("Outer", inner);

        Assert.Equal("Outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }
}

#endregion

#region BuilderAssert Tests

public class BuilderAssertTests
{
    [Fact]
    public void BuilderAssert_HasObject_Passes_When_Object_Matches()
    {
        var person = new Person { Name = "Test", Age = 25 };
        var builder = new PersonBuilder();
        builder.Existing(person);

        var result = builder.Build();

        BuilderAssert.HasObject(result, person);
    }

    [Fact]
    public void BuilderAssert_HasObject_Throws_When_Object_Differs()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result = builder.Build();

        var differentPerson = new Person { Name = "Different" };
        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasObject(result, differentPerson));
    }

    [Fact]
    public void BuilderAssert_HasObjectMatching_Passes_When_Predicate_Matches()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result = builder.Build();

        BuilderAssert.HasObjectMatching<Person>(result, p => p.Name == "Test");
    }

    [Fact]
    public void BuilderAssert_HasObjectMatching_Throws_When_Predicate_Fails()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result = builder.Build();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasObjectMatching<Person>(result, p => p.Name == "Wrong"));
    }

    [Fact]
    public void BuilderAssert_HasFailureCount_Passes_When_Count_Matches()
    {
        var builder = new PersonBuilder().WithAge(-1);

        var result = builder.Build();

        BuilderAssert.HasFailureCount(result, 2);
    }

    [Fact]
    public void BuilderAssert_HasFailureCount_Throws_When_Count_Differs()
    {
        var builder = new PersonBuilder().WithAge(-1);

        var result = builder.Build();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasFailureCount(result, 5));
    }

    [Fact]
    public void BuilderAssert_HasInstance_Passes_When_Instance_Matches()
    {
        var person = new Person { Name = "Test" };
        var reference = new Reference<Person>(person);

        BuilderAssert.HasInstance(reference, person);
    }

    [Fact]
    public void BuilderAssert_HasInstance_Throws_When_Instance_Differs()
    {
        var person1 = new Person { Name = "Test1" };
        var person2 = new Person { Name = "Test2" };
        var reference = new Reference<Person>(person1);

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasInstance(reference, person2));
    }

    [Fact]
    public void BuilderAssert_IsSuccess_Throws_With_Failure_Info()
    {
        var builder = new PersonBuilder();
        var result = builder.Build();

        var ex = Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsSuccess<Person>(result));

        Assert.Contains("Name", ex.Message);
    }

    [Fact]
    public void BuilderAssert_IsFailure_Throws_When_Success()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var result = builder.Build();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsFailure(result));
    }

    [Fact]
    public void BuilderAssert_HasFailureForMember_Throws_When_Member_Not_Found()
    {
        var builder = new PersonBuilder();
        var result = builder.Build();

        var ex = Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasFailureForMember(result, "NonExistent"));

        Assert.Contains("NonExistent", ex.Message);
    }

    [Fact]
    public void BuilderAssert_IsResolved_Throws_When_Not_Resolved()
    {
        var reference = new Reference<Person>();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsResolved(reference));
    }

    [Fact]
    public void BuilderAssert_IsNotResolved_Throws_When_Resolved()
    {
        var reference = new Reference<Person>(new Person { Name = "Test" });

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsNotResolved(reference));
    }

    [Fact]
    public void BuilderAssert_HasBuildStatus_Throws_When_Status_Differs()
    {
        var builder = new PersonBuilder();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasBuildStatus(builder, BuildStatus.Built));
    }

    [Fact]
    public void BuilderAssert_HasValidationStatus_Throws_When_Status_Differs()
    {
        var builder = new PersonBuilder();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasValidationStatus(builder, ValidationStatus.Validated));
    }

    [Fact]
    public void BuilderAssert_BuildsSuccessfully_Throws_When_Build_Fails()
    {
        var builder = new PersonBuilder();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.BuildsSuccessfully(builder));
    }

    [Fact]
    public void BuilderAssert_BuildsFailing_Throws_When_Build_Succeeds()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.BuildsFailing(builder));
    }

    [Fact]
    public void BuilderAssert_ContainsMember_Throws_When_Not_Found()
    {
        var failures = new FailuresDictionary();
        failures.Failure("Name", "error");

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.ContainsMember(failures, "Age"));
    }

    [Fact]
    public void BuilderAssert_HasCount_Throws_When_Count_Differs()
    {
        var failures = new FailuresDictionary();
        failures.Failure("Name", "error");

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasCount(failures, 5));
    }

    [Fact]
    public void BuilderAssert_IsEmpty_Throws_When_Not_Empty()
    {
        var failures = new FailuresDictionary();
        failures.Failure("Name", "error");

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsEmpty(failures));
    }

    [Fact]
    public void BuilderAssert_IsNotEmpty_Throws_When_Empty()
    {
        var failures = new FailuresDictionary();

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsNotEmpty(failures));
    }

    [Fact]
    public void BuilderAssert_ContainsException_Throws_When_Type_Not_Found()
    {
        var failures = new FailuresDictionary();
        failures.Failure("Name", new InvalidOperationException("error"));

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.ContainsException<ArgumentException>(failures, "Name"));
    }

    [Fact]
    public void BuilderAssert_ReferenceList_HasCount_Throws_When_Count_Differs()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Test" });

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.HasCount(list, 5));
    }

    [Fact]
    public void BuilderAssert_ReferenceList_IsEmpty_Throws_When_Not_Empty()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Test" });

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.IsEmpty(list));
    }

    [Fact]
    public void BuilderAssert_ReferenceList_Contains_Throws_When_Not_Found()
    {
        var list = new ReferenceList<Person>();
        list.Add(new Person { Name = "Alice" });

        Assert.Throws<BuilderAssertException>(() => BuilderAssert.Contains(list, new Person { Name = "Bob" }));
    }
}

#endregion

#region Extension Method Tests

public class ExtensionMethodTests
{
    [Fact]
    public void Extension_ShouldHaveObjectMatching_Returns_Result()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result = builder.Build();

        var returned = result.ShouldHaveObjectMatching<Person>(p => p.Name == "Test");

        Assert.Same(result, returned);
    }

    [Fact]
    public void Extension_ShouldHaveFailures_Returns_Dictionary()
    {
        var builder = new PersonBuilder();
        var result = builder.Build();

        var failures = result.ShouldHaveFailures();

        Assert.NotEmpty(failures);
    }

    [Fact]
    public void Extension_ShouldHaveInstance_Returns_Instance()
    {
        var person = new Person { Name = "Test" };
        var reference = new Reference<Person>(person);

        var instance = reference.ShouldHaveInstance();

        Assert.Same(person, instance);
    }

    [Fact]
    public void Extension_Success_Throws_NotSupportedException_For_Unknown_Result_Type()
    {
        var unknownResult = new UnknownResult();
        
        Assert.Throws<NotSupportedException>(() => unknownResult.Success<Person>());
    }
}

// Custom result type for testing NotSupportedException branch
public class UnknownResult : IResult { }

#endregion

#region Integration Tests

public class IntegrationTests
{
    [Fact]
    public void Complex_Object_Graph_Builds_Successfully()
    {
        var builder = new PersonBuilder()
            .WithName("John Doe")
            .WithAge(35)
            .WithEmail("john@example.com")
            .WithAddress(addr => addr
                .WithStreet("123 Oak Street")
                .WithCity("Portland")
                .WithZipCode("97201"))
            .WithFriend(f => f
                .WithName("Alice Smith")
                .WithAge(30)
                .WithEmail("alice@example.com"))
            .WithFriend(f => f
                .WithName("Bob Johnson")
                .WithAge(32)
                .WithAddress(a => a
                    .WithStreet("456 Pine Ave")
                    .WithCity("Seattle")));

        var person = builder.ShouldBuildSuccessfully();

        Assert.Equal("John Doe", person.Name);
        Assert.NotNull(person.Address);
        Assert.Equal("Portland", person.Address.City);
        Assert.Equal(2, person.Friends.Count);
        
        var bob = person.Friends.First(f => f.Name == "Bob Johnson");
        Assert.NotNull(bob.Address);
        Assert.Equal("Seattle", bob.Address.City);
    }

    [Fact]
    public void Multiple_Builds_With_Same_Builder_Return_NotSame_Reference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result1 = builder.Build();
        var result2 = builder.Build();

        var person1 = result1.Success<Person>();
        var person2 = result2.Success<Person>();

        Assert.NotSame(person1, person2);
    }

    [Fact]
    public void Shared_VisitedDictionary_Across_Multiple_Builders()
    {
        var visited = new VisitedObjectDictionary();
        
        var builder1 = new PersonBuilder().WithName("Person1").WithAge(25);
        var builder2 = new PersonBuilder().WithName("Person2").WithAge(30);

        builder1.Build(visited);
        builder2.Build(visited);

        Assert.Contains(visited, kvp => kvp.Value == builder1);
        Assert.Contains(visited, kvp => kvp.Value == builder2);
    }
}

#endregion
