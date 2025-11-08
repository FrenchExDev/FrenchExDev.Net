# _Scripts

This folder contains helper scripts and a small PowerShell module to run tests across the repository.

## Entry point

- `Run-SolutionTests.ps1` - Entry script to run all test projects referenced by the solution.

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

Generate a merged coverage report across all test projects (requires ReportGenerator):

```powershell
# generate a merged HTML report under ./test/reports/<timestamp>/MergedReport
.\_Scripts\Run-SolutionTests.ps1 -GenerateMergedReport -ReportTypes Html -MergedReportDir './test/reports/merged'
```

Run tests in parallel with throttling (useful for CI hosts). `-Parallel` enables background job runs and `-MaxParallel` controls concurrency (default 4):

```powershell
# run with up to 6 concurrent test jobs
.\_Scripts\Run-SolutionTests.ps1 -Parallel -MaxParallel 6
```

Notes on merging behavior:
 - `New-MergedCobertura` consolidates class entries by filename and classname and sums per-line hit counts across inputs. This produces a merged cobertura XML with accurate combined hit counts and a computed `line-rate` attribute.
- The module will call ReportGenerator (global tool) to produce human-readable reports. If `reportgenerator` is not available the script will attempt to install `dotnet-reportgenerator-globaltool` globally.

## Tests

Pester tests are in `./_Scripts/test` and can be run with:

```powershell
Install-Module -Name Pester -Force -Scope CurrentUser
Invoke-Pester ./_Scripts/test -Verbose
```

