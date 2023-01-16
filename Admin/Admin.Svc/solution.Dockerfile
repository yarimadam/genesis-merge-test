FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY NuGet.Config .
COPY Framework/Admin/Admin.Svc/Admin.Svc.csproj Framework/Admin/Admin.Svc/
COPY Framework/Base/CoreSvc/CoreSvc.csproj Framework/Base/CoreSvc/
COPY Framework/Base/CoreType/CoreType.csproj Framework/Base/CoreType/
COPY Framework/Base/CoreData/CoreData.csproj Framework/Base/CoreData/
COPY Framework/Admin/Admin.Type/Admin.Type.csproj Framework/Admin/Admin.Type/
COPY Framework/Admin/Admin.Data/Admin.Data.csproj Framework/Admin/Admin.Data/
RUN dotnet restore Framework/Admin/Admin.Svc/Admin.Svc.csproj
COPY Framework Framework
WORKDIR /src/Framework/Admin/Admin.Svc
RUN dotnet build Admin.Svc.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Admin.Svc.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Admin.Svc.dll"]
