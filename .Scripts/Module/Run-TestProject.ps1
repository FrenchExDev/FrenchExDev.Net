<#
.SYNOPSIS
Run dotnet test for a project and produce coverage artifacts and an HTML coverage report using ReportGenerator.

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

# Run tests for a specific project and produce HTML coverage under a custom output directory
.\_Scripts\Run-Tests.ps1 -ProjectPath "CSharp.Object.Result/test/FrenchExDev.Net.CSharp.Object.Result.Tests/FrenchExDev.Net.CSharp.Object.Result.Tests.csproj" -OutputDir "C:\tmp\my-output" -CoverageFormats @('opencover','cobertura') -UseMsBuildCoverlet -CoverageThreshold 80
#>
function Invoke-TestProject {
    param(
        [string]$ProjectPath = '',
        [string]$Configuration = 'Release',
        [string]$ResultsDir = 'TestResults',
        [string]$ReportDir = 'CoverageReport',
        [switch]$NoBuild,
        [switch]$NoRestore,
        [switch]$OpenReport,
        [string]$OutputDir = '',
        [string[]]$CoverageFormats = @('cobertura'),
        [string]$CoverageOutput = '',
        [string[]]$ReportTypes = @('Html'),
        [switch]$UseMsBuildCoverlet,
        [string[]]$IncludeFilters = @(),
        [string[]]$ExcludeFilters = @(),
        [int]$CoverageThreshold = -1,
        [bool]$FailOnError = $true,
        [string]$LogDir = '',
        [string]$LogFilePrefix = '',
        [switch]$KeepLogFiles
    )

    function Write-Header($text) {
        Write-Host "" -ForegroundColor DarkCyan
        Write-Host $text -ForegroundColor Cyan
    }

    Write-Header "Run-Tests: starting"

    if (-not [string]::IsNullOrWhiteSpace($OutputDir)) {
        # If OutputDir provided and ResultsDir/ReportDir are default values, place them under OutputDir
        if ($ResultsDir -eq 'TestResults') {
            $ResultsDir = Join-Path -Path $OutputDir -ChildPath 'TestResults'
        }
        if ($ReportDir -eq 'CoverageReport') {
            $ReportDir = Join-Path -Path $OutputDir -ChildPath 'CoverageReport'
        }
        Write-Host "Using OutputDir: $OutputDir -> Results: $ResultsDir, Report: $ReportDir"
    }

    # (standardized output root will be computed after ProjectPath resolution)

    if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
        Write-Host "ProjectPath not provided. Searching for a test project (*.csproj) with 'Test' or 'Tests' in the path..."
        $found = Get-ChildItem -Path . -Recurse -Filter '*.csproj' -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -match 'Test' -or $_.FullName -match 'Tests' } |
        Select-Object -First 1
        if ($null -eq $found) {
            $msg = "No test project found. Provide -ProjectPath parameter pointing to a .csproj file."
            Write-Error $msg
            if ($FailOnError) { throw $msg } else { return @{ Success = $false; ExitCode = 2; Message = $msg } }
        }

        $ProjectPath = $found.FullName
        Write-Host "Discovered project: $ProjectPath"
    }
    else {
        if (-not (Test-Path $ProjectPath)) {
            $msg = "ProjectPath '$ProjectPath' does not exist."
            Write-Error $msg
            if ($FailOnError) { throw $msg } else { return @{ Success = $false; ExitCode = 3; Message = $msg; ProjectPath = $ProjectPath } }
        }
    }

    # Standardize output dir when OutputDir was not provided: place two levels above the project directory -> ./test/reports/{timestamp}
    if ([string]::IsNullOrWhiteSpace($OutputDir)) {
        try {
            $projDir = Split-Path -Path $ProjectPath -Parent
            # climb two parents up from the project directory
            $oneUp = Split-Path -Path $projDir -Parent
            $twoUp = Split-Path -Path $oneUp -Parent
            if ([string]::IsNullOrWhiteSpace($twoUp)) { $twoUp = $oneUp }

            $timestamp = Get-Date -Format 'dd-MM-yyyy-HH-mm-ss'
            $reportRoot = Join-Path -Path $twoUp -ChildPath (Join-Path 'test' (Join-Path 'reports' $timestamp))
            if (-not (Test-Path $reportRoot)) { New-Item -ItemType Directory -Path $reportRoot -Force | Out-Null }

            if ($ResultsDir -eq 'TestResults') { $ResultsDir = Join-Path -Path $reportRoot -ChildPath 'TestResults' }
            if ($ReportDir -eq 'CoverageReport') { $ReportDir = Join-Path -Path $reportRoot -ChildPath 'CoverageReport' }

            Write-Host "Using standardized output root (two levels up): $reportRoot -> Results: $ResultsDir, Report: $ReportDir"
        }
        catch {
            Write-Warning "Could not compute standardized output directory from ProjectPath: $_"
        }
    }

    # Ensure results and report directories exist (create if missing)
    if (-not (Test-Path $ResultsDir)) {
        New-Item -ItemType Directory -Path $ResultsDir | Out-Null
    }
    if (-not (Test-Path $ReportDir)) {
        New-Item -ItemType Directory -Path $ReportDir | Out-Null
    }

    # Ensure ReportGenerator is available
    $rgAvailable = $false
    try {
        & reportgenerator -version 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { $rgAvailable = $true }
    }
    catch {
        $rgAvailable = $false
    }

    if (-not $rgAvailable) {
        Write-Host "ReportGenerator not found. Installing global tool 'dotnet-reportgenerator-globaltool'..."
        & dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.4 2>&1 | Write-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Failed to install reportgenerator global tool. You can install it manually with:`n  dotnet tool install --global dotnet-reportgenerator-globaltool`
Continuing without ReportGenerator will still run tests and collect coverage XML."
        }
        else {
            Write-Host "ReportGenerator installed."
        }
    }

    # Run dotnet test with coverage collection
    Write-Header "Running tests and collecting coverage"

    # If user requested msbuild coverlet, verify test project references coverlet.msbuild and add it if missing
    if ($UseMsBuildCoverlet) {
        try {
            Write-Host "UseMsBuildCoverlet requested - verifying test project has 'coverlet.msbuild' package..."
            $projPathResolved = Resolve-Path -Path $ProjectPath -ErrorAction SilentlyContinue
            if ($projPathResolved) {
                $projContent = Get-Content $projPathResolved.Path -ErrorAction SilentlyContinue -Raw
                if ($null -eq $projContent -or ($projContent -notmatch 'coverlet\.msbuild')) {
                    Write-Host "'coverlet.msbuild' not found in project. Attempting to add package 'coverlet.msbuild'..."
                    # Attempt to add the package to project
                    $addProc = Start-Process -FilePath dotnet -ArgumentList @('add', "`"$ProjectPath`"", 'package', 'coverlet.msbuild', '--version', '3.1.2') -NoNewWindow -PassThru -Wait -RedirectStandardOutput temp_out.txt -RedirectStandardError temp_err.txt
                    if ($addProc.ExitCode -ne 0) {
                        Write-Warning "Failed to add 'coverlet.msbuild' to project. Falling back to XPlat collector. See temp_out.txt and temp_err.txt for details."
                        $UseMsBuildCoverlet = $false
                    }
                    else {
                        Write-Host "'coverlet.msbuild' added to project successfully."
                    }
                    # cleanup temp files if exist
                    Remove-Item -Path temp_out.txt -ErrorAction SilentlyContinue
                    Remove-Item -Path temp_err.txt -ErrorAction SilentlyContinue
                }
                else {
                    Write-Host "'coverlet.msbuild' already referenced in project."
                }
            }
            else {
                Write-Warning "Could not resolve ProjectPath to verify packages. Proceeding assuming project references coverlet.msbuild."
            }
        }
        catch {
            Write-Warning "Error while checking/adding coverlet.msbuild: $_. Falling back to collector."
            $UseMsBuildCoverlet = $false
        }
    }

    if ($UseMsBuildCoverlet) {
        # Build msbuild properties
        $props = @()
        $props += '/p:CollectCoverage=true'
        $formats = ($CoverageFormats -join ',')
        if (-not [string]::IsNullOrWhiteSpace($CoverageOutput)) {
            # Ensure directory exists
            $covOutDir = Resolve-Path -Path (Split-Path -Path $CoverageOutput -Parent) -ErrorAction SilentlyContinue
            if (-not $covOutDir) { New-Item -ItemType Directory -Path (Split-Path -Path $CoverageOutput -Parent) | Out-Null }
            $props += "/p:CoverletOutput=$CoverageOutput"
        }
        else {
            # default under ResultsDir
            $defaultCov = Join-Path -Path $ResultsDir -ChildPath 'coverage'
            if (-not (Test-Path $defaultCov)) { New-Item -ItemType Directory -Path $defaultCov | Out-Null }
            $props += "/p:CoverletOutput=$defaultCov"
        }
        $props += "/p:CoverletOutputFormat=$formats"

        if ($IncludeFilters.Length -gt 0) { $props += "/p:Include=`"$($IncludeFilters -join ';')`"" }
        if ($ExcludeFilters.Length -gt 0) { $props += "/p:Exclude=`"$($ExcludeFilters -join ';')`"" }

        $dotnetTestArgs = @('test', "`"$ProjectPath`"", '--configuration', $Configuration)
        if ($NoBuild) { $dotnetTestArgs += '--no-build' }
        if ($NoRestore) { $dotnetTestArgs += '--no-restore' }
        $dotnetTestArgs += $props
    }
    else {
        # Default: use coverlet.collector via XPlat Code Coverage
        if ($UseMsBuildCoverlet) { Write-Warning "Falling back to collector due to previous errors." }
        $dotnetTestArgs = @('test', "`"$ProjectPath`"", '--configuration', $Configuration, '--logger', 'trx', '--results-directory', $ResultsDir, '--collect:"XPlat Code Coverage"')
        if ($NoBuild) { $dotnetTestArgs += '--no-build' }
        if ($NoRestore) { $dotnetTestArgs += '--no-restore' }
    }

    # Defensive: detect if this is a test project and disable package-on-build/pack to avoid NU5026
    $isTestProject = $false
    try {
        $projText = Get-Content -Path $ProjectPath -Raw -ErrorAction SilentlyContinue
        if ($projText) {
            if ($projText -match 'Microsoft\.NET\.Test\.Sdk' -or $projText -match '<PackageReference[^>]+Include\s*=\s*"(xunit|nunit|MSTest|xunit.runner.visualstudio|MSTest.TestAdapter)') {
                $isTestProject = $true
            }
        }
    } catch {
        # ignore
    }
    # Fallback: filename or path match for 'test' or 'tests'
    if (-not $isTestProject) {
        if ($ProjectPath -imatch 'test' -or $ProjectPath -imatch 'tests' -or $ProjectPath -imatch '\\\\test\\\\') { $isTestProject = $true }
    }

    if ($isTestProject) {
        # If using msbuild coverlet we already put msbuild props in $props; ensure pack is disabled
        if ($UseMsBuildCoverlet) {
            if ($props -notcontains '/p:GeneratePackageOnBuild=false') { $props += '/p:GeneratePackageOnBuild=false' }
            if ($props -notcontains '/p:IsPackable=false') { $props += '/p:IsPackable=false' }
            # rebuild dotnetTestArgs to include these props
            $dotnetTestArgs = @('test', "`"$ProjectPath`"", '--configuration', $Configuration)
            if ($NoBuild) { $dotnetTestArgs += '--no-build' }
            if ($NoRestore) { $dotnetTestArgs += '--no-restore' }
            $dotnetTestArgs += $props
        }
        else {
            # For the collector-based invocation, pass msbuild properties directly
            if ($dotnetTestArgs -notcontains '/p:GeneratePackageOnBuild=false') { $dotnetTestArgs += '/p:GeneratePackageOnBuild=false' }
            if ($dotnetTestArgs -notcontains '/p:IsPackable=false') { $dotnetTestArgs += '/p:IsPackable=false' }
        }
    }

    Write-Host "dotnet $($dotnetTestArgs -join ' ')"
    # create unique temp files for stdout/stderr so failing runs are diagnosable
    if (-not [string]::IsNullOrWhiteSpace($LogDir)) {
        if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir -Force | Out-Null }
        if ([string]::IsNullOrWhiteSpace($LogFilePrefix)) { $prefix = ([guid]::NewGuid().ToString()) } else { $prefix = $LogFilePrefix }
        $safePrefix = ($prefix -replace '[^a-zA-Z0-9_.-]','_')
        $dotnetOut = Join-Path -Path $LogDir -ChildPath ("dotnet_out_{0}.txt" -f $safePrefix)
        $dotnetErr = Join-Path -Path $LogDir -ChildPath ("dotnet_err_{0}.txt" -f $safePrefix)
    }
    else {
        $dotnetOut = Join-Path -Path $env:TEMP -ChildPath (("dotnet_out_{0}.txt" -f ([guid]::NewGuid().ToString())))
        $dotnetErr = Join-Path -Path $env:TEMP -ChildPath (("dotnet_err_{0}.txt" -f ([guid]::NewGuid().ToString())))
    }
    $process = Start-Process -FilePath dotnet -ArgumentList $dotnetTestArgs -NoNewWindow -PassThru -Wait -RedirectStandardOutput $dotnetOut -RedirectStandardError $dotnetErr
    if ($process.ExitCode -ne 0) {
        $tailOut = ''
        $tailErr = ''
        try { if (Test-Path $dotnetOut) { $tailOut = (Get-Content $dotnetOut -ErrorAction SilentlyContinue | Select-Object -Last 200) -join "`n" } } catch {}
        try { if (Test-Path $dotnetErr) { $tailErr = (Get-Content $dotnetErr -ErrorAction SilentlyContinue | Select-Object -Last 200) -join "`n" } } catch {}
        $msg = "dotnet test failed with exit code $($process.ExitCode). Check test output.`nStdout: $dotnetOut`nStderr: $dotnetErr"
        if ($tailOut) { $msg += "`n--- stdout (last 200 lines) ---`n$tailOut" }
        if ($tailErr) { $msg += "`n--- stderr (last 200 lines) ---`n$tailErr" }
        Write-Error $msg
        if ($FailOnError) { throw $msg } else { return @{ Success = $false; ExitCode = $process.ExitCode; Message = $msg; ProjectPath = $ProjectPath; StdOut = $dotnetOut; StdErr = $dotnetErr } }
    } else {
        # successful run: remove temp logs to avoid clutter unless caller asked to keep them
        if (-not $KeepLogFiles.IsPresent -and [string]::IsNullOrWhiteSpace($LogDir)) {
            try { Remove-Item -Path $dotnetOut -ErrorAction SilentlyContinue } catch {}
            try { Remove-Item -Path $dotnetErr -ErrorAction SilentlyContinue } catch {}
        }
    }

    # Find coverage file(s)
    Write-Header "Locating coverage file(s)"
    $foundCoverageFiles = @()
    $searchPatterns = @('*coverage*.xml', '*coverage*.cobertura.xml', '*coverage*.opencover.xml', '*coverage*.xml', 'coverage.*.xml')

    if (-not [string]::IsNullOrWhiteSpace($CoverageOutput)) {
        if (Test-Path $CoverageOutput -PathType Leaf) {
            $foundCoverageFiles += (Resolve-Path $CoverageOutput).Path
        }
        elseif (Test-Path $CoverageOutput -PathType Container) {
            foreach ($pat in $searchPatterns) {
                $foundCoverageFiles += Get-ChildItem -Path $CoverageOutput -Recurse -Filter $pat -File -ErrorAction SilentlyContinue | ForEach-Object { $_.FullName }
            }
        }
    }

    if ($foundCoverageFiles.Count -eq 0) {
        foreach ($pat in $searchPatterns) {
            $foundCoverageFiles += Get-ChildItem -Path $ResultsDir -Recurse -Filter $pat -File -ErrorAction SilentlyContinue | ForEach-Object { $_.FullName }
        }
    }

    if ($foundCoverageFiles.Count -eq 0) {
        # search test project's output/bin directories (common location for coverlet.msbuild outputs)
        $projDir = Split-Path -Path $ProjectPath -Parent
        $binPaths = Get-ChildItem -Path $projDir -Recurse -Directory -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match '\\bin\\' -or $_.FullName -match '\\TestResults\\' }
        foreach ($dir in $binPaths) {
            foreach ($pat in $searchPatterns) {
                $foundCoverageFiles += Get-ChildItem -Path $dir.FullName -Recurse -Filter $pat -File -ErrorAction SilentlyContinue | ForEach-Object { $_.FullName }
            }
        }
    }

    if ($foundCoverageFiles.Count -eq 0) {
        # fallback: search repository root for any coverage xml matching patterns
        $repoRoot = Resolve-Path -Path '.'
    }

    # Build a result object so callers can aggregate and report
    $result = [ordered]@{
        Success = $true
        ExitCode = 0
        ProjectPath = $ProjectPath
        ResultsDir = $ResultsDir
        ReportDir = $ReportDir
        CoverageFiles = $foundCoverageFiles
    }

    # If a threshold is requested and cobertura exists attempt to evaluate it
    if ($CoverageThreshold -ge 0 -and $foundCoverageFiles.Count -gt 0) {
        foreach ($cov in $foundCoverageFiles) {
            if ($cov -match 'cobertura' -or (Get-Content $cov -Raw) -match '<coverage') {
                try {
                    $xml = [xml](Get-Content $cov -Raw)
                    $lineRate = $null
                    if ($xml.coverage) {
                        try {
                            # common cobertura attribute
                            if ($xml.coverage.'@line-rate') { $lineRate = [double]$xml.coverage.'@line-rate' }
                            # some tools use different attribute names
                            elseif ($xml.coverage.'@line') { $lineRate = [double]$xml.coverage.'@line' }
                            # or nested element
                            elseif ($xml.coverage.'line-rate') { $lineRate = [double]$xml.coverage.'line-rate' }
                        } catch {
                            # ignore parse errors
                        }
                    }
                    if ($lineRate -ne $null) {
                        $percent = [math]::Round($lineRate * 100,2)
                        $result.CoveragePercent = $percent
                        if ($percent -lt $CoverageThreshold) {
                            $msg = "Coverage $percent% is below threshold $CoverageThreshold% for project $ProjectPath"
                            Write-Warning $msg
                            if ($FailOnError) { throw $msg } else { $result.Success = $false; $result.ExitCode = 4; $result.Message = $msg }
                        }
                    }
                } catch {
                    # ignore parsing errors
                }
            }
        }
    }

    return $result

}