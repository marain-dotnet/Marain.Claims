#
# Update-LocalConfigFiles.ps1
#

Param(
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroupName,
	[Parameter(Mandatory=$true)]
	[string] $KeyVaultName,
	[Parameter(Mandatory=$true)]
	[string] $CosmosDbName,
	[Parameter(Mandatory=$true)]
    [string] $AppInsightsName
)

$CosmosKeys = Invoke-AzureRmResourceAction `
	-Action listKeys `
	-ResourceType "Microsoft.DocumentDb/databaseAccounts" `
	-ApiVersion "2015-04-08" `
	-Name $CosmosDbName `
	-ResourceGroupName $ResourceGroupName `
	-Force

$AppInsightsInstance = Get-AzureRmApplicationInsights `
            -ResourceGroupName $ResourceGroupName `
			-Name $AppInsightsName

$hostJson = Get-Content '..\Endjin.Claims.Functions.Host\local.settings.template.json' -raw | ConvertFrom-Json
$hostJson.Values.KeyVaultName = $KeyVaultName
$hostJson.Values.CosmosDbAccountUri = "https://" + $CosmosDbName + ".documents.azure.com:443/"
$hostJson.Values.APPINSIGHTS_INSTRUMENTATIONKEY = $AppInsightsInstance.InstrumentationKey
$hostJson.kv.claimsstorecosmosdbkey = $CosmosKeys.primaryMasterKey
$hostJson | ConvertTo-Json  | Set-Content '..\Endjin.Claims.Functions.Host\local.settings.json'
