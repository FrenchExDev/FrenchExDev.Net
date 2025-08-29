# FrenchExDev.Net.Mm

**FrenchExDev.Net.Mm** is a lightweight, extensible framework for building modular monolithic applications in .NET 9. It provides a structured approach to organizing business logic, infrastructure, and application layers as independent modules, while maintaining the simplicity and performance of a monolith.

---

## Overview

This solution enables you to design .NET applications as a set of loosely coupled modules, each encapsulating a specific domain or feature. Modules can be developed, versioned, tested, and maintained independently, but are composed into a single deployable application. The framework supports a variety of application types, including CLI, Web API, Worker, and Desktop, and enforces code style and best practices at build time.

---

## Key Features

- **Modular Architecture**: Organize your application into self-contained modules with clear boundaries.
- **.NET 9 Support**: Built for the latest .NET platform, leveraging C# 13 features.
- **Flexible Integration**: Ready for CLI, Web API, Worker, and Desktop application types.
- **Versioning**: Built-in support for semantic and major-minor-patch versioning of modules.
- **Extensibility**: Easily add, remove, or update modules without impacting the application core.
- **Strong Typing and Contracts**: Abstractions for module information, versioning, and configuration.
- **Testing Support**: Dedicated test projects for each module and application type.
- **Code Style Enforcement**: .NET code conventions are enforced at build time.

---

## Solution Structure

The solution is organized into several core areas:

### Core Framework

- **FrenchExDev.Net.Mm**  
  The main framework project, providing the foundation for modular monolithic applications.

- **FrenchExDev.Net.Mm.Abstractions**  
  Contains interfaces and base types for module information, versioning, and contracts.  
  - `IModuleInformation`: Interface for module metadata (name, description, website, documentation).
  - `IModuleVersion`: Interface for module versioning.
  - `BasicModuleInformation`: Implementation of module metadata.
  - `MajorMinorPatchModuleVersion` & `SemanticModuleVersion`: Classes for versioning modules.
  - Builders and comparers for versioning.

### Infrastructure & Modules

- **FrenchExDev.Net.Mm.Infrastructure**  
  Provides infrastructure services and base implementations for cross-cutting concerns.

- **FrenchExDev.Net.Mm.Module**  
  Contains abstractions and base classes for defining and loading modules.

- **FrenchExDev.Net.Mm.Module.Library**  
  Example of a domain module, with its own dependencies, configuration, and loader.

### Application Types

- **FrenchExDev.Net.Mm.App.Cli**  
  Example CLI application using the modular framework.

- **FrenchExDev.Net.Mm.App.WebApi**  
  Example Web API application.

- **FrenchExDev.Net.Mm.App.WebWorker**  
  Example background worker application.

- **FrenchExDev.Net.Mm.App.Desktop**  
  Example desktop application.

Each application type has corresponding infrastructure, abstractions, and builder/generator projects to demonstrate modular composition and extensibility.

### Testing

- **Testing Projects**  
  Each module and application type includes a dedicated testing project to ensure correctness and maintainability.

---

## Versioning

The framework provides robust support for module versioning:

- **MajorMinorPatchModuleVersion**:  
  Simple versioning with major, minor, and patch numbers.  
  - Increment/decrement version parts.
  - String representation as `Major.Minor.Patch`.

- **SemanticModuleVersion**:  
  Full semantic versioning (SemVer) with support for pre-release and build metadata.  
  - String representation as `Major.Minor.Patch[-PreRelease][+BuildMetadata]`.
  - Comparers for equality and hash code generation.

- **Builders**:  
  Fluent builders for constructing version objects safely and consistently.

---

## Getting Started

Add the NuGet package reference to your .NET 9 project:

```powershell
dotnet add package FrenchExDev.Net.Mm
