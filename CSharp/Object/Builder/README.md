# FrenchExDev.Net.CSharp.Object.Builder

`FrenchExDev.Net.CSharp.Object.Builder` is a .NET 9 library that simplifies and standardizes the process of building and initializing complex objects in C#.  
It provides both synchronous and asynchronous builder patterns, making it easier to construct objects with multiple properties, dependencies, or configuration steps.

---

## Features

- Abstract base classes for implementing the builder pattern in C#
- Support for both synchronous and asynchronous object construction
- Extensible design for custom builders
- Clean separation of object configuration and instantiation logic
- Stepwise and lambda-based object building

---

## Key Classes

### LambdaStepObjectBuilder\<TClass\>

A builder that allows stepwise construction of an object using a lambda-based build action.

**Highlights:**
- Accepts a lambda to define custom build logic
- Tracks intermediate states, exceptions, and visited objects
- Implements `IStepObjectBuilder<TClass>`, which extends `IAbstractStep<TClass>`

**Usage Example:**
```csharp
var builder = new LambdaStepObjectBuilder<Person>((step, exceptions, intermediates, visited) => { 
	var person = new Person( name: intermediates.Get<string>("Name"), age: intermediates.Get<int>("Age") ); 
	step.Set(person); 
});
```

### Supporting Types

- **IntermediateObjectDictionary**:  
  A dictionary for storing and retrieving intermediate objects with type safety during the build process.

- **VisitedObjectsList**:  
  Tracks objects that have already been processed to prevent duplication or handle circular references.

- **ExceptionBuildList**:  
  A specialized list for collecting exceptions encountered during the build process.

- **IStepObjectBuilder\<TClass\>**:  
  Interface for step builders, supporting custom build steps and result management.

- **IAbstractStep\<TClass\>**:  
  Interface for steps that can produce a result and report if a result is available.

---

## Getting Started

1. Install the package via NuGet:
2. Inherit from `AbstractBuilder<T>` or `AbstractAsyncBuilder<T>` to create your own builders, or use `LambdaStepObjectBuilder<T>` for lambda-based steps.

---

## Example

```csharp
public class MyObjectBuilder : AbstractBuilder<MyObject> 
{ 
	public override IBuildObjectResult Build() { 
		// Custom build logic here return new MyObject(); 
	}
}
```

---

## Requirements

- .NET 9 or later

---

## License

This project is licensed under the MIT License.
