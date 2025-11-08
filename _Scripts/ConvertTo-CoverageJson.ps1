<# Convert Cobertura coverage XML to JSON - simplified, robust implementation #>
param(
    [Parameter(Mandatory = $true)] [string]$CoverageXmlPath,
    [string]$OutputDir = '',
    [string]$Title = 'Code Coverage Report',
    [switch]$GenerateHtml,
    [string]$JsonFileName = 'coverage.json',
    [string]$HtmlFileName = 'index.html'
)

# Minimal, robust conversion from Cobertura XML to a simple JSON structure
try {
    if (-not (Test-Path $CoverageXmlPath)) {
        Write-Error "Coverage XML file not found: $CoverageXmlPath"
        exit 1
    }

    [xml]$doc = Get-Content -Raw -Path $CoverageXmlPath

    $projects = @()

    # handle packages -> classes -> lines
    if ($null -ne $doc.coverage -and $null -ne $doc.coverage.packages) {
        $pkgNodes = $doc.coverage.packages.package
        if ($pkgNodes -is [System.Xml.XmlElement]) { $pkgNodes = ,$pkgNodes }
        foreach ($pkg in $pkgNodes) {
            if ($null -eq $pkg) { continue }
            $classNodes = $pkg.classes.class
            if ($classNodes -is [System.Xml.XmlElement]) { $classNodes = ,$classNodes }
            foreach ($cls in $classNodes) {
                if ($null -eq $cls) { continue }
                $className = 'unknown'
                $fileName = 'unknown'
                if ($cls.Attributes['name']) { $className = $cls.Attributes['name'].Value }
                if ($cls.Attributes['filename']) { $fileName = $cls.Attributes['filename'].Value }

                $lineNodes = @()
                if ($null -ne $cls.lines -and $null -ne $cls.lines.line) { $lineNodes = $cls.lines.line }
                if ($lineNodes -is [System.Xml.XmlElement]) { $lineNodes = ,$lineNodes }

                $lines = @()
                $totalLines = 0
                $coveredLines = 0
                foreach ($ln in $lineNodes) {
                    if ($null -eq $ln) { continue }
                    $num = 0; $hits = 0
                    if ($ln.Attributes['number']) { $num = [int]$ln.Attributes['number'].Value } elseif ($ln.number) { $num = [int]$ln.number }
                    if ($ln.Attributes['hits']) { $hits = [int]$ln.Attributes['hits'].Value } elseif ($ln.hits) { $hits = [int]$ln.hits }
                    $totalLines++
                    if ($hits -gt 0) { $coveredLines++ }
                    $lines += @{ number = $num; hits = $hits; covered = ($hits -gt 0) }
                }

                $classLineRate = 0
                if ($totalLines -gt 0) { $classLineRate = [math]::Round(($coveredLines / $totalLines), 4) }

                $projects += @{ name = $className; filename = $fileName; totalLines = $totalLines; coveredLines = $coveredLines; lineRate = $classLineRate; lines = $lines }
            }
        }
    }

    $totalLines = 0; $coveredLines = 0
    if ($projects.Count -gt 0) {
        $totalLines = ($projects | ForEach-Object { $_.totalLines } | Measure-Object -Sum).Sum
        $coveredLines = ($projects | ForEach-Object { $_.coveredLines } | Measure-Object -Sum).Sum
    }

    $coveragePercent = 0
    if ($totalLines -gt 0) { $coveragePercent = [math]::Round((($coveredLines / $totalLines) * 100), 2) }

    $out = @{ timestamp = (Get-Date).ToString('o'); totalLines = $totalLines; coveredLines = $coveredLines; coveragePercent = $coveragePercent; projects = $projects }

    if ([string]::IsNullOrWhiteSpace($OutputDir)) { $OutputDir = Split-Path -Path $CoverageXmlPath -Parent }
    if (-not (Test-Path $OutputDir)) { New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null }

    $jsonPath = Join-Path -Path $OutputDir -ChildPath $JsonFileName
    $out | ConvertTo-Json -Depth 10 | Set-Content -Path $jsonPath -Encoding UTF8
    Write-Host "Generated JSON: $jsonPath"

    if ($GenerateHtml) {
        $template = Join-Path -Path $PSScriptRoot -ChildPath 'Module\Resources\coverage-viewer.html'
        if (Test-Path $template) {
            $html = Get-Content -Path $template -Raw
            $html = $html -replace '__TITLE__', $Title
            $html = $html -replace '__JSON_FILE__', $JsonFileName
            $htmlPath = Join-Path -Path $OutputDir -ChildPath $HtmlFileName
            $html | Set-Content -Path $htmlPath -Encoding UTF8
            Write-Host "Generated HTML: $htmlPath"
        }
    }

    Write-Host "Coverage: $coveragePercent% ($coveredLines / $totalLines)"
    return $jsonPath
} catch {
    Write-Error "ConvertTo-CoverageJson failed: $($_.Exception.Message)"
    exit 1
}
