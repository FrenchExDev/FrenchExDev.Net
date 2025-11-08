# FrenchExDev.Net.CSharp.Object.Builder

`FrenchExDev.Net.CSharp.Object.Builder` is a .NET library designed to simplify and standardize the process of building and initializing complex objects in C#.  
It provides both synchronous and asynchronous builder patterns, making it easier to construct objects with multiple properties, dependencies, or configuration steps.

## Features

- Abstract base classes for implementing the builder pattern in C#
- Support for both synchronous and asynchronous object construction
- Extensible design for custom builders
- Clean separation of object configuration and instantiation logic

## Getting Started

1. Install the package via NuGet: `Install-Package FrenchExDev.Net.CSharp.Object.Builder`
2. Inherit from `AbstractBuilder<T>` or `AbstractAsyncBuilder<T>` to create your own builders or implement `IBuilder<T>` or `IAsyncBuilder<T>` interfaces directly.

## Example

```csharp
public class MyObjectBuilder : AbstractBuilder<MyObject> 
{
	public override MyObject Build() 
	{
		// Custom build logic here
		return new MyObject();
	}
}
```

## Requirements

- .NET 9 or later

## License

This project is licensed under the MIT License.

