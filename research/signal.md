# Competitor: Signal

## Purpose

Assess how Signal presents private messaging and manages the trade-off between discoverability and phone-number privacy.

## Evidence status

Review date: 18 July 2026.

Feature statements are based on Signal's official documentation. Communication conclusions are interpretations. This review does not independently compare Signal's security, usability or adoption with other messengers.

## Verified facts and company claims

Signal provides end-to-end encrypted messaging and calling. A phone number is still required to register.

Users can create an optional username to start a conversation without sharing their phone number. Signal also provides settings that limit who can see a number and who can find an account by number. Signal notes that stricter discoverability settings can make it harder for contacts to find one another.

Signal is operated by the nonprofit Signal Foundation and publishes client and server source code.

## Communication interpretation

Signal's public communication generally leads with private everyday conversation, while technical details remain available in support and engineering materials. Its phone-number documentation is a useful example of explaining both the privacy benefit and the usability cost of a setting.

## Strengths observed in the communication

- Privacy is described as part of ordinary messaging rather than a specialist workflow.
- Phone-number visibility and discoverability are explained separately.
- The documentation states practical limitations, including the continuing registration requirement.
- Nonprofit governance and open-source code provide inspectable trust signals.

## Limitations of this review

- No usability test or adoption analysis was conducted.
- The review does not establish that Signal has stronger metadata protection than another product.
- Claims about simplicity or audience breadth would require user research.

## Implications for Narayana

**Interpretation:** identity controls should explain the trade-off between being easy to find and revealing a phone number.

**Hypothesis to test:** users will choose privacy settings more confidently when each control states both its protection and its effect on reachability.

## Sources

- [Signal home](https://signal.org/)
- [Signal phone-number privacy and usernames](https://support.signal.org/hc/en-us/articles/6712070553754-Phone-Number-Privacy-and-Usernames)
- [Signal Foundation](https://signalfoundation.org/)
- [Signal source code](https://github.com/signalapp)
