param(
    [string]$CoverageResultsDir = 'CSharp.Object.Result/test/Inspect/TestResults'
)

$cov = Get-ChildItem -Path $CoverageResultsDir -Recurse -Filter 'coverage.cobertura.xml' -File -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $cov) {
    Write-Host 'No coverage file found under:' $CoverageResultsDir
    exit 0
}

Write-Host 'Using coverage file:' $cov.FullName

[xml]$doc = Get-Content $cov.FullName
$classes = @()
try {
    $classes = $doc.coverage.packages.package.classes.class
} catch {
    Write-Host 'No class nodes found in cobertura xml.'
    exit 0
}

foreach ($c in $classes) {
    $file = $c.filename
    $name = $c.name
    $zeros = @()
    if ($c.lines -and $c.lines.line) {
        foreach ($ln in $c.lines.line) {
            $hits = 0
            if ($ln.hits -ne $null) { [int]$hits = [int]$ln.hits }
            if ($hits -eq 0) { $zeros += $ln.number }
        }
    }
    if ($zeros.Count -gt 0) {
        Write-Host "$file | $name -> Uncovered lines: $($zeros -join ',')"
    } else {
        Write-Host "$file | $name -> No uncovered lines"
    }
}
