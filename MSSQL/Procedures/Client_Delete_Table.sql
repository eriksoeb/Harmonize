USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Delete_Table]    Script Date: 24/01/2026 17:17:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER   procedure [dbo].[Client_Delete_Table]
(
@tablename nvarchar(32),
@whereStr nvarchar(256)
)


--erik oct 25 delete from!! table
WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on




--if @col IN ( 'Code',  'Name', 'LongName', 'DimId', 'MetaId')
--begin
--set @value = QUOTENAME ( @val, char(39))
--end


declare @sql nvarchar (1024)


set @sql = 'Delete '+ @tablename+ '   ' + @whereStr
--logging tbc


--set @sql = 'Delete LoadsetsTST  where   loadsetId=  '+ @loadsetid 
--select @sql

EXEC (@sql)

insert into Client_TableMgrLog
--	[TableName] ,	[Updated] ,	Updated_by  ,	[Col] ,	[Val] ,	[SqlType],	[WClause]
select	@tablename, getutcdate(), ORIGINAL_LOGIN(),'*','*', 'DELETE', @whereStr


--hent ut samme raden insert den i history TBC


 end
GO


