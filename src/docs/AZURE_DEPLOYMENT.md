# Instrucciones de Despliegue en Azure - D&D Copilot

## 📋 Requisitos Previos

### Herramientas Necesarias
1. **Azure CLI** (v2.50+)
   ```bash
   az version
   ```
2. **Terraform** (v1.0+)
   ```bash
   terraform version
   ```
3. **Git** para control de versión
4. **Visual Studio Code** o IDE de preferencia

### Credenciales Azure
1. Cuenta de Azure activa con suscripción
2. Acceso a crear recursos
3. Permisos de contributor mínimo

## 🔐 Configuración Inicial

### Paso 1: Instalar Azure CLI

**Windows (PowerShell)**
```powershell
$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; .\AzureCLI.msi
```

**macOS (Homebrew)**
```bash
brew install azure-cli
```

**Linux (apt)**
```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### Paso 2: Instalar Terraform

Descargar desde: https://www.terraform.io/downloads.html

**Windows (Chocolatey)**
```powershell
choco install terraform
```

**macOS (Homebrew)**
```bash
brew install terraform
```

### Paso 3: Autenticarse en Azure

```bash
az login
# Se abrirá navegador para autenticación

# Ver cuenta activa
az account show

# Listar suscripciones disponibles
az account list --output table

# Cambiar de suscripción
az account set --subscription "<subscription-id>"
```

## 🏗️ Preparación del Proyecto

### Paso 1: Configurar Backend

#### Compilar API
```bash
cd src/DndCopilot.Api
dotnet build --configuration Release
```

#### Crear appsettings de Producción
```bash
# Copiar archivo de ejemplo
cp appsettings.json appsettings.Production.json
```

**appsettings.Production.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER.database.windows.net;Database=DndCopilotDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  },
  "JwtSettings": {
    "SecretKey": "GENERATE_STRONG_SECRET_KEY_HERE",
    "Issuer": "DndCopilotApi",
    "Audience": "DndCopilotClient",
    "ExpirationHours": "24"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### Paso 2: Preparar Frontend

#### Compilar React
```bash
cd client
npm install
npm run build
```

Esto genera carpeta `/client/build` lista para desplegar.

#### Configurar Variables de Entorno
**client/.env.production**
```
REACT_APP_API_URL=https://your-api.azurewebsites.net/api
REACT_APP_JWT_KEY=auth_token
```

### Paso 3: Configurar Terraform

#### Variables principales
**infrastructure/terraform/terraform.tfvars**
```hcl
environment     = "prod"
location        = "WestEurope"
project_name    = "dnd-copilot"
resource_group  = "dnd-copilot-rg"

# SQL Database
sql_admin_username = "dndadmin"
sql_admin_password = "P@ssw0rd123!Strong"  # Cambiar!!

# App Service
app_service_sku = "B2"  # Basic plan

# Static Web App
static_app_sku = "Standard"
```

#### Backend Storage Terraform
**infrastructure/terraform/main.tf** (actualizar)
```hcl
terraform {
  required_version = ">= 1.0"
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  
  # Configurar backend remoto en Azure
  backend "azurerm" {
    resource_group_name  = "tfstate-rg"
    storage_account_name = "tfstateglobal"
    container_name       = "tfstate"
    key                  = "dnd-copilot.terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}
```

## 🚀 Despliegue Paso a Paso

### Opción 1: Despliegue Automático con Terraform

#### 1. Crear Recurso Group (si no existe)
```bash
az group create \
  --name dnd-copilot-rg \
  --location WestEurope
```

#### 2. Crear Storage para Terraform State
```bash
az storage account create \
  --name tfstateglobal \
  --resource-group dnd-copilot-rg \
  --location WestEurope \
  --sku Standard_LRS

az storage container create \
  --name tfstate \
  --account-name tfstateglobal \
  --account-key $(az storage account keys list \
    --resource-group dnd-copilot-rg \
    --account-name tfstateglobal \
    --query "[0].value" -o tsv)
```

#### 3. Inicializar Terraform
```bash
cd infrastructure/terraform

# Configurar credenciales para backend
export ARM_ACCESS_KEY=$(az storage account keys list \
  --resource-group dnd-copilot-rg \
  --account-name tfstateglobal \
  --query "[0].value" -o tsv)

terraform init
```

#### 4. Planificar Despliegue
```bash
terraform plan -out=tfplan
```

Revisar recursos a crear:
- Resource Group
- App Service + App Service Plan
- SQL Server + Database
- Static Web App
- Key Vault
- Application Insights

#### 5. Aplicar Cambios
```bash
terraform apply tfplan
```

**Salida esperada:**
```
Apply complete! Resources: 15 added, 0 changed, 0 destroyed.

Outputs:
api_endpoint = "https://dnd-copilot-api.azurewebsites.net"
webapp_url = "https://dnd-copilot-web.azurestaticapps.net"
database_server = "dnd-copilot-sql.database.windows.net"
```

### Opción 2: Despliegue Manual

#### 1. Crear Resource Group
```bash
az group create --name dnd-copilot-rg --location WestEurope
```

#### 2. Desplegar Backend (App Service)

Crear App Service Plan:
```bash
az appservice plan create \
  --name dnd-copilot-plan \
  --resource-group dnd-copilot-rg \
  --sku B2 \
  --is-linux
```

Crear Web App:
```bash
az webapp create \
  --resource-group dnd-copilot-rg \
  --plan dnd-copilot-plan \
  --name dnd-copilot-api \
  --runtime "DOTNET:9.0"
```

Desplegar código:
```bash
cd src/DndCopilot.Api
dotnet publish -c Release -o ./publish

# Usar Azure CLI o Azure DevOps
az webapp deployment source config-zip \
  --resource-group dnd-copilot-rg \
  --name dnd-copilot-api \
  --src publish.zip
```

#### 3. Crear SQL Database

Crear SQL Server:
```bash
az sql server create \
  --name dnd-copilot-sql \
  --resource-group dnd-copilot-rg \
  --admin-user dndadmin \
  --admin-password "P@ssw0rd123!Strong"
```

Crear Database:
```bash
az sql db create \
  --resource-group dnd-copilot-rg \
  --server dnd-copilot-sql \
  --name DndCopilotDb \
  --edition Standard \
  --compute-model Serverless
```

Configurar Firewall:
```bash
az sql server firewall-rule create \
  --resource-group dnd-copilot-rg \
  --server dnd-copilot-sql \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

#### 4. Desplegar Frontend (Static Web App)

```bash
az staticwebapp create \
  --name dnd-copilot-web \
  --resource-group dnd-copilot-rg \
  --source https://github.com/YOUR_REPO \
  --branch main \
  --token YOUR_GITHUB_TOKEN \
  --location WestEurope \
  --sku Standard
```

O desplegar manualmente:
```bash
cd client
npm run build

az staticwebapp deploy \
  --name dnd-copilot-web \
  --source ./build \
  --location WestEurope
```

## 🔑 Configuración de Secretos

### Usar Azure Key Vault

#### Crear Key Vault
```bash
az keyvault create \
  --name dnd-copilot-kv \
  --resource-group dnd-copilot-rg \
  --location WestEurope
```

#### Almacenar Secretos
```bash
az keyvault secret set \
  --vault-name dnd-copilot-kv \
  --name "JwtSecretKey" \
  --value "YOUR_LONG_RANDOM_SECRET_HERE"

az keyvault secret set \
  --vault-name dnd-copilot-kv \
  --name "SqlConnectionString" \
  --value "Server=YOUR_SQL.database.windows.net;Database=DndCopilotDb;..."
```

#### Asignar Acceso a App Service
```bash
# Obtener identity de App Service
PRINCIPAL_ID=$(az webapp identity assign \
  --resource-group dnd-copilot-rg \
  --name dnd-copilot-api \
  --query principalId -o tsv)

# Asignar permisos al Key Vault
az keyvault set-policy \
  --name dnd-copilot-kv \
  --object-id $PRINCIPAL_ID \
  --secret-permissions get list
```

### Configurar App Settings

```bash
az webapp config appsettings set \
  --resource-group dnd-copilot-rg \
  --name dnd-copilot-api \
  --settings \
    ConnectionStrings__DefaultConnection="@Microsoft.KeyVault(SecretUri=https://dnd-copilot-kv.vault.azure.net/secrets/SqlConnectionString/)" \
    JwtSettings__SecretKey="@Microsoft.KeyVault(SecretUri=https://dnd-copilot-kv.vault.azure.net/secrets/JwtSecretKey/)" \
    DOTNET_ENVIRONMENT="Production"
```

## 🗄️ Inicializar Base de Datos

### Ejecutar Migrations

```bash
# Desde máquina local (o usar Azure Cloud Shell)
cd src/DndCopilot.Api

# Aplicar migrations
dotnet ef database update \
  --connection "Server=dnd-copilot-sql.database.windows.net;Database=DndCopilotDb;User Id=dndadmin;Password=P@ssw0rd123!Strong;"
```

### Seed de Datos
Se ejecutará automáticamente al iniciar la aplicación en producción (verificar `Program.cs`).

## ✅ Validación del Despliegue

### Comprobar Backend
```bash
# URLs esperadas
curl https://dnd-copilot-api.azurewebsites.net/swagger
curl https://dnd-copilot-api.azurewebsites.net/api/dice/roll
```

### Comprobar Frontend
```bash
# Abrir en navegador
https://dnd-copilot-web.azurestaticapps.net
```

### Comprobar Logs
```bash
# Ver logs del App Service
az webapp log tail \
  --resource-group dnd-copilot-rg \
  --name dnd-copilot-api

# Ver metrics
az monitor metrics list-definitions \
  --resource-group dnd-copilot-rg \
  --resource dnd-copilot-api \
  --resource-type "Microsoft.Web/sites"
```

## 🔄 CI/CD con GitHub Actions

### Crear Workflow
**.github/workflows/deploy-azure.yml**
```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]

env:
  AZURE_RESOURCE_GROUP: dnd-copilot-rg
  AZURE_APP_NAME: dnd-copilot-api

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0'
      
      - name: Build
        run: dotnet build --configuration Release
        
      - name: Test
        run: dotnet test
        
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_APP_NAME }}
          package: ./publish
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
```

## 🧹 Limpiar Recursos (Opcional)

### Eliminar Resource Group (elimina todo)
```bash
az group delete \
  --name dnd-copilot-rg \
  --yes \
  --no-wait
```

### O eliminar con Terraform
```bash
cd infrastructure/terraform
terraform destroy
```

## 📊 Monitoreo en Producción

### Application Insights
```bash
# Crear Application Insights
az monitor app-insights component create \
  --resource-group dnd-copilot-rg \
  --app dnd-copilot-insights \
  --location WestEurope

# Configurar en App Service
az webapp config appsettings set \
  --resource-group dnd-copilot-rg \
  --name dnd-copilot-api \
  --settings \
    APPINSIGHTS_INSTRUMENTATIONKEY="YOUR_INSTRUMENTATION_KEY"
```

### Alertas
```bash
# Crear alerta si App Service tiene error rate > 5%
az monitor metrics alert create \
  --resource-group dnd-copilot-rg \
  --name high-error-rate \
  --description "Alert when error rate is high" \
  --scopes /subscriptions/{subscription}/resourceGroups/dnd-copilot-rg/providers/Microsoft.Web/sites/dnd-copilot-api \
  --condition "avg Http5xxErrors > 5"
```

## 🔗 URLs Importantes

| Recurso | URL |
|---------|-----|
| Frontend | `https://dnd-copilot-web.azurestaticapps.net` |
| Backend API | `https://dnd-copilot-api.azurewebsites.net/api` |
| Swagger API | `https://dnd-copilot-api.azurewebsites.net/swagger` |
| Azure Portal | `https://portal.azure.com` |
| Key Vault | `https://dnd-copilot-kv.vault.azure.net` |

## ❓ Troubleshooting

### Error: Application is not available
```bash
# Verificar estado
az webapp show --resource-group dnd-copilot-rg --name dnd-copilot-api --query state

# Reiniciar
az webapp restart --resource-group dnd-copilot-rg --name dnd-copilot-api
```

### Error: Database connection failed
```bash
# Verificar firewall
az sql server firewall-rule list --resource-group dnd-copilot-rg --server dnd-copilot-sql

# Obtener IP actual
curl https://ifconfig.me
```

### Error: Static Web App deployment failed
```bash
# Ver logs detallados
az staticwebapp show --resource-group dnd-copilot-rg --name dnd-copilot-web
```

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Última Actualización**: Febrero 2026
