

select * from Curve where LoadSetId = 233

--ARD Endog Arrears and discrepancy
--BMOILREV Endog Eq17 Benchmark oil revenue


declare @name nvarchar(32)
declare @desc nvarchar(128)

declare @line nvarchar(128) = 'ARD Exog Exog Arrears and discrepancy'
--set name = 

--select charindex(' ',@line)

set @name = SUBSTRING(@line, 1, charindex(' ',@line)-1)
set @desc = SUBSTRING(@line, charindex(' ',@line), 64)

select '-'+@name+'-'
select @desc


update Curve
set Descr = @desc









