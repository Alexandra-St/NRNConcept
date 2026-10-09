# Architecture and data boundaries

Implementation baseline: `687de16`, audited 9 October 2026. This describes the code, not an intended production system.

## Project boundaries

`app/NRNConcept/NRNConcept.sln` contains two independent ASP.NET Core Blazor Web Apps and two xUnit v3 test projects. Both apps use Interactive Server rendering, so browser actions run through a server-side circuit. There is no separate SPA backend or MVC application.

| Project | Responsibilities | State and persistence |
| --- | --- | --- |
| NRN.Web | Learning pages, Finder rules/results, simulation scene rendering and outcomes | JSON resources; scoped state, no application database |
| NRN.Telegram | Browser preview, Telegram session validation, catalog/accounts/services, prototype purchases and Stars endpoints | Scoped preferences/session state; EF Core SQLite database |
| NRN.Web.Tests | Content/state/rule tests | Test fixtures and production resource copies |
| NRN.Telegram.Tests | Validation, localization, API-client and database/payment-store tests | Synthetic data and temporary test databases |

Each application has its own shared UI folder and CSS. The design directory supplies tokens, component guidance and canonical icons; no shared compiled UI project exists. Web localization is JSON-backed; Mini App localization uses C# dictionaries.

## Request and deployment flow

The Nginx gateway forwards root routes to Web and `/miniapp/` to Telegram, including WebSocket upgrade headers for Blazor. Telegram uses configurable `PathBase`; both apps process forwarded headers. `/health` endpoints confirm application startup; gateway health confirms Nginx only. A gateway waiting page handles some upstream cold-start errors, so a successful gateway health check is not sufficient proof of a working module.

Web registers Learning, Simulations and Finder via feature-specific dependency injection. Learning and simulation validators check content before the app starts. JSON resources are copied into publish output. Finder rules live in SolutionFinderService, while scoped state stores current answers. SimulationState follows scene transitions, records selected outcomes and supports restart.

Mini App startup creates a data directory, applies checked-in migrations and seeds the catalog plus synthetic preview account. Services endpoints expose public catalog reads and authenticated user-service reads/purchases. The authenticated ID is derived from server-validated `X-Telegram-Init-Data`, not caller-supplied account IDs. HttpNrnServicesApi is the HTTP boundary; PreviewNrnServicesApi calls the local store for synthetic sessions.

## Telegram entry points

Browser `/demo` presents a fixed simulated bot transcript. Its Open App control opens the same Mini App pages in an iframe with `demo=1`; the other command buttons have no handlers. Direct browser `/app` access also enters preview without initData. These are demonstration entry points, not a bot implementation or fake real authentication.

An authorized Telegram WebView can supply initData through the official JS bridge. TelegramInitDataValidator verifies the HMAC using a server-held bot token, compares hashes in fixed time, checks auth_date freshness and parses identity only after validation. Invalid data is rejected; missing data enters preview. Service APIs revalidate headers. The permitted timestamp skew is five minutes; the default maximum age is 60 minutes.

No live authorized bot, company backend or telecom provisioning was verified. The local database remains the backend even for validated Telegram sessions. Real service access is a planned integration, with a [replacement checklist](../app/NRNConcept/docs/TELEGRAM_REAL_DATA_TODO.md).

## Payments

The local store checks currency and balance and updates balance, purchase and subscription in one database transaction. These changes represent prototype ledger behavior, not provisioned service delivery. Stars code creates invoices through Telegram Bot API and receives webhooks guarded by a configured secret; payment-store tests cover matching and idempotency. Stars prices are empty in checked-in configuration. Real payment processing, refunds and reconciliation are not verified and must not be advertised as live.

## Privacy and data boundaries

Educational content is accessible without an account. LearningState, SimulationState and SolutionFinderState are scoped in-memory objects: no reading-history table or persistent questionnaire profile is implemented. This still uses server-side execution; answers and UI interactions reach the server over the Blazor connection. State may remain while a circuit is retained, and hosting infrastructure can log IP addresses/request metadata.

No custom analytics integration or learning browser storage was found. Both applications call UseAntiforgery; avoid claiming that the site is cookie-free. The Mini App stores language in localStorage and a same-site cookie for one year and loads JavaScript from Telegram's domain. Real sessions process Telegram identity on the server; SQLite stores user IDs, balances, subscriptions, purchase and payment records. Do not describe the whole ecosystem as collecting no personal data.

Preview uses synthetic user `0`, seeded in the same local database. Preview purchases can persist and be shared across browser sessions. A dedicated per-session demo store would require a later behavior change; it is not implemented by this documentation task.

## Configuration

Non-secret settings include HTTPS redirection, PathBase, SQLite connection path, public catalog source/date and support destinations. Optional bot and webhook credentials belong in environment secrets, never repository files. The catalog's July 2026 verification date is historical evidence, not an up-to-date commercial price guarantee.

Compose persists SQLite in a named volume. The Render free-tier blueprint declares no persistent disk, so it should not be relied upon for durable account/payment storage. Use an authorized production design before accepting real data or payments; deployment configuration is unchanged here.
