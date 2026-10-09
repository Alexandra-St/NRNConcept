# Interactive Simulations — MVP

[Complete original project document](archive/original-concept/docs/INTERACTIVE_SIMULATIONS_MVP.md) — preserved verbatim; this page describes the current implementation.

## Current status

Two scenarios are implemented in NRN.Web: `public-wifi` and `ordinary-day`. Both are marked Available in the production JSON resource and have English/Russian localization. This document retains the original Public Wi-Fi editorial requirements; the JSON scene graph and localized resources are the source of truth for exact rendered wording and transitions. Prior single-scenario scope is historical.

## Product role and promise

Continue learning after Privacy Lab by making decisions in a safe fictional situation and seeing consequences. This is not an exam: no right/wrong labels during scenes, no shame, no fear-based scoring. A 3–5 minute experience requires no account, personal profile or real contact information. Choices are held in server-side memory during the current circuit, not saved as a learning history.

The route sequence is `/simulations` → `/simulations/public-wifi` → introduction → scenes → explanation → takeaways. Related destinations are `/learn/everyday-situations/public-wifi`, `/finder/product/esim` and `/finder`.

## Connecting before departure

Slug `public-wifi`, estimated four minutes. Card question: can you connect without giving away more than you intended?

### Introduction: 18 minutes until boarding

At fictional Northstar Airport, mobile reception is weak and boarding is soon. The user needs to open an email containing a booking confirmation, with several free networks nearby. The task is to obtain the file while disclosing as little information as possible. The privacy notice explains that this is fictional and no real data is requested. Start opens the first scene.

### Scene 1: choosing a network

| Network | Signal | Security |
| --- | --- | --- |
| NORTHSTAR_FREE_WIFI | Strong | Open |
| Northstar Airport Guest | Medium | Open |
| FREE_AIRPORT_5G | Strong | Open |

All look plausible. Connecting to the strongest network records unverified-network risk. Choosing the official-looking name records trust in a name alone. Checking the sign confirms Northstar Airport Guest and records a verified-name action. All continue to an access page: a familiar name reduces accidental connection risk but does not prove subsequent pages are safe.

### Scene 2: the access page

A fictional `northstar-airport-access.com` page offers 30 minutes of Wi-Fi and requests email/phone, with small print about partner offers. Supplying primary contacts records disclosure of both. Separate contacts reduce linkage but do not establish trust. Looking for a registration-free option reveals Continue as guest and records data minimization. Access activates and the user opens email in a browser. No real form input is required.

### Scene 3: the browser warning

With 11 minutes left, a fictional certificate warning appears. Continuing records a critical ignored-warning outcome. Enabling a VPN and continuing records misplaced VPN trust: a VPN does not fix an invalid certificate or make a fake site authentic. Closing the page and switching to mobile data records leaving an untrusted connection.

Continuing leads to a suspicious login scene. Switching to mobile data leads to a safe fourth scene with a familiar, already configured mail app.

### Scene 4: unexpected login

The suspicious page says the session expired at fictional `mail-account-check.com`. The password manager offers no saved login. Entering credentials under time pressure records exposed-credentials risk. Checking the address and closing the page records a checked-domain action. Opening the mail app directly records a trusted-app action. Safe alternatives switch to mobile connectivity. The safe branch offers only the direct mail-app action.

The booking confirmation opens with seven minutes left. Show what happened opens the explanation.

## Explanation and takeaways

The result follows actual choices and shows consequences instead of a numerical grade. Safe results describe retaining control; moderate risk explains where the user stopped; critical risk explains how urgency created the trap. Outcome severity comes from the recorded choices, including critical certificate-warning decisions; exact headings come from localization resources.

| Decision | Explanation |
| --- | --- |
| Trusting a network name | Names can be copied; keep checking page addresses and warnings |
| Sharing primary contacts | Contact details may be linked or reused; consider whether disclosure is justified |
| Using separate contacts | Reduced linkage does not make a suspicious page safe |
| Ignoring a certificate warning | HTTPS identity failure is not repaired by trust or VPN |
| Unexpected login under time pressure | Stop, check the domain or open the known app directly |

The consequences table marks unchosen disclosures as prevented. Takeaways: verify the network, respect warnings, and pause before supplying a password. The main next step is the Privacy Lab Wi-Fi article. eSIM is a contextual connectivity option, with compatibility, coverage and tariff limitations; it does not create anonymity. Finder is an alternative next step.

## Ordinary Day extension

`/simulations/ordinary-day` follows ordinary location, cafe, social-content and ticket-booking decisions to show how small signals can combine. It uses a dedicated scene/results presentation while sharing the JSON content and scoped state infrastructure. This addition supersedes the original one-complete-simulation limit.

## Catalog, state and scope

The catalog now contains two active cards. Future teasers must be explicitly marked Coming soon and must not behave as working links. In-memory state tracks current scene, selected choice per scene, outcomes and completion; restart clears a run. Refresh may begin again. No persisted history or user profile is required.

Not implemented: administrative scenario authoring, universal editor, achievements/leaderboards, persistent progress, real email/phone/password analysis, real network/device checks, social sharing or broad event analytics. A data-driven scene graph is implemented; it should not be confused with a complete authoring platform.

## Acceptance requirements

A run reaches explanation without dead ends; decisions affect the explanation; scenes do not label answers correct/incorrect; no real personal data entry is requested; copy does not exaggerate HTTPS, VPN or eSIM; both locales work; keyboard/screen-reader behavior and mobile layout need continued QA; related educational/Finder exits resolve. Unit tests cover scene/state and production content validation. Current results are in [verification](VERIFICATION.md), not the historical implementation counts.
