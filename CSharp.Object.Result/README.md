# FrenchExDev.Net.CSharp.Object.Result

A functional programming library for .NET 9 that provides a robust Result pattern implementation for explicit error handling without exceptions.

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview

The Result pattern is a functional programming approach that makes success and failure explicit in method signatures. Instead of throwing exceptions or returning null, operations return a `Result` or `Result<T>` that encapsulates both the outcome and relevant data.

### Key Features

- ? **Type-Safe Error Handling**: Compiler-enforced handling of success and failure cases
- ? **Structured Error Information**: `FailureDictionary` for categorized, detailed error tracking
- ? **Fluent API**: Chainable methods for composing operations
- ? **Async/Await Support**: First-class async operation handling
- ? **Zero Allocation**: Struct-based implementation for performance
- ? **Railway Oriented Programming**: Elegant composition of operations
- ? **TryCatch Helpers**: Convert exception-throwing code to Results
- ? **Comprehensive Testing**: Full test coverage with xUnit and Shouldly

## Installation

```bash
dotnet add package FrenchExDev.Net.CSharp.Object.Result
```

Or via Package Manager Console:

```powershell
Install-Package FrenchExDev.Net.CSharp.Object.Result
```

## Quick Start

### Basic Usage

```csharp
using FrenchExDev.Net.CSharp.Object.Result;

// Non-generic Result for operations without return value
public Result DeleteUser(int userId)
{
    try
    {
        _repository.Delete(userId);
        return Result.Success();
    }
    catch (Exception ex)
    {
        return Result.Failure(ex);
    }
}

// Generic Result<T> for operations that return a value
public Result<User> GetUser(int userId)
{
    var user = _repository.Find(userId);
    
    return user != null
        ? user.ToSuccess()
        : Result<User>.Failure(d => d.Add("NotFound", $"User {userId} not found"));
}

// Consuming Results
var result = GetUser(123);
if (result.IsSuccess)
{
    var user = result.Object;
    Console.WriteLine($"Found: {user.Name}");
}
else
{
    var failures = result.FailuresOrThrow();
    Console.WriteLine($"Error: {failures}");
}
```

### Fluent Chaining

```csharp
return ValidateUser(request)
    .IfSuccess(user => EnrichUser(user))
    .IfSuccess(enriched => SaveUser(enriched))
    .IfFailure(failures => LogErrors(failures));
```

### Detailed Error Tracking

```csharp
public Result<Order> ValidateOrder(OrderRequest request)
{
    return Result<Order>.Failure(builder => builder
        .Add("Items", "Order must contain at least one item")
        .Add("Total", "Total must be positive")
        .Add("Customer", "Customer not found"));
}
```

### Exception Handling

```csharp
// TryCatch converts exceptions to failures
public Result<Data> LoadData(string path)
{
    return Result<Data>.TryCatch<Data>(() =>
    {
        var content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Data>(content).ToSuccess();
    });
}
```

### Async Operations

```csharp
public async Task<Result<User>> RegisterUserAsync(RegistrationRequest request)
{
    return await ValidateRegistrationAsync(request)
        .IfSuccessAsync(async _ =>
        {
            var user = await CreateUserAsync(request);
            await SendWelcomeEmailAsync(user.Email);
            return user;
        })
        .IfFailureAsync(async failures =>
        {
            await _logger.LogAsync($"Registration failed: {failures}");
        });
}
```

## Documentation

### ?? Comprehensive Guides

- **[Architecture Documentation](doc/ARCHITECTURE.md)** - Complete architecture overview
  - Solution structure and project organization
  - Type hierarchy with class diagrams
  - Component details and relationships
  - Design patterns and best practices
  - Testing support and coverage

- **[Result Pattern Guide](doc/RESULT-PATTERN.md)** - Deep dive into the Result pattern
  - Complete implementation details with code examples
  - Mermaid diagrams (class, sequence, flowcharts)
  - CRUD operation examples
  - Exception handling patterns
  - Validation patterns
  - Async/await patterns
  - Railway-oriented programming
  - **Comparison table** with other Result libraries (ErrorOr, CSharpFunctionalExtensions, LanguageExt, Ardalis.Result)
  - Side-by-side code comparisons
  - Library selection guidance

### ?? Quick Reference

#### Core Types

| Type | Purpose | Example |
|------|---------|---------|
| `Result` | Non-generic result for void operations | `Result.Success()` |
| `Result<T>` | Generic result for operations returning T | `Result<User>.Success(user)` |
| `FailureDictionary` | Structured error storage | `failures.Add("Email", "Invalid")` |
| `FailureDictionaryBuilder` | Fluent failure construction | `d => d.Add("Key", "Value")` |

#### Factory Methods

```csharp
// Success
Result.Success()
Result<T>.Success(value)
value.ToSuccess()

// Failure
Result.Failure()
Result.Failure(exception)
Result<T>.Failure()
Result<T>.Failure(failureDictionary)
Result<T>.Failure(builder => builder.Add("key", "value"))
exception.ToFailure<T>()

// TryCatch
Result<T>.TryCatch<T>(() => operation())
Result<T>.TryCatch<T, SpecificException>(() => operation())
```

#### Conditional Execution

```csharp
// Sync
result
    .IfSuccess(value => DoSomething(value))
    .IfFailure(failures => HandleErrors(failures))

// Async
await result
    .IfSuccessAsync(async value => await DoSomethingAsync(value))
    .IfFailureAsync(async failures => await HandleErrorsAsync(failures))
```

#### Value Access

```csharp
// Safe access
T? value = result.ObjectOrNull();          // Returns null on failure
bool success = result.TryGetSuccess(out T value);

// Throwing access
T value = result.Object;                   // Throws if failure
T value = result.ObjectOrThrow();          // Explicit throw
FailureDictionary errors = result.FailuresOrThrow();
```

## Project Structure

```
CSharp.Object.Result/
??? src/
?   ??? FrenchExDev.Net.CSharp.Object.Result/         # Core library
?   ??? FrenchExDev.Net.CSharp.Object.Result.Testing/ # Test utilities
??? test/
?   ??? FrenchExDev.Net.CSharp.Object.Result.Tests/   # Unit tests
??? doc/
    ??? ARCHITECTURE.md                                # Architecture documentation
    ??? RESULT-PATTERN.md                              # Pattern implementation guide
```

## When to Use Result Pattern

### ? Good Use Cases

- Business rule validation
- Expected failures (not found, already exists, etc.)
- Operations that may fail for known reasons
- API boundaries where errors need to be communicated
- Parsing/conversion operations
- Multi-step workflows with potential failures

### ? Avoid Using For

- Truly exceptional circumstances (out of memory, stack overflow)
- Programming errors (null reference, index out of range)
- Framework exceptions that should propagate
- Performance-critical tight loops

## Comparison with Other Libraries

| Feature | This Library | ErrorOr | CSharpFunctionalExtensions | LanguageExt |
|---------|--------------|---------|---------------------------|-------------|
| **Type Structure** | Struct (stack) | Struct | Class (heap) | Struct |
| **Categorized Errors** | ? FailureDictionary | ? Error types | ? | ? |
| **Multiple Errors** | ? List per category | ? List | ? Single | ? |
| **Pattern Matching** | Manual if/else | ? Match() | ? Match() | ? Match() |
| **Railway Oriented** | ? IfSuccess | ? Then | ? Bind | ? Bind |
| **TryCatch Helpers** | ? Generic & typed | ? | ? | ? |
| **Learning Curve** | Low | Low | Medium | High |

?? **[See detailed comparison in RESULT-PATTERN.md](doc/RESULT-PATTERN.md#comparison-with-other-result-patterns)**

## Examples

### Validation with Multiple Errors

```csharp
public Result<User> ValidateUser(UserRequest request)
{
    var errors = new List<string>();
    
    if (string.IsNullOrEmpty(request.Email))
        errors.Add("Email is required");
    if (!IsValidEmail(request.Email))
        errors.Add("Email format is invalid");
    if (request.Age < 18)
        errors.Add("Must be 18 or older");
        
    if (errors.Any())
    {
        return Result<User>.Failure(builder =>
        {
            foreach (var error in errors)
                builder.Add("Validation", error);
        });
    }
    
    return CreateUser(request).ToSuccess();
}
```

### Railway-Oriented Programming

```csharp
public Result<ProcessedOrder> ProcessOrderPipeline(OrderRequest request)
{
    return ValidateRequest(request)
        .IfSuccess(validated => EnrichOrder(validated))
        .IfSuccess(enriched => ApplyDiscounts(enriched))
        .IfSuccess(discounted => CalculateTotals(discounted))
        .IfSuccess(finalized => SaveOrder(finalized))
        .IfFailure(failures => LogFailures(failures));
}
```

### Async Workflow

```csharp
public async Task<Result<Transaction>> ProcessPaymentAsync(PaymentRequest request)
{
    return await ValidatePayment(request)
        .IfSuccessAsync(async valid =>
        {
            await DeductBalance(valid.AccountId, valid.Amount);
            return valid;
        })
        .IfSuccessAsync(async deducted =>
        {
            await RecordTransaction(deducted);
            return deducted.Transaction;
        })
        .IfFailureAsync(async failures =>
        {
            await NotifyPaymentFailure(request.UserId, failures);
        });
}
```

## Testing

The library includes comprehensive unit tests using xUnit and Shouldly:

```bash
dotnet test
```

Tests cover:
- Success and failure creation
- Property access and exceptions
- Conditional execution (sync and async)
- Extension methods
- Failure dictionary operations
- TryCatch patterns
- Edge cases

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Links

- ?? **[Architecture Documentation](doc/ARCHITECTURE.md)**
- ?? **[Result Pattern Implementation Guide](doc/RESULT-PATTERN.md)**
- ?? **[GitHub Repository](https://github.com/FrenchExDev/FrenchExDev.Net)**
- ?? **[NuGet Package](https://www.nuget.org/packages/FrenchExDev.Net.CSharp.Object.Result)**

## Related Projects

- [FrenchExDev.Net.CSharp.Object.Builder2](../CSharp.Object.Builder2/) - Builder pattern implementation
- [FrenchExDev.Net.CSharp.ManagedList](../CSharp.ManagedList/) - Managed collection implementations
- [FrenchExDev.Net.CSharp.ManagedDictionary](../CSharp.ManagedDictionary/) - Managed dictionary implementations

---

**Target Framework**: .NET 9  
**Version**: 1.0  
**Author**: FrenchExDev
