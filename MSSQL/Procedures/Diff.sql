USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Diff(n)]    Script Date: 24/01/2026 17:29:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








CREATE OR ALTER            procedure [dbo].[Diff(n)]   
--aug 2023erik pct5 som renamet --dette er siste med ny modell på navn
(	
@curvename varchar(128),
--@mycurveid  int = null,
--@fdate datetime = null,
@basis int,
@myinterval  varchar(128) = null,
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
--@myforecast varchar(32) = null, --navnet på trixet
--@tzconvert int = 0,
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

--myfcastint int = (select top (1)  [Lag] from  ForecastLag where Name = @myforecast and Active = 1 );  --0, 24 osv --starten på forecasyet

--declare @myfcastint int = (select top (1)  [FTrixId] from  FTrix where Name = @myforecast and Active = 1 );  --0, 24 osv --
--1000, 2000, 3000 osv



declare @curveid  int
declare @LoadsetId int -- = (select Loadsetid from Curve  where CurveId = @mycurveid)
declare @Curvetypeid int 
declare @DatatypeId int 
declare @curveidstr nvarchar(16) 
declare @SetName nvarchar(32)  = substring (@curvename,1, CHARINDEX(':', @curvename)-1)


select
@curveid = C.Curveid,
@curveidstr  = cast(C.Curveid as nvarchar(16)),
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
LS.Name = @SetName --substring (@curvename,1, CHARINDEX(':', @curvename)-1)



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

 

--select @mycurveid,@fdate, @mystart, @myend, @tzconvert ,@myfcastLagType
--select * from @output_table


--gjør diff
insert Into @output_table (DynDate, VDate, Value, Epodateval, CurveId)
select DynDate, VDate,
--(Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate)) / LAG(Value,  @myfnlagint) over (ORDER BY VDate) *100 Value,
Value -LAG(Value,  @myfnlagint) over (ORDER BY VDate)  Value,

'['+
convert(varchar,
convert(bigint, datediff_big(ss,  '01-01-1970 00:00:00',VDate)) *1000 
)
+ ', '+
convert(varchar,convert(decimal(38,8),
VALUE -LAG(Value,  @myfnlagint) over (ORDER BY VDate) 
 --(Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate) )/ LAG(Value,  @myfnlagint) over (ORDER BY VDate)  *100

) )
+']'
as Epodateval, CurveId
 from @temp_table
where Value is NOT NULL -- diff taaler 0 and  Value <> 0
--ikke helt 100 den nullif tbc


--select * from @output_table where Epodateval is not NULL order by Vdate asc
--kan droppe siste steget om dropper nullen med lagen.

--nested
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


