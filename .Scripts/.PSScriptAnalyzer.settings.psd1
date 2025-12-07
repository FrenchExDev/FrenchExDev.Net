@{
    # PSScriptAnalyzer settings scoped to the _Scripts folder
    # Disable rules that are intentionally used in these helper scripts.
    Rules = @{
        # Many scripts in this folder use Write-Host for user-facing console output.
        'PSAvoidUsingWriteHost' = @{ Enable = $false }
        # You can add more rule overrides here if needed, for example:
        # 'PSAvoidUsingCmdletAliases' = @{ Enable = $false }
    }

    # Optionally include/exclude rule lists (left empty to keep defaults)
    IncludeRules = @()
    ExcludeRules = @()
}
