USE [Harmonize]
GO

CREATE OR ALTER PROCEDURE [dbo].[UTILS_FillFreqDates]
(
    @freq  char(3),        -- ANN | MON | DAY | Q | QUA
    @start varchar(10) = null,  -- '2020-01-01' or just '2020' — defaults to 5 years ago
    @end   varchar(10) = null   -- '2026-12-31' or just '2026' — defaults to 2 years ahead
)

-- EXEC UTILS_FillFreqDates 'ANN', '2020', '2026'

  -- Full dates
  --EXEC UTILS_FillFreqDates 'ANN', '2020-01-01', '2026-12-31'
  --EXEC UTILS_FillFreqDates 'MON', '2020-01-01', '2026-12-31'
  --EXEC UTILS_FillFreqDates 'DAY', '2020-01-01', '2026-12-31'
 -- EXEC UTILS_FillFreqDates 'Q',   '2020-01-01', '2026-12-31'


WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on



    SET NOCOUNT ON;

    -- Resolve start/end: 4-char input treated as year only
    DECLARE @startDate date = CASE
        WHEN @start IS NULL              THEN DATEFROMPARTS(YEAR(GETDATE()) - 5, 1, 1)
        WHEN LEN(@start) = 4            THEN DATEFROMPARTS(CAST(@start AS int), 1, 1)
        ELSE CAST(@start AS date)
    END;

    DECLARE @endDate date = CASE
        WHEN @end IS NULL               THEN DATEFROMPARTS(YEAR(GETDATE()) + 2, 12, 31)
        WHEN LEN(@end) = 4              THEN DATEFROMPARTS(CAST(@end AS int), 12, 31)
        ELSE CAST(@end AS date)
    END;

    DECLARE @inserted int = 0;

    IF @freq = 'ANN'
    BEGIN
        -- Snap to Jan 1 of each year
        WITH DateRange AS (
            SELECT DATEFROMPARTS(YEAR(@startDate), 1, 1) AS FreqDate
            UNION ALL
            SELECT DATEADD(YEAR, 1, FreqDate)
            FROM DateRange
            WHERE FreqDate < DATEFROMPARTS(YEAR(@endDate), 1, 1)
        )
        INSERT INTO FreqDates_ANN (FreqDate)
        SELECT d.FreqDate FROM DateRange d
        WHERE NOT EXISTS (SELECT 1 FROM FreqDates_ANN e WHERE e.FreqDate = d.FreqDate)
        OPTION (MAXRECURSION 0);
        SET @inserted = @@ROWCOUNT;
    END

    ELSE IF @freq = 'MON'
    BEGIN
        -- Snap to 1st of each month
        WITH DateRange AS (
            SELECT DATEFROMPARTS(YEAR(@startDate), MONTH(@startDate), 1) AS FreqDate
            UNION ALL
            SELECT DATEADD(MONTH, 1, FreqDate)
            FROM DateRange
            WHERE FreqDate < DATEFROMPARTS(YEAR(@endDate), MONTH(@endDate), 1)
        )
        INSERT INTO FreqDates_MON (FreqDate)
        SELECT d.FreqDate FROM DateRange d
        WHERE NOT EXISTS (SELECT 1 FROM FreqDates_MON e WHERE e.FreqDate = d.FreqDate)
        OPTION (MAXRECURSION 0);
        SET @inserted = @@ROWCOUNT;
    END

    ELSE IF @freq = 'DAY'
    BEGIN
        WITH DateRange AS (
            SELECT @startDate AS FreqDate
            UNION ALL
            SELECT DATEADD(DAY, 1, FreqDate)
            FROM DateRange
            WHERE FreqDate < @endDate
        )
        INSERT INTO FreqDates_DAY (FreqDate)
        SELECT d.FreqDate FROM DateRange d
        WHERE NOT EXISTS (SELECT 1 FROM FreqDates_DAY e WHERE e.FreqDate = d.FreqDate)
        OPTION (MAXRECURSION 0);
        SET @inserted = @@ROWCOUNT;
    END

    ELSE IF @freq IN ('Q', 'QUA')
    BEGIN
        -- Snap start to nearest quarter begin (Jan/Apr/Jul/Oct)
        DECLARE @qStart date = DATEFROMPARTS(
            YEAR(@startDate),
            (((MONTH(@startDate) - 1) / 3) * 3) + 1,
            1
        );
        WITH DateRange AS (
            SELECT @qStart AS FreqDate
            UNION ALL
            SELECT DATEADD(MONTH, 3, FreqDate)
            FROM DateRange
            WHERE FreqDate < @endDate
        )
        INSERT INTO FreqDates_Q (FreqDate)
        SELECT d.FreqDate FROM DateRange d
        WHERE NOT EXISTS (SELECT 1 FROM FreqDates_Q e WHERE e.FreqDate = d.FreqDate)
        OPTION (MAXRECURSION 0);
        SET @inserted = @@ROWCOUNT;
    END

    ELSE
    BEGIN
        SELECT 'Unknown @freq: use ANN, MON, DAY, Q or QUA' AS Error;
        RETURN;
    END

    SELECT @freq AS Freq, @startDate AS StartDate, @endDate AS EndDate, @inserted AS RowsInserted;
END
GO
