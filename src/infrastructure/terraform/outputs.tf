output "resource_group_name" {
  description = "Resource Group name"
  value       = azurerm_resource_group.main.name
}

output "api_app_service_name" {
  description = "API App Service name"
  value       = azurerm_linux_web_app.api.name
}

output "api_app_service_url" {
  description = "API App Service URL"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "frontend_app_service_name" {
  description = "Frontend App Service name"
  value       = azurerm_linux_web_app.frontend.name
}

output "frontend_app_service_url" {
  description = "Frontend App Service URL"
  value       = "https://${azurerm_linux_web_app.frontend.default_hostname}"
}

output "storage_account_name" {
  description = "Storage Account name"
  value       = azurerm_storage_account.main.name
}
