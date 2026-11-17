# Frontend with Terraform / OpenTofu

This is an **alternative** to the frontend Kustomize deployment. The default
`make k8s-up` / `make k8s-deploy` workflow manages the frontend through Kustomize;
it does not need this stack. Do not let both workflows manage `mydisk-frontend`.

The stack reuses `apps/frontend/k8s/base/deployment.yml` and `service.yml`, with
configurable image, namespace, replicas and API upstream. It works with Terraform
or OpenTofu and targets an existing cluster, namespace and API. The namespace,
database credentials, ingress controller and certificate infrastructure remain
in `infra/cluster`. There is no authentication or Keycloak configuration.

The API upstream defaults to
`http://mydisk-api.<namespace>.svc.cluster.local:80`, using the configured
namespace. Port `80` is the API Service port, which forwards to container port
`8080`. Nginx resolves this fully qualified service name directly: its runtime
DNS resolver does not use Kubernetes DNS search suffixes. Set `api_upstream` to
a fully qualified HTTP(S) origin if the API is in another namespace or your
cluster uses a DNS domain other than `cluster.local`. Do not add a trailing slash.
The Kustomize local/cloud overlays similarly use the complete service name for
their respective `default` and `mydisk` namespaces.

To choose Terraform for a new installation, deploy the API's own overlay
(`apps/api/k8s/overlays/cloud`) instead of the parent `deploy/k8s/cloud` overlay.
Run the migration Job successfully before deploying the API, as in the existing
Makefile workflow. Build and publish the frontend image separately. Then:

```sh
cd infra/apps/frontend
cp terraform.tfvars.example terraform.tfvars
# Set the target context, namespace and immutable image tag.
tofu init
tofu plan
tofu apply
```

Use `terraform` instead of `tofu` for HashiCorp Terraform. A working kubeconfig
is required at plan time because the Kubernetes manifest provider queries the
target cluster's API schema. No cloud credentials or passwords belong in tfvars.

By default only a Deployment and ClusterIP Service are created. To preview it:

```sh
kubectl --context mydisk -n mydisk port-forward service/mydisk-frontend 3000:8080
# Open http://localhost:3000
```

Set `ingress_host` to create an ingress on a **dedicated hostname**, distinct from
the API ingress's hostname. Optionally set `tls_secret_name` and controller or
cert-manager annotations. A TLS secret must already exist, or cert-manager must
be configured to provision it. The frontend proxies `/api`, `/health`,
`/maintenance` and `/swagger` to the API, keeping browser requests on the same
origin. Frontend probes use `/frontend-health` and do not depend on the database.

For an existing Kustomize installation, keep using Kustomize unless you explicitly
plan a state import and ownership migration. Applying this stack over existing
objects can fail with field ownership conflicts and should not be used as a
shortcut for migration. This stack uses local state by default; configure an
appropriate backend before sharing it across a team.
