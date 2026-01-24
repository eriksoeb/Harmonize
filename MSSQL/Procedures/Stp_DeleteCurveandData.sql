USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Stp_DeleteCurveandData]    Script Date: 24/01/2026 17:31:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER         procedure [dbo].[Stp_DeleteCurveandData]
--erik 25 sletter i aktuell tabell iht tableid dynamisk
--erik 2026 testing access
@curveid int
AS
BEGIN
SET NOCOUNT ON;

declare @tablename varchar(128)
declare @dyndatasql varchar (1024)
declare @dyndefsql varchar (1024)
declare @loadsetid int

declare @curveidstr varchar(16) = cast(@curveid as varchar(16))


select  @tablename = DT.[Tablename], @loadsetid = LS.Id
from LoadSet LS
inner join Datatable DT on DT.tableid = LS.tableid 
inner join Curve C on C.loadsetid  = LS.Id
where C.CurveId = @curveid





if (dbo.IsAccessADw(@loadsetid ) = 0 )   --1,2,3 er access ok
begin
  DECLARE @msg nvarchar(400)
   SET @msg = 'Access denied for user:  '  + 'Original login: ' + ORIGINAL_LOGIN();
    THROW 50001, @msg, 1

end












set @dyndatasql = 'delete top(1000) from '+ @tablename + ' where curveid = '+@curveidstr 
--select @dyndatasql 

WHILE 1 = 1   
	BEGIN   
	--select (' sletter i loop')
	exec (@dyndatasql)

IF @@rowcount < 1000         
	BREAK;   
END


set @dyndefsql = 'delete from Curve where curveid = '+@curveidstr 
--select @dyndefsql
exec (@dyndefsql)
set @dyndefsql = 'delete from Curveinfo where curveid = '+@curveidstr 
--select @dyndefsql
exec (@dyndefsql)


end
GO


