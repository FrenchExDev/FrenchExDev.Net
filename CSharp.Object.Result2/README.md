# FrenchExDev.Net.CSharp.Object.Result2

[![NuGet](https://img.shields.io/nuget/v/FrenchExDev.Net.CSharp.Object.Result2.svg)](https://www.nuget.org/packages/FrenchExDev.Net.CSharp.Object.Result2)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A lightweight, strongly-typed Result pattern implementation for .NET that enables elegant error handling without relying on exceptions for control flow.

## Table of Contents

- [Overview](#overview)
- [Why Use Result Pattern?](#why-use-result-pattern)
- [When NOT to Use](#when-not-to-use)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Concepts](#core-concepts)
- [API Reference](#api-reference)
- [Testing Support](#testing-support)
- [Exception Types](#exception-types)
- [Best Practices](#best-practices)
- [Common Pitfalls](#common-pitfalls)
- [Examples](#examples)

---

## Overview

The Result pattern provides a way to represent the outcome of an operation that can either succeed with a value or fail with an exception. This approach offers several benefits:

- **Explicit error handling** - Callers must acknowledge that an operation can fail
- **No exception-based control flow** - Exceptions are reserved for truly exceptional situations
- **Type safety** - The compiler helps ensure proper handling of both success and failure cases
- **Fluent API** - Chain operations elegantly with method chaining support
- **Monadic composition** - Use `Bind` for flat pipelines without nesting

---

## Why Use Result Pattern?

### Traditional Exception Handling

```csharp
// ❌ Problems with exceptions for expected failures:
public User GetUser(int id)
{
    var user = _db.Users.Find(id);
    if (user == null)
        throw new UserNotFoundException($"User {id} not found");  // Expensive!
    return user;
}

// Caller must remember to catch - easy to forget
try
{
    var user = GetUser(123);
    ProcessUser(user);
}
catch (UserNotFoundException ex)
{
    // Handle error
}
// What if we forget the try/catch? Runtime crash!
```

### With Result Pattern

```csharp
// ✅ Benefits of Result pattern:
public Result<User> GetUser(int id)
{
    var user = _db.Users.Find(id);
    if (user == null)
        return Result<User>.Failure(new UserNotFoundException($"User {id} not found"));
    return Result<User>.Success(user);
}

// Caller MUST handle the result - compiler enforces it
var result = GetUser(123);
result.Match(
    onSuccess: user => ProcessUser(user),
    onFailure: ex => HandleError(ex)
);
// Cannot accidentally ignore the error case!
```

### Key Benefits

| Aspect | Exceptions | Result Pattern |
|--------|------------|----------------|
| Performance | Stack unwinding is expensive | No overhead for expected failures |
| Explicitness | Hidden control flow | Visible in method signatures |
| Composition | Try/catch nesting | Fluent chaining with `Bind`/`Map` |
| Compiler help | None - runtime failures | Type system enforces handling |

---

## When NOT to Use

The Result pattern is **not** a replacement for all exceptions. Use exceptions for:

- **Truly exceptional situations** - Out of memory, stack overflow, null reference bugs
- **Programming errors** - Invalid arguments that indicate bugs (use `ArgumentException`)
- **Unrecoverable failures** - Corrupted state, security violations
- **Framework integration** - ASP.NET filters, middleware expecting exceptions

```csharp
// ✅ Use Result for expected failures
public Result<User> FindUser(int id) { ... }           // User might not exist
public Result<Order> PlaceOrder(OrderRequest r) { ... } // Validation might fail

// ✅ Use exceptions for programming errors
public void ProcessUser(User user)
{
    ArgumentNullException.ThrowIfNull(user);  // Bug if null passed
    // ...
}

// ✅ Use exceptions for truly exceptional cases
public void SaveToDatabase(Data data)
{
    // Let database exceptions bubble up - they're truly exceptional
    _db.Save(data);
}
```

---

## Requirements

- **.NET 10.0** or later
- **C# 13.0** or later (for best experience with pattern matching)

---

## Installation

Add the package reference to your project:

```xml
<PackageReference Include="FrenchExDev.Net.CSharp.Object.Result2" Version="1.0.0" />
```

For testing support, also add:

```xml
<PackageReference Include="FrenchExDev.Net.CSharp.Object.Result2.Testing" Version="1.0.0" />
```

---

## Quick Start

```csharp
using FrenchExDev.Net.CSharp.Object.Result2;

// Create a successful result
var successResult = Result<int>.Success(42);

// Create a failed result
var failureResult = Result<int>.Failure(new InvalidOperationException("Something went wrong"));

// Handle the result
successResult.Match(
    onSuccess: value => Console.WriteLine($"Got value: {value}"),
    onFailure: ex => Console.WriteLine($"Error: {ex.Message}")
);

// Transform and chain operations without nesting
var result = Result<int>.Success(10)
    .Map(x => x * 2)                          // Transform: 10 -> 20
    .Bind(x => x > 0                          // Chain: returns new Result
        ? Result<string>.Success($"Value: {x}")
        : Result<string>.Failure(new ArgumentException("Must be positive")));
```

---

## Core Concepts

### Creating Results

#### Success

Create a successful result containing a value:

```csharp
// Simple value
var result = Result<int>.Success(42);

// Complex object
var userResult = Result<User>.Success(new User { Name = "John" });

// Nullable value (null is a valid success value)
var nullableResult = Result<string?>.Success(null);
```

#### Failure

Create a failed result containing an exception:

```csharp
// With a standard exception
var result = Result<int>.Failure(new InvalidOperationException("Operation failed"));

// With a custom exception
var result = Result<User>.Failure(new UserNotFoundException("User not found"))

// With ResultException (provided by the library)
var result = Result<string>.Failure(new ResultException("Custom error"));
```

### Checking Result State

```csharp
var result = Result<int>.Success(42);

// Check if successful
if (result.IsSuccess)
{
    Console.WriteLine("Operation succeeded!");
}
```

### Accessing Values and Exceptions

#### Direct Access (throws if invalid state)

```csharp
var result = Result<int>.Success(42;

// Access value (throws InvalidResultAccessOperationException if failure)
int value = result.Value;

// Access exception (throws InvalidResultAccessOperationException if success)
var failedResult = Result<int>.Failure(new Exception("error"));
Exception ex = failedResult.Exception<Exception>();
```

#### Safe Access (Try pattern)

```csharp
var result = Result<int>.Success(42);

// Try to get value
if (result.TryGetSuccessValue(out var value))
{
    Console.WriteLine($"Value: {value}");
}

// Try to get exception
if (result.TryGetException<InvalidOperationException>(out var exception))
{
    Console.WriteLine($"Error: {exception.Message}");
}
```

---

## API Reference

### Result\<TResult\> Members

A readonly struct representing the outcome of an operation.

#### Properties and Factory Methods

| Member | Type | Description |
|--------|------|-------------|
| `IsSuccess` | `bool` | Returns `true` if the operation succeeded |
| `Value` | `TResult` | Gets the success value (throws if failure) |
| `Success(TResult value)` | `static` | Creates a successful result |
| `Failure<TException>(TException exception)` | `static` | Creates a failed result |
| `Exception<TException>()` | `TException` | Gets the exception (throws if success) |
| `TryGetSuccessValue(out TResult? value)` | `bool` | Safely attempts to get the value |
| `TryGetException<TException>(out TException? exception)` | `bool` | Safely attempts to get the exception |

#### Transformation Methods

| Method | Description |
|--------|-------------|
| `Map<TNew>(Func<TResult, TNew>)` | Transforms the value if successful |
| `MapAsync<TNew>(Func<TResult, Task<TNew>>)` | Asynchronously transforms the value |
| `Bind<TNew>(Func<TResult, Result<TNew>>)` | Chains a result-producing operation |
| `BindAsync<TNew>(Func<TResult, Task<Result<TNew>>>)` | Asynchronously chains a result-producing operation |

#### Pattern Matching Methods

| Method | Description |
|--------|-------------|
| `Match(onSuccess, onFailure)` | Handles success or failure with `Action` delegates |
| `Match<TException>(onSuccess, onFailure)` | Handles success or typed exception failure |
| `MatchAsync(onSuccess, onFailure)` | Async version with `Func<T, Task>` delegates |
| `MatchAsync<TException>(onSuccess, onFailure)` | Async with typed exception |

#### Fluent Handler Methods

| Method | Description |
|--------|-------------|
| `IfSuccess(Action<TResult>)` | Executes action on success, returns result |
| `IfSuccessAsync(Func<TResult, Task>)` | Async version |
| `IfException(Action<Exception>)` | Executes action on failure, returns result |
| `IfException<TException>(Action<TException>)` | Executes action for specific exception type |
| `IfExceptionAsync(Func<Exception, Task>)` | Async version |
| `IfExceptionAsync<TException>(Func<TException, Task>)` | Async with typed exception |

### Pattern Matching

Use `Match` to handle both success and failure cases in a single call:

```csharp
var result = GetUserById(123);

// Basic match
result.Match(
    onSuccess: user => Console.WriteLine($"Found: {user.Name}"),
    onFailure: ex => Console.WriteLine($"Error: {ex.Message}")
);

// Match with typed exception
result.Match<UserNotFoundException>(
    onSuccess: user => SaveToCache(user),
    onFailure: ex => LogNotFound(ex.UserId)
);

// Async match
await result.MatchAsync(
    onSuccess: async user => await _cache.SetAsync(user.Id, user),
    onFailure: async ex => await _logger.ErrorAsync("Failed", ex));
```

### Transformation Methods

#### Map - Transform the Value

```csharp
var result = Result<int>.Success(10);

// Transform int to string
var stringResult = result.Map(x => $"Value: {x}");
// Success("Value: 10")

// Chain multiple transformations
var finalResult = result
    .Map(x => x * 2)      // 10 -> 20
    .Map(x => x + 5)      // 20 -> 25
    .Map(x => $"Result: {x}");  // 25 -> "Result: 25"

// Failure propagates without calling the function
var failure = Result<int>.Failure(new Exception("error"));
var mapped = failure.Map(x => x * 2);  // Still a failure
```

#### Bind - Chain Result-Producing Operations

```csharp
// With Bind - flat and readable
var finalResult = GetUser(id)
    .Bind(user => ValidateUser(user))   // Returns Result<User>
    .Bind(user => SaveUser(user))       // Returns Result<User>
    .Map(user => user.Name);            // Transform to string

// Async version
var result = await GetUserAsync(id)
    .BindAsync(user => ValidateUserAsync(user))
    .BindAsync(user => SaveUserAsync(user));
```

### Fluent Handlers

```csharp
result
    .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    .IfSuccess(value => _audit.Log($"Retrieved: {value}"))
    .IfException(ex => _logger.Error(ex))
    .IfException<NotFoundException>(ex => _cache.Invalidate(ex.Key));
```

---

## Testing Support

The `FrenchExDev.Net.CSharp.Object.Result2.Testing` package provides assertion helpers:

```csharp
using FrenchExDev.Net.CSharp.Object.Result2.Testing;

[Fact]
public void GetUser_WithValidId_ReturnsSuccess()
{
    var result = service.GetUser(123);
    ResultTesting.ShouldBeSuccess(result);
}

[Fact]
public void GetUser_WithValidId_ReturnsCorrectUser()
{
    var result = service.GetUser(123);
    var user = ResultTesting.ShouldBeSuccessWithValue(result);
    user.Name.ShouldBe("John");
}

[Fact]
public void GetUser_WithInvalidId_ReturnsNotFoundException()
{
    var result = service.GetUser(-1);
    var ex = ResultTesting.ShouldBeFailureWithException<User, NotFoundException>(result);
    ex.Message.ShouldContain("not found");
}
```

### Testing API Reference

| Method | Description |
|--------|-------------|
| `ShouldBeSuccess<T>(result)` | Asserts result is successful |
| `ShouldBeSuccessWithValue<T>(result)` | Asserts success and returns value |
| `ShouldBeSuccessWithValue<T>(result, expected)` | Asserts success with specific value |
| `ShouldBeFailure<T>(result)` | Asserts result is a failure |
| `ShouldBeFailureWithException<T, TEx>(result)` | Asserts failure with exception type |
| `ShouldBeFailureWithMessage<T, TEx>(result, message)` | Asserts failure with exact message |
| `ShouldBeFailureWithMessageContaining<T, TEx>(result, substring)` | Asserts message contains substring |

---

## Exception Types

### ResultException

Base exception for result-related errors:

```csharp
throw new ResultException("Operation failed");
throw new ResultException("Operation failed", innerException);
```

### InvalidResultAccessOperationException

Thrown when accessing result state incorrectly:

```csharp
var failure = Result<int>.Failure(new Exception("error"));
var value = failure.Value; // Throws InvalidResultAccessOperationException

var success = Result<int>.Success(42);
var ex = success.Exception<Exception>(); // Throws InvalidResultAccessOperationException
```

---

## Best Practices

### 1. Use Bind for Flat Pipelines

```csharp
// ✅ Good - flat and readable
var result = ValidateInput(input)
    .Bind(valid => ParseData(valid))
    .Bind(data => SaveData(data));

// ❌ Avoid - deeply nested
var result = ValidateInput(input).Match(
    onSuccess: valid => ParseData(valid).Match(
        onSuccess: data => SaveData(data),
        onFailure: ex => Result<Data>.Failure(ex)),
    onFailure: ex => Result<Data>.Failure(ex));
```

### 2. Use Map for Transformations, Bind for Operations That Can Fail

```csharp
// ✅ Map - transformation cannot fail
var result = GetUserId()
    .Map(id => id.ToString());

// ✅ Bind - operation returns Result<T>
var result = GetUserId()
    .Bind(id => FindUser(id))  // FindUser returns Result<User>
    .Map(user => user.Name);   // Just transform
```

### 3. Prefer Try Methods for Conditional Access

```csharp
// ✅ Good
if (result.TryGetSuccessValue(out var value))
{
    ProcessValue(value);
}

// ❌ Avoid - redundant check
if (result.IsSuccess)
{
    var value = result.Value;
}
```

### 4. Use Match for Exhaustive Handling

```csharp
// ✅ Good - handles both cases
result.Match(
    onSuccess: value => HandleSuccess(value),
    onFailure: ex => HandleError(ex)
);

// ❌ Avoid - might forget failure
if (result.IsSuccess)
{
    HandleSuccess(result.Value);
}
```

### 5. Choose the Right Method

| Scenario | Method |
|----------|--------|
| Transform a value (cannot fail) | `Map` / `MapAsync` |
| Chain operations that can fail | `Bind` / `BindAsync` |
| Handle both success and failure | `Match` / `MatchAsync` |
| Side effect on success only | `IfSuccess` / `IfSuccessAsync` |
| Side effect on failure only | `IfException` / `IfExceptionAsync` |

---

## Common Pitfalls

### 1. Forgetting That Map Doesn't Unwrap Results

```csharp
// ❌ Wrong - results in Result<Result<User>>
var wrong = GetUserId().Map(id => FindUser(id));

// ✅ Correct - use Bind to unwrap
var correct = GetUserId().Bind(id => FindUser(id));
```

### 2. Using Match When Bind Would Be Cleaner

```csharp
// ❌ Verbose
var result = GetUser(id).Match(
    onSuccess: user => ValidateUser(user),
    onFailure: ex => Result<User>.Failure(ex));

// ✅ Clean
var result = GetUser(id).Bind(user => ValidateUser(user));
```

### 3. Ignoring the Result

```csharp
// ❌ Compiles but ignores failure
GetUser(id);  // Result is discarded!

// ✅ Always handle the result
var result = GetUser(id);
result.Match(...);
// or
_ = GetUser(id).IfException(ex => _logger.Error(ex));
```

### 4. Throwing Inside Map/Bind

```csharp
// ❌ Defeats the purpose of Result
var result = GetUser(id).Map(user => {
    if (user.Age < 0) throw new Exception("Invalid age");
    return user;
});

// ✅ Return a failure Result instead
var result = GetUser(id).Bind(user => 
    user.Age < 0 
        ? Result<User>.Failure(new ValidationException("Invalid age"))
        : Result<User>.Success(user));
```

### 5. Not Propagating Async Properly

```csharp
// ❌ Wrong - blocks the thread
var result = GetUserAsync(id).Result;  // Blocks!

// ✅ Correct - await the result
var result = await GetUserAsync(id);
```

---

## Examples

### Repository Pattern

```csharp
public class UserRepository : IUserRepository
{
    public Result<User> GetById(int id)
    {
        try
        {
            var user = _dbContext.Users.Find(id);
            if (user is null)
                return Result<User>.Failure(new NotFoundException($"User {id} not found"));
            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(new RepositoryException("Database error", ex));
        }
    }
}
```

### Service Layer

```csharp
public class UserService
{
    public async Task<Result<UserDto>> GetUserAsync(int id)
    {
        return await _repository.GetById(id)
            .BindAsync(async user =>
            {
                await _cache.SetAsync($"user:{id}", user);
                return Result<User>.Success(user);
            })
            .Map(user => new UserDto(user));
    }
}
```

### API Controller

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id)
{
    var result = await _userService.GetUserAsync(id);

    return result.Match(
        onSuccess: user => Ok(user),
        onFailure: ex => ex switch
        {
            NotFoundException => NotFound(ex.Message),
            ValidationException vex => BadRequest(vex.Errors),
            _ => StatusCode(500, "An error occurred")
        }
    );
}
```

### Validation Pipeline

#### Simple Version (Recommended)

```csharp
public async Task<Result<Order>> CreateOrderAsync(OrderRequest request)
{
    return await ValidateCustomerAsync(request.CustomerId)
        .BindAsync(customer => ValidateProductsAsync(request.Products)
            .BindAsync(products => CreateOrderAsync(request, customer, products)));
}
```

#### With Helper Methods (Most Maintainable)

```csharp
public async Task<Result<Order>> CreateOrderAsync(OrderRequest request)
{
    return await ValidateCustomerAsync(request.CustomerId)
        .BindAsync(customer => CreateOrderWithCustomer(request, customer));
}

private async Task<Result<Order>> CreateOrderWithCustomer(OrderRequest request, Customer customer)
{
    return await ValidateProductsAsync(request.Products)
        .BindAsync(ValidateInventoryAsync)
        .BindAsync(products => FinalizeOrderAsync(request, customer, products));
}
```

### Railway-Oriented Programming

```csharp
// Each step either succeeds and passes data to the next,
// or fails and short-circuits the pipeline
public Result<ProcessedOrder> ProcessOrder(OrderRequest request)
{
    return Validate(request)
        .Bind(Authorize)
        .Bind(CalculatePricing)
        .Bind(CheckInventory)
        .Bind(CreateOrder)
        .Map(order => new ProcessedOrder(order));
    // If any step fails, subsequent steps are skipped automatically!
}
```

### Data Transformation Pipeline

```csharp
public Result<ReportDto> GenerateReport(int userId)
{
    return GetUser(userId)
        .Map(user => user.Orders)
        .Map(orders => orders.Where(o => o.IsComplete))
        .Map(orders => orders.Sum(o => o.Total))
        .Map(total => new ReportDto
        {
            UserId = userId,
            TotalSpent = total,
            GeneratedAt = DateTime.UtcNow
        });
}
```

---

## License

see LICENSE file for details.
