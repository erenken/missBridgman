param(
    [string] $ResourceGroupName = 'missBridgman',
    [string] $WebAppName = 'missbridgman-web-2026',
    [string] $GitHubOwner = 'erenken',
    [string] $GitHubRepository = 'missBridgman',
    [string] $Location = 'centralus',
    [string] $GitHubBranch = 'main',
    [string] $AppServiceSku = 'B1',
    [string] $DnsZoneName = 'missbridgman.com'
)

$ErrorActionPreference = 'Stop'

az deployment group create `
    --resource-group $ResourceGroupName `
    --name miss-bridgman-web `
    --template-file (Join-Path $PSScriptRoot 'main.bicep') `
    --parameters "webAppName=$WebAppName" "githubOwner=$GitHubOwner" "githubRepository=$GitHubRepository" "githubBranch=$GitHubBranch" "location=$Location" "appServiceSku=$AppServiceSku" "dnsZoneName=$DnsZoneName" `
    --output jsonc
if ($LASTEXITCODE -ne 0) { throw 'Bicep deployment failed.' }
