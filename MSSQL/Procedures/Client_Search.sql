USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Search]    Script Date: 24/01/2026 17:26:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER                        PROCEDURE [dbo].[Client_Search]
(
 @listOfIDstbl StringList readonly,
 @mywild nvarchar(64) ,
 @mystr nvarchar(64) 
 )

 --erik
 --using user defined type ass passing in a list od datasets to search
--CREATE TYPE [dbo].[StringList] AS TABLE([Item] [nvarchar](max) NULL)
--GO

WITH execute as owner
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;
--declare @datasets StringList =  (select  * from  @listOfIDstbl offset 1 rows)

--wilsdcardet
--declare @mystr  nvarchar(128) = (select top(1) * from  @listOfIDstbl   )

--declare @myparams StringList =  (select  * from  @listOfIDstbl order by 1 offset 2 rows)

--works alle databaser
;with myparams as (
select row_number() 
over ( order by (SELECT NULL) )  as myrow, 
Item
from @listOfIDstbl)


--declare @mywild  nvarchar(128) = (select Item from myparams where myrow = 1)





--select item from myparams
 --where myrow > 1



--Declare @listOfIDstbl StringList; -- readonly;
--DECLARE @SQL nvarchar(4000);



--INSERT INTO @listOfIDstbl VALUES ('KPI')
--INSERT INTO @listOfIDstbl VALUES ('FNavn')


--CREATE TYPE [dbo].[StringList] AS TABLE(
--    [Item] [NVARCHAR](MAX) NULL);


--where 1 = 1 
--and(( C.CurveName like @mystr) or (C.Descr like @mystr))

--declare @listOfIDs varchar(100)
/*
select C.CurveId, C.CurveName, C.Descr, LS.Name as LoadsetName, 
LS.Freq, C.Unit, LS.Url, Ls.Source1, LS.Source2, C.Updated, 
C.Points, LS.CurveType, LS.tableid, C.Created
from Curve C inner join LoadSets LS on LS.Loadsetid = C.LoadSetId 
where 1 = 1 
and(( C.CurveName like @mystr) or (C.Descr like @mystr))
and  ( LS.Name in (select Item from @listOfIDstbl)  or 'All' in  (select Item from @listOfIDstbl)  )


*/


-- erik 25 semikolon
--jan 26 source
select C.CurveId ,--as Id, 
upper(LS.Name)+':'+UPPER(C.CurveName) as CurveName, 
C.Descr, 

--select C.CurveId, (C.CurveName) as CurveName, C.Descr, --uten loadsettet no suksess

--LS.Name as LoadsetName --flesk paa smær men 
LS.Id as LoadSetId ,--tatt inn

LS.Freq, 
U.Code as Unit,
--C.Unit, --maa se litt paa litt ferre units
LS.Url, Ls.Source as Source , 
--LS.Source2,
CI.Updated,
CI.NumOfObs,
CI.LastDiff,
CI.MinDate,
CI.MaxDate,
--LS.CurveType,  --flytter til Curve??aug 10
--C.CurveTypeId, --not needed as longa as all  is timeseries numeric
IV.IName Interval
--CI.Hit men sorterer på denne jul23

from 
Curve C 
left join LoadSet LS on LS.Id = C.LoadSetId 
left join IntervalV IV on IV.ID = LS.IntervalId
--utgaarleft join MetaDim M on M.Metaid = C.Unit_id and M.DimId = 1 --unit kan ha flere ??
left join Unit U on U.Id = C.Unit_id 
left join CurveInfo CI on CI.CurveId = C.CurveId

where 1 = 1 

--ok mand and(( C.CurveName like @mystr) or (C.Descr like @mystr))

--and(( C.CurveName like @mywild) AND (C.Descr like @mystr))

AND C.CurveName LIKE COALESCE(NULLIF(@mywild, ''), '%')
AND C.Descr     LIKE COALESCE(NULLIF(@mystr,  ''), '%')

--do not show series id no access
and dbo.IsAccessADr(LS.Id) >= 1


--and  ( LS.Name in (select Item from myparams where myrow >1)  or 'All' in  (select Item from myparams where myrow >1)  )
and  ( LS.Name in (select Item from myparams where myrow >1)  or 'All' in  (select Item from myparams))  
order by CI.Hit desc,C.Curveid asc;


end



GO


