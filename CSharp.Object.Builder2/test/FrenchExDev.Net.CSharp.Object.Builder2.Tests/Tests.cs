using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

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

public class StringIsEmptyOrWhitespaceException : Exception
{
    public StringIsEmptyOrWhitespaceException()
    {
    }

    public StringIsEmptyOrWhitespaceException(string? message) : base(message)
    {
    }
}

/// <summary>
/// Simple builder for Person with validation.
/// </summary>
public class PersonBuilder : AbstractBuilder<Person>
{
    public PersonBuilder()
    {
    }

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


public class Tests
{
    #region Basic Build Tests

    [Fact]
    public void Build_SimpleObject_ShouldCreateInstance()
    {
        // Arrange
        var builder = new SimpleObjectBuilder()
            .WithValue("test");

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeSameAs("test");
    }

    [Fact]
    public void Build_PersonWithAllProperties_ShouldCreateCompleteInstance()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("John Doe")
            .WithAge(30)
            .WithEmail("john@example.com")
            .WithAddress(a => a
                .WithStreet("123 Main St")
                .WithCity("Paris")
                .WithZipCode("75001"));

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("John Doe");
        result.Age.ShouldBe(30);
        result.Email.ShouldBe("john@example.com");
        result.Address.ShouldNotBeNull();
        result.Address.Street.ShouldBe("123 Main St");
        result.Address.City.ShouldBe("Paris");
        result.Address.ZipCode.ShouldBe("75001");
    }

    [Fact]
    public void Build_PersonWithFriends_ShouldCreateNestedList()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("Alice")
            .WithAge(25)
            .WithFriend(f => f.WithName("Bob").WithAge(26))
            .WithFriend(f => f.WithName("Charlie").WithAge(27));

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Friends.Count.ShouldBe(2);
        result.Friends[0].Name.ShouldBe("Bob");
        result.Friends[1].Name.ShouldBe("Charlie");
    }

    #endregion

    #region Validation Tests


    #endregion

    #region Reference Tests

    [Fact]
    public void Reference_ShouldReturnSameInstanceAfterBuild()
    {
        // Arrange
        var builder = new SimpleObjectBuilder()
            .WithValue("test");

        // Act
        var reference = builder.Reference();
        builder.Build().Value.Resolved();

        // Assert
        reference.ResolvedOrNull().ShouldNotBeNull();
        reference.ResolvedOrNull()?.Value.ShouldBe("test");
    }

    [Fact]
    public void Reference_BeforeBuild_ShouldReturnNull()
    {
        // Arrange
        var builder = new SimpleObjectBuilder()
            .WithValue("test");

        // Act
        var reference = builder.Reference();

        // Assert
        reference.ResolvedOrNull().ShouldBeNull();
    }

    #endregion

    #region Circular Reference Tests

    [Fact]
    public void Build_DepartmentWithEmployees_ShouldHandleCircularReferences()
    {
        // Arrange
        var departmentBuilder = new DepartmentBuilder()
            .WithName("Engineering");

        departmentBuilder
            .WithManager(m => m
                .WithName("Alice")
                .WithDepartment(departmentBuilder.Reference()))
            .WithEmployee(e => e
                .WithName("Bob")
                .WithDepartment(departmentBuilder.Reference()))
            .WithEmployee(e => e
                .WithName("Charlie")
                .WithDepartment(departmentBuilder.Reference()));

        // Act
        var result = departmentBuilder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Engineering");
        result.Manager.ShouldNotBeNull();
        result.Manager.Name.ShouldBe("Alice");
        // Note: The department reference is resolved after build, so we check via the reference
        departmentBuilder.Reference().IsResolved.ShouldBeTrue();
        result.Employees.Count.ShouldBe(2);
    }

    #endregion

    #region BuilderList Tests

    [Fact]
    public void BuilderList_New_ShouldAddConfiguredItem()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("Parent")
            .WithAge(40);

        // Act
        builder.Friends.New(f => f.WithName("Friend1").WithAge(20));
        builder.Friends.New(f => f.WithName("Friend2").WithAge(21));
        var result = builder.Build().Value.Resolved();

        // Assert
        result.Friends.Count.ShouldBe(2);
    }

    [Fact]
    public void BuilderList_AsReferenceList_ShouldReturnReferences()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("Parent")
            .WithAge(40)
            .WithFriend(f => f.WithName("Friend1").WithAge(20));

        // Act
        builder.Build().Value.Resolved();
        var referenceList = builder.Friends.AsReferenceList();

        // Assert
        referenceList.AsEnumerable().Count().ShouldBe(1);
    }

    #endregion

    #region TaggedItem Validation Tests


    [Fact]
    public void Build_TaggedItemWithValidTags_ShouldSucceed()
    {
        // Arrange
        var builder = new TaggedItemBuilder()
            .WithTags(["tag1", "tag2", "tag3"]);

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Tags.Count.ShouldBe(3);
    }

    #endregion

    #region Multiple Validation Failures Tests

    #endregion

    #region Edge Cases

    [Fact]
    public void Build_PersonWithNullAddress_ShouldSucceed()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(25);

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Address.ShouldBeNull();
    }

    [Fact]
    public void Build_PersonWithEmptyFriendsList_ShouldSucceed()
    {
        // Arrange
        var builder = new PersonBuilder()
            .WithName("John")
            .WithAge(25);

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Friends.ShouldBeEmpty();
    }

    [Fact]
    public void Build_AddressWithNullZipCode_ShouldSucceed()
    {
        // Arrange
        var builder = new AddressBuilder()
            .WithStreet("123 Main St")
            .WithCity("Paris");

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.ZipCode.ShouldBeNull();
    }

    [Fact]
    public void Build_DepartmentWithNoManager_ShouldSucceed()
    {
        // Arrange
        var builder = new DepartmentBuilder()
            .WithName("Sales")
            .WithEmployee(e => e.WithName("John"));

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Manager.ShouldBeNull();
        result.Employees.Count.ShouldBe(1);
    }

    [Fact]
    public void Build_DepartmentWithNoEmployees_ShouldSucceed()
    {
        // Arrange
        var builder = new DepartmentBuilder()
            .WithName("Empty Dept");

        // Act
        var result = builder.Build().Value.Resolved();

        // Assert
        result.ShouldNotBeNull();
        result.Employees.ShouldBeEmpty();
    }

    #endregion

    #region Fluent API Tests

    [Fact]
    public void FluentApi_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new PersonBuilder();

        // Act
        var result = builder
            .WithName("Test")
            .WithAge(25)
            .WithEmail("test@test.com");

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void FluentApi_AddressBuilder_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new AddressBuilder();

        // Act
        var result = builder
            .WithStreet("Street")
            .WithCity("City")
            .WithZipCode("12345");

        // Assert
        result.ShouldBeSameAs(builder);
    }

    #endregion
}
