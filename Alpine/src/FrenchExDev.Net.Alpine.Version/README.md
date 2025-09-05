# FrenchExDev.Net.Alpine.Version

A .NET 9 library for representing and searching version information of Alpine. 

It provides a strongly-typed approach to handle Alpine versioning, including architecture and flavor metadata.
It also provides a searcher to find specific Alpine versions based on various criteria.
Finally it provides a filter builder to help construct search queries.

## Features
- **Searcher and Filter**: Find specific Alpine versions based on criteria.
- **Strongly-Typed Version Records**: Structured representation of version data with architecture and flavor fields.
- **Comparison and Equality**: Supports equality and comparison operations for version instances.
- **.NET 9 Compatibility**: Utilizes modern .NET features for safety and performance.

## Version comparing usage example

```csharp
using FrenchExDev.Net.Alpine.Version;

var version1 = new AlpineVersionArchFlavorRecord(1, 0, 0, AlpineArchitectures.aarch64, AlpineFlavors.Virt);
var version2 = new AlpineVersionArchFlavorRecord(1, 0, 0, AlpineArchitectures.aarch64, AlpineFlavors.Virt);

// Comparison
bool areEqual = version1 == version2; // true

// Display
Console.WriteLine(version1); // Output: 1.0.0-aarch64-virt
```

## Version searching usage example

```csharp
using FrenchExDev.Net.Alpine.Version;

var searcher = new AlpineVersionSearcher(new HttpClient());
var listOfVersions = await searcher.SearchAsync(filters, cancellationToken);
foreach(var version in listOfVersions)
{
	Console.WriteLine(version);
}
```

## Installation

Add the NuGet package to your .NET project:

```powershell
dotnet add package FrenchExDev.Net.Alpine.Version
```

---
