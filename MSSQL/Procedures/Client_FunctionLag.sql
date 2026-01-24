USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_FunctionLag]    Script Date: 24/01/2026 17:21:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   procedure  [dbo].[Client_FunctionLag]
WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
set nocount on

select 1 as FnLag
union all

select 2 as FnLag
union all

select 3 as FnLag
union all

select 4 as FnLag
union all

select 6 as FnLag

union all
select 12 as FnLag

end
GO


