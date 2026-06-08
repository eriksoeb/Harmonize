USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Stp_GetSeriesFromInnerTzls]    Script Date: 23/04/2026 09:42:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE OR ALTER            Procedure [dbo].[Stp_GetSeriesFromInnerTzls]  -- dyn intervall fra view, samt utv 
--erik aug 23
--basis agg inner tst
--okt 19 krever loadsset tabell tzone
--okt 24 ryddet for tz, ok, TBc aggregation ha med i @date
--tbc tidsone baseyear (obs)
--LAST fix 28 oct - ikke tz impl. my not be needed
--dec 2025,dec17

(
@SetName  nvarchar(32) = null,
@mycurveid  int = null,
@basis int, --basis year periode ikke lenger date ikke nais, men works.
@mystart datetime,
@myend datetime,
@aggp nvarchar(16) ,
@convert2freq char(3) = 'OFF',  -- ANN | MON | DAY | OFF (OFF = direct from table, no FreqDates join)
@sort nvarchar(4),
@top int =1000
)

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;


--declare @fdate datetime  = null --NOT needed ??? tbc
declare @agg1 nvarchar(8)
--declare @min nvarchar(8)
declare @grpby nvarchar(64)
declare @grpfor nvarchar(128)
declare @firstlast nvarchar(64) = ' and 2=2 '
declare @basisagg nvarchar(8) = 'AVG' --default om no agrregate given
declare @Vdate nvarchar (32) ='(VDate)'  --ta med tidsonen allere her !!!! 

if (@aggp !='None')
begin
	--select ' aggregate'
	--set @vdate
	--declare @tzDate nvarchar(128)= 'quotename(cast( VDate as varchar (32)),char(39)) as datetime)  AT TIME ZONE UTC' --TBC

	--set @min ='min'
	set @agg1 = substring ( @aggp, 1, charindex('_',@aggp) -1)
	--select @agg1
	set @basisagg = @agg1
	declare @freq nvarchar(8)  = substring ( @aggp, charindex('_',@aggp)+1 , 16)
	--set @grpby =' group by Curveid, datediff('+@freq+',0,VDate)'
	--set @grpfor =' select min(Vdate) VDate, AVG(Value) as Value, max(CurveId) as CurveId from ('
	set @grpfor =' select min(Vdate) DynDate, min(Vdate) VDate, '+@agg1+'(Value) as Value,NULL as EpoDateval, max(CurveId) as CurveId from ('
	--inn her: blir nested sql
	set @grpby = ' )p1 group by datediff('+@freq+',0,VDate)'

	--SELECT @grpby
	--SELECT ' group by Curveid, datediff('+@freq+',0,'+@tzDate +')'

	--maa testes etter tidsone....to be cont
	--revisit
	if (@agg1 = 'LAST')
		begin
		set @firstlast = ' and MONTH(VDate) = 12 '
		set @grpfor =''
		set @grpby =' '--group by Curveid, datediff('+@freq+',0,VDate)'
		--IKKNO AGGREGATE
		set @basisagg = ' '
		set @Vdate ='dateadd(MONTH,-11,VDate) '
		--set @min ='dateadd'
		set @agg1 = ' '--avg' 
		--select 'last', @firstlast, @freq
	end

	
 end
 else begin
	--set @min =''
	set @grpby =''
	set @grpfor =''
	--IKKNO AGGREGATE
	set @agg1 = '' --om common basis uten aggregate
  end
end

--select @agg1, @min, @grpby

--set datefirst 1 ;-- for at uker starter mandag ligger i pureagg

declare @curveidstr nvarchar(16) = cast(@mycurveid as nvarchar(16))
declare @offset int = 0
declare @offsetStr nvarchar(4) 
--declare @fdateoff datetime


--not needed declare @myfcastint int = (select top (1)  [Lag] from  ForecastLag where Name = @myforecast and Active = 1 );  --0, 24 osv --starten p forecasyet

--aug--declare @myfcastLagType int = @flagtype -- (select top (1)  [LagType] from  ForecastLag where Name = @myforecast and Active = 1 ); 

--declare @myfnlagint int = cast(@myfnlag as int ); --men her er ingen funk sbrukes ikke
--ned declare @mytabletype int = (select  CurveType from LoadSets where LoadsetId = ( select LoadsetId from Curve  where CurveId = @mycurvid));

declare @tablename nvarchar(128)
--ut oct 24declare @myconverttocet bit
declare @datatypeid int = null
declare @curvetypeid int = null
declare @LoadsetId int  --nov 10 utgaar timezone still for ADccess
DECLARE @targettimezone AS sysname --henter fra tabell okt 2023


--Default
declare @intervalid int
--just in case
if @top > 25000
begin
	set @top = 25000
end 


select 
@LoadsetId = LS.Id,
@tablename = DT.[Tablename],--erik dec25 !! +isnull(DTyp.[Name],'') ,
@curvetypeid = isnull(C.CurvetypeId,1), --om ikke finnes bruker tidsserie
@datatypeid = isnull(C.DatatypeId,10) , --om ikke finnes bruker 10 decimal
--@myconverttocet =isnull(LS.ConvertToCET,0),
@targettimezone =isnull(LS.c2timezone,'UTC'),  --default konverterer ikke
@intervalid  = isnull(IntervalID,1)
from LoadSet LS
left join Datatable DT on DT.tableid = LS.tableid 
left join curve C on LS.Id = c.loadsetid
--left join Datatype DTyp on DTyp.id = C.DatatypeId ut med denne
where 1= 1  -- LS.LoadsetId = @LoadsetId 
and c.curveid = @mycurveid
and LS.Name = @SetName

--select @tablename, @targettimezone, @SetName, @intervalid,@loadsetid

--time date interval
if ( @mystart is null  ) --default
	SELECT top(1) @mystart = [Start], 
	@myend  = [End] from IntervalV where ID = @intervalid



--DECLARE @targettimezone AS sysname = 'UTC';  --omnbingenting anngitt vises data as is
--DECLARE @targettimezone AS sysname = 'Central European Standard Time';  --omnbingenting anngitt vises data as is
--DECLARE @targettimezone AS sysname = 'US Eastern Standard Time';  --omnbingenting anngitt vises data as is
DECLARE @targettimezoneq AS sysname = quotename( @targettimezone,char(39));  --omnbingenting anngitt vises data as is
--kan kanskje ta dene i en go



declare @sql nvarchar(4000); --kjapper enn max holder i denne omg 4000 max
declare @asql nvarchar(4000); --kjapper enn max holder i denne omg 4000 max

--declare @output_table TABLE (DynDate datetime, VDate datetime,   Value [decimal](18, 8) , Epodateval varchar(64))
--declare @passed bit = dbo.IsAccess(@loadsetid) utgaar pga adgrupper
declare @passedAD int = dbo.IsAccessADr(@loadsetid) --readaccess



--//logging not needed..
INSERT INTO CurveUseLog
VALUES (  getutcdate(), @mycurveid, @loadsetid, @passedAD, 
substring(ORIGINAL_LOGIN(), 1, 62))

Update dbo.curveinfo
set Hit = isnull(Hit,0) +1
where curveid = @mycurveid






if (@passedAD <1) --testing read access
begin
	--set @sql = 'select '+ quotename('no access', char(39) ) + ' as Sorrry' 
	--exec (@sql)
	select 'No Access' as myJson
RETURN
--no access
--set @sql = 'select '+ quotename('no access', char(39) ) + ' as Sorrry' 
--set @sql = 'select NULL as CurveId,  NULL as Name, Null as Obs, Null as Value'
--select ' as Sorrry' 
end


--else if (@mytabletype = 1 ) -->Numeriske tidsserier --and dbo.IsAccess(@loadsetid) = 1) --access not needed
--om nulls maa highchart ha connectNulls: true
--value brukes ikke? kan utelates..


else
if ( @datatypeid=10 AND @curvetypeid= 1) --> Numeriske tidsserier
begin
--tallet man deler paaq
declare @basis_txt nvarchar(32) --= cast(@basis as varchar(10))
declare @multiplier nvarchar(3) 

declare @basis_dec decimal(38,8)


declare @basis_yyyy nvarchar(4) = left(cast(@basis as varchar(16)),4)
declare @basis_mnds nvarchar(32) =''  --ingen months eller flere--01,02,03
declare @qry_basism nvarchar(128) =''

--print 'testing basis 3000'

--baseis er param
if (@basis > 3000) --20200203 febmar m?neder4 IKKE PENT, MEN SLIPPER ET ELLER FLERE EXTRA BASIS ARG..
begin
	set @basis_mnds  ='00' --01,02,03
	set @basis_txt = CAST(@basis as varchar(32))
	--select @basis as integ, @basis_txt as mtxt ,RIGHT(cast(@basis as varchar(16)),2) as mnd, left(cast(@basis as varchar(16)),4) as yyyy

	while (LEN(@basis_txt) >4 )
	begin
		--print '> 4 '+@basis_txt
		set @basis_mnds = @basis_mnds+ ','+ RIGHT(cast(@basis_txt as varchar(16)),2)
		set @basis_txt = substring(@basis_txt,1,len(@basis_txt)-2)
		
	end

	--print 'while '+@basis_txt
	--print '@basis_mnds : '+@basis_mnds

	
	 SET @qry_basism =
    ' AND MONTH(CAST((VDate AT TIME ZONE ' + @targettimezoneq + ') AS datetime)) IN (' + @basis_mnds + ')';



	--cast(cast('+@VDate+' as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as datetime) as VDate,'


	--print '@qry_basism :'+@qry_basism
	
end


--select @basis_yyyy as basis_yyyy, @basis_mnds as basis_mnds--, @basis
--select @qry_basism as qrybasismnd
--select @min+@vdate


if (@basis > 1950 ) --and @basis < 3000)
begin
--print 'basis test 1950'
--select '1950 basiscalc',@basis as basisparam

DECLARE @SQLString NVARCHAR(500); 
DECLARE @ParmDefinition NVARCHAR(500); 
--SET @SQLString = N'(select @baseOUT = '+@agg1+'(value) from '+@tablename+' where curveid = '+@curveidstr+ ' and  YEAR(Vdate) = '+cast(@basis as nvarchar(32) )+ ' )'
--SET @SQLString = N'(select @baseOUT = AVG(value) from '+@tablename+' where curveid = '+@curveidstr+ ' and  YEAR(Vdate) = '+cast(@basis as nvarchar(32) )+ ' )'

--tz m? inn se over for mnd
--SET @SQLString = N'(select @baseOUT = '+@basisagg+'(value) from '+@tablename+' where curveid = '+@curveidstr+ ' and  YEAR(Vdate) = '+@basis_yyyy+@qry_basism + ' ) '


SET @SQLString = N'(select @baseOUT = '+@basisagg+'(value) from '+@tablename+' where curveid = '+@curveidstr+ ' and  YEAR(cast((Vdate at TIME ZONE ' +@targettimezoneq +') AS datetime)) = '+@basis_yyyy+@qry_basism + ' ) '


--print @sqlstring


SET @ParmDefinition = N'@baseOUT decimal(24,8) OUTPUT';  --var 12
EXECUTE sp_executesql @SQLString, @ParmDefinition,  @baseOUT=@basis_dec OUTPUT;
set @basis_txt = cast(@basis_dec as nvarchar(32))
set @multiplier = '100'
end --basis
else
begin
set @basis_txt = '1'
set @multiplier = '1'--debug
end


--print 'basistext: '+@basis_txt

--Value/'+@basis_txt+' as Value, '+
--' select min(Vdate) VDate, avg(Value) Value, max(CurveId) as CurveId from ('+
/*
select @grpfor
select @grpby
select @min
select @agg1
set @agg1 =''
select @agg1
select @Vdate

set @agg1 =''
--agg1 kan tas ut under okt 27

/*
--first main sql ok before force
set @sql = 
 'declare @mnull varchar(10) = '+ quotename('null',char(39) ) +
  + @grpfor 
+ ' select top('+cast(@top as nvarchar(32)) + ')'
--+ ' NULL as DynDate2, '
+ ' Vdate as DynDate,
cast(cast('+@VDate+' as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as datetime) as VDate,'
+'(Value)*'+@multiplier+'/'+@basis_txt+' as Value, '+
quotename('[',char(39)) + 
'+ convert(varchar,' +
' convert(bigint, datediff_big(MILLISECOND, '+ quotename('01-01-1970 00:00:00',char(39)) +', cast(cast(VDate as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as  datetime))) *1) +'
+ quotename(', ',char(39) )
+ ' + isnull(convert(varchar,convert(decimal(38,8),(Value)*'+@multiplier+'/'+@basis_txt+')), @mnull) +' 
+ quotename(']',char(39) )
+ ' as Epodateval '
+ ', '+@curveidstr + ' as CurveId'
+ ' from '+ @tablename
+ ' where CurveId =  ' +cast(@mycurveid as nvarchar(16))
+ ' and VDate >= cast ( ' + quotename(cast( @mystart as varchar (32)),char(39))+ 'as datetime)  AT TIME ZONE '+@targettimezoneq
+ ' and VDate < cast ( ' + quotename(cast( @myend as varchar (32)),char(39)) + 'as datetime)  AT TIME ZONE '+@targettimezoneq
--erik endret <= til < pga current year intervaller, hva er mest brukervemmlig ??
--oct24
--+ ' and VDate >= cast ( ' + quotename(cast( @mystart as nvarchar (32)),char(39))+ 'as datetime)  '
--+ ' and VDate < cast ( ' + quotename(cast( @myend as nvarchar (32)),char(39)) + 'as datetime)  '
+ @firstlast
+ @grpby
+ ' order by 1,2 '+@sort
--+' ) p1 group by datediff(DAY,0,VDate) '

--exec (@sql)
--select @sql
--force 

--'declare @mnull varchar(10) = '+ quotename('null',char(39)) +*/
*/



-- build FROM/JOIN/WHERE depending on @convert2freq
declare @dateCol     nvarchar(64)
declare @fromClause  nvarchar(512)
declare @joinClause  nvarchar(512)
declare @whereClause nvarchar(512)

if @convert2freq = 'OFF'
begin
    set @dateCol     = 'c.VDate'
    set @fromClause  = ' from ' + @tablename + ' c'
    set @joinClause  = ''
    set @whereClause = ' where c.CurveId = ' + @curveidstr
                     + ' and c.VDate >= cast(' + quotename(cast(@mystart as varchar(32)),char(39)) + ' as datetime)'
                     + ' and c.VDate <  cast(' + quotename(cast(@myend as varchar(32)),char(39)) + ' as datetime)'
end
else
begin
    set @dateCol     = 'f.FreqDate'
    set @fromClause  = ' from FreqDates_' + @convert2freq + ' f'
    set @joinClause  = ' left join ' + @tablename + ' c on c.VDate = f.FreqDate'
                     + '   and c.CurveId = ' + @curveidstr
    set @whereClause = ' where f.FreqDate >= cast(' + quotename(cast(@mystart as varchar(32)),char(39)) + ' as datetime)'
                     + '   and f.FreqDate <  cast(' + quotename(cast(@myend as varchar(32)),char(39)) + ' as datetime)'
end

 set @sql =
   'declare @mnull varchar(10) = '+ quotename('null',char(39)) +
    + @grpfor
  + ' select top('+cast(@top as nvarchar(32)) + ')'
  + ' ' + @dateCol + ' as DynDate,'
  + ' cast(cast(' + @dateCol + ' as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as datetime) as VDate,'
  + ' (c.Value)*'+@multiplier+'/'+@basis_txt+' as Value, '
  + quotename('[',char(39))
  + '+ convert(varchar,'
  + ' convert(bigint, datediff_big(MILLISECOND, '+ quotename('01-01-1970 00:00:00',char(39)) +', cast(cast(' + @dateCol + ' as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as datetime))) *1) +'
  + quotename(', ',char(39))
  + ' + isnull(convert(varchar,convert(decimal(38,8),(c.Value)*'+@multiplier+'/'+@basis_txt+')), @mnull) +'
  + quotename(']',char(39))
  + ' as Epodateval '
  + ', '+@curveidstr + ' as CurveId'
  + @fromClause
  + @joinClause
  + @whereClause
  + @firstlast
  + @grpby
  + ' order by 1 '+@sort+', 2 '+@sort


  





end





--textcurves different output for table display
--else if (@mytabletype = 3) --txt  @datatypeid
--TBC ikke p?begynt bare idea, hva med aggregering PCT etc...
else if (@datatypeid >10) --txt 256 64 etc 
begin --txt
--select (@mytabletype)
--2nd not in use pt
set @sql = 
' select top('+cast(@top as varchar(32)) + ')'
+ ' NULL as DynDate, 
cast(cast(VDate as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as datetime) as VDate,
Value as Value, '+
quotename('{"x":',char(39)) + 
'+ convert(varchar,' +
' convert(bigint, datediff_big(MILLISECOND, '+ quotename('01-01-1970 00:00:00',char(39)) +', cast(cast(VDate as datetimeoffset) AT TIME ZONE '+@targettimezoneq+' as  datetime))) *1) +'
--TIMEPROBL
+ quotename(', "t":"',char(39) )
+ ' + Value +' --m? ha fnutter rundt text.. erik for api

+ quotename('"}',char(39) )
+ ' as Epodateval '
+ ', '+@curveidstr + ' as CurveId'
+ ' from '+ @tablename
+ ' where CurveId =  ' +cast(@mycurveid as varchar(16))
+ ' and VDate >= cast ( ' + quotename(cast( @mystart as varchar (32)),char(39))+ 'as datetime)  '
+ ' and VDate < cast ( ' + quotename(cast( @myend as varchar (32)),char(39)) + 'as datetime)  '
+ ' order by VDate '+@sort

--slutt txt
end



	exec (@sql)



GO


