<#
This is called during Marain.Instance infrastructure deployment prior to the Marain-ArmDeploy.ps
script. It is our opportunity to perform initialization that needs to complete before any Azure
resources are created.

We create the Azure AD Applications that the Workflow functions will use to authenticate incoming
requests. (Currently, this application is used with Azure Easy Auth, but the service could also
use it directly.)

#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    $EngineApp = $ServiceDeploymentContext.DefineAzureAdAppForAppService("eng")
    $MessageIngestionApp = $ServiceDeploymentContext.DefineAzureAdAppForAppService("mi")

    $EngineControllerAppRoleId = "37a5c4e2-38e2-47de-8576-6b1ce7cc0ca2"
    $EngineApp.EnsureAppRolesContain(
        $EngineControllerAppRoleId,
        "Workflow Engine controller",
        "Able to create workflow instances, and send triggers",
        "WorkflowController",
        ("User", "Application"))

    # ensure the service tenancy exists
    $serviceManifest = Join-Path $PSScriptRoot "..\ServiceManifests\WorkflowServiceManifest.jsonc" -Resolve
    try {
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