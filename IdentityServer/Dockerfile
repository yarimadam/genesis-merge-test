FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY NuGet.Config .
COPY IdentityServer/IdentityServer.csproj IdentityServer/
COPY Admin/Admin.Data/Admin.Data.csproj Admin/Admin.Data/
COPY Admin/Admin.Type/Admin.Type.csproj Admin/Admin.Type/
RUN dotnet restore IdentityServer/IdentityServer.csproj
COPY Admin Admin
COPY IdentityServer IdentityServer
WORKDIR /src/IdentityServer
RUN dotnet build IdentityServer.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish IdentityServer.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]