# Base image for runtime (Linux-based)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the nuget.config file
RUN dotnet nuget locals all --clear
COPY nuget.config ./nuget.config

# Copy and restore dependencies for all projects
COPY ["CurrencyConverter/CurrencyConverter.API.csproj", "CurrencyConverter/"]
COPY ["CurrencyConverter.Application/CurrencyConverter.Application.csproj", "CurrencyConverter.Application/"]
COPY ["CurrencyConverter.Catching/CurrencyConverter.Catching.csproj", "CurrencyConverter.Catching/"]
COPY ["CurrencyConverter.Core/CurrencyConverter.Core.csproj", "CurrencyConverter.Core/"]
COPY ["CurrencyConverter.Infrastructure/CurrencyConverter.Infrastructure.csproj", "CurrencyConverter.Infrastructure/"]
RUN dotnet restore "./CurrencyConverter/CurrencyConverter.API.csproj" --configfile ./nuget.config

# Copy the entire source code and build the project
COPY . .
WORKDIR "/src/CurrencyConverter"
RUN dotnet build "./CurrencyConverter.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CurrencyConverter.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for Redis configuration
ENV Redis__ConnectionString="http://localhost:6379/, abortConnect=false"
ENV Redis__ApplicationCode="CC"
ENV Redis__Environment="DEV"
ENV Redis__UseReadReplica="false"

ENTRYPOINT ["dotnet", "CurrencyConverter.API.dll"]
