USE [Harmonize]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Sep 2026 erik - insert rows into BulkTemp staging table
-- Called by ffi.py via fast_executemany for bulk loading
-- Clears old staging data for the loadset on first call

CREATE OR ALTER PROCEDURE [dbo].[UTILS_BulkStagingInsert]
    @LoadsetName  NVARCHAR(64),
    @SeriesName   NVARCHAR(64),
    @Description  NVARCHAR(256),
    @UnitId       INT,
    @ValueDate    NVARCHAR(32),
    @Value        DECIMAL(38,8)
WITH EXECUTE AS OWNER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BulkTemp (LoadsetName, SeriesName, Description, UnitId, ValueDate, Value)
    VALUES (@LoadsetName, @SeriesName, @Description, @UnitId, @ValueDate, @Value);
END
GO
