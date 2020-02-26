<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    [MarainAppService]$TenancyService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy")
    [MarainAppService]$OperationsService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy.Operations.Control")

    $EngineAppId = $ServiceDeploymentContext.GetAppId("eng")
    $MessageIngestionAppId = $ServiceDeploymentContext.GetAppId("mi")
    $TemplateParameters = @{
        appName="workflow"
        engineFunctionAuthAadClientId=$EngineAppId
        messageIngestionFunctionAuthAadClientId=$MessageIngestionAppId
        operationsControlServiceBaseUrl=$OperationsService.BaseUrl
        operationsControlResourceIdForMsiAuthentication=$OperationsService.AuthAppId
        tenancyServiceResourceIdForMsiAuthentication=$TenancyService.AuthAppId
        tenancyServiceBaseUri=$TenancyService.BaseUrl
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