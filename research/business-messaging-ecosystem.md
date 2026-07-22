# Business Messaging Ecosystem

## Goal

Understand how modern messaging platforms are used as interfaces between businesses and users, and whether messenger-based interaction is relevant to Narayana.

Related research: [Conversational AI and Messaging Assistants](conversational-ai-messaging-assistants.md).

## Scope and method

Research date: 16 July 2026.

The document separates:

- platform capabilities documented by Telegram and Meta;
- observed business usage reported by the platform owners;
- findings from the [Communication Channels Audit](communication-channels-audit.md);
- product hypotheses for Narayana.

---

## Business messaging

Messaging applications are used for more than personal communication. Businesses use them for:

- customer support;
- product discovery and recommendations;
- bookings;
- order and delivery updates;
- notifications;
- account-related workflows;
- purchases and payments.

Meta reported in April 2025 that more than two billion people use WhatsApp daily and that millions of them chat with businesses. Examples given by Meta include booking bus tickets, receiving delivery updates and paying utility bills.

Source: [Meta — Ways To Manage Your Businesses Chats On WhatsApp](https://about.fb.com/news/2025/04/ways-to-manage-your-businesses-chats-on-whatsapp/).

---

## Telegram

### Interaction model

Telegram bots and Mini Apps can function as applications inside the messenger rather than only as automated conversations.

Telegram documents support for:

- bots;
- Mini Apps;
- custom keyboards and inline buttons;
- free-form text and file exchange;
- API integrations;
- AI chatbots;
- authentication;
- push notifications;
- payments;
- subscription and monetization features.

Telegram states that its Bot Platform hosts more than 10 million bots. It also reports that more than 500 million users interact with Mini Apps each month.

Sources: [Telegram bot platform](https://core.telegram.org/bots), [Telegram bot features](https://core.telegram.org/bots/features), [Telegram Mini Apps](https://core.telegram.org/bots/webapps).

### Payments constraint

Telegram requires digital goods and services sold inside Telegram apps to use Telegram Stars. Cryptocurrency cannot replace Stars for these in-app digital purchases. External payment and account flows would therefore need to be designed with Telegram's platform rules in mind.

Source: [Telegram Bot Payments for Digital Goods and Services](https://core.telegram.org/bots/payments-stars).

### Typical role

Telegram is technically suited to workflows that resemble a standalone product:

- selecting a service;
- displaying complex menus or forms;
- opening a Mini App;
- checking account information;
- integrating external APIs;
- delivering files or QR codes;
- completing supported payments;
- contacting automated or human support.

These are platform capabilities, not evidence that Narayana users want all of these actions inside Telegram.

---

## WhatsApp Business

### Interaction model

WhatsApp Business is organized around a conversation with a company. The interaction generally remains recognizable as a chat even when parts of it are automated.

Verified business uses include:

- answering customer questions;
- recommending products;
- capturing leads;
- booking appointments;
- sending order or delivery updates;
- browsing business catalogs;
- completing payments where the feature is available;
- escalating from automation to business staff.

Meta introduced Business AI for eligible small businesses in India in 2026. It can answer questions around the clock, recommend products, capture leads, book appointments and support sales inside WhatsApp Business without requiring the business to write code.

Sources: [Meta — Business AI on WhatsApp in India](https://about.fb.com/news/2026/05/introducing-business-ai-on-whatsapp-for-small-businesses-in-india/), [Meta — WhatsApp business conversations](https://about.fb.com/news/2025/04/ways-to-manage-your-businesses-chats-on-whatsapp/).

### Regional evidence

#### India

Meta cites a 2025 Kantar study reporting that 91% of online adults in India chat with a business weekly. This supports the use of messaging as a routine business interaction channel in that market.

Source: [Meta — Business AI on WhatsApp in India](https://about.fb.com/news/2026/05/introducing-business-ai-on-whatsapp-for-small-businesses-in-india/).

#### Brazil

WhatsApp supports a comparatively complete commerce flow in Brazil. Meta documents business discovery, product and service browsing, carts and in-chat payments to local small businesses.

Sources: [Meta — Payments to small businesses in Brazil](https://about.fb.com/news/2023/04/pay-small-businesses-in-brazil-on-whatsapp/), [Meta — Finding and buying from businesses](https://about.fb.com/news/2022/11/find-and-buy-from-businesses-on-whatsapp/).

#### Other markets

Meta has documented business discovery through WhatsApp in Brazil, Indonesia, Mexico, Colombia and the United Kingdom. Feature availability and the completeness of the customer journey differ by country, so the existence of WhatsApp Business does not imply the same interaction model in every region.

Source: [Meta — Finding and buying from businesses](https://about.fb.com/news/2022/11/find-and-buy-from-businesses-on-whatsapp/).

---

## Telegram and WhatsApp compared

| Dimension | Telegram | WhatsApp Business |
|---|---|---|
| Primary interaction model | Bot or embedded Mini App | Conversation with a business |
| Product-like interface | Strong Mini App support | Chat-led workflows with business tools and automation |
| Automation | Bot API, Mini Apps, AI integrations | Business automation and Business AI where available |
| Payments | Bot and Mini App payments subject to Telegram rules | In-chat payments available only in supported markets |
| Discovery | Bot links, Telegram search, channels and Mini App Store | Business profiles, links, ads and business search where available |
| Human support | Can be integrated into bot or business workflows | Natural fit for agent conversation and automation handoff |
| Regional consistency | Core bot platform is broadly consistent | Business and payment features vary more by market |

---

## Privacy-industry comparison

The [Communication Channels Audit](communication-channels-audit.md) found that the reviewed privacy-focused companies primarily use:

- websites and first-party applications for product access;
- documentation, help centers, email and ticket forms for support;
- Reddit, Discord, Matrix, GitHub, Telegram channels or first-party communities for updates and discussion.

No reviewed competitor was verified as using a Telegram or WhatsApp bot as the main interface for purchasing and managing telecom services.

Adjacent patterns do exist:

- Narayana and Proton maintain official Telegram presences;
- Proton uses an AI assistant in its web support flow;
- Signal has a one-way announcement chat inside Signal;
- Session and SimpleX use their own messengers for community interaction;
- SimpleX provides bot-development APIs.

---

## Potential Narayana workflows

The platforms could technically support Narayana-related workflows such as:

- selecting an eSIM or virtual number;
- viewing service availability and pricing;
- checking balance;
- topping up an account;
- receiving setup instructions;
- delivering an eSIM QR code;
- viewing service status;
- opening a support request;
- escalating to a human operator.

These are product hypotheses. The research does not establish which workflows users want in a messenger, whether Telegram or WhatsApp would produce higher conversion, or whether platform payment and privacy constraints make the full flow appropriate.

---

## Current conclusions

- Meta documents business messaging at large scale on WhatsApp; this establishes a material interaction pattern on that platform, not universal user preference.
- Telegram and WhatsApp support materially different product models.
- Telegram can host complex bot and Mini App workflows that resemble standalone applications.
- WhatsApp Business emphasizes conversational interaction with companies and has particularly strong documented business usage in India and commerce capabilities in Brazil.
- WhatsApp feature availability varies by market.
- Privacy-focused competitors still keep purchasing and account management primarily on websites or in first-party applications.
- A messenger-first telecom interface was not found as an established pattern among the reviewed competitors.
- A Narayana assistant is therefore a plausible product hypothesis, but platform choice requires internal customer geography, acquisition and support data.
