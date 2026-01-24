USE [Harmonize]
GO

/****** Object:  UserDefinedFunction [dbo].[IsAccessADr]    Script Date: 24/01/2026 17:40:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER   FUNCTION [dbo].[IsAccessADr] (@Loadsetid int)
RETURNS bit
--erik oct23 - oct 2025 loadset

WITH execute as owner
AS
Begin

return(
select  count(*)    from loadset LS
left join LoadSetADr LSAD on LSAD.LoadSetId = LS.Id
where 
(LS.Id = @loadsetid  and LS.accessAll = 1 and LS.Active=1 )  --alle access,  active remember
or
(  LSAD.loadsetid = @loadsetid   and  LSAD.active = 1 and  LSAD.Username =  ORIGINAL_LOGIN() )
)


END;
GO


