# CSharp.Object.Result - Architecture Documentation

## Overview

CSharp.Object.Result is a .NET 9 library that provides a functional programming approach to error handling through the Result pattern. It eliminates the need for exception-based control flow by encapsulating operation outcomes (success or failure) in strongly-typed value types, enabling more explicit and composable error handling.

## Solution Structure

### Projects

#### Core Library

- **FrenchExDev.Net.CSharp.Object.Result**
  - Main library implementing the Result pattern
  - Contains `Result` (non-generic) and `Result<T>` (generic) types
  - Failure dictionary and builder for detailed error tracking
  - Extension methods for fluent API usage
  - Async/await support for asynchronous operations

#### Testing Projects

- **FrenchExDev.Net.CSharp.Object.Result.Testing**
  - Testing utilities and helpers
  - Contains `ResultTester` class for test scenarios

- **FrenchExDev.Net.CSharp.Object.Result.Tests**
  - Comprehensive unit tests using xUnit and Shouldly
  - Tests for all Result APIs, edge cases, and async operations
  - Validates failure dictionaries and exception handling

## Architecture Diagrams

### Project Structure

```mermaid
graph TD
    subgraph "CSharp.Object.Result Solution"
        Core[FrenchExDev.Net.CSharp.Object.Result<br/>Core Library]
        Testing[FrenchExDev.Net.CSharp.Object.Result.Testing<br/>Test Utilities]
        Tests[FrenchExDev.Net.CSharp.Object.Result.Tests<br/>Unit Tests]
    end
    
    Tests -->|depends on| Core
    Tests -->|depends on| Testing
    Testing -->|depends on| Core
    
    Tests -.->|xUnit| TestFramework[xUnit Test Framework]
    Tests -.->|Shouldly| Assertions[Shouldly Assertions]
    
    style Core fill:#4a90e2,stroke:#2d5a8c,stroke-width:3px,color:#fff
    style Testing fill:#7cb342,stroke:#558b2f,stroke-width:2px
    style Tests fill:#fb8c00,stroke:#e65100,stroke-width:2px
```

### Type Hierarchy and Relationships

```mermaid
classDiagram
    class IResult {
        <<interface>>
        +bool IsSuccess
    }
    
    class Result {
        <<struct>>
        +bool IsSuccess
        +bool IsFailure
        +Exception? Exception
        +Success() Result$
        +Failure() Result$
        +Failure(Exception) Result$
        +IfSuccess(Action~bool~) Result
        +IfSuccessAsync(Func~bool,Task~) Task~Result~
        +IfFailure(Action~Exception?~) Result
        +IfFailureAsync(Func~Exception?,Task~) Task~Result~
    }
    
    class Result~T~ {
        <<struct>>
        +bool IsSuccess
        +bool IsFailure
        +T Object
        +FailureDictionary? Failures
        +Success(T) Result~T~$
        +Failure() Result~T~$
        +Failure(FailureDictionary) Result~T~$
        +Failure(Action~FailureDictionaryBuilder~) Result~T~$
        +Failure(Exception) Result~T~$
        +Exception(Exception) Result~T~$
        +TryCatch~TResult,TException~(Func~TResult~) Result~TResult~$
        +TryCatch~TResult~(Func~Result~TResult~~) Result~TResult~$
        +ObjectOrNull() T?
        +ObjectOrThrow() T
        +FailuresOrThrow() FailureDictionary
        +TryGetSuccess(out T) bool
        +IfSuccess(Action~T~) Result~T~
        +IfSuccessAsync(Func~T,Task~) Task~Result~T~~
        +IfFailure(Action~FailureDictionary?~) Result~T~
        +IfFailureAsync(Func~FailureDictionary?,Task~) Task~Result~T~~
    }
    
    class FailureDictionary {
        +FailureDictionary()
        +FailureDictionary(IDictionary~string,List~object~~)
        +Add(string, object) FailureDictionary
    }
    
    class FailureDictionaryBuilder {
        +Add~T~(string, T) FailureDictionaryBuilder
        +Build() FailureDictionary
    }
    
    class Extensions {
        <<static>>
        +ToSuccess~T~(T) Result~T~
        +ToResult(bool) Result
        +ToFailure~T~(T, object) Result~T~
        +ToFailure~T~(Exception) Result~T~
    }
    
    class ResultException {
        <<exception>>
    }
    
    class InvalidResultAccessException {
        <<exception>>
    }
    
    IResult <|.. Result : implements
    IResult <|.. Result~T~ : implements
    Result~T~ ..> FailureDictionary : uses
    Result~T~ ..> FailureDictionaryBuilder : uses
    FailureDictionary --|> Dictionary : inherits
    FailureDictionaryBuilder ..> FailureDictionary : creates
    Extensions ..> Result : creates
    Extensions ..> Result~T~ : creates
    ResultException <|-- InvalidResultAccessException : inherits
    Result~T~ ..> InvalidResultAccessException : throws
```

### Result Pattern Flow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant Client
    participant Operation
    participant Result as Result<T>
    participant Handler as Error Handler
    
    rect rgb(230, 255, 245)
    Note over Client,Result: Success Path
    Client->>Operation: Execute operation
    Operation->>Operation: Perform work
    Operation->>Result: Create Success(value)
    Result-->>Client: Result<T> (IsSuccess: true)
    Client->>Result: IfSuccess(action)
    Result->>Client: Execute action with value
    Client->>Result: ObjectOrThrow()
    Result-->>Client: Return value
    end
    
    rect rgb(255, 240, 240)
    Note over Client,Handler: Failure Path
    Client->>Operation: Execute operation
    Operation->>Operation: Perform work (fails)
    Operation->>Result: Create Failure(exception/failures)
    Result-->>Client: Result<T> (IsSuccess: false)
    Client->>Result: IfFailure(action)
    Result->>Handler: Execute failure action
    Handler-->>Client: Handle error
    Client->>Result: ObjectOrThrow()
    Result-->>Client: Throw InvalidOperationException
    end
    
    rect rgb(230, 245, 255)
    Note over Client,Handler: TryCatch Pattern
    Client->>Operation: TryCatch(() => riskyOperation())
    Operation->>Operation: Execute risky code
    
    alt Operation succeeds
        Operation-->>Client: Result<T>.Success(value)
    else Operation throws exception
        Operation-->>Client: Result<T>.Failure(exception)
    end
    end
```

### Failure Dictionary Building Flow

```mermaid
flowchart TD
    Start([Create Failure]) --> Choice{Failure Type?}
    
    Choice -->|Simple| SimpleFail[Result.Failure]
    Choice -->|With Exception| ExceptionFail[Result.Failure exception]
    Choice -->|With Details| BuilderFail[Use FailureDictionaryBuilder]
    
    BuilderFail --> Builder[new FailureDictionaryBuilder]
    Builder --> AddEntry1[builder.Add key, value]
    AddEntry1 --> MoreEntries{More entries?}
    MoreEntries -->|Yes| AddEntryN[builder.Add key, value]
    AddEntryN --> MoreEntries
    MoreEntries -->|No| BuildDict[builder.Build]
    BuildDict --> CreateFail[Result.Failure dictionary]
    
    SimpleFail --> ReturnResult[Return Result]
    ExceptionFail --> StoreEx[Store in Failures Dictionary]
    StoreEx --> ReturnResult
    CreateFail --> ReturnResult
    
    ReturnResult --> ClientCheck{Client checks result?}
    ClientCheck -->|IsSuccess| AccessValue[Access .Object]
    ClientCheck -->|IsFailure| AccessFailures[Access .Failures/.FailuresOrThrow]
    
    AccessValue --> End([Use value])
    AccessFailures --> InspectFailures[Inspect failure details]
    InspectFailures --> End
    
    style Start fill:#4caf50,stroke:#2e7d32,stroke-width:2px,color:#000
    style End fill:#4caf50,stroke:#2e7d32,stroke-width:2px,color:#000
    style Builder fill:#2196f3,stroke:#1565c0,stroke-width:2px,color:#000
    style BuildDict fill:#ff9800,stroke:#e65100,stroke-width:2px,color:#000
```

### Async Result Pattern

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant Client
    participant AsyncOp as Async Operation
    participant Result as Result<T>
    participant SuccessHandler
    participant FailureHandler
    
    Client->>AsyncOp: await ExecuteAsync()
    AsyncOp->>AsyncOp: Perform async work
    
    alt Success
        rect rgb(230, 255, 245)
        Note over AsyncOp,SuccessHandler: Success Path - Async Handling
        AsyncOp->>Result: Create Success(value)
        Result-->>Client: Task<Result<T>> (success)
        Client->>Result: await result.IfSuccessAsync(async handler)
        Result->>SuccessHandler: await handler(value)
        SuccessHandler-->>Result: Completed
        Result-->>Client: Same Result<T>
        end
    else Failure
        rect rgb(255, 240, 240)
        Note over AsyncOp,FailureHandler: Failure Path - Async Handling
        AsyncOp->>Result: Create Failure(failures)
        Result-->>Client: Task<Result<T>> (failure)
        Client->>Result: await result.IfFailureAsync(async handler)
        Result->>FailureHandler: await handler(failures)
        FailureHandler-->>Result: Completed
        Result-->>Client: Same Result<T>
        end
    end
    
    Client->>Client: Continue with result
```

## Core Components

### Result (Non-Generic)

A readonly struct representing the outcome of an operation without a return value.

**Key Properties:**
- `IsSuccess`: Indicates success
- `IsFailure`: Indicates failure (inverse of IsSuccess)
- `Exception`: Optional exception for failure cases

**Static Factory Methods:**
- `Success()`: Creates successful result
- `Failure()`: Creates failed result
- `Failure(Exception)`: Creates failed result with exception

**Fluent Methods:**
- `IfSuccess(Action<bool>)`: Execute action on success
- `IfSuccessAsync(Func<bool, Task>)`: Async version
- `IfFailure(Action<Exception?>)`: Execute action on failure
- `IfFailureAsync(Func<Exception?, Task>)`: Async version

### Result<T> (Generic)

A readonly struct representing the outcome of an operation that returns a value of type T.

**Key Properties:**
- `IsSuccess`: Indicates success
- `IsFailure`: Indicates failure
- `Object`: The successful value (throws if accessed on failure)
- `Failures`: Optional failure dictionary with detailed error information

**Static Factory Methods:**
- `Success(T)`: Creates successful result with value
- `Failure()`: Creates failed result
- `Failure(FailureDictionary)`: Creates failed result with failure details
- `Failure(Action<FailureDictionaryBuilder>)`: Creates failed result using builder
- `Failure(Exception)`: Creates failed result from exception
- `Exception(Exception)`: Creates failed result storing exception in failures
- `TryCatch<TResult, TException>(Func<TResult>)`: Catches specific exception type
- `TryCatch<TResult>(Func<Result<TResult>>)`: Catches all exceptions

**Value Access Methods:**
- `ObjectOrNull()`: Returns value or null
- `ObjectOrThrow()`: Returns value or throws InvalidOperationException
- `FailuresOrThrow()`: Returns failures or throws InvalidResultAccessException
- `TryGetSuccess(out T)`: Pattern for safe value retrieval

**Fluent Methods:**
- `IfSuccess(Action<T>)`: Execute action with value on success
- `IfSuccessAsync(Func<T, Task>)`: Async version
- `IfFailure(Action<FailureDictionary?>)`: Execute action with failures on failure
- `IfFailureAsync(Func<FailureDictionary?, Task>)`: Async version

### FailureDictionary

A specialized dictionary mapping string keys to lists of failure objects.

**Purpose:** Stores multiple failure reasons grouped by category

**Key Methods:**
- `Add(string, object)`: Adds failure entry, appending to existing key's list
- Inherits all Dictionary functionality

**Usage Pattern:**
```csharp
var failures = new FailureDictionary();
failures.Add("ValidationError", "Email is required");
failures.Add("ValidationError", "Password too short");
failures.Add("BusinessRule", "User already exists");
```

### FailureDictionaryBuilder

Fluent builder for constructing FailureDictionary instances.

**Key Methods:**
- `Add<T>(string, T)`: Adds typed failure entry
- `Build()`: Creates independent FailureDictionary

**Features:**
- Type-safe entry addition
- Creates deep copy on Build() for isolation
- Supports method chaining

**Usage Pattern:**
```csharp
var result = Result<User>.Failure(d => d
    .Add("Email", "Invalid format")
    .Add("Email", "Already registered")
    .Add("Password", "Too weak"));
```

### Extension Methods

Static helper methods for creating Results fluently.

**Methods:**
- `ToSuccess<T>(this T)`: Converts value to success result
- `ToResult(this bool)`: Converts boolean to Result
- `ToFailure<T>(this T, object)`: Creates failure with subject
- `ToFailure<T>(this Exception)`: Converts exception to failure

**Usage Examples:**
```csharp
// Success
var result = user.ToSuccess();

// Failure
var failure = new InvalidOperationException("error").ToFailure<User>();

// Boolean to Result
var result = isValid.ToResult();
```

## Exception Types

### ResultException

Base exception class for all Result-related exceptions.

### InvalidResultAccessException

Thrown when accessing properties on results in invalid states:
- Accessing `Object` on failed Result<T>
- Calling `FailuresOrThrow()` on Result<T> without failures

## Design Patterns

### Railway Oriented Programming

The Result pattern implements Railway Oriented Programming:
- **Success track**: Operations continue with values
- **Failure track**: Operations propagate failures
- **Switching**: IfSuccess/IfFailure switch between tracks

### Monad-like Behavior

Result<T> exhibits monad-like properties:
- **Unit** (return): `ToSuccess()`, `Success()`
- **Bind** (flatMap): Can be chained through IfSuccess
- **Pure values**: Immutable structs

### Try-Catch Elimination

Replace exception handling with explicit result handling:

**Traditional:**
```csharp
try {
    var user = GetUser(id);
    return user;
} catch (Exception ex) {
    logger.LogError(ex, "Failed to get user");
    return null;
}
```

**Result Pattern:**
```csharp
return Result<User>
    .TryCatch(() => GetUser(id))
    .IfFailure(failures => logger.LogError("Failed to get user"));
```

## Usage Patterns

### Basic Success/Failure

```csharp
public Result<User> GetUser(int id)
{
    var user = _repository.Find(id);
    return user != null 
        ? Result<User>.Success(user)
        : Result<User>.Failure(d => d.Add("NotFound", $"User {id} not found"));
}
```

### Exception Handling

```csharp
public Result<Data> LoadData(string path)
{
    return Result<Data>.TryCatch<Data>(() =>
    {
        var content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Data>(content).ToSuccess();
    });
}
```

### Conditional Execution

```csharp
var result = GetUser(userId)
    .IfSuccess(user => logger.LogInfo($"Found user: {user.Name}"))
    .IfFailure(failures => logger.LogError($"Errors: {failures}"));

if (result.IsSuccess)
{
    ProcessUser(result.Object);
}
```

### Async Operations

```csharp
public async Task<Result<Order>> CreateOrderAsync(OrderRequest request)
{
    return await ValidateRequest(request)
        .IfSuccessAsync(async validRequest =>
        {
            await _repository.SaveAsync(validRequest);
        })
        .IfFailureAsync(async failures =>
        {
            await _logger.LogAsync($"Validation failed: {failures}");
        });
}
```

### Detailed Failure Tracking

```csharp
public Result<User> RegisterUser(UserRequest request)
{
    var errors = new List<string>();
    
    if (string.IsNullOrEmpty(request.Email))
        errors.Add("Email is required");
    if (request.Password.Length < 8)
        errors.Add("Password must be at least 8 characters");
        
    if (errors.Any())
    {
        return Result<User>.Failure(d =>
        {
            foreach (var error in errors)
                d.Add("ValidationError", error);
        });
    }
    
    return CreateUser(request).ToSuccess();
}
```

## Benefits

### Type Safety
- Compiler enforces handling of both success and failure cases
- No silent failures or ignored exceptions
- Explicit return types document possible failures

### Composability
- Fluent API enables method chaining
- Async support integrates with async/await
- Extension methods provide natural syntax

### Testability
- Results are easy to construct in tests
- No exception setup required
- Failure dictionaries enable detailed assertions

### Performance
- Struct-based implementation (no heap allocation)
- No exception throwing/catching overhead
- Efficient for high-throughput scenarios

### Readability
- Intent is clear from method signatures
- Success and failure paths are explicit
- Railway-oriented programming paradigm

## Testing Support

### Test Utilities

The Testing project provides helpers for:
- Creating test results
- Asserting on result states
- Mocking result-returning methods

### Test Coverage

Comprehensive tests validate:
- Success and failure creation
- Property access and exceptions
- Conditional execution (If*/async)
- Extension method behavior
- Failure dictionary building
- Exception to failure conversion
- TryCatch patterns
- Edge cases and error conditions

## Best Practices

### When to Use Result Pattern

**Use Result for:**
- Expected failures (validation, not found, etc.)
- Business rule violations
- External service failures
- Parsing/conversion operations

**Don't use Result for:**
- Truly exceptional conditions (out of memory, etc.)
- Framework exceptions that should propagate
- Internal programming errors (nullref, etc.)

### Failure Dictionary Usage

**Good:**
```csharp
return Result<User>.Failure(d => d
    .Add("Validation", "Email invalid")
    .Add("Validation", "Phone invalid")
    .Add("Business", "User exists"));
```

**Avoid:**
```csharp
// Too generic
return Result<User>.Failure(d => d.Add("Error", "Something failed"));

// Should use exception instead
return Result<User>.Failure(d => d.Add("Critical", "Out of memory"));
```

### Method Signatures

**Prefer:**
```csharp
public Result<User> FindUser(int id);
public async Task<Result<Order>> ProcessOrderAsync(Order order);
```

**Over:**
```csharp
public User FindUser(int id); // throws if not found
public async Task<Order?> ProcessOrderAsync(Order order); // null on error
```

## Future Enhancements

- **Result.Combine()**: Combine multiple results
- **Result.Map/Bind**: Functional transformations
- **Result.Match()**: Pattern matching helper
- **Source generators**: Generate result handling code
- **Logging integration**: Automatic failure logging
- **Serialization support**: JSON/XML serialization
- **Result<T, TError>**: Typed error results

---

**Version**: 1.0  
**Last Updated**: 2024  
**Target Framework**: .NET 9  
**Architecture Style**: Functional Error Handling Library
