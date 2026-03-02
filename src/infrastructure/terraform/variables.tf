variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "app_name" {
  description = "Application name"
  type        = string
  default     = "dnd-copilot"
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
  default     = ""
}

variable "app_service_sku" {
  description = "App Service Plan SKU"
  type        = string
  default     = "B1"
}

variable "jwt_secret_key" {
  description = "JWT secret key for authentication"
  type        = string
  sensitive   = true
}

variable "jwt_issuer" {
  description = "JWT issuer"
  type        = string
  default     = "DndCopilotApi"
}

variable "jwt_audience" {
  description = "JWT audience"
  type        = string
  default     = "DndCopilotClient"
}
