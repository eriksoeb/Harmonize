USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[Client_Version2]    Script Date: 24/01/2026 17:29:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER                       procedure [dbo].[Client_Version2]  
@appid int, @ver nvarchar(16), @winuser varchar(32)
--ups custome 2026

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
set nocount on

--declare @OnPrem int
declare @DbVer nvarchar(32)
declare @StatusFlag int = 0
declare @StatusMsg nvarchar(128) = 'OK'
declare @env nvarchar(8)
declare @envcolor nvarchar(32)

declare @DispName nvarchar(64), @valid_fom date , @warning_fom date  , @valid_tom date 

select top (1) @DispName = DispName, @valid_fom=valid_fom, @warning_fom=warning_fom, @valid_tom=valid_tom, 
--@OnPrem=OnPrem, 
@DbVer = [Version],
@env=env, @envcolor=EnvColor 

 from AppVersion
where 1 = 1
and Active = 1
--and AppId = @appid 
order by [Version] desc
--and [Version] = @ver

/*
if   getutcdate() between  @warning_fom and @valid_tom 
begin
set  @StatusFlag = 1
set @StatusMsg =  'Warning Upgrade before: ' + cast (@valid_tom as nvarchar(16))
end

else if getutcdate() > @valid_tom 
begin
set  @StatusFlag = -1
set @StatusMsg =  'Error reinstall Harmonize'
end

else if getutcdate() < @warning_fom
begin
set  @StatusFlag = 0
set @StatusMsg =  'OK'
end

else
begin
set  @StatusFlag = -9
set @StatusMsg =  'Unknown'
end

*/



declare @finalsloganlog nvarchar(128)  = 'Harmonized' --(select  isnull(@myslogan, @randoms) )


-- comm out for trusted 
INSERT INTO Client_Applogg
VALUES (
ORIGINAL_LOGIN(),  
@winuser,
isnull(@DispName ,'Harmonized'),
getdate(), 
 @ver, 
 @finalsloganlog,
 @StatusMsg, 
 @StatusFlag);
 
 --retrut til app
--select isnull(@DispName+' : '+@finalsloganlog,'Horizon') as DispName, @StatusFlag as StatusFlag,@StatusMsg as StatusMsg , @OnPrem as OnPrem, @env as env, @envcolor as envcolor

 --retrut til app
select @DispName as DispName, @StatusFlag as StatusFlag,@StatusMsg as StatusMsg , 
--@OnPrem as OnPrem, 
@DbVer as DbVer,
@env as env, @envcolor as envcolor



end
GO


