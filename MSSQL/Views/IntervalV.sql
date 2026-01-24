USE [Harmonize]
GO

/****** Object:  View [dbo].[IntervalV]    Script Date: 24/01/2026 14:32:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








CREATE OR ALTER               View  [dbo].[IntervalV] as (

--Erik aUG 2026

--select 1 as ID, 'All Data' as IName, DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-150,0)) as 'Start', DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +5,-1)) AS 'End'
--when using this view for qry, view is converted to UTC
--getdate() returns CET , CET for readability only
--fra dato >=vdate  AND dato < vdate

select -1 as ID, 'Default' as IName,NULL as Start, NULL as 'End' -- henter da ut interval på loadsettet i inner..
union

select 0 as ID, 'NoData' as IName,'2040.01.01' as Start, '2039.01.01' as 'End'
union

select 1 as ID, 'AllData' as IName, '1753.01.01' as 'Start', DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +2,0)) AS 'End'

/*
union all
select 2 as ID, 'MonthCurrent-1' as IName, 
DATEADD(month,month(getdate())-2,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) , 
DATEADD(month,month(getdate())-1,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0)))

union all

select 40 as ID, 'MonthCurrent-2' as IName, 
DATEADD(month,month(getdate())-3,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) , 
DATEADD(month,month(getdate())-2,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0)))

union all
select 41 as ID, 'MonthCurrent-3' as IName, 
DATEADD(month,month(getdate())-4,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) , 
DATEADD(month,month(getdate())-3,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0)))

union all

select  3 as ID,'MonthCurrent' as IName,
DATEADD(month,month(getdate())-1,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS Start ,
DATEADD(month,month(getdate())-0,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS 'End'


union all

select 4 as ID, 'YearCurrent' as IName, 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-1,0)) AS Start, 
DATEADD(yy, DATEDIFF(yy,2,GETDATE())+1,0 ) AS 'End' 
*/





--union all

--select  4 as ID,'YearsLast1 ', 
--DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-1,0)) AS Start,
--DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'



union all



select  5 as ID,'YearsLast2 ', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-2,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'


union all


select  6 as ID,'YearsLast3', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-3,0)) AS Start, 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'
union all


select  7 as ID,'YearsLast4 ', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-4,0)) AS Start, 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'
union all

select  8 as ID,'YearsLast5 ', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-5,0)) AS Start, 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'

union all


select  10 as ID,'YearsLast10 ', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-10,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'

/*
union all




select  9 as ID, 'MonthsLast13', 
DATEADD(month,month(getdate())-13,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS Start ,
DATEADD(month,month(getdate()),DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS 'End'


union all

select  12 as ID, 'Today-1', 
cast(getdate()-1 as date) AS Start ,
cast(getdate()  as date) AS 'End'


union all

select  10 as ID, 'TodayCurrent', 
cast(getdate() as date) AS Start 
,CAST(convert(varchar(20),getdate()+1,112) as datetime)  AS 'End'


union all
select  13 as ID, 'Tomorrow', 
cast(getdate()+1 as date) AS Start ,
CAST(getdate()+2 as date)  AS 'End'


union all
select 14 as ID, 'TodayPlus3', 
dateadd(day, datediff(day, 0, getdate()), 0) as Start, 
dateadd(day, datediff(day, 0, getdate()), 3) 



union all
select 15 as ID, 'WeekCurrent' ,
cast(cast (DATEADD(DAY,  1-DATEPART(WEEKDAY, GETDATE()-7*0), GETDATE()-7*0) as date)as datetime)  as start,
--dateadd(SECOND,-1,	cast(cast (DATEADD(DAY,  7-DATEPART(WEEKDAY, GETDATE()-7*0), GETDATE()-7*0) as date)as datetime)) as enddate 
cast(cast (DATEADD(DAY,  8-DATEPART(WEEKDAY, GETDATE()-8*0), GETDATE()-8*0) as date) as datetime)   as enddate 

--endrer til 8 fra 7-og deretter til0
union all
select 16 as ID, 'WeekCurrent-1' ,
cast(cast (DATEADD(DAY,  1-DATEPART(WEEKDAY, GETDATE()-7*1), GETDATE()-7*1) as date)as datetime)  as start,
-- feil pa mandagercast(cast (DATEADD(DAY,  8-DATEPART(WEEKDAY, GETDATE()-8*1), GETDATE()-8*1) as date)as datetime) as enddate 
cast(cast (DATEADD(DAY,  1-DATEPART(WEEKDAY, GETDATE()-7*0), GETDATE()-7*0) as date)as datetime)  as endi


--union all anngir i UTC
union all
select 17 as ID, 'HourPrevious1', --dateadd(hour,-1,getutcdate() )  , getutcdate()  as enddate 
DATEADD(hh,DATEPART(hh,GETDATE())-1,DATEADD(dd,0, DATEDIFF(dd,0,GETDATE()))) as Start,
DATEADD(hh,DATEPART(hh,GETDATE()),DATEADD(dd,0, DATEDIFF(dd,0,GETDATE()))) as 'end'






select  19 as ID, 'MonthsLast3', 
DATEADD(month,month(getdate())-3,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS Start ,
DATEADD(month,month(getdate()),DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS 'End'

union all

select  20 as ID, 'MonthsLast2', DATEADD(month,month(getdate())-2,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS Start,
DATEADD(month,month(getdate()),DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))) AS 'End'


union all
select 21 as ID, 'MonthPrev2Next' as IName, DATEADD(month,month(getdate())-2,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0))),
DATEADD(month,month(getdate())+1,DATEADD(dd,0,DATEADD(yy, DATEDIFF(yy,0,GETDATE()),0)))

*/




--union all

--select  21 as ID,'YearCurrent', 
--DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-1,0)) AS Start,
--DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE()) +0,0)) AS 'End'


union all

select  22 as ID,'YearCurrent-1', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-2,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-1,0)) AS 'End'

union all
select  23 as ID,'YearCurrent-2', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-3,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-2,0)) AS 'End'

union all
select  24 as ID,'YearCurrent-3', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-4,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-3,0)) AS 'End'

union all
select  25 as ID,'YearCurrent-4', 
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-5,0)) AS Start,
DATEADD(month,12,DATEADD(yy, DATEDIFF(yy,1,GETDATE())-4,0)) AS 'End'



)


GO


