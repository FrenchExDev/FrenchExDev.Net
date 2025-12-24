# `FrenchExDev.Net.CSharp.Object.Result`

A lightweight .NET library for representing operation outcomes as `Result`, `Result<T>`, and `ResultEx<TResult, TException>` types, enabling explicit handling of success and failure states without throwing exceptions.

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
- **`ResultEx<TResult, TException>`** - Represents success with a value or failure with a strongly-typed exception
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

### ResultEx (with typed exception)

```csharp
// Create a success with value
var success = ResultEx<int, InvalidOperationException>.Success(42);

// Create a failure with a specific exception type
var failure = ResultEx<int, InvalidOperationException>.Failure(
    new InvalidOperationException("Something went wrong"));

// Access the value safely
if (success.IsSuccess)
{
    int value = success.Object;
}

// Access with null fallback
int? valueOrNull = success.ObjectOrNull();

// Access or throw the stored exception
try
{
    int value = failure.ObjectOrThrow(); // Throws the stored InvalidOperationException
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
}

// Try pattern for value
if (success.TryGetValue(out var result))
{
    Console.WriteLine($"Value: {result}");
}

// Try pattern for exception
if (failure.TryGetException(out var exception))
{
    Console.WriteLine($"Error: {exception.Message}");
}

// Conditional execution
success
    .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    .IfFailure(ex => Console.WriteLine($"Failed: {ex?.Message}"));

// Match pattern for handling both cases
failure.Match(
    onSuccess: value => Console.WriteLine($"Success: {value}"),
    onFailure: ex => Console.WriteLine($"Failure: {ex.Message}")
);

// TryCatch for automatic exception handling
var result = ResultEx<int, FormatException>.TryCatch(() =>
{
    return int.Parse("not a number"); // Throws FormatException
});
// result is now a Failure containing the FormatException
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

await ResultEx<string, Exception>.Success("data")
    .IfSuccessAsync(async value =>
    {
        await ProcessAsync(value);
    })
    .IfFailureAsync(async ex =>
    {
        await LogErrorAsync(ex);
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

### `ResultEx<TResult, TException>` (with typed exception)

| Member | Description |
|--------|-------------|
| `Success(TResult)` | Creates a successful result with a value |
| `Failure(TException)` | Creates a failed result with a typed exception |
| `IsSuccess` | Returns `true` if the result is successful |
| `IsFailure` | Returns `true` if the result is a failure |
| `Object` | Gets the value (throws `InvalidResultAccessException` if failure) |
| `Exception` | Gets the exception (throws `InvalidResultAccessException` if success) |
| `ObjectOrNull()` | Gets the value or `null` |
| `ObjectOrThrow()` | Gets the value or throws the stored exception |
| `TryGetValue(out TResult)` | Tries to get the value |
| `TryGetException(out TException)` | Tries to get the exception |
| `TryGetSuccess(out TResult)` | Tries to get the successful value |
| `TryGetFailure(out TException)` | Tries to get the failure exception |
| `IfSuccess(Action<TResult>)` | Executes action with value if successful |
| `IfSuccess(Action)` | Executes action if successful |
| `IfFailure(Action<TException?>)` | Executes action with exception if failed |
| `IfFailure(Action)` | Executes action if failed |
| `IfSuccessAsync(Func<TResult, Task>)` | Async version of `IfSuccess` |
| `IfFailureAsync(Func<TException?, Task>)` | Async version of `IfFailure` |
| `Match(Action<TResult>, Action<TException>)` | Executes appropriate action based on state |
| `TryCatch(Func<TResult>)` | Executes and catches `TException`, returning a result |

## 🛠️ Requirements

- .NET 10.0 or later

## 📄 License

See the [LICENSE](./LICENSE.md) file for details.
