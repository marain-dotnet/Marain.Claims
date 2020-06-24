<#
This is called during Marain.Instance infrastructure deployment prior to the Marain-ArmDeploy.ps
script. It is our opportunity to perform initialization that needs to complete before any Azure
resources are created.

We create the Azure AD Applications that the Claims functions will use to authenticate incoming
requests. (Currently, this application is used with Azure Easy Auth, but the service could also
use it directly.)

#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    $App = $ServiceDeploymentContext.DefineAzureAdAppForAppService()

    $AdminAppRoleId = "7619c293-764c-437b-9a8e-698a26250efd"
    $ClientAppRoleId = "be84cc43-d5fb-4e37-91f2-1d46fc353c69"

    $App.EnsureAppRolesContain(
        $AdminAppRoleId,
        "Claims administrator",
        "Full control over definition of claim permissions and rule sets",
        "ClaimsAdministrator",
        ("User", "Application"))

    $App.EnsureAppRolesContain(
        $ClientAppRoleId,
        "Client",
        "Can evaluate claim permissions in the Claims service",
        "Client",
        ("User", "Application"))
}