USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[UTILS_Create_Loadset]    Script Date: 24/01/2026 17:34:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO










CREATE OR ALTER     procedure [dbo].[UTILS_Create_Loadset]
@passedlsname varchar(64)
--@curvetypevchar varchar(16),
--@freq varchar(3)

--EXEC dbo.UTILS_Create_Loadset 'CPI_NO';

--EXEC dbo.UTILS_Create_Loadset 'testset';
--Erik oct 2025

WITH execute as owner
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
declare @curvetype int  = 1
declare @tableid int = 1
declare @intervalid int = 1 

/*
if upper(@curvetypevchar) = 'Timeseries'
begin
set @curvetype = 1 
set @tableid  = 1
set @intervalid = 8 --default years last5
end
else if upper(@curvetypevchar) = 'Forecast'
begin
set @curvetype = 2
set @tableid  = 2
set @intervalid = 4 --year current maa ha et som gaar litt frem i tid tbc*
end
else 
select 'Ups curvetype invalid must be Timeseries or Forecast'


--timeseries
if @Freq in ('H','H1','H2','H3') and @curvetype = 1
begin
set @tableid = 3
set @intervalid = 3 --month current
end
else if @Freq in ('m5','m10','m15') and @curvetype = 1
begin
set @intervalid = 15 --week current
set @tableid = 5
end


--forecast
if @Freq in ('H') and @curvetype = 2
set @tableid = 2
else if  @Freq in ('D') and @curvetype = 2
set @tableid = 4

*/

declare @existlsname varchar(64) 

select @existlsname = Name
From LoadSet 
where name = @passedlsname 

--select @existlsname 
--tbc in kan leste fra tabllene i databasen

if  @existlsname IS NULL --and @curvetype in (select curvetypeid from curvetype where Active=1) and @tableid in (select tableid from DataTable where active=1) --doeas not exist
begin
select 'Creating new Loadset: '+@passedlsname + ' curvetype: '+cast(@curvetype as char(1)) + ' as ' + cast( @tableid as char(2))

insert into Loadset ( id, Name, freq, Updated, Tableid, Active,AccessAll,IntervalID,C2Timezone,UpdatedBy)
select max(id)+1, @passedlsname, 'TBC',  getutcdate(),@tableid, 1, 1 ,@intervalid, NULL, ORIGINAL_LOGIN()
from Loadset


--the cretor also gets the write permission:

insert into LoadsetADW ( LoadSetId, username,  UpdatedDateTime, Active)
select max(id), ORIGINAL_LOGIN(),  getutcdate(), 1 
from Loadset


--erik dytter inn default uncategorizes ut jul 22
--insert into LSHier ( Parent, Child, LoadsetId)
--select 3,31,max(loadsetid) from Loadsets

select 'Created : '  + @passedlsname +': '+  cast (max(id)  as varchar(4))  from loadset where [name] = @passedlsname     


end
else if @existlsname = @passedlsname
begin
select 'Error '+@existlsname +', '+ @passedlsname + ' already exist, see Loadset table'
end
else
begin
select 'Ups Check input parameters'
end




end


GO


