<#

.EXAMPLE

.\deploy.ps1 `
	-Prefix "mar" `
	-AppName "claims" `
	-Environment "dev" `
	-FunctionsMsDeployPackagePath "..\Marain.Claims.Host.Functions\bin\Release\package\Marain.Claims.Host.Functions.zip"
#>

[CmdletBinding(DefaultParametersetName='None')] 
param(
    [string] $Prefix = "mar",
	[string] $AppName = "claims",
	[ValidateLength(3,12)]
	[string] $Suffix = "dev",
	[string] $FunctionsMsDeployPackagePath = "..\Marain.Claims.Host.Functions\bin\Release\package\Marain.Claims.Host.Functions.zip",	
	[string] $ResourceGroupLocation = "northeurope",
	[string] $ArtifactStagingDirectory = ".",
	[string] $ArtifactStorageContainerName = "stageartifacts",
	[switch] $IsDeveloperEnvironment,
	[switch] $UpdateLocalConfigFiles,
	[switch] $SkipDeployment
)

Begin{
	# Setup options and variables
	$ErrorActionPreference = 'Stop'
	Set-Location $PSScriptRoot

	$Suffix = $Suffix.ToLower()
	$AppName  = $AppName.ToLower()
	$Prefix = $Prefix.ToLower()

	$ResourceGroupName = $Prefix + "." + $AppName.ToLower() + "." + $Suffix
	$DefaultName = $Prefix + $AppName.ToLower() + $Suffix

	$ArtifactStorageResourceGroupName = $ResourceGroupName + ".artifacts";
	$ArtifactStorageAccountName = $Prefix + $AppName + $Suffix + "ar"
	$ArtifactStagingDirectory = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory))

	$FunctionsMsDeployPackageFolderName = "MsDeploy";

	$FunctionsAppPackageFileName = [System.IO.Path]::GetFileName($FunctionsMsDeployPackagePath)

	$CosmosDbName = $DefaultName
	$KeyVaultName = $DefaultName
}

Process{
	if ($SkipDeployment) {
		Write-Host "`nSkipping deployment steps due to SkipDeployment parameter being present"
	} else {
		# Create resource group and artifact storage account
		Write-Host "`nStep1: Creating resource group $ResourceGroupName and artifact storage account $ArtifactStorageAccountName" -ForegroundColor Green
		try {
			.\Scripts\Create-StorageAccount.ps1 `
				-ResourceGroupName $ArtifactStorageResourceGroupName `
				-ResourceGroupLocation $ResourceGroupLocation `
				-StorageAccountName $ArtifactStorageAccountName
		}
		catch{
			throw $_
		}

		# Copy msbuild package to artifact directory
		if ($IsDeveloperEnvironment) {
			Write-Host "`nStep2: Skipping function msdeploy package copy as we are deploying a developer environment only"  -ForegroundColor Green
		} else {
			Write-Host "`nStep2: Coping functions msdeploy packages to artifact directory $FunctionsMsDeployPackageFolderName"  -ForegroundColor Green
			try {
				Copy-Item -Path $FunctionsMsDeployPackagePath -Destination (New-Item -Type directory -Force "$FunctionsMsDeployPackageFolderName") -Force -Recurse
			}
			catch{
				throw $_
			}
		}

		Write-Host "`nStep3: Ensuring Azure AD application exists"  -ForegroundColor Green
        $EasyAuthAppRm = Get-AzureRmADApplication -DisplayNameStartWith $DefaultName | ?{$_.DisplayName -eq $DefaultName}
        if ($EasyAuthAppRm) {
            Write-Host "Found existing app with id $($EasyAuthAppRm.ApplicationId)"
        } else {
            $AppUri = "https://" + $DefaultName + ".azurewebsites.net"
            $ReplyUrl = $AppUri + "/.auth/login/aad/callback"
            $EasyAuthAppRm = New-AzureRmADApplication -DisplayName $DefaultName -IdentifierUris $AppUri -HomePage $AppUri -ReplyUrls $ReplyUrl
            Write-Host "Created new app with id $($EasyAuthAppRm.ApplicationId)"
        }
        # Check permissions. This requires the use of the AzureAD module because only that gives us
        # access to the permissions. We need to enable Graph Read because otherwise you can't use
        # this App to sign people in with Easy Auth. (Irritatingly, although the Azure Portal sets
        # this up correctly when it adds an app for you, the New-AzureRmADApplication cmdlet does not.
        # See https://blogs.msdn.microsoft.com/azuregov/2017/12/06/web-app-easy-auth-configuration-using-powershell/ )
        # If this fails ensure you run:
        #   Connect-AzureAD
        # and if that is unavailable, do this first:
        #   Install-Module AzureAD
        $EasyAuthAppAd = Get-AzureADApplication -ObjectId $EasyAuthAppRm.ObjectId
        $GraphApiAppId = "00000002-0000-0000-c000-000000000000"
        $SignInAndReadProfileScopeId = "311a71cc-e848-46a1-bdf8-97ff7156d8e6"
        $SignInAndReadProfileRequiredAccess = $EasyAuthAppAd.RequiredResourceAccess | ?{$_.resourceAppID -eq $GraphApiAppId -and $_.resourceAccess.id -eq $SignInAndReadProfileScopeId -and $_.resourceAccess.type -eq 'Scope'}
        if (-not $SignInAndReadProfileRequiredAccess) {
            Write-Host "Adding 'Sign in and read user profile' required resource access to Easy Auth AD Application"
            $SignInAndReadProfileRequiredAccess = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
            $SignInAndReadProfileRequiredAccess.ResourceAppId = $GraphApiAppId
            $SignInPermission = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList $SignInAndReadProfileScopeId,"Scope"
            $SignInAndReadProfileRequiredAccess.ResourceAccess = $SignInPermission

            $UpdatedRequirePermissions = $EasyAuthAppAd.RequiredResourceAccess
            $UpdatedRequirePermissions.Add($SignInAndReadProfileRequiredAccess)
            Set-AzureADApplication -ObjectId $EasyAuthAppRm.ObjectId -RequiredResourceAccess $UpdatedRequirePermissions
        }

        $AdminAppRoleId = "7619c293-764c-437b-9a8e-698a26250efd"
        $AdminAppRole = $EasyAuthAppAd.AppRoles | ?{$_.Id -eq $AdminAppRoleId}
        if (-not $AdminAppRole) {
            Write-Host "Adding Claims admin app role"
            $AppRole = New-Object Microsoft.Open.AzureAD.Model.AppRole
            $AppRole.AllowedMemberTypes = New-Object System.Collections.Generic.List[string]
            $AppRole.AllowedMemberTypes.Add("User");
            $AppRole.AllowedMemberTypes.Add("Application");
            $AppRole.DisplayName = "Claims administrator"
            $AppRole.Id = $AdminAppRoleId
            $AppRole.IsEnabled = $true
            $AppRole.Description = "Full control over definition of claim permissions and rule sets"
            $AppRole.Value = "ClaimsAdministrator"

            $UpdatedRoles = $EasyAuthAppAd.AppRoles
            $UpdatedRoles.Add($AppRole)
            Set-AzureADApplication -ObjectId $EasyAuthAppRm.ObjectId -AppRoles $UpdatedRoles
        }

		# Deploy main ARM template
		Write-Host "`nStep4: Deploying main resources template"  -ForegroundColor Green
		try{
			$parameters = New-Object -TypeName Hashtable

			$parameters["prefix"] = $Prefix
			$parameters["appName"] = $AppName
			$parameters["environment"] = $Suffix

			$parameters["functionEasyAuthAadClientId"] = $EasyAuthAppAd.AppId

			$parameters["functionsAppPackageFileName"] = $FunctionsAppPackageFileName

			$parameters["functionsAppPackageFolder"] = $FunctionsMsDeployPackageFolderName

			$parameters["isDeveloperEnvironment"] = $IsDeveloperEnvironment.IsPresent

			$TemplateFilePath = [System.IO.Path]::Combine($ArtifactStagingDirectory, "deploy.json")

			$str = $parameters | Out-String
			Write-Host $str

			Write-Host $ArtifactStagingDirectory

			$deploymentResult = .\Deploy-AzureResourceGroup.ps1 `
				-UploadArtifacts `
				-ResourceGroupLocation $ResourceGroupLocation `
				-ResourceGroupName $ResourceGroupName `
				-StorageAccountName $ArtifactStorageAccountName `
				-ArtifactStagingDirectory $ArtifactStagingDirectory `
				-StorageContainerName $ArtifactStorageContainerName `
				-TemplateParameters $parameters `
				-TemplateFile $TemplateFilePath
		}
		catch{
			throw $_
		}
	}

	Write-Host "`nStep 5: Applying configuration"

	Write-Host 'Granting KV secret access to current user'

	.\Scripts\Grant-CurrentAadUserKeyVaultSecretAccess `
		-ResourceGroupName $ResourceGroupName `
		-KeyVaultName $KeyVaultName

	.\Scripts\Add-CosmosAccessKeyToKeyVault `
		-ResourceGroupName $ResourceGroupName `
		-KeyVaultName $KeyVaultName `
		-CosmosDbName $CosmosDbName `
		-SecretName 'claimsstorecosmosdbkey'

	if ($IsDeveloperEnvironment) {
		Write-Host 'Skipping function app access grants because we are deploying a developer environment'
	} else {
		Write-Host 'Grant the function access to the KV'

		$FunctionAppName = $Prefix + $AppName.ToLower() + $Suffix

		.\Scripts\Grant-KeyVaultSecretGetToMsi `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-AppName $FunctionAppName

		Write-Host 'Revoking KV secret access to current user'

		.\Scripts\Revoke-CurrentAadUserKeyVaultSecretAccess `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName
	}

	if ($UpdateLocalConfigFiles) {
		Write-Host 'Updating local.settings.json files'

		.\Scripts\Update-LocalConfigFiles.ps1 `
			-ResourceGroupName $ResourceGroupName `
			-KeyVaultName $KeyVaultName `
			-CosmosDbName $CosmosDbName `
			-AppInsightsName $DefaultName
		}
}

End{
	Write-Host -ForegroundColor Green "`n######################################################################`n"
	Write-Host -ForegroundColor Green "Deployment finished"
	Write-Host -ForegroundColor Green "`n######################################################################`n"
}

