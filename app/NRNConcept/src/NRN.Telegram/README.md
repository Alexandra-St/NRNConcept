# NRN Telegram Mini App

Independent prototype; not an official or deployed Narayana application. Browser preview uses synthetic persisted data. Real company accounts, service provisioning and authorized live payments are not connected. See the [architecture boundary](../../../../docs/ARCHITECTURE.md).

The app runs as a .NET 9 Blazor Web App with Interactive Server rendering.

## Local configuration

Keep the Telegram bot token outside source control. Configure it with .NET user secrets:

```shell
dotnet user-secrets set "Telegram:BotToken" "<bot-token>"
```

For deployed environments, use the `Telegram__BotToken` environment variable.

Support destinations are optional in demo mode and must be absolute HTTPS URLs in production:

```shell
dotnet user-secrets set "Telegram:SupportUrl" "https://t.me/<support-destination>"
dotnet user-secrets set "Telegram:FeedbackUrl" "https://<feedback-form-url>"
```

`t.me` and `telegram.me` destinations open inside Telegram. Other HTTPS destinations use Telegram's external `openLink` flow. Invalid, non-HTTPS, relative, or credential-bearing URLs are rejected at startup.

If the app is opened in a regular browser without Telegram `initData`, it stays in preview mode. If Telegram sends `initData`, the app requires a valid signature and an `auth_date` no older than `Telegram:MaxAgeMinutes`.

## Services API boundary

The app exposes `GET /api/v1/catalog` and authenticated `GET /api/v1/services`. The services endpoint derives the current user only from a freshly validated `X-Telegram-Init-Data` header; it does not accept a Telegram user ID from the route or query string.

Authenticated UI sessions use `HttpNrnServicesApi` to call these endpoints. `UserServicesState` also rejects a response whose user ID does not match the validated session. Browser preview uses `PreviewNrnServicesApi` directly and never sends or exposes a real Telegram user ID.

Products and user subscriptions are stored in SQLite through EF Core. The default database is created at `Data/nrn-telegram.db` under the application's content root; override it with the `ConnectionStrings__NrnServices` environment variable. Prices are stored in integer minor units and subscriptions are isolated by the composite key `(TelegramUserId, ProductId)`.

The visible catalog uses the public Narayana price page as its historical source (checked 16 July 2026; not reverified current pricing): Virtual Numbers from `10 EUR/month`, eSIM from `9 EUR` one-time, and Physical SIM `14 EUR` one-time. Account balances, subscriptions and purchases remain explicit demo data until the existing bot/backend API is available. See [`docs/TELEGRAM_REAL_DATA_TODO.md`](../../docs/TELEGRAM_REAL_DATA_TODO.md) for the replacement checklist.

Purchases are balance-backed and executed in one database transaction: the server verifies the authenticated Telegram user, checks the account currency and balance, deducts the connection fee, records a purchase, and creates the subscription atomically. Preview user `0` receives a demo balance of `12 EUR`; real Telegram users start with `0 EUR` until an external payment provider credits their account.

## Telegram Stars

Stars payments are disabled until every production price is configured explicitly. Set a webhook secret and integer `XTR` price per product:

```bash
dotnet user-secrets set "TelegramPayments:WebhookSecret" "<random-secret>"
dotnet user-secrets set "TelegramPayments:StarsPrices:virtual-numbers" "<integer-stars-price>"
dotnet user-secrets set "TelegramPayments:StarsPrices:esim" "<integer-stars-price>"
dotnet user-secrets set "TelegramPayments:StarsPrices:physical-sim" "<integer-stars-price>"
```

For a future authorized setup, the webhook route is `/api/telegram/webhook` (under `/miniapp` when PathBase is enabled) with the same `secret_token`. This configured code path has not been verified with live payments. The server creates `XTR` invoice links, validates the Telegram user at pre-checkout, and activates a service only after a matching `successful_payment` webhook. The Telegram charge id is stored for idempotency and future refunds.

## Localization

The Mini App has Russian and English UI dictionaries, including catalog data, service states, errors, purchase confirmation, and Stars invoices. A saved language overrides Telegram's `language_code`; first-time users with an English Telegram locale receive English, while other locales currently fall back to Russian. The choice is persisted in local storage and a same-site cookie so server-rendered pages use the correct language immediately.

The app applies the checked-in migrations on startup and seeds the product catalog. Only the internal preview user (`TelegramUserId = 0`) receives sample subscriptions; authenticated Telegram users start with an empty service list until they complete a purchase.
