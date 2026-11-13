# Azure-specific inputs passed to the cloud-agnostic Kubernetes stack.
traefik_service_annotations = {
  "service.beta.kubernetes.io/azure-dns-label-name" = "mydisk-fabian"
}
