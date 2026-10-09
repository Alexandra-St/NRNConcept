# Running the NRN applications

## Requirements and verified environment

Both applications target .NET 9. A matching SDK and runtime are required. Audit host: macOS arm64, SDK 9.0.303, runtime 9.0.7; dependencies were already restored. The default SDK 10 installation lacked the .NET 9 runtime, so a separate .NET 9 installation was selected with DOTNET_ROOT. Use the normal `dotnet` command when it resolves to a suitable installation.

From repository root, these commands were tested:

```sh
dotnet restore app/NRNConcept/NRNConcept.sln
dotnet test app/NRNConcept/NRNConcept.sln --configuration Release --no-restore
dotnet publish app/NRNConcept/src/NRN.Web/NRN.Web.csproj --configuration Release --no-restore --output /tmp/nrn-lab
dotnet publish app/NRNConcept/src/NRN.Telegram/NRN.Telegram.csproj --configuration Release --no-restore --output /tmp/nrn-miniapp
```

Dependency restore succeeded for all four projects. This used the host package cache and does not prove a completely empty-cache installation. Run restore first on a fresh clone.

## Published application launch

Run each published app from its output directory so its content and static assets resolve correctly. Local preview requires no bot token. The Mini App creates its Data directory, applies migrations and seeds synthetic preview data on startup. Launch in separate terminals:

```sh
cd /tmp/nrn-lab
HttpsRedirection__Enabled=false dotnet NRN.Web.dll --urls http://localhost:5081
```

```sh
cd /tmp/nrn-miniapp
HttpsRedirection__Enabled=false dotnet NRN.Telegram.dll --urls http://localhost:5082
```

These published launches were tested. Open http://localhost:5081/learn and http://localhost:5082/demo (or /app). The audit used /private/tmp/nrn-portfolio-lab and /private/tmp/nrn-portfolio-miniapp as equivalent output directories; adapt the paths to your environment. Windows users should set the same environment variable with their shell's syntax; that variant was not tested.

## Configuration

For local HTTP, set `HttpsRedirection__Enabled=false`. Optional `PathBase=/miniapp` is used behind the gateway; omit it when running Mini App directly. `ConnectionStrings__NrnServices` selects the SQLite file; default is Data/nrn-telegram.db under the application's content root. Preview changes can persist in this database.

Bot token, webhook secret and payment prices are optional experimental integration settings, not required for recruiter preview. Never paste real secrets into tracked configuration. See [Mini App notes](NRNConcept/src/NRN.Telegram/README.md).

## Docker Compose

The existing setup uses three services and gateway port 8080. Docker engine plus Compose plugin are required. Configuration validated with standalone `docker-compose config --quiet`; image build and startup remain unverified on this host. See [deployment verification](../docs/DEPLOYMENT.md) for the exact limitation.

## Troubleshooting

Missing net9.0 targeting packs/runtime: verify both SDK and runtime installations. On a host with multiple installations, DOTNET_ROOT must match the runtime used by test executables. A raw Release run can serve unstyled pages when static asset configuration is unavailable; use the published directory. SQLite directory must be writable. Render demo loading delays may be cold starts, while gateway health alone cannot confirm the apps are ready.
