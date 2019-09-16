#
# Revoke-CurrentAadUserKeyVaultSecretAccess.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName
)

$ctx = Get-AzureRmContext
$Principal = Get-AzureRmADUser -Mail $ctx.Account.Id

Write-Host 'Revoking access from user' $Principal.DisplayName

Remove-AzureRmKeyVaultAccessPolicy `
    -VaultName $KeyVaultName `
    -ResourceGroupName $ResourceGroupName `
    -ObjectId $Principal.Id