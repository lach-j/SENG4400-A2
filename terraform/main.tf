# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "seng4400-a2" {
  name     = "SENG4400-A2"
  location = "australiaeast"
}

resource "azurerm_storage_account" "storage" {
  account_kind             = "Storage"
  account_tier             = "Standard"
  location                 = azurerm_resource_group.seng4400-a2.location
  name                     = "primesolvergroup8076"
  resource_group_name      = azurerm_resource_group.seng4400-a2.name
  account_replication_type = "LRS"

  timeouts {}
}