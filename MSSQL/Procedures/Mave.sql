USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Mave(n)]    Script Date: 24/01/2026 17:30:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER            procedure [dbo].[Mave(n)]   
--sep 2023erik 
(	
@curvename varchar(128),
@basis int,
@myinterval  varchar(128) = null,
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  varchar(8) = null,
@agg  varchar(16), -- = 'NONE',  --endret fra p
@top int,
@sort varchar(4),
@json NVARCHAR(MAX)
)

--text is on

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on


--declare @fdate datetime = NULL



declare @mystart datetime
declare @myend datetime
SELECT top(1) @mystart = [Start], --senere konverteres i inner ,
@myend  = [End] -- konverteres evt i inner 
from IntervalV where IName = @myinterval


declare @curveid  int
declare @LoadsetId int -- = (select Loadsetid from Curve  where CurveId = @mycurveid)
declare @Curvetypeid int 
declare @DatatypeId int 
declare @SetName nvarchar(32)  = substring (@curvename,1, CHARINDEX(':', @curvename)-1)

declare @curveidstr varchar(16) 

select
@curveid = C.Curveid,
@curveidstr  = cast(C.Curveid as varchar(16)),
@LoadsetId = LS.Id,
@curvetypeid = isnull(C.CurvetypeId,1),  --om ikke finnes bruker 1 tiddserie
@datatypeid = isnull(C.DatatypeId,10) --om ikke finnes decimal datatndard
from LoadSet LS
left join Datatable DT on DT.tableid = LS.tableid 
left join Curve C on LS.Id = C.loadsetid
left join Datatype DTyp on DTyp.id = C.DatatypeId
where 
C.CurveName = substring (@curvename,CHARINDEX(':', @curvename)+1,64)
and 
LS.Name = @SetName
--LS.Name = substring (@curvename,1, CHARINDEX(':', @curvename)-1)






if (@datatypeid <= 10 )-- pct om ikke text 10 er decimal txt and other > 10
begin 

declare @output_table TABLE (DynDate datetime, VDate datetime,   Value [decimal](38, 8) , Epodateval varchar(64), CurveId int)
declare @temp_table TABLE (DynDate datetime, VDate datetime,   Value [decimal](38, 8) , Epodateval varchar(64), CurveId int)


insert Into @temp_table (DynDate, VDate, Value, Epodateval, CurveId )

EXEC  Stp_GetSeriesFromInnerTzls
@SetName,
@curveid ,
@basis ,
@mystart ,
@myend , 
@agg,
@top,
@sort 

 


--gjør mave
insert Into @output_table (DynDate, VDate, Value, Epodateval, CurveId)
select DynDate, VDate,
--(Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate)) / LAG(Value,  @myfnlagint) over (ORDER BY VDate) *100 Value,
--Value -LAG(Value,  @myfnlagint) over (ORDER BY VDate)  Value,
(Value +LAG(Value,  @myfnlagint) over (ORDER BY VDate))/2   Value,

'['+
convert(varchar,
convert(bigint, datediff_big(ss,  '01-01-1970 00:00:00',VDate)) *1000 
)
+ ', '+
convert(varchar,convert(decimal(38,8),
--VALUE -LAG(Value,  @myfnlagint) over (ORDER BY VDate) 
(Value +LAG(Value,  @myfnlagint) over (ORDER BY VDate))/2 
-- (Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate) )/ LAG(Value,  @myfnlagint) over (ORDER BY VDate)  *100
--(Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate) )/ nullif(LAG(Value,  @myfnlagint) over (ORDER BY VDate) ,0 ) *100
) )
+']'
as Epodateval, CurveId
 from @temp_table
where Value is NOT NULL and  Value <> 0
--ikke helt 100 den nullif tbc


--select * from @output_table where Epodateval is not NULL order by Vdate asc
--kan droppe siste steget om dropper nullen med lagen.

if @format = 'json'
begin
select (select @Curveid as Curveid, 
@CurveName as Name,
	Obs =(
	select
	VDate,
	DynDate as DynDate,
	Value , 
	JSON_QUERY( Epodateval )as Epo 
	--kan jeg lage Epodateval her isteden.??
	from  @output_table 
	for JSON Path
	)
for JSON PATH ) as myJson
end


else -- clienten
begin
select * from @output_table  where Epodateval is not NULL order by VDate
end



end --if text


else --ingen curvetype ingen curve
begin
select (select 
isnull(@curveid,'-999')  as 'CurveId',
@curvename as CurveName,
@DatatypeId as DataType,
@agg as Agg,
'Not decimal, or no aggregation requested' as 'ApiError',
getdate() as 'ErrorDate'
for JSON PATH ) as myJson
end







end
GO


