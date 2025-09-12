# `FrenchExDev.Net.CSharp.Object.Builder`

A small library to build object graphs safely and predictably, with:
- A consistent result type (success/failure) for sync and async builders
- Aggregated validation errors (ExceptionBuildList)
- Cycle detection (VisitedObjectsList)
- Ready-to-use base classes (`AbstractObjectBuilder` / `AbstractAsyncObjectBuilder`)
- Lambda-based builders for quick prototypes
- A workflow-style builder for step pipelines
- Testing helpers (BuilderTester)

## Core concepts

- IObjectBuilder<TClass>, IAsyncObjectBuilder<TClass>
  Contracts to build an instance of TClass either synchronously or asynchronously.

- IObjectBuildResult<TClass>
  Discriminated result. Use SuccessObjectBuildResult<TClass> or FailureObjectBuildResult<TClass, TBuilder>/FailureAsyncObjectBuildResult<TClass, TBuilder>.

- ExceptionBuildList
  A list collecting validation or build exceptions. Prefer accumulating exceptions over throwing at the first error.

- VisitedObjectsList
  Tracks visited nodes to prevent infinite recursion in graphs (e.g., a person who knows a person that knows the first person).

- AbstractObjectBuilder<TClass, TBuilder>
  Base class for sync builders. Implement BuildInternal to create the object and return a Success/Failure result.

- AbstractAsyncObjectBuilder<TClass, TBuilder>
  Base class for async builders. Implement BuildInternalAsync and return a task with Success/Failure.

- LambdaObjectBuilder / LambdaAsyncObjectBuilder
  Light wrappers that let you implement builders with a delegate instead of subclassing.

- WorkflowObjectBuilder<TClass, TBuilder>
  Compose a sequence of steps (IAbstractStep<TClass>) executed in order to build TClass.

- BuildReference / AbstractBuildReference<T>
  Hold a reference to a built instance and queue actions to run once that reference is set (useful for deferred wiring).

Notes
- Base classes handle visited list creation and reuse. Always use the provided VisitedObjectsList parameter in your overrides and helpers.
- Builders are not thread-safe by default.

## Quick start (sync): Lambda builder

Example domain types

```csharp
public record Address(string Street, string ZipCode);
public record Person(string Name, int Age, IEnumerable<Address> Addresses, IEnumerable<Person> Knows);
```

Create a lambda-based builder for Person

```csharp
internal sealed class PersonBuilder : LambdaObjectBuilder<Person, PersonBuilder>
{
    public const string ErrorInvalidAge = "Invalid age";

    public PersonBuilder(
        Func<PersonBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<Person>> build
    ) : base(build) { }
}
```

Use it

```csharp
var builder = new PersonBuilder((self, exceptions, visited) =>
{
    // Validate
    var age = 30;
    if (age < 0)
    {
        exceptions.Add(new ArgumentOutOfRangeException(nameof(age), PersonBuilder.ErrorInvalidAge));
    }

    // ... other validations

    if (exceptions.Count > 0)
    {
        return new FailureObjectBuildResult<Person, PersonBuilder>(self, exceptions, visited);
    }

    // Build
    var person = new Person(
        name: "Alice",
        age: age,
        addresses: new[] { new Address("123 Main St", "12345") },
        knows: Array.Empty<Person>()
    );

    return SuccessObjectBuildResult<Person>.Success(person);
});

var result = builder.Build();
if (result is SuccessObjectBuildResult<Person> ok)
{
    Console.WriteLine(ok.Result.Name); // Alice
}
else if (result is FailureObjectBuildResult<Person, PersonBuilder> ko)
{
    foreach (var ex in ko.Exceptions) Console.Error.WriteLine(ex.Message);
}
```

## Classic builder (sync): subclass AbstractObjectBuilder

```csharp
internal sealed class AddressBuilder : AbstractObjectBuilder<Address, AddressBuilder>
{
    private string? _street;
    private string? _zip;

    public AddressBuilder Street(string value) { _street = value; return this; }
    public AddressBuilder ZipCode(string value) { _zip = value; return this; }

    protected override IObjectBuildResult<Address> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrWhiteSpace(_street))
            exceptions.Add(new ArgumentException("Invalid street"));
        if (string.IsNullOrWhiteSpace(_zip))
            exceptions.Add(new ArgumentException("Invalid zip code"));

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        return Success(new Address(_street!, _zip!));
    }
}
```

```csharp
internal sealed class PersonBuilder : AbstractObjectBuilder<Person, PersonBuilder>
{
    private string? _name;
    private int? _age;
    private readonly List<AddressBuilder> _addresses = new();
    private readonly List<PersonBuilder> _knows = new();

    public PersonBuilder Name(string? value) { _name = value; return this; }
    public PersonBuilder Age(int? value) { _age = value; return this; }
    public PersonBuilder Address(Action<AddressBuilder> body)
    {
        var ab = new AddressBuilder();
        body(ab);
        _addresses.Add(ab);
        return this;
    }
    public PersonBuilder Knows(Action<PersonBuilder> body)
    {
        var pb = new PersonBuilder();
        body(pb);
        _knows.Add(pb);
        return this;
    }

    protected override IObjectBuildResult<Person> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrWhiteSpace(_name))
            exceptions.Add(new ArgumentException("Invalid name"));
        if (_age is null or < 0)
            exceptions.Add(new ArgumentOutOfRangeException(nameof(_age), "Invalid age"));

        // Build children and collect failures
        var addressResults = BuildList<Address, AddressBuilder>(_addresses, visited);
        AddExceptions<Address, AddressBuilder>(addressResults, exceptions);

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        var addresses = addressResults
            .OfType<SuccessObjectBuildResult<Address>>()
            .Select(r => r.Result)
            .ToArray();

        var knows = _knows
            .Select(b => b.Build(visited))
            .OfType<SuccessObjectBuildResult<Person>>()
            .Select(r => r.Result)
            .ToArray();

        return Success(new Person(_name!, _age!.Value, addresses, knows));
    }
}
```

Usage

```csharp
var result = new PersonBuilder()
    .Name("Bob")
    .Age(42)
    .Address(a => a.Street("42 Galaxy Rd").ZipCode("424242"))
    .Build();
```

## Asynchronous builder: subclass AbstractAsyncObjectBuilder

```csharp
internal sealed class AsyncAddressBuilder : AbstractAsyncObjectBuilder<Address, AsyncAddressBuilder>
{
    private string? _street;
    private string? _zip;

    public AsyncAddressBuilder Street(string value) { _street = value; return this; }
    public AsyncAddressBuilder ZipCode(string value) { _zip = value; return this; }

    protected override Task<IObjectBuildResult<Address>> BuildInternalAsync(
        ExceptionBuildList exceptions,
        VisitedObjectsList visited,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(_street)) exceptions.Add(new ArgumentException("Invalid street"));
        if (string.IsNullOrWhiteSpace(_zip)) exceptions.Add(new ArgumentException("Invalid zip code"));

        return exceptions.Count > 0
            ? Task.FromResult<IObjectBuildResult<Address>>(AsyncFailureResult(exceptions, visited))
            : Task.FromResult<IObjectBuildResult<Address>>(SuccessObjectBuildResult<Address>.Success(new Address(_street!, _zip!)));
    }
}
```

```csharp
internal sealed class AsyncPersonBuilder : AbstractAsyncObjectBuilder<Person, AsyncPersonBuilder>
{
    private string? _name;
    private int? _age;
    private readonly List<AsyncAddressBuilder> _addresses = new();

    public AsyncPersonBuilder Name(string? value) { _name = value; return this; }
    public AsyncPersonBuilder Age(int? value) { _age = value; return this; }
    public AsyncPersonBuilder Address(Action<AsyncAddressBuilder> body)
    {
        var ab = new AsyncAddressBuilder();
        body(ab);
        _addresses.Add(ab);
        return this;
    }

    protected override async Task<IObjectBuildResult<Person>> BuildInternalAsync(
        ExceptionBuildList exceptions,
        VisitedObjectsList visited,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(_name)) exceptions.Add(new ArgumentException("Invalid name"));
        if (_age is null or < 0) exceptions.Add(new ArgumentOutOfRangeException(nameof(_age), "Invalid age"));

        // In async scenarios you can await child builds and aggregate exceptions
        var addressTasks = _addresses.Select(a => a.BuildAsync(visited, cancellationToken));
        var results = await Task.WhenAll(addressTasks);
        foreach (var r in results)
        {
            if (r is FailureAsyncObjectBuildResult<Address, AsyncAddressBuilder> f)
                exceptions.AddRange(f.Exceptions);
        }

        if (exceptions.Count > 0)
            return AsyncFailureResult(exceptions, visited);

        var addresses = results
            .OfType<SuccessObjectBuildResult<Address>>()
            .Select(r => r.Result)
            .ToArray();

        return SuccessObjectBuildResult<Person>.Success(new Person(_name!, _age!.Value, addresses, Array.Empty<Person>()));
    }
}
```

Usage

```csharp
var result = await new AsyncPersonBuilder()
    .Name("Carol")
    .Age(25)
    .Address(a => a.Street("Avenue Q").ZipCode("10000"))
    .BuildAsync();
```

## Workflow builder

```csharp
public interface IAbstractStep<T>
{
    Task ExecuteAsync(WorkflowContext<T> ctx, CancellationToken ct);
}

public sealed class WorkflowObjectBuilder<T, TBuilder> : IAsyncObjectBuilder<T>
    where TBuilder : IAsyncObjectBuilder<T>
{
    // Provided by the library
}
```

Example

```csharp
var wf = new WorkflowObjectBuilder<Person, WorkflowObjectBuilder<Person, IAsyncObjectBuilder<Person>>>();
wf.Step(new InitPersonStep())
  .Step(new ValidateStep())
  .Step(new FillAddressesStep());

var result = await wf.BuildAsync();
```

Remarks
- Steps run in the order they are added.
- Use the builder’s Exceptions/Visited context to coordinate across steps.

## Deferred wiring with BuildReference

When relationships require the final instance, defer the wiring until the reference exists.

```csharp
var personRef = new BuildReference<Person, PersonBuilder>(reference: null);

personRef.AddAction(p => Console.WriteLine($"Built: {p.Name}"));

// Later, once you have the built instance
personRef.SetReference(new Person("Dave", 33, Array.Empty<Address>(), Array.Empty<Person>()));
// The queued action runs now
```

Notes
- Actions execute in the order they were added.
- If an action throws, execution stops and the exception bubbles up.
- Adding an action when a reference is already set will run on the next SetReference call.

## Testing builders with BuilderTester

Validate success

```csharp
await BuilderTester.TestValid<PersonBuilder, Person>(
    builderFactory: () => new PersonBuilder((b, ex, vis) =>
        SuccessObjectBuildResult<Person>.Success(new Person("foo", 30, new[] { new Address("123 Main St", "12345") }, Array.Empty<Person>()))
    ),
    body: b => { /* setup \/ mutate */ },
    asserts: p =>
    {
        p.Name.ShouldBe("foo");
        p.Age.ShouldBe(30);
        p.Addresses.Count().ShouldBe(1);
    }
);
```

Validate failure (sync)

```csharp
BuilderTester.Invalid<PersonBuilder, Person>(
    builderFactory: () => new PersonBuilder((b, ex, vis) =>
    {
        ex.Add(new ArgumentException("Invalid age"));
        return new FailureObjectBuildResult<Person, PersonBuilder>(b, ex, vis);
    }),
    body: _ => { },
    assert: failure => failure.Exceptions.ShouldNotBeEmpty()
);
```

Validate failure (async)

```csharp
await BuilderTester.InvalidAsync<AsyncPersonBuilder, Person>(
    builderFactory: () => new AsyncPersonBuilder(),
    body: async (b, ct) => { await Task.CompletedTask; },
    assert: failure => failure.Exceptions.ShouldNotBeEmpty()
);
```

## Implementation guidelines

- Prefer accumulating errors in ExceptionBuildList; return a failure at the end of BuildInternal/BuildInternalAsync.
- Use the provided Failure/Success helpers in base classes where available.
- Always respect the visited parameter; pass it to child builders to avoid infinite recursion.
- Keep builders side-effect free; do not mutate external state during build.
- Do not assume thread-safety.
- In async builders, propagate CancellationToken to all nested async calls.
- For collection children, build all, collect failures, short-circuit to failure when any child fails.
- For lambda builders, ensure the delegate returns a Failure result when exceptions are present.

## Troubleshooting

- Getting cycles? Ensure all nested Build/BuildAsync calls pass along the same VisitedObjectsList.
- Missing data in success results? Confirm that you only return Success when exceptions.Count == 0.
- Exceptions not visible? Add them to ExceptionBuildList instead of throwing immediately.
