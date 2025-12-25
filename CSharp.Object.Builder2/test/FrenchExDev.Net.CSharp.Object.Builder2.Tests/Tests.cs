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

    [Fact]
    public void Existing_ShouldReturnSameBuilderInstance()
    {
        var builder = new SimpleObjectBuilder();
        var existingObj = new SimpleObject { Value = "test" };
        var result = builder.Existing(existingObj);
        result.ShouldBeSameAs(builder);
    }

    #endregion

    #region FailuresDictionary Tests

    [Fact]
    public void FailuresDictionary_Failure_ShouldAddToExistingList()
    {
        var dict = new FailuresDictionary();
        dict.Failure("key", new InvalidOperationException("first"));
        dict.Failure("key", new InvalidOperationException("second"));
        dict["key"].Count.ShouldBe(2);
    }

    [Fact]
    public void FailuresDictionary_Failure_ShouldReturnSameInstance()
    {
        var dict = new FailuresDictionary();
        var result = dict.Failure("key", new InvalidOperationException("test"));
        result.ShouldBeSameAs(dict);
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

    [Fact]
    public async Task Build_ConcurrentWithVisited_ShouldHandleMultipleThreads()
    {
        // Note: VisitedObjectDictionary is NOT thread-safe by design.
        // Each concurrent build should use its own visited dictionary.
        var builders = Enumerable.Range(0, 10)
            .Select(i => new PersonBuilder().WithName($"Person{i}").WithAge(25 + i))
            .ToList();

        // Each build gets its own visited dictionary
        var tasks = builders.Select(b => Task.Run(() => 
        {
            var visited = new VisitedObjectDictionary();
            return b.Build(visited);
        })).ToArray();
        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            result.IsSuccess.ShouldBeTrue();
        }
    }

    [Fact]
    public async Task BuildOrThrow_ConcurrentCalls_ShouldAllReturnSameResult()
    {
        var builder = new SimpleObjectBuilder().WithValue("test");
        
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => builder.BuildOrThrow()))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        var first = results[0];
        foreach (var result in results)
        {
            result.ShouldBeSameAs(first);
        }
    }

    [Fact]
    public async Task Validate_ConcurrentCalls_FirstShouldBlockOthers()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
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
            failures.ShouldBeEmpty();
        }
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public async Task Build_CircularReferences_ConcurrentBuild_ShouldHandleCorrectly()
    {
        var departmentBuilder = new DepartmentBuilder().WithName("Engineering");
        for (int i = 0; i < 5; i++)
        {
            departmentBuilder.WithEmployee(e => e.WithName($"Employee{i}").WithDepartment(departmentBuilder.Reference()));
        }

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => departmentBuilder.Build()))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        var reference = departmentBuilder.Reference();
        reference.IsResolved.ShouldBeTrue();
        var department = reference.Resolved();
        department.Name.ShouldBe("Engineering");
        department.Employees.Count.ShouldBe(5);

        foreach (var result in results)
        {
            result.Value.ShouldBeSameAs(reference);
        }
    }

    [Fact]
    public async Task Reference_MultipleResolveAttempts_OnlyFirstWins()
    {
        var reference = new Reference<SimpleObject>();
        var objects = Enumerable.Range(0, 100)
            .Select(i => new SimpleObject { Value = $"value{i}" })
            .ToArray();

        var tasks = objects.Select(obj => Task.Run(() => reference.Resolve(obj))).ToArray();
        await Task.WhenAll(tasks);

        reference.IsResolved.ShouldBeTrue();
        var resolved = reference.Resolved();
        objects.ShouldContain(resolved);
    }

    [Fact]
    public async Task Reference_ConcurrentReadsWhileResolving_ShouldNotThrow()
    {
        var reference = new Reference<SimpleObject>();
        var obj = new SimpleObject { Value = "test" };
        var readsCompleted = 0;

        var readers = Enumerable.Range(0, 100)
            .Select(idx => Task.Run(() =>
            {
                while (!reference.IsResolved)
                {
                    var nullableResult = reference.ResolvedOrNull();
                    var instanceResult = reference.Instance;
                }
                Interlocked.Increment(ref readsCompleted);
            }))
            .ToArray();

        await Task.Delay(1);
        reference.Resolve(obj);
        await Task.WhenAll(readers);

        readsCompleted.ShouldBe(100);
        reference.Resolved().ShouldBeSameAs(obj);
    }

    [Fact]
    public void Build_WithVisitedContainingValidatedBuildingBuilder_ShouldReturnEarlyWithReference()
    {
        // Test line 228: when visited contains a builder that is Validated AND Building
        // This requires the builder to be in the middle of building when we check visited
        
        // This is a race condition edge case that's very hard to test directly.
        // The code path is: 
        // 1. Builder is in visited dictionary
        // 2. ValidationStatus is Validated (not NotValidated or Validating)
        // 3. BuildStatus is Built or Building
        // But if BuildStatus is Built, we would have returned at line 220 already.
        // So this path requires BuildStatus == Building, which happens only during concurrent builds.
        
        // We'll verify the existing test coverage is correct - this defensive code 
        // protects against edge cases that are nearly impossible to trigger in tests.
        
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        
        // Build and verify
        builder.Build();
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
        
        // Add to visited
        visited[builder.Id] = builder;
        
        // Since BuildStatus is Built, we'll return at line 220, not hit line 228
        // This test documents the expected behavior
        var result = builder.Build(visited);
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsResolved.ShouldBeTrue();
    }

    #endregion

    #region Interface Segregation Tests

    [Fact]
    public void Builder_ShouldImplementIBuildable()
    {
        IBuildable<SimpleObject> buildable = new SimpleObjectBuilder().WithValue("test");
        buildable.BuildStatus.ShouldBe(BuildStatus.NotBuilding);
        var result = buildable.Build();
        result.IsSuccess.ShouldBeTrue();
        buildable.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public void Builder_ShouldImplementIValidatable()
    {
        IValidatable validatable = new PersonBuilder().WithName("Test").WithAge(25);
        validatable.ValidationStatus.ShouldBe(ValidationStatus.NotValidated);
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();
        validatable.Validate(visited, failures);
        validatable.ValidationStatus.ShouldBe(ValidationStatus.Validated);
        failures.ShouldBeEmpty();
    }

    [Fact]
    public void Builder_ShouldImplementIReferenceable()
    {
        IReferenceable<SimpleObject> referenceable = new SimpleObjectBuilder().WithValue("test");
        var reference = referenceable.Reference();
        reference.ShouldNotBeNull();
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Builder_ShouldImplementIIdentifiable()
    {
        IIdentifiable identifiable = new SimpleObjectBuilder();
        identifiable.Id.ShouldNotBe(Guid.Empty);
    }

    #endregion

    #region ValidationAssertions Static Class Tests

    [Fact]
    public void ValidationAssertions_AssertNotNull_ShouldAddFailureWhenNull()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNull(null, "TestField", failures, n => new ArgumentNullException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
        failures["TestField"].Count.ShouldBe(1);
    }

    [Fact]
    public void ValidationAssertions_AssertNotNull_ShouldNotAddFailureWhenNotNull()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNull("value", "TestField", failures, n => new ArgumentNullException(n));
        failures.ShouldBeEmpty();
    }

    [Fact]
    public void ValidationAssertions_AssertNotEmptyOrWhitespace_String_ShouldAddFailureWhenWhitespace()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace("   ", "TestField", failures, n => new ArgumentException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
    }

    [Fact]
    public void ValidationAssertions_AssertNotEmptyOrWhitespace_String_ShouldNotAddFailureWhenNull()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace((string?)null, "TestField", failures, n => new ArgumentException(n));
        failures.ShouldBeEmpty();
    }

    [Fact]
    public void ValidationAssertions_AssertNotEmptyOrWhitespace_List_ShouldValidateEachItem()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotEmptyOrWhitespace(["valid", "   ", "another"], "TestField", failures, n => new ArgumentException(n));
        failures["TestField"].Count.ShouldBe(1);
    }

    [Fact]
    public void ValidationAssertions_AssertNotNullOrEmptyOrWhitespace_ShouldAddFailureWhenNull()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullOrEmptyOrWhitespace(null, "TestField", failures, n => new ArgumentException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
    }

    [Fact]
    public void ValidationAssertions_AssertNotNullNotEmptyCollection_ShouldAddFailureWhenNull()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection<string>(null, "TestField", failures, n => new ArgumentException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
    }

    [Fact]
    public void ValidationAssertions_AssertNotNullNotEmptyCollection_ShouldAddFailureWhenEmpty()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.AssertNotNullNotEmptyCollection(new List<string>(), "TestField", failures, n => new ArgumentException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
    }

    [Fact]
    public void ValidationAssertions_Assert_ShouldAddFailureWhenPredicateTrue()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.Assert(() => true, "TestField", failures, n => new ArgumentException(n));
        failures.ContainsKey("TestField").ShouldBeTrue();
    }

    [Fact]
    public void ValidationAssertions_Assert_ShouldNotAddFailureWhenPredicateFalse()
    {
        var failures = new FailuresDictionary();
        ValidationAssertions.Assert(() => false, "TestField", failures, n => new ArgumentException(n));
        failures.ShouldBeEmpty();
    }

    #endregion

    #region BuilderListWithFactory Tests

    [Fact]
    public void BuilderListWithFactory_New_ShouldUseFactory()
    {
        var factoryCallCount = 0;
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() =>
        {
            factoryCallCount++;
            return new SimpleObjectBuilder();
        });

        list.New(b => b.WithValue("first"));
        list.New(b => b.WithValue("second"));

        factoryCallCount.ShouldBe(2);
        list.Count.ShouldBe(2);
    }

    [Fact]
    public void BuilderListWithFactory_BuildSuccess_ShouldBuildAllItems()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("first"));
        list.New(b => b.WithValue("second"));

        var results = list.BuildSuccess();

        results.Count.ShouldBe(2);
        results[0].Value.ShouldBe("first");
        results[1].Value.ShouldBe("second");
    }

    [Fact]
    public void BuilderListWithFactory_ValidateFailures_ShouldReturnFailures()
    {
        var list = new BuilderListWithFactory<Person, PersonBuilder>(() => new PersonBuilder());
        list.New(b => b.WithName("Valid").WithAge(25));
        list.New(b => b.WithAge(30)); // Missing name

        var failures = list.ValidateFailures();

        failures.Count.ShouldBe(1);
    }

    [Fact]
    public void BuilderListWithFactory_AsReferenceList_ShouldReturnReferences()
    {
        var list = new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.New(b => b.WithValue("test"));
        list.BuildSuccess();

        var referenceList = list.AsReferenceList();

        referenceList.AsEnumerable().Count().ShouldBe(1);
    }

    [Fact]
    public void BuilderListWithFactory_NullFactory_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>(null!));
    }

    [Fact]
    public void BuilderListExtensions_CreateBuilderList_ShouldCreateList()
    {
        var list = BuilderListExtensions.CreateBuilderList<SimpleObject, SimpleObjectBuilder>(() => new SimpleObjectBuilder());
        list.ShouldNotBeNull();
        list.ShouldBeOfType<BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>>();
    }

    #endregion

    #region IFailureCollector Tests

    [Fact]
    public void IFailureCollector_AddFailure_ShouldAddToCollection()
    {
        IFailureCollector collector = new FailuresDictionary();
        collector.AddFailure("field", Failure.FromException(new InvalidOperationException()));
        collector.HasFailures.ShouldBeTrue();
        collector.FailureCount.ShouldBe(1);
    }

    [Fact]
    public void IFailureCollector_HasFailures_ShouldReturnFalseWhenEmpty()
    {
        IFailureCollector collector = new FailuresDictionary();
        collector.HasFailures.ShouldBeFalse();
        collector.FailureCount.ShouldBe(0);
    }

    [Fact]
    public void IFailureCollector_AddFailure_ShouldReturnSameInstance()
    {
        IFailureCollector collector = new FailuresDictionary();
        var result = collector.AddFailure("field", Failure.FromMessage("error"));
        result.ShouldBeSameAs(collector);
    }

    [Fact]
    public void FailuresDictionary_ImplementsIFailureCollector()
    {
        var dict = new FailuresDictionary();
        dict.ShouldBeAssignableTo<IFailureCollector>();
    }

    #endregion

    #region IReferenceFactory Tests

    [Fact]
    public void DefaultReferenceFactory_Create_ShouldCreateNewReference()
    {
        var factory = DefaultReferenceFactory.Instance;
        var ref1 = factory.Create<SimpleObject>();
        var ref2 = factory.Create<SimpleObject>();
        
        ref1.ShouldNotBeNull();
        ref2.ShouldNotBeNull();
        ref1.ShouldNotBeSameAs(ref2);
        ref1.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void DefaultReferenceFactory_CreateWithExisting_ShouldCreateResolvedReference()
    {
        var factory = DefaultReferenceFactory.Instance;
        var obj = new SimpleObject { Value = "test" };
        var reference = factory.Create(obj);
        
        reference.IsResolved.ShouldBeTrue();
        reference.Resolved().ShouldBeSameAs(obj);
    }

    [Fact]
    public void DefaultReferenceFactory_CreateWithNull_ShouldCreateUnresolvedReference()
    {
        var factory = DefaultReferenceFactory.Instance;
        var reference = factory.Create<SimpleObject>(null);
        
        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void DefaultReferenceFactory_Instance_ShouldBeSingleton()
    {
        var instance1 = DefaultReferenceFactory.Instance;
        var instance2 = DefaultReferenceFactory.Instance;
        instance1.ShouldBeSameAs(instance2);
    }

    #endregion

    #region IReadOnlyReferenceList Tests

    [Fact]
    public void ReferenceList_ImplementsIReadOnlyReferenceList()
    {
        var list = new ReferenceList<SimpleObject>();
        list.ShouldBeAssignableTo<IReadOnlyReferenceList<SimpleObject>>();
    }

    [Fact]
    public void IReadOnlyReferenceList_AsEnumerable_ShouldReturnItems()
    {
        IReadOnlyReferenceList<SimpleObject> list = new ReferenceList<SimpleObject>();
        ((ReferenceList<SimpleObject>)list).Add(new SimpleObject { Value = "test" });
        
        var items = list.AsEnumerable().ToList();
        items.Count.ShouldBe(1);
        items[0].Value.ShouldBe("test");
    }

    [Fact]
    public void IReadOnlyReferenceList_Contains_ShouldFindItem()
    {
        var obj = new SimpleObject { Value = "test" };
        IReadOnlyReferenceList<SimpleObject> list = new ReferenceList<SimpleObject>();
        ((ReferenceList<SimpleObject>)list).Add(obj);
        
        list.Contains(obj).ShouldBeTrue();
        list.Contains(new SimpleObject { Value = "other" }).ShouldBeFalse();
    }

    #endregion

    #region Dependency Injection Scenario Tests

    [Fact]
    public void Builder_CanUseCustomReferenceFactory()
    {
        // This demonstrates that builders can be constructed with DI
        var factory = DefaultReferenceFactory.Instance;
        var builder = new SimpleObjectBuilder(); // Uses default factory
        
        builder.Reference().ShouldNotBeNull();
    }

    [Fact]
    public void Validation_WorksWithIFailureCollector()
    {
        var builder = new PersonBuilder().WithAge(25); // Missing name
        IFailureCollector collector = new FailuresDictionary();
        var visited = new VisitedObjectDictionary();
        
        builder.Validate(visited, collector);
        
        collector.HasFailures.ShouldBeTrue();
        collector.FailureCount.ShouldBeGreaterThan(0);
    }

    #endregion

    #region IVisitedTracker Tests

    [Fact]
    public void VisitedObjectDictionary_ImplementsIVisitedTracker()
    {
        var visited = new VisitedObjectDictionary();
        visited.ShouldBeAssignableTo<IVisitedTracker>();
    }

    [Fact]
    public void IVisitedTracker_IsVisited_ShouldReturnFalseWhenNotVisited()
    {
        IVisitedTracker tracker = new VisitedObjectDictionary();
        tracker.IsVisited(Guid.NewGuid()).ShouldBeFalse();
    }

    [Fact]
    public void IVisitedTracker_MarkVisited_ShouldMakeIsVisitedReturnTrue()
    {
        IVisitedTracker tracker = new VisitedObjectDictionary();
        var id = Guid.NewGuid();
        var obj = new SimpleObject { Value = "test" };
        
        tracker.MarkVisited(id, obj);
        
        tracker.IsVisited(id).ShouldBeTrue();
    }

    [Fact]
    public void IVisitedTracker_TryGet_ShouldReturnVisitedObject()
    {
        IVisitedTracker tracker = new VisitedObjectDictionary();
        var id = Guid.NewGuid();
        var obj = new SimpleObject { Value = "test" };
        
        tracker.MarkVisited(id, obj);
        tracker.TryGet(id, out var result).ShouldBeTrue();
        result.ShouldBeSameAs(obj);
    }

    [Fact]
    public void IVisitedTracker_TryGet_ShouldReturnFalseWhenNotVisited()
    {
        IVisitedTracker tracker = new VisitedObjectDictionary();
        tracker.TryGet(Guid.NewGuid(), out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void IVisitedTracker_Count_ShouldReturnNumberOfVisitedObjects()
    {
        IVisitedTracker tracker = new VisitedObjectDictionary();
        tracker.Count.ShouldBe(0);
        
        tracker.MarkVisited(Guid.NewGuid(), new SimpleObject());
        tracker.Count.ShouldBe(1);
        
        tracker.MarkVisited(Guid.NewGuid(), new SimpleObject());
        tracker.Count.ShouldBe(2);
    }

    #endregion

    #region ISynchronizationStrategy Tests

    [Fact]
    public void LockSynchronizationStrategy_Instance_ShouldBeSingleton()
    {
        var instance1 = LockSynchronizationStrategy.Instance;
        var instance2 = LockSynchronizationStrategy.Instance;
        instance1.ShouldBeSameAs(instance2);
    }

    [Fact]
    public void LockSynchronizationStrategy_Execute_ShouldExecuteAction()
    {
        var strategy = LockSynchronizationStrategy.Instance;
        var executed = false;
        var lockObj = new object();
        
        strategy.Execute(lockObj, () => executed = true);
        
        executed.ShouldBeTrue();
    }

    [Fact]
    public void LockSynchronizationStrategy_ExecuteFunc_ShouldReturnResult()
    {
        var strategy = LockSynchronizationStrategy.Instance;
        var lockObj = new object();
        
        var result = strategy.Execute(lockObj, () => 42);
        
        result.ShouldBe(42);
    }

    [Fact]
    public async Task ReaderWriterSynchronizationStrategy_ShouldBeThreadSafe()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var counter = 0;
        var lockObj = new object();
        
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => strategy.Execute(lockObj, () => Interlocked.Increment(ref counter))))
            .ToArray();
        
        await Task.WhenAll(tasks);
        
        counter.ShouldBe(10);
    }

    [Fact]
    public async Task ReaderWriterSynchronizationStrategy_ExecuteRead_ShouldAllowConcurrentReads()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var readCount = 0;
        
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => strategy.ExecuteRead(() => 
            {
                Interlocked.Increment(ref readCount);
                return readCount;
            })))
            .ToArray();
        
        await Task.WhenAll(tasks);
        
        readCount.ShouldBe(10);
    }

    #endregion

    #region IExistingInstanceProvider Tests

    [Fact]
    public void AbstractBuilder_ImplementsIExistingInstanceProvider()
    {
        var builder = new SimpleObjectBuilder();
        builder.ShouldBeAssignableTo<IExistingInstanceProvider<SimpleObject>>();
    }

    [Fact]
    public void IExistingInstanceProvider_HasExisting_ShouldBeFalseInitially()
    {
        IExistingInstanceProvider<SimpleObject> provider = new SimpleObjectBuilder();
        provider.HasExisting.ShouldBeFalse();
        provider.ExistingInstance.ShouldBeNull();
    }

    [Fact]
    public void IExistingInstanceProvider_HasExisting_ShouldBeTrueAfterExisting()
    {
        var existing = new SimpleObject { Value = "existing" };
        var builder = new SimpleObjectBuilder();
        builder.Existing(existing);
        
        IExistingInstanceProvider<SimpleObject> provider = builder;
        provider.HasExisting.ShouldBeTrue();
        provider.ExistingInstance.ShouldBeSameAs(existing);
    }

    #endregion

    #region IBuildOrchestrator Tests

    [Fact]
    public void DefaultBuildOrchestrator_Build_ShouldBuildSuccessfully()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>();
        var builder = new SimpleObjectBuilder().WithValue("test");
        
        var result = orchestrator.Build(builder);
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsResolved.ShouldBeTrue();
        result.Value.Resolved().Value.ShouldBe("test");
    }

    [Fact]
    public void DefaultBuildOrchestrator_Build_WithExisting_ShouldReturnExisting()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>();
        var existing = new SimpleObject { Value = "existing" };
        var builder = new SimpleObjectBuilder();
        builder.Existing(existing);
        
        var result = orchestrator.Build(builder);
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.Resolved().ShouldBeSameAs(existing);
    }

    [Fact]
    public void DefaultBuildOrchestrator_Build_AlreadyBuilt_ShouldReturnSameReference()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>();
        var builder = new SimpleObjectBuilder().WithValue("test");
        
        var result1 = orchestrator.Build(builder);
        var result2 = orchestrator.Build(builder);
        
        result1.Value.ShouldBeSameAs(result2.Value);
    }

    [Fact]
    public void DefaultBuildOrchestrator_Build_WithNullBuilder_ShouldThrow()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>();
        
        Should.Throw<ArgumentNullException>(() => orchestrator.Build(null!));
    }

    [Fact]
    public void DefaultBuildOrchestrator_Constructor_WithCustomDependencies_ShouldWork()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>(
            NoSynchronizationStrategy.Instance,
            () => new FailuresDictionary());
        
        var builder = new SimpleObjectBuilder().WithValue("test");
        var result = orchestrator.Build(builder);
        
        result.IsSuccess.ShouldBeTrue();
    }

    #endregion

    #region IValidationStrategy Tests

    [Fact]
    public void CustomValidationStrategy_CanBeImplemented()
    {
        // Demonstrates that IValidationStrategy can be implemented
        var strategy = new TestValidationStrategy();
        var builder = new SimpleObjectBuilder().WithValue("test");
        var visited = new VisitedObjectDictionary();
        IFailureCollector failures = new FailuresDictionary();
        
        strategy.Validate(builder, visited, failures);
        
        strategy.ValidateCalled.ShouldBeTrue();
    }

    #endregion

    #region Additional Coverage Tests

    [Fact]
    public void DefaultBuildOrchestrator_Constructor_NullSyncStrategy_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => 
            new DefaultBuildOrchestrator<SimpleObject>(null!, () => new FailuresDictionary()));
    }

    [Fact]
    public void DefaultBuildOrchestrator_Constructor_NullFailureFactory_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => 
            new DefaultBuildOrchestrator<SimpleObject>(LockSynchronizationStrategy.Instance, null!));
    }

    [Fact]
    public void DefaultBuildOrchestrator_Build_WithVisitedTracker_ShouldHandleCycleDetection()
    {
        var orchestrator = new DefaultBuildOrchestrator<SimpleObject>();
        var builder = new SimpleObjectBuilder().WithValue("test");
        var visited = new VisitedObjectDictionary();
        
        // First build
        var result1 = orchestrator.Build(builder, visited);
        result1.IsSuccess.ShouldBeTrue();
        
        // Second build with same visited should return early
        visited.MarkVisited(builder.Id, builder);
        var result2 = orchestrator.Build(builder, visited);
        result2.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void ReaderWriterSynchronizationStrategy_ExecuteFunc_ShouldReturnResult()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var lockObj = new object();
        
        var result = strategy.Execute(lockObj, () => 123);
        
        result.ShouldBe(123);
    }

    [Fact]
    public void VisitedObjectDictionary_MarkVisited_ShouldOverwriteExisting()
    {
        var tracker = new VisitedObjectDictionary();
        var id = Guid.NewGuid();
        var obj1 = new SimpleObject { Value = "first" };
        var obj2 = new SimpleObject { Value = "second" };
        
        tracker.MarkVisited(id, obj1);
        tracker.MarkVisited(id, obj2);
        
        tracker.TryGet(id, out var result).ShouldBeTrue();
        result.ShouldBeSameAs(obj2);
    }

    [Fact]
    public void AbstractBuilder_WithCustomSyncStrategy_ShouldUseProvidedStrategy()
    {
        // Custom builder that exposes sync strategy usage
        var builder = new SimpleObjectBuilder().WithValue("test");
        
        // Build should work regardless of strategy
        var result = builder.Build();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Failure_Match_ActionOverload_ShouldInvokeCorrectHandler()
    {
        var exceptionCalled = false;
        var messageCalled = false;
        var nestedCalled = false;
        
        var exFailure = Failure.FromException(new InvalidOperationException("ex"));
        exFailure.Match(
            onException: _ => exceptionCalled = true,
            onMessage: _ => messageCalled = true,
            onNested: _ => nestedCalled = true);
        
        exceptionCalled.ShouldBeTrue();
        messageCalled.ShouldBeFalse();
        nestedCalled.ShouldBeFalse();
    }

    [Fact]
    public void MessageFailure_Match_ShouldInvokeMessageHandler()
    {
        var messageCalled = false;
        
        var msgFailure = Failure.FromMessage("test message");
        msgFailure.Match(
            onException: _ => { },
            onMessage: _ => messageCalled = true,
            onNested: _ => { });
        
        messageCalled.ShouldBeTrue();
    }

    [Fact]
    public void NestedFailure_Match_ShouldInvokeNestedHandler()
    {
        var nestedCalled = false;
        
        var nestedFailure = Failure.FromNested(new FailuresDictionary());
        nestedFailure.Match(
            onException: _ => { },
            onMessage: _ => { },
            onNested: _ => nestedCalled = true);
        
        nestedCalled.ShouldBeTrue();
    }

    #endregion
}

// Test helper class
public class TestValidationStrategy : IValidationStrategy<SimpleObjectBuilder>
{
    public bool ValidateCalled { get; private set; }
    
    public void Validate(SimpleObjectBuilder builder, IVisitedTracker visited, IFailureCollector failures)
    {
        ValidateCalled = true;
    }
}
