
resource "azurerm_servicebus_namespace" "a2-servicebus" {
  name                = "A2ServiceBus335"
  resource_group_name = azurerm_resource_group.seng4400-a2.name
  location            = azurerm_resource_group.seng4400-a2.location
  sku                 = "Basic"
}

resource "azurerm_servicebus_queue" "primenums" {
  dead_lettering_on_message_expiration = false
  default_message_ttl                  = "PT1H"
  enable_partitioning                  = false
  lock_duration                        = "PT1M"
  max_delivery_count                   = 10
  max_size_in_megabytes                = 1024
  name                                 = "primenums"
  namespace_id                         = azurerm_servicebus_namespace.a2-servicebus.id
}

resource "azurerm_servicebus_queue_authorization_rule" "clientaccess" {
  name     = "ClientAccess"
  queue_id = azurerm_servicebus_queue.primenums.id
  listen   = true
  manage   = false
  send     = false
}

resource "azurerm_servicebus_queue_authorization_rule" "serveraccess" {
  name     = "ServerAccess"
  queue_id = azurerm_servicebus_queue.primenums.id
  listen   = false
  manage   = false
  send     = true
}