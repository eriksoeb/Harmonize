USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Aggregate]    Script Date: 22/04/2026 13:08:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--exec Client_FreqType



CREATE  or alter  procedure [dbo].[Client_FreqType]  

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

select Id, Name from dbo.FreqType where Active = 1
union select -1,'OFF'  --default



end
GO


