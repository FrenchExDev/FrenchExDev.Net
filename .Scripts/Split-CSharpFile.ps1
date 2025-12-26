<#
.SYNOPSIS
    Breaks a large C# file into smaller, focused files following SOLID principles.
.DESCRIPTION
    This script analyzes a C# source file and splits it into separate files
    based on type definitions (classes, interfaces, enums, records, structs).
.PARAMETER SourceFile
    The path to the C# file to split.
.PARAMETER OutputDir
    The directory where split files will be created. Defaults to the source file's directory.
.PARAMETER RemoveSource
    If specified, removes the original source file after successful split.
.PARAMETER DryRun
    If specified, shows what would be done without making changes.
.EXAMPLE
    .\Split-CSharpFile.ps1 -SourceFile ".\Code.cs"
.EXAMPLE
    .\Split-CSharpFile.ps1 -SourceFile ".\Code.cs" -RemoveSource -DryRun
#>

param(
    [Parameter(Mandatory = $false)]
    [string]$SourceFile = "$PSScriptRoot\..\CSharp.Object.Builder2\src\FrenchExDev.Net.CSharp.Object.Builder2\Code.cs",
    
    [Parameter(Mandatory = $false)]
    [string]$OutputDir,
    
    [switch]$RemoveSource,
    
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# Resolve paths
$SourceFile = Resolve-Path $SourceFile -ErrorAction Stop
if (-not $OutputDir) {
    $OutputDir = Split-Path $SourceFile -Parent
}

Write-Host "C# File Splitter" -ForegroundColor Cyan
Write-Host "================" -ForegroundColor Cyan
Write-Host "Source: $SourceFile" -ForegroundColor Gray
Write-Host "Output: $OutputDir" -ForegroundColor Gray
if ($DryRun) { Write-Host "[DRY RUN MODE]" -ForegroundColor Yellow }
Write-Host ""

# Read source file
$sourceContent = Get-Content $SourceFile -Raw
$sourceLines = Get-Content $SourceFile

# Extract namespace
$namespaceMatch = [regex]::Match($sourceContent, 'namespace\s+([\w.]+)\s*;')
if (-not $namespaceMatch.Success) {
    $namespaceMatch = [regex]::Match($sourceContent, 'namespace\s+([\w.]+)\s*\{')
}
if (-not $namespaceMatch.Success) {
    throw "Could not find namespace in source file"
}
$namespace = $namespaceMatch.Groups[1].Value
Write-Host "Namespace: $namespace" -ForegroundColor Gray

# Extract using statements
$usingStatements = @()
$usingMatches = [regex]::Matches($sourceContent, '^\s*using\s+[^;]+;', [System.Text.RegularExpressions.RegexOptions]::Multiline)
foreach ($match in $usingMatches) {
    $usingStatements += $match.Value.Trim()
}

Write-Host "Found $($usingStatements.Count) using statements" -ForegroundColor Gray

# Parse type definitions with their content
function Get-TypeDefinitions {
    param([string]$Content, [string[]]$Lines)
    
    $types = @()
    
    # Pattern to match type declarations
    $pattern = '(?m)^(public|internal)\s+((?:sealed|abstract|static|partial)\s+)*(class|interface|enum|record(?:\s+struct)?|struct)\s+(\w+(?:<[^>]+>)?)'
    
    $lineIndex = 0
    while ($lineIndex -lt $Lines.Count) {
        $line = $Lines[$lineIndex]
        
        if ($line -match $pattern) {
            $accessModifier = $Matches[1]
            $modifiers = if ($Matches[2]) { $Matches[2].Trim() } else { "" }
            $typeKind = $Matches[3]
            $typeName = $Matches[4]
            
            # Clean up generic type names for file naming
            $fileName = $typeName -replace '<.*>', ''
            
            # Look backwards for XML doc comments
            $docStartLine = $lineIndex
            while ($docStartLine -gt 0 -and $Lines[$docStartLine - 1] -match '^\s*///') {
                $docStartLine--
            }
            
            # Find the end of this type by counting braces
            $braceCount = 0
            $foundStart = $false
            $endLine = $lineIndex
            
            for ($i = $lineIndex; $i -lt $Lines.Count; $i++) {
                $currentLine = $Lines[$i]
                
                # Count braces
                foreach ($char in $currentLine.ToCharArray()) {
                    if ($char -eq '{') { $braceCount++; $foundStart = $true }
                    if ($char -eq '}') { $braceCount-- }
                }
                
                # Check for semicolon-terminated types (like record struct Foo;)
                if (-not $foundStart -and $currentLine -match ';\s*$') {
                    $endLine = $i
                    break
                }
                
                if ($foundStart -and $braceCount -eq 0) {
                    $endLine = $i
                    break
                }
            }
            
            $typeContent = $Lines[$docStartLine..$endLine] -join "`n"
            
            $types += @{
                Name = $typeName
                FileName = $fileName
                Kind = $typeKind
                Modifiers = $modifiers
                AccessModifier = $accessModifier
                StartLine = $docStartLine
                EndLine = $endLine
                Content = $typeContent
            }
            
            # Skip to after this type
            $lineIndex = $endLine + 1
        } else {
            $lineIndex++
        }
    }
    
    return $types
}

# Get all type definitions
$types = Get-TypeDefinitions -Content $sourceContent -Lines $sourceLines

if ($types.Count -eq 0) {
    Write-Host "No type definitions found in source file" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found $($types.Count) type definitions:" -ForegroundColor Cyan
foreach ($type in $types) {
    $lineCount = $type.EndLine - $type.StartLine + 1
    Write-Host "  $($type.Kind) $($type.Name) ($lineCount lines)" -ForegroundColor Gray
}
Write-Host ""

# Group related types (concrete classes with their abstract base classes in same file)
# Note: Abstract classes implementing interfaces are NOT grouped with the interface
function Group-RelatedTypes {
    param($Types)
    
    $groups = @{}
    $assigned = @{}
    
    # First pass: identify base types and group derived types with them
    foreach ($type in $Types) {
        if ($assigned.ContainsKey($type.Name)) { continue }
        
        $groupName = $type.FileName
        
        # Skip grouping if this type is abstract - abstract types should be in their own files
        $isAbstract = $type.Modifiers -match 'abstract'
        
        if (-not $isAbstract) {
            # Check if this type inherits from another type in our list
            foreach ($potentialBase in $Types) {
                if ($potentialBase.Name -eq $type.Name) { continue }
                
                # Skip if potential base is an interface - don't group implementations with interfaces
                if ($potentialBase.Kind -eq 'interface') { continue }
                
                # Check for inheritance pattern: ": BaseType" or ": BaseType," or ": BaseType<"
                $inheritPattern = ":\s*$([regex]::Escape($potentialBase.Name))[\s,<(]"
                if ($type.Content -match $inheritPattern) {
                    $groupName = $potentialBase.FileName
                    break
                }
            }
        }
        
        if (-not $groups.ContainsKey($groupName)) {
            $groups[$groupName] = @()
        }
        $groups[$groupName] += $type
        $assigned[$type.Name] = $groupName
    }
    
    return $groups
}

$typeGroups = Group-RelatedTypes -Types $types

Write-Host "Grouped into $($typeGroups.Count) files:" -ForegroundColor Cyan
foreach ($groupName in ($typeGroups.Keys | Sort-Object)) {
    $group = $typeGroups[$groupName]
    $typeNames = ($group | ForEach-Object { $_.Name }) -join ", "
    Write-Host "  $groupName.cs: $typeNames" -ForegroundColor Gray
}
Write-Host ""

# Generate file content
function New-CSharpFile {
    param(
        [string]$FileName,
        [array]$Types,
        [string[]]$Usings,
        [string]$Namespace
    )
    
    $content = @()
    
    # Determine which usings are actually needed
    $typeContent = ($Types | ForEach-Object { $_.Content }) -join "`n"
    $neededUsings = @()
    
    foreach ($using in $Usings) {
        $usingNamespace = $using -replace 'using\s+', '' -replace ';', ''
        $parts = $usingNamespace.Split('.')
        
        # Check if any part of the namespace is referenced
        $isNeeded = $false
        foreach ($part in $parts) {
            if ($part.Length -gt 2 -and $typeContent -match "\b$([regex]::Escape($part))\b") {
                $isNeeded = $true
                break
            }
        }
        
        # Also check full namespace reference
        if ($typeContent -match [regex]::Escape($usingNamespace)) {
            $isNeeded = $true
        }
        
        if ($isNeeded) {
            $neededUsings += $using
        }
    }
    
    # Add usings
    if ($neededUsings.Count -gt 0) {
        $content += ($neededUsings | Sort-Object -Unique)
        $content += ""
    }
    
    # Add namespace
    $content += "namespace $Namespace;"
    $content += ""
    
    # Add each type
    foreach ($type in $Types) {
        $content += $type.Content
        $content += ""
    }
    
    return ($content -join "`n").TrimEnd() + "`n"
}

# Create output files
$createdFiles = @()
$skippedFiles = @()

foreach ($groupName in ($typeGroups.Keys | Sort-Object)) {
    $group = $typeGroups[$groupName]
    $fileName = "$groupName.cs"
    $filePath = Join-Path $OutputDir $fileName
    
    $newContent = New-CSharpFile -FileName $fileName -Types $group -Usings $usingStatements -Namespace $namespace
    
    if ($DryRun) {
        Write-Host "Would create: $fileName" -ForegroundColor Yellow
        Write-Host "  Types: $(($group | ForEach-Object { $_.Name }) -join ', ')" -ForegroundColor Gray
        Write-Host "  Lines: $(($newContent -split "`n").Count)" -ForegroundColor Gray
    } else {
        # Check if file exists and is different from source
        $sourceFileName = Split-Path $SourceFile -Leaf
        if ((Test-Path $filePath) -and ($fileName -ne $sourceFileName)) {
            Write-Host "Skipping existing: $fileName" -ForegroundColor Yellow
            $skippedFiles += $fileName
            continue
        }
        
        Set-Content -Path $filePath -Value $newContent -Encoding UTF8 -NoNewline
        $createdFiles += $fileName
        Write-Host "Created: $fileName" -ForegroundColor Green
    }
}

# Remove source file if requested and we created new files
if ($RemoveSource -and -not $DryRun -and $createdFiles.Count -gt 0) {
    $sourceFileName = Split-Path $SourceFile -Leaf
    if ($sourceFileName -notin $createdFiles) {
        Remove-Item $SourceFile -Force
        Write-Host "Removed: $sourceFileName" -ForegroundColor Yellow
    }
}

Write-Host ""

# Verify build if not dry run and we created files
if (-not $DryRun -and $createdFiles.Count -gt 0) {
    Write-Host "Verifying build..." -ForegroundColor Cyan
    
    # Find solution directory
    $slnDir = $OutputDir
    while ($slnDir -and -not (Test-Path (Join-Path $slnDir "*.sln"))) {
        $parent = Split-Path $slnDir -Parent
        if ($parent -eq $slnDir) { break }
        $slnDir = $parent
    }
    
    if ($slnDir -and (Test-Path (Join-Path $slnDir "*.sln"))) {
        Push-Location $slnDir
        try {
            $buildResult = dotnet build --verbosity quiet 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Build successful!" -ForegroundColor Green
            } else {
                Write-Host "Build FAILED!" -ForegroundColor Red
                $buildResult | ForEach-Object { Write-Host $_ -ForegroundColor Red }
                exit 1
            }
        } finally {
            Pop-Location
        }
    } else {
        Write-Host "Could not find solution file to verify build" -ForegroundColor Yellow
    }
}

# Summary
Write-Host ""
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "-------" -ForegroundColor Cyan
Write-Host "  Source: $(Split-Path $SourceFile -Leaf)" -ForegroundColor Gray
Write-Host "  Types:  $($types.Count)" -ForegroundColor Gray
Write-Host "  Files:  $($createdFiles.Count) created, $($skippedFiles.Count) skipped" -ForegroundColor Gray

if ($createdFiles.Count -gt 0) {
    Write-Host ""
    Write-Host "Created files:" -ForegroundColor Gray
    foreach ($file in ($createdFiles | Sort-Object)) {
        $filePath = Join-Path $OutputDir $file
        if (Test-Path $filePath) {
            $lines = (Get-Content $filePath | Measure-Object -Line).Lines
            Write-Host "  $file ($lines lines)" -ForegroundColor Gray
        }
    }
}
