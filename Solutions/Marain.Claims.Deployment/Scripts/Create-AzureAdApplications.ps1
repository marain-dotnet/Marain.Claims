<#

.EXAMPLE

.\Create-AzureAdApplications.ps1 `
    -TenantId "0f621c67-98a0-4ed5-b5bd-31a35be41e29"

.EXAMPLE

.\Create-AzureAdApplications.ps1 `
    -TenantId "a4e4416d-72dc-4a59-b15b-895d81af6de2" `
    -ClientName "aul" `
    -AppCoreName "cl" `
	-EnvironmentName "dev" `
    -AadAppReplyUrls ("https://foo.example.com/*", "https://bar.example.com/.auth/callback") `
	-Suffix "AAD"
#>
param(
	[Parameter(Mandatory=$true)]
	[string] $TenantId,

    [string] $ClientName = "end",

    [string] $AppCoreName = "claims",

	[string] $EnvironmentName = "dev",

    [string[]] $AadAppReplyUrls,

    [Guid] $ClaimsAdminRoleId = "7619c293-764c-437b-9a8e-698a26250efd",

    [Guid] $ClientRoleId = "be84cc43-d5fb-4e37-91f2-1d46fc353c69",

	[string] $Suffix = "AAD"
)

Begin {

	function ConfigureAdApplication($adAppName, $replyUrls, $defaultDomain, $appRoles)
	{
		$uri = "http://$defaultDomain/$adAppName"

		# Create web AD application

		Write-Host "Looking for existing application for web app..."
		$app = Get-AzureADApplication -Filter "DisplayName eq '$adAppName'"

		if ($app) {
			Write-Host "Application for web app already exists. Updating..."
			Set-AzureADApplication -ObjectId $app.ObjectId -IdentifierUris $uri -ReplyUrls $replyUrls -Oauth2AllowImplicitFlow $true -Homepage $uri -AppRoles $appRoles -GroupMembershipClaims "All"
		}
		else {
			Write-Host "Creating new application for web app..."
			$newApp = New-AzureADApplication -DisplayName $adAppName -IdentifierUris $uri -ReplyUrls $replyUrls -Oauth2AllowImplicitFlow $true -Homepage $uri -AppRoles $appRoles -GroupMembershipClaims "All"

            # Azure AD takes a while to propagate a new App around its storage systems,
            # so we need to give it a few seconds to avoid errors that will otherwise
            # occasionally occur when we try to add the service principal for this app.
            Start-Sleep 15
		}

		$app = Get-AzureADApplication -Filter "DisplayName eq '$adAppName'"

		$appId = $app.AppID

        $sp = Get-AzureADServicePrincipal -Filter "AppId eq '$appId'"
		if ($sp){
			Write-Host "Service principal for web app already exists. Updating..."
			Set-AzureADServicePrincipal -ObjectId $sp.ObjectId -AppRoleAssignmentRequired $true
		}
		else {
			Write-Host "Creating new service principal for web app..."
			$newSp = New-AzureADServicePrincipal -AppId $appId -AppRoleAssignmentRequired $true
		}

		Write-Host "Successfully set application and service principal."

		return $appId
	}

	function CreateAppRole([string] $Name, [string] $Description, [Guid] $Id, [string[]] $AllowedMemberTypes)
	{
		$appRole = New-Object Microsoft.Open.AzureAD.Model.AppRole
		$appRole.AllowedMemberTypes = New-Object System.Collections.Generic.List[string]
        Foreach($AllowedMemberType in $AllowedMemberTypes)
        {
		    $appRole.AllowedMemberTypes.Add($AllowedMemberType);
        }
		$appRole.DisplayName = $Name
		$appRole.Id = $Id
		$appRole.IsEnabled = $true
		$appRole.Description = $Description
		$appRole.Value = $Name;

		return $appRole
	}

	function CreateMappings($DefaultDomain, $ReplyUrls)
	{
		$claimsAppName = $ClientName + $AppName + $EnvironmentName + $Suffix

		$claimsAppRoles = New-Object System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.AppRole]
		$claimsAppRoles.Add((CreateAppRole -AllowedMemberType ("Application", "User") -Name "ClaimsAdministrator" -Description "Can define, edit, and delete all settings in the Claims service" -Id $ClaimsAdminRoleId))
		$claimsAppRoles.Add((CreateAppRole -AllowedMemberType ("Application", "User") -Name "Client" -Description "CanCan evaluate claim permissions in the Claims service" -Id $ClientRoleId))

		$mappings = @(
			@{AdAppName= $claimsAppName; ReplyUrls=$ReplyUrls; AppRoles= $claimsAppRoles; DefaultDomain =$DefaultDomain}
		)
		return $mappings
	}

}

Process {

	$ErrorActionPreference = 'Stop'

	if (!(Get-Module -ListAvailable AzureAD)) {
		Write-Host "Installing/updating AzureAD module..."
		Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force -Scope CurrentUser
		Install-Module AzureAD -Scope CurrentUser -Force
	}

	Write-Host "Connecting to Azure AD..."
	Write-Host
	Connect-AzureAD -TenantId $TenantId

	$tenantDetail = Get-AzureADTenantDetail
	$defaultDomain = $tenantDetail.VerifiedDomains[0].Name

    if ((-not $AadAppReplyUrls) -or ($AadAppReplyUrls.Count -eq 0))
    {
        $defaultReplyUrl = "https://" + $ClientName + $AppName + $EnvironmentName + ".azurewebsites.net/*"
    }

	$results = @{}
	
	$mappings = CreateMappings($defaultDomain, $AadAppReplyUrls)

	foreach($entry in $mappings){

		Write-Host "Creating AD application for" $entry.AdAppName
		$AppId = ConfigureAdApplication @entry

		$results.Add($entry.AdAppName, $AppId)
	}

	# Write out application IDs
	Write-Host
	Write-Host "Applications IDs:"
	Write-Host ($results | Out-String)
}