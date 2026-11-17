# Share the Kubernetes source with Kustomize. Only deployment-specific values
# are overridden here, so probes, security settings and resources stay aligned.
locals {
  manifests_path = "${path.module}/../../../apps/frontend/k8s/base"
  deployment     = yamldecode(file("${local.manifests_path}/deployment.yml"))
  service        = yamldecode(file("${local.manifests_path}/service.yml"))
  container      = local.deployment.spec.template.spec.containers[0]
  api_upstream   = var.api_upstream == null ? "http://mydisk-api.${var.namespace}.svc.cluster.local:80" : var.api_upstream
}

resource "kubernetes_manifest" "deployment" {
  manifest = merge(local.deployment, {
    metadata = merge(local.deployment.metadata, { namespace = var.namespace })
    spec = merge(local.deployment.spec, {
      replicas = var.replicas
      template = merge(local.deployment.spec.template, {
        spec = merge(local.deployment.spec.template.spec, {
          containers = [merge(local.container, {
            image           = var.image
            imagePullPolicy = var.image_pull_policy
            env = [for env in local.container.env : merge(env,
              env.name == "API_UPSTREAM" ? { value = local.api_upstream } : {}
            )]
          })]
        })
      })
    })
  })

  wait {
    rollout = true
  }
}

resource "kubernetes_manifest" "service" {
  manifest = merge(local.service, {
    metadata = merge(local.service.metadata, { namespace = var.namespace })
  })
}

resource "kubernetes_manifest" "ingress" {
  count = var.ingress_host == null ? 0 : 1

  manifest = {
    apiVersion = "networking.k8s.io/v1"
    kind       = "Ingress"
    metadata = {
      name        = "mydisk-frontend"
      namespace   = var.namespace
      annotations = var.ingress_annotations
    }
    spec = merge({
      ingressClassName = var.ingress_class_name
      rules = [{
        host = var.ingress_host
        http = {
          paths = [{
            path     = "/"
            pathType = "Prefix"
            backend = {
              service = {
                name = local.service.metadata.name
                port = { name = "http" }
              }
            }
          }]
        }
      }]
      }, var.tls_secret_name == null ? {} : {
      tls = [{ hosts = [var.ingress_host], secretName = var.tls_secret_name }]
    })
  }

  depends_on = [kubernetes_manifest.service]
}
