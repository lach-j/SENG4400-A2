resource "azurerm_service_plan" "dashboardserviceplan" {
  name                = "ASP-SENG4400A2-9e87"
  resource_group_name = azurerm_resource_group.seng4400-a2.name
  location            = azurerm_resource_group.seng4400-a2.location
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "dashboard" {
  resource_group_name = azurerm_resource_group.seng4400-a2.name
  name                = "SENG-4400-A2-dashboard"
  location            = azurerm_service_plan.dashboardserviceplan.location
  service_plan_id     = azurerm_service_plan.dashboardserviceplan.id
  site_config {
    always_on = false
  }
}

resource "azurerm_linux_web_app" "server" {
  app_settings = {
    "AppSettings__ServiceBusConnectionString" = azurerm_servicebus_queue_authorization_rule.serveraccess.primary_connection_string
  }
  https_only = true
  logs {
    detailed_error_messages = false
    failed_request_tracing  = false

    http_logs {
      file_system {
        retention_in_days = 1
        retention_in_mb   = 35
      }
    }
  }
  resource_group_name = azurerm_resource_group.seng4400-a2.name
  name                = "SENG4400-A2-Server"
  location            = azurerm_service_plan.dashboardserviceplan.location
  service_plan_id     = azurerm_service_plan.dashboardserviceplan.id
  site_config {
    always_on  = false
    ftps_state = "FtpsOnly"
  }
  timeouts {}
}