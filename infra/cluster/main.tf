resource "helm_release" "traefik" {
  name             = "traefik"
  repository       = "oci://ghcr.io/traefik/helm"
  chart            = "traefik"
  version          = var.traefik_chart_version
  namespace        = "traefik"
  create_namespace = true
  wait             = true
  values = [yamlencode({
    service = { annotations = var.traefik_service_annotations }
  })]
}

resource "kubernetes_namespace_v1" "app" {
  metadata {
    name = var.namespace
    labels = {
      "pod-security.kubernetes.io/enforce" = "baseline"
      "pod-security.kubernetes.io/warn"    = "restricted"
    }
  }
}

resource "random_password" "postgres" {
  for_each = toset(["POSTGRES_PASSWORD", "POSTGRES_MIGRATOR_PASSWORD", "POSTGRES_APP_PASSWORD"])

  length  = 32
  special = false
}

resource "kubernetes_secret_v1" "postgres_credentials" {
  metadata {
    name      = "postgres-credentials"
    namespace = kubernetes_namespace_v1.app.metadata[0].name
  }

  data = { for key, password in random_password.postgres : key => password.result }
}

resource "helm_release" "cert_manager" {
  name             = "cert-manager"
  repository       = "oci://quay.io/jetstack/charts"
  chart            = "cert-manager"
  version          = "v1.21.3"
  namespace        = "cert-manager"
  create_namespace = true
  wait             = true
  timeout          = 300
  values = [yamlencode({
    crds = { enabled = true }
  })]
}
