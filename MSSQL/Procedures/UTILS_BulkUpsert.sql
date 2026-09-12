USE [Harmonize]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Sep 2026 erik - bulk upsert from BulkTemp staging table
-- Replaces row-by-row UTILS_PutTime for batch ETL loads
-- Same logic as UTILS_PutTime but set-based

CREATE OR ALTER PROCEDURE [dbo].[UTILS_BulkUpsert]
    @passedlsname NVARCHAR(64)
WITH EXECUTE AS OWNER
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    DECLARE @loadsetid INT = (SELECT Id FROM LoadSet WHERE Name = @passedlsname);

    IF @loadsetid IS NULL
    BEGIN
        THROW 50001, 'UPS! no loadset Found', 1;
        RETURN;
    END

    -- Check write access
    IF (dbo.IsAccessADw(@loadsetid) <= 0)
    BEGIN
        DECLARE @msg NVARCHAR(400);
        SET @msg = 'Access denied for user: Original login: ' + ORIGINAL_LOGIN();
        THROW 50001, @msg, 1;
        RETURN;
    END

    -- Get target data table name
    DECLARE @tablename NVARCHAR(128);
    SELECT @tablename = ISNULL(DT.[Tablename], 'CurveData')
    FROM LoadSet LS
    INNER JOIN Datatable DT ON DT.tableid = LS.tableid
    WHERE LS.Id = @loadsetid;

    -- Step 1: Ensure all series exist in Curve table (one row per series)
    MERGE Curve AS [Target]
    USING (
        SELECT SeriesName, Description, UnitId
        FROM (
            SELECT SeriesName, Description, UnitId,
                   ROW_NUMBER() OVER (PARTITION BY SeriesName ORDER BY ValueDate DESC) AS rn
            FROM dbo.BulkTemp
            WHERE LoadsetName = @passedlsname
        ) X
        WHERE rn = 1
    ) AS [Source]
    ON [Target].CurveName = [Source].SeriesName
       AND [Target].LoadSetID = @loadsetid
    WHEN MATCHED THEN
        UPDATE SET [Target].Doc = 'doc to tbc'
    WHEN NOT MATCHED THEN
        INSERT (CurveName, CurveTypeId, DataTypeId, Descr, Doc, Unit_id, LoadSetId, Created)
        VALUES ([Source].SeriesName, 1, 10, [Source].Description, NULL, [Source].UnitId, @loadsetid, GETDATE());

    -- Step 2: Upsert data into target table (CurveData or custom)
    DECLARE @sql NVARCHAR(MAX);
    SET @sql = '
    MERGE ' + @tablename + ' AS [Target]
    USING (
        SELECT
            C.CurveId,
            B.ValueDate AS VDate,
            B.Value
        FROM (
            SELECT SeriesName, ValueDate, Value,
                   ROW_NUMBER() OVER (PARTITION BY SeriesName, ValueDate ORDER BY (SELECT NULL)) AS rn
            FROM dbo.BulkTemp
            WHERE LoadsetName = ' + QUOTENAME(@passedlsname, CHAR(39)) + '
        ) B
        INNER JOIN Curve C
            ON C.CurveName = B.SeriesName
           AND C.LoadSetID = ' + CAST(@loadsetid AS VARCHAR(16)) + '
        WHERE B.rn = 1
    ) AS [Source]
    ON [Target].CurveId = [Source].CurveId
       AND [Target].VDate = [Source].VDate
    WHEN MATCHED THEN
        UPDATE SET [Target].Value = [Source].Value, [Target].Updated = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (CurveId, VDate, Value, Updated)
        VALUES ([Source].CurveId, [Source].VDate, [Source].Value, GETDATE());';

    EXEC (@sql);

    -- Step 3: Clean up staging data
    DELETE FROM dbo.BulkTemp WHERE LoadsetName = @passedlsname;

    PRINT 'Bulk upsert completed for loadset: ' + @passedlsname;
END
GO
