#
# Add-EventHubConnectionStringToKeyVault.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName
)

$ctx = Get-AzureRmContext
$Principal = Get-AzureRmADUser -Mail $ctx.Account.Id
if (-not $Principal)
{
    # Maybe we're an external user, in which case forename.surname@example.com becomes
    # forename.surname_example.com#EXT#@tenantname.onmicrosoft.com
    $Principal = Get-AzureRmADUser | ?{$_.UserPrincipalName.StartsWith($ctx.Account.Id.Replace("@","_") + "#EXT#")}
}

Write-Host 'Revoking access from user' $Principal.DisplayName

Remove-AzureRmKeyVaultAccessPolicy `
    -VaultName $KeyVaultName `
    -ResourceGroupName $ResourceGroupName `
    -ObjectId $Principal.Id