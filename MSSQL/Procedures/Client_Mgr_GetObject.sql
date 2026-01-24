USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Mgr_GetObject]    Script Date: 24/01/2026 17:25:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER  procedure [dbo].[Client_Mgr_GetObject]
--erik juli 2022
@objectname varchar (32)   
--maa ha med type tbc

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select top 1 [Name], Pid1, isnull(Pid2,'NULL') Pid2, isnull(Pid3,'NULL') 
Pid3, Orderby, isnull(Topp,1000) as Topp

from Client_TableMgr 
 where [Name] = @objectname




 end
GO


