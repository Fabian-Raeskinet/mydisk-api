FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["global.json", "Nuget.config", "./"]

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Contracts/Contracts.csproj", "src/Contracts/"]
COPY ["src/Contracts.Validators/Contracts.Validators.csproj", "src/Contracts.Validators/"]
COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/Data.Configurations/Data.Configurations.csproj", "src/Data.Configurations/"]
COPY ["src/Data.Migrations/Data.Migrations.csproj", "src/Data.Migrations/"]
COPY ["src/DependencyInjection.Configurations/DependencyInjection.Configurations.csproj", "src/DependencyInjection.Configurations/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/RandomServices/RandomServices.csproj", "src/RandomServices/"]
COPY ["src/RetryService/RetryService.csproj", "src/RetryService/"]
COPY ["src/Services/Services.csproj", "src/Services/"]

RUN dotnet restore src/Api/Api.csproj \
 && dotnet restore src/Data.Migrations/Data.Migrations.csproj

COPY ["src/Api/", "src/Api/"]
COPY ["src/Contracts/", "src/Contracts/"]
COPY ["src/Contracts.Validators/", "src/Contracts.Validators/"]
COPY ["src/Data/", "src/Data/"]
COPY ["src/Data.Configurations/", "src/Data.Configurations/"]
COPY ["src/Data.Migrations/", "src/Data.Migrations/"]
COPY ["src/DependencyInjection.Configurations/", "src/DependencyInjection.Configurations/"]
COPY ["src/Domain/", "src/Domain/"]
COPY ["src/RandomServices/", "src/RandomServices/"]
COPY ["src/RetryService/", "src/RetryService/"]
COPY ["src/Services/", "src/Services/"]

FROM build AS bundle-migrations
RUN dotnet tool install --tool-path /tools dotnet-ef --version 10.0.*
RUN /tools/dotnet-ef migrations bundle \
      --project src/Data.Migrations \
      --startup-project src/Data.Migrations \
      --configuration Release \
      --output /app/efbundle

FROM build AS publish-api
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrations
WORKDIR /app
COPY --from=bundle-migrations ["/app/efbundle", "."]
USER $APP_UID
ENTRYPOINT ["./efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
COPY --from=publish-api ["/app/publish", "."]
USER $APP_UID
ENTRYPOINT ["dotnet", "MyDisks.Api.dll"]
