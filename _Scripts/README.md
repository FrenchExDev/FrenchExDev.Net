# _Scripts

This folder contains helper scripts and a small PowerShell module to run tests across the repository.

## Entry points and utilities

- `Run-SolutionTests.ps1` - Entry script to run all test projects referenced by the solution.
- `Run-TestProject.ps1` - Standalone script to run a single test project with coverage.
- `ConvertTo-CoverageJson.ps1` - Convert Cobertura XML coverage reports to JSON and generate an interactive HTML viewer.

## Module

The module is located at `./Module/Module.psm1` and exports:

- `Get-SolutionTestProjects` - discover test projects from a `.sln` or `.slnx` file (auto-detects first solution in current folder if none provided).
- `Invoke-TestProject` - run `dotnet test` for a single test project and collect coverage artifacts.
- `Invoke-SolutionTests` - run all discovered test projects and return a summary object.
 - `New-MergedCobertura` - utility that merges multiple Cobertura XML files by consolidating classes/files and summing per-line hit counts. Use this when you want a combined cobertura file that reflects hits from multiple test runs.

## Clear-HostIfNecessary (test safety)

- The module exposes `Clear-HostIfNecessary` which delegates to the built-in `Clear-Host` in production.
- Tests should stub or mock `Clear-HostIfNecessary` (example in Pester: `Mock -CommandName Clear-HostIfNecessary -MockWith {}`) to avoid clearing the terminal during automated test runs.

## Usage examples

Dry-list projects (no tests executed):

```powershell
Set-Location 'path/to/repo/root'
.\_Scripts\Run-SolutionTests.ps1 -NoRun
```

Run solution tests, skip build:

```powershell
.\_Scripts\Run-SolutionTests.ps1 -NoBuild
```

Filter projects to include/exclude via substring pattern:

```powershell
.\_Scripts\Run-SolutionTests.ps1 -Include 'CSharp.Object' -Exclude 'ProjectDependency'
```

Run tests and fail if coverage below threshold (uses msbuild coverlet mode):

```powershell
.\_Scripts\Run-SolutionTests.ps1 -UseMsBuildCoverlet -CoverageThreshold 80
```

Generate a merged coverage report across all test projects (requires ReportGenerator).

Preferred (repo-local) approach — recommended for CI and reproducibility:

1. Restore repository tools (once per workspace/CI job):

```powershell
dotnet tool restore
```

2. Run the script; the runner will prefer the repo-local tool if present:

```powershell
.# generate a merged HTML report under the run folder (default: ./test/reports/<timestamp>/Run/MergedReport)
.\_Scripts\Run-SolutionTests.ps1 -GenerateMergedReport -ReportTypes Html -WriteMergedCobertura -RunOutputRoot '.\test\reports\my-run-2025-11-08'
```

Notes:
- This repository includes a tool manifest at `.config/dotnet-tools.json` pinning `dotnet-reportgenerator-globaltool` so `dotnet tool restore` will install a local copy. The script will automatically try `dotnet tool restore` if a manifest is present and prefer `dotnet tool run reportgenerator`.
- We avoid auto-installing a global tool from the script to prevent side-effects on developer machines. If you prefer a global installation you can run manually:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.4
```

Run tests in parallel with throttling (useful for CI hosts). `-Parallel` enables background job runs and `-MaxParallel` controls concurrency (default 4):

```powershell
# run with up to 6 concurrent test jobs
.\_Scripts\Run-SolutionTests.ps1 -Parallel -MaxParallel 6
```

Notes on merging behavior:
 - `New-MergedCobertura` consolidates class entries by filename and classname and sums per-line hit counts across inputs. This produces a merged cobertura XML with accurate combined hit counts and a computed `line-rate` attribute.
- The module will call ReportGenerator (global tool) to produce human-readable reports. If `reportgenerator` is not available the script will attempt to install `dotnet-reportgenerator-globaltool` globally.

## Generate interactive JSON-driven coverage report

### Automatic (Integrated with Run-SolutionTests)

The easiest way is to use the integrated parameters which automatically enable merged report generation:

```powershell
# JSON + HTML coverage report (auto-enables merged report & merged Cobertura)
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -CoverageReportTitle "My Project Coverage" `
    -RunOutputRoot "./test/reports/my-run"

# JSON only (no HTML viewer)
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -RunOutputRoot "./test/reports/my-run"

# Open the generated report
Start-Process "./test/reports/my-run/MergedReport/index.html"
```

**Note**: Specifying `-GenerateCoverageJson` or `-GenerateCoverageHtml` automatically enables:
- `-GenerateMergedReport` (creates merged HTML report via ReportGenerator)
- `-WriteMergedCobertura` (creates merged Cobertura XML required for JSON conversion)

### Manual (Standalone Script)

You can also convert existing coverage reports manually:

```powershell
# Generate JSON only (default - minimal output)
.\_Scripts\ConvertTo-CoverageJson.ps1 -CoverageXmlPath ".\test\reports\latest\MergedReport\merged-coverage.cobertura.xml"

# Generate JSON + HTML viewer (uses template from _Scripts/Module/Resources/)
.\_Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -OutputDir ".\web-report" `
    -Title "My Project Coverage" `
    -GenerateHtml

# Custom file names
.\_Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -JsonFileName "report-data.json" `
    -HtmlFileName "report.html" `
    -GenerateHtml

# Open the generated report
Start-Process ".\web-report\index.html"
```

The HTML viewer template is stored in `_Scripts/Module/Resources/coverage-viewer.html` and uses simple placeholder replacement (`__TITLE__`, `__JSON_FILE__`) for customization.

### Serve the Coverage Report via HTTP

To serve the merged report via a local HTTP server (useful for testing CORS-restricted features or sharing on local network):

```powershell
# Generate and serve the report (requires Node.js/npx)
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -CoverageReportTitle "My Project Coverage" `
    -Serve

# Specify custom port (default: 8080)
.\_Scripts\Run-SolutionTests.ps1 `
    -GenerateCoverageJson `
    -GenerateCoverageHtml `
    -Serve `
    -ServePort 3000
```

The `-Serve` switch uses `npx serve` to start a local HTTP server serving the MergedReport directory. The browser will automatically open to view the report. Press `Ctrl+C` to stop the server.

**Requirements**: Node.js must be installed (includes npx). Download from https://nodejs.org/

Features of the JSON-driven report:
- **Separation of concerns**: JSON data can be consumed by other tools, HTML is optional
- **Interactive filtering**: Search by class name, filename, or coverage percentage
- **Expandable packages**: Click to view class-level details
- **Beautiful design**: Gradient visuals with responsive layout
- **Pure client-side**: No web server required, works offline (or via HTTP with `-Serve`)
- **Color-coded indicators**: Visual coverage status (excellent/high/medium/low)

Benefits:
- **Portable**: Two files (JSON + HTML), no dependencies or external libraries
- **Fast**: Client-side rendering with instant filtering
- **Customizable**: Modify the HTML template in Resources/ or consume JSON elsewhere
- **CI-friendly**: Perfect for build artifacts and static hosting
- **Flexible**: Generate JSON for machine consumption, HTML for humans

## Tests

Pester tests are in `./_Scripts/test` and can be run with:

```powershell
Install-Module -Name Pester -Force -Scope CurrentUser
Invoke-Pester ./_Scripts/test -Verbose
```

