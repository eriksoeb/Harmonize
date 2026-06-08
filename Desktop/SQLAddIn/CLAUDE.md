# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build

Open `SQLAddIn.sln` in Visual Studio (2022 recommended) and build with **Build > Build Solution** (F6). There is no CLI build workflow — the project requires Visual Studio with the VSTO workload installed.

- Target framework: .NET Framework 4.8
- Output type: Library (VSTO Excel Add-in)
- Debug launches Excel with the add-in loaded automatically (F5)

There are no automated tests in this project.

## Architecture

This is a **VSTO Excel Add-in** that adds a "Harmonize" ribbon tab to Excel for reading and writing time series data from a SQL Server database.

### Key files

| File | Purpose |
|---|---|
| `SQLAddIn.cs` | All business logic — ribbon button handlers, SQL queries, Excel cell manipulation |
| `SQLAddIn.Designer.cs` | Auto-generated ribbon UI layout (buttons, groups) |
| `ThisAddIn.cs` | VSTO host entry point; provides `GetActiveWorkSheet()`, `GetActiveCells()` helpers |

### Connection

The SQL connection string is read at startup from `C:\Harmonize\App\connection.txt` (first line only). This file must exist on the user's machine; it is not bundled with the project.

### Cell format (HARMONIZE links)

Header cells in row 1 use pipe-delimited format:
```
HARMONIZE|LOADSET:SERIESNAME|BaseyearDateParam|RangeParam|AggParam|convert|sort
HARMONIZE|CPI:C00.IDX|2025|YearsLast5|NONE|OFF|asc
```
Example: `HARMONIZE|CPI:C00.IDX|NULL|YearsLast5|NONE|Q|desc`

## Parse format (HARMONIZE links)
 string basis        = parameters.Length > 0 ? parameters[0].Trim() : "NULL";
 string myInterval   = parameters.Length > 1 ? parameters[1].Trim() : "NULL";
 string agg          = parameters.Length > 2 ? parameters[2].Trim() : "NONE";
 string convert2freq = parameters.Length > 3 ? parameters[3].Trim() : "OFF";
 string sort         = parameters.Length > 4 ? parameters[4].Trim() : "desc";

##Test Harmonize link :
basisyear: from 1950 - 2090 or NULL
myInterval not numeric ie : YearsLast5 , thius tezt may vary so no rule? or NOT only digits
agg one of: NONE , AVG_YEAR | AVG_QUARTER|AVG_MONTH|AVG_DAY|SUM_YEAR|LAST_MONTH|AVG_DAY|SUM_DAY|LAST_MONTH
convert2freq:  OFF|MON |Q| ANN
sort: desc | asc

 string sql = $"execute StpGetSeries '{seriesName}',{basis},'{myInterval}',NULL,1,'client','{agg}','{convert2freq}','{sort}',250,NULL";






Excel layout per series (header cell at row 1, column N — as visible in `excel_screen.jpg`):

```
         col N              col N+1
row 1:   HARMONIZE|...link  (empty)
row 2:   Description        IndexName       ← from StpGetMeta
row 3:   Date               Value           ← column headers
row 4:   2022-01-01         85.5            ← data rows
row 5:   2022-02-01         86.5
...
```

- The HARMONIZE link cell (row 1, col N) is the "current cell" the user selects.
- Description is one row below the current cell, same column (row 2, col N).
- IndexName is one row below and one column to the right (row 2, col N+1).
- "Date" / "Value" headers are two rows below (row 3).
- Actual date/value data starts three rows below (row 4+).

### Ribbon buttons and their handlers

| Button label | Handler method | Action |
|---|---|---|
| TestConnect | `button2_Click` | Opens connection, calls `dbo.GetOriginalLogin`, shows server info |
| GetSerie | `btnExecuteSQL_Click` → `ExecuteSQLForCell` | Reads series data via `StpGetSeries` + `StpGetMeta`, fills cells |
| RefreshAll | `RefreshAll_btn_Click` | Scans all used cells for `HARMONIZE|` prefix, calls `ExecuteSQLForCell` on each |
| PutSerie | `Put_btn_Click` | Writes selected header column's data back to DB via `dbo.UTILS_PutTime` + `dbo.UTILS_UpdateMetaXLS` |
| DeleteSeries | `delete_serie_btn_Click` | Calls `dbo.UTILS_DeletefromXLS` to remove the series from the database |
| About | `button1_Click` | Shows version, user, machine, connection file path |

### Stored procedures

- `StpGetSeries` — returns date/value rows for a series
- `StpGetMeta` — returns `Descr` and `IndexName` for a series
- `dbo.UTILS_PutTime` — upserts a single date/value row
- `dbo.UTILS_UpdateMetaXLS` — updates description and unit name for a series
- `dbo.UTILS_DeletefromXLS` — deletes a series
- `dbo.GetOriginalLogin` — returns login/server diagnostics


### StpGetSeries: looks like this in mssql:

CREATE or alter  procedure [dbo].[StpGetSeries]  
(	
@curvename nvarchar(64),
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfunc  nvarchar(128) = null, ------------------------------------ny
@myfnlagint int = 0 , -- function lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@convert2freq char(3) = 'OFF',  -- ANN | MON | DAY | OFF  --new parameter
@sort nvarchar(4),  --new position
@top int, --moved last position
@json NVARCHAR(MAX)
)


### proper call from excel is OK in A1 (default qry):
GetDataFromDatabaseAndFillCells(
      "execute StpGetSeries 'CPI:C00.IDX',NULL,'YEARSLAST5',NULL,1,'client', 'NONE','MON','desc',250, NULL",
    activeCell.Row + 3,    activeCell.Column - 1
);


### Header columns in xls ( not show format=client, top=250 , myfunc-notinuse from excel, myfnlagparam-notinuse from excel:
SetHeaderNameCell("HARMONIZE|CPI:C00.IDX|NULL|YearsLast5|AVG_MONTH|MON|desc", 1, 1);
parameters separator = |, and from the header I need to create the correct st proz execute in correct order.




### Status feedback

Errors and status messages are written to `ribbon_grp.Label` (the ribbon group label) rather than `MessageBox.Show`, except for the About and TestConnect dialogs. Cell backgrounds turn red on error, light blue on success, orange when PutSerie is in progress.


