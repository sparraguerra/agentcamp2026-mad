# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name != "" ? var.resource_group_name : "${var.app_name}-${var.environment}-rg"
  location = var.location

  tags = {
    Environment = var.environment
    Application = var.app_name
    ManagedBy   = "Terraform"
  }
}

# App Service Plan for API
resource "azurerm_service_plan" "api" {
  name                = "${var.app_name}-api-${var.environment}-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = var.app_service_sku

  tags = {
    Environment = var.environment
    Application = var.app_name
  }
}

# API App Service (ASP.NET Core)
resource "azurerm_linux_web_app" "api" {
  name                = "${var.app_name}-api-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.api.id

  site_config {
    always_on = true
    
    application_stack {
      dotnet_version = "9.0"
    }

    cors {
      allowed_origins     = ["*"]
      support_credentials = false
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"           = var.environment
    "ConnectionStrings__DefaultConnection" = "Data Source=/home/site/wwwroot/dndcopilot.db"
    "JwtSettings__SecretKey"           = var.jwt_secret_key
    "JwtSettings__Issuer"              = var.jwt_issuer
    "JwtSettings__Audience"            = var.jwt_audience
    "JwtSettings__ExpirationHours"     = "24"
  }

  logs {
    application_logs {
      file_system_level = "Information"
    }
    
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  tags = {
    Environment = var.environment
    Application = var.app_name
  }
}

# App Service Plan for Frontend
resource "azurerm_service_plan" "frontend" {
  name                = "${var.app_name}-frontend-${var.environment}-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = var.app_service_sku

  tags = {
    Environment = var.environment
    Application = var.app_name
  }
}

# Frontend App Service (React)
resource "azurerm_linux_web_app" "frontend" {
  name                = "${var.app_name}-frontend-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.frontend.id

  site_config {
    always_on = true
    
    application_stack {
      node_version = "20-lts"
    }
  }

  app_settings = {
    "REACT_APP_API_URL" = "https://${azurerm_linux_web_app.api.default_hostname}/api"
    "NODE_ENV"          = var.environment == "prod" ? "production" : "development"
  }

  tags = {
    Environment = var.environment
    Application = var.app_name
  }
}

# Storage Account for database backups
resource "azurerm_storage_account" "main" {
  name                     = "${replace(var.app_name, "-", "")}${var.environment}sa"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  blob_properties {
    versioning_enabled = true
  }

  tags = {
    Environment = var.environment
    Application = var.app_name
  }
}

# Storage Container for database backups
resource "azurerm_storage_container" "backups" {
  name                  = "database-backups"
  storage_account_id    = azurerm_storage_account.main.id
  container_access_type = "private"
}
