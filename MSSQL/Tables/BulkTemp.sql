USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveDate]    Script Date: 2026-09-12 20:06:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE TABLE dbo.BulkTemp (
    LoadsetName   NVARCHAR(64)   NOT NULL,
    SeriesName    NVARCHAR(64)   NOT NULL,
    Description   NVARCHAR(256)  NULL,
    UnitId        INT            NOT NULL,
    ValueDate     DATETIME       NOT NULL,   -- handles both date and datetime input
    Value         DECIMAL(18,8)  NOT NULL
);

CREATE NONCLUSTERED INDEX IX_BulkTemp_Loadset ON dbo.BulkTemp (LoadsetName);

go
