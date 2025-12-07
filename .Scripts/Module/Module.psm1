## Clear-HostIfNecessary
## A thin wrapper around the built-in Clear-Host so production code calls this function
## and tests can stub or mock it without altering runtime behaviour.
function Clear-HostIfNecessary {
	param()
	# Default behaviour: call the real Clear-Host
	Clear-Host
}

Export-ModuleMember -Function Clear-HostIfNecessary

. $PSScriptRoot/Get-CoverageSummary.ps1
Export-ModuleMember -Function Get-CoverageSummary

. $PSScriptRoot/Run-TestProject.ps1
Export-ModuleMember -Function Invoke-TestProject

. $PSScriptRoot/Get-SolutionTestProjects.ps1
Export-ModuleMember -Function Get-SolutionTestProjects

. $PSScriptRoot/Run-SolutionTests.ps1
Export-ModuleMember -Function Invoke-SolutionTests
Export-ModuleMember -Function New-MergedCobertura

. $PSScriptRoot/Format-StatusTable.ps1
Export-ModuleMember -Function Format-StatusTable
