#
# Add-EventHubConnectionStringToKeyVault.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName, 
	[Parameter(Mandatory=$true)]
	[string] $EventHubNamespace,
	[Parameter(Mandatory=$true)]
	[string] $EventHubName,
	[Parameter(Mandatory=$true)]
	[string] $AuthorizationRuleName,
	[Parameter(Mandatory=$true)]
	[string] $SecretName
)

# Get the key to add to keyvault

Write-Host 'Adding primary key for event hub' $EventHubNamespace '/' $EventHubName 'to KeyVault ' $KeyVaultName

$Keys = Get-AzureRmEventHubKey `
			-ResourceGroupName $ResourceGroupName `
			-NamespaceName $EventHubNamespace `
			-EventHubName $EventHubName `
			-AuthorizationRuleName $AuthorizationRuleName

# Convert to a secure string for insertion into KeyVault

$ConnectionString = ConvertTo-SecureString `
			-String $Keys.PrimaryConnectionString `
			-AsPlainText `
			-Force

# Add to KV

Set-AzureKeyVaultSecret `
    -VaultName $KeyVaultName `
	-Name $SecretName `
	-SecretValue $ConnectionString
