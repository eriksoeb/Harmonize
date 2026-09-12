USE [Harmonize]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Sep 2026 erik - clear staging table before bulk load
-- Called by ffi.py before inserting new rows

CREATE OR ALTER PROCEDURE [dbo].[UTILS_BulkStagingClear]
    @LoadsetName NVARCHAR(64)
WITH EXECUTE AS OWNER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.BulkTemp WHERE LoadsetName = @LoadsetName;
END
GO
