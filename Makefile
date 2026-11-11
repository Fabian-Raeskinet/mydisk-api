
COMPOSE = docker compose --env-file .env

.PHONY: up down restart logs ps build clean

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
