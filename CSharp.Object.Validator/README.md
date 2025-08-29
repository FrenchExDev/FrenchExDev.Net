# FrenchExDev.Net.CSharp.Object.Validator

**FrenchExDev.Net.CSharp.Object.Validator** is a lightweight library providing interfaces and implementations for object validation in .NET. It offers an extensible, interface-oriented model for validating objects of any type, synchronously or asynchronously, making it easy to integrate custom validation rules into your applications.

## Features

- Generic interfaces for object validation (`IObjectValidator<T>`, `IAsyncValidator<T>`)
- Abstract base implementations to speed up custom validator creation
- Support for both synchronous and asynchronous validation
- Lambda-based validators for quick configuration
- Compatible with .NET 9

## Installation

Add the NuGet package to your project:

```powershell
dotnet add package FrenchExDev.Net.CSharp.Object.Validator
```

## Quick Start

### Lambda Synchronous Validation

```csharp
using FrenchExDev.Net.CSharp.Object.Validator;

var validator = new LambdaObjectValidator<MyClass>(instance => { 
	var results = new List<IValidation>(); 
	if (string.IsNullOrEmpty(instance.Name)) results.Add(new MyValidationError("Le nom est requis.")); 
	return results; 
});

var errors = validator.Validate(new MyClass { Name = "" });

foreach (var error in errors)
{
	Console.WriteLine(error.Message);
}
```

### Lambda Asynchronous Validation
```csharp
using FrenchExDev.Net.CSharp.Object.Validator;

var asyncValidator = new LambdaAsyncValidator<MyClass>(async instance => {
	var results = new List<IValidation>();
	if (string.IsNullOrEmpty(instance.Name)) results.Add(new MyValidationError("Le nom est requis."));
	return await Task.FromResult(results);
});

var asyncErrors = await asyncValidator.ValidateAsync(new MyClass { Name = "" });

foreach (var error in asyncErrors)
{
	Console.WriteLine(error.Message);
}
```

### Class Definitions

```csharp
public class MyClass
{
	public string Name { get; set; }
}

public class MyClassValidator : ObjectValidatorBase<MyClass>
{
	public override IEnumerable<IValidation> Validate(MyClass instance)
	{
		var results = new List<IValidation>();
		if (string.IsNullOrEmpty(instance.Name))
			results.Add(new MyValidationError("Le nom est requis."));
		return results;
	}
}
```
