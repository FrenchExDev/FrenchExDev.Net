# FrenchExDev.Net.CSharp.Object.Builder2

A robust, thread-safe implementation of the **Builder Pattern** for .NET, designed to construct complex object graphs with validation, circular reference support, and deferred resolution.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Concepts](#core-concepts)
- [Working with Results](#working-with-results)
- [Circular References](#circular-references)
- [API Reference](#api-reference)
- [Advanced Usage](#advanced-usage)
- [Thread Safety](#thread-safety)
- [Testing](#testing)
- [Architecture](#architecture)

## Overview

Builder2 provides a powerful abstraction for constructing complex object graphs in .NET applications. It addresses common challenges like:

- **Validation before construction** - Ensure all required data is present
- **Circular references** - Handle bidirectional relationships between objects
- **Thread safety** - Safe concurrent builds of the same builder
- **Deferred resolution** - Reference objects before they're built
- **Fluent API** - Clean, readable builder configuration
- **Result Pattern** - No exceptions for expected failures

## Features

| Feature | Description |
|---------|-------------|
| 🔨 **Fluent Builder API** | Chain configuration methods for readable code |
| ✅ **Built-in Validation** | Validate before building with detailed failure collection |
| 🔄 **Circular Reference Support** | Handle bidirectional relationships automatically |
| 🔒 **Thread-Safe** | Concurrent builds produce consistent results |
| 📦 **Deferred Resolution** | Reference objects that haven't been built yet |
| 🎯 **Type-Safe** | Full generic support with compile-time safety |
| 🧩 **Extensible** | Custom synchronization and reference strategies |
| 📋 **Result Pattern** | Explicit success/failure handling without exceptions |
| 🚂 **Railway Oriented** | Composable operations with `Map`, `Bind`, and `Match` |

## Installation

```xml
<PackageReference Include="FrenchExDev.Net.CSharp.Object.Builder2" Version="1.0.0" />
```

**Dependencies:**
- .NET 10.0+
- FrenchExDev.Net.CSharp.Object.Result2

## Quick Start

### 1. Define Your Domain Model

```csharp
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Address? Address { get; set; }
    public List<Person> Friends { get; set; } = [];
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
```

### 2. Create a Builder

```csharp
public class PersonBuilder : AbstractBuilder<Person>
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public AddressBuilder? AddressBuilder { get; set; }
    public BuilderList<Person, PersonBuilder> Friends { get; } = [];

    // Required: Define how to create the instance
    protected override Person Instantiate() => new()
    {
        Name = Require(Name),  // Safe access - throws if null
        Age = Age,
        Address = AddressBuilder?.Reference().ResolvedOrNull(),
        Friends = [.. Friends.AsReferenceList().AsEnumerable()]
    };

    // Optional: Define validation rules
    protected override void ValidateInternal(
        VisitedObjectDictionary visited, 
        IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, 
            n => new ArgumentException($"{n} is required"));
        Assert(() => Age < 0, nameof(Age), failures, 
            n => new ArgumentOutOfRangeException(n));
    }

    // Optional: Build child objects
    protected override void BuildInternal(VisitedObjectDictionary visited)
    {
        if (AddressBuilder is not null)
            BuildChild(AddressBuilder, visited);
        BuildList<PersonBuilder, Person>(Friends, visited);
    }

    // Fluent API methods
    public PersonBuilder WithName(string name) { Name = name; return this; }
    public PersonBuilder WithAge(int age) { Age = age; return this; }
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
```

### 3. Use the Builder

```csharp
// Build and handle with functional style
new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .WithAddress(a => a.WithStreet("123 Main St").WithCity("Paris"))
    .Build()
    .Map(r => r.Resolved())
    .Match(
        onSuccess: person => Console.WriteLine($"Created: {person.Name}"),
        onFailure: ex => Console.WriteLine($"Failed: {ex.Message}")
    );

// Build with nested objects
new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .WithFriend(f => f.WithName("Bob").WithAge(25))
    .WithFriend(f => f.WithName("Charlie").WithAge(28))
    .Build()
    .Map(r => r.Resolved())
    .IfSuccess(person => Console.WriteLine($"{person.Name} has {person.Friends.Count} friends"));
```

## Core Concepts

### AbstractBuilder

The base class for all builders:

```csharp
public abstract class AbstractBuilder<TClass> : IBuilder<TClass>
{
    protected abstract TClass Instantiate();
    protected virtual void ValidateInternal(...) { }
    protected virtual void BuildInternal(...) { }
}
```

#### Key Methods

| Method | Description |
|--------|-------------|
| `Build()` | Returns `Result<Reference<TClass>>` - success with reference or failure with errors |
| `Validate()` | Validates without building |
| `Reference()` | Gets the deferred reference (can be obtained before building) |
| `Existing()` | Use an existing instance instead of building |

### Reference

A thread-safe wrapper for deferred object resolution:

```csharp
var builder = new PersonBuilder().WithName("Alice").WithAge(30);

// Get reference BEFORE building
var reference = builder.Reference();
Console.WriteLine(reference.IsResolved); // false

// Build resolves the reference
builder.Build();
Console.WriteLine(reference.IsResolved); // true
Console.WriteLine(reference.Resolved().Name); // "Alice"
```

| Method | Description |
|--------|-------------|
| `Resolved()` | Returns instance or throws `ReferenceNotResolvedException` |
| `ResolvedOrNull()` | Returns instance or `null` |
| `IsResolved` | Whether the reference has been resolved |
| `OnResolve(action)` | Register callback for when resolved |

### Validation

Built-in validation assertions:

```csharp
protected override void ValidateInternal(
    VisitedObjectDictionary visited, 
    IFailureCollector failures)
{
    AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, 
        n => new ArgumentException(n));
    AssertNotNull(RequiredObject, nameof(RequiredObject), failures,
        n => new ArgumentNullException(n));
    AssertNotNullNotEmptyCollection(Items, nameof(Items), failures,
        n => new ArgumentException(n));
    Assert(() => Age < 0, nameof(Age), failures,
        n => new ArgumentOutOfRangeException(n));
    
    // Validate nested builders
    AddressBuilder?.Validate(visited, failures);
    ValidateListInternal(Friends, nameof(Friends), visited, failures);
}
```

### Required Properties in Instantiate

Use the `Require` helper method to safely access required properties in `Instantiate()`. If a property is null, it throws `RequiredPropertyNotSetException` with the property name automatically captured:

```csharp
public class OrderBuilder : AbstractBuilder<Order>
{
    public string? CustomerId { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }

    protected override Order Instantiate() => new()
    {
        CustomerId = Require(CustomerId),  // Throws if null
        Quantity = Require(Quantity),       // Works with nullable value types
        Price = Require(Price)
    };

    // Fluent API...
}

// Usage - throws RequiredPropertyNotSetException with PropertyName = "CustomerId"
var builder = new OrderBuilder().WithQuantity(10).WithPrice(99.99m);
builder.Build(); // Throws: "Required property 'CustomerId' was not set before instantiation."
```

The `Require` method:
- Works with both reference types and nullable value types
- Automatically captures the property name using `CallerArgumentExpression`
- Throws `RequiredPropertyNotSetException` (inherits from `InvalidOperationException`)
- Provides clear error messages for debugging

## Working with Results

Builder2 returns `Result<Reference<TClass>>` which integrates with Result2 for functional error handling.

### Basic Result Handling

```csharp
var result = new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .Build();

// Option 1: Check IsSuccess
if (result.IsSuccess)
{
    var person = result.Value.Resolved();
    Console.WriteLine(person.Name);
}

// Option 2: TryGetSuccessValue
if (result.TryGetSuccessValue(out var reference))
{
    Console.WriteLine(reference.Resolved().Name);
}

// Option 3: TryGetException
if (result.TryGetException<BuildFailureException>(out var failure))
{
    foreach (var (field, errors) in failure.Failures)
        Console.WriteLine($"{field}: {errors.Count} errors");
}
```

### Functional Composition

#### Map - Transform Values

```csharp
var nameResult = new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .Build()
    .Map(reference => reference.Resolved())  // Result<Reference<Person>> → Result<Person>
    .Map(person => person.Name);              // Result<Person> → Result<string>

nameResult.IfSuccess(name => Console.WriteLine(name)); // "Alice"
```

#### Bind - Chain Fallible Operations

```csharp
Result<Person> ValidatePerson(Person p) =>
    p.Age >= 0 ? Result<Person>.Success(p) 
               : Result<Person>.Failure(new ArgumentException("Invalid age"));

Result<string> GetGreeting(Person p) =>
    Result<string>.Success($"Hello, {p.Name}!");

var greeting = new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .Build()
    .Map(r => r.Resolved())
    .Bind(ValidatePerson)
    .Bind(GetGreeting);

greeting.IfSuccess(msg => Console.WriteLine(msg)); // "Hello, Alice!"
```

#### Match - Handle Both Cases

```csharp
new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .Build()
    .Map(r => r.Resolved())
    .Match(
        onSuccess: person => 
        {
            Console.WriteLine($"Built: {person.Name}");
            SaveToDatabase(person);
        },
        onFailure: ex => 
        {
            Logger.Error("Build failed", ex);
        }
    );
```

#### IfSuccess / IfException

```csharp
builder.Build()
    .Map(r => r.Resolved())
    .IfSuccess(person => Console.WriteLine(person.Name))
    .IfException(ex => Logger.Warn(ex.Message));

// Typed exception handling
result.IfException<BuildFailureException>(ex => 
{
    foreach (var (field, failures) in ex.Failures)
        Console.WriteLine($"Error on {field}");
});
```

### Async Operations

```csharp
var result = await new PersonBuilder()
    .WithName("Alice")
    .WithAge(30)
    .Build()
    .MapAsync(async r => 
    {
        var person = r.Resolved();
        await _repository.SaveAsync(person);
        return person;
    })
    .BindAsync(async person => 
    {
        var enriched = await _enrichmentService.EnrichAsync(person);
        return enriched is not null
            ? Result<Person>.Success(enriched)
            : Result<Person>.Failure(new Exception("Enrichment failed"));
    });

await result.MatchAsync(
    onSuccess: async person => await NotifyAsync(person),
    onFailure: async ex => await LogErrorAsync(ex)
);
```

### Processing Pipeline Example

```csharp
public async Task<Result<OrderConfirmation>> ProcessOrderAsync(OrderRequest request)
{
    return await new OrderBuilder()
        .WithCustomerId(request.CustomerId)
        .WithItems(request.Items)
        .Build()
        .Map(r => r.Resolved())
        .BindAsync(ValidateOrderAsync)
        .BindAsync(CheckInventoryAsync)
        .BindAsync(ProcessPaymentAsync)
        .MapAsync(async order =>
        {
            await _repository.SaveAsync(order);
            return new OrderConfirmation(order.Id, order.Total);
        });
}
```

### Result Methods Reference

| Method | Description |
|--------|-------------|
| `IsSuccess` | Boolean indicating success/failure |
| `Value` | Gets success value (throws if failure) |
| `TryGetSuccessValue(out T)` | Safe value access |
| `TryGetException<T>(out T)` | Safe typed exception access |
| `Map<T>(Func)` | Transform success value |
| `MapAsync<T>(Func)` | Async transformation |
| `Bind<T>(Func)` | Chain result-producing operations |
| `BindAsync<T>(Func)` | Async chaining |
| `Match(onSuccess, onFailure)` | Handle both cases |
| `MatchAsync(...)` | Async match |
| `IfSuccess(Action)` | Handle success only |
| `IfException(Action)` | Handle failure only |
| `IfException<T>(Action)` | Typed failure handler |

## Circular References

Handle bidirectional relationships using `Reference<T>`:

```csharp
public class Department
{
    public string Name { get; set; } = string.Empty;
    public List<Employee> Employees { get; set; } = [];
}

public class Employee
{
    public string Name { get; set; } = string.Empty;
    public Department? Department { get; set; }
}

public class EmployeeBuilder : AbstractBuilder<Employee>
{
    public string? Name { get; set; }
    public Reference<Department>? DepartmentRef { get; set; }
    
    protected override Employee Instantiate() => new()
    {
        Name = Require(Name),
        Department = DepartmentRef?.ResolvedOrNull()
    };
    
    public EmployeeBuilder WithName(string name) { Name = name; return this; }
    public EmployeeBuilder WithDepartment(Reference<Department> dept)
    {
        DepartmentRef = dept;
        return this;
    }
}

// Usage
var deptBuilder = new DepartmentBuilder().WithName("Engineering");
deptBuilder
    .WithEmployee(e => e.WithName("Alice").WithDepartment(deptBuilder.Reference()))
    .WithEmployee(e => e.WithName("Bob").WithDepartment(deptBuilder.Reference()));

deptBuilder.Build()
    .Map(r => r.Resolved())
    .IfSuccess(dept => 
    {
        Console.WriteLine(dept.Employees[0].Department == dept); // true
    });
```

**How it works:**
1. `deptBuilder.Reference()` returns an unresolved reference
2. Employees store this reference during configuration
3. During build, `ResolvedOrNull()` returns `null` (not yet resolved)
4. After department instantiation, `Resolve()` is called
5. All employee references now point to the built department

## API Reference

### Interfaces

| Interface | Description |
|-----------|-------------|
| `IBuilder<T>` | Main builder contract |
| `IBuildable<T>` | Can build objects (returns `Result<Reference<T>>`) |
| `IValidatable` | Can be validated |
| `IReferenceable<T>` | Provides references |

### Classes

| Class | Description |
|-------|-------------|
| `AbstractBuilder<T>` | Base class for builders |
| `Reference<T>` | Deferred reference wrapper |
| `BuilderList<T, TBuilder>` | List of builders |
| `BuilderListWithFactory<T, TBuilder>` | List with custom factory |
| `FailuresDictionary` | Validation failure collector |

### Synchronization Strategies

| Strategy | Use Case |
|----------|----------|
| `LockSynchronizationStrategy` | Default, general purpose |
| `NoSynchronizationStrategy` | Single-threaded scenarios |
| `ReaderWriterSynchronizationStrategy` | Read-heavy scenarios |

## Advanced Usage

### Custom Synchronization

```csharp
public class MyBuilder : AbstractBuilder<MyClass>
{
    public MyBuilder() 
        : base(DefaultReferenceFactory.Instance, NoSynchronizationStrategy.Instance)
    { }
}
```

### Using Existing Instances

```csharp
var existing = new Person { Name = "Alice", Age = 30 };
var builder = new PersonBuilder().Existing(existing);

builder.Build()
    .Map(r => r.Resolved())
    .IfSuccess(person => Console.WriteLine(person == existing)); // true
```

### Builder Lists with Factory

```csharp
var list = new BuilderListWithFactory<Person, PersonBuilder>(
    () => serviceProvider.GetRequiredService<PersonBuilder>());

list.New(b => b.WithName("Alice"));
list.New(b => b.WithName("Bob"));
```

## Thread Safety

```csharp
var builder = new PersonBuilder().WithName("Alice").WithAge(30);

var tasks = Enumerable.Range(0, 100)
    .Select(_ => Task.Run(() => builder.Build()))
    .ToArray();

await Task.WhenAll(tasks);

// All tasks receive the same instance
var people = tasks
    .Select(t => t.Result)
    .Where(r => r.IsSuccess)
    .Select(r => r.Value.Resolved())
    .Distinct();

Console.WriteLine(people.Count()); // 1
```

### Guarantees

- ✅ `Build()` only instantiates once
- ✅ `Validate()` only validates once
- ✅ `Reference.Resolve()` is atomic
- ✅ Status reads are volatile
- ✅ Callbacks execute exactly once

## Testing

### Test Coverage: 95.4%

| Component | Coverage |
|-----------|----------|
| AbstractBuilder | 90.9% |
| BuilderList | 100% |
| Reference | 100% |
| Validation | 100% |

### Running Tests

```bash
cd CSharp.Object.Builder2
dotnet test
```

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        IBuilder<T>                          │
│  ┌─────────────┐ ┌────────────┐ ┌──────────────┐            │
│  │ IBuildable  │ │IValidatable│ │IReferenceable│            │
│  └─────────────┘ └────────────┘ └──────────────┘            │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌──────────────────────────────────────────────────────────────┐
│                   AbstractBuilder<T>                         │
│  ┌───────────────────────────────────────────────────────┐   │
│  │ Build() → Validate() → BuildInternal() → Instantiate()│   │
│  │ Returns Result<Reference<T>> for deferred resolution  │   │
│  └───────────────────────────────────────────────────────┘   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐     │
│  │ Reference<T>│ │ BuildStatus │ │ValidationStatus     │     │
│  └─────────────┘ └─────────────┘ └─────────────────────┘     │
└──────────────────────────────────────────────────────────────┘
                            │
              ┌─────────────┼─────────────┐
              ▼             ▼             ▼
     ┌─────────────┐ ┌───────────┐ ┌─────────────┐
     │BuilderList  │ │ Failure   │ │ISyncStrategy│
     │<T,TBuilder> │ │Dictionary │ │             │
     └─────────────┘ └───────────┘ └─────────────┘
```

## License

MIT License - see [LICENSE.md](LICENSE.md)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Write tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

---

**Made with ❤️ by FrenchExDev**
