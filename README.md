# FrenchExDev.Net

A comprehensive .NET 9 ecosystem providing infrastructure libraries, development tools, and utilities for building modern .NET applications.

## Table of Contents

- [Overview](#overview)
- [Solution Directories](#solution-directories)
- [Quick Links](#quick-links)
- [Getting Started](#getting-started)
- [Testing Infrastructure](#testing-infrastructure)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

FrenchExDev.Net is a modular .NET 9 ecosystem consisting of multiple specialized solution directories, each focusing on specific aspects of .NET development. The repository provides infrastructure libraries, development tools, testing utilities, and comprehensive documentation.

**Key Features:**
- **.NET 9** target framework
- Modular architecture with independent solution directories
- Comprehensive testing infrastructure with code coverage
- Mermaid diagrams for architecture visualization
- PowerShell automation scripts
- Aspire distributed application support

---

## Solution Directories

| Solution Directory | Description | Documentation | Key Projects |
|--------------------|-------------|---------------|--------------|
| **Alpine.Version** | Alpine Linux version management and utilities | [Docs](Alpine.Version/README.md) | Version detection, Testing utilities |
| **CSharp.Aspire.Dev** | .NET Aspire development infrastructure | [Docs](CSharp.Aspire.Dev/doc/ARCHITECTURE.md) | DevAppHost, WebApplication, WebAssembly |
| **CSharp.ManagedDictionary** | Managed dictionary implementation with advanced features | [Docs](CSharp.ManagedDictionary/README.md) | Core library, Testing utilities |
| **CSharp.ManagedList** | Managed list implementation with enhanced functionality | [Docs](CSharp.ManagedList/README.md) | Core library, Testing utilities |
| **CSharp.Object.Biz** | Facetted object pattern implementation | [Docs](Object.Biz/doc/ARCHITECTURE.md) | FacetObject, Testing utilities |
| **CSharp.Object.Builder2** | Fluent object builder pattern implementation | [Docs](CSharp.Object.Builder2/README.md) | Builder pattern, Testing utilities |
| **CSharp.Object.Model** | Object modeling infrastructure | [Docs](CSharp.Object.Model/README.md) | Model abstractions, Infrastructure |
| **CSharp.Object.Result** | Result pattern implementation for error handling | [Docs](CSharp.Object.Result/README.md) | Result monad, Testing utilities |
| **CSharp.ProjectDependency5** | Distributed application example with Aspire | [Docs](CSharp.ProjectDependency5/README.md) | API, Viz2, Worker3, DevAppHost |
| **CSharp.SolutionEngineering** | Solution engineering tools and utilities | [Docs](CSharp.SolutionEngineering/README.md) | Solution management, Testing |
| **Dotnet.Project** | .NET project type abstractions and implementations | [Docs](Dotnet.Project/README.md) | Multiple project types (CLI, Desktop, WebAPI, etc.) |
| **Dotnet.Solution** | .NET solution management utilities | [Docs](Dotnet.Solution/README.md) | Solution parsing, Infrastructure |
| **HttpClient** | Enhanced HTTP client with testing support | [Docs](HttpClient/README.md) | HTTP utilities, Testing mocks |
| **Mm** | Module management system | [Docs](Mm/README.md) | Desktop, CLI, WebAPI, WebWorker apps |
| **Object.Biz** | Business object patterns and utilities | See CSharp.Object.Biz | Facet pattern implementation |
| **Packer** | Packaging and bundling utilities | [Docs](Packer/README.md) | Core packer, Bundle utilities |
| **Packer.Alpine** | Alpine Linux specific packaging | [Docs](Packer.Alpine/README.md) | Alpine packer, Testing |
| **Packer.Alpine.Docker** | Docker packaging for Alpine | [Docs](Packer.Alpine.Docker/README.md) | Docker infrastructure, Testing |
| **Packer.Alpine.Kubernetes** | Kubernetes packaging for Alpine | [Docs](Packer.Alpine.Kubernetes/README.md) | K8s abstractions, Testing |
| **Ssh.Config** | SSH configuration management | [Docs](Ssh.Config/README.md) | Config parsing, Testing |
| **Testing** | Testing utilities and infrastructure | [Docs](Testing/README.md) | Common testing utilities |
| **Vagrant** | Vagrant integration and management | [Docs](Vagrant/README.md) | Vagrant utilities, Testing |
| **VirtualBox.Version** | VirtualBox version detection and management | [Docs](VirtualBox.Version/README.md) | Version detection, DI, Testing |
| **Vos** | Vagrant On Steroid environment automation | [Docs](Vos/README.md) | Core abstractions, Infrastructure |
| **Vos.Alpine** | Vagrant On Steroid profiles for Alpine Linux | [Docs](Vos.Alpine/README.md) | Alpine scenarios, Testing |
| **Vos.Alpine.Docker** | Vagrant On Steroid Docker integrations | [Docs](Vos.Alpine.Docker/README.md) | Docker abstractions, Infrastructure |
| **Vos.Alpine.Kubernetes** | Vagrant On Steroid Kubernetes integrations | [Docs](Vos.Alpine.Kubernetes/README.md) | K8s abstractions, Infrastructure |

---

## Quick Links

### Architecture Documentation
- [CSharp.Aspire.Dev Architecture](CSharp.Aspire.Dev/doc/ARCHITECTURE.md) - Comprehensive Aspire development infrastructure
- [Object.Biz Architecture](Object.Biz/doc/ARCHITECTURE.md) - Facetted object pattern documentation
- [Testing Scripts](\_Scripts/README.md) - PowerShell testing infrastructure

### Development Tools
- **DevAppHost** - Local .NET Aspire development environment setup
- **Testing Scripts** - Automated test execution with coverage reporting
- **Packer Tools** - Cross-platform packaging utilities

### Core Libraries
- **Object Patterns** - Result, Facet, Builder patterns
- **Collections** - Managed List and Dictionary implementations
- **Infrastructure** - Project types, Solution management

---

## Getting Started

### Prerequisites
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PowerShell 7+** - For running test scripts
- **Git** - Version control

### Clone Repository

```bash
git clone https://github.com/FrenchExDev/FrenchExDev.Net.git
cd FrenchExDev.Net
```

### Build All Solutions

```powershell
# Restore tools
dotnet tool restore

# Build all projects
dotnet build FrenchExDev.Net.sln
```

### Run Tests

```powershell
# Run all tests with coverage
.\_Scripts\Run-SolutionTests.ps1

# Run tests for specific solution directory
.\_Scripts\Run-SolutionTests.ps1 -Include 'CSharp.Object'

# Generate merged coverage report
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateMergedReport `
    -GenerateCoverageJson `
    -GenerateCoverageHtml
```

---

## Testing Infrastructure

The repository includes a comprehensive PowerShell-based testing infrastructure:

### Features
- Automated test discovery across all solution directories
- Parallel test execution with throttling
- Code coverage collection (Cobertura XML)
- Merged coverage reports via ReportGenerator
- Interactive JSON-driven coverage viewers
- HTTP server support for report hosting

### Quick Start

```powershell
# List all test projects
.\_Scripts\Run-SolutionTests.ps1 -NoRun

# Run tests in parallel (up to 6 concurrent jobs)
.\_Scripts\Run-SolutionTests.ps1 -Parallel -MaxParallel 6

# Generate and serve coverage report
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -Serve
```

**Documentation:** [Testing Scripts README](_Scripts/README.md)

---

## Project Structure Convention

Each solution directory follows a consistent structure:

```
SolutionDirectory/
|-- README.md              # Solution overview and documentation
|-- doc/
|   |-- ARCHITECTURE.md    # Architecture diagrams and details
|   `-- *.md               # Additional documentation
|-- src/                   # Source projects
|   |-- ProjectName/
|   |-- ProjectName.Abstractions/
|   |-- ProjectName.Infrastructure/
|   `-- ProjectName.Testing/
`-- test/                  # Test projects
    `-- ProjectName.Tests/
```

---

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Follow naming conventions** - Solution directories use PascalCase
4. **Write tests** - Maintain or improve code coverage
5. **Update documentation** - Include README and architecture docs
6. **Commit changes** (`git commit -m 'Add amazing feature'`)
7. **Push to branch** (`git push origin feature/amazing-feature`)
8. **Open a Pull Request**

### Coding Standards
- Target .NET 9
- Use nullable reference types
- Follow C# coding conventions
- Include XML documentation comments
- Write comprehensive tests

---

## Architecture Patterns

The repository demonstrates several architectural patterns:

### **Facetted Object Pattern**
Dynamic composition of behaviors without modifying core structure  
**Location:** `Object.Biz/`

### **Result Pattern**
Type-safe error handling without exceptions  
**Location:** `CSharp.Object.Result/`

### **Builder Pattern**
Fluent object construction with validation  
**Location:** `CSharp.Object.Builder2/`

### **Type System**
.NET project type abstractions and implementations  
**Location:** `Dotnet.Project/`

---

## Technology Stack

### Frameworks & Runtimes
- .NET 9
- ASP.NET Core
- .NET Aspire

### Testing
- xUnit
- Shouldly
- coverlet
- ReportGenerator

### Tools
- PowerShell 7+
- Mermaid (diagram generation)
- mkcert (certificate generation)
- Docker & Kubernetes support

---

## License

This project is licensed under the MIT License - see individual solution directories for specific license files.

---

## Contact & Support

- **Repository:** [GitHub - FrenchExDev.Net](https://github.com/FrenchExDev/FrenchExDev.Net)
- **Issues:** [GitHub Issues](https://github.com/FrenchExDev/FrenchExDev.Net/issues)
- **Branch:** `develop`

---

## Quick Navigation

| Category | Solutions |
|----------|-----------|
| **Object Patterns** | Object.Biz, Object.Result, Object.Builder2, Object.Model |
| **Collections** | ManagedList, ManagedDictionary |
| **Infrastructure** | Dotnet.Project, Dotnet.Solution, SolutionEngineering |
| **Packaging** | Packer, Packer.Alpine, Packer.Alpine.Docker, Packer.Alpine.Kubernetes |
| **Vagrant On Steroid** | Vos, Vos.Alpine, Vos.Alpine.Docker, Vos.Alpine.Kubernetes |
| **DevOps** | Alpine.Version, VirtualBox.Version, Vagrant, Ssh.Config |
| **Web/API** | HttpClient, Aspire.Dev |
| **Applications** | Mm (Module Management), ProjectDependency5 (Example) |

---

**Last Updated:** January 2025  
**Target Framework:** .NET 9  
**Repository Structure:** Multi-solution monorepo