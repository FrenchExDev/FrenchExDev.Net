[cmdletbinding()]
param(
    [string[]] $Solutions,
    [parameter(Mandatory = $true)] [string] $Title,
    [string] $OutputDirectory,
    [int] $MaxParallel = 4
)

$OutputDirectory = if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    Join-Path -Path $(get-location) -ChildPath $("test/report/$(Get-Date -Format 'yyyy-MM-dd-HH-mm-ss')") 
}
else {
    $OutputDirectory
}

$RunSolutionTestsParams = @{
    Include              = $Solutions
    GenerateCoverageJson = $true
    GenerateCoverageHtml = $true
    CoverageReportTitle  = $Title
    RunOutputRoot        = $OutputDirectory
    Serve                = $true
    MaxParallel          = $MaxParallel
    Parallel             = $true
}

. $PSScriptRoot\Run-SolutionTests.ps1 @RunSolutionTestsParams