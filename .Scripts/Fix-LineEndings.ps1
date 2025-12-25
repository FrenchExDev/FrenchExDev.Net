<#
.SYNOPSIS
    Fixes line endings in C# files to use consistent LF or CRLF.
.DESCRIPTION
    This script scans all .cs files in a solution directory and normalizes
    line endings to either LF (Unix) or CRLF (Windows) format.
.PARAMETER Path
    The root path to scan for .cs files. Defaults to the solution root.
.PARAMETER LineEnding
    The line ending format to use: 'LF' or 'CRLF'. Defaults to 'LF'.
.PARAMETER Exclude
    Patterns to exclude from processing (e.g., obj, bin directories).
.PARAMETER DryRun
    If specified, shows what would be changed without making modifications.
.PARAMETER Verbose
    If specified, shows detailed output for each file processed.
.EXAMPLE
    .\Fix-LineEndings.ps1
    Converts all .cs files to LF line endings.
.EXAMPLE
    .\Fix-LineEndings.ps1 -LineEnding CRLF
    Converts all .cs files to CRLF line endings.
.EXAMPLE
    .\Fix-LineEndings.ps1 -DryRun
    Shows which files would be modified without making changes.
#>

param(
    [Parameter(Mandatory = $false)]
    [string]$Path = "$PSScriptRoot\..",
    
    [Parameter(Mandatory = $false)]
    [ValidateSet('LF', 'CRLF')]
    [string]$LineEnding = 'LF',
    
    [Parameter(Mandatory = $false)]
    [string[]]$Exclude = @('*\obj\*', '*\bin\*', '*\.git\*', '*\node_modules\*'),
    
    [switch]$DryRun,
    
    [switch]$VerboseOutput
)

$ErrorActionPreference = "Stop"

# Resolve path
$Path = Resolve-Path $Path -ErrorAction Stop

Write-Host "Line Ending Fixer" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan
Write-Host "Path: $Path" -ForegroundColor Gray
Write-Host "Target: $LineEnding" -ForegroundColor Gray
if ($DryRun) { Write-Host "[DRY RUN MODE]" -ForegroundColor Yellow }
Write-Host ""

# Statistics
$stats = @{
    Scanned = 0
    Modified = 0
    Skipped = 0
    Errors = 0
}

# Get target line ending bytes
$targetEnding = if ($LineEnding -eq 'LF') { "`n" } else { "`r`n" }

# Find all .cs files
$files = Get-ChildItem -Path $Path -Filter "*.cs" -Recurse -File | Where-Object {
    $filePath = $_.FullName
    $excluded = $false
    foreach ($pattern in $Exclude) {
        if ($filePath -like $pattern) {
            $excluded = $true
            break
        }
    }
    -not $excluded
}

Write-Host "Found $($files.Count) .cs files to process" -ForegroundColor Gray
Write-Host ""

foreach ($file in $files) {
    $stats.Scanned++
    $relativePath = $file.FullName.Substring($Path.Path.Length + 1)
    
    try {
        # Read file as bytes to detect line endings
        $content = [System.IO.File]::ReadAllText($file.FullName)
        
        # Check current line endings
        $hasCRLF = $content.Contains("`r`n")
        $hasLF = $content.Contains("`n") -and -not $hasCRLF
        $hasMixed = $content.Contains("`r`n") -and ($content -replace "`r`n", "" ).Contains("`n")
        
        # Determine current state
        $currentEnding = if ($hasMixed) { "Mixed" } elseif ($hasCRLF) { "CRLF" } elseif ($hasLF) { "LF" } else { "None" }
        
        # Check if conversion needed
        $needsConversion = ($LineEnding -eq 'LF' -and ($hasCRLF -or $hasMixed)) -or
                          ($LineEnding -eq 'CRLF' -and ($hasLF -or $hasMixed))
        
        if ($needsConversion) {
            if ($VerboseOutput -or $DryRun) {
                Write-Host "  $relativePath" -ForegroundColor White -NoNewline
                Write-Host " ($currentEnding -> $LineEnding)" -ForegroundColor Yellow
            }
            
            if (-not $DryRun) {
                # Normalize to LF first, then convert to target
                $normalized = $content -replace "`r`n", "`n"
                if ($LineEnding -eq 'CRLF') {
                    $normalized = $normalized -replace "`n", "`r`n"
                }
                
                # Write back without BOM
                $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
                [System.IO.File]::WriteAllText($file.FullName, $normalized, $utf8NoBom)
            }
            
            $stats.Modified++
        }
        else {
            if ($VerboseOutput) {
                Write-Host "  $relativePath" -ForegroundColor DarkGray -NoNewline
                Write-Host " (already $currentEnding)" -ForegroundColor DarkGray
            }
            $stats.Skipped++
        }
    }
    catch {
        Write-Host "  ERROR: $relativePath - $($_.Exception.Message)" -ForegroundColor Red
        $stats.Errors++
    }
}

# Summary
Write-Host ""
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "-------" -ForegroundColor Cyan
Write-Host "  Scanned:  $($stats.Scanned)" -ForegroundColor Gray
Write-Host "  Modified: $($stats.Modified)" -ForegroundColor $(if ($stats.Modified -gt 0) { "Green" } else { "Gray" })
Write-Host "  Skipped:  $($stats.Skipped)" -ForegroundColor Gray
if ($stats.Errors -gt 0) {
    Write-Host "  Errors:   $($stats.Errors)" -ForegroundColor Red
}

if ($DryRun -and $stats.Modified -gt 0) {
    Write-Host ""
    Write-Host "Run without -DryRun to apply changes." -ForegroundColor Yellow
}

# Return exit code
exit $(if ($stats.Errors -gt 0) { 1 } else { 0 })
