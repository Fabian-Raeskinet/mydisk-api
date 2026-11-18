
API_DIR = apps/api
GRAFANA_DIR = apps/grafana
FRONTEND_DIR = apps/frontend
FRONTEND_PROJECT = $(FRONTEND_DIR)/src/MyDisk.Frontend.csproj
API_K8S = $(API_DIR)/k8s
LOCAL_API_OVERLAY = $(API_K8S)/overlays/local
COMPOSE_ENV_FILE = $(API_DIR)/postgres.env
COMPOSE = docker compose --env-file $(COMPOSE_ENV_FILE) -f $(API_DIR)/compose.yml
KEYCLOAK_OVERLAY = apps/keycloak/k8s/overlays/local
KEYCLOAK_HOST = auth.mydisk.localhost
KEYCLOAK_URL = https://$(KEYCLOAK_HOST):8443
ENV ?= local
K8S_OVERLAY = deploy/k8s/$(ENV)
IMAGE_NAME = mydisk-api:dev
MIGRATIONS_IMAGE_NAME = mydisk-api-migrations:dev
GRAFANA_IMAGE_NAME = mydisk-grafana:1.0.0
FRONTEND_IMAGE_NAME = mydisk-frontend:dev
CLUSTER_NAME ?= mydisk
HTTP_PORT ?= 8080
K8S_MIGRATIONS_TIMEOUT ?= 5m
LOAD_TEST_SCRIPT = $(API_K8S)/bench/load-test.js
LOAD_TEST_PATH ?= /api/Disk/disks
API_DEPLOYMENT = deployment/mydisk-api
GRAFANA_DEPLOYMENT = deployment/mydisk-grafana
FRONTEND_DEPLOYMENT = deployment/mydisk-frontend
KEYCLOAK_DEPLOYMENT = deployment/keycloak
APP_DEPLOYMENTS = $(API_DEPLOYMENT) $(FRONTEND_DEPLOYMENT)
ROLLOUT_TIMEOUT ?= 5m
ROLLOUT_AUTO_UNDO ?= 1
ROLLOUT_BROKEN_PATH ?= /health/broken
RELEASE_OVERLAY = deploy/k8s/cloud-release
TEST_RESULTS ?= TestResults

REGISTRY ?= ghcr.io/fabian-raeskinet
IMAGE_TAG ?= latest
AKS_CLUSTER_NAME ?= mydisk
ifeq ($(ENV),local)
K8S_CONTEXT ?= k3d-$(CLUSTER_NAME)
K8S_NAMESPACE ?= default
else
K8S_CONTEXT ?= $(AKS_CLUSTER_NAME)
K8S_NAMESPACE ?= mydisk
endif
KUBECTL = kubectl --context $(K8S_CONTEXT) -n $(K8S_NAMESPACE)
ifeq ($(ENV),local)
API_URL = https://localhost:8443
else
API_URL = https://mydisk-fabian.westeurope.cloudapp.azure.com
endif

TOFU ?= tofu
INFRA_CLUSTER = infra/cluster
INFRA_PLATFORM = infra/platform/azure
INFRA_FRONTEND = infra/apps/frontend
INFRA_CONTEXT ?= $(AKS_CLUSTER_NAME)

.PHONY: help up down restart logs ps build clean test-unit frontend-restore frontend-dev frontend-check k8s-preflight k8s-smoke \
        local-https k8s-init-tls cluster-https-port cluster-create cluster-start cluster-stop cluster-delete cluster-reset cluster-status cluster-load \
        k8s-help k8s-init-secrets k8s-build k8s-render k8s-apply k8s-delete \
        k8s-status k8s-restart k8s-logs k8s-logs-db k8s-logs-migrations k8s-logs-keycloak k8s-forward-api k8s-forward-frontend k8s-forward-db k8s-smoke-keycloak k8s-up k8s-down \
        k8s-push k8s-load-test k8s-watch-hpa dev-up dev-down \
        k8s-rollout-ok k8s-rollout-broken k8s-rollout-undo k8s-rollout-history k8s-watch-rollout k8s-watch-traffic \
        k8s-release-overlay k8s-deploy k8s-rollout-check \
        infra-cluster infra-credentials infra-platform infra-up infra-down infra-check

# -----------------------------------------------------------------------------
# Default & Help
# -----------------------------------------------------------------------------
help:
	@echo "Usage: make [target] [ENV=local|cloud] [CLUSTER_NAME=mydisk] [HTTP_PORT=8080]"
	@echo "  security               Audit dependencies, build amd64 images and scan (no deployment)"
	@echo "  security-build         Build images for the same architecture as AKS"
	@echo "  security-dependencies  Audit all NuGet dependencies (High/Critical blocks)"
	@echo "  security-images        Scan built images with Trivy (requires Trivy)"
	@echo ""
	@echo "K3d Cluster targets:"
	@echo "  local-https         Enable HTTPS on an existing local cluster (no migrations)"
	@echo "  cluster-create      Create the local k3d cluster if not existing"
	@echo "  cluster-start       Start the stopped k3d cluster"
	@echo "  cluster-stop        Stop the k3d cluster to free resources"
	@echo "  cluster-delete      Delete the k3d cluster"
	@echo "  cluster-reset       Delete and recreate a fresh k3d cluster"
	@echo "  cluster-status      Show k3d clusters and nodes status"
	@echo "  cluster-load        Import built Docker images into k3d cluster"
	@echo ""
	@echo "Combined workflow targets:"
	@echo "  dev-up              Ensure cluster is ready, build image, import, and deploy"
	@echo "  dev-down            Delete Kubernetes deployment and stop cluster"
	@echo ""
	@echo "Docker Compose targets:"
	@echo "  up                  Build and start all services in detached mode"
	@echo "  down                Stop and remove all containers"
	@echo "  restart             Restart all running services"
	@echo "  logs                Stream logs from all services"
	@echo "  ps                  Display the status of all services"
	@echo "  build               Build or rebuild service images"
	@echo "  clean               Stop containers and remove volumes (WARNING: deletes DB)"
	@echo ""
	@echo "Test targets:"
	@echo "  test-unit           Run every *.Tests project, TRX results in $(TEST_RESULTS)/"
	@echo "  frontend-restore    Restore Blazor WebAssembly dependencies"
	@echo "  frontend-dev        Run Blazor WebAssembly with dotnet watch"
	@echo "  frontend-check      Build the Blazor WebAssembly frontend"
	@echo ""
	@echo "Infrastructure targets (OpenTofu, requires TF_VAR_state_passphrase):"
	@echo "  infra-up            Create the AKS cluster, fetch credentials, install the platform"
	@echo "  infra-down          Destroy the platform, then the AKS cluster"
	@echo "  infra-platform      Create or update the Azure platform ($(INFRA_PLATFORM))"
	@echo "  infra-credentials   Merge the AKS kubeconfig into ~/.kube/config"
	@echo "  infra-cluster       Install Traefik, namespace and DB Secret on context INFRA_CONTEXT=$(INFRA_CONTEXT)"
	@echo "  infra-check         Format check and validate platform and frontend stacks"
	@echo ""
	@echo "Kubernetes targets (default ENV=local, ENV=cloud targets context $(AKS_CLUSTER_NAME)):"
	@echo "  k8s-up              local: build, import to k3d, deploy | cloud: build, push to registry, deploy"
	@echo "  k8s-push            Build linux/amd64 images and push them to $(REGISTRY) with tag $(IMAGE_TAG)"
	@echo "  k8s-deploy          CD entry point, ENV=cloud only: pin IMAGE_TAG, run migrations, deploy, roll back on failure"
	@echo "  k8s-rollout-check   Wait for app rollouts; restore changed revisions on failure"
	@echo "  k8s-down            Delete Kubernetes resources for current overlay"
	@echo "  k8s-apply           Run migrations, roll out API/frontend, then update ingress"
	@echo "  k8s-delete          Delete overlay resources"
	@echo "  k8s-render          Render and preview Kustomize manifests without applying"
	@echo "  k8s-build           Build local API, migrations, frontend and Grafana images"
	@echo "  k8s-init-secrets    Generate $(K8S_SECRETS) and Keycloak env files (gitignored) if missing, local only"
	@echo "  k8s-status          Display status of pods, services, statefulsets, and PVCs"
	@echo "  k8s-smoke           Check the frontend and API through the HTTPS ingress"
	@echo "  k8s-restart         Rollout restart the API and frontend deployments"
	@echo "  k8s-logs            Stream logs from the API pods"
	@echo "  k8s-logs-db         Stream logs from the PostgreSQL pod"
	@echo "  k8s-logs-migrations Show logs from the migrations Job"
	@echo "  k8s-logs-keycloak   Stream logs from the Keycloak pod"
	@echo "  k8s-forward-api     Port-forward API deployment to localhost:8081 (bypasses Ingress)"
	@echo "  k8s-forward-frontend Port-forward frontend to localhost:3000"
	@echo "  k8s-forward-db      Port-forward PostgreSQL service to localhost:5432"
	@echo "  k8s-smoke-keycloak  Check Keycloak through the ingress at $(KEYCLOAK_URL), local only"
	@echo "  k8s-load-test       Run k6 CPU load test on LOAD_TEST_PATH=$(LOAD_TEST_PATH) to trigger HPA scale-up"
	@echo "  k8s-watch-hpa       Watch HPA and API pods while the load test runs"
	@echo "  k8s-rollout-ok      Roll out a healthy revision: surge a new pod, then remove the old one"
	@echo "  k8s-rollout-broken  Roll out a revision whose readiness never passes, then undo it (ROLLOUT_AUTO_UNDO=0 to keep it stuck)"
	@echo "  k8s-rollout-undo    Roll back the API to the previous revision"
	@echo "  k8s-rollout-history Show the API revision history"
	@echo "  k8s-watch-rollout   Watch ReplicaSets and API pods during a rollout"
	@echo "  k8s-watch-traffic   Call the API through the Ingress every 0.5s and print the status codes"

# -----------------------------------------------------------------------------
# K3d Cluster Lifecycle
# -----------------------------------------------------------------------------
# Create local k3d cluster if it does not exist
cluster-create:
	@set -e; if k3d cluster list | grep -q "^$(CLUSTER_NAME)"; then \
		echo "Cluster '$(CLUSTER_NAME)' already exists."; \
		k3d cluster start $(CLUSTER_NAME) --timeout 120s; \
	else \
		echo "Creating cluster '$(CLUSTER_NAME)'..."; \
		k3d cluster create $(CLUSTER_NAME) \
			--servers 1 \
			--agents 1 \
			-p "$(HTTP_PORT):80@loadbalancer" \
			-p "8443:443@loadbalancer" \
			--wait; \
	fi
	$(MAKE) cluster-https-port
	@kubectl config use-context k3d-$(CLUSTER_NAME)

# Start stopped k3d cluster
cluster-start:
	@echo "Starting cluster '$(CLUSTER_NAME)'..."
	@k3d cluster start $(CLUSTER_NAME)
	@kubectl config use-context k3d-$(CLUSTER_NAME)

# Stop k3d cluster to free system resources
cluster-stop:
	@echo "Stopping cluster '$(CLUSTER_NAME)'..."
	@k3d cluster stop $(CLUSTER_NAME)

# Delete k3d cluster
cluster-delete:
	@echo "Deleting cluster '$(CLUSTER_NAME)'..."
	@k3d cluster delete $(CLUSTER_NAME)

# Reset cluster (delete and recreate)
cluster-reset: cluster-delete cluster-create

# Check k3d cluster status
cluster-status:
	@k3d cluster list

# Load local Docker images into the k3d cluster nodes
cluster-load:
	@echo "Importing images into cluster $(CLUSTER_NAME)..."
	@k3d image import $(IMAGE_NAME) $(MIGRATIONS_IMAGE_NAME) $(GRAFANA_IMAGE_NAME) $(FRONTEND_IMAGE_NAME) -c $(CLUSTER_NAME)

# -----------------------------------------------------------------------------
# Docker Compose
# -----------------------------------------------------------------------------
# Build and start all services in detached mode
up:
	$(COMPOSE) up --build -d

# Stop and remove all containers without deleting persistent data
down:
	$(COMPOSE) down

# Restart all running services without rebuilding their images
restart:
	$(COMPOSE) restart

# Stream logs from all services
logs:
	$(COMPOSE) logs -f

# Display the status of all services
ps:
	$(COMPOSE) ps

# Build or rebuild service images
build:
	$(COMPOSE) build

# Stop containers and remove all volumes (WARNING: deletes database data)
clean:
	$(COMPOSE) down -v

# -----------------------------------------------------------------------------
# Tests
# -----------------------------------------------------------------------------
# Run every unit test project, keep going after a failure so all results are reported
test-unit:
	@cd $(API_DIR) || exit 1; rc=0; for project in src/*.Tests/*.Tests.csproj; do \
		dotnet test "$$project" -c Release --logger trx --results-directory "$(abspath $(TEST_RESULTS))" || rc=1; \
	done; exit $$rc

# Blazor WebAssembly development (.NET SDK).
frontend-restore:
	dotnet restore $(FRONTEND_PROJECT)

frontend-dev:
	dotnet watch --project $(FRONTEND_PROJECT)

frontend-check:
	dotnet build $(FRONTEND_PROJECT) -c Release --no-restore

# -----------------------------------------------------------------------------
# Kubernetes (K8s)
# -----------------------------------------------------------------------------
# Generate overlay secrets.env, adding any missing password (reuses postgres.env password to keep existing PVC usable)
K8S_SECRETS = $(LOCAL_API_OVERLAY)/secrets.env
k8s-init-secrets:
	@[ "$(ENV)" = local ] || exit 0; \
	set -e; umask 077; touch $(K8S_SECRETS); \
	for key in POSTGRES_PASSWORD POSTGRES_MIGRATOR_PASSWORD POSTGRES_APP_PASSWORD; do \
		grep -q "^$$key=" $(K8S_SECRETS) && continue; \
		pw=; \
		[ "$$key" = POSTGRES_PASSWORD ] && [ -f $(COMPOSE_ENV_FILE) ] && pw=$$(sed -n 's/^POSTGRES_PASSWORD=//p' $(COMPOSE_ENV_FILE)); \
		[ -n "$$pw" ] || pw=$$(openssl rand -hex 32); \
		printf '%s=%s\n' "$$key" "$$pw" >> $(K8S_SECRETS); \
		echo "Added $$key to $(K8S_SECRETS)."; \
	done; \
	for entry in postgres.env:POSTGRES_USER=keycloak postgres.env:POSTGRES_PASSWORD= postgres.env:POSTGRES_DB=keycloak \
		keycloak.env:ADMIN_USERNAME=admin keycloak.env:ADMIN_PASSWORD=; do \
		file=$(KEYCLOAK_OVERLAY)/$${entry%%:*}; pair=$${entry#*:}; key=$${pair%%=*}; value=$${pair#*=}; \
		touch "$$file"; \
		grep -q "^$$key=" "$$file" && continue; \
		[ -n "$$value" ] || value=$$(openssl rand -hex 32); \
		printf '%s=%s\n' "$$key" "$$value" >> "$$file"; \
		echo "Added $$key to $$file."; \
	done

# Build Docker images for local Kubernetes cluster
k8s-build:
	docker build --target api -t $(IMAGE_NAME) $(API_DIR)
	docker build --target migrations -t $(MIGRATIONS_IMAGE_NAME) $(API_DIR)
	docker build -t $(GRAFANA_IMAGE_NAME) $(GRAFANA_DIR)
	docker build -t $(FRONTEND_IMAGE_NAME) $(FRONTEND_DIR)

# Build linux/amd64 images and push them to the registry for cloud clusters
k8s-push:
	docker buildx build --platform linux/amd64 --target api -t $(REGISTRY)/mydisk-api:$(IMAGE_TAG) --push $(API_DIR)
	docker buildx build --platform linux/amd64 --target migrations -t $(REGISTRY)/mydisk-api-migrations:$(IMAGE_TAG) --push $(API_DIR)
	docker buildx build --platform linux/amd64 -t $(REGISTRY)/mydisk-frontend:$(IMAGE_TAG) --push $(FRONTEND_DIR)

# Render manifests to inspect generated YAML (prints Secret data)
k8s-render: k8s-init-secrets k8s-init-tls
	kubectl kustomize $(K8S_OVERLAY)

# Update existing ingress routes before replacing the frontend image.
# Jobs are immutable: recreate the migration Job for each deployment.
k8s-apply: k8s-init-secrets k8s-init-tls
	@set -e; \
	baseline=; \
	for deployment in $(APP_DEPLOYMENTS); do \
		revision=$$($(KUBECTL) get "$$deployment" --ignore-not-found -o 'jsonpath={.metadata.annotations.deployment\.kubernetes\.io/revision}'); \
		baseline="$$baseline $$deployment=$${revision:-none}"; \
	done; \
	$(KUBECTL) delete job mydisk-api-migrations --ignore-not-found; \
	$(KUBECTL) apply -k $(K8S_OVERLAY) -l 'mydisk.io/deploy-after-migrations!=true,mydisk.io/deploy-after-frontend!=true'; \
	echo "Waiting for PostgreSQL migrations to complete..."; \
	$(KUBECTL) wait --for=condition=complete job/mydisk-api-migrations --timeout=$(K8S_MIGRATIONS_TIMEOUT); \
	existing_deployments=; \
	if [ "$(K8S_REFRESH_API_IMAGE)" = "1" ]; then \
		existing_deployments=$$($(KUBECTL) get $(APP_DEPLOYMENTS) --ignore-not-found -o name); \
	fi; \
	frontend=$$($(KUBECTL) get $(FRONTEND_DEPLOYMENT) --ignore-not-found -o name); \
	if [ -n "$$frontend" ]; then \
		$(KUBECTL) apply -k $(K8S_OVERLAY) -l 'mydisk.io/deploy-after-frontend=true'; \
	fi; \
	$(KUBECTL) apply -k $(K8S_OVERLAY) -l 'mydisk.io/deploy-after-migrations=true'; \
	for deployment in $$existing_deployments; do \
		$(KUBECTL) rollout restart "$$deployment"; \
	done; \
	$(MAKE) k8s-rollout-check K8S_ROLLBACK_REVISIONS="$$baseline"; \
	$(KUBECTL) apply -k $(K8S_OVERLAY) -l 'mydisk.io/deploy-after-frontend=true'
	@echo "Frontend after rollout is $(API_URL); API documentation: $(API_URL)/swagger"

# Delete Kubernetes overlay resources
k8s-delete: k8s-init-secrets k8s-init-tls
	$(KUBECTL) delete --ignore-not-found -k $(K8S_OVERLAY)

# Full setup: build images, ship them to the cluster, apply manifests, roll out the rebuilt image
ifeq ($(ENV),local)
k8s-up: k8s-preflight
	$(MAKE) k8s-build
	$(MAKE) cluster-load
	$(MAKE) k8s-preflight
	$(MAKE) k8s-apply K8S_REFRESH_API_IMAGE=1
	$(KUBECTL) rollout restart $(GRAFANA_DEPLOYMENT)
	$(KUBECTL) rollout status $(GRAFANA_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT)
	$(KUBECTL) rollout status $(KEYCLOAK_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT)
	$(MAKE) k8s-smoke
	$(MAKE) k8s-smoke-keycloak
else
k8s-up: k8s-push
	$(MAKE) k8s-deploy
endif

# Alias to delete overlay resources
k8s-down: k8s-delete

# Generate a gitignored overlay on top of the cloud overlay, pinning all application images to IMAGE_TAG
k8s-release-overlay:
	@[ "$(ENV)" = cloud ] || { echo "k8s-release-overlay requires ENV=cloud."; exit 1; }
	@mkdir -p $(RELEASE_OVERLAY)
	@printf '%s\n' \
		'apiVersion: kustomize.config.k8s.io/v1beta1' \
		'kind: Kustomization' \
		'resources:' \
		'  - ../cloud' \
		'images:' \
		'  - name: $(REGISTRY)/mydisk-api' \
		'    newTag: "$(IMAGE_TAG)"' \
		'  - name: $(REGISTRY)/mydisk-api-migrations' \
		'    newTag: "$(IMAGE_TAG)"' \
		'  - name: $(REGISTRY)/mydisk-frontend' \
		'    newTag: "$(IMAGE_TAG)"' > $(RELEASE_OVERLAY)/kustomization.yml
	@rendered=$$(kubectl kustomize $(RELEASE_OVERLAY)); \
	for image in mydisk-api mydisk-api-migrations mydisk-frontend; do \
		echo "$$rendered" | grep -q "image: $(REGISTRY)/$$image:$(IMAGE_TAG)$$" || { \
			echo "Image $(REGISTRY)/$$image:$(IMAGE_TAG) not found in the rendered manifests: REGISTRY must match the image names in the cloud overlays."; \
			exit 1; \
		}; \
	done
	@echo "Release overlay pins images to $(REGISTRY)/*:$(IMAGE_TAG)."

# CD entry point: deploy already pushed images, run migrations first, roll back the API if its rollout fails
k8s-deploy:
	@[ "$(ENV)" = cloud ] || { echo "k8s-deploy requires ENV=cloud, use k8s-up locally."; exit 1; }
	$(MAKE) k8s-release-overlay
	$(MAKE) k8s-apply K8S_OVERLAY=$(RELEASE_OVERLAY)
	$(KUBECTL) annotate $(APP_DEPLOYMENTS) kubernetes.io/change-cause="deploy $(IMAGE_TAG)" --overwrite

# A deployment run passes its baseline revisions, so unchanged workloads are
# never downgraded. A standalone check only rolls back deployments that fail.
k8s-rollout-check:
	@status=0; failed_deployments=; \
	for deployment in $(APP_DEPLOYMENTS); do \
		if ! $(KUBECTL) rollout status "$$deployment" --timeout=$(ROLLOUT_TIMEOUT); then \
			status=1; failed_deployments="$$failed_deployments $$deployment"; \
		fi; \
	done; \
	if [ "$$status" != 0 ]; then \
		echo "Application rollout failed; restoring changed deployments with an existing baseline."; \
		$(KUBECTL) get rs,pods -l 'app in (mydisk-api,mydisk-frontend)'; \
		$(KUBECTL) get events --field-selector reason=Unhealthy --sort-by=.lastTimestamp | tail -n 5; \
		for deployment in $(APP_DEPLOYMENTS); do \
			baseline=; \
			for entry in $(K8S_ROLLBACK_REVISIONS); do \
				case "$$entry" in "$$deployment="*) baseline=$${entry#*=} ;; esac; \
			done; \
			if [ "$$baseline" = none ]; then \
				echo "$$deployment was newly created; no previous revision exists."; \
			elif [ -n "$$baseline" ]; then \
				current=$$($(KUBECTL) get "$$deployment" -o 'jsonpath={.metadata.annotations.deployment\.kubernetes\.io/revision}'); \
				if [ "$$current" != "$$baseline" ]; then \
					$(KUBECTL) rollout undo "$$deployment" --to-revision="$$baseline" && \
					$(KUBECTL) rollout status "$$deployment" --timeout=$(ROLLOUT_TIMEOUT); \
				fi; \
			else \
				case " $$failed_deployments " in \
					*" $$deployment "*) $(KUBECTL) rollout undo "$$deployment" && \
						$(KUBECTL) rollout status "$$deployment" --timeout=$(ROLLOUT_TIMEOUT) ;; \
				esac; \
			fi; \
		done; \
		exit 1; \
	fi

# Complete local developer lifecycle: start cluster and deploy everything
dev-up: cluster-create
	$(MAKE) k8s-up

# Complete teardown: remove deployed resources and stop cluster
dev-down: k8s-down
	$(MAKE) cluster-stop

# Display status of all deployed Kubernetes resources
k8s-status:
	$(KUBECTL) get nodes
	$(KUBECTL) get pods,services,statefulsets,deployments,pvc

# A Ready node can still reject every pod because its Docker disk is full.
k8s-preflight:
	$(KUBECTL) wait --for=condition=Ready nodes --all --timeout=60s
	@set -e; conditions=$$($(KUBECTL) get nodes -o 'jsonpath={range .items[*]}{.metadata.name}{" "}{range .status.conditions[?(@.type=="DiskPressure")]}{.status}{end}{"\n"}{end}'); \
	pressure=$$(printf '%s\n' "$$conditions" | awk '$$2 == "True" {print $$1}'); \
	if [ -n "$$pressure" ]; then \
		echo "Kubernetes cannot schedule pods: DiskPressure on $$pressure" >&2; \
		echo "Check Docker storage with 'docker system df', free build cache or increase Docker's disk, then wait for DiskPressure to clear before retrying." >&2; \
		exit 1; \
	fi

# Exercise both Traefik backends through the public application URL.
k8s-smoke:
	@set -eu; set --; \
	if [ "$(ENV)" = local ]; then set -- --cacert "$$(mkcert -CAROOT)/rootCA.pem"; fi; \
	html=$$(curl "$$@" --fail --silent --show-error --retry 5 --retry-delay 2 --retry-all-errors --connect-timeout 5 --max-time 20 "$(API_URL)/"); \
	printf '%s' "$$html" | grep -Fq '_framework/blazor.webassembly.js' || { echo "The ingress did not return the Blazor application." >&2; exit 1; }; \
	health=$$(curl "$$@" --fail --silent --show-error --retry 5 --retry-delay 2 --retry-all-errors --connect-timeout 5 --max-time 20 "$(API_URL)/health/ready"); \
	[ "$$health" = Healthy ] || { echo "The API readiness endpoint did not return Healthy." >&2; exit 1; }; \
	content_type=$$(curl "$$@" --fail --silent --show-error --retry 5 --retry-delay 2 --retry-all-errors --connect-timeout 5 --max-time 20 --output /dev/null --write-out '%{content_type}' "$(API_URL)/api/Disk/disks"); \
	case "$$content_type" in application/json*) ;; *) echo "The collection endpoint did not return JSON." >&2; exit 1 ;; esac; \
	echo "Frontend and API verified through $(API_URL)."

# Rollout restart application deployments
k8s-restart:
	$(KUBECTL) rollout restart $(APP_DEPLOYMENTS)

# Stream logs from API deployment
k8s-logs:
	$(KUBECTL) logs -l app=mydisk-api -f --tail=100

# Stream logs from PostgreSQL statefulset
k8s-logs-db:
	$(KUBECTL) logs -l app=postgres -f --tail=100

# Show logs from the migrations Job
k8s-logs-migrations:
	$(KUBECTL) logs job/mydisk-api-migrations --tail=100

# Stream logs from the Keycloak deployment
k8s-logs-keycloak:
	$(KUBECTL) logs -l app=keycloak -f --tail=100

# Forward local port 8081 to API deployment (8080 is taken by the k3d load balancer)
k8s-forward-api:
	$(KUBECTL) port-forward deployment/mydisk-api 8081:8080

# Preview the static frontend independently of Traefik.
k8s-forward-frontend:
	$(KUBECTL) port-forward svc/mydisk-frontend 3000:8080

# Forward local port 5432 to PostgreSQL service
k8s-forward-db:
	$(KUBECTL) port-forward svc/postgres 5432:5432

# KC_HOSTNAME pins every redirect to KEYCLOAK_URL, so a port-forward would not work.
k8s-smoke-keycloak:
	@set -eu; \
	metadata=$$(curl --cacert "$$(mkcert -CAROOT)/rootCA.pem" --fail --silent --show-error --retry 10 --retry-delay 3 --retry-all-errors --connect-timeout 5 --max-time 20 \
		"$(KEYCLOAK_URL)/realms/master/.well-known/openid-configuration"); \
	issuer=$$(printf '%s' "$$metadata" | sed -n 's/.*"issuer":"\([^"]*\)".*/\1/p'); \
	[ "$$issuer" = "$(KEYCLOAK_URL)/realms/master" ] || { echo "Keycloak issuer is '$$issuer', expected $(KEYCLOAK_URL)/realms/master: check KC_HOSTNAME." >&2; exit 1; }; \
	redirect=$$(curl --fail --silent --show-error --connect-timeout 5 --max-time 20 --output /dev/null --write-out '%{redirect_url}' \
		"http://$(KEYCLOAK_HOST):$(HTTP_PORT)/admin"); \
	[ "$$redirect" = "$(KEYCLOAK_URL)/admin" ] || { echo "Keycloak HTTP redirect is '$$redirect', expected $(KEYCLOAK_URL)/admin." >&2; exit 1; }; \
	echo "Keycloak verified, admin console: $(KEYCLOAK_URL)/admin"

# Run the k6 load test through the Ingress to push CPU above the HPA target
k8s-load-test:
	k6 run -e BASE_URL=$(API_URL) -e TARGET_PATH='$(LOAD_TEST_PATH)' $(LOAD_TEST_SCRIPT)

# Watch HPA decisions and API pods being created during the load test
k8s-watch-hpa:
	@while true; do clear; $(KUBECTL) get hpa mydisk-api; echo; $(KUBECTL) top pods -l app=mydisk-api 2>/dev/null; echo; $(KUBECTL) get pods -l app=mydisk-api; sleep 3; done

# Roll out a new healthy revision: maxSurge adds a pod, the old one leaves once the new one is Ready
k8s-rollout-ok:
	$(KUBECTL) patch $(API_DEPLOYMENT) --type merge -p \
		'{"metadata":{"annotations":{"kubernetes.io/change-cause":"demo: healthy rollout"}},"spec":{"template":{"metadata":{"annotations":{"mydisk.io/rollout-demo":"'"$$(date +%s)"'"}}}}}'
	$(KUBECTL) rollout status $(API_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT)

# Roll out a revision whose readiness probe never passes; the old pods keep serving, then the rollout is undone
k8s-rollout-broken:
	$(KUBECTL) patch $(API_DEPLOYMENT) --type strategic -p \
		'{"metadata":{"annotations":{"kubernetes.io/change-cause":"demo: broken readiness on $(ROLLOUT_BROKEN_PATH)"}},"spec":{"template":{"spec":{"containers":[{"name":"api","readinessProbe":{"httpGet":{"path":"$(ROLLOUT_BROKEN_PATH)"}}}]}}}}'
	@if $(KUBECTL) rollout status $(API_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT); then \
		echo "Rollout unexpectedly succeeded."; \
	else \
		echo ""; \
		echo "Rollout did not complete: the new pod never became Ready, the previous pods kept serving traffic."; \
		$(KUBECTL) get rs,pods -l app=mydisk-api; \
		echo ""; \
		$(KUBECTL) get events --field-selector reason=Unhealthy --sort-by=.lastTimestamp | tail -n 5; \
		if [ "$(ROLLOUT_AUTO_UNDO)" = 1 ]; then \
			echo ""; \
			echo "Rolling back to the previous revision..."; \
			$(KUBECTL) rollout undo $(API_DEPLOYMENT); \
			$(KUBECTL) rollout status $(API_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT); \
		else \
			echo ""; \
			echo "Rollout left stuck on purpose. Run 'make k8s-rollout-undo' to roll back."; \
		fi; \
	fi

# Roll back the API to the previous revision
k8s-rollout-undo:
	$(KUBECTL) rollout undo $(API_DEPLOYMENT)
	$(KUBECTL) rollout status $(API_DEPLOYMENT) --timeout=$(ROLLOUT_TIMEOUT)

# Show the API revision history with change causes
k8s-rollout-history:
	$(KUBECTL) rollout history $(API_DEPLOYMENT)

# Watch ReplicaSets and pods: the new ReplicaSet scales up while the old one scales down
k8s-watch-rollout:
	@while true; do clear; $(KUBECTL) get $(API_DEPLOYMENT); echo; $(KUBECTL) get rs -l app=mydisk-api; echo; $(KUBECTL) get pods -l app=mydisk-api -o wide; sleep 2; done

# Call the API through the Ingress to show that a rollout never interrupts traffic
k8s-watch-traffic:
	@url=$(API_URL)/health/ready; while true; do \
		printf '%s %s\n' "$$(date +%T)" "$$(curl -s -o /dev/null -w '%{http_code}' --max-time 2 $$url)"; \
		sleep 0.5; \
	done

# -----------------------------------------------------------------------------
# Infrastructure (OpenTofu)
# -----------------------------------------------------------------------------
# Create or update the AKS cluster (the only cloud-specific stack)
infra-platform:
	$(TOFU) -chdir=$(INFRA_PLATFORM) init -input=false
	$(TOFU) -chdir=$(INFRA_PLATFORM) apply -var cluster_name=$(AKS_CLUSTER_NAME)

# Merge credentials from the Azure platform into the local kubeconfig.
infra-credentials:
	az aks get-credentials --overwrite-existing \
		--resource-group $$($(TOFU) -chdir=$(INFRA_PLATFORM) output -raw resource_group_name) \
		--name $$($(TOFU) -chdir=$(INFRA_PLATFORM) output -raw cluster_name)

# Configure Kubernetes through its API, using the selected context.
infra-cluster:
	$(TOFU) -chdir=$(INFRA_CLUSTER) init -input=false
	$(TOFU) -chdir=$(INFRA_CLUSTER) apply -var-file=../platform/azure/ingress.tfvars -var kube_context=$(INFRA_CONTEXT)

infra-up: infra-platform
	$(MAKE) infra-credentials
	$(MAKE) infra-cluster

# Release Kubernetes resources before removing their hosting platform.
infra-down:
	$(TOFU) -chdir=$(INFRA_CLUSTER) destroy -var-file=../platform/azure/ingress.tfvars -var kube_context=$(INFRA_CONTEXT)
	$(TOFU) -chdir=$(INFRA_PLATFORM) destroy -var cluster_name=$(AKS_CLUSTER_NAME)

# Check formatting and validate all stacks without touching any backend
infra-check:
	$(TOFU) fmt -check -recursive infra
	@for stack in $(INFRA_PLATFORM)/bootstrap $(INFRA_PLATFORM) $(INFRA_CLUSTER) $(INFRA_FRONTEND); do \
		$(TOFU) -chdir=$$stack init -backend=false -input=false >/dev/null && \
		$(TOFU) -chdir=$$stack validate || exit 1; \
	done

# Local TLS only: never changes the cloud cluster or reruns database migrations.
k8s-init-tls:
	@set -e; if [ "$(ENV)" = local ]; then \
		bash deploy/k8s/local/init-tls.sh "$(LOCAL_API_OVERLAY)/.tls" localhost 127.0.0.1 ::1; \
		bash deploy/k8s/local/init-tls.sh "$(KEYCLOAK_OVERLAY)/.tls" "$(KEYCLOAK_HOST)"; \
	fi

cluster-https-port:
	@set -e; \
	ports=$$(docker port k3d-$(CLUSTER_NAME)-serverlb 443/tcp 2>/dev/null || true); \
	if [ -z "$$ports" ]; then \
		k3d node edit k3d-$(CLUSTER_NAME)-serverlb --port-add "8443:443"; \
	elif ! printf '%s\n' "$$ports" | grep -q ':8443$$'; then \
		echo "Port 443 is already mapped to another host port: $$ports"; exit 1; \
	fi

local-https: cluster-https-port
	$(MAKE) k8s-init-tls ENV=local
	$(MAKE) k8s-init-secrets ENV=local
	@set -e; rendered=$$(mktemp); trap 'rm -f "$$rendered"' EXIT; \
	kubectl kustomize deploy/k8s/local > "$$rendered"; \
	kubectl --context k3d-$(CLUSTER_NAME) -n default apply -f "$$rendered" --dry-run=client >/dev/null
	kubectl --context k3d-$(CLUSTER_NAME) -n default create secret tls mydisk-api-local-tls --cert=$(LOCAL_API_OVERLAY)/.tls/tls.crt --key=$(LOCAL_API_OVERLAY)/.tls/tls.key --dry-run=client -o yaml | kubectl --context k3d-$(CLUSTER_NAME) -n default apply -f -
	@set -e; \
	kubectl --context k3d-$(CLUSTER_NAME) -n default apply -f $(LOCAL_API_OVERLAY)/https-redirect.yml; \
	frontend=$$(kubectl --context k3d-$(CLUSTER_NAME) -n default get deployment mydisk-frontend --ignore-not-found -o name); \
	if [ -n "$$frontend" ]; then \
		kubectl --context k3d-$(CLUSTER_NAME) -n default apply -f $(API_DIR)/k8s/base/same-origin-api.yml; \
		kubectl --context k3d-$(CLUSTER_NAME) -n default apply -k deploy/k8s/local -l 'mydisk.io/deploy-after-frontend=true'; \
	else \
		kubectl --context k3d-$(CLUSTER_NAME) -n default apply -f $(LOCAL_API_OVERLAY)/https.yml; \
		kubectl --context k3d-$(CLUSTER_NAME) -n default patch ingress mydisk-api --type=merge --patch-file=$(LOCAL_API_OVERLAY)/api-ingress-http.yml; \
	fi
	@echo "Local API: https://localhost:8443/swagger/index.html"

# Security checks: same commands locally and in GitHub Actions.
TRIVY ?= trivy
API_IMAGE ?= $(IMAGE_NAME)
MIGRATIONS_IMAGE ?= $(MIGRATIONS_IMAGE_NAME)
FRONTEND_IMAGE ?= $(FRONTEND_IMAGE_NAME)
SECURITY_PLATFORM ?= linux/amd64

.PHONY: security security-dependencies security-build security-images
# Run sequentially even when make is invoked with -j; never publish or deploy.
security:
	$(MAKE) security-dependencies
	$(MAKE) security-build
	$(MAKE) security-images

security-dependencies:
	@mkdir -p security-reports
	@rm -f security-reports/nuget.json
	@status=0; \
	(cd $(API_DIR) && dotnet restore mydisk-api.sln --force-evaluate) > security-reports/nuget-restore.txt 2>&1 || status=$$?; \
	cat security-reports/nuget-restore.txt; \
	exit $$status
	(cd $(API_DIR) && dotnet list mydisk-api.sln package --vulnerable --include-transitive --no-restore --format json) > security-reports/nuget.json

# Build once for the deployment architecture; scanning and pushing reuse these tags.
security-build:
	docker build --pull --platform $(SECURITY_PLATFORM) --target api -t "$(API_IMAGE)" $(API_DIR)
	docker build --pull --platform $(SECURITY_PLATFORM) --target migrations -t "$(MIGRATIONS_IMAGE)" $(API_DIR)
	docker build --pull --platform $(SECURITY_PLATFORM) -t "$(FRONTEND_IMAGE)" $(FRONTEND_DIR)

security-images:
	@command -v "$(TRIVY)" >/dev/null || { echo 'Trivy is required. On macOS: brew install trivy'; exit 1; }
	@mkdir -p security-reports
	@status=0; \
	for target in api migrations frontend; do \
		case "$$target" in api) image="$(API_IMAGE)" ;; migrations) image="$(MIGRATIONS_IMAGE)" ;; frontend) image="$(FRONTEND_IMAGE)" ;; esac; \
		rm -f "security-reports/$$target.json"; \
		echo "Scanning $$image: full report, then blocking check"; \
		if ! "$(TRIVY)" image --image-src docker --scanners vuln --timeout 10m \
			--format json --output "security-reports/$$target.json" --exit-code 0 "$$image"; then \
			status=1; continue; \
		fi; \
		"$(TRIVY)" image --image-src docker --scanners vuln --timeout 10m \
			--severity HIGH,CRITICAL --ignore-unfixed --exit-code 1 "$$image" || status=1; \
	done; \
	exit $$status
