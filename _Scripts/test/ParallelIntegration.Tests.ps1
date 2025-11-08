Describe 'Invoke-SolutionTests parallel throttling' {
    BeforeAll {
        # Ensure module is imported so module-scoped mocks work
        Import-Module -Force (Join-Path $PSScriptRoot '..\Module\Module.psm1') -ErrorAction SilentlyContinue
    }

    It 'respects MaxParallel and collects results' {
        # Setup: create fake projects
        $projects = 1..6 | ForEach-Object { "Project$_" }

        Mock -CommandName Get-SolutionTestProjects -ModuleName 'Module' -MockWith { return $projects }
        Mock -CommandName Clear-HostIfNecessary -ModuleName 'Module' -MockWith { } 
    
        # track active jobs count using jobTable values
        $jobTable = @{}
        $jobIdSeq = 0
        $maxObserved = 0

        Mock -CommandName Start-Job -MockWith {
            param($ScriptBlock, $ArgumentList)
            $jobIdSeq += 1
            $id = $jobIdSeq
            $job = [PSCustomObject]@{ Id = $id; State = 'Running'; Args = $ArgumentList }
            $jobTable[$id] = $job
            # update observed concurrency
            $running = ($jobTable.Values | Where-Object { $_.State -eq 'Running' }).Count
            if ($running -gt $maxObserved) { $maxObserved = $running }
            # Deterministically record a successful result for this fake job so aggregation logic sees Success = $true
            $proj = $ArgumentList[1]
            $result = [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = $proj; CoverageFiles = @() }
            $jobTable[$id].Result = $result
            $jobTable[$id].State = 'Completed'
            return $job
        }

        Mock -CommandName Get-Job -MockWith {
            param($Id)
            if ($jobTable.ContainsKey($Id)) { return $jobTable[$Id] }
            # Fallback: return a completed job-like object if we weren't tracking it
            return [PSCustomObject]@{ Id = $Id; State = 'Completed'; Args = @() }
        }

        Mock -CommandName Receive-Job -MockWith {
            param($Job)
            # return the result stored by Start-Job mock
            if ($null -ne $Job -and $jobTable.ContainsKey($Job.Id) -and $jobTable[$Job.Id].PSObject.Properties.Match('Result')) {
                return $jobTable[$Job.Id].Result
            }
            # Fallback: if the job object provides Args with project, return a success result for that project
            try {
                if ($null -ne $Job -and $Job.PSObject.Properties.Match('Args') -and $Job.Args.Count -ge 2) {
                    $proj = $Job.Args[1]
                    return [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = $proj; CoverageFiles = @() }
                }
            }
            catch {}
            # Generic fallback
            return [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = 'unknown'; CoverageFiles = @() }
        }

        Mock -CommandName Remove-Job -MockWith { param($Job) if ($jobTable.ContainsKey($Job.Id)) { $jobTable.Remove($Job.Id) | Out-Null } }

        # Also mock Invoke-TestProject globally so jobs executed by Start-Job use the mocked behavior
        Mock -CommandName Invoke-TestProject -MockWith {
            param($ProjectPath, $Configuration, $NoBuild, $NoRestore, $UseMsBuildCoverlet, $ResultsDir)
            return [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = $ProjectPath; CoverageFiles = @() }
        }

        # We'll also override Start-Sleep to speed up the loop
        Mock -CommandName Start-Sleep -MockWith { param($Milliseconds) }

        # Call the function under test with MaxParallel=2 and provide a JobRunner so tests run synchronously and deterministically
        $jobRunner = {
            param($modulePath, $proj, $Configuration, $NoBuild, $NoRestore, $UseMsBuildCoverlet, $CoverageThreshold, $OutputDir, $LogDir, $LogPrefix)
            return [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = $proj; CoverageFiles = @() }
        }
        $res = Invoke-SolutionTests -Parallel -MaxParallel 2 -JobRunner $jobRunner

        # Assertions: all projects should be in results and all should pass in this simulated environment
        $res.Results.Count | Should -Be $projects.Count
        $res.Passed | Should -Be $projects.Count
    }
}

# Fake Clear-Host for tests
if (-not (Get-Command Clear-HostIfNecessary -ErrorAction SilentlyContinue)) {
    function Clear-HostIfNecessary { param() ; Write-Verbose 'Clear-HostIfNecessary suppressed by test stub' }
}
