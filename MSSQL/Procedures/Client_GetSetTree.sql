USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_GetSetTree]    Script Date: 24/01/2026 17:23:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER                    procedure [dbo].[Client_GetSetTree]
 --@myclientuser varchar(32) 
 --erik aug 2025 nested hierarki max 2 levels 2BC
 --using use defined type ass passing in a list od datasets to search
--CREATE TYPE [dbo].[StringList] AS TABLE([Item] [nvarchar](max) NULL)
--oct hierchild id i loadsettabele
--GO

WITH execute as owner
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

/*
select distinct Parent, LSG2.Name PName, LS.HierChildId Child, LSG.Name CName, LS.Name LSName 
from Loadset LS
left join LSHier_phasedout LSH on LSH.Child =  LS.HierChildId
left join LSGroups LSG on LSG.GrpId = LSH.Child  --flyttet 
left join LSGroups LSG2 on LSG2.GrpId = LSH.Parent 
left join LoadSetADr LSGR on LSGR.LoadSetId = LS.Id and  LSGR.Active = 1 
--and getdate()  >= isnull(LSGR.Fom,getdate()-1)
--and getdate()  <= isnull(LSGR.Tom,getdate()+1)
where 1 = 1 
and ( isnull(LS.AccessAll,1) = 1 or username = ORIGINAL_LOGIN() )  --fra sql server
and LSH.Child != 31 --ikke definert som undefined allerede
and LSH.Parent != 3 --ikke definert som undefined allerede
--and ( isnull(LS.AccessAll,1) = 1 or  Clientid = @myclientuser ) --fra windows
and ( isnull(LS.AccessAll,1) = 1 ) or  username = ORIGINAL_LOGIN()    --fra sql server otogonal es tbc
and Parent is not NULL
*/




select distinct LS.parentGroupId Parent, LSG2.Name PName, LS.HierChildId Child, LSG.Name CName, LS.Name LSName 
from Loadset LS
--left join LSHier LSH on LSH.Child =  LS.HierChildId
left join LSGroups LSG on LSG.GrpId = LS.HierChildId  
left join LSGroups LSG2 on LSG2.GrpId = LS.parentGroupId
left join LoadSetADr LSGR on LSGR.LoadSetId = LS.Id and  LSGR.Active = 1 
--and getdate()  >= isnull(LSGR.Fom,getdate()-1)
--and getdate()  <= isnull(LSGR.Tom,getdate()+1)
where 1 = 1 
and ( isnull(LS.AccessAll,1) = 1 ) --or username = ORIGINAL_LOGIN() )  --fra sql server
and LS.HierChildId != 31 --ikke definert som undefined allerede
and LS.parentGroupId != 3 --ikke definert som undefined allerede
--and ( isnull(LS.AccessAll,1) = 1 or  Clientid = @myclientuser ) --fra windows
--and ( isnull(LS.AccessAll,1) = 1 ) or  username = ORIGINAL_LOGIN()    --fra sql server otogonal es tbc
and LS.parentGroupId is not NULL







union all
select 3 as Parent, 'NoCategory' as PName, 31 as Child, 'Undefined' as CName, LS2.Name LSName 
from Loadset LS2
left join LoadSetADr LSGR on LSGR.LoadSetId = LS2.Id and  LSGR.Active = 1 
where 1 = 1 
--and ( isnull(LS.AccessAll,1) = 1 or  Clientid = @myclientuser ) --fra windows
--to test and ( isnull(LS2.AccessAll,1) = 1 )
--and LS2.LoadSetId not in ( select Loadsetid from LSHier)
and (LS2.HierChildId is NULL or LS2.HierChildId =31) -- ikke definert da undefinded
and LS2.Name is NOT NULL
--og tilgang
and (( isnull(LS2.AccessAll,1) = 1 ) or (LSGR.username = ORIGINAL_LOGIN() ))



--and LS2.Active = 1



union all
select 4 as Parent, 'NoCategory' as PName, 41 as Child, 'NoAccess' as CName, LS3.Name LSName 
from Loadset LS3
left join LoadSetADr LSGR on LSGR.LoadSetId = LS3.Id and  LSGR.Active = 1 
where 1 = 1 
and ( isnull(LS3.AccessAll,1) <> 1 )
and LS3.Active = 1
and LSGR.username <> ORIGINAL_LOGIN() 
--to test and ( isnull(LS3.AccessAll,1) = 1 )
and LS3.Name is NOT NULL


--order by 5
order by 1,2,3,4,5



end



GO


