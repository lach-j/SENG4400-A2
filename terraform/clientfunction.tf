
resource "azurerm_service_plan" "functionserviceplan" {
  name                = "ASP-SENG4400-Functions"
  resource_group_name = azurerm_resource_group.seng4400-a2.name
  location            = azurerm_resource_group.seng4400-a2.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_function_app" "clientfunc" {
  client_certificate_mode    = "Required"
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key
  app_settings = {
    A2ServiceBus_SERVICEBUS = azurerm_servicebus_queue_authorization_rule.clientaccess.primary_connection_string
  }
  enabled              = true
  location             = azurerm_service_plan.functionserviceplan.location
  name                 = "SENG440-A2-ClientFunc"
  resource_group_name  = azurerm_resource_group.seng4400-a2.name
  service_plan_id      = azurerm_service_plan.functionserviceplan.id
  storage_account_name = azurerm_storage_account.storage.name
  site_config {
    always_on  = true
    ftps_state = "FtpsOnly"
    application_stack {
      dotnet_version = "6.0"
    }
  }

  timeouts {}
}

resource "azurerm_function_app_function" "primenumstrigger" {
  enabled         = true
  function_app_id = azurerm_linux_function_app.clientfunc.id
  name            = "PrimeNumsTrigger"
  config_json = jsonencode(
    {

      entryPoint  = "A2.ClientFunc.PrimeNumsTrigger.RunAsync"
      generatedBy = "Microsoft.NET.Sdk.Functions.Generator-4.1.1"
      scriptFile  = "../bin/A2.ClientFunc.dll"
      bindings = [
        {
          autoComplete      = true
          connection        = "A2ServiceBus_SERVICEBUS"
          isSessionsEnabled = false
          name              = "message"
          queueName         = azurerm_servicebus_queue.primenums.name
          type              = "serviceBusTrigger"
        },
      ]
      configurationSource = "attributes"
      disabled            = false
    }
  )
  timeouts {}
}