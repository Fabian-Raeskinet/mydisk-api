# Provision MyDisk with GitHub Actions

## Initial setup

Provisioning is triggered in GitHub through the **Infrastructure AKS** workflow,
using the **apply** action. It prepares state storage, AKS, and the Kubernetes configuration.

The Azure application `mydisk-github` uses a federated identity for the
`Fabian-Raeskinet/mydisk-api` repository and the `production` GitHub environment.
The OIDC subject must match the subject issued by GitHub for this repository.

Assign the following two roles to this application on the selected subscription
as a one-time setup:

- **Contributor** to create resource groups, storage, and AKS.
- **Storage Blob Data Contributor** to read and write state using Entra ID.
  Subscription scope grants access to the storage account as soon as it is created.
  It also grants access to other Blob data in that subscription. A subscription
  dedicated to learning helps limit this scope.

The workflow uses these existing permissions. Permission to manage IAM role
assignments remains with the administrator performing the initial setup.

In GitHub, open `Settings → Environments → production`. Restrict allowed branches
to `master`.

| GitHub environment variable | Value |
|---|---|
| `AZURE_CLIENT_ID` | Application ID of `mydisk-github` |
| `AZURE_TENANT_ID` | Entra tenant ID |
| `AZURE_SUBSCRIPTION_ID` | Selected subscription ID |
| `TF_STATE_STORAGE_ACCOUNT` | Globally unique name, 3–24 lowercase letters or digits, such as `mydisktfstatefabian` |
| `AKS_RESOURCE_GROUP` | Optional; defaults to `rg-mydisk` |
| `AKS_CLUSTER_NAME` | Optional; defaults to `mydisk` |
| `AZURE_LOCATION` | Optional; defaults to `westeurope` |
| `AKS_NODE_VM_SIZE` | Optional; defaults to `Standard_D4ls_v6`; verify availability for your subscription |

Add the **`TF_STATE_PASSPHRASE`** secret using a long, random value of at least
16 characters. Keep it in a password manager. OpenTofu states and plans are
encrypted with this value. Subsequent runs must use the same passphrase.

## Run the workflows

1. Push the files to `master` to make the workflows available.
2. Open `Actions → Infrastructure AKS → Run workflow` and select `apply`.
3. The workflow creates the `rg-mydisk-tfstate` resource group, a Standard LRS storage account, and its private `tfstate` container.
4. It migrates its encrypted initial state into that storage.
5. It initializes the Azure and Kubernetes states, creates AKS, and configures Traefik, the namespace, and the PostgreSQL Secret.
6. Then run the `CD` workflow to publish the images and deploy MyDisk.

The `apply` action creates billable resources. It displays and applies its plans
within the same run. The `plan` action previews AKS only, after state storage has
been initialized. To require approval before the job runs, configure GitHub
environment protections according to the features available for the repository.

Azure role assignments may take a few minutes to become effective. If access is
denied immediately after an assignment, wait and retry. After an interruption,
bootstrap recovers resources already present in the dedicated group and uses its
remote state when available. If bootstrap fails, the workflow retains its encrypted
local state, if created, for seven days in the `encrypted-bootstrap-recovery` artifact.

## Directory structure

| Directory | Responsibility |
|---|---|
| `infra/platform/azure/bootstrap` | Azure state resource group and storage |
| `infra/platform/azure` | Application resource group and AKS |
| `infra/cluster` | Kubernetes and Helm through an explicit kubeconfig context |

Azure backend declarations are generated in the repository checkout used by GitHub.
The Kubernetes configuration can therefore use a different backend locally or on
another cloud.

The `tfstate` container holds three states: `mydisk-bootstrap.tfstate`,
`mydisk-cluster-aks.tfstate` for Azure, and `mydisk-platform.tfstate` for Kubernetes.
The last two names were retained after the directory restructuring to preserve
access to existing states.

If resources are already managed by a local state, migrate that state and retain
its passphrase before running the workflows. Bootstrap can adopt a manually
created storage resource group. If another OpenTofu state already manages that
group, transfer the state first to maintain a single owner.

## Costs and shutdown

The current default is AKS Free with one node. Verify the VM size's eligibility
and price in the subscription before the first run. OpenTofu plans do not calculate
prices. Virtual machines, disks, load balancers, and IP addresses may incur charges.

After each session, run **AKS start or stop** with the **stop** action. Retained
resources, including disks and state storage, may still incur charges. The **start**
action resumes the cluster before a CD deployment.

For an extended break, run **Infrastructure AKS** with the **destroy** action and
enter the cluster name to confirm. This action starts AKS if needed to remove
Kubernetes components, then destroys the cluster. Application data in this
environment may be lost. State storage is retained.

## Application deployment

Automatic CD on `master` assumes that the cluster has been created and started.
The first push may fail during deployment before provisioning has completed.
Rerun CD afterward.

GHCR images must be accessible to AKS. Use public packages for a portfolio, or
configure `imagePullSecrets` for private packages. The token used to publish images
in GitHub Actions does not automatically grant image access to pods.
