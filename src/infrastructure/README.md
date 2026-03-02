# D&D Copilot - Azure Infrastructure

This directory contains Terraform templates to deploy the D&D Copilot application to Azure.

## Architecture

The infrastructure deploys:
- **Resource Group**: Container for all Azure resources
- **App Service Plan (API)**: Hosts the ASP.NET Core Web API
- **Linux Web App (API)**: Runs the .NET 9.0 API application
- **App Service Plan (Frontend)**: Hosts the React frontend
- **Linux Web App (Frontend)**: Runs the Node.js React application
- **Storage Account**: For database backups and file storage
- **Storage Container**: For database backup files

## Prerequisites

1. **Azure CLI** installed and configured
2. **Terraform** >= 1.0 installed
3. **Azure Subscription** with appropriate permissions
4. **Service Principal** or Azure CLI authentication

## Configuration

### Environment Variables

Set the following environment variables:

```bash
export TF_VAR_jwt_secret_key="your-secret-key-here"
export ARM_CLIENT_ID="your-service-principal-client-id"
export ARM_CLIENT_SECRET="your-service-principal-secret"
export ARM_SUBSCRIPTION_ID="your-azure-subscription-id"
export ARM_TENANT_ID="your-azure-tenant-id"
```

### Terraform Variables

Copy `terraform.tfvars.example` to `terraform.tfvars` and customize:

```bash
cp terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars`:

```hcl
environment      = "dev"
location         = "eastus"
app_service_sku  = "B1"
jwt_secret_key   = "your-secret-key-here"  # Or use TF_VAR_jwt_secret_key
```

## Deployment

### Manual Deployment

1. **Initialize Terraform**:
```bash
cd infrastructure/terraform
terraform init
```

2. **Plan the deployment**:
```bash
terraform plan
```

3. **Apply the configuration**:
```bash
terraform apply
```

4. **Confirm** by typing `yes` when prompted

### Automated Deployment (GitHub Actions)

The project includes a GitHub Actions workflow that automatically deploys using these Terraform templates. See `.github/workflows/azure-deploy.yml`.

## Backend State

The Terraform state is stored in Azure Storage. Make sure you create the backend resources first:

```bash
# Create resource group for state
az group create --name tfstate-rg --location eastus

# Create storage account
az storage account create \
  --name tfstatedev \
  --resource-group tfstate-rg \
  --location eastus \
  --sku Standard_LRS

# Create container
az storage container create \
  --name tfstate \
  --account-name tfstatedev
```

Or comment out the `backend` block in `main.tf` to use local state.

## Outputs

After deployment, Terraform outputs:

- `resource_group_name`: Name of the resource group
- `api_app_service_url`: URL of the API application
- `frontend_app_service_url`: URL of the frontend application
- `storage_account_name`: Name of the storage account

## Deploying Application Code

### API Deployment

```bash
cd src/DndCopilot.Api
dotnet publish -c Release -o ./publish
cd publish
zip -r ../api.zip .
az webapp deployment source config-zip \
  --resource-group dnd-copilot-dev-rg \
  --name dnd-copilot-api-dev \
  --src ../api.zip
```

### Frontend Deployment

```bash
cd client
npm install
npm run build
cd build
zip -r ../frontend.zip .
az webapp deployment source config-zip \
  --resource-group dnd-copilot-dev-rg \
  --name dnd-copilot-frontend-dev \
  --src ../frontend.zip
```

## Cost Estimation

With B1 App Service Plan:
- **App Service Plans**: ~$13/month each (2 plans)
- **Storage Account**: ~$1/month
- **Total**: ~$27/month

Upgrade to production-ready SKU (P1V2) for better performance.

## Cleanup

To destroy all resources:

```bash
terraform destroy
```

## Security Notes

1. Store JWT secret keys in **Azure Key Vault** for production
2. Enable **HTTPS only** for App Services
3. Configure **Application Insights** for monitoring
4. Use **Azure SQL Database** instead of SQLite for production
5. Enable **authentication/authorization** at App Service level

## Troubleshooting

### State Lock Issues

If Terraform state is locked:

```bash
terraform force-unlock <lock-id>
```

### App Service Logs

View logs:

```bash
az webapp log tail \
  --resource-group dnd-copilot-dev-rg \
  --name dnd-copilot-api-dev
```

### Connection Issues

Ensure CORS is configured correctly and the frontend has the correct API URL in environment variables.
