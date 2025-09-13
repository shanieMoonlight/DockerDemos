# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Apps/Movies/Movies.API/Movies.API.csproj", "Apps/Movies/Movies.API/"]
COPY ["Apps/Movies/Movies.Shared/Movies.Shared.csproj", "Apps/Movies/Movies.Shared/"]
COPY ["Apps/Movies/Movies.Persistence.Cache.Memory/Movies.Persistence.Cache.Memory.csproj", "Apps/Movies/Movies.Persistence.Cache.Memory/"]
COPY ["Apps/Movies/Movies.Persistence.Cache.Redis/Movies.Persistence.Cache.Redis.csproj", "Apps/Movies/Movies.Persistence.Cache.Redis/"]
COPY ["Apps/Movies/Movies.Persistence.Ef/Movies.Persistence.Ef.csproj", "Apps/Movies/Movies.Persistence.Ef/"]
RUN dotnet restore "./Apps/Movies/Movies.API/Movies.API.csproj"
COPY . .
WORKDIR "/src/Apps/Movies/Movies.API"
RUN dotnet build "./Movies.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Movies.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Movies.API.dll"]


#  docker build -t movies-api-kst:latest -f Apps/Movies/Movies.API/Dockerfile  .