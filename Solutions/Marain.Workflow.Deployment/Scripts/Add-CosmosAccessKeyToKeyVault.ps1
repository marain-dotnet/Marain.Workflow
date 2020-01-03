#
# Add-EventHubConnectionStringToKeyVault.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName, 
	[Parameter(Mandatory=$true)]
	[string] $CosmosDbName,
	[Parameter(Mandatory=$true)]
	[string] $SecretName
)

# Get the key to add to keyvault

Write-Host 'Adding primary key for Cosmos DB ' $CosmosDbName 'to KeyVault ' $KeyVaultName

$CosmosKeys = Invoke-AzureRmResourceAction `
	-Action listKeys `
	-ResourceType "Microsoft.DocumentDb/databaseAccounts" `
	-ApiVersion "2015-04-08" `
	-Name $CosmosDbName `
	-ResourceGroupName $ResourceGroupName `
	-Force

# Convert to a secure string for insertion into KeyVault

$SecretValue = ConvertTo-SecureString `
			-String $CosmosKeys.primaryMasterKey `
			-AsPlainText `
			-Force

# Add to KV

Set-AzureKeyVaultSecret `
    -VaultName $KeyVaultName `
	-Name $SecretName `
	-SecretValue $SecretValue
