# Repository audit — 9 October 2026

## Baseline and scope

Audited GitHub `Alexandra-St/NRNConcept` and the clean local checkout at commit `687de16`. After fetching origin, `HEAD...origin/main` was 0 ahead / 0 behind. No tracked or untracked local changes were present. Work continues in a separate copied checkout on `docs/recruiter-portfolio`; the original checkout and synced project reference files remain unchanged. Eight reachable commits were inspected through their text blobs.

## Implementation inventory

| Area | Evidence | Actual status |
| --- | --- | --- |
| Privacy Lab | NRN.Web Learning pages, JSON content, startup validators, en/ru dictionaries | Implemented MVP |
| Solution Finder | Questionnaire, conditional follow-ups, deterministic rules, results and product details | Implemented MVP; no AI or checkout |
| Simulations | JSON scene graph, outcome state and specialized scene components | Two available scenarios: public-wifi and ordinary-day |
| Telegram Mini App | Separate Blazor app; dashboard, catalog, services, account, help and learning | Implemented browser prototype; real commercial integration unfinished |
| Bot demo | BotDemo.razor with fixed conversation and an iframe opened by Open App | Implemented presentation; command buttons are decorative, not a conversational bot |
| Telegram authentication | Server HMAC validation, freshness checks, protected service endpoints | Implemented and unit-tested; no authorized live bot verified |
| Payments | SQLite purchase transactions, Stars invoice/webhook code and idempotency store | Experimental; Stars prices empty, live payments not verified |
| Design system | CSS tokens, shared components within each app, canonical SVG icon library | Implemented assets; no shared compiled UI package |
| Backend | Blazor Interactive Server; Mini App minimal APIs, EF Core/SQLite migrations | Implemented local prototype, no authoritative company backend |

## Security and tracked files

Scanned all 365 reachable text blobs for private keys, GitHub tokens, Telegram-shaped tokens, AWS access keys and quoted credential assignments. The only candidate is an explicitly synthetic test token in TelegramInitDataValidatorTests. No confirmed credential was found by this limited pattern scan. This is not a comprehensive security certification; unreachable objects, local ignored secrets and private company systems were not audited.

Tracked configuration contains credential-free SQLite paths. No database, bin/obj output, .env or private key was tracked. .gitignore already excludes these common local artifacts. A tracked Google Docs .webloc shortcut exists: its destination permissions/content have not been verified. Review it before distributing the portfolio; no confidential document contents were retrieved or reproduced. Public support contacts and company product references are present in research; these are not evidence of company authorization.

## Verification before documentation changes

- Release compilation succeeded using installed .NET SDK 9.0.303.
- xUnit v3: 55 Web tests and 42 Telegram tests passed, zero failures/skips.
- The default dotnet installation is SDK 10 with no .NET 9 runtime. The local .NET 9 installation requires DOTNET_ROOT to point at it for test executables. This is a host setup issue.
- docker-compose config --quiet passed. Docker daemon is stopped; docker compose plugin is absent although standalone docker-compose exists. Container build/start is not yet verified.
- Render gateway, Finder, simulations and bot demo returned HTTP 200 with Blazor HTML. Both upstream health endpoints returned app-specific healthy JSON. One /learn request hit a transient DNS failure; the browser independently rendered Privacy Lab.
- HTTP success and server HTML do not prove all interactions or real Telegram payments work.

## Documentation gaps and outdated material

Root README is a Russian demo link plus an unqualified Docker command. No license file exists. Mini App README uses machine-specific executable paths. Simulation specification describes one scenario although two are implemented. Finder specification embeds an old 22-test result; current solution has 97 tests. Screen list and user flows mix English/Russian and include future screens as if current. Ecosystem text incorrectly suggests all modules have separate applications: Learning, Finder and simulations share NRN.Web.

Research consists of public desk research and hypotheses, not representative customer interviews or measured conversion improvements. Historical price checks date to July 2026 and must not be presented as current commercial quotations. Research is preserved, and planning artifacts will be explicitly distinguished from implementation evidence.

## Documentation plan

Rewrite the English README around verified MVP functionality and independent authorship; add a factual architecture/privacy/local setup overview; refresh module specifications and design maps; capture actual application screenshots; record deployment and test limitations; prepare repository metadata and a reviewable pull request without merging. Leave source code and deployment configuration unchanged. License selection remains an owner decision.
