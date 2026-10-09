# Conversational AI and Messaging Assistants

## Goal

Assess whether users are comfortable interacting with AI assistants, which telecom tasks are suitable for conversational interfaces, and whether a Telegram-based Narayana assistant has practical value.

## Scope and method

Research date: 16 July 2026.

The document separates:

- general customer-service behavior;
- verified examples from telecom operators;
- Telegram platform capabilities;
- product hypotheses for Narayana.

There is no public evidence that measures demand for a Narayana assistant or identifies Telegram as the preferred channel among Narayana customers.

---

## Reported acceptance of AI assistants

Zendesk's 2025 customer-experience survey found that:

- 67% of consumers were ready to delegate tasks such as order tracking and product recommendations to AI;
- 64% were more likely to trust AI agents that appeared friendly and empathetic.

Its 2026 research found that:

- 74% of consumers expected customer service to be available 24/7 because of AI;
- 76% would choose a company that allowed text, images and video to be used in the same conversation without restarting;
- 63% said their demand for transparency about AI use had increased.

These vendor-published survey results indicate stated acceptance of some AI-assisted service tasks. They are not observed behaviour and are not evidence about telecom, Telegram or Narayana users specifically. The underlying sample and methodology should be reviewed before using the percentages in a decision model.

Sources: [Zendesk CX Trends 2025](https://www.zendesk.com/newsroom/articles/2025-cx-trends-report/), [Zendesk CX Trends 2026](https://cxtrends.zendesk.com/).

### Expectations reported in the surveys

The research points to several recurring expectations:

- immediate response;
- 24/7 availability;
- continuity across the conversation;
- resolution rather than a generic answer;
- clear disclosure when AI is involved;
- access to a human when automation cannot complete the task.

The relevant measure is therefore not whether the assistant can produce a plausible reply, but whether it can resolve the user's task accurately and with low friction.

---

## AI inside business messaging

Meta reported more than one million weekly conversations with Business AIs in Mexico and the Philippines in early 2026. It also stated that these agents were being developed beyond answering product-availability questions toward completing tasks directly in WhatsApp.

This is evidence of real conversational AI usage inside a messenger. It does not establish global adoption or demand in privacy-focused telecom.

Source: [Meta — 2026: AI Drives Performance](https://about.fb.com/news/2026/01/2026-ai-drives-performance/).

---

## Telecom examples

### Vodafone TOBi and SuperTOBi

Vodafone reported in 2024 that TOBi handled nearly 45 million customer questions per month across 13 countries and 15 languages.

SuperTOBi adds generative AI for more natural and complex conversations. Vodafone also documents automatic transfer to a human agent, including a conversation summary so that the customer does not need to repeat the issue.

The assistant is used for service journeys such as appointments and billing. This demonstrates that conversational interfaces can operate at telecom scale and can support task resolution rather than only static FAQ answers.

Sources: [Vodafone — GenAI tools for customer experience](https://www.vodafone.com/news/newsroom/technology/vodafone-supercharging-customer-experience-with-microsoft-s-gen-ai-tools), [Vodafone — Meet SuperTOBi](https://www.vodafone.com/news/newsroom/technology/meet-super-tobi-vodafone-s-new-generative-ai-virtual-assistant-now-serving-customers-in-multiple-countries).

### Deutsche Telekom Frag Magenta

Deutsche Telekom's Frag Magenta is available through its website, MeinMagenta app, WhatsApp and selected voice channels. It provides 24/7 assistance and can access customer data for tasks such as:

- displaying and downloading bills;
- checking contract-extension eligibility;
- changing bank details;
- answering questions about contracts, SIM cards, connectivity and faults.

Deutsche Telekom reported more than four million customer dialogues in 2022 and stated that the assistant resolved more than one third of requests immediately. Its 2024 annual report describes the addition of generative AI to handle requests without a predefined script and unclear or ungrammatical wording.

Sources: [Deutsche Telekom — Frag Magenta channels](https://www.telekom.com/de/konzern/themenspecials/special-kundenservice/details/digitaler-serviceassistent-544878), [Deutsche Telekom — AI in customer service](https://www.telekom.com/en/company/digital-responsibility/details/artificial-intelligence-at-deutsche-telekom-1055154), [Deutsche Telekom Annual Report 2024](https://report.telekom.com/annual-report-2024/management-report/group-strategy/data-ai.html).

### What the telecom examples establish

They verify that conversational assistants can support:

- high-volume customer service;
- natural-language problem description;
- authenticated account information;
- account actions;
- troubleshooting;
- human handoff with retained context.

They do not verify that a third-party messenger is the best primary interface for Narayana. Vodafone primarily embeds its assistant in its own digital service environment; Deutsche Telekom uses several first- and third-party channels.

---

## Telegram as an assistant platform

Telegram supports:

- bots and natural-language assistants;
- buttons, menus and forms;
- Mini Apps with a complete graphical interface;
- authentication;
- API integrations;
- files and QR-code delivery;
- notifications;
- human-support escalation;
- payments subject to Telegram's platform rules.

Telegram states that its platform hosts more than 10 million bots and that more than 500 million users interact with Mini Apps each month. These figures demonstrate broad use of the platform, but do not reveal how frequently users interact with AI bots, which bot categories they use or whether they want telecom management through Telegram.

Sources: [Telegram Bot Platform](https://core.telegram.org/bots), [Telegram bot features](https://core.telegram.org/bots/features), [Telegram Mini Apps](https://core.telegram.org/bots/webapps).

### Payment constraint

Telegram requires digital goods and services sold inside Telegram apps to use Telegram Stars. A Narayana purchase flow inside Telegram would therefore require a policy and legal review; cryptocurrency cannot simply replace Stars for an in-app digital purchase.

Source: [Telegram payments for digital goods and services](https://core.telegram.org/bots/payments-stars).

---

## Where conversation adds value

Conversation is useful when the user:

- does not know which product is appropriate;
- describes a situation rather than a telecom specification;
- has several requirements that must be clarified;
- needs setup or troubleshooting guidance;
- wants a quick account action without navigating a dashboard;
- cannot find the correct documentation.

Example:

> I need a second number for WhatsApp that will work while I travel in Portugal.

An assistant could clarify:

- whether the user needs SMS, calls or data;
- whether the number must be long-term;
- which country code is required;
- whether WhatsApp compatibility is available and verified;
- which Narayana product matches the requirements.

This is more useful than exposing the user to a list of telecom terms without guidance.

---

## Where structured controls remain necessary

Conversation should not replace explicit interfaces for:

- exact prices and product comparison;
- payment details;
- transaction approval;
- security settings;
- account deletion;
- access to sensitive SMS content;
- irreversible account or telecom actions.

A safer interaction pattern is:

```text
Natural-language request
→ clarification
→ verified product and account data
→ structured options
→ explicit confirmation
→ action
```

The assistant may guide the journey, but the system should display authoritative data and require confirmation before sensitive or irreversible actions.

---

## Potential Narayana assistant workflows

### Product discovery

- explain the difference between SIM, eSIM, virtual number and SIP;
- ask clarifying questions;
- recommend only products supported by current inventory and rules;
- show verified coverage, availability and pricing.

### Number selection

- ask for country, number type, SMS and calling requirements;
- filter available numbers;
- state known service compatibility without unsupported guarantees.

### Account and balance

- display balance after authentication;
- explain recent charges;
- show active services;
- provide a controlled top-up flow.

### eSIM setup

- provide device-specific installation instructions;
- deliver the QR code through a protected flow;
- explain activation timing and roaming settings;
- troubleshoot common installation problems.

### Support

- diagnose common issues;
- use current documentation and service status;
- collect the information needed for a ticket;
- transfer the conversation to a human with its context preserved.

---

## Required system access

An assistant becomes a product interface only if it can use authoritative Narayana systems. Relevant integrations include:

- product catalog;
- live prices and availability;
- coverage information;
- number inventory;
- user authentication;
- balance and transaction history;
- purchase and activation state;
- eSIM status;
- payment status;
- documentation and service-status data;
- support tickets and human handoff.

Without these integrations, the assistant is primarily a conversational FAQ. It may improve navigation and support, but it cannot reliably manage telecom services.

---

## Risks and constraints

### Incorrect claims

An AI assistant must not guess whether a number works with a specific bank, messenger or verification system. Product compatibility should come from verified data and should include limitations.

### Authentication and authorization

Balance, SMS, QR codes and account actions require stronger controls than possession of a Telegram account alone. Sensitive actions need authentication, authorization and explicit confirmation.

### Privacy

The assistant introduces Telegram and potentially an AI provider into the interaction chain. Narayana would need to disclose:

- which data is processed;
- where it is processed;
- how long conversations are retained;
- whether conversation data is used for model training;
- which actions and content remain outside the assistant.

### Platform dependency

Telegram policies, account restrictions, API changes and regional availability can affect the product. The website and account dashboard should remain available as independent channels.

### Human escalation

The assistant needs a clear path to a person when confidence is low, a transaction fails, an answer depends on an exception or the request is security-sensitive.

---

## Assessment for Narayana

**Interpretation:** Telegram is technically capable of serving as an additional Narayana entry point because it supports conversation, structured controls, Mini Apps and API-backed actions in one interface.

**Hypothesis:** an initial assistant may be more useful for guided selection and support than as a replacement for the website or dashboard. Candidate tasks are:

1. product explanation and selection;
2. current availability and pricing;
3. balance and service status;
4. eSIM installation guidance;
5. first-line troubleshooting;
6. contextual transfer to human support.

The research does not establish:

- how many Narayana customers use Telegram;
- whether they prefer a bot to the website;
- which tasks they would entrust to the assistant;
- whether a Telegram-first purchase flow would improve conversion;
- whether the additional platform and privacy risks are acceptable.

These questions require Narayana's internal channel data and a limited user test. Until then, the Telegram assistant should be treated as a supported product hypothesis, not a validated user demand.
