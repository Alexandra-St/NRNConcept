FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

COPY src/NRN.Telegram/NRN.Telegram.csproj src/NRN.Telegram/
RUN dotnet restore src/NRN.Telegram/NRN.Telegram.csproj

COPY src/NRN.Telegram/ src/NRN.Telegram/
RUN dotnet publish src/NRN.Telegram/NRN.Telegram.csproj \
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
RUN mkdir -p /app/Data && chown -R $APP_UID:$APP_UID /app/Data
USER $APP_UID
ENTRYPOINT ["dotnet", "NRN.Telegram.dll"]
