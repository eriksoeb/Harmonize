# Data Explorer — Spec

## Goal

Build a web page (HTML + PHP) that lets the user search, select, and compare series across domains (e.g. CPI vs PPI) on a single Highcharts chart.

## Data source

The pre-built JSON files already available under each domain's `series/` folder:

```
www/data/cpi/series/*.json
www/data/ppi/series/*.json
www/data/energy/series/*.json
www/data/satellites/series/*.json
...
```

Each JSON file follows the standard Harmonize format used by the existing chart templates.

## User workflow

1. **Open** `dataexplorer.php` (or `dataexplorer.html` with a PHP back-end).
2. **Filter by domain** (optional) — dropdown or toggle buttons for CPI, PPI, Energy, etc. (populated dynamically from subdirectories, same pattern as `series.php`).
3. **Search / select series** — a searchable list (or multi-select) built from each domain's `catalog.json`. The user picks 1 or 2 series to compare.
4. **View chart** — the selected series are rendered together on a single Highcharts chart so they can be visually compared.

## Chart rendering

Re-use as much of the existing `/templates/Chart.html` logic as possible. Two options to evaluate:

| Approach | How it works | Pros | Cons |
|---|---|---|---|
| **A. Multi-file Chart.html** | Extend `Chart.html` to accept multiple `filename` params, e.g. `?filename=...&filename2=...` | Re-uses existing template directly | Requires modifying Chart.html; only supports a fixed number of series params |
| **B. Client-side merge** | Data Explorer page fetches the selected JSON files via `fetch()`, merges the `series` arrays, and renders its own Highcharts chart | No changes to Chart.html; supports N series; more flexible | Some duplication of chart init code |

**Recommendation:** Approach B — the explorer page owns its own chart instance, fetches the selected JSON files client-side, merges the series arrays, and renders. This keeps Chart.html unchanged and gives full flexibility (filter, scale, dual Y-axis, etc.).

## Key UI elements

- **Domain filter** — buttons or dropdown (multi-select OK) to narrow the catalog list.
- **Series picker** — searchable list showing series name + description. User checks 1-2 (or more) series.
- **Compare button** — loads the selected JSON files and renders the combined chart.
- **Chart container** — standard `<div id="container">` with Highcharts, matching existing Harmonize styling.

## Open questions

- Support for more than 2 series? (Multi-select with no hard limit?)
- Dual Y-axis when units differ (e.g. index vs MW)?
- Should selected series be shareable via URL params (e.g. `?s=cpi/c00.idx&s=ppi/p01.idx`)?
