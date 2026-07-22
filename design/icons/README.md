# NRN Icon System

The set contains 61 original interface icons based on the approved NRN icon-system mockup, including the product-card book icon.

## Geometry

- ViewBox: `0 0 24 24`
- Stroke: `1.75`
- Caps: square
- Joins: bevel
- Primary stroke: `currentColor`
- Accent stroke: `var(--nrn-icon-accent, #FF9A00)`
- Style: broken polygonal line with an offset accent gap

## Files

- `svg/<category>/<name>.svg` — individual icons
- `nrn-icons.svg` — generated SVG sprite
- `preview.html` — generated visual catalogue
- `preview.svg` — generated portable contact sheet
- `manifest.json` — canonical icon list

## Individual SVG

```html
<img src="icons/svg/navigation/home.svg" alt="">
```

For full color control, inline the SVG. The primary line inherits `color`; the accent can be overridden:

```css
.icon {
  color: #fff;
  --nrn-icon-accent: #ff9a00;
}
```

## Sprite

Inline `nrn-icons.svg` once, then reference a symbol:

```html
<svg class="icon" aria-hidden="true">
  <use href="#nrn-home"></use>
</svg>
```

Rebuild the sprite and preview after changing an individual icon:

```sh
node design/icons/build-icons.mjs
```
