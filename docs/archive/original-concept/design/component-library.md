# Component Library

Phase 6.5 defines reusable interface atoms before additional screens are designed.

## Figma structure

```text
00 Foundations
01 Navigation
02 Cards
03 Feedback
04 Controls
```

Every component must include:

- Light and Dark variants.
- Default, Hover, Focus and Disabled states where applicable.
- Desktop and Mobile behavior.
- Auto Layout and 8 pt spacing.
- Named properties instead of detached visual variants.

## Category Card

Purpose: entry point from Learning Home to a category.

Content:

- Icon
- Title
- Description
- Action label and arrow

Variants: Default, Hover, Unavailable.

## Topic Card

Purpose: entry point from a category to a topic.

Content:

- Order
- Title
- Tagline
- Reading duration
- Arrow

Variants: Default, Hover, Completed (later).

## Info Card

Status: Planned.

Purpose: supporting information that is useful but not essential to the main flow.

Content:

- Optional icon
- Label or title
- Body

Variants: Neutral, Accent.

## Alert

Status: Planned.

Purpose: communicate a risk, recommendation or practical tip.

Variants:

- Risk
- Recommendation
- Tip

Every variant must pair color with an icon and label.

## Callout

Status: Implemented through solution and toolkit callouts; extraction into a shared component remains optional.

Purpose: emphasize the most important conclusion inside a topic.

Content:

- Optional step number
- Heading
- Body

Variants: Neutral, Solution.

## Breadcrumbs

Status: Deferred. Privacy Lab currently uses a clear back link plus previous/next topic navigation.

Purpose: show hierarchy and provide a predictable path back.

Pattern:

```text
Privacy Lab / Foundations / Topic
```

Mobile: collapse intermediate levels when space is limited.

## Navigation

Status: Implemented for Privacy Lab. Deferred modules are visible as non-interactive coming-soon items.

Purpose: global movement across the ecosystem.

MVP content:

- Product identity
- Learn
- Simulations
- Solution Finder
- Preferences

Desktop: horizontal. Mobile: wrapped, horizontally scrollable navigation without hidden destinations.

## Language Switch

Purpose: switch content language without losing the current location.

Options: EN, RU.

States: Default, Active, Focus.

Accessibility:

- Use a semantic button group.
- Expose selected state through `aria-pressed`.
- Never rely on color alone.

## Shared rules

- Inter typography.
- 8 pt spacing grid.
- Thin neutral borders.
- Restrained small radii consistent with the existing Narayana visual language.
- Current shared card radius: `4 px`.
- No decorative shadows.
- Motion: 150–200 ms for hover and state changes.
- Preserve complete usability under `prefers-reduced-motion`.
