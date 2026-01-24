USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Select_Table]    Script Date: 24/01/2026 17:27:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER  procedure [dbo].[Client_Select_Table]
--erik juni 2022
@tablename varchar (32),  --table name as arg kanskje sort og.. tbc
@top int,
@orderby varchar (32)
WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

declare @sql varchar(128)

declare @otype char(1) = (SELECT type FROM sys.objects  where object_id = (OBJECT_ID(@tablename))   )



if ( @otype in ( 'U', 'V' ) ) --tables views
begin
set @sql = 'select top '+cast(@top as varchar(8) ) + ' * from '+ @tablename+ '  order by  '+ @orderby
end
else if (  @otype = 'P' )  --// proc
begin
set @sql = 'sp_helptext '+@tablename
end





--select @sql
EXEC (@sql)

--insert into Client_Debug_tmp
--select getdate(), @sql

 end
GO


