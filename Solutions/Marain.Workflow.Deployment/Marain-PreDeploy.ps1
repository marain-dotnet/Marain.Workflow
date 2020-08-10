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
    $QueryApp = $ServiceDeploymentContext.DefineAzureAdAppForAppService("qry")

    $EngineControllerAppRoleId = "37a5c4e2-38e2-47de-8576-6b1ce7cc0ca2"
    $EngineApp.EnsureAppRolesContain(
        $EngineControllerAppRoleId,
        "Workflow Engine controller",
        "Able to create workflow instances, and send triggers",
        "WorkflowController",
        ("User", "Application"))
}