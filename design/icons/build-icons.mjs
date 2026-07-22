import { readFile, writeFile } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const root = dirname(fileURLToPath(import.meta.url));
const manifest = JSON.parse(await readFile(join(root, "manifest.json"), "utf8"));
const labels = {
  navigation: "Navigation", everyday: "Everyday situations", control: "Control",
  privacy: "Privacy", network: "Network", devices: "Devices and data",
  actions: "Actions", services: "Services and tools", status: "States and status", more: "More"
};

const symbols = [];
const cards = [];
const svgRows = [];
let rowIndex = 0;

for (const [category, names] of Object.entries(manifest)) {
  const icons = [];
  for (const name of names) {
    const source = await readFile(join(root, "svg", category, `${name}.svg`), "utf8");
    const match = source.match(/<svg\s+([^>]*)>([\s\S]*?)<\/svg>/);
    if (!match) throw new Error(`Invalid SVG: ${category}/${name}.svg`);
    const attributes = match[1]
      .replace(/\s+xmlns="[^"]*"/g, "")
      .replace(/\s+viewBox="[^"]*"/g, "")
      .replace(/\s+aria-hidden="[^"]*"/g, "")
      .trim();
    const id = `nrn-${name}`;
    symbols.push(`<symbol id="${id}" viewBox="0 0 24 24" ${attributes}>${match[2].trim()}</symbol>`);
    icons.push(`<article class="icon-card"><svg aria-hidden="true"><use href="#${id}"/></svg><code>${name}</code></article>`);
  }
  cards.push(`<section><h2>${labels[category]}</h2><div class="icon-grid">${icons.join("")}</div></section>`);
  svgRows.push(`<text x="40" y="${rowIndex * 170 + 34}" class="section-title">${labels[category]}</text>`);
  names.forEach((name, index) => {
    const x = 40 + (index % 6) * 190;
    const y = rowIndex * 170 + 58 + Math.floor(index / 6) * 104;
    svgRows.push(`<g transform="translate(${x} ${y})"><rect width="160" height="86" class="card"/><svg x="56" y="10" width="48" height="48"><use href="#nrn-${name}"/></svg><text x="80" y="74" text-anchor="middle" class="label">${name}</text></g>`);
  });
  rowIndex += Math.ceil(names.length / 6);
}

const sprite = `<svg xmlns="http://www.w3.org/2000/svg" style="display:none">\n${symbols.join("\n")}\n</svg>\n`;
await writeFile(join(root, "nrn-icons.svg"), sprite);

const preview = `<!doctype html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>NRN Icon System</title><style>
:root{--bg:#080b0c;--surface:#0d1112;--line:#263033;--text:#f5f5f5;--muted:#9da5a7;--nrn-icon-accent:#FF9A00;color-scheme:dark}
*{box-sizing:border-box}body{margin:0;background:var(--bg);color:var(--text);font:15px Inter,system-ui,sans-serif}
main{width:min(1180px,calc(100% - 40px));margin:auto;padding:56px 0 80px}header{display:flex;align-items:end;justify-content:space-between;gap:24px;border-bottom:1px solid var(--line);padding-bottom:24px}h1{margin:0;font-size:clamp(30px,5vw,58px);letter-spacing:-.05em}header p{max-width:440px;margin:0;color:var(--muted);line-height:1.6}section{padding-top:42px}h2{font-size:13px;text-transform:uppercase;letter-spacing:.12em;color:var(--nrn-icon-accent)}
.icon-grid{display:grid;grid-template-columns:repeat(6,1fr);border:1px solid var(--line)}.icon-card{min-height:142px;display:grid;place-items:center;gap:14px;padding:22px;border-right:1px solid var(--line);border-bottom:1px solid var(--line)}.icon-card svg{width:48px;height:48px}.icon-card code{color:var(--muted);font-size:12px}.icon-card:hover{background:#111719}.icon-card:hover svg{transform:translateY(-2px)}svg{transition:transform 180ms ease-out}
@media(max-width:800px){.icon-grid{grid-template-columns:repeat(3,1fr)}}@media(max-width:480px){main{width:min(100% - 24px,1180px)}header{display:block}header p{margin-top:16px}.icon-grid{grid-template-columns:repeat(2,1fr)}}
</style></head><body>${sprite}<main><header><div><small>NRN ICON SYSTEM</small><h1>Broken line.<br>Shifted gap.</h1></div><p>60 original interface icons reconstructed from the approved visual direction. Primary strokes inherit <code>currentColor</code>; the gap uses <code>--nrn-icon-accent</code>.</p></header>${cards.join("")}</main></body></html>`;
await writeFile(join(root, "preview.html"), preview);

const svgPreview = `<svg xmlns="http://www.w3.org/2000/svg" width="1200" height="${rowIndex * 170 + 20}" viewBox="0 0 1200 ${rowIndex * 170 + 20}" style="color:#f5f5f5;--nrn-icon-accent:#FF9A00">
<style>.bg{fill:#080b0c}.card{fill:#0d1112;stroke:#263033}.section-title{fill:#FF9A00;font:600 13px sans-serif;letter-spacing:1.5px;text-transform:uppercase}.label{fill:#9da5a7;font:12px monospace}</style>
<rect class="bg" width="100%" height="100%"/>${symbols.join("")}${svgRows.join("")}</svg>`;
await writeFile(join(root, "preview.svg"), svgPreview);

console.log(`Built ${symbols.length} icons.`);
