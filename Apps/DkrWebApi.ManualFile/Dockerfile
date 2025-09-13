
# Get the .Net image that we'll need to build and run the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Directory.Packages.props .
COPY ./Apps/DkrWebApi.ManualFile/DkrWebApi.ManualFile.csproj ./DkrWebApi.ManualFile/
#COPY ["DkrWebApi.ManualFile/DkrWebApi.ManualFile.csproj", "DkrWebApi.ManualFile/"]
RUN dotnet restore "./DkrWebApi.ManualFile/DkrWebApi.ManualFile.csproj"

# COPY . .
COPY ./DkrWebApi.ManualFile/ ./DkrWebApi.ManualFile/
COPY ./DockerDemos.sln .

RUN dotnet publish "./DkrWebApi.ManualFile/DkrWebApi.ManualFile.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
EXPOSE 8080
COPY --from=publish /app/publish ./app

ENV DOTNET_URLS="http://+:5000" #Instead of the default 8080

# Stay running so we can investigate the container
#WORKDIR /app
#CMD ["tail", "-f", "/dev/null"]
ENTRYPOINT ["dotnet", "app/DkrWebApi.ManualFile.dll"]













# docker build -t dkr_mnl:1.0.0  -f ./DkrWebApi.ManualFile/Dockerfile   .
# docker run --name dkr_mnl_ctr -d -p 999:8080 dkr_mnl:1.0.0


#THe port mapping, '-p 999:8080 worked and I'm not sure why because I never 