param(
    [string]$JsonPath
)
if(-not (Test-Path $JsonPath)){
    Write-Output "Coverage JSON not found: $JsonPath"
    exit 1
}
$j = Get-Content -Raw $JsonPath | ConvertFrom-Json
$out = @()
foreach($pkg in $j.packages){
    foreach($cls in $pkg.classes){
        $filename = $cls.filename
        if($cls.lines -ne $null){
            foreach($ln in $cls.lines){
                if(-not $ln.covered){
                    $out += [pscustomobject]@{ File = $filename; Line = $ln.number; Method = '<class>' }
                }
            }
        }
        if($cls.methods -ne $null){
            foreach($m in $cls.methods){
                foreach($ln in $m.lines){
                    if(-not $ln.covered){
                        $out += [pscustomobject]@{ File = $filename; Line = $ln.number; Method = $m.name }
                    }
                }
            }
        }
    }
}
if($out.Count -eq 0){
    Write-Output 'No uncovered lines found'
    exit 0
}
$out | Sort-Object File,Line | Format-Table -AutoSize
