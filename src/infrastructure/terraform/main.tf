terraform {
  required_version = ">= 1.0"
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "tfstate-rg"
    storage_account_name = "tfstate${var.environment}"
    container_name       = "tfstate"
    key                  = "dnd-copilot.terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}
