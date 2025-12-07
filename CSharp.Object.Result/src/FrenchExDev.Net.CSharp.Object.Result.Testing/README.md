# FrenchExDev.Net.CSharp.Object.Result.Testing

A testing utility library for the `FrenchExDev.Net.CSharp.Object.Result` package, providing assertion methods and fluent extensions to simplify unit testing of Result pattern implementations.

## ?? Installation

Add a reference to this project in your test project:

```xml
<ProjectReference Include="..\src\FrenchExDev.Net.CSharp.Object.Result.Testing\FrenchExDev.Net.CSharp.Object.Result.Testing.csproj" />
```

## ?? Features

- **Static Assertions** - `ResultAssert` class with traditional assertion methods
- **Fluent Extensions** - `ShouldBe*` extension methods for readable test code
- **Custom Exception** - `ResultAssertException` for clear test failure messages
- **Full Coverage** - Support for both `Result` and `Result<T>` types

## ?? Usage

### Using Static Assertions

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;

public class MyServiceTests
{
    [Fact]
    public void Operation_ReturnsSuccess()
    {
        // Arrange
        var service = new MyService();

        // Act
        var result = service.DoSomething();

        // Assert
        ResultAssert.IsSuccess(result);
    }

    [Fact]
    public void Operation_ReturnsFailure()
    {
        // Arrange
        var service = new MyService();

        // Act
        var result = service.DoSomethingThatFails();

        // Assert
        ResultAssert.IsFailure(result);
    }
}
```

### Using Fluent Extensions

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;

public class UserServiceTests
{
    [Fact]
    public void GetUser_WithValidId_ReturnsUser()
    {
        // Arrange
        var service = new UserService();

        // Act
        var result = service.GetUser(1);

        // Assert - Fluent style
        result.ShouldBeSuccess()
              .ShouldHaveValueMatching(user => user.Id == 1);
    }

    [Fact]
    public void GetUser_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        var service = new UserService();

        // Act
        var result = service.GetUser(-1);

        // Assert - Fluent style
        result.ShouldBeFailure()
              .ShouldHaveFailureKey("ValidationError");
    }
}
```

## ?? API Reference

### ResultAssert (Static Class)

#### Success/Failure Assertions

```csharp
// Non-generic Result
ResultAssert.IsSuccess(result);
ResultAssert.IsSuccess(result, "Custom error message");

ResultAssert.IsFailure(result);
ResultAssert.IsFailure(result, "Custom error message");

// Generic Result<T>
ResultAssert.IsSuccess(result);
ResultAssert.IsFailure(result);
```

#### Value Assertions

```csharp
// Assert exact value
ResultAssert.HasValue(result, "expected value");

// Assert value matches predicate
ResultAssert.HasValueMatching(result, value => value.Length > 5);
```

#### Exception Assertions (Non-generic Result)

```csharp
// Assert has any exception
ResultAssert.HasException(result);

// Assert has specific exception type
var ex = ResultAssert.HasException<InvalidOperationException>(result);
```

#### Failure Dictionary Assertions (Generic Result<T>)

```csharp
// Assert failure contains key
ResultAssert.HasFailureKey(result, "ValidationError");

// Assert failure contains key with specific value
ResultAssert.HasFailure(result, "Field", "Username");

// Assert failure contains exception
var ex = ResultAssert.HasFailureException<User>(result);

// Assert failure contains specific exception type
var ex = ResultAssert.HasFailureException<User, ArgumentException>(result);
```

### ResultAssertExtensions (Fluent Methods)

#### Non-generic Result

```csharp
result.ShouldBeSuccess();
result.ShouldBeFailure();
result.ShouldHaveException();
var ex = result.ShouldHaveException<InvalidOperationException>();
```

#### Generic Result<T>

```csharp
result.ShouldBeSuccess();
result.ShouldBeFailure();
result.ShouldHaveValue("expected");
result.ShouldHaveValueMatching(v => v.IsValid);
result.ShouldHaveFailureKey("Error");
result.ShouldHaveFailure("Error", "Something went wrong");
var ex = result.ShouldHaveFailureException();
var ex = result.ShouldHaveFailureException<MyType, ArgumentException>();
```

## ?? Complete Test Examples

### Testing a Repository

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;
using Xunit;

public class ProductRepositoryTests
{
    private readonly ProductRepository _repository = new();

    [Fact]
    public void GetById_ExistingProduct_ReturnsSuccess()
    {
        var result = _repository.GetById(1);

        result.ShouldBeSuccess()
              .ShouldHaveValueMatching(p => p.Id == 1 && p.Name != null);
    }

    [Fact]
    public void GetById_NonExistingProduct_ReturnsFailure()
    {
        var result = _repository.GetById(999);

        result.ShouldBeFailure()
              .ShouldHaveFailureKey("NotFound");
    }

    [Fact]
    public void Create_ValidProduct_ReturnsCreatedProduct()
    {
        var product = new Product { Name = "Test Product", Price = 9.99m };

        var result = _repository.Create(product);

        ResultAssert.IsSuccess(result);
        ResultAssert.HasValueMatching(result, p => p.Id > 0);
    }

    [Fact]
    public void Create_InvalidProduct_ReturnsValidationErrors()
    {
        var product = new Product { Name = "", Price = -1 };

        var result = _repository.Create(product);

        result.ShouldBeFailure()
              .ShouldHaveFailure("ValidationError", "Name is required")
              .ShouldHaveFailure("ValidationError", "Price must be positive");
    }
}
```

### Testing Exception Handling

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;
using Xunit;

public class FileServiceTests
{
    [Fact]
    public void ReadFile_NonExistingFile_ReturnsFailureWithException()
    {
        var service = new FileService();

        var result = service.ReadFile("nonexistent.txt");

        result.ShouldBeFailure();
        var ex = result.ShouldHaveFailureException<string, FileNotFoundException>();
        Assert.Contains("nonexistent.txt", ex.Message);
    }

    [Fact]
    public void NonGenericResult_WithException_CanBeVerified()
    {
        var result = Result.Failure(new ArgumentNullException("param"));

        result.ShouldBeFailure()
              .ShouldHaveException();

        var ex = result.ShouldHaveException<ArgumentNullException>();
        Assert.Equal("param", ex.ParamName);
    }
}
```

### Testing with Custom Messages

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public void PlaceOrder_EmptyCart_ReturnsFailure()
    {
        var service = new OrderService();
        var emptyCart = new Cart();

        var result = service.PlaceOrder(emptyCart);

        // Custom message appears in test output on failure
        ResultAssert.IsFailure(result, "PlaceOrder should fail when cart is empty");
        ResultAssert.HasFailureKey(result, "CartEmpty", "Expected CartEmpty error for empty cart");
    }
}
```

### Chaining Multiple Assertions

```csharp
using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.Object.Result.Testing;
using Xunit;

public class ValidationTests
{
    [Fact]
    public void Validate_InvalidInput_ReturnsMultipleErrors()
    {
        var validator = new UserValidator();
        var invalidUser = new User { Email = "invalid", Age = -5 };

        var result = validator.Validate(invalidUser);

        // Chain multiple assertions fluently
        result.ShouldBeFailure()
              .ShouldHaveFailureKey("Email")
              .ShouldHaveFailureKey("Age")
              .ShouldHaveFailure("Email", "Invalid email format")
              .ShouldHaveFailure("Age", "Age must be positive");
    }
}
```

## ??? Requirements

- .NET 10.0 or later
- Reference to `FrenchExDev.Net.CSharp.Object.Result`

## ?? License

See the [LICENSE](../../LICENSE.md) file for details.
