﻿<#
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
    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Workflow.Api.QueryHost.zip",
        $ServiceDeploymentContext.AppName + "qry"
    )

}