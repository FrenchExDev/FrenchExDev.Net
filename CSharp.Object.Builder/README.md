# `FrenchExDev.Net.CSharp.Object.Builder`

A lightweight .NET library implementing the Builder pattern with built-in validation, deferred reference resolution, and failure tracking. Build complex object graphs with validation at construction time.

## 📦 Project Structure

```
CSharp.Object.Builder/
├── src/
│   ├── FrenchExDev.Net.CSharp.Object.Builder/          # Core library
│   └── FrenchExDev.Net.CSharp.Object.Builder.Testing/  # Testing utilities
└── test/
    └── FrenchExDev.Net.CSharp.Object.Builder.Tests/    # Unit tests
```

## 🎯 Features

- **`AbstractBuilder<T>`** - Base class for implementing type-safe builders with validation
- **`Reference<T>`** - Deferred reference resolution for handling circular dependencies
- **`ReferenceList<T>`** - Collection of references with LINQ support
- **`BuilderList<T, TBuilder>`** - Batch building and validation of multiple objects
- **`FailuresDictionary`** - Structured collection of validation failures per member
- **Validation helpers** - Built-in assertions for common validation scenarios
- **Cycle detection** - Automatic handling of circular references during build

## 🚀 Usage

### Creating a Builder

```csharp
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Person? Spouse { get; set; }
}

public class PersonBuilder : AbstractBuilder<Person>
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public PersonBuilder? Spouse { get; set; }

    protected override void ValidateInternal(
        VisitedObjectDictionary visitedCollector, 
        FailuresDictionary failures)
    {
        // Validate required fields
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, 
            n => new StringIsEmptyOrWhitespaceException(n));
        
        AssertNotNull(Age, nameof(Age), failures, 
            n => new ArgumentNullException(n));
        
        // Validate related objects
        Spouse?.Validate(visitedCollector, failures);
    }

    protected override Person Instantiate() => new()
    {
        Name = Name!,
        Age = Age!.Value,
        Spouse = Spouse?.Reference().ResolvedOrNull()
    };

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // Build related objects first
        Spouse?.Build(visitedCollector);
    }
}
```

### Building Objects

```csharp
// Simple build
var builder = new PersonBuilder
{
    Name = "John",
    Age = 30
};

var result = builder.Build();

if (result.IsSuccess<Person>())
{
    Person person = result.Success<Person>();
    Console.WriteLine($"Created: {person.Name}");
}

// Using BuildSuccess (throws on failure)
Person person = builder.BuildSuccess();
```

### Handling Build Failures

```csharp
var builder = new PersonBuilder
{
    Name = "",  // Invalid: empty
    Age = null  // Invalid: null
};

var result = builder.Build();

if (result.IsFailure())
{
    var failures = result.Failures<Person>();
    
    foreach (var (member, errors) in failures)
    {
        Console.WriteLine($"{member}:");
        foreach (var error in errors)
        {
            Console.WriteLine($"  - {error.Value}");
        }
    }
}
```

### Using References for Circular Dependencies

```csharp
// Create builders with circular reference
var john = new PersonBuilder { Name = "John", Age = 30 };
var jane = new PersonBuilder { Name = "Jane", Age = 28 };

// Set up circular reference (spouses reference each other)
john.Spouse = jane;
jane.Spouse = john;

// Build handles the cycle automatically
var result = john.Build();
Person johnPerson = result.Success<Person>();

// Both references are resolved
Console.WriteLine(johnPerson.Spouse?.Name);         // "Jane"
Console.WriteLine(johnPerson.Spouse?.Spouse?.Name); // "John"
```

### Getting a Reference Before Building

```csharp
var builder = new PersonBuilder { Name = "Alice", Age = 25 };

// Get a reference that will be resolved after build
Reference<Person> reference = builder.Reference();

Console.WriteLine(reference.IsResolved); // false

builder.Build();

Console.WriteLine(reference.IsResolved); // true
Person alice = reference.Resolved();
```

### Using BuilderList for Batch Operations

```csharp
var builders = new BuilderList<Person, PersonBuilder>();

builders
    .New(b => { b.Name = "Alice"; b.Age = 25; })
    .New(b => { b.Name = "Bob"; b.Age = 30; })
    .New(b => { b.Name = "Charlie"; b.Age = 35; });

// Build all and get successes
List<Person> people = builders.BuildSuccess();

// Or validate all and get failures
List<FailuresDictionary> failures = builders.ValidateFailures();

// Convert to reference list for later resolution
ReferenceList<Person> refs = builders.AsReferenceList();
```

### Working with Existing Objects

```csharp
var existingPerson = new Person { Name = "Existing", Age = 40 };

var builder = new PersonBuilder();
builder.Existing(existingPerson);

var result = builder.Build();
Person person = result.Success<Person>();

// person == existingPerson (same instance)
```

### ReferenceList Operations

```csharp
var list = new ReferenceList<Person>();

// Add by instance
list.Add(new Person { Name = "Alice", Age = 25 });

// Add by reference
list.Add(builder.Reference());

// Query operations
bool hasAlice = list.Any(p => p.Name == "Alice");
var adults = list.Where(p => p.Age >= 18);
var names = list.Select(p => p.Name);

// LINQ support
var query = list.Queryable.Where(p => p.Age > 20);
```

## 📋 API Reference

### `AbstractBuilder<TClass>`

| Member | Description |
|--------|-------------|
| `Id` | Unique identifier for the builder instance |
| `Result` | The result of the last build operation |
| `BuildStatus` | Current build status (`NotBuilding`, `Building`, `Built`) |
| `ValidationStatus` | Current validation status (`NotValidated`, `Validating`, `Validated`) |
| `Build(VisitedObjectDictionary?)` | Validates and builds the object |
| `BuildSuccess(VisitedObjectDictionary?)` | Builds and returns object or throws |
| `Validate(VisitedObjectDictionary, FailuresDictionary)` | Validates the builder state |
| `Reference()` | Gets a reference to the built object |
| `Existing(TClass)` | Uses an existing instance instead of building |

### `Reference<TClass>`

| Member | Description |
|--------|-------------|
| `Instance` | The resolved instance (or null) |
| `IsResolved` | Whether the reference has been resolved |
| `Resolve(TClass)` | Associates an instance with the reference |
| `Resolved()` | Gets the instance or throws `NotResolvedException` |
| `ResolvedOrNull()` | Gets the instance or null |

### `ReferenceList<TClass>`

| Member | Description |
|--------|-------------|
| `Add(TClass)` | Adds an instance |
| `Add(Reference<TClass>)` | Adds a reference |
| `Contains(TClass)` | Checks if instance is in list |
| `Contains(Reference<TClass>)` | Checks if reference is in list |
| `Any()` / `Any(predicate)` | Checks if any items match |
| `All(predicate)` | Checks if all items match |
| `Where(predicate)` | Filters items |
| `Select(mapper)` | Projects items |
| `Queryable` | Gets IQueryable for LINQ |
| `AsEnumerable()` | Gets resolved items as enumerable |

### `BuilderList<TClass, TBuilder>`

| Member | Description |
|--------|-------------|
| `New(Action<TBuilder>)` | Creates and configures a new builder |
| `BuildSuccess()` | Builds all and returns successful objects |
| `BuildFailures()` | Builds all and returns failure dictionaries |
| `ValidateFailures()` | Validates all and returns failures |
| `AsReferenceList()` | Converts to reference list |

### Validation Helpers (in `AbstractBuilder<TClass>`)

| Method | Description |
|--------|-------------|
| `AssertNotNull(value, name, failures, exBuilder)` | Asserts value is not null |
| `AssertNotEmptyOrWhitespace(value, name, failures, exBuilder)` | Asserts string is not empty/whitespace (null allowed) |
| `AssertNotNullOrEmptyOrWhitespace(value, name, failures, exBuilder)` | Asserts string is not null/empty/whitespace |
| `AssertNotNullNotEmptyCollection(list, name, failures, exBuilder)` | Asserts collection is not null/empty |
| `Assert(predicate, name, failures, exBuilder)` | Custom assertion |
| `ValidateListInternal(list, name, visited, failures)` | Validates a BuilderList |
| `BuildList(builders, visited)` | Builds all items in a BuilderList |

### Result Types

| Type | Description |
|------|-------------|
| `IResult` | Base interface for build results |
| `SuccessResult<TClass>` | Contains successfully built object |
| `FailureResult` | Contains validation failures dictionary |

### Extension Methods

| Method | Description |
|--------|-------------|
| `result.Success<T>()` | Gets object or throws `BuildFailedException` |
| `result.Failures<T>()` | Gets failures or throws `BuildSucceededException` |
| `result.IsSuccess<T>()` | Checks if result is successful |
| `result.IsFailure()` | Checks if result is a failure |

## 🧪 Testing Utilities

The `FrenchExDev.Net.CSharp.Object.Builder.Testing` package provides assertion helpers:

```csharp
// Static assertions
BuilderAssert.IsSuccess<Person>(result);
BuilderAssert.IsFailure(result);
BuilderAssert.HasFailureForMember(result, "Name");
BuilderAssert.IsResolved(reference);
BuilderAssert.BuildsSuccessfully(builder);

// Fluent assertions
result.ShouldBeSuccess<Person>();
result.ShouldBeFailure();
result.ShouldHaveFailureForMember("Name");

builder.ShouldBuildSuccessfully<Person>();
builder.ShouldBuildFailing<Person>()
    .ShouldContainMember("Name")
    .ShouldContainException<StringIsEmptyOrWhitespaceException>("Name");

reference.ShouldBeResolved();
reference.ShouldNotBeResolved();
```

## 🛠️ Requirements

- .NET 10.0 or later

## 📄 License

See the [LICENSE](./LICENSE.md) file for details.
