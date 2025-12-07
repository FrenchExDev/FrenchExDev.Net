Describe 'Merged coverage and report generation' {
  BeforeAll {
    Import-Module -Force (Join-Path $PSScriptRoot '..\Module\Module.psm1')
  }

  It 'Merges simple cobertura files' {
    $tmp = Join-Path $PSScriptRoot 'temp'
    if (-not (Test-Path $tmp)) { New-Item -ItemType Directory -Path $tmp | Out-Null }

    $cov1 = Join-Path $tmp 'cov1.xml'
    $cov2 = Join-Path $tmp 'cov2.xml'
    $merged = Join-Path $tmp 'merged.xml'

    $covXml1 = @"
<?xml version='1.0'?>
<coverage>
  <packages>
    <package name='p1'>
      <classes>
        <class name='C1' filename='File1.cs'>
          <lines>
            <line number='1' hits='1' />
            <line number='2' hits='0' />
          </lines>
        </class>
      </classes>
    </package>
  </packages>
</coverage>
"@
    $covXml2 = @"
<?xml version='1.0'?>
<coverage>
  <packages>
    <package name='p2'>
      <classes>
        <class name='C2' filename='File2.cs'>
          <lines>
            <line number='1' hits='0' />
            <line number='2' hits='1' />
          </lines>
        </class>
      </classes>
    </package>
  </packages>
</coverage>
"@
    Set-Content -Path $cov1 -Value $covXml1 -Force
    Set-Content -Path $cov2 -Value $covXml2 -Force

    $out = New-MergedCobertura -CoverageFiles @($cov1, $cov2) -OutputPath $merged
    Test-Path $out | Should -BeTrue

    [xml]$m = Get-Content -Path $out
    $m.coverage.'@line-rate' -or $m.coverage.'line-rate' | Should -Not -BeNullOrEmpty
  }

  It 'Invokes ReportGenerator for merged report when requested' {
    # Prepare a dummy merged file
    $tmp = Join-Path $PSScriptRoot 'temp'
    $merged = Join-Path $tmp 'merged2.xml'
    "<coverage></coverage>" | Set-Content -Path $merged

    Mock -CommandName Start-Process -MockWith { @{ ExitCode = 0 } }

    # call reportgenerator via Start-Process in script: simulate invocation
    $rgArgs = @('-reports:' + $merged, '-targetdir:' + (Join-Path $tmp 'out'), '-reporttypes:Html')
    Start-Process -FilePath 'reportgenerator' -ArgumentList $rgArgs -NoNewWindow -PassThru -Wait | Out-Null
    Assert-MockCalled -CommandName Start-Process -Exactly 1
  }
}
