USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Stp_DeleteCurveDataOnly]    Script Date: 24/01/2026 17:32:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER    procedure [dbo].[Stp_DeleteCurveDataOnly]
--loadset
--erik 2025 sletter i aktuell tabell iht tableid dynamisk
--exec Stp_DeleteCurveDataOnly 101437
--when running this proc u should use the UTILS updatecurveInfo later to update the stats on the loadset
--2026 testing access

@curveid int
AS
BEGIN
SET NOCOUNT ON;

declare @tablename varchar(128)
declare @dyndatasql varchar (1024)
declare @dyndefsql varchar (1024)
declare @loadsetid int


declare @curveidstr varchar(16) = cast(@curveid as varchar(16))


--select 'deleter ...'+@curveidstr




select  @tablename = DT.[Tablename], @loadsetid =LS.Id
from LoadSet LS
inner join Datatable DT on DT.tableid = LS.tableid 
inner join Curve C on C.loadsetid  = LS.Id
where C.CurveId = @curveid


--select  @loadsetid


if (dbo.IsAccessADw(@loadsetid ) = 0 )   --1,2,3 er access ok
begin
  DECLARE @msg nvarchar(400)
   SET @msg = 'Access denied for user:  '  + 'Original login: ' + ORIGINAL_LOGIN();
    THROW 50001, @msg, 1

end



--select @tablename

set @dyndatasql = 'delete top(1000) from '+ @tablename + ' where curveid = '+@curveidstr 
--select @dyndatasql 

WHILE 1 = 1   
	BEGIN   
	--select (' sletter i loop')
	exec (@dyndatasql)

IF @@rowcount < 1000    
	begin
	return;
	BREAK;   
	end
set nocount off
END


end
GO


