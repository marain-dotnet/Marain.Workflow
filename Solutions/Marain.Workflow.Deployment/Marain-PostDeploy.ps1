<#
This is called during Marain.Instance infrastructure deployment after the Marain-ArmDeploy.ps
script. It is our opportunity to do any deployment work that needs to happen after Azure resources
have been deployed.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    Write-Host 'Uploading function code packages'

    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Workflow.Api.MessageProcessingHost.zip",
        $ServiceDeploymentContext.AppName + "mi"
    )
    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Workflow.Api.EngineHost.zip",
        $ServiceDeploymentContext.AppName + "eng"
    )

    # ensure the service tenancy exists
    $serviceManifest = Join-Path $PSScriptRoot "ServiceManifests\WorkflowServiceManifest.jsonc" -Resolve
    try {
        # TODO: If tenancy wasn't run, we won't currently have the marain cli configuration setup
        $cliOutput = & $ServiceDeploymentContext.InstanceContext.MarainCliPath create-service $serviceManifest
        if ( $LASTEXITCODE -ne 0 -and -not ($cliOutput -imatch 'service tenant.*already exists') ) {
            # TODO: Ignore error when service tenant already exists
            Write-Error "Error whilst trying to register the Workflow service tenant: ExitCode=$LASTEXITCODE`n$cliOutput"
        }
    }
    catch {
        throw $_
    }
}