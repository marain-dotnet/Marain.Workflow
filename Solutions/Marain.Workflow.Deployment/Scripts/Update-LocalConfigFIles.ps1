#
# Update-LocalConfigFiles.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName,
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName,
	[Parameter(Mandatory=$true)]
	[string] $CosmosDbName,
	[Parameter(Mandatory=$true)]
	[string] $EventHubNamespace,
	[Parameter(Mandatory=$true)]
    [string] $EventHubName,
	[Parameter(Mandatory=$true)]
    [string] $LeasingStorageAccountName,
	[Parameter(Mandatory=$true)]
    [string] $AppInsightsName

)

$CosmosKeys = Invoke-AzureRmResourceAction `
	-Action listKeys `
	-ResourceType "Microsoft.DocumentDb/databaseAccounts" `
	-ApiVersion "2015-04-08" `
	-Name $CosmosDbName `
	-ResourceGroupName $ResourceGroupName `
	-Force

$EventHubSenderKeys = Get-AzureRmEventHubKey `
			-ResourceGroupName $ResourceGroupName `
			-NamespaceName $EventHubNamespace `
			-EventHubName $EventHubName `
			-AuthorizationRuleName "Sender"

$EventHubListenerKeys = Get-AzureRmEventHubKey `
            -ResourceGroupName $ResourceGroupName `
            -NamespaceName $EventHubNamespace `
            -EventHubName $EventHubName `
            -AuthorizationRuleName "Listener"

$StorageAccountKeys = Get-AzureRmStorageAccountKey `
            -ResourceGroupName $ResourceGroupName `
            -Name $LeasingStorageAccountName

$LeasingStorageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=' + $LeasingStorageAccountName + ';AccountKey=' + $StorageAccountKeys[0].Value + ';EndpointSuffix=core.windows.net'

$AppInsightsInstance = Get-AzureRmApplicationInsights `
            -ResourceGroupName $ResourceGroupName `
			-Name $AppInsightsName

$engineHostJson = Get-Content '..\Endjin.Workflow.Functions.EngineHost\local.settings.template.json' -raw | ConvertFrom-Json
$engineHostJson.Values.KeyVaultName = $KeyVaultName
$engineHostJson.Values.CosmosDbAccountUri = "https://" + $CosmosDbName + ".documents.azure.com:443/"
$engineHostJson.Values.LeasingStorageAccountConnectionString = $LeasingStorageAccountConnectionString
$engineHostJson.Values.APPINSIGHTS_INSTRUMENTATIONKEY = $AppInsightsInstance.InstrumentationKey
$engineHostJson.kv.workflowstorecosmosdbkey = $CosmosKeys.primaryMasterKey
$engineHostJson | ConvertTo-Json  | Set-Content '..\Endjin.Workflow.Functions.EngineHost\local.settings.json'

$messageIngestionHostJson = Get-Content '..\Endjin.Workflow.Functions.MessageIngestionHost\local.settings.template.json' -raw | ConvertFrom-Json
$messageIngestionHostJson.Values.KeyVaultName = $KeyVaultName
$messageIngestionHostJson.Values.APPINSIGHTS_INSTRUMENTATIONKEY = $AppInsightsInstance.InstrumentationKey
$messageIngestionHostJson.kv.triggereventhubconnectionstring = $EventHubSenderKeys.PrimaryConnectionString
$messageIngestionHostJson | ConvertTo-Json  | Set-Content '..\Endjin.Workflow.Functions.MessageIngestionHost\local.settings.json'

$messagePreProcessingHostJson = Get-Content '..\Endjin.Workflow.Functions.MessagePreProcessingHost\local.settings.template.json' -raw | ConvertFrom-Json
$messagePreProcessingHostJson.Values.KeyVaultName = $KeyVaultName
$messagePreProcessingHostJson.Values.CosmosDbAccountUri = "https://" + $CosmosDbName + ".documents.azure.com:443/"
$messagePreProcessingHostJson.Values.EventHubConnectionString = $EventHubListenerKeys.PrimaryConnectionString
$messagePreProcessingHostJson.Values.LeasingStorageAccountConnectionString = $LeasingStorageAccountConnectionString
$messagePreProcessingHostJson.Values.APPINSIGHTS_INSTRUMENTATIONKEY = $AppInsightsInstance.InstrumentationKey
$messagePreProcessingHostJson.kv.workflowstorecosmosdbkey = $CosmosKeys.primaryMasterKey
$messagePreProcessingHostJson | ConvertTo-Json  | Set-Content '..\Endjin.Workflow.Functions.MessagePreProcessingHost\local.settings.json'

$specFlowJson = Get-Content '..\Endjin.Workflow.Functions.Specs\local.settings.template.json' -raw | ConvertFrom-Json
$specFlowJson.EventHubListenerConnectionString = $EventHubListenerKeys.PrimaryConnectionString
$specFlowJson.EventHubStorageAccountConnectionString = $LeasingStorageAccountConnectionString
$specFlowJson.LeasingStorageAccountConnectionString = $LeasingStorageAccountConnectionString
$specFlowJson.KeyVaultName = $KeyVaultName
$specFlowJson.CosmosDbAccountUri = "https://" + $CosmosDbName + ".documents.azure.com:443/"
$specFlowJson.'kv:workflowstorecosmosdbkey' = $CosmosKeys.primaryMasterKey
$specFlowJson | ConvertTo-Json  | Set-Content '..\Endjin.Workflow.Functions.Specs\local.settings.json'
