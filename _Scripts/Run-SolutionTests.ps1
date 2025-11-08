[CmdletBinding()]
param(
    [string]$SolutionPath = '',
    [string[]]$Include = @(),
    [string[]]$Exclude = @(),
    [switch]$Parallel,
    [switch]$NoBuild,
    [switch]$NoRestore,
    [switch]$NoRun,
    [switch]$UseMsBuildCoverlet,
    [string]$Configuration = 'Release',
    [int]$CoverageThreshold = -1,
    [switch]$GenerateMergedReport,
    [string]$MergedReportDir = '',
    [string[]]$ReportTypes = @('Html'),
    [int] $MaxParallel = 5,
    [string]$RunOutputRoot = '',
    [switch]$WriteMergedCobertura,
    [string]$MergedCoberturaPath = '',
    [switch]$GenerateCoverageJson,
    [switch]$GenerateCoverageHtml,
    [string]$CoverageReportTitle = 'Code Coverage Report',
    [switch]$Serve,
    [int]$ServePort = 8080,
    [switch]$HaltOnError,
    [switch]$TableOnly,
    [switch]$VerboseRunnerOutput
)

# Import module from _Scripts/Module
Import-Module -Force -Name (Join-Path -Path $PSScriptRoot -ChildPath 'Module')

# Helper: get available test projects for completion
function Get-AvailableTestProjects {
    try { Get-SolutionTestProjects -SolutionPath $SolutionPath } catch { return @() }
}

# Register argument completers for Include/Exclude to match project names/paths
Register-ArgumentCompleter -CommandName $MyInvocation.MyCommand.Name -ParameterName Include -ScriptBlock {
    param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameters)
    $projects = Get-AvailableTestProjects
    $projects | ForEach-Object { [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_) } | Where-Object { $_.ListItemText -like "*$wordToComplete*" }
}
Register-ArgumentCompleter -CommandName $MyInvocation.MyCommand.Name -ParameterName Exclude -ScriptBlock {
    param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameters)
    $projects = Get-AvailableTestProjects
    $projects | ForEach-Object { [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_) } | Where-Object { $_.ListItemText -like "*$wordToComplete*" }
}

if ($NoRun) {
    Write-Host "NoRun specified. Listing matching test projects..."
    $list = Get-SolutionTestProjects -SolutionPath $SolutionPath -IncludePatterns $Include -ExcludePatterns $Exclude
    $list | ForEach-Object { Write-Host $_ }
    return
}

Write-Host "Running solution tests (SolutionPath=$SolutionPath)" -ForegroundColor Green
# honor -Verbose for more detailed logging
$DetailedLogging = $PSBoundParameters.ContainsKey('Verbose')
$res = Invoke-SolutionTests -SolutionPath $SolutionPath -Include $Include -Exclude $Exclude -NoBuild:$NoBuild -NoRestore:$NoRestore -Configuration $Configuration -UseMsBuildCoverlet:$UseMsBuildCoverlet -CoverageThreshold $CoverageThreshold -GenerateMergedReport:$GenerateMergedReport -MergedReportDir $MergedReportDir -ReportTypes $ReportTypes -Parallel:$Parallel -RunOutputRoot $RunOutputRoot -WriteMergedCobertura:$WriteMergedCobertura -MergedCoberturaPath $MergedCoberturaPath -GenerateCoverageJson:$GenerateCoverageJson -GenerateCoverageHtml:$GenerateCoverageHtml -CoverageReportTitle $CoverageReportTitle -Serve:$Serve -ServePort $ServePort -HaltOnError:$HaltOnError -TableOnly:$TableOnly -VerboseRunnerOutput:$VerboseRunnerOutput -DetailedLogging:$DetailedLogging -MaxParallel:$MaxParallel

Write-Host "Summary: Total=$($res.Total) Passed=$($res.Passed) Failed=$($res.Failed)" -ForegroundColor Yellow

if ($res.Results) {
    foreach ($r in $res.Results) {
        if ($r.Success) { $status = 'OK'; $color = 'Green' } else { $status = 'FAILED'; $color = 'Red' }
        Write-Host "[$status] $($r.ProjectPath) - CoverageFiles: $($r.CoverageFiles.Count)" -ForegroundColor $color
    }
}
