# Frontend with Terraform / OpenTofu

This is an **alternative** to the frontend Kustomize deployment. The default
`make k8s-up` / `make k8s-deploy` workflow manages the frontend through Kustomize;
it does not need this stack. Do not let both workflows manage `mydisk-frontend`.

The stack reuses `apps/frontend/k8s/base/deployment.yml` and `service.yml`, with
configurable image, namespace and replicas. It works with Terraform
or OpenTofu and targets an existing cluster, namespace and API. The namespace,
database credentials, ingress controller and certificate infrastructure remain
in `infra/cluster`. This stack uses the frontend image's default authentication
settings. See the [frontend setup](../../../README.md#frontend) for the Keycloak
settings to supply when enabling sign-in.

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
# Preview static assets at http://localhost:3000
```

Set `ingress_host` to create an ingress on a **dedicated hostname**, distinct from
the API ingress's hostname. Optionally set `tls_secret_name` and controller or
cert-manager annotations. A TLS secret must already exist, or cert-manager must
be configured to provision it. Traefik routes `/api`, `/health`, `/maintenance` and `/swagger` to
the `mydisk-api` Service in the same namespace, using the same-origin middleware
from the API overlay. The frontend Service handles other paths. Frontend probes use `/frontend-health` and do not depend on the database.

For an existing Kustomize installation, keep using Kustomize unless you explicitly
plan a state import and ownership migration. Applying this stack over existing
objects can fail with field ownership conflicts and should not be used as a
shortcut for migration. This stack uses local state by default; configure an
appropriate backend before sharing it across a team.
