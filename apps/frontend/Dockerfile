FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src
COPY src/MyDisk.Frontend.csproj ./
RUN dotnet restore MyDisk.Frontend.csproj
COPY src/ ./
RUN dotnet publish MyDisk.Frontend.csproj --no-restore -c Release -o /app/publish

FROM nginxinc/nginx-unprivileged:1.30-alpine AS runtime
USER root
# Apply distribution security fixes released since the base image was published.
RUN apk upgrade --no-cache
ENV API_UPSTREAM=http://mydisk-api:8080 \
    NGINX_ENVSUBST_OUTPUT_DIR=/tmp \
    NGINX_ENTRYPOINT_LOCAL_RESOLVERS=1 \
    NGINX_ENVSUBST_FILTER="API_UPSTREAM|NGINX_LOCAL_RESOLVERS"
COPY nginx/nginx.conf /etc/nginx/nginx.conf
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
USER 101:101
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget -q -O /dev/null http://127.0.0.1:8080/frontend-health || exit 1
