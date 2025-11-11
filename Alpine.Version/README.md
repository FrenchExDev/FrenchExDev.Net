# Alpine.Version

A .NET 9 library for discovering, parsing, comparing, and searching Alpine Linux versions, architectures, and flavors from the official Alpine Linux repository.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
- [Architecture](#architecture)
- [Usage Examples](#usage-examples)
- [Testing](#testing)
- [Dependencies](#dependencies)
- [Contributing](#contributing)

---

## Overview

Alpine.Version provides a robust solution for working with Alpine Linux version information programmatically. It enables developers to query the official Alpine Linux CDN, parse version strings, compare versions using semantic versioning logic, and filter results by architecture, flavor, and release status.

**Key Capabilities:**
- Parse Alpine Linux version strings (major.minor.patch with RC support)
- Compare versions using semantic versioning rules
- Search the Alpine Linux CDN for available versions
- Filter by architecture (x86_64, aarch64, armv7, etc.)
- Filter by flavor (standard, extended, virt, etc.)
- Retrieve SHA256/SHA512 checksums for ISO files
- Support for "edge" (rolling release) versions
- Handle release candidates (RC) versions

---

## Features

### Version Parsing & Comparison
- Parse Alpine version strings: `3.18.2`, `3.19.0_rc1`, `edge`
- Compare versions using comprehensive comparison logic
- Support for major.minor.patch components
- Handle release candidates (RC) with proper ordering
- Special handling for "edge" versions

### Alpine Linux Repository Search
- Query official Alpine Linux CDN (dl-cdn.alpinelinux.org)
- Filter by minimum/maximum version ranges
- Filter by exact version match
- Filter by architecture (x86, x86_64, aarch64, armv7, etc.)
- Filter by flavor (standard, extended, virt, xen, rpi, etc.)
- Include/exclude release candidates
- Parallel HTTP requests for optimal performance

### Checksum Retrieval
- Fetch SHA256 checksums for verification
- Fetch SHA512 checksums for verification
- Parallel checksum retrieval
- Direct download URLs for ISO files

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- Internet connection (for querying Alpine Linux CDN)

### Installation

```bash
dotnet add package FrenchExDev.Net.Alpine.Version
```

### Quick Start

#### Parse and Compare Versions

```csharp
using FrenchExDev.Net.Alpine.Version;

// Parse Alpine version strings
var version1 = AlpineVersion.From("3.18.2");
var version2 = AlpineVersion.From("3.19.0");
var edgeVersion = AlpineVersion.From("edge");

// Compare versions
int comparison = version1.CompareTo(version2);  // Returns -1 (v1 < v2)

// Use comparison operators
bool isNewer = AlpineVersion.Compare(
    version2, 
    AlpineVersion.Operator.GreaterThan, 
    version1
);  // Returns true

// Access version components
Console.WriteLine($"Major: {version1.Major}");    // "3"
Console.WriteLine($"Minor: {version1.Minor}");    // "18"
Console.WriteLine($"Patch: {version1.Patch}");    // "2"
Console.WriteLine($"Is Edge: {version1.IsEdge}"); // false
```

#### Search Alpine Versions

```csharp
using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.HttpClient;

// Create HTTP client (using library HttpClient wrapper)
var httpClient = new StandardHttpClient();
var searcher = new AlpineVersionSearcher(httpClient);

// Search for Alpine 3.18+ versions, x86_64 architecture, standard flavor
var results = await searcher.SearchAsync(filters => filters
    .WithMinimumVersion("3.18.0")
    .WithArchitectures(new[] { AlpineArchitectures.x86_64 })
    .WithFlavors(new[] { AlpineFlavors.standard })
    .WithRc(false)  // Exclude release candidates
);

// Process results
foreach (var result in results)
{
    Console.WriteLine($"Version: {result.Version}");
    Console.WriteLine($"Architecture: {result.Architecture}");
    Console.WriteLine($"Flavor: {result.Flavor}");
    Console.WriteLine($"Download URL: {result.Url}");
    Console.WriteLine($"SHA256: {result.Sha256}");
    Console.WriteLine($"SHA512: {result.Sha512}");
    Console.WriteLine();
}
```

---

## Architecture

[Link to ARCHITECTURE.md for detailed diagrams and documentation](doc/ARCHITECTURE.md)

### Project Structure

| Project | Description |
|---------|-------------|
| **FrenchExDev.Net.Alpine.Version** | Core library with version parsing, comparison, and search functionality |
| **FrenchExDev.Net.Alpine.Version.Testing** | Test utilities and helper classes |
| **FrenchExDev.Net.Alpine.Version.Tests** | Comprehensive unit tests using xUnit |

---

## Usage Examples

### Example 1: Find Latest Stable Version

```csharp
var httpClient = new StandardHttpClient();
var searcher = new AlpineVersionSearcher(httpClient);

// Search for all versions, exclude RC, x86_64 only
var results = await searcher.SearchAsync(filters => filters
    .WithArchitectures(new[] { AlpineArchitectures.x86_64 })
    .WithRc(false)
);

// Get the latest version (results are ordered)
var latest = results.LastOrDefault();
if (latest != null)
{
    Console.WriteLine($"Latest stable: {latest.Version}");
    Console.WriteLine($"Download: {latest.Url}");
}
```

### Example 2: Version Range Query

```csharp
// Find all versions between 3.16 and 3.18
var results = await searcher.SearchAsync(filters => filters
    .WithMinimumVersion("3.16.0")
    .WithMaximumVersion("3.18.9")
    .WithArchitectures(new[] { AlpineArchitectures.x86_64, AlpineArchitectures.aarch64 })
);

Console.WriteLine($"Found {results.Count} matching versions");
```

### Example 3: Specific Version Lookup

```csharp
// Find exact version with all flavors
var results = await searcher.SearchAsync(filters => filters
    .WithExactVersion("3.19.0")
    .WithArchitectures(new[] { AlpineArchitectures.x86_64 })
);

// Results include all flavors for 3.19.0
foreach (var result in results)
{
    Console.WriteLine($"{result.Flavor}: {result.Url}");
}
```

### Example 4: Edge Version Query

```csharp
// Query the edge (rolling release) version
var results = await searcher.SearchAsync(filters => filters
    .WithExactVersion("edge")
    .WithArchitectures(new[] { AlpineArchitectures.x86_64 })
    .WithFlavors(new[] { AlpineFlavors.standard })
);

if (results.Any())
{
    Console.WriteLine($"Edge version available: {results.First().Url}");
}
```

---

## Testing

### Run Tests

```powershell
# Run all Alpine.Version tests
.\_Scripts\Run-SolutionTests.ps1 -Include 'Alpine.Version'

# Run with coverage
.\_Scripts\Run-SolutionTests.ps1 -Include 'Alpine.Version' -GenerateMergedReport
```

### Test Coverage

The library includes comprehensive tests for:
- **Version Parsing**: Valid and invalid version strings
- **Version Comparison**: Major, minor, patch, RC, edge versions
- **Version Operators**: All comparison operators (==, !=, >, >=, <, <=)
- **Search Functionality**: Version ranges, exact matches, architecture filters
- **Flavor Filtering**: Multiple flavors, regex matching
- **RC Handling**: Including/excluding release candidates
- **Edge Cases**: Null handling, empty strings, malformed versions

---

## Dependencies

### Internal Dependencies
- **FrenchExDev.Net.HttpClient** - HTTP client wrapper for web requests

### External Dependencies
- **System.Text.RegularExpressions** - URL and version parsing
- **System.Collections.Concurrent** - Thread-safe parallel operations
- **System.Runtime.Serialization** - Version serialization support

### Testing Dependencies
- **xUnit** - Testing framework
- **Shouldly** - Fluent assertions

---

## Contributing

See main repository [CONTRIBUTING.md](../CONTRIBUTING.md)

---

**Target Framework:** .NET 9  
**License:** MIT  
**Documentation:** [Architecture](doc/ARCHITECTURE.md)  
**Repository:** https://github.com/FrenchExDev/FrenchExDev.Net
