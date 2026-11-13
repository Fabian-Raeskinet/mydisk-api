variable "state_passphrase" {
  type        = string
  sensitive   = true
  description = "Passphrase encrypting the state, which contains the cluster admin kubeconfig."

  validation {
    condition     = length(var.state_passphrase) >= 16
    error_message = "The state passphrase must be at least 16 characters."
  }
}

variable "cluster_name" {
  type    = string
  default = "mydisk"
}

variable "resource_group_name" {
  type    = string
  default = "rg-mydisk"
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "kubernetes_version" {
  type        = string
  default     = null
  description = "Null selects the AKS default version."
}

variable "node_vm_size" {
  type    = string
  default = "Standard_D4ls_v6"
}

variable "node_count" {
  type    = number
  default = 1
}

variable "tags" {
  type = map(string)
  default = {
    project    = "mydisk"
    managed-by = "opentofu"
  }
}
