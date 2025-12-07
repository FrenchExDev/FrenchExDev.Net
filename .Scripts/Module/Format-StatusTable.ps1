function Format-StatusTable {
    [CmdletBinding()]
    param(
        [hashtable]$jobStatus,
        [int]$Total,
        [int]$Passed,
        [int]$Failed,
        [int]$ActiveCount,
        [int]$spinnerIndex = 0,
        [switch]$TableOnly,
        [switch]$VerboseRunnerOutput,
        [switch]$DetailedLogging
    )

    # Local spinner so the function is self-contained
    $spinner = @('|','/','-','\\')
    # Determine console width and allocate column widths
    try { $consoleWidth = [int][System.Console]::WindowWidth } catch { $consoleWidth = 120 }
    $idxW = 4
    $stateW = 10
    $elapsedW = 10
    $padding = 3
    $projectW = [Math]::Max(20, $consoleWidth - ($idxW + $stateW + $elapsedW + $padding))

    function Get-TruncatedText {
        param(
            [Parameter(Mandatory=$true)][string]$Text,
            [Parameter(Mandatory=$true)][int]$Max
        )
        if ($null -eq $Text) { return '' }
        $t = [string]$Text
        if ($t.Length -le $Max) { return $t }
        if ($Max -le 3) { return $t.Substring(0,$Max) }
        return ($t.Substring(0,$Max-3) + '...')
    }

    $lines = @()
    $header = "{0,-$idxW} {1,-$projectW} {2,-$stateW} {3,-$elapsedW}" -f 'Idx','Project','State','Elapsed'
    $divider = ('-' * [Math]::Min($consoleWidth, ($idxW + $projectW + $stateW + $elapsedW + 3)))

    $idx = 0
    $durations = @()
    foreach ($k in $jobStatus.Keys) {
        $s = $jobStatus[$k]
        # compute elapsed safely
        $elapsed = ''
        try {
            if (($s.Started -is [DateTime]) -and ($s.Ended -is [DateTime])) { $elapsed = ($s.Ended - $s.Started).ToString('hh\:mm\:ss') }
            elseif ($s.Started -is [DateTime]) { $elapsed = ((Get-Date) - $s.Started).ToString('hh\:mm\:ss') }
        } catch { $elapsed = '' }
        if (($s.Started -is [DateTime]) -and ($s.Ended -is [DateTime])) { $durations += ($s.Ended - $s.Started).TotalSeconds }

        $projName = Get-TruncatedText -Text ([IO.Path]::GetFileName($s.Project)) -Max $projectW
        $line = "{0,-$idxW} {1,-$projectW} {2,-$stateW} {3,-$elapsedW}" -f $idx, $projName, $s.State, $elapsed
        # determine color for state column
        $color = 'White'
        if ($s.State -eq 'Running') { $color = 'Yellow' }
        elseif ($s.State -eq 'Done') {
            if ($s.Result -and $s.Result.Success) { $color = 'Green' } else { $color = 'Red' }
        } elseif ($s.State -match 'Fail') { $color = 'Red' }
        $lines += [ordered]@{ Text = $line; Color = $color }
        $idx += 1
    }

    $avg = 0
    if ($durations.Count -gt 0) { $avg = ([math]::Round(($durations | Measure-Object -Average).Average,2)) }
    $remaining = $Total - ($Passed + $Failed)
    $eta = ''
    if ($avg -gt 0 -and $remaining -gt 0) { $eta = (New-TimeSpan -Seconds ([int]($avg * $remaining))).ToString('hh\:mm\:ss') }

    $summary = "Total: $Total  Passed: $Passed  Failed: $Failed  Active: $ActiveCount  ETA: $eta  Spinner: $($spinner[$spinnerIndex % $spinner.Count])"
    # Clear host when table-only or when not printing verbose runner output. Delegate to wrapper
    if ($TableOnly -or (-not $VerboseRunnerOutput)) { Clear-HostIfNecessary }
    Write-Host $summary -ForegroundColor Cyan
    Write-Host $header -ForegroundColor Gray
    Write-Host $divider -ForegroundColor DarkGray
    foreach ($l in $lines) { Write-Host $l.Text -ForegroundColor $l.Color }
    if ($DetailedLogging) { Write-Host "[Debug] Rendered $($lines.Count) rows; consoleWidth=$consoleWidth; projectW=$projectW" -ForegroundColor DarkGray }
}
