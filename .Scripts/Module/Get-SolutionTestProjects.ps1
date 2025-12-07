function Get-SolutionTestProjects {
    param(
        [string]$SolutionPath = '',
        [string[]]$IncludePatterns = @(),
        [string[]]$ExcludePatterns = @()
    )

    if ([string]::IsNullOrWhiteSpace($SolutionPath)) {
        # default: look in current directory for .sln or .slnx
        $searchDir = Get-Location
    }
    else {
        $searchDir = $SolutionPath
    }

    # If a directory was provided, search it for a .sln/.slnx (non-recursive)
    if (Test-Path $searchDir) {
        $item = Get-Item -Path $searchDir -ErrorAction SilentlyContinue
        if ($item -and $item.PSIsContainer) {
            $found = Get-ChildItem -Path $searchDir -File -Filter '*.sln' -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($null -eq $found) { $found = Get-ChildItem -Path $searchDir -File -Filter '*.slnx' -ErrorAction SilentlyContinue | Select-Object -First 1 }
            if ($null -eq $found) { throw "No solution file (*.sln or *.slnx) found in directory '$searchDir'. Provide -SolutionPath pointing to a .sln file." }
            $SolutionPath = $found.FullName
        } else {
            # treat searchDir as a file path
            $SolutionPath = $searchDir
        }
    } else {
        throw "Solution path '$searchDir' does not exist."
    }

    if (-not (Test-Path $SolutionPath)) { throw "Solution file '$SolutionPath' does not exist." }

    $content = Get-Content -Path $SolutionPath -ErrorAction Stop
    $projects = @()
    $solutionDir = Split-Path -Path $SolutionPath -Parent

    # Example line: Project("{GUID}") = "Name", "path\to\proj.csproj", "{GUID}"
    $projectLineRegex = 'Project\("\{[A-F0-9a-f-]+\}"\)\s*=\s*"(?<name>[^"]+)",\s*"(?<path>[^"]+)"'
    foreach ($line in $content) {
        if ($line -match $projectLineRegex) {
            $p = $matches['path'] -replace '/', '\\'
            if ($p -match '\.csproj$') {
                $full = Join-Path -Path $solutionDir -ChildPath $p
                try {
                    $resolved = Resolve-Path -Path $full -ErrorAction Stop
                    $projects += $resolved.Path
                } catch {
                    # ignore missing project files
                }
            }
        }
    }

    # Filter by projects that reference the Microsoft.NET.Test.Sdk package
    $testProjects = @()
    foreach ($project in $projects) {
        try {
            $projectContent = Get-Content -Path $project -Raw -ErrorAction Stop
            if ($projectContent -match '<\s*PackageReference\s+Include\s*=\s*"Microsoft\.NET\.Test\.Sdk"') {
                $testProjects += [System.IO.Path]::GetFullPath($project)
            }
        } catch {
            # ignore unreadable project files
        }
    }

    if ($IncludePatterns.Length -gt 0) {
        $incRegex = ($IncludePatterns | ForEach-Object { [regex]::Escape($_) }) -join '|'
        $testProjects = $testProjects | Where-Object { $_ -match $incRegex }
    }
    if ($ExcludePatterns.Length -gt 0) {
        $excRegex = ($ExcludePatterns | ForEach-Object { [regex]::Escape($_) }) -join '|'
        $testProjects = $testProjects | Where-Object { -not ($_ -match $excRegex) }
    }

    return $testProjects
}
