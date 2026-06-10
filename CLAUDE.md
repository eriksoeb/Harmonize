
Git: https://github.com/eriksoeb/Harmonize/
Do NOT commit: `bin/`, `debug/`, `obj/`, compiled executables, `.bak` database backups, `*.suo`, `.vs/` folders, connection string files.
**Exception:** `Desktop/WindowsFormsAppCSharp19/CSharp/bin/Debug/` — commit `.html`, `.css`, and `.csv` files only (chart templates and data files used by the app).

# Harmonize — Project Guide for Claude Code

Harmonize is a time-series data platform built on Microsoft SQL Server with a C# Windows desktop client, an Excel VSTO Add-in, and Python/R APIs. It lets analysts store, compare, and visualize datasets of different frequencies and base years — without programming skills.

---

## Repository Layout

```
C:\HarmonizeCode\
├── MSSQL/                        # Database: SQL Server scripts
│   ├── Tables/                   # Table DDL (Curve, CurveData, LoadSet, Unit, …)
│   ├── Procedures/               # Stored procedures (StpGetSeries, UTILS_PutTime, …)
│   ├── Functions/                # Scalar/table-valued functions
│   ├── Views/                    # Views
│   ├── Types/                    # User-defined types
│   ├── Harmonize.bak             # Full database backup (do not commit)
│   └── Harmonizedb.sql           # Schema script
│
├── Desktop/
│   ├── WindowsFormsAppCSharp19/  # Main Harmonize C# WinForms desktop app
│   │   └── CSharp/              # Solution: CSharp.csproj (.NET 4.6.1)
│   ├── SQLAddIn/                 # Excel VSTO Add-in (.NET 4.8)
│   └── UpdateTable/              # Small utility WinForms app
│
├── Py/                           # Python scripts (ETL, API examples)
├── R/                            # R integration scripts
├── www/                          # Web front-end (HTML/CSS/Highcharts templates)
│   ├── gitdoc/                   # Docs published to GitHub Pages
│   └── api/                      # Web API layer
└── db/                           # Additional DB helpers
```

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                 CLIENT and USER LAYER                                    │
│  C# Harmonize app     Python        R          Excel Add-in              │
│  - Visualization    - ETL         - Analysis   - Read                    │
│  - Highcharts       - Upserts     - Models     - Update                  │
└───────────────┬───────────────────────────────────────────────────────────┘
                │  (calls stored procedures)
┌───────────────▼─────────────────────────────┐
│      STORED PROCEDURE API                   │
│  StpGetSeries · UTILS_PutTime               │
│  UTILS_Create_Loadset · UTILS_Delete_Loadset│
│  Client_Update_Table · Stp_Delete*          │
└───────────────────────┬─────────────────────┘
                        │
┌───────────────────────▼─────────────────────┐
│            DATA LAYER — SQL Server           │
│  Tables: LoadSet · Curve · CurveData        │
│  CurveInfo · Unit · DataType · Functions    │
│  All time series stored in UTC              │
└─────────────────────────────────────────────┘
```

---

## Database (MSSQL/)

### Setup
```sql
RESTORE DATABASE Harmonize FROM DISK = 'Harmonize.bak'
```

### Core Tables
| Table | Purpose |
|---|---|
| `LoadSet` | Dataset / series group; holds time-zone and frequency metadata |
| `Curve` | Individual time series definitions |
| `CurveData` | Observations (UTC datetime + value) |
| `CurveInfo` | Descriptions, units, base-year info |
| `Unit` | Unit definitions |
| `Functions` | Aggregation/transform definitions |

### Key Stored Procedures (MSSQL/Procedures/)
| Procedure | Purpose |
|---|---|
| `StpGetSeries` | Retrieve a series with frequency conversion, aggregation, base-year, sorting |
| `UTILS_PutTime` | Upsert a single date/value observation |
| `UTILS_Create_Loadset` | Create a new loadset/dataset |
| `UTILS_Delete_Loadset` | Delete a loadset and its curves |
| `Stp_DeleteCurveandData` | Delete a curve and all its observations |
| `Stp_DeleteCurveDataOnly` | Delete observations only (keep curve definition) |
| `Client_Update_Table` | Table management from client |
| `UTILS_UpdateCurveinfo` | Update description / unit for a series |

### StpGetSeries signature
```sql
CREATE OR ALTER PROCEDURE [dbo].[StpGetSeries]
(
    @curvename   nvarchar(64),
    @basis       int,               -- base year (e.g. 2015), or NULL
    @myinterval  nvarchar(128) = NULL,
    @myfunc      nvarchar(128) = NULL,
    @myfnlagint  int = 0,           -- lag for pct/diff functions
    @format      nvarchar(8) = NULL,
    @agg         nvarchar(16),      -- NONE | AVG_YEAR | AVG_QUARTER | AVG_MONTH | SUM_* | LAST_MONTH
    @convert2freq char(3) = 'OFF',  -- OFF | ANN | Q | MON | DAY
    @sort        nvarchar(4),       -- asc | desc
    @top         int,
    @json        NVARCHAR(MAX)
)
```

---

## C# Desktop App (Desktop/WindowsFormsAppCSharp19/)

- **Solution:** `CSharp.csproj` — .NET Framework 4.6.1, WinForms
- **Build:** Open in Visual Studio → Build Solution (F6)
- **Entry point:** `Program.cs` → `Form1.cs`
- **Connection:** reads from `C:\Harmonize\App\connection.txt` (not bundled)
- **Charts:** Highcharts (rendered in embedded browser); output file `MyData.js` is overwritten on each chart generation — copy it elsewhere to preserve

Do **not** commit `bin/`, `obj/`, `*.exe`, `*.pdb`, `*.user`.

---

## Excel VSTO Add-in (Desktop/SQLAddIn/)

- **Solution:** `SQLAddIn.sln` — .NET Framework 4.8, VSTO Excel Add-in
- **Build:** Visual Studio 2022 with VSTO workload → F6; F5 launches Excel with add-in loaded
- **Key files:**

| File | Purpose |
|---|---|
| `SQLAddIn.cs` | All business logic — ribbon handlers, SQL queries, Excel cell manipulation |
| `SQLAddIn.Designer.cs` | Auto-generated ribbon UI |
| `ThisAddIn.cs` | VSTO host entry point |

- **Connection:** reads from `C:\Harmonize\App\connection.txt` (first line only)

### Excel Cell Format (HARMONIZE links)
Header cells in row 1 use pipe-delimited format:
```
HARMONIZE|LOADSET:SERIESNAME|BaseyearDateParam|RangeParam|AggParam|convert|sort
HARMONIZE|CPI:C00.IDX|2025|YearsLast5|NONE|OFF|asc
```
Parameters (0-indexed after the series name):
```
[0] basis       — base year 1950–2090 or NULL
[1] myInterval  — named interval e.g. YearsLast5 (non-numeric)
[2] agg         — NONE | AVG_YEAR | AVG_QUARTER | AVG_MONTH | AVG_DAY | SUM_YEAR | SUM_DAY | LAST_MONTH
[3] convert2freq— OFF | MON | Q | ANN
[4] sort        — desc | asc
```

### Excel Layout per Series
```
         col N              col N+1
row 1:   HARMONIZE|...link  (empty)       ← user selects this cell
row 2:   Description        IndexName     ← from StpGetMeta
row 3:   Date               Value         ← column headers
row 4+:  2022-01-01         85.5          ← data rows
```

### Ribbon Buttons
| Button | Handler | Action |
|---|---|---|
| TestConnect | `button2_Click` | Calls `dbo.GetOriginalLogin`, shows server info |
| GetSerie | `btnExecuteSQL_Click` | Reads via `StpGetSeries` + `StpGetMeta`, fills cells |
| RefreshAll | `RefreshAll_btn_Click` | Refreshes all `HARMONIZE|` cells in the sheet |
| PutSerie | `Put_btn_Click` | Writes data back via `UTILS_PutTime` + `UTILS_UpdateMetaXLS` |
| DeleteSeries | `delete_serie_btn_Click` | Calls `UTILS_DeletefromXLS` |
| About | `button1_Click` | Shows version, user, machine, connection path |

Status messages go to `ribbon_grp.Label`. Cells turn red on error, light blue on success, orange during PutSerie.

---

## Python Integration (Py/)

Python scripts connect directly to SQL Server and call stored procedures. Examples:
- `cpi.py`, `kpi.py`, `ppi.py` — load index data
- `utslippssb.py` — emissions data from SSB
- `worldSPI.py`, `Waterlevel.py` — external data ingestion
- `ffi.py` — FFI dataset loading
- `GetCurve2pythonChart.py` — extract series for charting
- `PutDescrline.py` — update descriptions
- `getJsonCurve.py` — JSON output helper

Connection string is typically stored in a local `connection.txt` or `connection_tst.txt`.

---

## R Integration (R/)

Similar to Python — connects to SQL Server, calls stored procedures for reading and writing time series.

---

## HTML Chart Templates — Two Versions

There are two sets of HTML chart templates that must be kept **separate** — do NOT merge or sync them:

| Location | Used by | Data loading |
|---|---|---|
| `Desktop/WindowsFormsAppCSharp19/CSharp/bin/Debug/*.html` | C# app + setup package | Loads `.js` files via dynamic `<script>` tag (`window.seriesData = [...]`) |
| `www/demo/*.html` | Web / browser | Loads `.json` files via `fetch()` |

The `.js` format is required by the C# app because it uses a local file system (no web server, so `fetch()` is blocked by the browser). The web versions use pure JSON fetched over HTTP.

When updating chart logic (tooltips, aggregations, date formats, legend behaviour), apply the change to **both** sets of templates.

---

## CSS

There are two `Harmonize.css` files — keep core chart styles in sync between them:

| File | Used by |
|---|---|
| `Desktop/WindowsFormsAppCSharp19/CSharp/bin/Debug/Harmonize.css` | C# app — **also** renders charts in an embedded web browser |
| `www/Harmonize.css` | All web pages under `www/` and `www/demo/` |

The web version adds `flex`/sticky-footer layout and `.demo-list`, `.download-box` classes not needed by the app. Core chart styles (`.topnav`, `#container`, `#chart-series-names`, `.series-hidden`, `.highcharts-data-table`) must stay identical in both.

---

## Web Layer (www/)

- Static HTML + CSS templates using **Highcharts**
- `HarmonizeNO.css` — main stylesheet
- The C# app writes `MyData.js` which is consumed by the HTML templates
- Templates support Chart, MultiChart, YearChart types
- Demonstrated at www.Harmonize.no

---

## Key Rules for This Project

- **Do not commit** `bin/`, `obj/`, `debug/` build output folders — **exception:** `Desktop/WindowsFormsAppCSharp19/CSharp/bin/Debug/` `.html`, `.css`, `.csv` files are committed (chart templates + data)
- **Do not commit** compiled executables (`.exe`, `.dll`) or debug symbols (`.pdb`)
- **Do not commit** `.bak` database backups
- **Do not commit** `.vs/`, `*.suo`, `*.user` IDE files
- **Do not commit** `connection.txt` / connection string files (contain credentials)
- All time series data must be stored in **UTC** in the database
- The stored procedure API is the single entry point for all data access — do not query tables directly from client code

---

## Git Repository

https://github.com/eriksoeb/Harmonize/
