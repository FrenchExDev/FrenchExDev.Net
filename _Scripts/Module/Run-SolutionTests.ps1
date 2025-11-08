function New-MergedCobertura {
    param(
        [string[]]$CoverageFiles,
        [string]$OutputPath
    )

    if ($CoverageFiles.Count -eq 0) { throw 'No coverage files provided' }

    # We'll build a merged cobertura xml by concatenating package/class elements and summing line hits to compute overall line-rate
    $mergedDoc = New-Object System.Xml.XmlDocument
    $root = $mergedDoc.CreateElement('coverage')
    $mergedDoc.AppendChild($root) | Out-Null

    $totalLines = 0
    $coveredLines = 0

    # map of classes -> lines (keyed by filename|classname)
    $classesMap = @{}

    foreach ($file in $CoverageFiles) {
        try {
            [xml]$doc = Get-Content $file -Raw
        }
        catch {
            continue
        }

        # Cobertura variants may represent attributes as either attributes or child elements; handle both.
        # Walk packages -> package -> classes -> class, but also allow classes directly under coverage.
        $candidateClasses = @()
        try {
            if ($doc.coverage.packages -and $doc.coverage.packages.package) {
                foreach ($pkg in $doc.coverage.packages.package) {
                    try { $candidateClasses += $pkg.classes.class } catch {}
                }
            }
        }
        catch {}
        # fallback: classes directly under coverage
        if ($candidateClasses.Count -eq 0) {
            try { $candidateClasses = $doc.coverage.classes.class } catch {}
        }
        # Also tolerate case where class nodes are present as single node
        if ($candidateClasses -is [System.Xml.XmlElement]) { $candidateClasses = , $candidateClasses }

        foreach ($c in $candidateClasses) {
            if (-not $c) { continue }

            # class name and filename: prefer attribute, fallback to child element or attribute variations
            $className = $null
            $fileName = $null
            try { $className = $c.Attributes['name'].Value } catch { }
            if (-not $className) {
                try { $className = $c.name } catch { }
            }
            try { $fileName = $c.Attributes['filename'].Value } catch { }
            if (-not $fileName) {
                try { $fileName = $c.filename } catch { }
            }
            if (-not $fileName) { $fileName = 'unknown' }
            if (-not $className) { $className = "Class_in_$fileName" }

            $key = "$fileName||$className"
            if (-not $classesMap.ContainsKey($key)) {
                $classesMap[$key] = [ordered]@{ Name = $className; Filename = $fileName; Lines = @{} }
            }

            # lines may be under c.lines.line or c.lines/line element with attributes or child nodes.
            $linesNodes = @()
            try { $linesNodes = $c.lines.line } catch { }
            if ($linesNodes.Count -eq 0) {
                try { $linesNodes = $c.line } catch { }
            }
            if ($linesNodes -is [System.Xml.XmlElement]) { $linesNodes = , $linesNodes }

            foreach ($ln in $linesNodes) {
                if (-not $ln) { continue }
                # number and hits: attribute preferred, fallback to child element text
                $lnNum = $null; $lnHits = $null
                try { $lnNum = $ln.Attributes['number'].Value } catch { }
                if (-not $lnNum) { try { $lnNum = $ln.Attributes['nr'].Value } catch { } }
                try { $lnHits = $ln.Attributes['hits'].Value } catch { }
                if (-not $lnHits) { try { $lnHits = $ln.Attributes['count'].Value } catch { } }
                if (-not $lnNum) {
                    try { $lnNum = $ln.number } catch { }
                    if (-not $lnNum) { try { $lnNum = $ln.nr } catch { } }
                }
                if (-not $lnHits) {
                    try { $lnHits = $ln.hits } catch { }
                    if (-not $lnHits) { try { $lnHits = $ln.count } catch { } }
                }
                if ($null -eq $lnNum) { continue }
                $num = 0; $hits = 0
                try { $num = [int]$lnNum } catch { continue }
                try { $hits = [int]$lnHits } catch { $hits = 0 }
                if ($classesMap[$key].Lines.ContainsKey($num)) { $classesMap[$key].Lines[$num] += $hits } else { $classesMap[$key].Lines[$num] = $hits }
            }
        }
    }
    # build packages/classes structure from classesMap
    $packagesElem = $mergedDoc.CreateElement('packages')
    $root.AppendChild($packagesElem) | Out-Null
    $pkg = $mergedDoc.CreateElement('package')
    $pkgName = $mergedDoc.CreateAttribute('name'); $pkgName.Value = 'merged'
    $pkg.Attributes.Append($pkgName) | Out-Null
    $classesContainer = $mergedDoc.CreateElement('classes')

    foreach ($k in $classesMap.Keys) {
        $entry = $classesMap[$k]
        $classElem = $mergedDoc.CreateElement('class')
        $attrName = $mergedDoc.CreateAttribute('name'); $attrName.Value = $entry.Name; $classElem.Attributes.Append($attrName) | Out-Null
        $attrFile = $mergedDoc.CreateAttribute('filename'); $attrFile.Value = $entry.Filename; $classElem.Attributes.Append($attrFile) | Out-Null

        $linesElem = $mergedDoc.CreateElement('lines')
        foreach ($lnNum in ($entry.Lines.Keys | Sort-Object)) {
            $hits = [int]$entry.Lines[$lnNum]
            $lineElem = $mergedDoc.CreateElement('line')
            $attrNum = $mergedDoc.CreateAttribute('number'); $attrNum.Value = [string]$lnNum; $lineElem.Attributes.Append($attrNum) | Out-Null
            $attrHits = $mergedDoc.CreateAttribute('hits'); $attrHits.Value = [string]$hits; $lineElem.Attributes.Append($attrHits) | Out-Null
            $linesElem.AppendChild($lineElem) | Out-Null

            $totalLines += 1
            if ($hits -gt 0) { $coveredLines += 1 }
        }

        $classElem.AppendChild($linesElem) | Out-Null
        $classesContainer.AppendChild($classElem) | Out-Null
    }

    $pkg.AppendChild($classesContainer) | Out-Null
    $packagesElem.AppendChild($pkg) | Out-Null

    # set basic attributes
    $lineRate = 0
    if ($totalLines -gt 0) { $lineRate = [math]::Round(($coveredLines / $totalLines), 4) }
    $attr = $mergedDoc.CreateAttribute('line-rate'); $attr.Value = $lineRate.ToString()
    $root.Attributes.Append($attr) | Out-Null

    # write merged xml
    $mergedDir = Split-Path -Path $OutputPath -Parent
    if (-not (Test-Path $mergedDir)) { New-Item -ItemType Directory -Path $mergedDir -Force | Out-Null }
    $mergedDoc.Save($OutputPath)
    return $OutputPath
}

function Invoke-SolutionTests {
    [CmdletBinding()]
    param(
        [string]$SolutionPath = '',
        [string[]]$Include = @(),
        [string[]]$Exclude = @(),
        [switch]$NoBuild,
        [switch]$NoRestore,
        [string]$Configuration = 'Release',
        [switch]$UseMsBuildCoverlet,
        [int]$CoverageThreshold = -1,
        [switch]$GenerateMergedReport,
        [string]$MergedReportDir = '',
        [string[]]$ReportTypes = @('Html'),
        [switch]$Parallel,
        [int]$MaxParallel = 4,
        # Optional test hook: when provided, $JobRunner will be invoked synchronously for each project
        # during parallel execution. This allows tests to simulate job execution without spawning
        # background jobs. The JobRunner should accept the following arguments in order:
        #   ($modulePath, $proj, $Configuration, $NoBuild, $NoRestore, $UseMsBuildCoverlet, $CoverageThreshold, $LogDir, $LogPrefix)
        # and return a result object similar to Invoke-TestProject, e.g.:
        #   [PSCustomObject]@{ Success = $true; ExitCode = 0; ProjectPath = $proj; CoverageFiles = @() }
        [scriptblock]$JobRunner = $null,
        [switch]$WriteMergedCobertura,
        [string]$MergedCoberturaPath = '',
        [switch]$HaltOnError,
        [switch]$TableOnly,
        [switch]$VerboseRunnerOutput,
        [switch]$DetailedLogging
    )

    # Respect common -Verbose/-Debug switches as a DetailedLogging toggle
    $DetailedLogging = $false
    try {
        if ($PSBoundParameters.ContainsKey('Verbose') -or $PSBoundParameters.ContainsKey('Debug')) { $DetailedLogging = $true }
    }
    catch { $DetailedLogging = $false }

    # Discover projects
    $projects = Get-SolutionTestProjects -SolutionPath $SolutionPath -IncludePatterns $Include -ExcludePatterns $Exclude
    if ($projects.Count -eq 0) { Write-Warning "No test projects found after filters."; return @{ Total = 0; Passed = 0; Failed = 0; Results = @() } }

    $summary = [ordered]@{
        Total   = $projects.Count
        Passed  = 0
        Failed  = 0
        Results = @()
    }

    # ensure log dir for capturing dotnet outputs (used for both parallel and sequential runs)
    $globalLogDir = Join-Path -Path (Get-Location) -ChildPath '_Scripts\logs'
    if (-not (Test-Path $globalLogDir)) { New-Item -ItemType Directory -Path $globalLogDir -Force | Out-Null }

    if ($Parallel) {
        # Progress UI helper: show total, passed, failed, active and current operation
        function Update-ProgressUI {
            param(
                [int]$Total,
                [int]$Passed,
                [int]$Failed,
                [int]$ActiveCount,
                [string]$CurrentOperation
            )
            $completed = $Passed + $Failed
            $percent = 0
            if ($Total -gt 0) { $percent = [int](([double]$completed / $Total) * 100) }
            $status = "Passed: $Passed  Failed: $Failed  Active: $ActiveCount  Total: $Total"
            if (-not $CurrentOperation) { $CurrentOperation = '' }
            Write-Progress -Activity 'Running tests' -Status $status -PercentComplete $percent -CurrentOperation $CurrentOperation
        }

        # Status table, spinner and ETA helpers
        $jobStatus = @{}
        $statusSpinnerIndex = 0

        # Run projects in parallel using a throttled background job pool
        $modulePath = Join-Path -Path $PSScriptRoot -ChildPath 'Module.psm1'
        if ($MaxParallel -lt 1) { $MaxParallel = 1 }
        $activeJobs = @{}
        $allJobs = @()
        if ($DetailedLogging) { Write-Host "[Debug] Status table helper initialized" -ForegroundColor DarkGray }
        # ensure log dir
        $globalLogDir = Join-Path -Path (Get-Location) -ChildPath '_Scripts\logs'
        if (-not (Test-Path $globalLogDir)) { New-Item -ItemType Directory -Path $globalLogDir -Force | Out-Null }
        foreach ($proj in $projects) {
            Update-ProgressUI -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -CurrentOperation "Starting: $proj"
            # throttle
            while ($activeJobs.Count -ge $MaxParallel) {
                Start-Sleep -Milliseconds 200
                $finished = @()
                foreach ($k in $activeJobs.Keys) {
                    $st = Get-Job -Id $k -ErrorAction SilentlyContinue
                    if ($null -eq $st -or $st.State -ne 'Running') { $finished += $k }
                }
                foreach ($f in $finished) {
                    $j = $activeJobs[$f]
                    $out = Receive-Job -Job $j -ErrorAction SilentlyContinue
                    if ($null -ne $out) { $res = $out | Select-Object -First 1 } else { $res = @{ Success = $false; ExitCode = 98; Message = 'No output from job' } }
                    if ($res.Success) { $summary.Passed += 1 } else { $summary.Failed += 1 }
                    $summary.Results += $res
                    # update jobStatus
                    if ($jobStatus.ContainsKey($f)) { $jobStatus[$f].State = 'Done'; $jobStatus[$f].Result = $res; $jobStatus[$f].Ended = Get-Date }
                    # collect error output if any
                    if (-not $res.Success) {
                        try {
                            $logPrefix = $jobStatus[$f].LogPrefix
                            $logDir = $jobStatus[$f].LogDir
                            $errFile = Join-Path -Path $logDir -ChildPath ("dotnet_err_{0}.txt" -f $logPrefix)
                            if (Test-Path $errFile) { $errText = (Get-Content $errFile -Raw -ErrorAction SilentlyContinue) } else { $errText = $res.Message }
                        }
                        catch { $errText = $res.Message }
                        if (-not $summary.Contains('Errors')) { $summary['Errors'] = @() }
                        $summary.Errors += [ordered]@{ Project = $res.ProjectPath; Error = $errText }
                        if ($HaltOnError) {
                            Write-Host "Halting on first error for project: $($res.ProjectPath)" -ForegroundColor Red
                            throw $errText
                        }
                    }
                    # If verbose runner output requested, print captured stdout/stderr for this job
                    if ($VerboseRunnerOutput) {
                        try {
                            $outFile = Join-Path -Path $jobStatus[$f].LogDir -ChildPath ("dotnet_out_{0}.txt" -f $jobStatus[$f].LogPrefix)
                            $errFile = Join-Path -Path $jobStatus[$f].LogDir -ChildPath ("dotnet_err_{0}.txt" -f $jobStatus[$f].LogPrefix)
                            if (Test-Path $outFile) {
                                Write-Host "\n=== stdout for $($jobStatus[$f].Project) ===" -ForegroundColor DarkGray
                                Get-Content -Path $outFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                            }
                            if (Test-Path $errFile) {
                                Write-Host "\n=== stderr for $($jobStatus[$f].Project) ===" -ForegroundColor Red
                                Get-Content -Path $errFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                            }
                        }
                        catch { }
                    }
                    Remove-Job -Job $j -Force -ErrorAction SilentlyContinue
                    $activeJobs.Remove($f) | Out-Null
                    Update-ProgressUI -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -CurrentOperation "Processed: $($res.ProjectPath)"
                    Format-StatusTable -jobStatus $jobStatus -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -spinnerIndex $statusSpinnerIndex -TableOnly:$TableOnly -VerboseRunnerOutput:$VerboseRunnerOutput
                    $statusSpinnerIndex++
                }
            }

            # prepare per-job logs
            $logPrefix = [guid]::NewGuid().ToString()

            if ($null -ne $JobRunner) {
                # If tests provided a JobRunner, run it synchronously in-process. The JobRunner should accept the same params as the job scriptblock.
                try {
                    $res = & $JobRunner $modulePath $proj $Configuration $NoBuild.IsPresent $NoRestore.IsPresent $UseMsBuildCoverlet.IsPresent $CoverageThreshold $globalLogDir $logPrefix
                }
                catch {
                    $res = @{ Success = $false; ExitCode = 99; Message = $_.Exception.Message; ProjectPath = $proj }
                }
                # immediately aggregate result so behavior mirrors the real job processing loop
                if ($res) { $r = $res } else { $r = @{ Success = $false; ExitCode = 98; Message = 'No output from job'; ProjectPath = $proj } }
                if ($r.Success) { $summary.Passed += 1 } else { $summary.Failed += 1 }
                $summary.Results += $r
                # maintain jobStatus for consistency with UI
                $fakeId = ([guid]::NewGuid()).ToString()
                $jobStatus[$fakeId] = [ordered]@{ Project = $proj; State = 'Done'; Result = $r; Started = Get-Date; Ended = Get-Date; LogPrefix = $logPrefix; LogDir = $globalLogDir }
                Format-StatusTable -jobStatus $jobStatus -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -spinnerIndex $statusSpinnerIndex -TableOnly:$TableOnly -VerboseRunnerOutput:$VerboseRunnerOutput
                $statusSpinnerIndex++
            }
            else {
                $script = {
                    param($modulePath, $proj, $Configuration, $NoBuild, $NoRestore, $UseMsBuildCoverlet, $CoverageThreshold, $LogDir, $LogPrefix)
                    Import-Module -Name $modulePath -Force
                    try {
                        $r = Invoke-TestProject -ProjectPath $proj -Configuration $Configuration -NoBuild:$NoBuild -NoRestore:$NoRestore -UseMsBuildCoverlet:$UseMsBuildCoverlet -CoverageThreshold $CoverageThreshold -FailOnError:$false -LogDir $LogDir -LogFilePrefix $LogPrefix -KeepLogFiles
                        return $r
                    }
                    catch {
                        return @{ Success = $false; ExitCode = 99; Message = $_.Exception.Message; ProjectPath = $proj }
                    }
                }
                $j = Start-Job -ScriptBlock $script -ArgumentList $modulePath, $proj, $Configuration, $NoBuild.IsPresent, $NoRestore.IsPresent, $UseMsBuildCoverlet.IsPresent, $CoverageThreshold, $globalLogDir, $logPrefix
                $activeJobs[$j.Id] = $j
                $allJobs += $j
                # initialize job status
                $jobStatus[$j.Id] = [ordered]@{ Project = $proj; State = 'Running'; Result = $null; Started = Get-Date; Ended = $null; LogPrefix = $logPrefix; LogDir = $globalLogDir }
            }
        }

        # wait for remaining active jobs
        while ($activeJobs.Count -gt 0) {
            Start-Sleep -Milliseconds 250
            $finished = @()
            foreach ($k in $activeJobs.Keys) {
                $st = Get-Job -Id $k -ErrorAction SilentlyContinue
                if ($null -eq $st -or $st.State -ne 'Running') { $finished += $k }
            }
            foreach ($f in $finished) {
                $j = $activeJobs[$f]
                $out = Receive-Job -Job $j -ErrorAction SilentlyContinue
                if ($null -ne $out) { $res = $out | Select-Object -First 1 } else { $res = @{ Success = $false; ExitCode = 98; Message = 'No output from job' } }
                if ($res.Success) { $summary.Passed += 1 } else { $summary.Failed += 1 }
                $summary.Results += $res
                if ($jobStatus.ContainsKey($f)) { $jobStatus[$f].State = 'Done'; $jobStatus[$f].Result = $res; $jobStatus[$f].Ended = Get-Date }
                # collect error output if any
                if (-not $res.Success) {
                    try {
                        $logPrefix = $jobStatus[$f].LogPrefix
                        $logDir = $jobStatus[$f].LogDir
                        $errFile = Join-Path -Path $logDir -ChildPath ("dotnet_err_{0}.txt" -f $logPrefix)
                        if (Test-Path $errFile) { $errText = (Get-Content $errFile -Raw -ErrorAction SilentlyContinue) } else { $errText = $res.Message }
                    }
                    catch { $errText = $res.Message }
                    if (-not $summary.Contains('Errors')) { $summary['Errors'] = @() }
                    $summary['Errors'] += [ordered]@{ Project = $res.ProjectPath; Error = $errText }
                    if ($HaltOnError) {
                        Write-Host "Halting on first error for project: $($res.ProjectPath)" -ForegroundColor Red
                        throw $errText
                    }
                }
                # Optionally print per-job stdout/stderr in parallel mode when verbose output requested
                if ($VerboseRunnerOutput) {
                    try {
                        $outFile = Join-Path -Path $jobStatus[$f].LogDir -ChildPath ("dotnet_out_{0}.txt" -f $jobStatus[$f].LogPrefix)
                        $errFile = Join-Path -Path $jobStatus[$f].LogDir -ChildPath ("dotnet_err_{0}.txt" -f $jobStatus[$f].LogPrefix)
                        if (Test-Path $outFile) {
                            Write-Host "\n=== stdout for $($jobStatus[$f].Project) ===" -ForegroundColor DarkGray
                            Get-Content -Path $outFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                        }
                        if (Test-Path $errFile) {
                            Write-Host "\n=== stderr for $($jobStatus[$f].Project) ===" -ForegroundColor Red
                            Get-Content -Path $errFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                        }
                    }
                    catch { }
                }
                Remove-Job -Job $j -Force -ErrorAction SilentlyContinue
                $activeJobs.Remove($f) | Out-Null
                Format-StatusTable -jobStatus $jobStatus -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -spinnerIndex $statusSpinnerIndex -TableOnly:$TableOnly -VerboseRunnerOutput:$VerboseRunnerOutput
                $statusSpinnerIndex++
            }
            # render intermediate status even if no finished jobs to update spinner
            if ($finished.Count -eq 0) { Format-StatusTable -jobStatus $jobStatus -Total $projects.Count -Passed $summary.Passed -Failed $summary.Failed -ActiveCount $activeJobs.Count -spinnerIndex $statusSpinnerIndex -TableOnly:$TableOnly -VerboseRunnerOutput:$VerboseRunnerOutput; $statusSpinnerIndex++ }
        }
        # Completed
        Write-Progress -Activity 'Running tests' -Completed
        # Print accumulated errors if any
        if ($summary.Contains('Errors') -and $summary['Errors'].Count -gt 0) {
            # Print errors summary (do not print captured logs here to keep table-only experience)
            Write-Host "\nErrors encountered (see logs under _Scripts\logs):" -ForegroundColor Red
            foreach ($e in $summary['Errors']) {
                Write-Host "Project: $($e.Project)" -ForegroundColor Red
                Write-Host ($e.Error -split "\n" | Select-Object -First 5)
                Write-Host "----"
            }
        }
    }
    else {
        # Run projects sequentially so mocks work in tests
        foreach ($proj in $projects) {
            Write-Host "Running tests for: $proj" -ForegroundColor Cyan
            $logPrefix = [guid]::NewGuid().ToString()
            $res = Invoke-TestProject -ProjectPath $proj -Configuration $Configuration -NoBuild:$NoBuild -NoRestore:$NoRestore -UseMsBuildCoverlet:$UseMsBuildCoverlet -CoverageThreshold $CoverageThreshold -FailOnError:$false -LogDir $globalLogDir -LogFilePrefix $logPrefix -KeepLogFiles
            if ($res.Success) { $summary.Passed += 1 } else { $summary.Failed += 1 }
            $summary.Results += $res
            # print captured output for sequential run (unless TableOnly and not VerboseRunnerOutput)
            if ($VerboseRunnerOutput -or (-not $TableOnly)) {
                try {
                    $outFile = Join-Path -Path $globalLogDir -ChildPath ("dotnet_out_{0}.txt" -f $logPrefix)
                    $errFile = Join-Path -Path $globalLogDir -ChildPath ("dotnet_err_{0}.txt" -f $logPrefix)
                    if (Test-Path $outFile) {
                        Write-Host "\n=== stdout for $proj ===" -ForegroundColor DarkGray
                        Get-Content -Path $outFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                    }
                    if (Test-Path $errFile) {
                        Write-Host "\n=== stderr for $proj ===" -ForegroundColor Red
                        Get-Content -Path $errFile -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
                    }
                }
                catch { }
            }
        }
    }

    # Optionally generate merged coverage report using ReportGenerator
    $allCoverageFiles = @()
    foreach ($r in $summary.Results) {
        if ($r.CoverageFiles) { $allCoverageFiles += $r.CoverageFiles }
    }

    if ($GenerateMergedReport -and $allCoverageFiles.Count -gt 0) {
        # default merged report dir when not provided
        if ([string]::IsNullOrWhiteSpace($MergedReportDir)) {
            $timestamp = Get-Date -Format 'dd-MM-yyyy-HH-mm-ss'
            $MergedReportDir = Join-Path -Path (Get-Location) -ChildPath (Join-Path 'test' (Join-Path 'reports' $timestamp 'MergedReport'))
        }
        if (-not (Test-Path $MergedReportDir)) { New-Item -ItemType Directory -Path $MergedReportDir -Force | Out-Null }

        # optionally write a merged cobertura XML for other tooling
        if ($WriteMergedCobertura) {
            if ([string]::IsNullOrWhiteSpace($MergedCoberturaPath)) {
                $MergedCoberturaPath = Join-Path -Path $MergedReportDir -ChildPath 'merged-coverage.cobertura.xml'
            }
            try {
                New-MergedCobertura -CoverageFiles $allCoverageFiles -OutputPath $MergedCoberturaPath | Out-Null
                Write-Host "Wrote merged cobertura to: $MergedCoberturaPath"
                # prefer merged cobertura in ReportGenerator inputs
                $allCoverageFiles = , $MergedCoberturaPath
                $summary.MergedCoberturaPath = $MergedCoberturaPath
            }
            catch {
                Write-Warning "Failed to create merged cobertura: $($_.Exception.Message)"
            }
        }

        # Ensure reportgenerator is available
        $rgAvailable = $false
        try { & reportgenerator -version 2>$null | Out-Null; if ($LASTEXITCODE -eq 0) { $rgAvailable = $true } } catch { $rgAvailable = $false }
        if (-not $rgAvailable) {
            Write-Host "ReportGenerator not found. Installing global tool 'dotnet-reportgenerator-globaltool'..."
            & dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.4 2>&1 | Write-Host
            if ($LASTEXITCODE -ne 0) { Write-Warning "Failed to install reportgenerator global tool. Skipping merged report generation." }
            else { $rgAvailable = $true }
        }

        if ($rgAvailable) {
            # join reports with semicolon (ReportGenerator accepts ; separated list)
            $reportsArg = ($allCoverageFiles | ForEach-Object { $_ }) -join ';'
            $typesArg = ($ReportTypes -join ';')
            $rgArgs = @('-reports:' + $reportsArg, '-targetdir:' + $MergedReportDir, '-reporttypes:' + $typesArg)
            Write-Host "Generating merged report: reportgenerator $($rgArgs -join ' ')"
            $proc = Start-Process -FilePath reportgenerator -ArgumentList $rgArgs -NoNewWindow -PassThru -Wait -RedirectStandardOutput temp_rg_out.txt -RedirectStandardError temp_rg_err.txt
            if ($proc.ExitCode -ne 0) {
                Write-Warning "ReportGenerator failed (exit $($proc.ExitCode)). See temp_rg_out.txt and temp_rg_err.txt for details."
            }
            else {
                Write-Host "Merged report generated at: $MergedReportDir"
                Remove-Item -Path temp_rg_out.txt, temp_rg_err.txt -ErrorAction SilentlyContinue
                $summary.MergedReportDir = $MergedReportDir
                $summary.MergedReportFiles = $allCoverageFiles
            }
        }
    }

    return $summary
}
