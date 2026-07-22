# Digital Identity and Phone-Number Usage

## Goal

Separate verified phone-number and eSIM capabilities from hypotheses about why Narayana users may want additional numbers.

## Evidence status

Platform and device capabilities below are supported by official documentation. Proposed user motivations are hypotheses because this library does not include representative interviews, survey data or product analytics.

## Verified facts

Phone numbers can serve functions beyond calls and SMS:

- Telegram ties each account to a phone number and allows several accounts associated with different numbers in one app.
- Signal requires a phone number for registration, although optional usernames and privacy settings can reduce disclosure and discoverability by number.
- Compatible multi-SIM and eSIM devices can hold multiple subscriptions or profiles. Whether more than one profile can be active simultaneously depends on the device and eSIM implementation.
- An eSIM profile may provide data only or may include voice, SMS and a phone number; these capabilities depend on the plan and operator.

Sources: [Telegram FAQ](https://telegram.org/faq), [Signal phone-number privacy and usernames](https://support.signal.org/hc/en-us/articles/6712070553754-Phone-Number-Privacy-and-Usernames), [GSMA requirements for multi-SIM devices](https://www.gsma.com/get-involved/working-groups/gsma_resources/ts-37-requirements-for-multi-sim-devices/).

## Candidate user needs

The following are plausible use cases, not measured prevalence:

- separating personal and work contact details;
- keeping a home-country line while using local connectivity;
- limiting disclosure of a primary number to marketplaces or unfamiliar contacts;
- creating distinct accounts where a service permits the selected number type;
- maintaining a local contact point in another country;
- adding travel data without replacing a primary SIM.

Compatibility must be verified per product. A virtual or VoIP number may not be accepted by a bank, messenger, government service or verification provider, and no universal compatibility claim should be made.

## Product distinctions

### Virtual number

May support inbound or outbound calls, SMS or both, depending on the number and service. It should not be described as suitable for verification unless compatibility with the specific service is known.

### Secondary SIM or eSIM

Provides a carrier subscription. Capabilities can include data, voice, SMS and a phone number, but vary by plan. Device support and activation rules also vary.

### Travel eSIM

Often focuses on mobile data. It should not be assumed to include a phone number, voice or SMS.

### SIP or business number

Supports communication workflows defined by the provider and integration. It is not interchangeable with a mobile subscription.

## Hypotheses to test

- Which situations actually lead Narayana customers to acquire an additional number?
- Do customers think in terms of identity separation, travel connectivity, local presence or a specific app?
- Which services must a number work with, and how often does compatibility fail?
- Which trade-offs matter most: longevity, country code, SMS, calls, privacy, price or speed of activation?

## Current conclusion

Phone numbers can function as account identifiers and contact points as well as communication endpoints. However, increasing demand for multiple identities or additional numbers has not been quantified in this research. Narayana should validate the proposed use cases with customer data before treating them as established behaviour.
