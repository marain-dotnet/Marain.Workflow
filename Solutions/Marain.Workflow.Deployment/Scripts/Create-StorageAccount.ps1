#
# Create_StorageAccount.ps1
#

Param(
	[string] $ResourceGroupName, 
	[string] $ResourceGroupLocation, 
	[string] $StorageAccountName
)

Write-Host "Creating storage account..."

$storageAccount = Get-AzureRmStorageAccount | Where-Object{$_.StorageAccountName -eq $StorageAccountName}
if ($storageAccount -eq $null)
{  
	New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop
	New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageAccountName -Location $ResourceGroupLocation -Type "Standard_LRS"
}

Write-Host "Created storage account..."