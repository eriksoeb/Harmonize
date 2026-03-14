
# Harmonize

Harmonize is a time-series data platform designed to store, manage, and visualize datasets with different frequencies and base years — without requiring programming skills — while also helping analysts and management save time.


The system enables analysts and non-technical users to:

• Store time series from multiple domains in a unified format  
• Compare datasets with different base years and frequencies  
• Generate charts and reports without programming  
• Integrate with Python and R for advanced analysis  

Architecture:

- Database: Microsoft SQL Server
- Desktop UI: C# Windows application
- APIs: Python and R
- Charts: Highcharts



## Target Audience

Harmonize is designed for **organizations that need to store, share, and analyze time series data across multiple domains**. Typical use cases include data related to **environment, prices, health, employment, power, energy**, and other statistical or operational datasets.

The system is especially well suited for organizations that want to **centralize data from different sources**, make it **accessible to users with varying technical skills**, and enable **consistent analysis and comparison across domains and time frequencies**.




## Harmonize Interface

<p align="center">
  <img src="https://raw.githubusercontent.com/eriksoeb/Harmonize/main/docs/harmonize2.jpg" width="800">
</p>



## Architecture


```
┌──────────────────────────────────────────────────────────────────────────┐
│                 CLIENT and USER LAYER                                    │
│                                                                          │
│  C# Desktop App     Python        R            Excel Addin               │
│  - Visualization    - ETL         - Analysis   -Read                     │
│  - Highcharts       - Upserts     - Models     -Update                   │
│                                                -Ad -hoc analysis         │
└───────────────┬───────────────┬──────────────────────────────────────────┘
                │               │
                │ calls stored procedures
                │
┌───────────────v─────────────────────────────┐
│           STORED PROCEDURE API  (main ones) │
│                                             │
│  GetSeris                                   │
│  PutTime                                    │
│  Create Datasets                            │
│  Update Statistics                          │
│  Delete Time Series / Datasets              │
│                                             │
│  Centralized validation & business logic    │
│                                             │
└───────────────────────┬─────────────────────┘
                        │
                        │
┌───────────────────────v─────────────────────┐
│                DATA LAYER                   │
│                                             │
│            Microsoft SQL Server             │
│                                             │
│  Tables:                                    │
│  - Loadset                                  │
│  - Timeseries                               │
│  - Observations                             │
│                                             │
│  Time series using converions stored in UTC │
│                                             │
└─────────────────────────────────────────────┘

```


## Quick Start

1. Install Microsoft SQL Server. (only if you do not have one already)
2. Restore the Harmonize database:

```
RESTORE DATABASE Harmonize
FROM DISK = 'Harmonize.bak'
```

3. Open the C# solution in Visual Studio.
4. Build and run the application. (possible to use already compiled )
To test drive Harmonize with a testdatabase in cloud see downloads at www.Harmonize.com

Python examples are available in the `Python/` directory.






## Requirements

### Database (Required when you want to have control of your data)
- **Microsoft SQL Server**
- Database model: `Harmonize.sql`
- Backup file: `Harmonize.bak`  
  These ensure the Harmonize database is available and correctly configured.

### C# Application (Optional but Recommended)
- Windows C# application for visualization and interaction
- Requires **Visual Studio** to compile and build
- Not required, but significantly improves usability

### Python / R Integration
- **Python installation required** for API interaction
- Python samples are available in the `Python/` directory
- Used for:
  - Uploading / upserting new datasets
  - Extracting data for further calculations (e.g., seasonal adjustment)
  - Writing processed data back into the database
- Similar workflows apply for **R**


### Utilities
-- Stored procedures available from Python / R and MSSQL Database
  - Create loadset
  - PutTime  :Update or Inserts date to  existing series, or create new series if not already exists
  - Update statistics : Number of observations, first and last observations, updated time etc
  - Delete loadsets
  - Delete Data on a timeseries
  - Delete a timeseries and its data




---

## Key Features

### Time Series Flexibility
- Store time series with **different frequencies** (weekly, monthly, quarterly, annual)
- Seamless comparison of datasets with different base years  
  (e.g. `2015=100` vs `2020=100`)
- Use of **datetime formats** enables zoomable and scrollable charts

### Aggregations & Functions
- Aggregations:
  - `avg_quarter`
  - `avg_year`
- Functions:
  - `diff_n`
  - `pct_1`, `pct_2`, … `pct_n` (lag-based calculations)

### Examples
- Chart a **quarterly index** together with a **monthly index**
- Correctly add weekly, monthly, quarterly, and annual series
- Use a **common base year or base period**, even if datasets differ

---

## Charting & Reporting

### Available Chart Types
- **Chart**
- **MultiChart**
- **YearChart**  
  (available under the Chart menu)

### Reports
- **User Report** button for quick:
  - Horizontal reports
  - Vertical reports
- Report tables available directly inside charts
- Number of decimals and datatime format can be chosen from the Harmonize and template

### Saved Selections
- Users can save and retrieve series selections
- Saved selections are automatically repopulated with **fresh data** (if available)
- Supports both:
  - **Dynamic intervals** (e.g. always current year)
  - **Static intervals** (e.g. always showing data up to 2025)

---

## Customization

### HTML & CSS
- Templates can be modified (optional)
- Customize:
  - Fonts
  - Colors - can also be selected from harmonize app
  - Legends
  - Tooltips
- Modify `Harmonize.css` to match your design standards
- The templates takes a file as arguments and could easily be used for a simple web site:
- Charts used at web is demonstrated at www.Harmonize.no 
- Harmonize let you save datafiles in custom names to be used by the templates on your web site
- what you need is then just to copy / move newer datafiles to your web- when it already has the templates


### Chart Libraries
- Uses **Highcharts** by default
- Can be replaced with other compatible JavaScript charting libraries  
  *(not recommended)*

### Important Notes
- The C# application **overwrites `MyData.js`** each time charts are generated  
  - Copy it elsewhere if you need to preserve the data
- You may:
  - Copy and modify `Chart.html` templates
  - Create your own HTML templates
  - Fully customize all HTML files

### YearChart Template
- Only use intervals containing data **up to one year**
  - Examples: `CurrentYear-1`, `CurrentYear-2`
- Additional intervals can be added in the database

---

## Time Zone Handling

Harmonize supports high-frequency data (e.g. hourly) and correctly handles **summer and winter time changes**.

### Key Rules
- Data must be stored in **UTC**
- Each dataset can be configured (in the `Loadset` table) to display in a different time zone

### Important Considerations
- Time zones affect not only visualization, but also **aggregation logic**
- The system converts time zones **before** applying functions and aggregations

### Example
- Hourly spot prices:
  - **25 observations** when switching to winter time
  - **23 observations** when switching to summer time
- Charts may show **two observations at 02:00** once per year when CET clocks are set back

---

## Multilingual Support

- The user interface supports **multiple languages**
- Users can:
  - Change translations themselves
  - Add new languages as needed
  - Harmonize currently has English, Norwegian, Ukrainian, Albanian, Hindi and Arabic do demonstrate the functionality

---

## Summary

Harmonize provides a powerful yet user-friendly platform for:
- Managing time series data
- Comparing datasets across frequencies and base years
- Generating interactive charts and reports
- Integrating with Python and R for advanced analytics, updates and downloads
- Integrating with XLSAddIn (read & update) and save as csv options
- Harmonize can save / retrieve selections and populate with fresh data later
- Harmonize can delete and upload data on a dataset if user is configured with write-access
- The system has configurable access control and logging

It is suitable for analysts, managers, and non-programmers & programmers alike.
