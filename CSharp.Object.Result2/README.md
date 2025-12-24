# FrenchExDev.Net.CSharp.Object.Result2

A lightweight, strongly-typed Result pattern implementation for .NET that enables elegant error handling without relying on exceptions for control flow.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Concepts](#core-concepts)
  - [Creating Results](#creating-results)
  - [Checking Result State](#checking-result-state)
  - [Accessing Values and Exceptions](#accessing-values-and-exceptions)
- [API Reference](#api-reference)
  - [Result\<TResult\>](#resulttresult)
  - [Pattern Matching](#pattern-matching)
  - [Fluent Handlers](#fluent-handlers)
  - [Async Support](#async-support)
- [Testing Support](#testing-support)
- [Exception Types](#exception-types)
- [Best Practices](#best-practices)
- [Examples](#examples)

---

## Overview

The Result pattern provides a way to represent the outcome of an operation that can either succeed with a value or fail with an exception. This approach offers several benefits:

- **Explicit error handling** - Callers must acknowledge that an operation can fail
- **No exception-based control flow** - Exceptions are reserved for truly exceptional situations
- **Type safety** - The compiler helps ensure proper handling of both success and failure cases
- **Fluent API** - Chain operations elegantly with method chaining support

## Installation

Add the package reference to your project:

```xml
<PackageReference Include="FrenchExDev.Net.CSharp.Object.Result2" Version="1.0.0" />
```

For testing support, also add:

```xml
<PackageReference Include="FrenchExDev.Net.CSharp.Object.Result2.Testing" Version="1.0.0" />
```

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
var result = Result<User>.Failure(new UserNotFoundException("User not found"));

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

### Result\<TResult\>

A readonly struct representing the outcome of an operation.

| Member | Type | Description |
|--------|------|-------------|
| `IsSuccess` | `bool` | Returns `true` if the operation succeeded |
| `Value` | `TResult` | Gets the success value (throws if failure) |
| `Success(TResult value)` | `static` | Creates a successful result |
| `Failure<TException>(TException exception)` | `static` | Creates a failed result |
| `Exception<TException>()` | `TException` | Gets the exception (throws if success) |
| `TryGetSuccessValue(out TResult? value)` | `bool` | Safely attempts to get the value |
| `TryGetException<TException>(out TException? exception)` | `bool` | Safely attempts to get the exception |

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

// Method chaining (Match returns the result)
result
    .Match(
        onSuccess: user => Audit("User retrieved"),
        onFailure: ex => Audit($"Retrieval failed: {ex.Message}"))
    .IfSuccess(user => SendNotification(user));
```

#### MatchAsync

For asynchronous handlers, use `MatchAsync` to handle both success and failure cases:

```csharp
var result = GetUserById(123);

// Basic async match
await result.MatchAsync(
    onSuccess: async user =>
    {
        await _cache.SetAsync(user.Id, user);
        await _eventBus.PublishAsync(new UserRetrievedEvent(user));
    },
    onFailure: async ex =>
    {
        await _logger.ErrorAsync("Failed to retrieve user", ex);
        await _alertService.NotifyAsync("User retrieval failed");
    });

// Async match with typed exception
await result.MatchAsync<UserNotFoundException>(
    onSuccess: async user => await SaveToCacheAsync(user),
    onFailure: async ex => await LogNotFoundAsync(ex.UserId)
);

// Method chaining with async match
await result
    .MatchAsync(
        onSuccess: async user => await AuditAsync("User retrieved"),
        onFailure: async ex => await AuditAsync($"Retrieval failed: {ex.Message}"))
    .ContinueWith(t => t.Result.IfSuccess(user => SendNotification(user)));
```

#### Match Methods Reference

| Method | Description |
|--------|-------------|
| `Match(onSuccess, onFailure)` | Synchronously handles success or failure with `Action` delegates |
| `Match<TException>(onSuccess, onFailure)` | Synchronously handles success or typed exception failure |
| `MatchAsync(onSuccess, onFailure)` | Asynchronously handles success or failure with `Func<T, Task>` delegates |
| `MatchAsync<TException>(onSuccess, onFailure)` | Asynchronously handles success or typed exception failure |

### Fluent Handlers

Execute actions based on result state with method chaining support:

#### IfSuccess

Execute an action only when the result is successful:

```csharp
result
    .IfSuccess(value => Console.WriteLine($"Got: {value}"))
    .IfSuccess(value => SaveToDatabase(value));
```

#### IfException

Execute an action only when the result is a failure:

```csharp
// Handle any exception
result.IfException(ex => Logger.Error(ex));

// Handle specific exception type
result.IfException<ValidationException>(ex => 
    ShowValidationErrors(ex.Errors));
```

#### Chaining Multiple Handlers

```csharp
GetUserResult()
    .IfSuccess(user => Console.WriteLine($"User: {user.Name}"))
    .IfSuccess(user => AuditLog.Record($"Retrieved user {user.Id}"))
    .IfException(ex => Logger.Error("Failed to get user", ex))
    .IfException<NotFoundException>(ex => Cache.Invalidate(ex.Key));
```

### Async Support

All fluent handlers and match methods have async variants for asynchronous operations:

#### MatchAsync

Handle both success and failure cases asynchronously:

```csharp
var result = Result<User>.Success(user);

await result.MatchAsync(
    onSuccess: async user =>
    {
        await database.SaveAsync(user);
        await cache.SetAsync(user.Id, user);
    },
    onFailure: async ex =>
    {
        await logger.ErrorAsync(ex);
        await metrics.IncrementAsync("user_save_failures");
    });

// With typed exception
await result.MatchAsync<DatabaseException>(
    onSuccess: async user => await SaveUserAsync(user),
    onFailure: async ex => await HandleDatabaseErrorAsync(ex));
```

#### IfSuccessAsync

```csharp
var result = Result<User>.Success(user);

await result.IfSuccessAsync(async user =>
{
    await database.SaveAsync(user);
    await cache.InvalidateAsync(user.Id);
});
```

#### IfExceptionAsync

```csharp
await result.IfExceptionAsync(async ex =>
{
    await Logger.ErrorAsync(ex);
    await AlertService.NotifyAsync("Operation failed");
});

// With typed exception
await result.IfExceptionAsync<DatabaseException>(async ex =>
{
    await RetryQueue.EnqueueAsync(operation);
});
```

#### Complete Async Example

```csharp
await GetUserAsync(userId)
    .MatchAsync(
        onSuccess: async user =>
        {
            await _cache.SetAsync(user.Id, user);
            await _eventBus.PublishAsync(new UserRetrievedEvent(user));
        },
        onFailure: async ex =>
        {
            await _logger.ErrorAsync("Failed to retrieve user", ex);
        });

// Or using fluent handlers
await GetUserAsync(userId)
    .IfSuccessAsync(async user =>
    {
        await _cache.SetAsync(user.Id, user);
        await _eventBus.PublishAsync(new UserRetrievedEvent(user));
    })
    .IfExceptionAsync(async ex =>
    {
        await _logger.ErrorAsync("Failed to retrieve user", ex);
    });
```

#### Async Methods Reference

| Method | Description |
|--------|-------------|
| `MatchAsync(onSuccess, onFailure)` | Async pattern matching for both cases |
| `MatchAsync<TException>(onSuccess, onFailure)` | Async pattern matching with typed exception |
| `IfSuccessAsync(onSuccess)` | Async handler for success case only |
| `IfExceptionAsync(onFailure)` | Async handler for failure case only |
| `IfExceptionAsync<TException>(onFailure)` | Async handler for specific exception type |

---

## Testing Support

The `FrenchExDev.Net.CSharp.Object.Result2.Testing` package provides assertion helpers for unit testing:

```csharp
using FrenchExDev.Net.CSharp.Object.Result2.Testing;

[Fact]
public void GetUser_WithValidId_ReturnsSuccess()
{
    // Arrange
    var service = new UserService();

    // Act
    var result = service.GetUser(123);

    // Assert
    ResultTesting.ShouldBeSuccess(result);
}

[Fact]
public void GetUser_WithValidId_ReturnsCorrectUser()
{
    var result = service.GetUser(123);

    // Assert with expected value
    ResultTesting.ShouldBeSuccessWithValue(result, expectedUser);

    // Or get the value for further assertions
    var user = ResultTesting.ShouldBeSuccessWithValue(result);
    user.Name.ShouldBe("John");
}

[Fact]
public void GetUser_WithInvalidId_ReturnsFailure()
{
    var result = service.GetUser(-1);

    ResultTesting.ShouldBeFailure(result);
}

[Fact]
public void GetUser_WithInvalidId_ReturnsNotFoundException()
{
    var result = service.GetUser(-1);

    // Assert exception type
    var exception = ResultTesting.ShouldBeFailureWithException<User, NotFoundException>(result);
    exception.Message.ShouldContain("not found");
}

[Fact]
public void GetUser_WithInvalidId_ReturnsCorrectMessage()
{
    var result = service.GetUser(-1);

    // Assert exact message
    ResultTesting.ShouldBeFailureWithMessage<User, NotFoundException>(
        result, 
        "User with ID -1 not found");

    // Or assert message contains substring
    ResultTesting.ShouldBeFailureWithMessageContaining<User, NotFoundException>(
        result, 
        "not found");
}
```

### Testing API Reference

| Method | Description |
|--------|-------------|
| `ShouldBeSuccess<T>(result)` | Asserts result is successful |
| `ShouldBeSuccessWithValue<T>(result)` | Asserts success and returns value |
| `ShouldBeSuccessWithValue<T>(result, expected)` | Asserts success with specific value |
| `ShouldBeFailure<T>(result)` | Asserts result is a failure |
| `ShouldBeFailureWithException<T, TEx>(result)` | Asserts failure with exception type, returns exception |
| `ShouldBeFailureWithMessage<T, TEx>(result, message)` | Asserts failure with exact message |
| `ShouldBeFailureWithMessageContaining<T, TEx>(result, substring)` | Asserts failure with message containing substring |

---

## Exception Types

The library provides custom exception types for result-related errors:

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

### 1. Prefer Try Methods for Conditional Access

```csharp
// ✅ Good - safe access
if (result.TryGetSuccessValue(out var value))
{
    ProcessValue(value);
}

// ⚠️ Avoid - redundant check
if (result.IsSuccess)
{
    var value = result.Value; // Already checked IsSuccess
}
```

### 2. Use Match for Exhaustive Handling

```csharp
// ✅ Good - handles both cases explicitly
result.Match(
    onSuccess: value => HandleSuccess(value),
    onFailure: ex => HandleError(ex)
);

// ✅ Good - async exhaustive handling
await result.MatchAsync(
    onSuccess: async value => await HandleSuccessAsync(value),
    onFailure: async ex => await HandleErrorAsync(ex)
);

// ⚠️ Avoid - might forget to handle failure
if (result.IsSuccess)
{
    HandleSuccess(result.Value);
}
// What about failures?
```

### 3. Chain Fluent Methods for Clean Code

```csharp
// ✅ Good - fluent and readable
return await GetUserAsync(id)
    .IfSuccessAsync(user => _cache.SetAsync(user))
    .IfExceptionAsync(ex => _logger.ErrorAsync(ex));

// ✅ Good - using MatchAsync for comprehensive handling
await GetUserAsync(id)
    .MatchAsync(
        onSuccess: async user => await _cache.SetAsync(user),
        onFailure: async ex => await _logger.ErrorAsync(ex));

// ⚠️ Avoid - nested and harder to read
var result = await GetUserAsync(id);
if (result.IsSuccess)
{
    await _cache.SetAsync(result.Value);
}
else
{
    await _logger.ErrorAsync(result.Exception<Exception>());
}
```

### 4. Use Typed Exceptions for Specific Error Handling

```csharp
// ✅ Good - handle specific errors differently
result
    .IfException<ValidationException>(ex => ShowValidationErrors(ex))
    .IfException<NotFoundException>(ex => ShowNotFound())
    .IfException(ex => ShowGenericError(ex)); // Fallback

// ✅ Good - async with typed exceptions
await result.MatchAsync<ValidationException>(
    onSuccess: async value => await ProcessAsync(value),
    onFailure: async ex => await ShowValidationErrorsAsync(ex));

// ⚠️ Avoid - treating all errors the same
result.IfException(ex => ShowError(ex.Message));
```

### 5. Choose Match vs IfSuccess/IfException

```csharp
// ✅ Use Match when you need to handle BOTH cases
result.Match(
    onSuccess: value => SaveToDb(value),
    onFailure: ex => LogError(ex));

// ✅ Use IfSuccess/IfException when you only care about ONE case
result.IfSuccess(value => Console.WriteLine($"Got: {value}"));

// ✅ Use IfSuccess/IfException for multiple independent handlers
result
    .IfSuccess(value => Log(value))
    .IfSuccess(value => Cache(value))
    .IfException(ex => Alert(ex));
```

---

## Examples

### Repository Pattern

```csharp
public interface IUserRepository
{
    Result<User> GetById(int id);
    Result<IEnumerable<User>> GetAll();
    Result<User> Create(CreateUserRequest request);
}

public class UserRepository : IUserRepository
{
    public Result<User> GetById(int id)
    {
        try
        {
            var user = _dbContext.Users.Find(id);
            if (user is null)
            {
                return Result<User>.Failure(
                    new NotFoundException($"User {id} not found"));
            }
            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(
                new RepositoryException("Database error", ex));
        }
    }
}
```

### Service Layer

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger _logger;
    private readonly ICache _cache;

    public async Task<Result<UserDto>> GetUserAsync(int id)
    {
        var result = _repository.GetById(id);

        // Using MatchAsync for comprehensive handling
        await result.MatchAsync(
            onSuccess: async user =>
            {
                _logger.Info($"Retrieved user {id}");
                await _cache.SetAsync($"user:{id}", user);
            },
            onFailure: async ex =>
            {
                _logger.Error($"Failed to get user {id}", ex);
                await _metrics.IncrementAsync("user_retrieval_failures");
            });

        // Transform to DTO
        if (result.TryGetSuccessValue(out var user))
        {
            return Result<UserDto>.Success(new UserDto(user));
        }

        return Result<UserDto>.Failure(result.Exception<Exception>());
    }
}
```

### API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var result = await _userService.GetUserAsync(id);

        // Using Match for clean response mapping
        return result.Match<Exception>(
            onSuccess: user => Ok(user),
            onFailure: ex => ex switch
            {
                NotFoundException => NotFound(ex.Message),
                ValidationException vex => BadRequest(vex.Errors),
                _ => StatusCode(500, "An error occurred")
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);

        // Using MatchAsync with async response handling
        return await result.MatchAsync<Exception>(
            onSuccess: async user =>
            {
                await _eventBus.PublishAsync(new UserCreatedEvent(user));
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            },
            onFailure: async ex =>
            {
                await _logger.ErrorAsync("User creation failed", ex);
                return BadRequest(ex.Message);
            });
    }
}
```

### Validation Pipeline

```csharp
public async Task<Result<Order>> ValidateAndCreateOrderAsync(OrderRequest request)
{
    // Chain validations with MatchAsync
    var customerResult = await ValidateCustomerAsync(request.CustomerId);
    
    return await customerResult.MatchAsync(
        onSuccess: async customer =>
        {
            var productsResult = await ValidateProductsAsync(request.Products);
            
            return await productsResult.MatchAsync(
                onSuccess: async products =>
                {
                    var inventoryResult = await ValidateInventoryAsync(products);
                    
                    return await inventoryResult.MatchAsync(
                        onSuccess: async validProducts => 
                            await CreateOrderAsync(request, customer, validProducts),
                        onFailure: async ex => 
                            Result<Order>.Failure(ex));
                },
                onFailure: async ex => Result<Order>.Failure(ex));
        },
        onFailure: async ex => Result<Order>.Failure(ex));
}
```

### Event-Driven Processing

```csharp
public class OrderProcessor
{
    public async Task ProcessOrderAsync(Order order)
    {
        var result = await ValidateOrderAsync(order);

        await result.MatchAsync(
            onSuccess: async validOrder =>
            {
                await _paymentService.ProcessPaymentAsync(validOrder);
                await _inventoryService.ReserveItemsAsync(validOrder);
                await _notificationService.SendConfirmationAsync(validOrder);
                await _eventBus.PublishAsync(new OrderProcessedEvent(validOrder));
            },
            onFailure: async ex =>
            {
                await _logger.ErrorAsync($"Order {order.Id} failed", ex);
                await _notificationService.SendFailureNoticeAsync(order, ex);
                await _eventBus.PublishAsync(new OrderFailedEvent(order, ex));
            });
    }
}
```

---

## License

MIT License - see LICENSE file for details.
