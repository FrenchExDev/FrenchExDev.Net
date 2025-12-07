# .Scripts

This folder contains helper scripts and a small PowerShell module to run tests across the repository.

## Table of Contents

- [Overview](#overview)
- [Entry Points](#entry-points)
  - [Run-TestProject.ps1](#run-testprojectps1)
  - [ConvertTo-CoverageJson.ps1](#convertto-coveragejsonps1)
- [Module](#module)
  - [Exported Functions](#exported-functions)
  - [Module Architecture](#module-architecture)
- [Sequence Diagrams](#sequence-diagrams)
  - [Run-SolutionTests Workflow](#run-solutiontests-workflow)
  - [Run-TestProject Workflow](#run-testproject-workflow)
  - [ConvertTo-CoverageJson Workflow](#convertto-coveragejson-workflow)
- [Clear-HostIfNecessary (Test Safety)](#clear-hostifnecessary-test-safety)
- [Usage Examples](#usage-examples)
  - [Basic Usage](#basic-usage)
  - [Coverage Reports](#coverage-reports)
  - [Interactive JSON Reports](#interactive-json-reports)
  - [HTTP Server](#http-server)
- [Tests](#tests)

---

## Overview

The `.Scripts` directory provides a comprehensive testing infrastructure for .NET solutions with:

- Automated test discovery and execution across solution projects
- Code coverage collection (Cobertura XML)
- Merged coverage reports using ReportGenerator
- Interactive JSON-driven coverage viewers
- Parallel test execution with throttling
- HTTP server support for local report hosting

---

## Entry Points

### Run-TestProject.ps1

Standalone script to run tests for a single project with coverage collection.

**Key Features:**
- Auto-discovers test project if not specified
- Supports both coverlet.msbuild and coverlet.collector
- Generates HTML coverage reports using ReportGenerator
- Configurable output directories
- Coverage threshold validation
- Browser auto-open support

**Parameters:**

| Parameter | Description | Default |
|-----------|-------------|---------|
| `ProjectPath` | Path to test project (auto-detected) | `''` |
| `Configuration` | Build configuration | `'Release'` |
| `ResultsDir` | Test results directory | `'TestResults'` |
| `ReportDir` | Coverage report directory | `'CoverageReport'` |
| `OutputDir` | Root output directory (overrides above) | `''` |
| `NoBuild` | Skip build phase | `$false` |
| `OpenReport` | Open report in browser | `$false` |
| `CoverageFormats` | Coverage output formats | `@('cobertura')` |
| `ReportTypes` | ReportGenerator output types | `@('Html')` |
| `UseMsBuildCoverlet` | Use coverlet.msbuild | `$false` |
| `IncludeFilters` | Coverage include filters | `@()` |
| `ExcludeFilters` | Coverage exclude filters | `@()` |
| `CoverageThreshold` | Minimum coverage percentage | `-1` |

---

### ConvertTo-CoverageJson.ps1

Utility script to convert Cobertura XML coverage reports to JSON format with optional HTML viewer generation.

**Key Features:**
- Parses Cobertura XML structure (packages, classes, lines)
- Generates compact JSON with coverage statistics
- Creates interactive HTML viewer using embedded template
- Calculates line-level and class-level coverage metrics
- Pure client-side HTML (no external dependencies)

**Parameters:**

| Parameter | Description | Default |
|-----------|-------------|---------|
| `CoverageXmlPath` | Path to Cobertura XML file (required) | - |
| `OutputDir` | Output directory (defaults to XML parent) | `''` |
| `Title` | Report title | `'Code Coverage Report'` |
| `GenerateHtml` | Generate HTML viewer | `$false` |
| `JsonFileName` | Output JSON filename | `'coverage.json'` |
| `HtmlFileName` | Output HTML filename | `'index.html'` |

---

## Module

The PowerShell module is located at `./Module/Module.psm1` and provides core functionality for test execution and coverage processing.

### Exported Functions

| Function | Description |
|----------|-------------|
| `Clear-HostIfNecessary` | Test-safe wrapper around `Clear-Host` |
| `Get-SolutionTestProjects` | Discover test projects from solution file |
| `Invoke-TestProject` | Run single test project with coverage |
| `Invoke-SolutionTests` | Run all solution test projects |
| `New-MergedCobertura` | Merge multiple Cobertura XML files |
| `Get-CoverageSummary` | Parse coverage statistics from XML |
| `Format-StatusTable` | Format test results as table |

---

### Module Architecture

```mermaid
classDiagram
    class Module {
        +Clear-HostIfNecessary()
    }
    
    class GetSolutionTestProjects {
        +Get-SolutionTestProjects(SolutionPath, IncludePatterns, ExcludePatterns)
        -ParseSolutionFile(path)
        -FilterProjects(projects, include, exclude)
    }
    
    class InvokeTestProject {
        +Invoke-TestProject(ProjectPath, Configuration, OutputDir, ...)
        -EnsureCoverletPackage()
        -CollectCoverageFiles()
        -ValidateCoverageThreshold()
    }
    
    class InvokeSolutionTests {
        +Invoke-SolutionTests(SolutionPath, Configuration, Parallel, ...)
        +New-MergedCobertura(InputFiles, OutputPath)
        -RunTestsSequential(projects)
        -RunTestsParallel(projects, maxParallel)
        -GenerateMergedReport()
        -GenerateCoverageJson()
    }
    
    class GetCoverageSummary {
        +Get-CoverageSummary(CoberturaXmlPath)
        -ParseXml(path)
        -CalculateMetrics()
    }
    
    class FormatStatusTable {
        +Format-StatusTable(TestResults)
        -CalculateColumnWidths()
        -RenderTable()
    }
    
    class ConvertToCoverageJson {
        +ConvertTo-CoverageJson(XmlPath, OutputDir, Title, GenerateHtml)
        -ParseCobertura(xml)
        -GenerateJson(data)
        -GenerateHtmlViewer(template, json)
    }
    
    Module ..> GetSolutionTestProjects : exports
    Module ..> InvokeTestProject : exports
    Module ..> InvokeSolutionTests : exports
    Module ..> GetCoverageSummary : exports
    Module ..> FormatStatusTable : exports
    
    InvokeSolutionTests --> GetSolutionTestProjects : uses
    InvokeSolutionTests --> InvokeTestProject : uses
    InvokeSolutionTests --> GetCoverageSummary : uses
    InvokeSolutionTests --> FormatStatusTable : uses
    InvokeSolutionTests --> ConvertToCoverageJson : uses
    
    InvokeTestProject --> GetCoverageSummary : uses
    
    style Module fill:#e3f2fd,stroke:#1976d2,stroke-width:2px,color:#000
    style GetSolutionTestProjects fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#000
    style InvokeTestProject fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px,color:#000
    style InvokeSolutionTests fill:#e8f5e9,stroke:#388e3c,stroke-width:2px,color:#000
    style GetCoverageSummary fill:#fce4ec,stroke:#c2185b,stroke-width:2px,color:#000
    style FormatStatusTable fill:#fff9c4,stroke:#f9a825,stroke-width:2px,color:#000
    style ConvertToCoverageJson fill:#e0f2f1,stroke:#00695c,stroke-width:2px,color:#000
```

---

## Sequence Diagrams

### Run-SolutionTests Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant User
    participant Script as Run-SolutionTests.ps1
    participant Module as Module.psm1
    participant Discovery as Get-SolutionTestProjects
    participant Runner as Invoke-SolutionTests
    participant TestProj as Invoke-TestProject
    participant DotNet as dotnet CLI
    participant RG as ReportGenerator
    participant JsonConv as ConvertTo-CoverageJson
    participant Server as npx serve
    
    User->>Script: Execute with parameters
    Script->>Module: Import-Module
    
    alt NoRun Mode
        rect rgb(230, 245, 255)
        Note over Script,Discovery: Dry-run mode: list projects only
        Script->>Discovery: Get-SolutionTestProjects()
        Discovery->>Discovery: Parse .sln/.slnx
        Discovery->>Discovery: Filter by Include/Exclude
        Discovery-->>Script: Project list
        Script->>User: Display projects (exit)
        end
    end
    
    Script->>Runner: Invoke-SolutionTests()
    Runner->>Discovery: Get-SolutionTestProjects()
    Discovery-->>Runner: Test projects
    
    alt Parallel Execution
        rect rgb(255, 245, 230)
        Note over Runner,DotNet: Parallel test execution
        loop For each project batch
            par Concurrent execution
                Runner->>TestProj: Invoke-TestProject (Job 1)
                Runner->>TestProj: Invoke-TestProject (Job 2)
                Runner->>TestProj: Invoke-TestProject (Job N)
            end
            
            TestProj->>DotNet: dotnet test with coverage
            DotNet-->>TestProj: Test results + coverage XML
        end
        end
    else Sequential Execution
        rect rgb(245, 255, 230)
        Note over Runner,DotNet: Sequential test execution
        loop For each project
            Runner->>TestProj: Invoke-TestProject()
            TestProj->>DotNet: dotnet test with coverage
            DotNet-->>TestProj: Test results + coverage XML
        end
        end
    end
    
    Runner->>Runner: Collect all coverage files
    
    opt GenerateMergedReport or GenerateCoverageJson
        rect rgb(255, 230, 245)
        Note over Runner,RG: Generate merged reports
        Runner->>Runner: New-MergedCobertura()
        Runner->>Runner: Merge XML files
        Runner-->>Runner: merged-coverage.cobertura.xml
        
        alt GenerateMergedReport
            Runner->>RG: reportgenerator
            RG->>RG: Generate HTML reports
            RG-->>Runner: Report files
        end
        
        alt GenerateCoverageJson
            Runner->>JsonConv: ConvertTo-CoverageJson()
            JsonConv->>JsonConv: Parse Cobertura XML
            JsonConv->>JsonConv: Generate JSON
            JsonConv-->>Runner: coverage.json
            
            opt GenerateCoverageHtml
                JsonConv->>JsonConv: Load HTML template
                JsonConv->>JsonConv: Replace placeholders
                JsonConv-->>Runner: index.html
            end
        end
        end
    end
    
    opt Serve
        rect rgb(230, 255, 245)
        Note over Runner,Server: Start HTTP server
        Runner->>Server: npx serve MergedReportDir
        Server->>Server: Start HTTP server
        Server->>User: Open browser
        end
    end
    
    Runner-->>Script: Test results summary
    Script->>User: Display summary table
```

---

### Run-TestProject Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant User
    participant Script as Run-TestProject.ps1
    participant FS as File System
    participant DotNet as dotnet CLI
    participant RG as ReportGenerator
    participant Browser
    
    User->>Script: Execute with parameters
    
    alt ProjectPath not provided
        rect rgb(230, 240, 255)
        Note over Script,FS: Auto-discover test project
        Script->>FS: Search for *Tests*.csproj
        FS-->>Script: First matching project
        end
    end
    
    Script->>Script: Resolve OutputDir
    Script->>FS: Create ResultsDir, ReportDir
    
    rect rgb(255, 245, 230)
    Note over Script,DotNet: Setup and preparation
    Script->>Script: Check ReportGenerator availability
    
    alt Tool manifest exists
        Script->>DotNet: dotnet tool restore
        DotNet-->>Script: Tools restored
    end
    end
    
    alt UseMsBuildCoverlet
        rect rgb(255, 240, 240)
        Note over Script,DotNet: MSBuild Coverlet mode
        Script->>FS: Check project for coverlet.msbuild
        
        opt Not found
            Script->>DotNet: dotnet add package coverlet.msbuild
            DotNet-->>Script: Package added
        end
        
        Script->>DotNet: dotnet test with /p:CollectCoverage=true
        end
    else Use Collector (default)
        rect rgb(240, 255, 240)
        Note over Script,DotNet: Collector mode (default)
        Script->>DotNet: dotnet test --collect:"XPlat Code Coverage"
        end
    end
    
    DotNet->>DotNet: Run tests
    DotNet->>FS: Write coverage XML
    DotNet-->>Script: Exit code + results
    
    Script->>FS: Search for coverage files
    FS-->>Script: Coverage XML paths
    
    alt CoverageThreshold >= 0
        rect rgb(255, 235, 235)
        Note over Script,Script: Validate coverage threshold
        Script->>Script: Parse coverage percentage
        
        alt Below threshold
            Script->>User: Exit with error
        end
        end
    end
    
    opt ReportGenerator available
        rect rgb(240, 245, 255)
        Note over Script,Browser: Generate HTML report
        Script->>RG: reportgenerator -reports:coverage.xml
        RG->>FS: Generate HTML report
        RG-->>Script: Report path
        
        opt OpenReport
            Script->>Browser: Start-Process index.html
        end
        end
    end
    
    Script-->>User: Success + coverage files
```

---

### ConvertTo-CoverageJson Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant User
    participant Script as ConvertTo-CoverageJson.ps1
    participant FS as File System
    participant XML as XML Parser
    participant JSON as JSON Converter
    
    User->>Script: Execute with CoverageXmlPath
    
    rect rgb(230, 245, 255)
    Note over Script,FS: Load and validate XML
    Script->>FS: Test-Path CoverageXmlPath
    FS-->>Script: File exists
    
    Script->>XML: Load Cobertura XML
    XML-->>Script: XML Document
    end
    
    rect rgb(245, 255, 230)
    Note over Script,Script: Parse coverage structure
    Script->>Script: Parse coverage structure
    
    loop For each package
        loop For each class
            Script->>Script: Extract class metadata
            
            loop For each line
                Script->>Script: Parse line number, hits
                Script->>Script: Calculate covered flag
            end
            
            Script->>Script: Calculate class line rate
            Script->>Script: Add to projects array
        end
    end
    end
    
    rect rgb(255, 245, 230)
    Note over Script,JSON: Generate JSON output
    Script->>Script: Calculate totals
    Script->>Script: Calculate overall percentage
    
    Script->>Script: Build output object
    Script->>JSON: ConvertTo-Json -Depth 10
    JSON-->>Script: JSON string
    
    Script->>FS: Write coverage.json
    FS-->>Script: File written
    end
    
    opt GenerateHtml
        rect rgb(255, 235, 245)
        Note over Script,FS: Generate HTML viewer
        Script->>FS: Load coverage-viewer.html template
        FS-->>Script: Template content
        
        Script->>Script: Replace __TITLE__ placeholder
        Script->>Script: Replace __JSON_FILE__ placeholder
        
        Script->>FS: Write index.html
        FS-->>Script: File written
        end
    end
    
    Script-->>User: JSON path + statistics
```

---

## Clear-HostIfNecessary (Test Safety)

The module exposes `Clear-HostIfNecessary` which delegates to the built-in `Clear-Host` in production.

**Purpose:** Tests should stub or mock `Clear-HostIfNecessary` to avoid clearing the terminal during automated test runs.

**Example (Pester):**
```powershell
Mock -CommandName Clear-HostIfNecessary -MockWith {}
```

---

## Usage Examples

### Basic Usage

#### Dry-list projects (no tests executed)

```powershell
Set-Location 'path/to/repo/root'
.\.Scripts\Run-SolutionTests.ps1 -NoRun
```

#### Run solution tests, skip build

```powershell
.\.Scripts\Run-SolutionTests.ps1 -NoBuild
```

#### Filter projects to include/exclude via substring pattern

```powershell
.\.Scripts\Run-SolutionTests.ps1 -Include 'CSharp.Object' -Exclude 'ProjectDependency'
```

#### Run tests and fail if coverage below threshold

```powershell
.\.Scripts\Run-SolutionTests.ps1 -UseMsBuildCoverlet -CoverageThreshold 80
```

---

### Coverage Reports

#### Generate merged coverage report (repo-local tool)

**Preferred approach — recommended for CI and reproducibility:**

1. Restore repository tools (once per workspace/CI job):

```powershell
dotnet tool restore
```

2. Run the script; the runner will prefer the repo-local tool if present:

```powershell
# Generate merged HTML report under the run folder
.\.Scripts\Run-SolutionTests.ps1 `
    -GenerateMergedReport `
    -ReportTypes Html `
    -WriteMergedCobertura `
    -RunOutputRoot '.\test\reports\my-run-2025-01-08'
```

**Notes:**
- This repository includes a tool manifest at `.config/dotnet-tools.json` pinning `dotnet-reportgenerator-globaltool`
- The script automatically tries `dotnet tool restore` if a manifest is present
- Prefers `dotnet tool run reportgenerator` for reproducible builds
- Avoids auto-installing global tools to prevent side-effects on developer machines

**Alternative (global tool):**
```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.4
```

#### Run tests in parallel with throttling

Useful for CI hosts with multiple cores:

```powershell
# Run with up to 6 concurrent test jobs
.\.Scripts\Run-SolutionTests.ps1 -Parallel -MaxParallel 6
```

**Notes on merging behavior:**
- `New-MergedCobertura` consolidates class entries by filename and classname
- Sums per-line hit counts across inputs
- Produces merged cobertura XML with accurate combined hit counts
- Computes `line-rate` attribute automatically

---

### Interactive JSON Reports

#### Automatic (Integrated with Run-SolutionTests)

The easiest way is to use the integrated parameters which automatically enable merged report generation:

```powershell
# JSON + HTML coverage report (auto-enables merged report & merged Cobertura)
.\.Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -CoverageReportTitle "My Project Coverage" `
    -RunOutputRoot "./test/reports/my-run"

# JSON only (no HTML viewer)
.\.Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -RunOutputRoot "./test/reports/my-run"

# Open the generated report
Start-Process "./test/reports/my-run/MergedReport/index.html"
```

**Note:** Specifying `-GenerateCoverageJson` or `-GenerateCoverageHtml` automatically enables:
- `-GenerateMergedReport` (creates merged HTML report via ReportGenerator)
- `-WriteMergedCobertura` (creates merged Cobertura XML required for JSON conversion)

#### Manual (Standalone Script)

You can also convert existing coverage reports manually:

```powershell
# Generate JSON only (default - minimal output)
.\.Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\test\reports\latest\MergedReport\merged-coverage.cobertura.xml"

# Generate JSON + HTML viewer (uses template from .Scripts/Module/Resources/)
.\.Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -OutputDir ".\web-report" `
    -Title "My Project Coverage" `
    -GenerateHtml

# Custom file names
.\.Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -JsonFileName "report-data.json" `
    -HtmlFileName "report.html" `
    -GenerateHtml

# Open the generated report
Start-Process ".\web-report\index.html"
```

**HTML Viewer Features:**
- **Separation of concerns**: JSON data can be consumed by other tools, HTML is optional
- **Interactive filtering**: Search by class name, filename, or coverage percentage
- **Expandable packages**: Click to view class-level details
- **Beautiful design**: Gradient visuals with responsive layout
- **Pure client-side**: No web server required, works offline
- **Color-coded indicators**: Visual coverage status (excellent/high/medium/low)

The HTML viewer template is stored in `.Scripts/Module/Resources/coverage-viewer.html` and uses simple placeholder replacement (`__TITLE__`, `__JSON_FILE__`) for customization.

---

### HTTP Server

#### Serve the Coverage Report via HTTP

To serve the merged report via a local HTTP server (useful for testing CORS-restricted features or sharing on local network):

```powershell
# Generate and serve the report (requires Node.js/npx)
.\.Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -CoverageReportTitle "My Project Coverage" `
    -Serve

# Specify custom port (default: 8080)
.\.Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -Serve `
    -ServePort 3000
```

The `-Serve` switch uses `npx serve` to start a local HTTP server serving the MergedReport directory. The browser will automatically open to view the report. Press `Ctrl+C` to stop the server.

**Requirements:** Node.js must be installed (includes npx). Download from https://nodejs.org/

---

## Tests

Pester tests are in `./.Scripts/test` and can be run with:

```powershell
Install-Module -Name Pester -Force -Scope CurrentUser
Invoke-Pester ./.Scripts/test -Verbose
```

**Test Coverage:**
- Module function exports
- Solution parsing (.sln/.slnx)
- Project filtering logic
- Coverage XML parsing
- Merged Cobertura generation
- JSON conversion accuracy

