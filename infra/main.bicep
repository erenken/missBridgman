targetScope = 'resourceGroup'

@description('Globally unique name for the Linux Web App. Use lowercase letters, numbers, and hyphens.')
@minLength(2)
@maxLength(60)
param webAppName string

@description('GitHub account or organization that owns the repository.')
param githubOwner string

@description('GitHub repository name, without the owner.')
param githubRepository string

@description('Branch allowed to deploy through GitHub OIDC.')
param githubBranch string = 'main'

@description('Existing Azure DNS zone to allow the GitHub deployment identity to update.')
param dnsZoneName string = 'missbridgman.com'

@description('Existing Application Insights component receiving browser and server telemetry.')
param applicationInsightsName string = 'missBridgmanApi'

@description('Azure region for App Service and the deployment identity.')
param location string = resourceGroup().location

@description('App Service plan SKU. B1 supports Always On and WebSockets for the Interactive Server app.')
@allowed([
  'B1'
  'B2'
  'B3'
  'P0v3'
  'P1v3'
])
param appServiceSku string = 'B1'

var planName = '${take(webAppName, 20)}-plan-${uniqueString(webAppName)}'
var deployIdentityName = '${webAppName}-github-deploy'
var roleIds = {
  websiteContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'de139f84-1756-47ae-9be6-808fbbe84772')
  dnsZoneContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'befefa01-2a29-4197-83a8-272ff33ce314')
}

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' existing = {
  name: dnsZoneName
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: planName
  location: location
  kind: 'linux'
  sku: {
    name: appServiceSku
    tier: contains(appServiceSku, 'P') ? 'PremiumV3' : 'Basic'
    size: appServiceSku
    family: contains(appServiceSku, 'P') ? 'P' : 'B'
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource deployIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: deployIdentityName
  location: location
}

resource githubFederation 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2023-01-31' = {
  parent: deployIdentity
  name: 'github-deploy'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:${githubOwner}/${githubRepository}:ref:refs/heads/${githubBranch}'
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: true
    siteConfig: {
      linuxFxVersion: 'dotnet|11'
      alwaysOn: true
      http20Enabled: true
      webSocketsEnabled: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      healthCheckPath: '/healthz'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ASPNETCORE_HTTP_PORTS'
          value: '8080'
        }
        {
          name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
          value: 'true'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'OTEL_SERVICE_NAME'
          value: 'MissBridgman.Web'
        }
      ]
    }
  }
}

resource githubWebAppDeploy 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webApp.id, deployIdentity.id, roleIds.websiteContributor)
  scope: webApp
  properties: {
    roleDefinitionId: roleIds.websiteContributor
    principalId: deployIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource githubDnsDeploy 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(dnsZone.id, deployIdentity.id, roleIds.dnsZoneContributor)
  scope: dnsZone
  properties: {
    roleDefinitionId: roleIds.dnsZoneContributor
    principalId: deployIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output githubClientId string = deployIdentity.properties.clientId
output githubTenantId string = tenant().tenantId
output githubSubscriptionId string = subscription().subscriptionId
output customDomain string = dnsZoneName
