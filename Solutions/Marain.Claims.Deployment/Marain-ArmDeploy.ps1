<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    # TODO: make this discoverable
    $serviceTenantId = '3633754ac4c9be44b55bfe791b1780f1ca7153e8fbe1b54b9f44002217f1c51c'
    $serviceTenantDisplayName = 'Claims v1'

    [MarainAppService]$TenancyService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy")

    $AppId = $ServiceDeploymentContext.GetAppId()
    $TemplateParameters = @{
        appName="claims"
        functionAuthAadClientId=$AppId
        tenancyServiceResourceIdForMsiAuthentication=$TenancyService.AuthAppId
        tenancyServiceBaseUri=$TenancyService.BaseUrl
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
        marainServiceTenantId=$serviceTenantId
        marainServiceTenantDisplayName=$serviceTenantDisplayName
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("claims")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.functionServicePrincipalId.Value)

    # ensure the service tenancy exists
    Write-Host "Ensuring Claims service tenancy..."
    $serviceManifest = Join-Path $PSScriptRoot "ServiceManifests\ClaimsServiceManifest.jsonc" -Resolve
    try {
        $cliOutput = & $ServiceDeploymentContext.InstanceContext.MarainCliPath create-service $serviceManifest
        if ( $LASTEXITCODE -ne 0 -and -not ($cliOutput -imatch 'service tenant.*already exists') ) {
            Write-Error "Error whilst trying to register the Claims service tenant: ExitCode=$LASTEXITCODE`n$cliOutput"
        }
    }
    catch {
        throw $_
    }
}