# Telegram Mini App — real data replacement checklist

This file is the source of truth for every demo or local-MVP data boundary that must be replaced when access to the existing Narayana bot/backend becomes available.

## Verified public data

Source: <https://narayana.im/about/prices>  
Last checked: 2026-07-16

| Product | Public price | Public qualification |
| --- | ---: | --- |
| Virtual Numbers | from EUR 10 / month | Country-dependent; mobile and toll-free numbers; voice and SMS; 20+ countries |
| eSIM | from EUR 9 one-time | Data and voice; instant digital delivery |
| Physical SIM | EUR 14 one-time | Delivery is not included; data, calls and SMS |

Public support contacts found on the same page:

- Telegram: <https://t.me/narayanaim>
- Email: `support@narayana.im`

No verified public Feedback URL has been found.

## Demo-only data currently in the app

- Preview Telegram user (`TelegramUserId = 0`).
- Preview balance (`12 EUR`).
- Preview eSIM subscription and local service status.
- Local SQLite accounts, balances, subscriptions and purchase ledger.
- Demo purchase that deducts the first visible charge from the preview balance.
- Telegram Stars product prices until official product ids and commercial prices are supplied.

These values must never be presented as live Narayana account data.

## Backend contracts required

- Account lookup by validated Telegram user id.
- Current balance and currency.
- Active SIM/eSIM/virtual-number services.
- Service lifecycle status and status details.
- Country-level virtual number and eSIM availability.
- Authoritative current prices, taxes, delivery and discounts.
- Purchase creation, confirmation, cancellation and idempotency.
- Telegram Stars mapping, refunds and payment reconciliation.

## Code replacement points

Search the solution for `TODO(real-data)`.

- `NrnDatabaseInitializer`: remove preview account seed from production wiring.
- `SqliteNrnServicesStore`: replace local account/service/purchase storage with the bot backend.
- `HttpNrnServicesApi`: point the client boundary to the real authenticated service API.
- `ServiceDetails`: enable Telegram checkout only after authoritative product and price mapping exists.
- `TelegramOptions.FeedbackUrl`: configure the verified destination.
- `UsePublicNarayanaCatalog` migration: do not reuse its demo-ledger cleanup as a production migration.

## Telegram production setup still required

- Real `Telegram:BotToken` stored outside source control.
- Public HTTPS domain.
- Mini App URL configured in BotFather.
- `Open App` button connected in the existing bot.
- Real webhook and secret for payments.
- QA inside a real Telegram WebView in light and dark themes.

## Completion rule

Real-data integration is complete only when demo user `0` is isolated to `/demo`, every authenticated response comes from the authoritative backend, catalog prices can be reconciled with the public page, and purchase/status changes are verified end to end in Telegram.
