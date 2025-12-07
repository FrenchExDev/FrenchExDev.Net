# FrenchExDev.Net

A collection of .NET libraries for building robust, type-safe applications with explicit error handling and object construction patterns.

## Projects

### 🎯 CSharp.Object.Builder

A lightweight .NET library implementing the **Builder pattern** with built-in validation, deferred reference resolution, and failure tracking.

| Feature | Description |
|---------|-------------|
| `AbstractBuilder<T>` | Base class for implementing type-safe builders with validation |
| `Reference<T>` | Deferred reference resolution for handling circular dependencies |
| `ReferenceList<T>` | Collection of references with LINQ support |
| `BuilderList<T, TBuilder>` | Batch building and validation of multiple objects |
| `FailuresDictionary` | Structured collection of validation failures per member |
| Cycle detection | Automatic handling of circular references during build |

**Quick Example:**
```csharp
var builder = new PersonBuilder { Name = "John", Age = 30 };
var result = builder.Build();

if (result.IsSuccess<Person>())
{
    Person person = result.Success<Person>();
}
```

[Full Documentation](./CSharp.Object.Builder/README.md) | [License](./CSharp.Object.Builder/LICENSE.md)

---

### 🎯 CSharp.Object.Result

A lightweight .NET library for representing operation outcomes as `Result` and `Result<T>` types, enabling explicit handling of success and failure states without throwing exceptions.

| Feature | Description |
|---------|-------------|
| `Result` | Represents operation outcome without a return value |
| `Result<T>` | Represents operation outcome with a typed return value |
| Exception handling | Optional encapsulation of exceptions in failure results |
| Fluent API | `IfSuccess()` and `IfFailure()` methods for conditional execution |
| Async support | `IfSuccessAsync()` and `IfFailureAsync()` methods |
| Failure dictionary | Structured storage of multiple failure details |

**Quick Example:**
```csharp
// Create and use results
var success = Result<string>.Success("Hello, World!");

success
    .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    .IfFailure(failures => Console.WriteLine("Failed"));

// Safe value access
string? valueOrNull = success.ObjectOrNull();
```

[Full Documentation](./CSharp.Object.Result/README.md) | [License](./CSharp.Object.Result/LICENSE.md)

---

## Requirements

- .NET 10.0 or later

## Licensing

Both projects are licensed under the same terms:

### Educational Use
Free to use for:
- Reading and studying the source code
- Academic projects
- Personal learning and experimentation
- Referencing in educational materials

### Production Use
**Requires a separate license agreement.** Contact the author for:
- Production environment deployments
- Commercial products or services
- Revenue-generating applications
- Commercial distribution

### Contact

For production licensing inquiries:
- GitHub: [FrenchExDev.Net](https://github.com/FrenchExDev)

---

## 📦 Repository Structure

```
FrenchExDev.Net/
+-- CSharp.Object.Builder/
|   +-- src/
|   |   +-- FrenchExDev.Net.CSharp.Object.Builder/          # Core library
|   |   +-- FrenchExDev.Net.CSharp.Object.Builder.Testing/  # Testing utilities
|   +-- test/
|   |   +-- FrenchExDev.Net.CSharp.Object.Builder.Tests/    # Unit tests
|   +-- README.md
|   +-- LICENSE.md
|
+-- CSharp.Object.Result/
|   +-- src/
|   |   +-- FrenchExDev.Net.CSharp.Object.Result/           # Core library
|   |   +-- FrenchExDev.Net.CSharp.Object.Result.Testing/   # Testing utilities
|   +-- test/
|   |   +-- FrenchExDev.Net.CSharp.Object.Result.Tests/     # Unit tests
|   +-- README.md
|   +-- LICENSE.md
|
+-- README.md
