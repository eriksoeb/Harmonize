USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[UTILS_UpdateCurveinfo]    Script Date: 24/01/2026 17:35:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE OR ALTER          procedure [dbo].[UTILS_UpdateCurveinfo]
--@loadsetId int
@loadsetName nvarchar(64)
--erik aug 2025 name as arg without type txt etc tbc



WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on


declare @loadsetId int

declare @basetablename nvarchar(64)

select @basetablename = DT.[Tablename] , @loadsetId=LS.Id
from LoadSet LS
left join Datatable DT on DT.tableid = LS.tableid where LS.Name = @loadsetName 




if @loadsetid >= 0 --alle skipper  debug
begin


--PRINT 'LS value gt 0  ' + CAST(@loadsetId AS VARCHAR(100));

--isnull curvename oct
--select top 20000 C.Curveid, @basetablename+Dtyp.name as Tablename ,C.Curvetypeid, C.CurveName into #ControlTable 
select top 20000 C.Curveid, @basetablename as Tablename ,C.Curvetypeid, C.CurveName into #ControlTable 
from dbo.Curve C
--left join dbo.Datatype Dtyp on Dtyp.id = C.DatatypeId
where loadsetid =  @loadsetId

--select * from #ControlTable

declare @curveid int
declare @mindate datetime
declare @maxdate datetime
declare @numofobs int
--declare @hist int --historie hit
declare @hit int --ny hit
declare @maxupdateddata datetime
DECLARE @temptable TABLE (mindate datetime, maxdate datetime, numofobs int, maxupdateddata datetime)


declare @diff_table TABLE ( VDate datetime,   Value [decimal](38, 8) )


declare @tablename nvarchar(128)  --med type
declare @curvename nvarchar(128)  --test
declare @diff [decimal](38, 8)
declare @curvetype int
declare @datatypename nvarchar(16)

declare @fdateStr nvarchar(32) --= quotename ((select convert (  varchar (16),  @fdate ,126) )+':00',char(39) )



DECLARE @procname sysname = QUOTENAME(OBJECT_SCHEMA_NAME(@@PROCID))
                        +'.'+QUOTENAME(OBJECT_NAME(@@PROCID))




declare @sql nvarchar(1024); --kjappere enn max holder i denne omg
declare @curveidstr nvarchar(16)

begin transaction
DELETE FROM CURVEINFO where loadsetid = @LoadsetId
commit

;

while exists (select * from #ControlTable )
begin
select @curveid = (select top 1  curveid 
from #ControlTable order by curveid)
--kan skrives i en go?
select @tablename = (select top 1 tablename
from #ControlTable order by curveid)



--kan skrives i en go?
select @curvename = (select top 1 CurveName
from #ControlTable order by curveid)

-- no successselect  @hist = (select isnull(Hit,0) from CurveInfo where CurveId = @curveid and Loadsetid = @loadsetId )
select @hit = (select COUNT(isnull(Passed,1)) from curveuseLog CUL where @curveid=CUL.Curveid and CUL.Loadsetid = @loadsetId )

--select @hist,@hit, @hist+@hit

select @curvetype = (select top 1  CurveTypeId
from #ControlTable order by curveid)

set  @curveidstr = cast(@curveid as varchar(16))

if (@curvetype = 1 ) --OR  @curvetype = 3)  --time
BEGIN


--PRINT 'curvetyoepe   ' + CAST(@curvetype AS VARCHAR(100));

set @sql = 'select min(vdate) , max(vdate) , count(vdate), max(updated) from '+@tablename +' where Curveid = '+@curveidstr + ' and value is not null '

--ok 3
insert into @temptable ( mindate, maxdate, numofobs, maxupdateddata)-- and more 
exec (@sql) --datoen inn i tabellen
select @mindate = mindate, @maxdate = maxdate, @numofobs = numofobs, @maxupdateddata = maxupdateddata from @temptable

--madness finding diff erik
set @sql = 'select top(2) VDate, Value -Lag(Value,1) over (ORDER BY VDate) from '+@tablename +' where Curveid = '+@curveidstr + ' and value is not null order by VDate desc '
insert  @diff_table ( VDate, Value )
exec (@sql)
--select * from @diff_table
set @diff =  (select top(1) Value from @diff_table)
--select @diff


END --if curvetype 1



   begin transaction
	insert into CurveInfo (Curveid, MinDate, MaxDate,NumOfObs, Updated,Calledby,Executedby,Executed,Loadsetid,Hit,LastDiff)
	Values (@curveid, @mindate, @maxdate, @numofobs, @maxupdateddata, 
	substring(ORIGINAL_LOGIN(), 1, 8),
	--ORIGINAL_LOGIN(), 
	@procname, getutcdate(), @loadsetid,@hit,@diff   ) --hist + ny hit + diff
	--commit
	--tbc commit i procen delted så hist is gone
	--update CurveInfo set hit = isnull(hit,0) + @hit where Loadsetid = @loadsetId and CurveId = @curveid
	commit

	delete @diff_table
  delete @temptable
  delete #ControlTable where CurveId =  @curveid
  
end

drop table #ControlTable
--delete from CurveUseLog where LoadsetId = @loadsetid
end  --debug erik 


end
GO


