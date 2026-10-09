# From Research to Product

This document connects competitor research to concrete decisions in Privacy Lab.

The research was not performed to collect references or copy features. Its purpose was to extract principles, test them against Narayana's goals and turn the useful conclusions into product behavior.

## Traceability

| Research source | Insight | Product decision | Privacy Lab implementation |
|---|---|---|---|
| Proton | Human benefit comes before technology. | Begin with situations people recognize instead of technical definitions. | Every topic follows Situation → Problem → What really happens → What you can do. |
| Mullvad | Trust is created through product decisions, not marketing. | Make privacy part of the experience rather than only a subject of the content. | Privacy Lab requires no account or personal profile and visibly explains this on Home. |
| Silent.Link | Teaching only privacy enthusiasts limits growth. | Write for ordinary users and teach before recommending a product. | Topics assume no security background; product relationships appear only after the explanation and only when relevant. |
| IVPN | Honest limitations create more trust than perfect promises. | Explain what a tool does and what it does not do. | Related Product cards contain both a benefit and a limitation; VPN and SMS topics avoid absolute claims. |
| SimpleX | The strongest privacy decision is often collecting less data. | Do not create unnecessary identity or learning-history data. | Privacy Lab does not require registration, create a personal profile or save which topics a user reads. Home states this behavior explicitly. |
| Signal | Privacy should not create unnecessary work. | Keep the learning entry point simple and make protective behavior understandable without configuration. | Users can begin reading immediately; there is no onboarding form, account setup or privacy configuration. |
| Session | Every privacy choice has a usability cost. | Explain trade-offs instead of presenting one universal answer. | Articles and product cards describe context, alternatives and limitations; the toolkit topic explicitly rejects one-size-fits-all setups. |

## Visible Privacy Promise

The Home page communicates two product properties:

> No account or personal profile required.
>
> Privacy Lab does not save which topics you read.

This decision closes two gaps identified after reviewing the competitor research:

1. Privacy was part of the subject matter but was not yet visible in the behavior of Privacy Lab itself.
2. Privacy Lab already avoided unnecessary data collection, but users had no way to know that.

The promise is deliberately narrow. It does not claim complete anonymity or the absence of all infrastructure logs. It describes only behavior implemented and controlled by Privacy Lab.

## Result

Competitor research directly influenced:

- topic structure;
- tone of voice;
- product recommendation timing;
- honest limitation copy;
- absence of registration and personal profiles;
- absence of learning-history storage;
- the visible privacy promise;
- the balance between privacy and usability.

## Solution Finder extension

The same principles are now visible in Solution Finder:

- recommendations are deterministic and explain which answer made a product relevant;
- Virtual Number, eSIM and SIP cards include both fit and limitation copy;
- VPN remains educational and is never presented as a Narayana product;
- no-product results link back to Privacy Lab instead of forcing a sale;
- questionnaire answers live only in the current session and do not create a profile;
- unsupported claims such as guaranteed eSIM coverage, SIP emergency calling or multi-device behavior are not made.

This traceability should be preserved as the ecosystem expands. New modules should be able to explain which research principle informed each important product decision.
