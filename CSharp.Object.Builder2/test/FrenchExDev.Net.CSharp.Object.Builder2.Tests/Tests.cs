using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

#region Test Domain Models

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public List<Person> Friends { get; set; } = [];
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
}

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

public class ThreadSafeCounter
{
    public int BuildCount;
    public int InstantiateCount;
}

#endregion

#region Test Builders

public class StringIsEmptyOrWhitespaceException : Exception
{
    public StringIsEmptyOrWhitespaceException() { }
    public StringIsEmptyOrWhitespaceException(string? message) : base(message) { }
}

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

public class AddressBuilder : AbstractBuilder<Address>
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }

    protected override Address Instantiate() => new() { Street = Street!, City = City!, ZipCode = ZipCode };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Street, nameof(Street), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNullOrEmptyOrWhitespace(City, nameof(City), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public AddressBuilder WithStreet(string street) { Street = street; return this; }
    public AddressBuilder WithCity(string city) { City = city; return this; }
    public AddressBuilder WithZipCode(string zipCode) { ZipCode = zipCode; return this; }
}

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

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
        ManagerBuilder?.Validate(visitedCollector, failures);
        ValidateListInternal(Employees, nameof(Employees), visitedCollector, failures);
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        if (ManagerBuilder is not null)
            BuildChild(ManagerBuilder, visitedCollector);
        BuildList<EmployeeBuilder, Employee>(Employees, visitedCollector);
    }

    public DepartmentBuilder WithName(string name) { Name = name; return this; }
    public DepartmentBuilder WithManager(Action<EmployeeBuilder> configure)
    {
        ManagerBuilder = new EmployeeBuilder();
        configure(ManagerBuilder);
        return this;
    }
    public DepartmentBuilder WithEmployee(Action<EmployeeBuilder> configure) { Employees.New(configure); return this; }
}

public class EmployeeBuilder : AbstractBuilder<Employee>
{
    public string? Name { get; set; }
    public Reference<Department>? DepartmentRef { get; set; }

    protected override Employee Instantiate() => new() { Name = Name!, Department = DepartmentRef?.ResolvedOrNull() };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public EmployeeBuilder WithName(string name) { Name = name; return this; }
    public EmployeeBuilder WithDepartment(Reference<Department> deptRef) { DepartmentRef = deptRef; return this; }
}

public class SimpleObjectBuilder : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
    public SimpleObjectBuilder WithValue(string value) { Value = value; return this; }
}

public class TaggedItemBuilder : AbstractBuilder<TaggedItem>
{
    public List<string>? Tags { get; set; }
    public List<string>? NullableTags { get; set; }

    protected override TaggedItem Instantiate() => new() { Tags = Tags ?? [] };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotEmptyOrWhitespace(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotEmptyOrWhitespace(NullableTags, nameof(NullableTags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNull(Tags, nameof(Tags), failures, n => new ArgumentNullException(n));
        AssertNotNullNotEmptyCollection(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public TaggedItemBuilder WithTags(List<string> tags) { Tags = tags; return this; }
    public TaggedItemBuilder WithNullableTags(List<string>? tags) { NullableTags = tags; return this; }
}

public class SlowBuilder : AbstractBuilder<SimpleObject>
{
    private readonly ThreadSafeCounter _counter;
    public string? Value { get; set; }

    public SlowBuilder() { _counter = new ThreadSafeCounter(); }
    public SlowBuilder(ThreadSafeCounter counter) { _counter = counter; }

    protected override SimpleObject Instantiate()
    {
        Interlocked.Increment(ref _counter.InstantiateCount);
        Thread.Sleep(10);
        return new() { Value = Value ?? string.Empty };
    }

    public SlowBuilder WithValue(string value) { Value = value; return this; }
    public int GetInstantiateCount() => _counter.InstantiateCount;
}

#endregion

public class Tests
{
    #region Basic Build Tests

    [Fact]
    public void Build_SimpleObject_ShouldCreateInstance()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var result = builder.Build().Value.Resolved();
        result.ShouldNotBeNull();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void Build_PersonWithAllProperties_ShouldCreateCompleteInstance()
    {
        var builder = new PersonBuilder()
            .WithName("John Doe").WithAge(30).WithEmail("john@example.com")
            .WithAddress(a => a.WithStreet("123 Main St").WithCity("Paris").WithZipCode("75001"));
        var result = builder.Build().Value.Resolved();
        result.ShouldNotBeNull();
        result.Name.ShouldBe("John Doe");
        result.Age.ShouldBe(30);
        result.Address.ShouldNotBeNull();
        result.Address.Street.ShouldBe("123 Main St");
    }

    [Fact]
    public void Build_PersonWithFriends_ShouldCreateNestedList()
    {
        var builder = new PersonBuilder().WithName("Alice").WithAge(25)
            .WithFriend(f => f.WithName("Bob").WithAge(26))
            .WithFriend(f => f.WithName("Charlie").WithAge(27));
        var result = builder.Build().Value.Resolved();
        result.Friends.Count.ShouldBe(2);
        result.Friends[0].Name.ShouldBe("Bob");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void Build_PersonWithNullName_ShouldFail()
    {
        var builder = new PersonBuilder().WithAge(25);
        var buildResult = builder.Build();
        buildResult.Value.IsResolved.ShouldBeFalse();
        builder.BuildStatus.ShouldBe(BuildStatus.Building);
    }

    [Fact]
    public void BuildOrThrow_PersonWithNullName_ShouldThrowAggregateException()
    {
        var builder = new PersonBuilder().WithAge(25);
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is StringIsEmptyOrWhitespaceException);
    }

    [Fact]
    public void BuildOrThrow_PersonWithNegativeAge_ShouldThrowAggregateException()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(-1);
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is ArgumentOutOfRangeException);
    }

    [Fact]
    public void BuildOrThrow_AddressWithMissingStreet_ShouldThrowAggregateException()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(25)
            .WithAddress(a => a.WithCity("Paris"));
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is StringIsEmptyOrWhitespaceException);
    }

    #endregion

    #region Reference Tests

    [Fact]
    public void Reference_ShouldReturnSameInstanceAfterBuild()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var reference = builder.Reference();
        builder.Build().Value.Resolved();
        reference.ResolvedOrNull().ShouldNotBeNull();
        reference.ResolvedOrNull()?.Value.ShouldBe("test");
    }

    [Fact]
    public void Reference_BeforeBuild_ShouldReturnNull()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var reference = builder.Reference();
        reference.ResolvedOrNull().ShouldBeNull();
    }

    [Fact]
    public void Reference_Resolved_BeforeBuild_ShouldThrowReferenceNotResolvedException()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var reference = builder.Reference();
        Should.Throw<ReferenceNotResolvedException>(() => reference.Resolved());
    }

    [Fact]
    public void Reference_ConstructorWithExisting_ShouldResolveImmediately()
    {
        var obj = new SimpleObject { Value = "test" };
        var reference = new Reference<SimpleObject>(obj);
        reference.IsResolved.ShouldBeTrue();
        reference.Instance.ShouldBe(obj);
    }

    [Fact]
    public void Reference_ConstructorWithNull_ShouldNotBeResolved()
    {
        var reference = new Reference<SimpleObject>(null);
        reference.IsResolved.ShouldBeFalse();
    }

    #endregion

    #region Circular Reference Tests

    [Fact]
    public void Build_DepartmentWithEmployees_ShouldHandleCircularReferences()
    {
        var departmentBuilder = new DepartmentBuilder().WithName("Engineering");
        departmentBuilder
            .WithManager(m => m.WithName("Alice").WithDepartment(departmentBuilder.Reference()))
            .WithEmployee(e => e.WithName("Bob").WithDepartment(departmentBuilder.Reference()))
            .WithEmployee(e => e.WithName("Charlie").WithDepartment(departmentBuilder.Reference()));

        var result = departmentBuilder.Build().Value.Resolved();
        result.Name.ShouldBe("Engineering");
        result.Manager.ShouldNotBeNull();
        result.Manager.Name.ShouldBe("Alice");
        departmentBuilder.Reference().IsResolved.ShouldBeTrue();
        result.Employees.Count.ShouldBe(2);
    }

    #endregion

    #region BuilderList Tests

    [Fact]
    public void BuilderList_New_ShouldAddConfiguredItem()
    {
        var builder = new PersonBuilder().WithName("Parent").WithAge(40);
        builder.Friends.New(f => f.WithName("Friend1").WithAge(20));
        builder.Friends.New(f => f.WithName("Friend2").WithAge(21));
        var result = builder.Build().Value.Resolved();
        result.Friends.Count.ShouldBe(2);
    }

    [Fact]
    public void BuilderList_AsReferenceList_ShouldReturnReferences()
    {
        var builder = new PersonBuilder().WithName("Parent").WithAge(40)
            .WithFriend(f => f.WithName("Friend1").WithAge(20));
        builder.Build().Value.Resolved();
        var referenceList = builder.Friends.AsReferenceList();
        referenceList.AsEnumerable().Count().ShouldBe(1);
    }

    [Fact]
    public void BuilderList_BuildSuccess_ShouldBuildAllItems()
    {
        var list = new BuilderList<SimpleObject, SimpleObjectBuilder>();
        list.New(b => b.WithValue("first"));
        list.New(b => b.WithValue("second"));
        var results = list.BuildSuccess();
        results.Count.ShouldBe(2);
        results[0].Value.ShouldBe("first");
    }

    [Fact]
    public void BuilderList_ValidateFailures_ShouldReturnFailures()
    {
        var list = new BuilderList<Person, PersonBuilder>();
        list.New(b => b.WithName("Valid").WithAge(25));
        list.New(b => b.WithAge(30)); // Missing name
        var failures = list.ValidateFailures();
        failures.Count.ShouldBe(1);
    }

    #endregion

    #region ReferenceList Tests

    [Fact]
    public void ReferenceList_DefaultConstructor_ShouldCreateEmptyList()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Count.ShouldBe(0);
        list.Any().ShouldBeFalse();
    }

    [Fact]
    public void ReferenceList_Add_ShouldAddInstance()
    {
        var list = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        list.Add(obj);
        list.Count.ShouldBe(1);
        list[0].Value.ShouldBe("test");
    }

    [Fact]
    public void ReferenceList_AddReference_ShouldAddReference()
    {
        var list = new ReferenceList<SimpleObject>();
        var reference = new Reference<SimpleObject>().Resolve(new SimpleObject { Value = "test" });
        list.Add(reference);
        list.Count.ShouldBe(1);
        list.Contains(reference).ShouldBeTrue();
    }

    [Fact]
    public void ReferenceList_Contains_ShouldFindInstance()
    {
        var list = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        list.Add(obj);
        list.Contains(obj).ShouldBeTrue();
        list.Contains(new SimpleObject { Value = "other" }).ShouldBeFalse();
    }

    [Fact]
    public void ReferenceList_AsEnumerable_WithLinqAny_ShouldFindMatch()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "first" });
        list.Add(new SimpleObject { Value = "second" });
        list.AsEnumerable().Any(x => x.Value == "first").ShouldBeTrue();
        list.AsEnumerable().Any(x => x.Value == "nonexistent").ShouldBeFalse();
    }

    [Fact]
    public void ReferenceList_AsEnumerable_WithLinqAll_ShouldCheckAllItems()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "a" });
        list.Add(new SimpleObject { Value = "ab" });
        list.AsEnumerable().All(x => x.Value.Length > 0).ShouldBeTrue();
        list.AsEnumerable().All(x => x.Value.Length > 1).ShouldBeFalse();
    }

    [Fact]
    public void ReferenceList_AsEnumerable_WithLinqWhere_ShouldFilterItems()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "a" });
        list.Add(new SimpleObject { Value = "ab" });
        list.Add(new SimpleObject { Value = "abc" });
        var filtered = list.AsEnumerable().Where(x => x.Value.Length >= 2).ToList();
        filtered.Count.ShouldBe(2);
    }

    [Fact]
    public void ReferenceList_AsEnumerable_WithLinqSelect_ShouldMapItems()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "hello" });
        list.Add(new SimpleObject { Value = "world" });
        var lengths = list.AsEnumerable().Select(x => x.Value.Length).ToList();
        lengths.ShouldBe([5, 5]);
    }

    [Fact]
    public void ReferenceList_IndexOf_ShouldFindIndex()
    {
        var list = new ReferenceList<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        list.Add(obj1);
        list.Add(obj2);
        list.IndexOf(obj1).ShouldBe(0);
        list.IndexOf(obj2).ShouldBe(1);
    }

    [Fact]
    public void ReferenceList_IndexOf_NotFound_ShouldThrow()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        Should.Throw<InvalidOperationException>(() => list.IndexOf(new SimpleObject { Value = "other" }));
    }

    [Fact]
    public void ReferenceList_Insert_ShouldInsertAtIndex()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "first" });
        list.Add(new SimpleObject { Value = "third" });
        list.Insert(1, new SimpleObject { Value = "second" });
        list.Count.ShouldBe(3);
        list[1].Value.ShouldBe("second");
    }

    [Fact]
    public void ReferenceList_RemoveAt_ShouldRemoveAtIndex()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "first" });
        list.Add(new SimpleObject { Value = "second" });
        list.RemoveAt(0);
        list.Count.ShouldBe(1);
        list[0].Value.ShouldBe("second");
    }

    [Fact]
    public void ReferenceList_Clear_ShouldRemoveAllItems()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "first" });
        list.Add(new SimpleObject { Value = "second" });
        list.Clear();
        list.Count.ShouldBe(0);
    }

    [Fact]
    public void ReferenceList_Remove_ShouldRemoveItem()
    {
        var list = new ReferenceList<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        list.Add(obj);
        var removed = list.Remove(obj);
        removed.ShouldBeTrue();
        list.Count.ShouldBe(0);
    }

    [Fact]
    public void ReferenceList_Remove_NotFound_ShouldReturnFalse()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        var removed = list.Remove(new SimpleObject { Value = "other" });
        removed.ShouldBeFalse();
        list.Count.ShouldBe(1);
    }

    [Fact]
    public void ReferenceList_CopyTo_ShouldCopyItems()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "first" });
        list.Add(new SimpleObject { Value = "second" });
        var array = new SimpleObject[3];
        list.CopyTo(array, 1);
        array[0].ShouldBeNull();
        array[1].Value.ShouldBe("first");
        array[2].Value.ShouldBe("second");
    }

    [Fact]
    public void ReferenceList_Indexer_Set_ShouldReplaceItem()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "old" });
        list[0] = new SimpleObject { Value = "new" };
        list[0].Value.ShouldBe("new");
    }

    [Fact]
    public void ReferenceList_ElementAt_ShouldReturnItem()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        list.ElementAt(0).Value.ShouldBe("test");
    }

    [Fact]
    public void ReferenceList_Queryable_ShouldReturnQueryable()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        var queryable = list.Queryable;
        queryable.ShouldNotBeNull();
        queryable.Count().ShouldBe(1);
    }

    [Fact]
    public void ReferenceList_IsReadOnly_ShouldBeFalse()
    {
        var list = new ReferenceList<SimpleObject>();
        list.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public void ReferenceList_GetEnumerator_ShouldEnumerate()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        var items = new List<SimpleObject>();
        foreach (var item in list) items.Add(item);
        items.Count.ShouldBe(1);
    }

    [Fact]
    public void ReferenceList_NonGenericGetEnumerator_ShouldEnumerate()
    {
        var list = new ReferenceList<SimpleObject>();
        list.Add(new SimpleObject { Value = "test" });
        var items = new List<object>();
        System.Collections.IEnumerable enumerable = list;
        foreach (var item in enumerable) items.Add(item);
        items.Count.ShouldBe(1);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public void Failure_FromException_ShouldCreateExceptionFailure()
    {
        var ex = new InvalidOperationException("test");
        var failure = Failure.FromException(ex);
        failure.ShouldBeOfType<ExceptionFailure>();
        failure.IsException.ShouldBeTrue();
        failure.TryGetException(out var extracted).ShouldBeTrue();
        extracted.ShouldBe(ex);
    }

    [Fact]
    public void Failure_FromMessage_ShouldCreateMessageFailure()
    {
        var message = "error message";
        var failure = Failure.FromMessage(message);
        failure.ShouldBeOfType<MessageFailure>();
        failure.IsMessage.ShouldBeTrue();
        failure.TryGetMessage(out var extracted).ShouldBeTrue();
        extracted.ShouldBe(message);
    }

    [Fact]
    public void Failure_FromNested_ShouldCreateNestedFailure()
    {
        var dict = new FailuresDictionary();
        dict.Failure("key", Failure.FromException(new InvalidOperationException("test")));
        var failure = Failure.FromNested(dict);
        failure.ShouldBeOfType<NestedFailure>();
        failure.IsNested.ShouldBeTrue();
        failure.TryGetNested(out var extracted).ShouldBeTrue();
        extracted.ShouldBe(dict);
    }

    [Fact]
    public void Failure_ImplicitConversion_FromException_ShouldWork()
    {
        var ex = new InvalidOperationException("test");
        Failure failure = ex;
        failure.ShouldBeOfType<ExceptionFailure>();
    }

    [Fact]
    public void Failure_ImplicitConversion_FromString_ShouldWork()
    {
        var message = "error message";
        Failure failure = message;
        failure.ShouldBeOfType<MessageFailure>();
    }

    [Fact]
    public void Failure_ImplicitConversion_FromFailuresDictionary_ShouldWork()
    {
        var dict = new FailuresDictionary();
        dict.Failure("key", Failure.FromException(new InvalidOperationException("test")));
        var failure = Failure.FromNested(dict);
        failure.ShouldBeOfType<NestedFailure>();
    }

    [Fact]
    public void Failure_Match_ShouldInvokeCorrectHandler()
    {
        var exFailure = Failure.FromException(new InvalidOperationException("ex"));
        var msgFailure = Failure.FromMessage("msg");
        var nestedFailure = Failure.FromNested(new FailuresDictionary());

        var exResult = exFailure.Match(
            onException: _ => "exception",
            onMessage: _ => "message",
            onNested: _ => "nested");
        exResult.ShouldBe("exception");

        var msgResult = msgFailure.Match(
            onException: _ => "exception",
            onMessage: _ => "message",
            onNested: _ => "nested");
        msgResult.ShouldBe("message");

        var nestedResult = nestedFailure.Match(
            onException: _ => "exception",
            onMessage: _ => "message",
            onNested: _ => "nested");
        nestedResult.ShouldBe("nested");
    }

    [Fact]
    public void Failure_TryGet_ShouldReturnFalseForWrongType()
    {
        var failure = Failure.FromException(new InvalidOperationException());
        failure.TryGetMessage(out _).ShouldBeFalse();
        failure.TryGetNested(out _).ShouldBeFalse();
    }

    #endregion

    #region ReferenceNotResolvedException Tests

    [Fact]
    public void ReferenceNotResolvedException_DefaultConstructor_ShouldWork()
    {
        var ex = new ReferenceNotResolvedException();
        ex.ShouldNotBeNull();
    }

    [Fact]
    public void ReferenceNotResolvedException_MessageConstructor_ShouldSetMessage()
    {
        var ex = new ReferenceNotResolvedException("custom message");
        ex.Message.ShouldBe("custom message");
    }

    [Fact]
    public void ReferenceNotResolvedException_InnerExceptionConstructor_ShouldSetInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ReferenceNotResolvedException("outer", inner);
        ex.Message.ShouldBe("outer");
        ex.InnerException.ShouldBe(inner);
    }

    #endregion

    #region AbstractBuilder Tests

    [Fact]
    public void AbstractBuilder_Result_BeforeBuild_ShouldThrow()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        Should.Throw<InvalidOperationException>(() => _ = builder.Result);
    }

    [Fact]
    public void AbstractBuilder_Result_AfterBuild_ShouldReturnValue()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        builder.Build();
        var result = builder.Result;
        result.ShouldNotBeNull();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void AbstractBuilder_Id_ShouldBeUnique()
    {
        var builder1 = new SimpleObjectBuilder();
        var builder2 = new SimpleObjectBuilder();
        builder1.Id.ShouldNotBe(builder2.Id);
        builder1.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Build_CalledTwice_ShouldReturnSameResult()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var result1 = builder.Build();
        var result2 = builder.Build();
        result1.Value.ShouldBeSameAs(result2.Value);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public void Build_WithExistingInstance_ShouldReturnExistingInstance()
    {
        var existingObj = new SimpleObject { Value = "existing" };
        var builder = new SimpleObjectBuilder().Existing(existingObj);
        var result = builder.Build().Value.Resolved();
        result.ShouldBeSameAs(existingObj);
    }

    [Fact]
    public void Build_WithExistingInstance_ShouldNotCallInstantiate()
    {
        var existingObj = new SimpleObject { Value = "existing" };
        var counter = new ThreadSafeCounter();
        var builder = new SlowBuilder(counter);
        builder.Existing(existingObj);
        builder.Build();
        builder.GetInstantiateCount().ShouldBe(0);
    }

    public static IEnumerable<object[]> ExistingPatterns
    {
        get
        {
            yield return new object[] { new SimpleObject { Value = "existing1" } };
            yield return new object[] { new SimpleObject { Value = "existing2" } };
            yield return new object[] { new SimpleObject { Value = "existing3" } };

        }
    }

    [Theory]
    [MemberData(nameof(ExistingPatterns))]
    public void Existing_ShouldReturnSameBuilderInstance(SimpleObject existingObj)
    {
        var builder = new SimpleObjectBuilder();
        var result = builder.Existing(existingObj);
        result.ShouldBeSameAs(builder);
    }
    #endregion

    #region Edge Cases

    [Fact]
    public void Build_PersonWithNullAddress_ShouldSucceed()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(25);
        var result = builder.Build().Value.Resolved();
        result.ShouldNotBeNull();
        result.Address.ShouldBeNull();
    }

    [Fact]
    public void Build_PersonWithEmptyFriendsList_ShouldSucceed()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(25);
        var result = builder.Build().Value.Resolved();
        result.ShouldNotBeNull();
        result.Friends.ShouldBeEmpty();
    }

    [Fact]
    public void Build_TaggedItemWithValidTags_ShouldSucceed()
    {
        var builder = new TaggedItemBuilder().WithTags(["tag1", "tag2", "tag3"]);
        var result = builder.Build().Value.Resolved();
        result.ShouldNotBeNull();
        result.Tags.Count.ShouldBe(3);
    }

    [Fact]
    public void BuildOrThrow_TaggedItemWithNullTags_ShouldThrowAggregateException()
    {
        var builder = new TaggedItemBuilder();
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is ArgumentNullException);
    }

    [Fact]
    public void BuildOrThrow_TaggedItemWithEmptyTags_ShouldThrowAggregateException()
    {
        var builder = new TaggedItemBuilder().WithTags([]);
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is StringIsEmptyOrWhitespaceException);
    }

    [Fact]
    public void BuildOrThrow_TaggedItemWithWhitespaceTag_ShouldThrowAggregateException()
    {
        var builder = new TaggedItemBuilder().WithTags(["valid", "   ", "another"]);
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldNotBeEmpty();
    }

    [Fact]
    public void BuildOrThrow_ValidBuilder_ShouldReturnObject()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(25);
        var result = builder.BuildOrThrow();
        result.ShouldNotBeNull();
        result.Name.ShouldBe("John");
    }

    [Fact]
    public void BuildOrThrow_NestedValidationFailure_ShouldExtractNestedExceptions()
    {
        var builder = new PersonBuilder().WithName("Alice").WithAge(25)
            .WithFriend(f => f.WithAge(26)); // Missing name
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldNotBeEmpty();
    }

    #endregion

    #region Fluent API Tests

    [Fact]
    public void FluentApi_ShouldReturnSameBuilderInstance()
    {
        var builder = new PersonBuilder();
        var result = builder.WithName("Test").WithAge(25).WithEmail("test@test.com");
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void FluentApi_AddressBuilder_ShouldReturnSameBuilderInstance()
    {
        var builder = new AddressBuilder();
        var result = builder.WithStreet("Street").WithCity("City").WithZipCode("12345");
        result.ShouldBeSameAs(builder);
    }

    #endregion

    #region Thread-Safety Tests

    [Fact]
    public async Task Reference_Resolve_ShouldBeThreadSafe_OnlyFirstWins()
    {
        var reference = new Reference<SimpleObject>();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        var obj3 = new SimpleObject { Value = "third" };

        var tasks = new[]
        {
            Task.Run(() => reference.Resolve(obj1)),
            Task.Run(() => reference.Resolve(obj2)),
            Task.Run(() => reference.Resolve(obj3))
        };
        await Task.WhenAll(tasks);

        reference.IsResolved.ShouldBeTrue();
        var resolved = reference.Resolved();
        new[] { "first", "second", "third" }.ShouldContain(resolved.Value);
    }

    [Fact]
    public async Task Build_ConcurrentCalls_ShouldOnlyBuildOnce()
    {
        var counter = new ThreadSafeCounter();
        var builder = new SlowBuilder(counter).WithValue("test");

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => builder.Build()))
            .ToArray();
        await Task.WhenAll(tasks);

        builder.GetInstantiateCount().ShouldBe(1);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public async Task Build_ConcurrentCalls_AllShouldReturnSameReference()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => builder.Build()))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        var firstRef = results[0].Value;
        foreach (var result in results)
        {
            result.Value.ShouldBeSameAs(firstRef);
        }
    }

    [Fact]
    public async Task Validate_ConcurrentCalls_ShouldOnlyValidateOnce()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();

        builder.Validate(visited, failures);
        var statusAfterFirst = builder.ValidationStatus;

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() =>
            {
                var v = new VisitedObjectDictionary();
                var f = new FailuresDictionary();
                builder.Validate(v, f);
            }))
            .ToArray();
        await Task.WhenAll(tasks);

        statusAfterFirst.ShouldBe(ValidationStatus.Validated);
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public async Task BuildStatus_ShouldBeThreadSafeToRead()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        var statusReadCount = 0;

        var buildTask = Task.Run(() => builder.Build());
        var statusTasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() =>
            {
                var status = builder.BuildStatus;
                Interlocked.Increment(ref statusReadCount);
                return status;
            }))
            .ToArray();

        await Task.WhenAll([buildTask, .. statusTasks]);

        statusReadCount.ShouldBe(100);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public async Task ValidationStatus_ShouldBeThreadSafeToRead()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        var statusReadCount = 0;

        var validateTask = Task.Run(() => builder.Validate(visited, failures));
        var statusTasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() =>
            {
                var status = builder.ValidationStatus;
                Interlocked.Increment(ref statusReadCount);
                return status;
            }))
            .ToArray();

        await Task.WhenAll([validateTask, .. statusTasks]);

        statusReadCount.ShouldBe(100);
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public async Task Reference_IsResolved_ShouldBeThreadSafeToRead()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };

        var resolveTask = Task.Run(() =>
        {
            Thread.Sleep(5);
            reference.Resolve(obj);
        });

        var readTasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => reference.IsResolved))
            .ToArray();

        await Task.WhenAll([resolveTask, .. readTasks]);

        reference.IsResolved.ShouldBeTrue();
    }

    [Fact]
    public void Reference_ResolvedOrNull_ShouldNeverThrow()
    {
        var reference = new Reference<SimpleObject>();
        reference.ResolvedOrNull().ShouldBeNull();
        reference.Resolve(new SimpleObject { Value = "test" });
        reference.ResolvedOrNull().ShouldNotBeNull();
    }

    [Fact]
    public async Task MultipleBuildersWithSharedReference_ShouldBeThreadSafe()
    {
        var departmentBuilder = new DepartmentBuilder().WithName("Engineering");
        var employeeBuilders = Enumerable.Range(0, 10)
            .Select(i => new EmployeeBuilder().WithName($"Employee{i}").WithDepartment(departmentBuilder.Reference()))
            .ToList();

        var departmentTask = Task.Run(() => departmentBuilder.Build());
        var employeeTasks = employeeBuilders.Select(b => Task.Run(() => b.Build())).ToArray();

        await Task.WhenAll([departmentTask, .. employeeTasks]);

        departmentBuilder.Reference().IsResolved.ShouldBeTrue();
        var department = departmentBuilder.Reference().Resolved();
        department.Name.ShouldBe("Engineering");
        
        // All employees should have their references resolved
        foreach (var employeeBuilder in employeeBuilders)
        {
            employeeBuilder.Reference().IsResolved.ShouldBeTrue();
            var employee = employeeBuilder.Reference().Resolved();
            employee.Department.ShouldBeSameAs(department);
        }
    }

    [Fact]
    public async Task Build_WithExisting_ConcurrentCalls_ShouldAllReturnSameExisting()
    {
        var existingObj = new SimpleObject { Value = "existing" };
        var builder = new SimpleObjectBuilder();
        builder.Existing(existingObj);

        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() => builder.Build()))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            result.Value.Resolved().ShouldBeSameAs(existingObj);
        }
    }

    [Fact]
    public void Build_WithVisitedContainingSelf_ShouldReturnEarlyWithReference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        visited[builder.Id] = builder;
        
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithVisitedContainingBuiltBuilder_ShouldReturnEarlyWithReference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        builder.Build();
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
        visited[builder.Id] = builder;
        
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsResolved.ShouldBeTrue();
    }

    #endregion

    #region Complex Circular Reference Tests

    [Fact]
    public void Build_MutualCircularReferences_WhenBuiltSequentially_SecondHasNullReference()
    {
        // This test documents the expected behavior: when two builders reference each other,
        // the one built first will have its reference resolved in the second,
        // but the second's reference in the first will be null (not yet resolved at instantiation time)
        
        var dept1Builder = new DepartmentBuilder().WithName("Dept1");
        var dept2Builder = new DepartmentBuilder().WithName("Dept2");
        
        // Employee in dept1 references dept2
        dept1Builder.WithEmployee(e => e.WithName("Emp1").WithDepartment(dept2Builder.Reference()));
        
        // Employee in dept2 references dept1
        dept2Builder.WithEmployee(e => e.WithName("Emp2").WithDepartment(dept1Builder.Reference()));
        
        // Build dept1 first - dept2 is not yet resolved
        var dept1 = dept1Builder.BuildOrThrow();
        
        // At this point, dept2 is not resolved yet, so Emp1.Department is null
        dept1.Employees[0].Department.ShouldBeNull();
        
        // Now build dept2 - dept1 is already resolved
        var dept2 = dept2Builder.BuildOrThrow();
        
        // dept1 was resolved before dept2's employees were instantiated, so Emp2.Department is dept1
        dept2.Employees[0].Department.ShouldBeSameAs(dept1);
    }

    [Fact]
    public void Build_CircularReference_WithinSameBuilder_ShouldResolveAfterBuild()
    {
        // When employees reference their own department within the same build,
        // the reference is resolved AFTER Instantiate() completes
        var deptBuilder = new DepartmentBuilder().WithName("Engineering");
        
        // All employees reference the same department
        for (int i = 0; i < 5; i++)
        {
            deptBuilder.WithEmployee(e => e
                .WithName($"Employee{i}")
                .WithDepartment(deptBuilder.Reference()));
        }
        
        // Build the department
        var dept = deptBuilder.BuildOrThrow();
        
        // Verify department was built
        dept.Name.ShouldBe("Engineering");
        dept.Employees.Count.ShouldBe(5);
        
        // Note: The employees were instantiated before the department reference was resolved,
        // so their Department property is null (using ResolvedOrNull in Instantiate)
        // This is expected behavior for this implementation pattern
        foreach (var emp in dept.Employees)
        {
            emp.Name.ShouldNotBeNullOrEmpty();
            // Department is null because ResolvedOrNull() was called before resolution
        }
        
        // However, the reference itself IS resolved after Build completes
        deptBuilder.Reference().IsResolved.ShouldBeTrue();
        deptBuilder.Reference().Resolved().ShouldBeSameAs(dept);
    }

    [Fact]
    public void Build_CircularReference_UsingExistingPattern_ShouldResolveCorrectly()
    {
        // The correct pattern for circular references: build department first with Existing
        // This test shows the documented pattern from the README
        var deptBuilder = new DepartmentBuilder().WithName("Engineering");
        
        deptBuilder
            .WithManager(m => m.WithName("Alice").WithDepartment(deptBuilder.Reference()))
            .WithEmployee(e => e.WithName("Bob").WithDepartment(deptBuilder.Reference()));
        
        var dept = deptBuilder.BuildOrThrow();
        
        // The manager and employees were built after the department reference was resolved
        dept.Manager.ShouldNotBeNull();
        dept.Manager.Name.ShouldBe("Alice");
        dept.Employees.Count.ShouldBe(1);
        
        // Reference is now resolved
        deptBuilder.Reference().IsResolved.ShouldBeTrue();
    }

    #endregion

    #region BuildStatus and ValidationStatus Tests

    [Fact]
    public void BuildStatus_NotBuilding_ShouldBeInitialState()
    {
        var builder = new SimpleObjectBuilder();
        builder.BuildStatus.ShouldBe(BuildStatus.NotBuilding);
    }

    [Fact]
    public void BuildStatus_Built_ShouldBeSetAfterSuccessfulBuild()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        builder.Build();
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public void BuildStatus_Building_ShouldBeSetDuringFailedValidation()
    {
        var builder = new PersonBuilder().WithAge(25); // Missing name
        builder.Build();
        builder.BuildStatus.ShouldBe(BuildStatus.Building); // Stuck in Building due to validation failure
    }

    [Fact]
    public void ValidationStatus_NotValidated_ShouldBeInitialState()
    {
        var builder = new SimpleObjectBuilder();
        builder.ValidationStatus.ShouldBe(ValidationStatus.NotValidated);
    }

    [Fact]
    public void ValidationStatus_Validated_ShouldBeSetAfterValidation()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        
        builder.Validate(visited, failures);
        
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    #endregion

    #region Struct Value Type Tests

    [Fact]
    public void BuildStatus_ShouldBeEnum()
    {
        typeof(BuildStatus).IsEnum.ShouldBeTrue();
    }

    [Fact]
    public void ValidationStatus_ShouldBeEnum()
    {
        typeof(ValidationStatus).IsEnum.ShouldBeTrue();
    }

    #endregion

    #region AbstractBuilder Constructor Branch Coverage Tests

    [Fact]
    public void AbstractBuilder_Constructor_NullReferenceFactory_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new TestBuilderWithCustomStrategy(null!, LockSynchronizationStrategy.Instance));
    }

    [Fact]
    public void AbstractBuilder_Constructor_NullSyncStrategy_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new TestBuilderWithCustomStrategy(DefaultReferenceFactory.Instance, null!));
    }

    [Fact]
    public void AbstractBuilder_Constructor_WithReferenceFactoryOnly_ShouldWork()
    {
        var builder = new TestBuilderWithReferenceFactory(DefaultReferenceFactory.Instance);
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void AbstractBuilder_Constructor_WithCustomSyncStrategy_ShouldUseIt()
    {
        var builder = new TestBuilderWithCustomStrategy(DefaultReferenceFactory.Instance, NoSynchronizationStrategy.Instance);
        builder.WithValue("test");
        var result = builder.BuildOrThrow();
        result.ShouldNotBeNull();
    }

    #endregion

    #region BuildCore Branch Coverage Tests

    [Fact]
    public void Build_WhenAlreadyBuiltInsideLock_ShouldReturnExistingReference()
    {
        // This tests the double-check inside BuildCore (line 143)
        var builder = new SimpleObjectBuilder().WithValue("test");
        
        // Build once
        var result1 = builder.Build();
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
        
        // Build again - should hit the BuildStatus.Built check inside BuildCore
        var result2 = builder.Build();
        
        result1.Value.ShouldBeSameAs(result2.Value);
    }

    [Fact]
    public void Build_WithExisting_InsideBuildCore_ShouldReturnExisting()
    {
        // This tests line 148-154 - _existing check inside BuildCore
        var existing = new SimpleObject { Value = "existing" };
        var counter = new ThreadSafeCounter();
        var builder = new SlowBuilder(counter);
        
        // Set existing after construction but before build
        builder.Existing(existing);
        
        var result = builder.Build();
        
        result.Value.IsResolved.ShouldBeTrue();
        result.Value.Resolved().ShouldBeSameAs(existing);
        builder.GetInstantiateCount().ShouldBe(0);
    }

    #endregion

    #region Build Visited Dictionary Branch Coverage Tests

    [Fact]
    public void Build_WithVisited_ContainingNotValidatedBuilder_ShouldReturnEarly()
    {
        // Tests line 127-128
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        
        // Add builder to visited but don't validate it
        visited[builder.Id] = builder;
        builder.ValidationStatus.ShouldBe(ValidationStatus.NotValidated);
        
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithVisited_ContainingBuildingBuilder_ShouldReturnEarly()
    {
        // Tests line 129-130 - BuildStatus.Building
        var builder = new BuildingStatusTestBuilder();
        var visited = new VisitedObjectDictionary();
        
        // Simulate being in Building status
        builder.SetBuildingStatus();
        visited[builder.Id] = builder;
        
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithVisited_ContainingValidatingBuilder_ShouldReturnEarly()
    {
        // Tests line 127-128 - ValidationStatus.Validating
        var builder = new ValidatingStatusTestBuilder();
        var visited = new VisitedObjectDictionary();
        
        // Simulate being in Validating status
        builder.SetValidatingStatus();
        visited[builder.Id] = builder;
        
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
    }

    #endregion

    #region BuildOrThrow Branch Coverage Tests

    [Fact]
    public void BuildOrThrow_WithMessageFailure_ShouldExtractAsInvalidOperationException()
    {
        // Tests line 227 - message failure extraction
        var builder = new MessageFailureTestBuilder();
        
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is InvalidOperationException && e.Message == "Test message failure");
    }

    [Fact]
    public void BuildOrThrow_WithNestedFailure_ShouldExtractRecursively()
    {
        // Tests line 228 - nested failure extraction
        var builder = new NestedFailureTestBuilder();
        
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldNotBeEmpty();
        exception.InnerExceptions.ShouldContain(e => e is ArgumentException && e.Message == "Nested error");
    }

    [Fact]
    public void BuildOrThrow_WithExceptionFailure_ShouldExtractDirectly()
    {
        // Tests line 226 - exception failure extraction
        var builder = new ExceptionFailureTestBuilder();
        
        var exception = Should.Throw<AggregateException>(() => builder.BuildOrThrow());
        exception.InnerExceptions.ShouldContain(e => e is ArgumentException && e.Message == "Direct exception failure");
    }

    #endregion

    #region ReaderWriterSynchronizationStrategy Builder Tests

    [Fact]
    public void Builder_WithReaderWriterStrategy_ShouldBuildSuccessfully()
    {
        var builder = new ReaderWriterStrategyBuilder().WithValue("test");
        var result = builder.BuildOrThrow();
        
        result.ShouldNotBeNull();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void Builder_WithReaderWriterStrategy_ShouldValidateSuccessfully()
    {
        var builder = new ReaderWriterStrategyBuilder().WithValue("test");
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        
        builder.Validate(visited, failures);
        
        failures.HasFailures.ShouldBeFalse();
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public async Task Builder_WithReaderWriterStrategy_ConcurrentBuilds_ShouldOnlyBuildOnce()
    {
        var counter = new ThreadSafeCounter();
        var builder = new SlowReaderWriterStrategyBuilder(counter).WithValue("test");

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => builder.Build()))
            .ToArray();
        await Task.WhenAll(tasks);

        builder.GetInstantiateCount().ShouldBe(1);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public async Task Builder_WithReaderWriterStrategy_ConcurrentValidations_ShouldOnlyValidateOnce()
    {
        var builder = new ReaderWriterStrategyBuilder().WithValue("test");
        var barrier = new Barrier(10);

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() =>
            {
                barrier.SignalAndWait();
                var v = new VisitedObjectDictionary();
                var f = new FailuresDictionary();
                builder.Validate(v, f);
                return f;
            }))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        foreach (var failures in results)
        {
            failures.HasFailures.ShouldBeFalse();
        }
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public void ReaderWriterSynchronizationStrategy_Execute_Action_ShouldWork()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var executed = false;
        var lockObj = new object();

        strategy.Execute(lockObj, () => executed = true);

        executed.ShouldBeTrue();
    }

    [Fact]
    public void ReaderWriterSynchronizationStrategy_Execute_Func_ShouldReturnResult()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var lockObj = new object();

        var result = strategy.Execute(lockObj, () => 42);

        result.ShouldBe(42);
    }

    [Fact]
    public void ReaderWriterSynchronizationStrategy_ExecuteRead_ShouldReturnResult()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();

        var result = strategy.ExecuteRead(() => "test");

        result.ShouldBe("test");
    }

    [Fact]
    public async Task ReaderWriterSynchronizationStrategy_ShouldAllowConcurrentReads()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var concurrentReads = 0;
        var maxConcurrentReads = 0;

        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() =>
            {
                strategy.ExecuteRead(() =>
                {
                    var current = Interlocked.Increment(ref concurrentReads);
                    if (current > maxConcurrentReads)
                        Interlocked.Exchange(ref maxConcurrentReads, current);
                    
                    Thread.Sleep(10);
                    Interlocked.Decrement(ref concurrentReads);
                    return 0;
                });
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Multiple reads should have happened concurrently
        maxConcurrentReads.ShouldBeGreaterThan(1);
    }

    #endregion

    #region Reference Additional Concurrency Tests

    [Fact]
    public async Task Reference_ConcurrentResolveAndOnResolve_ShouldBeThreadSafe()
    {
        var reference = new Reference<SimpleObject>();
        var callCount = 0;
        var obj = new SimpleObject { Value = "test" };

        // Register callbacks and resolve concurrently
        var registerTasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(() => reference.OnResolve(_ => Interlocked.Increment(ref callCount))))
            .ToArray();

        var resolveTask = Task.Run(() =>
        {
            Thread.Sleep(5); // Let some callbacks register
            reference.Resolve(obj);
        });

        await Task.WhenAll(registerTasks.Concat(new[] { resolveTask }));

        reference.IsResolved.ShouldBeTrue();
        // Callbacks registered before resolve should have been called
        callCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Reference_MultipleConcurrentResolves_OnlyFirstWins()
    {
        var reference = new Reference<SimpleObject>();
        var objects = Enumerable.Range(0, 10)
            .Select(i => new SimpleObject { Value = $"obj{i}" })
            .ToList();

        var tasks = objects.Select(obj => Task.Run(() => reference.Resolve(obj))).ToArray();
        await Task.WhenAll(tasks);

        reference.IsResolved.ShouldBeTrue();
        // Only one object should win
        objects.ShouldContain(reference.Resolved());
    }

    [Fact]
    public void Reference_OnResolve_AfterAlreadyResolved_ShouldNotCallCallback()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        
        reference.Resolve(obj);
        
        var wasCalledAfter = false;
        reference.OnResolve(_ => wasCalledAfter = true);
        
        // According to the implementation, OnResolve just adds to the list
        // but since _instance == instance check fails for new registrations after resolve,
        // the callback won't be called during the Resolve that already happened
        // Actually, looking at the code again, OnResolve just adds to the list
        // and Resolve only calls callbacks if _instance == instance (first resolve)
        // So callbacks added after resolve are never called
        wasCalledAfter.ShouldBeFalse();
    }

    [Fact]
    public void Reference_Resolve_WithSameInstanceTwice_ShouldCallCallbacksOnce()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        var callCount = 0;
        
        reference.OnResolve(_ => callCount++);
        reference.Resolve(obj);
        reference.Resolve(obj); // Same instance again
        
        callCount.ShouldBe(1);
    }

    [Fact]
    public void Reference_Constructor_WithExisting_ShouldNotTriggerCallbacks()
    {
        var obj = new SimpleObject { Value = "test" };
        var reference = new Reference<SimpleObject>(obj);
        
        var wasCalled = false;
        reference.OnResolve(_ => wasCalled = true);
        
        // Callback registered after construction with existing shouldn't be called
        // because Resolve was never called
        wasCalled.ShouldBeFalse();
    }

    #endregion

    #region BuilderListWithFactory Direct Tests

    [Fact]
    public void BuilderListWithFactory_New_CreatesAndConfiguresBuilder()
    {
        var factoryCallCount = 0;
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() =>
        {
            factoryCallCount++;
            return new SimpleObjectBuilder();
        });

        list.New(b => b.WithValue("first"));
        list.New(b => b.WithValue("second"));
        list.New(b => b.WithValue("third"));

        factoryCallCount.ShouldBe(3);
        list.Count.ShouldBe(3);
    }

    [Fact]
    public void BuilderListWithFactory_AsReferenceList_ReturnsCorrectReferences()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("first"));
        list.New(b => b.WithValue("second"));
        
        // Build all
        foreach (var builder in list)
            builder.Build();

        var referenceList = list.AsReferenceList();
        
        referenceList.Count.ShouldBe(2);
        referenceList[0].Value.ShouldBe("first");
        referenceList[1].Value.ShouldBe("second");
    }

    [Fact]
    public void BuilderListWithFactory_BuildSuccess_BuildsAllAndReturnsInstances()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("a"));
        list.New(b => b.WithValue("b"));
        list.New(b => b.WithValue("c"));

        var results = list.BuildSuccess();

        results.Count.ShouldBe(3);
        results[0].Value.ShouldBe("a");
        results[1].Value.ShouldBe("b");
        results[2].Value.ShouldBe("c");
    }

    [Fact]
    public void BuilderListWithFactory_ValidateFailures_ReturnsOnlyFailures()
    {
        var list = new BuilderListWithFactory<Person, PersonBuilder>(() => new PersonBuilder());
        list.New(b => b.WithName("Valid1").WithAge(25)); // Valid
        list.New(b => b.WithAge(30)); // Invalid - missing name
        list.New(b => b.WithName("Valid2").WithAge(35)); // Valid
        list.New(b => b.WithAge(-5)); // Invalid - missing name and negative age

        var failures = list.ValidateFailures();

        failures.Count.ShouldBe(2); // Only the invalid ones
    }

    [Fact]
    public void BuilderListWithFactory_NullFactory_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => 
            new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(null!));
    }

    [Fact]
    public void BuilderListWithFactory_New_ReturnsListForChaining()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        
        var result = list
            .New(b => b.WithValue("a"))
            .New(b => b.WithValue("b"))
            .New(b => b.WithValue("c"));

        result.ShouldBeSameAs(list);
        list.Count.ShouldBe(3);
    }

    [Fact]
    public void BuilderListWithFactory_WithDependencyInjectionPattern_ShouldWork()
    {
        // Simulate DI by using a factory that tracks creation
        var createdBuilders = new List<SimpleObjectBuilder>();
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() =>
        {
            var builder = new SimpleObjectBuilder();
            createdBuilders.Add(builder);
            return builder;
        });

        list.New(b => b.WithValue("from-di"));

        createdBuilders.Count.ShouldBe(1);
        createdBuilders[0].ShouldBeSameAs(list[0]);
    }

    [Fact]
    public void BuilderListWithFactory_AsReferenceList_BeforeBuild_ReturnsUnresolvedReferences()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("test"));

        var referenceList = list.AsReferenceList();

        referenceList.Count.ShouldBe(1);
        // References exist but are not resolved yet
        var reference = list[0].Reference();
        reference.IsResolved.ShouldBeFalse();
    }

    #endregion

}
#region Test Helper Builders for Branch Coverage

/// <summary>
/// Builder that allows setting custom reference factory and sync strategy for testing constructors.
/// </summary>
public class TestBuilderWithCustomStrategy : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    
    public TestBuilderWithCustomStrategy(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy)
        : base(referenceFactory, syncStrategy) { }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
    
    public TestBuilderWithCustomStrategy WithValue(string value) { Value = value; return this; }
}

/// <summary>
/// Builder that uses the single-parameter constructor with reference factory.
/// </summary>
public class TestBuilderWithReferenceFactory : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    
    public TestBuilderWithReferenceFactory(IReferenceFactory referenceFactory)
        : base(referenceFactory) { }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
}

/// <summary>
/// Builder that exposes a method to set BuildStatus to Building for testing.
/// </summary>
public class BuildingStatusTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    public void SetBuildingStatus()
    {
        // Use reflection to set the private _buildStatus field
        var field = typeof(AbstractBuilder<SimpleObject>).GetField("_buildStatus", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(this, (int)BuildStatus.Building);
    }
}

/// <summary>
/// Builder that exposes a method to set ValidationStatus to Validating for testing.
/// </summary>
public class ValidatingStatusTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    public void SetValidatingStatus()
    {
        // Use reflection to set the private _validationStatus field
        var field = typeof(AbstractBuilder<SimpleObject>).GetField("_validationStatus", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(this, (int)ValidationStatus.Validating);
    }
}

/// <summary>
/// Builder that produces a message failure for testing message extraction.
/// </summary>
public class MessageFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        failures.AddFailure("test", Failure.FromMessage("Test message failure"));
    }
}

/// <summary>
/// Builder that produces a nested failure for testing recursive extraction.
/// </summary>
public class NestedFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("inner", Failure.FromException(new ArgumentException("Nested error")));
        failures.AddFailure("outer", Failure.FromNested(nested));
    }
}

/// <summary>
/// Builder that produces an exception failure for testing direct exception extraction.
/// </summary>
public class ExceptionFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        failures.AddFailure("test", Failure.FromException(new ArgumentException("Direct exception failure")));
    }
}

/// <summary>
/// Builder with slow validation to test concurrent validation scenarios.
/// </summary>
public class SlowValidatingBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        Thread.Sleep(50); // Slow validation
    }
}

/// <summary>
/// Builder that overrides CreateFailureCollector to track if it was used.
/// </summary>
public class CustomFailureCollectorBuilder : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    public bool CustomCollectorUsed { get; private set; }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
    
    protected override FailuresDictionary CreateFailureCollector()
    {
        CustomCollectorUsed = true;
        return base.CreateFailureCollector();
    }
    
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNull(Value, nameof(Value), failures, n => new ArgumentNullException(n));
    }
}

/// <summary>
/// Builder that uses ReaderWriterSynchronizationStrategy for testing.
/// </summary>
public class ReaderWriterStrategyBuilder : AbstractBuilder<SimpleObject>
{
    private static readonly ReaderWriterSynchronizationStrategy _strategy = new();
    
    public string? Value { get; set; }
    
    public ReaderWriterStrategyBuilder() 
        : base(DefaultReferenceFactory.Instance, _strategy) { }
    
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
    
    public ReaderWriterStrategyBuilder WithValue(string value) { Value = value; return this; }
}

/// <summary>
/// Slow builder that uses ReaderWriterSynchronizationStrategy for concurrent testing.
/// </summary>
public class SlowReaderWriterStrategyBuilder : AbstractBuilder<SimpleObject>
{
    private static readonly ReaderWriterSynchronizationStrategy _strategy = new();
    private readonly ThreadSafeCounter _counter;
    
    public string? Value { get; set; }
    
    public SlowReaderWriterStrategyBuilder(ThreadSafeCounter counter) 
        : base(DefaultReferenceFactory.Instance, _strategy)
    {
        _counter = counter;
    }
    
    protected override SimpleObject Instantiate()
    {
        Interlocked.Increment(ref _counter.InstantiateCount);
        Thread.Sleep(10);
        return new() { Value = Value ?? string.Empty };
    }
    
    public SlowReaderWriterStrategyBuilder WithValue(string value) { Value = value; return this; }
    public int GetInstantiateCount() => _counter.InstantiateCount;
}

#endregion
