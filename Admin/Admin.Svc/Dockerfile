FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY NuGet.Config .
COPY Admin/Admin.Svc/Admin.Svc.csproj Admin/Admin.Svc/
COPY Admin/Admin.Type/Admin.Type.csproj Admin/Admin.Type/
COPY Admin/Admin.Data/Admin.Data.csproj Admin/Admin.Data/
RUN dotnet restore Admin/Admin.Svc/Admin.Svc.csproj
COPY Admin Admin
WORKDIR /src/Admin/Admin.Svc
RUN dotnet build Admin.Svc.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Admin.Svc.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Admin.Svc.dll"]