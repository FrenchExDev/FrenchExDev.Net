# Analyze-LatestCoverage.ps1
param()

# Find newest coverage JSON under test/report
$repoRoot = Resolve-Path -Path '.'
$searchRoot = Join-Path $repoRoot 'test\report'
if (-not (Test-Path $searchRoot)) { Write-Output "No test/report directory found at: $searchRoot"; exit 0 }

$files = Get-ChildItem -Path $searchRoot -Recurse -Filter '*.json' -File -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
if ($null -eq $files -or $files.Count -eq 0) { Write-Output "No coverage JSON found under: $searchRoot"; exit 0 }

$latest = $files | Select-Object -First 1
Write-Output "Using coverage JSON: $($latest.FullName)"

# Read JSON
try {
    $j = Get-Content -Raw $latest.FullName | ConvertFrom-Json
} catch {
    Write-Error "Failed to read/parse JSON: $_"
    exit 1
}

# Coverage numbers may be in different fields depending on generator
$linesTotal = $null
$linesCovered = $null
$percent = $null
if ($j.PSObject.Properties.Name -contains 'totalLines') { $linesTotal = $j.totalLines }
if ($j.PSObject.Properties.Name -contains 'coveredLines') { $linesCovered = $j.coveredLines }
if ($j.PSObject.Properties.Name -contains 'coveragePercent') { $percent = $j.coveragePercent }
if ($null -eq $percent -and $j.PSObject.Properties.Name -contains 'lineRate') { $percent = [math]::Round(($j.lineRate * 100), 2) }
if ($null -eq $percent -and $linesTotal -and $linesCovered) { $percent = [math]::Round((($linesCovered / $linesTotal) * 100), 2) }

Write-Output "Coverage: $percent% ($linesCovered/$linesTotal)"
Write-Output "Packages: $($j.packages.Count)"
Write-Output "Timestamp: $($j.timestamp)"
Write-Output ""
Write-Output "Uncovered lines (file, line):"

# Call Parse-LowestCoverage.ps1 (shim will forward if necessary)
$parse = Join-Path $PSScriptRoot 'Parse-LowestCoverage.ps1'
if (-not (Test-Path $parse)) { $parse = Join-Path $PSScriptRoot 'parse_coverage.ps1' }
if (-not (Test-Path $parse)) { Write-Error "No parser script found (_Scripts\\Parse-LowestCoverage.ps1 or _Scripts\\parse_coverage.ps1)"; exit 1 }

& $parse -JsonPath $latest.FullName
