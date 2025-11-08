<#
.SYNOPSIS
Convert Cobertura coverage XML to JSON and optionally generate an HTML viewer.

.DESCRIPTION
This script parses Cobertura coverage XML files and generates a coverage.json file.
Optionally, it can also copy the HTML viewer template from the module resources to create
an interactive HTML report that loads the JSON data.

.PARAMETER CoverageXmlPath
Path to the Cobertura coverage XML file (e.g., merged-coverage.cobertura.xml)

.PARAMETER OutputDir
Directory where coverage.json (and optionally index.html) will be written. Defaults to the same directory as the input XML.

.PARAMETER Title
Title for the HTML report. Defaults to "Code Coverage Report". Only used when -GenerateHtml is specified.

.PARAMETER GenerateHtml
If specified, copies the HTML viewer template and generates an index.html file alongside the JSON.

.PARAMETER JsonFileName
Name of the JSON file to create. Defaults to "coverage.json".

.PARAMETER HtmlFileName
Name of the HTML file to create when -GenerateHtml is specified. Defaults to "index.html".

.EXAMPLE
# Generate JSON only
.\_Scripts\ConvertTo-CoverageJson.ps1 -CoverageXmlPath ".\test\reports\latest\MergedReport\merged-coverage.cobertura.xml"

.EXAMPLE
# Generate JSON and HTML viewer
.\_Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -OutputDir ".\web-report" `
    -Title "My Project Coverage" `
    -GenerateHtml

.EXAMPLE
# Custom file names
.\_Scripts\ConvertTo-CoverageJson.ps1 `
    -CoverageXmlPath ".\coverage.xml" `
    -JsonFileName "report-data.json" `
    -HtmlFileName "report.html" `
    -GenerateHtml
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$CoverageXmlPath,
    
    [string]$OutputDir = '',
    
    [string]$Title = 'Code Coverage Report',
    
    [switch]$GenerateHtml,
    
    [string]$JsonFileName = 'coverage.json',
    
    [string]$HtmlFileName = 'index.html'
)

function ConvertFrom-CoberturaXml {
    <#
    .SYNOPSIS
    Parse a Cobertura XML coverage file and return structured data.
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$XmlPath
    )
    
    if (-not (Test-Path $XmlPath)) {
        throw "Coverage XML file not found: $XmlPath"
    }
    
    Write-Host "Parsing coverage XML: $XmlPath" -ForegroundColor Cyan
    [xml]$doc = Get-Content $XmlPath -Raw
    
    # Extract root-level coverage statistics
    $lineRate = 0
    $branchRate = 0
    try {
        $lineRate = [double]$doc.coverage.'line-rate'
    } catch {
        try { $lineRate = [double]$doc.coverage.Attributes['line-rate'].Value } catch { }
    }
    try {
        $branchRate = [double]$doc.coverage.'branch-rate'
    } catch {
        try { $branchRate = [double]$doc.coverage.Attributes['branch-rate'].Value } catch { }
    }
    
    # Parse packages and classes
    $packages = @()
    $allClasses = @()
    
    try {
        $packageNodes = @()
        if ($doc.coverage.packages.package) {
            $packageNodes = $doc.coverage.packages.package
        }
        
        if ($packageNodes -is [System.Xml.XmlElement]) {
            $packageNodes = , $packageNodes
        }
        
        foreach ($pkg in $packageNodes) {
            if (-not $pkg) { continue }
            
            $pkgName = 'default'
            try { $pkgName = $pkg.Attributes['name'].Value } catch {
                try { $pkgName = $pkg.name } catch { }
            }
            
            $pkgLineRate = 0
            try { $pkgLineRate = [double]$pkg.Attributes['line-rate'].Value } catch {
                try { $pkgLineRate = [double]$pkg.'line-rate' } catch { }
            }
            
            $classes = @()
            $classNodes = @()
            try {
                if ($pkg.classes.class) {
                    $classNodes = $pkg.classes.class
                }
            } catch { }
            
            if ($classNodes -is [System.Xml.XmlElement]) {
                $classNodes = , $classNodes
            }
            
            foreach ($cls in $classNodes) {
                if (-not $cls) { continue }
                
                $className = 'unknown'
                $fileName = 'unknown'
                
                try { $className = $cls.Attributes['name'].Value } catch {
                    try { $className = $cls.name } catch { }
                }
                try { $fileName = $cls.Attributes['filename'].Value } catch {
                    try { $fileName = $cls.filename } catch { }
                }
                
                # Parse lines
                $lines = @()
                $lineNodes = @()
                try {
                    if ($cls.lines.line) {
                        $lineNodes = $cls.lines.line
                    }
                } catch { }
                
                if ($lineNodes -is [System.Xml.XmlElement]) {
                    $lineNodes = , $lineNodes
                }
                
                $totalLines = 0
                $coveredLines = 0
                
                foreach ($ln in $lineNodes) {
                    if (-not $ln) { continue }
                    
                    $lineNum = 0
                    $hits = 0
                    
                    try { $lineNum = [int]$ln.Attributes['number'].Value } catch {
                        try { $lineNum = [int]$ln.number } catch { continue }
                    }
                    try { $hits = [int]$ln.Attributes['hits'].Value } catch {
                        try { $hits = [int]$ln.hits } catch { }
                    }
                    
                    $totalLines++
                    if ($hits -gt 0) { $coveredLines++ }
                    
                    $lines += @{
                        number = $lineNum
                        hits   = $hits
                        covered = $hits -gt 0
                    }
                }
                
                $classLineRate = 0
                if ($totalLines -gt 0) {
                    $classLineRate = [math]::Round(($coveredLines / $totalLines), 4)
                }
                
                $classObj = @{
                    name          = $className
                    filename      = $fileName
                    lineRate      = $classLineRate
                    totalLines    = $totalLines
                    coveredLines  = $coveredLines
                    lines         = $lines
                }
                
                $classes += $classObj
                $allClasses += $classObj
            }
            
            $packages += @{
                name     = $pkgName
                lineRate = $pkgLineRate
                classes  = $classes
            }
        }
    } catch {
        Write-Warning "Error parsing packages: $($_.Exception.Message)"
    }
    
    # Compute overall stats
    $totalLines = ($allClasses | ForEach-Object { $_.totalLines } | Measure-Object -Sum).Sum
    $coveredLines = ($allClasses | ForEach-Object { $_.coveredLines } | Measure-Object -Sum).Sum
    $coveragePercent = 0
    if ($totalLines -gt 0) {
        $coveragePercent = [math]::Round((($coveredLines / $totalLines) * 100), 2)
    }
    
    return @{
        lineRate        = $lineRate
        branchRate      = $branchRate
        totalLines      = $totalLines
        coveredLines    = $coveredLines
        coveragePercent = $coveragePercent
        packages        = $packages
        timestamp       = (Get-Date).ToString('o')
    }
}

function Copy-CoverageHtmlTemplate {
    <#
    .SYNOPSIS
    Copy the HTML viewer template and customize it for the JSON data.
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$OutputPath,
        
        [Parameter(Mandatory = $true)]
        [string]$Title,
        
        [Parameter(Mandatory = $true)]
        [string]$JsonFileName
    )
    
    # Find the template in module resources
    # PSScriptRoot is _Scripts, so template is at _Scripts/Module/Resources/coverage-viewer.html
    $templatePath = Join-Path -Path $PSScriptRoot -ChildPath 'Module\Resources\coverage-viewer.html'
    
    if (-not (Test-Path $templatePath)) {
        throw "HTML template not found at: $templatePath"
    }
    
    # Read template and replace placeholders
    $htmlContent = Get-Content -Path $templatePath -Raw
    $htmlContent = $htmlContent -replace '__TITLE__', $Title
    $htmlContent = $htmlContent -replace '__JSON_FILE__', $JsonFileName
    
    # Write customized HTML
    $htmlContent | Set-Content -Path $OutputPath -Encoding UTF8
}

# Main script execution
try {
    Write-Host "Converting coverage report to JSON..." -ForegroundColor Cyan
    
    # Resolve output directory
    if ([string]::IsNullOrWhiteSpace($OutputDir)) {
        $OutputDir = Split-Path -Path $CoverageXmlPath -Parent
    }
    
    if (-not (Test-Path $OutputDir)) {
        New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    }
    
    # Parse coverage XML
    $coverageData = ConvertFrom-CoberturaXml -XmlPath $CoverageXmlPath
    
    # Write JSON file
    $jsonPath = Join-Path -Path $OutputDir -ChildPath $JsonFileName
    $coverageData | ConvertTo-Json -Depth 10 | Set-Content -Path $jsonPath -Encoding UTF8
    Write-Host "✓ Generated: $jsonPath" -ForegroundColor Green
    
    # Optionally generate HTML viewer
    if ($GenerateHtml) {
        $htmlPath = Join-Path -Path $OutputDir -ChildPath $HtmlFileName
        Copy-CoverageHtmlTemplate -OutputPath $htmlPath -Title $Title -JsonFileName $JsonFileName
        Write-Host "✓ Generated: $htmlPath" -ForegroundColor Green
        
        Write-Host "`nOpen the report with:" -ForegroundColor Cyan
        Write-Host "  Start-Process '$htmlPath'" -ForegroundColor White
    }
    
    # Display summary
    Write-Host "`nCoverage Summary:" -ForegroundColor Cyan
    $coverageColor = if ($coverageData.coveragePercent -ge 80) { 'Green' } elseif ($coverageData.coveragePercent -ge 50) { 'Yellow' } else { 'Red' }
    Write-Host "  Coverage: $($coverageData.coveragePercent)%" -ForegroundColor $coverageColor
    Write-Host "  Total Lines: $($coverageData.totalLines)"
    Write-Host "  Covered Lines: $($coverageData.coveredLines)"
    Write-Host "  Packages: $($coverageData.packages.Count)"
    
    $totalClasses = ($coverageData.packages | ForEach-Object { $_.classes.Count } | Measure-Object -Sum).Sum
    Write-Host "  Classes: $totalClasses"
    
    # Return the JSON path for pipeline use
    return $jsonPath
}
catch {
    Write-Error "Failed to convert coverage report: $($_.Exception.Message)"
    Write-Error $_.ScriptStackTrace
    exit 1
}
