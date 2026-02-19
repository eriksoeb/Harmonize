USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_GetColor]    Script Date: 19/02/2026 14:55:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER              procedure [dbo].[Client_GetColor]  

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;


select ColorId, ColorName, ColorCode  
from Client_Color 
order by ColorId


end
GO


