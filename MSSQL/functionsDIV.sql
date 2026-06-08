


declare @myfnlagint int =2

declare @myfnlagstr int ='2'
--select  VDate,
--(Value - LAG(Value,  @myfnlagint) over (ORDER BY VDate)) / LAG(Value,  @myfnlagint) over (ORDER BY VDate) *100 Value
--(avg(lag(Value,@myfnlagint) (ORDER BY VDate) )  over (ORDER BY VDate))  Value
--from curvedate


select  VDate,
(Value +LAG(Value,  @myfnlagint) over (ORDER BY VDate))/2  as mave,
STDEV(Value) OVER (PARTITION BY CurveId ORDER BY VDate ROWS BETWEEN 1 PRECEDING AND CURRENT ROW) As std3

from curvedate
where curveid = 101019