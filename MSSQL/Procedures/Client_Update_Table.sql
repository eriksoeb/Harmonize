USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Update_Table]    Script Date: 24/01/2026 17:27:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER          procedure [dbo].[Client_Update_Table]
(
@tablename nvarchar(32),
@col  nvarchar(32) ,
@val  nvarchar(32) ,
@where    nvarchar(256) = ' where 1 = 2'
)


--erik 
WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

declare @value nvarchar (32) = @val
declare @intvalue nvarchar (32) = @val


declare @value2 nvarchar (32) = '' --tom str om concate key

declare @sqltype nvarchar (16) = 'UPSERT'



declare @coltype nvarchar(32) = (select data_type
from INFORMATION_SCHEMA.COLUMNS     
where COLUMN_NAME = @col     
and TABLE_NAME =@tablename)




if @coltype in ('char', 'varchar', 'nvarchar', 'nchar')
begin
set @value = QUOTENAME ( @val, char(39))
--select 'fnutter txt felter her'
end

--select @col, @coltype,@value


/*
 declare @col2 varchar(32) = (select top(1)
 ','+ (column_name) 
from INFORMATION_SCHEMA.COLUMNS     
where 
1 = 1     
and is_nullable = 'NO'
and Column_name <> @col
and TABLE_NAME =@tablename)


if  @col2 IS NOT NULL  -- enda en nøkkel må ha verdi dummy
begin
set @value2 = ', -999'
end
else
begin
set @value2 = '' --ingen flere enn 1 nøkkel
end

*/



declare @sql nvarchar (1024)




if CHARINDEX('NULL', @where) >0    --NULL exist and need to in
begin
select 'insertter'

--set @sql = 'insert into '+@tablename + '  (' + @col + ' ) values( ' + @value + ')'
set @sqltype = 'INSERT'
--set @sql = 'insert into '+@tablename + '  (' + @col + isnull(@col2,'') + ' ) values( ' + @value + isnull(@value2,'') +  ')'
--select @sql
set @sql = 'insert into '+@tablename + '  (' + @col  + ' ) values( ' + @value + isnull(@value2,'') +  ')'




end


else
begin
if @col not in (  'Updatedby', 'Updated')  --//disse er read only i appen

set @sqltype = 'UPDATE'
set @sql = 'Update '+@tablename+ '  set  Updated = getutcdate(), Updatedby= ORIGINAL_LOGIN(),   ' + @col  + '  =  ' + @value   + '  '+ @where   
--set @sql =''
--set @sql = 'Update '+@tablename+ '  set   ' + @col  + '  =  ' + @value   + '  '+ @where   
select @sql

end

--insert into Client_Debug_tmp
--select getdate(), @col 

insert into Client_TableMgrLog
--	[TableName] ,	[Updated] ,	Updated_by  ,	[Col] ,	[Val] ,	[SqlType],	[WClause]
select	@tablename, getutcdate(), ORIGINAL_LOGIN(),@col,@val, @sqltype, @where



EXEC (@sql)
--select @sql



 end
GO


