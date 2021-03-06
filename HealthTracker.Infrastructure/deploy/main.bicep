@description('The location that our resources will be deployed to. Default is location of resource group')
param location string = resourceGroup().location

@description('Name of the App Service Plan')
param appServicePlanName string

@description('Name of the Cosmos DB account that will be deployed')
param cosmosDBAccountName string

@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('The name of the Key Vault that will be deployed')
param keyVaultName string

@description('The time that the resource was last deployed')
param lastDeployed string = utcNow()

@description('The name of the log analytics workspace that will be deployed')
param logAnalyticsWorkspaceName string

@description('The name of the Service Bus Namespace that will be deployed')
param serviceBusNamespaceName string

@description('The name of the APIM instance that will be deployed')
param apimName string

@description('The Publisher Email for APIM')
param apimPublisherEmail string

@description('The name of the Publisher for APIM')
param apimPublisherName string

var cosmosDBName = 'MyHealthTrackerDB'
var cosmosContainerName = 'Records'
var tags = {
  'ApplicationName': 'HealthTracker'
  'Component': 'CommonInfrastructure'
  'Environment': 'Production'
  'LastDeployed': lastDeployed
}
var retentionPolicy = {
  days: 30
  enabled: true 
}
var apimSkuName = 'Developer'

resource appServicePlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: appServicePlanName
  tags: tags
  location: location
  kind: 'functionapp'
  sku: {
    name: 'Y1'
  } 
  properties: {
    
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  tags: tags
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logAnalytics.id 
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-11-15-preview' = {
  name: cosmosDBAccountName
  location: location
  tags: tags
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-11-15-preview' = {
  name: cosmosDBName
  parent: cosmosAccount
  properties: {
    resource: {
      id: cosmosDBName
    }
  }
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-11-15-preview' = {
  name: cosmosContainerName
  parent: cosmosDB  
  properties: {
    resource: {
      id: cosmosContainerName
      partitionKey: {
        paths: [
          '/DocumentType'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
      }
    }
    options: {
      autoscaleSettings: {
        maxThroughput: 4000
      }
    }
  }
}

resource cosmosDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'cosmosDiagnosticSettings'
  scope: cosmosAccount
  properties: {
    logs: [
      {
        category: 'DataPlaneRequests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'QueryRuntimeStatistics'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'PartitionKeyStatistics'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'PartitionKeyRUConsumption'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'ControlPlaneRequests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
    ]
    metrics: [
      {
        category: 'Requests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
    ]
    workspaceId: logAnalytics.id
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName
  tags: tags
  location: location 
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enabledForTemplateDeployment: true
    accessPolicies: [
      
    ]
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusNamespaceName
  tags: tags
  location: location 
  sku: {
    name: 'Basic'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource apim 'Microsoft.ApiManagement/service@2021-12-01-preview' = {
  name: apimName
  location: location 
  sku: {
    capacity: 1
    name: apimSkuName
  }
  properties: {
    publisherEmail: apimPublisherEmail
    publisherName: apimPublisherName
  }
}
