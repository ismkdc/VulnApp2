FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app
COPY . .

RUN dotnet publish "VulnApp2/VulnApp2.csproj" -c Release --property:PublishDir=/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

USER app

WORKDIR /app
COPY --from=build-env /out .

ENTRYPOINT dotnet VulnApp2.dll