FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY NuGet.Config .
COPY Framework/IdentityServer/IdentityServer.csproj Framework/IdentityServer/
COPY Framework/Admin/Admin.Data/Admin.Data.csproj Framework/Admin/Admin.Data/
COPY Framework/Base/CoreType/CoreType.csproj Framework/Base/CoreType/
COPY Framework/Base/CoreData/CoreData.csproj Framework/Base/CoreData/
COPY Framework/Admin/Admin.Type/Admin.Type.csproj Framework/Admin/Admin.Type/
COPY Framework/Base/CoreSvc/CoreSvc.csproj Framework/Base/CoreSvc/
RUN dotnet restore Framework/IdentityServer/IdentityServer.csproj
COPY Framework Framework
WORKDIR /src/Framework/IdentityServer
RUN dotnet build IdentityServer.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish IdentityServer.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
