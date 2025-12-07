# `FrenchExDev.Net.CSharp.Object.Result`

A lightweight .NET library for representing operation outcomes as `Result` and `Result<T>` types, enabling explicit handling of success and failure states without throwing exceptions.

## 📦 Project Structure

```
CSharp.Object.Result/
├── src/
│   ├── FrenchExDev.Net.CSharp.Object.Result/          # Core library
│   └── FrenchExDev.Net.CSharp.Object.Result.Testing/  # Testing utilities
└── test/
    └── FrenchExDev.Net.CSharp.Object.Result.Tests/    # Unit tests
```

## 🎯 Features

- **`Result`** - Represents the outcome of an operation without a return value (success/failure)
- **`Result<T>`** - Represents the outcome of an operation with a typed return value
- **Exception handling** - Optional encapsulation of exceptions in failure results
- **Fluent API** - `IfSuccess()` and `IfFailure()` methods for conditional execution
- **Async support** - `IfSuccessAsync()` and `IfFailureAsync()` methods
- **Failure dictionary** - Structured storage of multiple failure details
- **Extension methods** - Convenient conversions to result types

## 🚀 Usage

### Simple Result (without value)

```csharp
// Create a success
var success = Result.Success();

// Create a failure
var failure = Result.Failure();

// Create a failure with exception
var failureWithEx = Result.Failure(new InvalidOperationException("error"));

// Check the result
if (success.IsSuccess)
{
    // Handle success
}

// Conditional execution with fluent API
Result.Success()
    .IfSuccess(() => Console.WriteLine("Operation succeeded"))
    .IfFailure(ex => Console.WriteLine($"Operation failed: {ex?.Message}"));
```

### Generic Result (with value)

```csharp
// Create a success with value
var success = Result<string>.Success("Hello, World!");

// Create a failure
var failure = Result<string>.Failure();

// Create a failure with details
var failureWithDetails = Result<string>.Failure(builder =>
{
    builder.Add("Reason", "Invalid input");
    builder.Add("Field", "Username");
});

// Access the value
if (success.IsSuccess)
{
    string value = success.Object;
}

// Safe access with null fallback
string? valueOrNull = success.ObjectOrNull();

// Try pattern
if (success.TryGetSuccess(out var result))
{
    Console.WriteLine(result);
}

// Conditional execution
success
    .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    .IfFailure(failures => Console.WriteLine("Failed"));
```

### Async Operations

```csharp
await Result.Success()
    .IfSuccessAsync(async () =>
    {
        await SomeAsyncOperation();
    });

await Result<string>.Success("data")
    .IfSuccessAsync(async value =>
    {
        await ProcessAsync(value);
    });
```

### Exception Handling with TryCatch

```csharp
// Catch specific exception type
var result = Result<int>.TryCatch<int, FormatException>(() =>
{
    return int.Parse("not a number");
});

// Catch any exception
var result2 = Result<int>.TryCatch(() =>
{
    return RiskyOperation().ToSuccess();
});
```

### Extension Methods

```csharp
// Convert value to success result
var success = "hello".ToSuccess();

// Convert boolean to result
var result = (1 == 1).ToResult(); // Result.Success()

// Convert exception to failure
var failure = new Exception("error").ToFailure<string>();

// Convert value with failure reason
var failedValue = myObject.ToFailure("Validation failed");
```

### Failure Dictionary

```csharp
// Create failure with multiple details
var failure = Result<Order>.Failure(builder =>
{
    builder.Add("ValidationError", "Name is required");
    builder.Add("ValidationError", "Email is invalid");
    builder.Add("Field", "CustomerInfo");
});

// Access failures
failure.IfFailure(failures =>
{
    if (failures != null && failures.TryGetValue("ValidationError", out var errors))
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }
    }
});
```

## 📋 API Reference

### `Result` (non-generic)

| Member | Description |
|--------|-------------|
| `Success()` | Creates a successful result |
| `Failure()` | Creates a failed result |
| `Failure(Exception)` | Creates a failed result with an exception |
| `IsSuccess` | Returns `true` if the result is successful |
| `IsFailure` | Returns `true` if the result is a failure |
| `Exception` | Gets the exception associated with the failure |
| `IfSuccess(Action)` | Executes action if successful |
| `IfFailure(Action<Exception?>)` | Executes action if failed |
| `IfSuccessAsync(Func<Task>)` | Async version of `IfSuccess` |
| `IfFailureAsync(Func<Exception?, Task>)` | Async version of `IfFailure` |

### `Result<T>` (generic)

| Member | Description |
|--------|-------------|
| `Success(T)` | Creates a successful result with a value |
| `Failure()` | Creates a failed result |
| `Failure(FailureDictionary)` | Creates a failed result with failure details |
| `Failure(Action<FailureDictionaryBuilder>)` | Creates a failed result using a builder |
| `Failure(Exception)` | Creates a failed result with an exception |
| `IsSuccess` | Returns `true` if the result is successful |
| `IsFailure` | Returns `true` if the result is a failure |
| `Object` | Gets the value (throws if failure) |
| `ObjectOrNull()` | Gets the value or `null` |
| `ObjectOrThrow()` | Gets the value or throws |
| `TryGetSuccess(out T)` | Tries to get the value |
| `Failures` | Gets the failure dictionary |
| `FailuresOrThrow()` | Gets failures or throws |
| `TryCatch<TResult, TException>(Func<TResult>)` | Executes and catches specific exception |
| `TryCatch<TResult>(Func<Result<TResult>>)` | Executes and catches any exception |

## 🛠️ Requirements

- .NET 10.0 or later

## 📄 License

See the [LICENSE](./LICENSE.md) file for details.
