FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

COPY src/NRN.Web/NRN.Web.csproj src/NRN.Web/
RUN dotnet restore src/NRN.Web/NRN.Web.csproj

COPY src/NRN.Web/ src/NRN.Web/
RUN dotnet publish src/NRN.Web/NRN.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "NRN.Web.dll"]
