USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Stp_DeleteLoadsetDataOnly]    Script Date: 24/01/2026 17:33:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




--select * from Curve

CREATE OR ALTER   procedure [dbo].[Stp_DeleteLoadsetDataOnly]
@loadset int
AS
BEGIN
SET NOCOUNT ON;
--erik 2025



select  Curveid into #ControlTable from 
dbo.Curve where loadsetid =  @loadset

declare @curveid int

while exists (select * from #ControlTable )
begin

select @curveid = (select top 1 curveid from #ControlTable order by 1)


exec dbo.Stp_DeleteCurveDataOnly @curveid
--select curveid, curvename from Curve  where Curveid =  @curveid

delete #ControlTable where CurveId =  @curveid

  
end


drop table #ControlTable


end



GO


