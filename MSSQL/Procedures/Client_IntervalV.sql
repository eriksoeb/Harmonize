USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_IntervalV]    Script Date: 24/01/2026 17:25:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure  [dbo].[Client_IntervalV]
WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
set nocount on

select * from IntervalV


end
GO


