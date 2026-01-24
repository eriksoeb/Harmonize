USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Mgr_Objects]    Script Date: 24/01/2026 17:26:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








CREATE OR ALTER  procedure [dbo].[Client_Mgr_Objects]
--erik juli 2022
@winusername varchar (32)   

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select distinct  Object_type as Otype, * from Client_TableMgr 
where 1 = 1
and (@winusername = RAccess or RAccess = '*')
--and DBType =1
and Object_type  ='U'

union all

select distinct Object_type as Otype, * from Client_TableMgr  --Views
where 1 = 1
and (@winusername = RAccess or RAccess = '*')
and Object_type  ='V'

union all


select distinct Object_type as Otype, * from Client_TableMgr  --StoredProcs
where 1 = 1
and (@winusername = RAccess or RAccess = '*')
and Object_type  ='P'

union all

select distinct  Object_type+'P' as Otype, * from Client_TableMgr --update tables 
where 1 = 1
and ( @winusername = WAccess or WAccess = '*')
and Object_type  ='U'


 end
GO


