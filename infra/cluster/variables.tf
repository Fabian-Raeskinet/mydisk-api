variable "state_passphrase" {
  type        = string
  sensitive   = true
  description = "Passphrase encrypting the state, which contains the database passwords."

  validation {
    condition     = length(var.state_passphrase) >= 16
    error_message = "The state passphrase must be at least 16 characters."
  }
}

variable "kube_context" {
  type        = string
  description = "Kubeconfig context of the target cluster, required to never target the wrong cluster."
}

variable "kubeconfig_path" {
  type    = string
  default = "~/.kube/config"
}

variable "namespace" {
  type    = string
  default = "mydisk"
}

variable "traefik_chart_version" {
  type    = string
  default = "42.4.0"
}

variable "traefik_service_annotations" {
  type        = map(string)
  default     = {}
  description = "Optional platform annotations for the public ingress service."
}
