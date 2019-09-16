#
# Grant-CurrentAadUserKeyVaultSecretAccess.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName
)

$ctx = Get-AzureRmContext
$Principal = Get-AzureRmADUser -Mail $ctx.Account.Id

Write-Host 'Granting access to user' $Principal.DisplayName

Set-AzureRmKeyVaultAccessPolicy `
    -VaultName $KeyVaultName `
    -ResourceGroupName $ResourceGroupName `
    -ObjectId $Principal.Id `
    -PermissionsToSecrets List,Set,Get
