<#
This is called during Marain.Instance infrastructure deployment after the Marain-ArmDeploy.ps
script. It is our opportunity to do any deployment work that needs to happen after Azure resources
have been deployed.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    Write-Host "Assigning application service principals to the tenancy service app's reader role"

    $TenancyReaderAppRoleId = "60743a6a-63b6-42e5-a464-a08698a0e9ed"
    $EngineControllerAppRoleId = "37a5c4e2-38e2-47de-8576-6b1ce7cc0ca2"
    $ControllerAppRoleId = "77d9c620-a258-4f0b-945c-a7128e82f3ec"
    $ServiceDeploymentContext.AssignServicePrincipalToCommonServiceAppRole(
        "Marain.Tenancy",
        $TenancyReaderAppRoleId,
        "mi"
    )
    $ServiceDeploymentContext.AssignServicePrincipalToInternalServiceAppRole(
        "eng",
        $EngineControllerAppRoleId,
        "mi"
    )
    $ServiceDeploymentContext.AssignServicePrincipalToCommonServiceAppRole(
        "Marain.Tenancy.Operations.Control",
        $ControllerAppRoleId,
        "mi"
    )

    $ServiceDeploymentContext.AssignServicePrincipalToCommonServiceAppRole(
        "Marain.Tenancy",
        $TenancyReaderAppRoleId,
        "eng"
    )
    
    $ServiceDeploymentContext.AssignServicePrincipalToCommonServiceAppRole(
        "Marain.Tenancy",
        $TenancyReaderAppRoleId,
        "qry"
    )

}