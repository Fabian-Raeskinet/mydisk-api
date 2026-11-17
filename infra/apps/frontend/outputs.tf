output "frontend_service" {
  value = "${local.service.metadata.name}.${var.namespace}.svc.cluster.local:8080"
}

output "frontend_url" {
  value = var.ingress_host == null ? null : "${var.tls_secret_name == null ? "http" : "https"}://${var.ingress_host}"
}
