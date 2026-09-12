# Bulk Upload Plan

Replace row-by-row `EXEC UTILS_PutTime` calls with a bulk insert via a staging table.

## Current approach (slow)

Python loops through rows, calling `EXEC UTILS_PutTime ?, ?, ?, ?, ?, ?` once per row.
Each call is a network roundtrip. For 33,000+ rows this takes minutes and risks connection drops.

## CSV format (ffi.py input)

```
SeriesName, UnitId, Date, Value, Description, Extra
GeorgsCURVE,20,1761-01-01,896.1,date sjekk hist,med luft
sommervinter,20,2023-10-28T22:00:00,-2,lagret as utc,hei
```

- Date column can be either `YYYY-MM-DD` (date) or ISO datetime with time component
- UnitId is an integer reference
- Value is numeric (decimal)

## Proposed approach

### 1. Create staging table

```sql
CREATE TABLE dbo.BulkTemp (
    LoadsetName   NVARCHAR(64)   NOT NULL,
    SeriesName    NVARCHAR(64)   NOT NULL,
    Description   NVARCHAR(256)  NULL,
    UnitId        INT            NOT NULL,
    ValueDate     DATETIME       NOT NULL,   -- handles both date and datetime input
    Value         DECIMAL(18,8)  NOT NULL
);

CREATE NONCLUSTERED INDEX IX_BulkTemp_Loadset ON dbo.BulkTemp (LoadsetName);
```

### 2. Python: bulk insert into staging table

Use `pyodbc` `executemany` with `fast_executemany = True` to insert all rows into `BulkTemp` in one batch. This sends the entire dataset in a single network call.

```python
cursor.fast_executemany = True
cursor.executemany(
    "INSERT INTO BulkTemp (LoadsetName, SeriesName, Description, UnitId, ValueDate, Value) VALUES (?, ?, ?, ?, ?, ?)",
    rows
)
conn.commit()
```

### 3. Stored procedure: move from staging to production tables (upsert)

```sql
CREATE PROCEDURE dbo.UTILS_BulkUpsert
    @LoadsetName NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    -- Upsert from BulkTemp into production tables
    -- (uses same logic as UTILS_PutTime but set-based via MERGE or INSERT/UPDATE)
    --
    -- Step 1: Ensure Curve rows exist for each distinct SeriesName
    -- Step 2: MERGE CurveData — update existing date/value, insert new
    -- Step 3: Update CurveInfo descriptions if changed

    -- Clean up staging
    DELETE FROM dbo.BulkTemp WHERE LoadsetName = @LoadsetName;
END
```

### 4. Python workflow (new ffi.py)

```
1. DELETE FROM BulkTemp WHERE LoadsetName = @loadsetName   -- clear old staging data
2. Bulk INSERT from CSV into BulkTemp                      -- fast_executemany
3. EXEC UTILS_BulkUpsert @loadsetName                      -- set-based upsert
4. EXEC UTILS_UpdateCurveInfo @loadsetName                 -- update statistics
```

## Performance estimate

| Step | Current | Bulk |
|------|---------|------|
| 33,000 rows insert | ~5-10 min (row-by-row) | ~2-5 sec (bulk) |
| Upsert to production | included above | ~5-10 sec (set-based MERGE) |
| **Total** | **5-10 min** | **~15 sec** |

## Considerations

- **Description deduplication**: Description is per-series, not per-observation. The upsert proc should only update CurveInfo description once per distinct series, not once per row. This avoids redundant writes.
- **Date format**: `DATETIME` column handles both `YYYY-MM-DD` and `YYYY-MM-DDTHH:MM:SS` formats natively.
- **Error handling**: If the MERGE fails, staging data is preserved for debugging. Only delete staging after successful upsert.
- **Concurrency**: Add `WITH (HOLDLOCK)` on the MERGE target to prevent race conditions if multiple uploads run simultaneously.
- **Backwards compatible**: Keep `UTILS_PutTime` for single-row upserts (Excel add-in, interactive use). Bulk path is for batch ETL only.
