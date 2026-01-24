USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Stp_DeleteLoadsetCurvesandData]    Script Date: 24/01/2026 17:32:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








--select * from Curve

CREATE OR ALTER       procedure [dbo].[Stp_DeleteLoadsetCurvesandData]
@loadsetid int
--perhaps take the name instead
AS
BEGIN
SET NOCOUNT ON;
--erik 2025



--if (2 >1) --alltid ok
if (dbo.IsAccessADw(@loadsetid ) <> 0 )   --1,2,3 er access ok
begin --write access is ok



select  Curveid into #ControlTable from 
dbo.Curve where loadsetid =  @loadsetid

declare @curveid int

while exists (select * from #ControlTable )
begin

select @curveid = (select top 1 curveid from #ControlTable order by 1)


exec dbo.Stp_DeleteCurveandData @curveid
  --select curveid, curvename from Curve  where Curveid =  @curveid

delete #ControlTable where CurveId =  @curveid

  
end --while


drop table #ControlTable
end --if access ok

else  --not access
begin
  DECLARE @msg nvarchar(400)
   SET @msg = 'Access denied for user:  '  + 'Original login: ' + ORIGINAL_LOGIN();
   THROW 50001, @msg, 1
end




end --proc



GO


