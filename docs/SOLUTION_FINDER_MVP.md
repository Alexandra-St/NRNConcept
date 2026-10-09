# Solution Finder — MVP

## Current status and role

Implemented in NRN.Web: Start → Questionnaire → Results → Product Details, with links to related Privacy Lab content and official provider destinations. The implementation source is SolutionFinderService and scoped SolutionFinderState. This English specification preserves the product rules while replacing historical test counts with the current [verification report](VERIFICATION.md).

Finder helps users choose a relevant communication solution after understanding their problem. It uses five required questions and up to two follow-ups, takes no free text, requires no account, creates no profile and persists no run history. Equal answers produce equal results; recommendations must solve an identified need.

## Catalog and evidence boundary

| Product | Need | Benefit | Limitation |
| --- | --- | --- | --- |
| Virtual number | Keep primary contact separate from registrations, listings or work | Separates contexts | Does not protect all message content or metadata |
| eSIM | Mobile connectivity while travelling | Data without a physical SIM | Verify device support, coverage and current cost |
| SIP | Internet or international calls | Calls through a compatible SIP client | Internet/client setup and minute billing; emergency calling unconfirmed |

Physical SIM is outside the current Finder catalog, although it is present in the separate Mini App catalog. VPN is educational, not presented as a Narayana product. Provider compatibility with external VPN/proxy tools does not mean a provider-operated VPN exists.

The original public review was dated 13 July 2026. It recorded SIM/eSIM internet connectivity, international and SIP calls, inbound virtual-number calls/SMS, compatibility with external VPN/proxy tools, stated lack of mandatory KYC, and a EUR 51 initial balance payment. These are historical research observations, not a current price/service guarantee or independent validation of company claims. Exact eSIM coverage, roaming, emergency calling and concurrent SIP device support were not established. Those gaps must remain visible rather than being turned into promises.

Historical sources: [home](https://narayana.im/welcome), [FAQ](https://narayana.im/about/faq), and the previously cited `www4.narayana.im/about/pricing` address, which is not promoted as a verified current link. The Mini App uses a separate historical public catalog source/date; do not assume the catalogs match.

## Questionnaire

| Question | Options | Purpose |
| --- | --- | --- |
| Q1: What would you like to improve? Multiple choice | Safer internet away from home; avoid using personal number everywhere; travel connectivity; separate work/personal; protect accounts/passwords; explore | Identify goals and relevant follow-ups |
| Q2: Where do you use your primary number? Multiple choice | Registrations; listings/one-off contacts; work; close circle only; avoid sharing | Identify number separation needs |
| Q3: How often do you travel/live abroad? | Often; a few times a year; rarely/never | Identify travel context without persisting it |
| Q4: How do you use calls? Multiple choice | Personal; international; separate work number; internet app; rare calls | Distinguish SIP from second-number needs |
| Q5: What matters most? | Privacy; simplicity; connectivity across countries; work/life separation | Adjust explanation/order of relevant results, never create relevance alone |
| Q6: Separate number purpose? Conditional | Temporary registrations/contacts; ongoing personal communication; work/business; several purposes | Clarify second-number scenarios |
| Q7: Hardest travel challenge? Conditional | Suitable mobile internet; public Wi-Fi; keep usual number; none | Avoid recommending eSIM only because someone travels |

Q6 appears after registration/listing/work contexts. Q7 appears for frequent or several-times-yearly travel. Users can return and change answers.

## Decision rules

| Product | High-priority signal | Other relevant signals | Exclusion principle |
| --- | --- | --- | --- |
| Virtual number | Registration/listing use or applicable temporary-contact follow-up | Work contacts, number/work separation, separate work number | Close-circle-only use without another need does not justify it |
| eSIM | Frequent travel with a mobile internet need | Other relevant travel/connectivity combinations | Rare travel/no connectivity problem does not justify it |
| SIP | Internet-app calling | International calls | Registration-only number need does not justify SIP |

These summarize product intent; the code defines exact combinations. Recommendation explanations identify the triggering context. Virtual-number learning links include messaging-apps. eSIM always includes device/coverage/tariff checks. SIP requires internet and a configured client and does not promise emergency service.

## Control scenarios

| User situation | Expected outcome |
| --- | --- |
| Primary number in registrations and listings | High-priority virtual number |
| Work/personal separation | Virtual number |
| Calls through an internet app | High-priority SIP |
| Regular international calls | Relevant SIP |
| Frequent travel and mobile internet need | Relevant eSIM |
| Several matching needs | Up to three results, ordered by relevance and stated priority |
| Public Wi-Fi safety only | No product forced; Wi-Fi/VPN learning |
| Password/account protection only | No product forced; educational next step |

No-product outcomes also cover phishing, browser tracking and social-post privacy where the catalog does not solve the need.

## Results and product details

Show at most three recommendations. Each explains the identified need, suggested change, product, reason based on answers, capability and limitation, product details and any related topic. Internal scores are not exposed. High-priority matches come first; Q5 affects ordering among relevant results.

Product details contain the need, capability, fit, non-fit, limitation, official destination and return to results. Provider CTA currently goes to its registration page; Finder itself performs no purchase. External provider links do not imply integration or endorsement.

## Trust, scope and readiness

No account, stored questionnaire history or personal profile. Blazor interactions do reach the server and remain in circuit memory; this is not an entirely client-only calculator. Future analytics require a separate explicit decision consistent with the privacy promise. Finder never claims complete anonymity.

Not implemented: AI recommendations, accounts, saved/shared results, product comparison, live prices/checkout, numeric privacy scoring, administration or unverified product recommendations.

Readiness requires concrete reasons for every recommendation, understandable rule effects, one/many/zero-result checks, both locales, reachable destinations and honest limits. Prior notes recorded 22 passing tests and responsive/accessibility checks at 1280×800 and 390×844; those are historical observations, not a new comprehensive QA claim. Current solution tests total 97; see the verification report for what was actually rerun.

## Decision inputs

[Vision](VISION.md), [principles](PROJECT_PRINCIPLES.md), [research traceability](RESEARCH_TO_PRODUCT.md) and [product hypotheses](PRODUCT_RESEARCH.md) support understandable explanations, minimal state and education before sales. They are independent research inputs, not official company requirements.
