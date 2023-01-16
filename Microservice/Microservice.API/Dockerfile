FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app
EXPOSE 5051

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY NuGet.Config .
COPY Microservice/Microservice.API/Microservice.API.csproj Microservice/Microservice.API/
COPY Microservice/Microservice.DataLib/Microservice.DataLib.csproj Microservice/Microservice.DataLib/
COPY Microservice/Microservice.TypeLib/Microservice.TypeLib.csproj Microservice/Microservice.TypeLib/
RUN dotnet restore Microservice/Microservice.API/Microservice.API.csproj
COPY Microservice Microservice
WORKDIR /src/Microservice/Microservice.API
RUN dotnet build Microservice.API.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Microservice.API.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Microservice.API.dll"]