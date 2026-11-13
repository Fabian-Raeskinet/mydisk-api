#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
: "${STATE_ACCOUNT:?Set TF_STATE_STORAGE_ACCOUNT in GitHub}"
: "${ARM_SUBSCRIPTION_ID:?Missing subscription}"
: "${TF_VAR_state_passphrase:?Missing state passphrase}"
if [[ ! "$STATE_ACCOUNT" =~ ^[a-z0-9]{3,24}$ ]]; then
  echo 'Storage account names require 3-24 lowercase letters or digits.' >&2
  exit 1
fi
export TF_VAR_storage_account_name="$STATE_ACCOUNT"
state_group=rg-mydisk-tfstate
account_id="/subscriptions/$ARM_SUBSCRIPTION_ID/resourceGroups/$state_group/providers/Microsoft.Storage/storageAccounts/$STATE_ACCOUNT"
container_id="$account_id/blobServices/default/containers/tfstate"
key=mydisk-bootstrap.tfstate

write_backend() {
  cat > backend.tf <<'BACKEND'
terraform {
  backend "azurerm" {
    use_azuread_auth = true
  }
}
BACKEND
}
init_remote() {
  tofu init -input=false "$@" \
    -backend-config="storage_account_name=$STATE_ACCOUNT" \
    -backend-config=container_name=tfstate \
    -backend-config="key=$key" -backend-config=use_oidc=true
}

# Query ARM before accessing blobs so a fresh subscription needs no existing storage.
group_exists=$(az group exists --name "$state_group")
account_exists=false
container_exists=false
remote_exists=false
if [ "$group_exists" = true ]; then
  count=$(az storage account list -g "$state_group" --query "[?name=='$STATE_ACCOUNT'] | length(@)" -o tsv)
  if [ "$count" = 1 ]; then
    account_exists=true
    container_exists=$(az storage container exists --account-name "$STATE_ACCOUNT" \
      --name tfstate --auth-mode login --query exists -o tsv)
    if [ "$container_exists" = true ]; then
      remote_exists=$(az storage blob exists --account-name "$STATE_ACCOUNT" \
        --container-name tfstate --name "$key" --auth-mode login --query exists -o tsv)
    fi
  fi
fi

if [ "$remote_exists" = true ]; then
  write_backend
  init_remote
else
  tofu init -input=false
  # Recover an interrupted first run or adopt the dedicated manually created backend.
  if [ "$group_exists" = true ]; then
    tofu import azurerm_resource_group.state "/subscriptions/$ARM_SUBSCRIPTION_ID/resourceGroups/$state_group"
  fi
  if [ "$account_exists" = true ]; then
    tofu import azurerm_storage_account.state "$account_id"
  fi
  if [ "$container_exists" = true ]; then
    tofu import azurerm_storage_container.state "$container_id"
  fi
fi

tofu plan -out=bootstrap.tfplan
tofu apply bootstrap.tfplan
if [ "$remote_exists" != true ]; then
  write_backend
  init_remote -migrate-state -force-copy
fi
