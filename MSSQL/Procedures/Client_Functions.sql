USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Functions]    Script Date: 24/01/2026 17:22:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER            procedure [dbo].[Client_Functions]  

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;


select * from functions where Active = 1 --order by 1 
union all

select 0 ,'None' , 'Default' , getdate(), getdate(), 1
--where ORIGINAL_LOGIN() = 'erik.soberg@ssb.no'


--union all
--select 300 ,'COUNT_MONTH' , 'utvikling' , getdate(), getdate(), 1
--where ORIGINAL_LOGIN() = 'erik.soberg@ssb.no'

order by 1
end
GO


