Describe 'Solution test runner module' {
    BeforeAll {
        # Import module so functions are available under module scope
        Import-Module -Force (Join-Path $PSScriptRoot '..\Module\Module.psm1')
    }

    # Fake Clear-Host in test scope so importing the module or calling Format-StatusTable won't clear terminal
    if (-not (Get-Command Clear-HostIfNecessary -ErrorAction SilentlyContinue)) {
        function Clear-HostIfNecessary { param() ; Write-Verbose 'Clear-HostIfNecessary suppressed by test stub' }
    }

    It 'Aggregates results when all projects succeed' {
        # Mock Get-SolutionTestProjects to return two fake projects
        Mock -CommandName 'Get-SolutionTestProjects' -ModuleName 'Module' -MockWith { @('projA.csproj', 'projB.csproj') }
        # Mock Invoke-TestProject to return success objects
        Mock -CommandName 'Invoke-TestProject' -ModuleName 'Module' -MockWith { @{ Success = $true; ProjectPath = $args[0] ; CoverageFiles = @() } }

        $res = Invoke-SolutionTests -SolutionPath '.' -NoBuild -Parallel:$false

        $res.Total | Should -Be 2
        $res.Passed | Should -Be 2
        $res.Failed | Should -Be 0
    }

    It 'Reports failure when a project fails' {
        Mock -CommandName 'Get-SolutionTestProjects' -ModuleName 'Module' -MockWith { @('projA.csproj', 'projB.csproj') }
        Mock -CommandName 'Invoke-TestProject' -ModuleName 'Module' -MockWith {
            param($ProjectPath)
            if ($ProjectPath -like '*projA*') { @{ Success = $false; ExitCode = 1; ProjectPath = $ProjectPath } }
            else { @{ Success = $true; ProjectPath = $ProjectPath } }
        }

        $res = Invoke-SolutionTests -SolutionPath '.' -NoBuild -Parallel:$false

        $res.Total | Should -Be 2
        $res.Passed | Should -Be 1
        $res.Failed | Should -Be 1
    }
}
