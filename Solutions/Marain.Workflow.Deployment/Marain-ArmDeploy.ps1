<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    [MarainAppService]$OperationsService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Operations")

    # This is temporary - once https://github.com/marain-dotnet/Marain.Workflow/issues/50 is done, these
    # hard-coded values will no longer be needed
    $TenancyStorageAccountName = "mardevtenancyyn7robbeb3r"
    $TenancyKeyVaultName = "mardevtenancyyn7robbeb3r"
    $TenancyStorageAccountKeyName = "TenancyStorageAccountKey"

    $EngineAppId = $ServiceDeploymentContext.GetAppId("eng")
    $MessageIngestionAppId = $ServiceDeploymentContext.GetAppId("mi")
    $TemplateParameters = @{
        appName="workflow"
        engineFunctionAuthAadClientId=$EngineAppId
        messageIngestionFunctionAuthAadClientId=$MessageIngestionAppId
        operationsControlServiceBaseUrl=$OperationsService.BaseUrl
        operationsControlResourceIdForMsiAuthentication=$OperationsService.AuthAppId
        tenancyStorageAccountName=$TenancyStorageAccountName
        tenancyKeyVaultName=$TenancyKeyVaultName
        tenancyStorageAccountKeyName=$TenancyStorageAccountKeyName
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("workflow")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.messageIngestionFunctionServicePrincipalId.Value, "mi", $null)
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.engineFunctionServicePrincipalId.Value, "eng", $null)
}