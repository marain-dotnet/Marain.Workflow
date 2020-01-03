<#

.EXAMLE

.\deploy.ps1 `
	-Prefix "end" `
	-AppName "workflow" `
	-Environment "dev" `
	-FunctionsMsDeployPackagePath "..\Endjin.Mdp.DataCatalog.Functions\bin\Debug\package\Endjin.Mdp.DataCatalog.Functions.zip"
#>

[CmdletBinding(DefaultParametersetName='None')] 
param(
    [string] $Prefix = "end",
	[string] $AppName = "workflow",
	[ValidateLength(3,12)]
	[string] $Suffix = "dev",
	[string] $MessageIngestionHostFunctionsMsDeployPackagePath = "..\Endjin.Workflow.Functions.MessageIngestionHost\bin\Release\package\Endjin.Workflow.Functions.MessageIngestionHost.zip",	
	[string] $EngineHostFunctionsMsDeployPackagePath = "..\Endjin.Workflow.Functions.EngineHost\bin\Release\package\Endjin.Workflow.Functions.EngineHost.zip",	
	[string] $MessagePreProcessingHostFunctionsMsDeployPackagePath = "..\Endjin.Workflow.Functions.MessagePreProcessingHost\bin\Release\package\Endjin.Workflow.Functions.MessagePreProcessingHost.zip",	
	[string] $ResourceGroupLocation = "northeurope",
	[string] $ArtifactStagingDirectory = ".",
	[string] $ArtifactStorageContainerName = "stageartifacts",
	[string] $AadTenantId,
	[string] $EngineAadClientId,
	[string] $IngestionAadClientId,
	[string] $PreprocessorAadClientId,
	[string] $OperationsControlServiceBaseUrl,
	[string] $OperationsControlServiceAadResourceId,
	[int] $EventHubThroughputUnits = 2,
	[switch] $IsDeveloperEnvironment,
	[switch] $UpdateLocalConfigFiles,
	[switch] $SkipDeployment
)

Begin{
	# Setup options and variables
	$ErrorActionPreference = 'Stop'
	Set-Location $PSScriptRoot

	$Suffix = $Suffix.ToLower()
	$AppName  = $AppName.ToLower()
	$Prefix = $Prefix.ToLower()

	$ResourceGroupName = $Prefix + "." + $AppName.ToLower() + "." + $Suffix
	$DefaultName = $Prefix + $AppName.ToLower() + $Suffix

	$ArtifactStorageResourceGroupName = $ResourceGroupName + ".artifacts";
	$ArtifactStorageAccountName = $Prefix + $AppName + $Suffix + "ar"
	$ArtifactStagingDirectory = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory))

	$FunctionsMsDeployPackageFolderName = "MsDeploy";

	$MessageIngestionHostFunctionsAppPackageFileName = [System.IO.Path]::GetFileName($MessageIngestionHostFunctionsMsDeployPackagePath)
	$EngineHostFunctionsAppPackageFileName = [System.IO.Path]::GetFileName($EngineHostFunctionsMsDeployPackagePath)
	$MessagePreProcessingHostFunctionsAppPackageFileName = [System.IO.Path]::GetFileName($MessagePreProcessingHostFunctionsMsDeployPackagePath)

	$EventHubNamespaceName = $DefaultName
	$EventHubHubName = $Prefix + $AppName.ToLower()
	$CosmosDbName = $DefaultName
	$KeyVaultName = $DefaultName

    if (-not $OperationsControlServiceBaseUrl)
    {
        $OperationsControlServiceBaseUrl = "https://" + $Prefix + "operations" + $Suffix + ".azurewebsites.net/"
    }
}

Process{
	if ($SkipDeployment) {
		Write-Host "`nSkipping deployment steps due to SkipDeployment parameter being present"
	} else {
		# Create resource group and artifact storage account
		Write-Host "`nStep1: Creating resource group $ResourceGroupName and artifact storage account $ArtifactStorageAccountName" -ForegroundColor Green
		try {
			.\Scripts\Create-StorageAccount.ps1 `
				-ResourceGroupName $ArtifactStorageResourceGroupName `
				-ResourceGroupLocation $ResourceGroupLocation `
				-StorageAccountName $ArtifactStorageAccountName
		}
		catch{
			throw $_
		}

		# Copy msbuild package to artifact directory
		if ($IsDeveloperEnvironment) {
			Write-Host "`nStep2: Skipping function msdeploy package copy as we are deploying a developer environment only"  -ForegroundColor Green
		} else {
			Write-Host "`nStep2: Coping functions msdeploy packages to artifact directory $FunctionsMsDeployPackageFolderName"  -ForegroundColor Green
			try {
				Copy-Item -Path $MessageIngestionHostFunctionsMsDeployPackagePath -Destination (New-Item -Type directory -Force "$FunctionsMsDeployPackageFolderName") -Force -Recurse
				Copy-Item -Path $EngineHostFunctionsMsDeployPackagePath -Destination (New-Item -Type directory -Force "$FunctionsMsDeployPackageFolderName") -Force -Recurse
				Copy-Item -Path $MessagePreProcessingHostFunctionsMsDeployPackagePath -Destination (New-Item -Type directory -Force "$FunctionsMsDeployPackageFolderName") -Force -Recurse
			}
			catch{
				throw $_
			}
		}

		# Deploy main ARM template
		Write-Host "`nStep3: Deploying main resources template"  -ForegroundColor Green
		try{
			$parameters = New-Object -TypeName Hashtable

			$parameters["prefix"] = $Prefix
			$parameters["appName"] = $AppName
			$parameters["environment"] = $Suffix

			$parameters["messageIngestionHostFunctionsAppPackageFileName"] = $MessageIngestionHostFunctionsAppPackageFileName
			$parameters["engineHostFunctionsAppPackageFileName"] = $EngineHostFunctionsAppPackageFileName
			$parameters["messagePreProcessingHostFunctionsAppPackageFileName"] = $MessagePreProcessingHostFunctionsAppPackageFileName

			$parameters["functionsAppPackageFolder"] = $FunctionsMsDeployPackageFolderName

			$parameters["eventHubNamespaceName"] = $EventHubNamespaceName
			$parameters["eventHubHubName"] = $EventHubHubName
			$parameters["eventHubThroughputUnits"] = $EventHubThroughputUnits

			$parameters["isDeveloperEnvironment"] = $IsDeveloperEnvironment.IsPresent

			$parameters["aadTenantId"] = $AadTenantId
			$parameters["engineAadClientId"] = $EngineAadClientId
			$parameters["ingestionAadClientId"] = $IngestionAadClientId
			$parameters["preprocessorAadClientId"] = $PreprocessorAadClientId
			$parameters["operationsControlServiceBaseUrl"] = $OperationsControlServiceBaseUrl
			$parameters["operationsControlServiceAadResourceId"] = $OperationsControlServiceAadResourceId

			$TemplateFilePath = [System.IO.Path]::Combine($ArtifactStagingDirectory, "deploy.json")

			$str = $parameters | Out-String
			Write-Host $str

			Write-Host $ArtifactStagingDirectory

			$deploymentResult = .\Deploy-AzureResourceGroup.ps1 `
				-UploadArtifacts `
				-ResourceGroupLocation $ResourceGroupLocation `
				-ResourceGroupName $ResourceGroupName `
				-StorageAccountName $ArtifactStorageAccountName `
				-ArtifactStagingDirectory $ArtifactStagingDirectory `
				-StorageContainerName $ArtifactStorageContainerName `
				-TemplateParameters $parameters `
				-TemplateFile $TemplateFilePath
		}
		catch{
			throw $_
		}
	}

	Write-Host "`nStep 4: Applying configuration"

	Write-Host 'Granting KV secret access to current user'

	.\Scripts\Grant-CurrentAadUserKeyVaultSecretAccess `
		-ResourceGroupName $ResourceGroupName `
		-KeyVaultName $KeyVaultName

	Write-Host 'Adding Event Hub connection string to KV'

	.\Scripts\Add-EventHubConnectionStringToKeyVault `
		-ResourceGroupName $ResourceGroupName `
		-KeyVaultName $KeyVaultName `
		-EventHubNamespace $EventHubNamespaceName `
		-EventHubName $EventHubHubName `
		-AuthorizationRuleName 'Sender' `
		-SecretName 'triggereventhubconnectionstring'

			Write-Host 'Adding Event Hub connection string to KV'

	.\Scripts\Add-CosmosAccessKeyToKeyVault `
		-ResourceGroupName $ResourceGroupName `
		-KeyVaultName $KeyVaultName `
		-CosmosDbName $CosmosDbName `
		-SecretName 'workflowstorecosmosdbkey'

	if ($IsDeveloperEnvironment) {
		Write-Host 'Skipping function app access grants because we are deploying a developer environment'
	} else {
		Write-Host 'Grant the trigger function access to the KV'

		$TriggerFunctionAppName = $Prefix + $AppName.ToLower() + 'ingestion' + $Suffix

		.\Scripts\Grant-KeyVaultSecretGetToMsi `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-AppName $TriggerFunctionAppName

		Write-Host 'Grant the message pre-processing function access to the KV'

		$TriggerFunctionAppName = $Prefix + $AppName.ToLower() + 'preprocess' + $Suffix

		.\Scripts\Grant-KeyVaultSecretGetToMsi `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-AppName $TriggerFunctionAppName

		Write-Host 'Grant the engine host function access to the KV'

		$TriggerFunctionAppName = $Prefix + $AppName.ToLower() + 'engine' + $Suffix

		.\Scripts\Grant-KeyVaultSecretGetToMsi `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-AppName $TriggerFunctionAppName

		Write-Host 'Revoking KV secret access to current user'

		.\Scripts\Revoke-CurrentAadUserKeyVaultSecretAccess `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName
	}

	if ($UpdateLocalConfigFiles) {
		Write-Host 'Updating local.settings.json files'

		.\Scripts\Update-LocalConfigFIles.ps1 `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-CosmosDbName $CosmosDbName `
			-EventHubNamespace $EventHubNamespaceName `
			-EventHubName $EventHubHubName `
			-LeasingStorageAccountName $DefaultName `
			-AppInsightsName $DefaultName
		}
}

End{
	Write-Host -ForegroundColor Green "`n######################################################################`n"
	Write-Host -ForegroundColor Green "Deployment finished"
	Write-Host -ForegroundColor Green "`n######################################################################`n"
}

