locals{
    ressourceName = "mydisks-int"
}

resource "azurerm_linux_web_app" "WebApp" {
  name = local.ressourceName
  location = 
}