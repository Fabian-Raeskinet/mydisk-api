
COMPOSE = docker compose --env-file .env
ENV ?= local
K8S_OVERLAY = k8s/overlays/$(ENV)
IMAGE_NAME = mydisk-api:dev
MIGRATIONS_IMAGE_NAME = mydisk-api-migrations:dev
CLUSTER_NAME ?= mydisk
HTTP_PORT ?= 8080

.PHONY: help up down restart logs ps build clean \
        cluster-create cluster-start cluster-stop cluster-delete cluster-reset cluster-status cluster-load \
        k8s-help k8s-init-secrets k8s-build k8s-render k8s-apply k8s-delete \
        k8s-status k8s-restart k8s-logs k8s-logs-db k8s-logs-migrations k8s-forward-api k8s-forward-db k8s-up k8s-down \
        dev-up dev-down

# -----------------------------------------------------------------------------
# Default & Help
# -----------------------------------------------------------------------------
help:
	@echo "Usage: make [target] [ENV=local] [CLUSTER_NAME=mydisk] [HTTP_PORT=8080]"
	@echo ""
	@echo "K3d Cluster targets:"
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
	@echo "Kubernetes targets (default ENV=local):"
	@echo "  k8s-up              Build images, import to k3d, prepare secrets, apply overlay, restart API"
	@echo "  k8s-down            Delete Kubernetes resources for current overlay"
	@echo "  k8s-apply           Apply overlay with Kustomize (reruns migrations Job)"
	@echo "  k8s-delete          Delete overlay resources"
	@echo "  k8s-render          Render and preview Kustomize manifests without applying"
	@echo "  k8s-build           Build local Docker images ($(IMAGE_NAME), $(MIGRATIONS_IMAGE_NAME))"
	@echo "  k8s-init-secrets    Generate $(K8S_OVERLAY)/secrets.env (gitignored) if missing"
	@echo "  k8s-status          Display status of pods, services, statefulsets, and PVCs"
	@echo "  k8s-restart         Rollout restart the API deployment"
	@echo "  k8s-logs            Stream logs from the API pods"
	@echo "  k8s-logs-db         Stream logs from the PostgreSQL pod"
	@echo "  k8s-logs-migrations Show logs from the migrations Job"
	@echo "  k8s-forward-api     Port-forward API deployment to localhost:8081 (bypasses Ingress)"
	@echo "  k8s-forward-db      Port-forward PostgreSQL service to localhost:5432"

# -----------------------------------------------------------------------------
# K3d Cluster Lifecycle
# -----------------------------------------------------------------------------
# Create local k3d cluster if it does not exist
cluster-create:
	@if k3d cluster list | grep -q "^$(CLUSTER_NAME)"; then \
		echo "Cluster '$(CLUSTER_NAME)' already exists."; \
		k3d cluster start $(CLUSTER_NAME) 2>/dev/null || true; \
	else \
		echo "Creating cluster '$(CLUSTER_NAME)'..."; \
		k3d cluster create $(CLUSTER_NAME) \
			--servers 1 \
			--agents 1 \
			-p "$(HTTP_PORT):80@loadbalancer" \
			--wait; \
	fi
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
	@k3d image import $(IMAGE_NAME) $(MIGRATIONS_IMAGE_NAME) -c $(CLUSTER_NAME)

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
# Kubernetes (K8s)
# -----------------------------------------------------------------------------
# Generate overlay secrets.env if missing (reuses .env password to keep existing PVC usable)
K8S_SECRETS = $(K8S_OVERLAY)/secrets.env
k8s-init-secrets:
	@if [ ! -f $(K8S_SECRETS) ]; then \
		pw=$$( [ -f .env ] && sed -n 's/^POSTGRES_PASSWORD=//p' .env ); \
		[ -n "$$pw" ] || pw=$$(openssl rand -base64 32 | tr -d '/+=' | cut -c1-32); \
		umask 077; printf 'POSTGRES_PASSWORD=%s\n' "$$pw" > $(K8S_SECRETS); \
		echo "Created $(K8S_SECRETS) (mode 600)."; \
	fi

# Build Docker images for local Kubernetes cluster
k8s-build:
	docker build --target api -t $(IMAGE_NAME) .
	docker build --target migrations -t $(MIGRATIONS_IMAGE_NAME) .

# Render manifests to inspect generated YAML (prints Secret data)
k8s-render: k8s-init-secrets
	kubectl kustomize $(K8S_OVERLAY)

# Apply Kubernetes overlay manifests (Jobs are immutable: recreate it to rerun migrations)
k8s-apply: k8s-init-secrets
	kubectl delete job mydisk-api-migrations --ignore-not-found
	kubectl apply -k $(K8S_OVERLAY)
	@echo "API available at http://localhost:$(HTTP_PORT)"

# Delete Kubernetes overlay resources
k8s-delete:
	kubectl delete --ignore-not-found -k $(K8S_OVERLAY)

# Full local setup: build images, import to cluster, ensure secrets, apply manifests, roll out the rebuilt :dev image
k8s-up: k8s-init-secrets k8s-build cluster-load k8s-apply k8s-restart

# Alias to delete overlay resources
k8s-down: k8s-delete

# Complete local developer lifecycle: start cluster and deploy everything
dev-up: cluster-create k8s-up

# Complete teardown: remove deployed resources and stop cluster
dev-down: k8s-down cluster-stop

# Display status of all deployed Kubernetes resources
k8s-status:
	kubectl get pods,services,statefulsets,deployments,pvc

# Rollout restart the API deployment
k8s-restart:
	kubectl rollout restart deployment/mydisk-api

# Stream logs from API deployment
k8s-logs:
	kubectl logs -l app=mydisk-api -f --tail=100

# Stream logs from PostgreSQL statefulset
k8s-logs-db:
	kubectl logs -l app=postgres -f --tail=100

# Show logs from the migrations Job
k8s-logs-migrations:
	kubectl logs job/mydisk-api-migrations --tail=100

# Forward local port 8081 to API deployment (8080 is taken by the k3d load balancer)
k8s-forward-api:
	kubectl port-forward deployment/mydisk-api 8081:8080

# Forward local port 5432 to PostgreSQL service
k8s-forward-db:
	kubectl port-forward svc/postgres 5432:5432
