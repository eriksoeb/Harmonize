USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[StpGetSeriesOuterTzls]    Script Date: 28/12/2025 00:10:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER procedure [dbo].[StpGetSeriesOuterTzls]  --forsøker på dyn intervall fra view, samt utv 
(	
--maa json format og @json..

--@mycurveid  int = null,
--erik juli26


@curvename nvarchar(64),
--@fdate  datetime =  NULL,
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@top int,
@sort nvarchar(4),
@json NVARCHAR(MAX)
)

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on


--declare @curvenamecomp as nvarchar(64) 

--declare @fdate datetime = NULL
--declare @curveid varchar(16) = cast(@mycurveid as varchar(16) )

declare @mystart datetime
declare @myend datetime

set datefirst 1 ;-- for at uker starter mandag
SELECT top(1) @mystart = [Start], --senere konverteres i inner ,
@myend  = [End] from IntervalV where IName = @myinterval

	

--declare @curveidstr varchar(16) = cast(@mycurveid as varchar(16) )
declare @curveid  int
declare @LoadsetId int -- = (select Loadsetid from Curve  where CurveId = @mycurveid)
declare @Curvetypeid int 
declare @DatatypeId int 
declare @SetName nvarchar(32) = substring (@curvename,1, CHARINDEX(':', @curvename)-1)



select
@curveid = C.Curveid,
@LoadsetId = LS.Id,
@curvetypeid = isnull(C.CurvetypeId,1),  --om ikke finnes bruker 1 tiddserie
@datatypeid = isnull(C.DatatypeId,10) --om ikke finnes decimal datatndard
from LoadSet LS
left join Datatable DT on DT.tableid = LS.tableid 
left join Curve C on LS.Id = C.loadsetid
left join Datatype DTyp on DTyp.id = C.DatatypeId
where 1=1 

and C.CurveName = substring (@curvename,CHARINDEX(':', @curvename)+1,64)
and LS.Name = @SetName --substring (@curvename,1, CHARINDEX(':', @curvename)-1)


declare @mnull varchar(10) = quotename('null',char(39) )

if @datatypeid <= 10 --@mytabletype <> 3  --vanlif decimLE er 10
begin

declare @output_table TABLE (DynDate datetime, VDate datetime,   Value [decimal](38, 8) , Epodateval nvarchar(64) ,CurveId int)
insert Into @output_table (DynDate, VDate, Value, Epodateval, CurveId)

EXEC  Stp_GetSeriesfromInnerTzls @SetName,@curveid ,@basis, @mystart,@myend, @agg,@top,@sort



 if   @datatypeid >  10 -- text data @mytabletype = 3
begin

declare @output_tablet32 TABLE (DynDate datetime, VDate datetime,   Value nvarchar(32) , Epodateval nvarchar(64) ,CurveId int)
insert Into @output_tablet32 (DynDate, VDate, Value, Epodateval, CurveId)
--EXEC  Stp_GetCurveDatafromInnerAggToptxt5 @curveid ,@fdate, @mystart,@myend, @tzconvert, @myTrixId,'NONE',@top,@sort
--tbc
EXEC  Stp_GetCurveDatafromInner2 @curveid ,@basis, @mystart,@myend, 'NONE',@top,@sort
  
if @format = 'json' --and text tbc nesting
begin

--select (select top (10000) Curveid, VDate, allerede top et
select (select Curveid, @CurveName as Name, VDate,
DynDate as DynDate,
Value  ,
JSON_QUERY ( Epodateval ) as Epo 
from  @output_tableT32
for JSON PATH ) as myJson
end

else -- clienten
begin
--maa toppe i dynamiske sqlen og bruke et @top argument top e rok
select  *  from @output_tableT32 
order by VDate desc -- fikser zoom
--select VDate,DynDate,  Epodateval from @output_table order by VDate
end




else --ingen curvetype ingen curve
begin
--select 'erik'
select (select 
'Curveid '+cast(@curveid as nvarchar(16) ) +' is not defined' as 'ApiError',
getdate() as 'RunDate'
for JSON PATH ) as myJson
end

--end
end

--select 'end of pure'
GO


