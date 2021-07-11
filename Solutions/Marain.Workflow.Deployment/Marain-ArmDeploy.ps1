<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    # TODO: make this discoverable
    $serviceTenantId = '3633754ac4c9be44b55bfe791b1780f177b464860334774cabb2f9d1b95b0c18'
    $serviceTenantDisplayName = 'Workflow v1'

    [MarainAppService]$TenancyService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy")
    [MarainAppService]$OperationsService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy.Operations.Control")

    $EngineAppId = $ServiceDeploymentContext.GetAppId("eng")
    $QueryAppId = $ServiceDeploymentContext.GetAppId("qry")
    $MessageIngestionAppId = $ServiceDeploymentContext.GetAppId("mi")
    $TemplateParameters = @{
        appName="workflow"
        engineFunctionAuthAadClientId=$EngineAppId
        queryFunctionAuthAadClientId=$QueryAppId
        messageIngestionFunctionAuthAadClientId=$MessageIngestionAppId
        operationsControlServiceBaseUrl=$OperationsService.BaseUrl
        operationsControlResourceIdForMsiAuthentication=$OperationsService.AuthAppId
        tenancyServiceResourceIdForMsiAuthentication=$TenancyService.AuthAppId
        tenancyServiceBaseUri=$TenancyService.BaseUrl
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
        marainServiceTenantId=$serviceTenantId
        marainServiceTenantDisplayName=$serviceTenantDisplayName
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("workflow")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.messageIngestionFunctionServicePrincipalId.Value, "mi", $null)
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.engineFunctionServicePrincipalId.Value, "eng", $null)
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.queryFunctionServicePrincipalId.Value, "qry", $null)


    # ensure the service tenancy exists
    Write-Host "Ensuring Workflow service tenancy..."
    $serviceManifest = Join-Path $PSScriptRoot "ServiceManifests\WorkflowServiceManifest.jsonc" -Resolve
    try {
        $cliOutput = & $ServiceDeploymentContext.InstanceContext.MarainCliPath create-service $serviceManifest
        if ( $LASTEXITCODE -ne 0 -and -not ($cliOutput -imatch 'service tenant.*already exists') ) {
            Write-Error "Error whilst trying to register the Workflow service tenant: ExitCode=$LASTEXITCODE`n$cliOutput"
        }
    }
    catch {
        throw $_
    }
}