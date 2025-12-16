# 1. Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 3. Copy csproj and restore
COPY ["CatalogService.API/CatalogService.API.csproj", "CatalogService.API/"]
RUN dotnet restore "CatalogService.API/CatalogService.API.csproj"

# 4. Copy everything and build
COPY . .
WORKDIR "/src/CatalogService.API"
RUN dotnet publish -c Release -o /app/publish

# 5. Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.API.dll"]
