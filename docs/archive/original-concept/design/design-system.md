# Product Design System

## 1. Principles

- **Narayana-native.** Every product feels like part of one Narayana ecosystem.
- **Clean Engineering.** Simple, functional, structured and free from visual noise.
- **Calm.** Explain consequences without fear-based communication.
- **Educational.** Teach through small blocks, real situations and interaction.
- **Trustworthy.** Create a sense of control, transparency and reliability.
- **Minimal.** Every visible element has a purpose.
- **Consistent.** Products share color, typography, grid, components, motion and language.
- **Monochrome-first.** Color is a functional accent, never decoration.
- **Theme-aware.** Light and dark themes are equally important.
- **Interactive.** Users explore, compare and receive feedback instead of only reading.

Narayana core: dark, monochrome, technical, independent.

Our layer: clear, educational, interactive, human.

> Narayana provides privacy tools. Privacy Lab explains why those tools matter.

## 2. Visual Style

- Clean Engineering style focused on information.
- Light and dark themes are equally complete and must preserve the same hierarchy.
- Centered content with generous whitespace and constrained reading width.
- Cards are the primary structural element.
- Restrained, small radii consistent with Narayana; minimal shadows; thin neutral borders.
- Comfortable density and progressive disclosure.
- One idea per semantic block.

### Layout and density

- Interfaces must never feel crowded.
- Reading comfort is more important than fitting more information on screen.
- Long content is divided into short logical sections, examples, diagrams and interactions.
- Information is revealed gradually; users should not face every detail at once.

### Surfaces, borders and shadows

- Separation comes primarily from spacing and thin neutral borders.
- Shadows are subtle and used only when hierarchy cannot be expressed through spacing or borders.
- Decorative floating surfaces are avoided.

### Shape

- Cards and controls use restrained small radii rather than large soft rounding.
- Shape should feel consistent with Narayana's existing website: precise, quiet and engineering-oriented.
- Current implementation token: `--radius-card: .25rem` (`4 px`).

## 3. Color

- Monochrome foundation with one functional accent per screen.
- Accent color is reserved for navigation, state and feedback.
- Semantic tokens must be used instead of raw colors in components.
- Light and dark palettes preserve the same hierarchy and contrast.
- Most of every screen remains monochrome.
- A screen should not introduce several competing accent colors.
- Color never acts as the only way to communicate meaning.

## 4. Typography

- Font family: **Inter**, with a system sans-serif fallback.
- Hierarchy: Display, H1, H2, Body, Caption.
- Priorities: readability, generous line height, short lines and minimal decoration.
- Display is reserved for hero-level messages.
- H1 identifies a page; H2 identifies a semantic section.
- Body carries primary educational content; Caption carries supporting metadata.

## 5. Spacing and Grid

- Base unit: **8 px**.
- Maximum content width: approximately **1200 px**.
- Reading column: approximately **800–900 px**.
- Standard card gap: **24 px**.
- All spacing values use multiples of the base unit.
- Large vertical gaps separate major sections.
- Layouts support desktop, tablet and mobile without sacrificing reading width.

## 6. Components

Implemented for Privacy Lab MVP:

- Category Card
- Topic Card
- Callout
- Navigation
- Language Switch
- Theme Switch
- Related Product Card
- Loading and Not Found states

Planned when required by real content:

- Info Card
- Alert
- Breadcrumbs

Later iterations:

- Quiz
- Simulation Card
- Progress Bar
- Tag
- Chip
- Badge

## 7. Motion

- Functional, calm and non-blocking.
- 150–200 ms for hover and small state changes.
- 200–300 ms for disclosure and transitions.
- Use ease-out for appearance and movement; ease-in-out for longer transitions.
- Support `prefers-reduced-motion`.
- Avoid bounce, aggressive scaling and simultaneous decorative movement.

Use:

- subtle background, border or opacity changes on hover;
- minimal card lift where it clarifies interactivity;
- fade for appearing and disappearing content;
- accordion motion for disclosure;
- slide for future quiz or simulation steps;
- very light page transitions only when they do not delay navigation.

Do not use:

- bounce;
- sharp scaling;
- long decorative transitions;
- simultaneous movement of many unrelated elements.

## 8. Icons

- Simple line icons that improve comprehension.
- Icons support labels and never replace essential text.
- Icons are predominantly monochrome and are not used as decoration.
- The canonical NRN set lives in `design/icons` and contains 61 interface icons.
- Icon geometry uses a `24 × 24` viewBox, `1.75` stroke, square caps and bevel joins.
- Primary strokes inherit `currentColor`; the shifted gap uses `--nrn-icon-accent` with `#FF9A00` as fallback.
- Individual SVG files, the generated sprite and visual catalogue must be rebuilt together after geometry changes.

## 9. Illustrations

- Minimal, predominantly monochrome line illustrations.
- Used only when they materially improve understanding.
- Each illustration communicates one idea through a readable visual metaphor.
- Illustrations should feel related to Narayana without tracing or copying existing artwork.
- Light and dark variants must remain equally legible.

## 10. Responsive Rules

- Support desktop, tablet and mobile.
- Preserve reading width and hierarchy before fitting more information.
- Reflow multi-column layouts into a single column without horizontal scrolling.
- Maintain comfortable touch targets and spacing on mobile.
- Preserve information hierarchy before reducing whitespace or typography.
- Avoid horizontal scrolling in page content; navigation may scroll only when every destination remains visible and understandable.
- Interactive targets should be at least `44 × 44 px` where practical.

## Product relationship

Narayana provides privacy tools. Privacy Lab explains why those tools matter.

The user should experience Privacy Lab as another part of the Narayana ecosystem, not as an unrelated educational website. Recognition is created through shared monochrome foundations, typography, spacing, restrained shapes, line artwork and calm engineering-oriented communication.
