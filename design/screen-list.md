# Screen inventory

[Complete original project document](../docs/archive/original-concept/design/screen-list.md) — preserved verbatim; this page describes the current implementation.

This separates current screens from the original L1 planning inventory. Baseline audited 9 October 2026.

## Telegram Mini App — implemented prototype

Dashboard (`/app`): greeting, quick actions, services, catalog, learning and help. My Services (`/app/services`): service list/status. Catalog (`/app/catalog`) and details: product cards, capabilities and prototype purchase options. Account and Language: identity/balance presentation and locale preference. Learn: educational entry points. Help: support destination. Browser bot presentation (`/demo`): scripted transcript and Open App iframe.

Dedicated price comparison and real service management/provisioning remain planned. The public gateway prefixes these paths with `/miniapp`.

## Educational Web — implemented MVP

Privacy Lab (`/learn`): hero, privacy notice and categories. Category (`/learn/{category}`): topics. Topic: situation, explanation, practical actions and relevant links. The original broad category list was a planning sketch; production categories and topics come from JSON.

Finder: Start, Questionnaire, Results and Product Details. Results explain why each product fits, limitations, alternatives and learning links.

Simulations: catalog, introduction, interactive scenes, explanation, takeaways and related exits. Two scenarios are available. Filtering, persistent Continue Learning, glossary/search and separate About screens remain planning items, not implemented routes.

## Visual requirements

Use existing monochrome foundations, orange accent, geometric illustrations, accessible labels and responsive layouts. Wireframes describe intended hierarchy; actual screenshots and code demonstrate delivered screens. No screen inventory implies full accessibility certification or company production integration.
