#
# Grant-KeyVaultSecretGetToMsi.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName, 
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName, 
	[Parameter(Mandatory=$true)]
	[string] $AppName
)

$Principal = Get-AzureRmADServicePrincipal -SearchString $AppName | ?{$_.DisplayName -eq $AppName}

Set-AzureRmKeyVaultAccessPolicy `
    -VaultName $KeyVaultName `
    -ResourceGroupName $ResourceGroupName `
    -ObjectId $Principal.Id `
    -PermissionsToSecrets Get
