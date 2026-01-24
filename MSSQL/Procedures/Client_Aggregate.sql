USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Aggregate]    Script Date: 24/01/2026 17:16:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO











CREATE OR ALTER           procedure [dbo].[Client_Aggregate]  

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

--select FTrixId as ID, Name as Name
--from FTrix where Active = 1 order by 1
select 0 as ID, 'None' as Name
union all
select 1 as ID, 'AVG_YEAR' as Name
union all
select 2 as ID, 'AVG_QUARTER' as Name
union all
select 3 as ID, 'AVG_MONTH' as Name
union all
select 4 as ID, 'AVG_DAY' as Name
union all
select 5 as ID, 'SUM_DAY' as Name
union all
select 6  as ID, 'SUM_YEAR' as Name
union all
select 7 as ID, 'LAST_MONTH' as Name




end
GO


