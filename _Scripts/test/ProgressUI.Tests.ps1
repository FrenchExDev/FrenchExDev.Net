# Ensure the helper module is loadable for tests
try {
    $modulePath = Join-Path -Path $PSScriptRoot -ChildPath '..\Module\Module.psm1'
    Import-Module -Name $modulePath -Force -ErrorAction SilentlyContinue
}
catch {
    # fallback: dot-source individual script files
    . (Join-Path $PSScriptRoot '..\Module\Run-SolutionTests.ps1')
    . (Join-Path $PSScriptRoot '..\Module\Get-SolutionTestProjects.ps1')
    . (Join-Path $PSScriptRoot '..\Module\Run-TestProject.ps1')
}

# Ensure the core script is dot-sourced so Invoke-SolutionTests is available
. (Join-Path $PSScriptRoot '..\Module\Run-SolutionTests.ps1')
. (Join-Path $PSScriptRoot '..\Module\Get-SolutionTestProjects.ps1')
. (Join-Path $PSScriptRoot '..\Module\Run-TestProject.ps1')

# Ensure stub functions exist so Pester Mock can replace them even if module import didn't expose them
if (-not (Get-Command Get-SolutionTestProjects -ErrorAction SilentlyContinue)) {
    function Get-SolutionTestProjects { param($SolutionPath, $IncludePatterns, $ExcludePatterns) ; return @() }
}
if (-not (Get-Command Invoke-TestProject -ErrorAction SilentlyContinue)) {
    function Invoke-TestProject { param($ProjectPath) ; return @{ Success = $true; ProjectPath = $ProjectPath; CoverageFiles = @() } }
}
if (-not (Get-Command Clear-HostIfNecessary -ErrorAction SilentlyContinue)) {
    function Clear-HostIfNecessary { param() ; Write-Verbose 'Clear-HostIfNecessary suppressed by test stub' }
}

Describe 'Invoke-SolutionTests progress UI (sequential)' {
    It 'returns valid summary when Invoke-TestProject mocked' {
        # Use Pester Mock to avoid touching the filesystem or running dotnet
        Mock -CommandName Get-SolutionTestProjects -ModuleName 'Module' -MockWith { return @('C:\proj\A.Tests.csproj', 'C:\proj\B.Tests.csproj') }
        Mock -CommandName Invoke-TestProject -ModuleName 'Module' -MockWith {
            param($ProjectPath)
            if ($ProjectPath -like '*A*') { return @{ Success = $true; ProjectPath = $ProjectPath; CoverageFiles = @('a.xml') } }
            else { return @{ Success = $false; ProjectPath = $ProjectPath; CoverageFiles = @(); Message = 'failed' } }
        }

        $res = Invoke-SolutionTests -SolutionPath '' -Include @() -Exclude @() -Parallel:$false -GenerateMergedReport:$false -TableOnly
        $res | Should -Not -BeNullOrEmpty
        $res.Total | Should -Be 2
        $res.Passed | Should -Be 1
        $res.Failed | Should -Be 1
    }
}

Describe 'Format-StatusTable UI' {
    It 'renders header, divider and rows for sample jobStatus' {
        # Arrange: construct sample jobStatus hashtable
        $jobStatus = @{}
        $jobStatus[1] = [ordered]@{ Project = 'Project.A.Tests.csproj'; State = 'Running'; Result = $null; Started = (Get-Date).AddSeconds(-5); Ended = $null; LogPrefix = 'p1'; LogDir = '.' }
        $jobStatus[2] = [ordered]@{ Project = 'Project.B.Tests.csproj'; State = 'Done'; Result = @{ Success = $true }; Started = (Get-Date).AddSeconds(-20); Ended = (Get-Date).AddSeconds(-15); LogPrefix = 'p2'; LogDir = '.' }
        $jobStatus[3] = [ordered]@{ Project = 'Project.With.A.Very.Long.Name.ThatMightBeTruncated.Tests.csproj'; State = 'Done'; Result = @{ Success = $false }; Started = (Get-Date).AddSeconds(-30); Ended = (Get-Date).AddSeconds(-5); LogPrefix = 'p3'; LogDir = '.' }

        # Act: capture output of Format-StatusTable
        Import-Module -Force (Join-Path $PSScriptRoot '..\Module')

        Mock -CommandName Clear-HostIfNecessary -ModuleName 'Module' -MockWith { }

        $out = & {
            # call the function and capture host-written lines
            $sb = [scriptblock]::Create("Format-StatusTable -jobStatus `$jobStatus -Total 3 -Passed 1 -Failed 1 -ActiveCount 1 -spinnerIndex 0 -TableOnly -DetailedLogging")
            & $sb *>&1
        } | Out-String

        # Assert: header and divider and at least 3 rows appear in output
        $out | Should -Match 'Idx\s+Project\s+State\s+Elapsed'
        $out | Should -Match 'Total: 3\s+Passed: 1\s+Failed: 1'
        $out | Should -Match 'Project.A.Tests.csproj'
        $out | Should -Match 'Project.B.Tests.csproj'
        $out | Should -Match 'Project.With.A.Very.Long.Name'
        # Debug line when DetailedLogging is on
        $out | Should -Match '\[Debug\] Rendered'
    }
}
