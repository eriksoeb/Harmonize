USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_FTrix]    Script Date: 24/01/2026 17:19:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO









CREATE OR ALTER       procedure [dbo].[Client_FTrix]  

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

select FTrixId as ID, Name as Name
from FTrix where Active = 1 order by 1
--not in use



end
GO


