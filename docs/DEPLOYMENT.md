# Deployment verification

## Checked-in deployment

`render.yaml` defines three free-tier Docker web services in Frankfurt: Web, Mini App and Nginx gateway. The gateway points at the two Render upstreams. Docker Compose uses the same application Dockerfiles with internal service names, publishes port 8080 and gives the Mini App a named SQLite volume. No deployment settings were changed during portfolio preparation.

## Observed on 9 October 2026

The gateway root, Finder, simulations and `/miniapp/demo` returned HTTP 200 with Blazor markup. Both upstream health endpoints returned app-specific healthy JSON. Privacy Lab was rendered independently in a browser. Free-tier cold starts can delay page loads; the gateway includes a waiting/retry page. A gateway health response proves only the proxy is listening.

No Telegram BotFather settings, bot credentials, live payments, external account service or telecom provisioning were accessed. No redeployment was triggered. Screenshot evidence comes from local published builds, not a claim that every production route was fully exercised.

## Docker limitation

Standalone `docker-compose config --quiet` passes on the audit host. The `docker compose` plugin is unavailable and the daemon is stopped, so image build and `docker compose up --build` are not verified here. CI defines a Compose image build, but that configuration alone is not proof of a passing run. Once a Docker engine and Compose plugin are available, verify the existing entry point and gateway port 8080 before using it as the sole setup path.

## Operational gaps

Render configuration has no persistent disk for Mini App SQLite. Accounts and payment records may be lost when service storage is replaced. Browser preview user 0 is shared and persisted locally. Current Stars prices are empty. Production bot/backend wiring, authoritative pricing, refunds and real WebView QA remain required. Historical public catalog checks must be refreshed before any commercial use.
