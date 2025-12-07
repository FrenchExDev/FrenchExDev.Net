<#
.SYNOPSIS
Run dotnet test for a project and produce coverage artifacts and an HTML coverage report using ReportGenerator.

.DESCRIPTION
This script runs .NET tests with code coverage collection and generates HTML reports.
It supports both coverlet.collector (XPlat Code Coverage) and coverlet.msbuild approaches.
Coverage results are validated against optional thresholds and reports are generated using ReportGenerator.

.PARAMETER ProjectPath
Path to the test project (.csproj). If omitted the script will try to find a *Tests*.csproj in the current directory tree.

.PARAMETER Configuration
Build configuration (Default: Release)

.PARAMETER ResultsDir
Directory where dotnet test writes results (Default: ./TestResults)

.PARAMETER ReportDir
Directory where the HTML report will be generated (Default: ./CoverageReport)

.PARAMETER NoBuild
If specified, pass --no-build to dotnet test (default: false)

.PARAMETER NoRestore
If specified, pass --no-restore to dotnet test (default: false)

.PARAMETER OpenReport
If specified, open the generated index.html in the OS default browser/application.

.PARAMETER OutputDir
Optional root output directory. When provided and ResultsDir/ReportDir are left as defaults, they will be created under this directory
so the caller can control where artifacts are placed and where the script looks for coverage files.

.PARAMETER CoverageFormats
Array of coverage formats to produce (e.g. 'cobertura','opencover','lcov'). Default: 'cobertura'. When using msbuild coverlet these will be respected.

.PARAMETER CoverageOutput
Path (file or directory) where coverage output should be written. If empty the script will use ResultsDir.

.PARAMETER ReportTypes
Report types passed to ReportGenerator (default: Html). Can be comma separated values.

.PARAMETER UseMsBuildCoverlet
If specified, use coverlet.msbuild by passing /p:CollectCoverage=true and related properties to dotnet test. Otherwise use coverlet.collector (--collect:"XPlat Code Coverage").

.PARAMETER IncludeFilters
Array of include filters for coverlet (eg '[MyAssembly*]').

.PARAMETER ExcludeFilters
Array of exclude filters for coverlet (eg '[xunit.*]*').

.PARAMETER CoverageThreshold
If >=0, the script will attempt to read the cobertura coverage percentage and exit non-zero if coverage < threshold.

.EXAMPLE
# Run tests for a specific project with default settings
.\.Scripts\Run-TestProject.ps1 -ProjectPath "MyProject.Tests/MyProject.Tests.csproj"

.EXAMPLE
# Run tests with custom output directory and coverage threshold
.\.Scripts\Run-TestProject.ps1 -ProjectPath "MyProject.Tests/MyProject.Tests.csproj" -OutputDir "C:\tmp\coverage" -CoverageThreshold 80

.EXAMPLE
# Run tests using MSBuild coverlet with multiple coverage formats
.\.Scripts\Run-TestProject.ps1 -ProjectPath "MyProject.Tests/MyProject.Tests.csproj" -UseMsBuildCoverlet -CoverageFormats @('cobertura','opencover')

.EXAMPLE
# Auto-discover test project and open report in browser
.\.Scripts\Run-TestProject.ps1 -OpenReport

.NOTES
Requirements:
- .NET SDK installed
- coverlet.collector or coverlet.msbuild package in test project
- ReportGenerator global tool (will be installed automatically if missing)
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$ProjectPath = '',

    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [Parameter()]
    [string]$ResultsDir = 'TestResults',

    [Parameter()]
    [string]$ReportDir = 'CoverageReport',

    [Parameter()]
    [switch]$NoBuild,

    [Parameter()]
    [switch]$NoRestore,

    [Parameter()]
    [switch]$OpenReport,

    [Parameter()]
    [string]$OutputDir = '',

    [Parameter()]
    [string[]]$CoverageFormats = @('cobertura'),

    [Parameter()]
    [string]$CoverageOutput = '',

    [Parameter()]
    [string[]]$ReportTypes = @('Html'),

    [Parameter()]
    [switch]$UseMsBuildCoverlet,

    [Parameter()]
    [string[]]$IncludeFilters = @(),

    [Parameter()]
    [string[]]$ExcludeFilters = @(),

    [Parameter()]
    [ValidateRange(-1, 100)]
    [int]$CoverageThreshold = -1
)

#region Helper Functions

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor DarkCyan
    Write-Host " $Text" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor DarkCyan
}

function Write-Step {
    param([string]$Text)
    Write-Host ""
    Write-Host ">> $Text" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Text)
    Write-Host "[SUCCESS] $Text" -ForegroundColor Green
}

function Write-Info {
    param([string]$Text)
    Write-Host "[INFO] $Text" -ForegroundColor Gray
}

function Test-ReportGeneratorAvailable {
    # Try repo-local tool first
    try {
        $null = & dotnet tool run reportgenerator --version 2>&1
        if ($LASTEXITCODE -eq 0) {
            return @{ Available = $true; UseDotnetToolRun = $true }
        }
    } catch { }

    # Try global tool
    if (Get-Command reportgenerator -ErrorAction SilentlyContinue) {
        return @{ Available = $true; UseDotnetToolRun = $false }
    }

    return @{ Available = $false; UseDotnetToolRun = $false }
}

function Install-ReportGenerator {
    # Check for tool manifest first
    $toolManifest = Join-Path -Path (Get-Location) -ChildPath '.config\dotnet-tools.json'
    if (Test-Path $toolManifest) {
        Write-Info "Found tool manifest. Running 'dotnet tool restore'..."
        & dotnet tool restore
        $rgCheck = Test-ReportGeneratorAvailable
        if ($rgCheck.Available) {
            Write-Success "ReportGenerator restored from tool manifest."
            return $rgCheck
        }
    }

    Write-Warning @"
ReportGenerator not found. To enable coverage report generation:
  Option 1 (Recommended): dotnet tool restore (if this repo has a tool manifest)
  Option 2: dotnet tool install --global dotnet-reportgenerator-globaltool

Continuing without ReportGenerator - tests will run and coverage XML will be collected.
"@
    return @{ Available = $false; UseDotnetToolRun = $false }
}

function Get-CoveragePercentage {
    param([string]$CoberturaPath)

    if (-not (Test-Path $CoberturaPath)) { return $null }

    try {
        $xml = [xml](Get-Content $CoberturaPath -Raw)
        if ($xml.coverage) {
            $lineRate = $xml.coverage.GetAttribute('line-rate')
            if ($lineRate) {
                return [math]::Round([double]$lineRate * 100, 2)
            }
        }
    } catch {
        Write-Warning "Failed to parse coverage file: $_"
    }
    return $null
}

function Find-CoverageFiles {
    param(
        [string]$SearchRoot,
        [string[]]$AdditionalPaths = @()
    )

    $patterns = @('coverage.cobertura.xml', '*coverage*.xml', 'coverage.*.xml')
    $found = @()

    $searchPaths = @($SearchRoot) + $AdditionalPaths | Where-Object { $_ -and (Test-Path $_) }

    foreach ($path in $searchPaths) {
        foreach ($pattern in $patterns) {
            $files = Get-ChildItem -Path $path -Recurse -Filter $pattern -File -ErrorAction SilentlyContinue
            foreach ($file in $files) {
                if ($found -notcontains $file.FullName) {
                    $found += $file.FullName
                }
            }
        }
    }

    return $found
}

#endregion

#region Main Script

Write-Header "Run-TestProject: Starting Test Execution"

# Resolve project path
if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    Write-Step "Searching for test project..."
    $found = Get-ChildItem -Path . -Recurse -Filter '*.csproj' -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -match 'Test' -or $_.FullName -match 'Tests' } |
        Select-Object -First 1

    if ($null -eq $found) {
        Write-Error "No test project found. Provide -ProjectPath parameter pointing to a .csproj file."
        exit 2
    }

    $ProjectPath = $found.FullName
    Write-Info "Discovered project: $ProjectPath"
}
else {
    if (-not (Test-Path $ProjectPath)) {
        Write-Error "ProjectPath '$ProjectPath' does not exist."
        exit 3
    }
    Write-Info "Using project: $ProjectPath"
}

# Resolve output directories
if (-not [string]::IsNullOrWhiteSpace($OutputDir)) {
    if ($ResultsDir -eq 'TestResults') {
        $ResultsDir = Join-Path -Path $OutputDir -ChildPath 'TestResults'
    }
    if ($ReportDir -eq 'CoverageReport') {
        $ReportDir = Join-Path -Path $OutputDir -ChildPath 'CoverageReport'
    }
    Write-Info "Using OutputDir: $OutputDir"
}
else {
    # Compute standardized output directory (two levels up from project -> test/reports/timestamp)
    try {
        $projDir = Split-Path -Path $ProjectPath -Parent
        $oneUp = Split-Path -Path $projDir -Parent
        $twoUp = Split-Path -Path $oneUp -Parent
        if ([string]::IsNullOrWhiteSpace($twoUp)) { $twoUp = $oneUp }

        $timestamp = Get-Date -Format 'yyyy-MM-dd-HH-mm-ss'
        $reportRoot = Join-Path -Path $twoUp -ChildPath "test/reports/$timestamp"

        if ($ResultsDir -eq 'TestResults') {
            $ResultsDir = Join-Path -Path $reportRoot -ChildPath 'TestResults'
        }
        if ($ReportDir -eq 'CoverageReport') {
            $ReportDir = Join-Path -Path $reportRoot -ChildPath 'CoverageReport'
        }
        Write-Info "Using standardized output: $reportRoot"
    }
    catch {
        Write-Warning "Could not compute standardized output directory: $_"
    }
}

# Create directories
if (-not (Test-Path $ResultsDir)) {
    New-Item -ItemType Directory -Path $ResultsDir -Force | Out-Null
}
if (-not (Test-Path $ReportDir)) {
    New-Item -ItemType Directory -Path $ReportDir -Force | Out-Null
}

Write-Info "Results directory: $ResultsDir"
Write-Info "Report directory: $ReportDir"

# Check ReportGenerator availability
Write-Step "Checking ReportGenerator availability..."
$rgStatus = Test-ReportGeneratorAvailable
if (-not $rgStatus.Available) {
    $rgStatus = Install-ReportGenerator
}
else {
    Write-Success "ReportGenerator is available."
}

# Handle MSBuild Coverlet package verification
if ($UseMsBuildCoverlet) {
    Write-Step "Verifying coverlet.msbuild package..."
    try {
        $projContent = Get-Content $ProjectPath -Raw -ErrorAction SilentlyContinue
        if ($projContent -notmatch 'coverlet\.msbuild') {
            Write-Info "Adding coverlet.msbuild package to project..."
            & dotnet add $ProjectPath package coverlet.msbuild --version 6.0.0
            if ($LASTEXITCODE -ne 0) {
                Write-Warning "Failed to add coverlet.msbuild. Falling back to XPlat collector."
                $UseMsBuildCoverlet = $false
            }
            else {
                Write-Success "coverlet.msbuild added successfully."
            }
        }
        else {
            Write-Info "coverlet.msbuild already referenced."
        }
    }
    catch {
        Write-Warning "Error checking coverlet.msbuild: $_. Falling back to XPlat collector."
        $UseMsBuildCoverlet = $false
    }
}

# Build dotnet test arguments
Write-Step "Running tests with coverage..."

if ($UseMsBuildCoverlet) {
    # MSBuild Coverlet approach
    $coverageDir = if ([string]::IsNullOrWhiteSpace($CoverageOutput)) {
        Join-Path -Path $ResultsDir -ChildPath 'coverage/'
    } else { $CoverageOutput }

    if (-not (Test-Path (Split-Path $coverageDir -Parent))) {
        New-Item -ItemType Directory -Path (Split-Path $coverageDir -Parent) -Force | Out-Null
    }

    $formats = $CoverageFormats -join ','

    $dotnetArgs = @(
        'test'
        $ProjectPath
        '--configuration', $Configuration
        '--logger', 'trx'
        '--results-directory', $ResultsDir
        "/p:CollectCoverage=true"
        "/p:CoverletOutput=$coverageDir"
        "/p:CoverletOutputFormat=$formats"
        '/p:GeneratePackageOnBuild=false'
        '/p:IsPackable=false'
    )

    if ($IncludeFilters.Count -gt 0) {
        $dotnetArgs += "/p:Include=`"$($IncludeFilters -join ';')`""
    }
    if ($ExcludeFilters.Count -gt 0) {
        $dotnetArgs += "/p:Exclude=`"$($ExcludeFilters -join ';')`""
    }
}
else {
    # XPlat Code Coverage (default)
    $dotnetArgs = @(
        'test'
        $ProjectPath
        '--configuration', $Configuration
        '--logger', 'trx'
        '--results-directory', $ResultsDir
        '--collect', 'XPlat Code Coverage'
        '/p:GeneratePackageOnBuild=false'
        '/p:IsPackable=false'
    )
}

if ($NoBuild) { $dotnetArgs += '--no-build' }
if ($NoRestore) { $dotnetArgs += '--no-restore' }

Write-Info "Executing: dotnet $($dotnetArgs -join ' ')"

# Execute dotnet test
& dotnet @dotnetArgs
$testExitCode = $LASTEXITCODE

if ($testExitCode -ne 0) {
    Write-Error "dotnet test failed with exit code $testExitCode"
    exit $testExitCode
}

Write-Success "Tests completed successfully."

# Find coverage files
Write-Step "Locating coverage files..."

$projDir = Split-Path -Path $ProjectPath -Parent
$coverageFiles = Find-CoverageFiles -SearchRoot $ResultsDir -AdditionalPaths @(
    $projDir
    (Join-Path $projDir 'bin')
    (Join-Path $projDir 'TestResults')
)

if ($coverageFiles.Count -eq 0) {
    Write-Warning "No coverage files found. ReportGenerator will be skipped."
}
else {
    Write-Info "Found $($coverageFiles.Count) coverage file(s):"
    foreach ($f in $coverageFiles) {
        Write-Info "  - $f"
    }

    # Generate HTML report
    if ($rgStatus.Available -and $coverageFiles.Count -gt 0) {
        Write-Step "Generating coverage report..."

        $reportInput = $coverageFiles -join ';'
        $reportTypesStr = $ReportTypes -join ';'

        $rgArgs = @(
            "-reports:$reportInput"
            "-targetdir:$ReportDir"
            "-reporttypes:$reportTypesStr"
        )

        if ($rgStatus.UseDotnetToolRun) {
            Write-Info "Executing: dotnet tool run reportgenerator $($rgArgs -join ' ')"
            & dotnet tool run reportgenerator @rgArgs
        }
        else {
            Write-Info "Executing: reportgenerator $($rgArgs -join ' ')"
            & reportgenerator @rgArgs
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Success "Coverage report generated at: $ReportDir"

            $indexHtml = Join-Path $ReportDir 'index.html'
            if ($OpenReport -and (Test-Path $indexHtml)) {
                Write-Info "Opening report in browser..."
                Start-Process $indexHtml
            }
        }
        else {
            Write-Warning "ReportGenerator failed with exit code $LASTEXITCODE"
        }
    }

    # Check coverage threshold
    if ($CoverageThreshold -ge 0) {
        Write-Step "Checking coverage threshold..."

        $coberturaFile = $coverageFiles | Where-Object { $_ -match 'cobertura' } | Select-Object -First 1
        if (-not $coberturaFile) {
            $coberturaFile = $coverageFiles | Select-Object -First 1
        }

        if ($coberturaFile) {
            $coveragePercent = Get-CoveragePercentage -CoberturaPath $coberturaFile
            if ($null -ne $coveragePercent) {
                Write-Info "Coverage: $coveragePercent%"

                if ($coveragePercent -lt $CoverageThreshold) {
                    Write-Error "Coverage $coveragePercent% is below threshold $CoverageThreshold%"
                    exit 4
                }
                else {
                    Write-Success "Coverage $coveragePercent% meets threshold $CoverageThreshold%"
                }
            }
            else {
                Write-Warning "Could not determine coverage percentage."
            }
        }
    }
}

# Summary
Write-Header "Summary"
Write-Info "Project:    $ProjectPath"
Write-Info "Results:    $ResultsDir"
Write-Info "Report:     $ReportDir"
Write-Info "Coverage:   $($coverageFiles.Count) file(s) found"
if ($coverageFiles.Count -gt 0 -and $CoverageThreshold -ge 0) {
    Write-Info "Threshold:  $CoverageThreshold% (PASSED)"
}
Write-Success "Test execution completed."

exit 0

#endregion
