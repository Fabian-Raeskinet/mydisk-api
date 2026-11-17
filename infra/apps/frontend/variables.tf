variable "kube_context" {
  type        = string
  description = "Explicit kubeconfig context of an existing cluster."
}

variable "kubeconfig_path" {
  type    = string
  default = "~/.kube/config"
}

variable "namespace" {
  type        = string
  default     = "mydisk"
  description = "Existing namespace containing the API; this stack does not create it."
}

variable "image" {
  type        = string
  description = "Frontend image including an immutable tag or digest."
}

variable "image_pull_policy" {
  type    = string
  default = "IfNotPresent"

  validation {
    condition     = contains(["Always", "IfNotPresent", "Never"], var.image_pull_policy)
    error_message = "Use Always, IfNotPresent or Never."
  }
}

variable "replicas" {
  type    = number
  default = 2

  validation {
    condition     = var.replicas >= 1 && floor(var.replicas) == var.replicas
    error_message = "replicas must be a positive integer."
  }
}

variable "ingress_host" {
  type        = string
  default     = null
  nullable    = true
  description = "Optional dedicated frontend hostname. Use a hostname not owned by an existing ingress. Null creates no ingress."

  validation {
    condition     = var.ingress_host == null || can(regex("^[a-z0-9]([a-z0-9.-]*[a-z0-9])?$", var.ingress_host))
    error_message = "Use a lowercase DNS hostname without a scheme or path, or null."
  }
}

variable "ingress_class_name" {
  type    = string
  default = "traefik"
}

variable "ingress_annotations" {
  type        = map(string)
  default     = {}
  description = "Optional ingress controller or cert-manager annotations."
}

variable "tls_secret_name" {
  type        = string
  default     = null
  nullable    = true
  description = "Optional TLS secret in the application namespace. Certificates are managed separately."
}
