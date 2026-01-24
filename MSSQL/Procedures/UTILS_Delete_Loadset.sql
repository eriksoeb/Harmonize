USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[UTILS_Delete_Loadset]    Script Date: 24/01/2026 17:35:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





--exec UTILS_Delete_Loadset 'DELETE_DATA', 4, 'CPI'
--exec UTILS_Delete_Loadset 'DELETE_CURVES', 4, 'CPI'
--exec UTILS_Delete_Loadset 'DELETE_ALL', 4, 'CPI'
--erik dec 2025 , bot id & name not needed but extra security..

CREATE OR ALTER         procedure [dbo].[UTILS_Delete_Loadset]
@deletetype nvarchar(64),
@lsid_param int,
@lsname_param nvarchar(64)

WITH execute as owner
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
declare @lsid int 
declare @lsname nvarchar(64)


select 
@lsname = LS.Name,
@lsid = LS.id
from Loadset LS
where LS.id = @lsid_param and LS.Name = @lsname_param




if (dbo.IsAccessADw(@lsid ) = 0 )   --1,2,3 er access ok
begin
  DECLARE @msg nvarchar(400)
   SET @msg = 'Access denied for user:  '  + 'Original login: ' + ORIGINAL_LOGIN();
    THROW 50001, @msg, 1

end




if ((@lsname = @lsname_param) and (@lsid= @lsid_param) and @deletetype in ('DELETE_ALL', 'DELETE_DATA', 'DELETE_SERIES'))
begin
--select 'Good to Go! check completd - performing '+@deletetype

--delete from Loader_fileinfo where LoaderId = @lsid
delete from CurveInfo where Loadsetid = @lsid  --juni 2025 erik

--select 'Deleted loader/loadset from loaderfileinfor better reruns'


	if @deletetype = 'DELETE_DATA'
		begin
		--select 'Deleting data only'
		exec Stp_DeleteLoadsetDataOnly @lsid
		--rebuilding stats as curves has 0 observations:
		exec UTILS_UpdateCurveinfo @lsname

		end

	else if @deletetype = 'DELETE_SERIES' --delete curves
		begin
		--select 'Deleting curves og curvedata'
		exec Stp_DeleteLoadsetCurvesandData @lsid
		
		end

	else  if @deletetype = 'DELETE_ALL'
		begin
		--select 'Deleting curvedata,  curves and the loadset'
		exec Stp_DeleteLoadsetCurvesandData @lsid
		delete LoadSet where id = @lsid  --deletes the row in loadst as well

		end


end
else
begin

 DECLARE @msg2 nvarchar(400)
   SET @msg2 = 'sample: exec Util_Delete_Loadset '+quotename('DELETE_ALL',char(39)) + ', 99, '+ Quotename('MyLoadsName', char(39));
    THROW 50001, @msg, 1

	--select 'Ups mismatch. Loadsetid:'+ cast (@lsid_param as varchar(16)) + ' and '+@lsname_param+ ' must match&exist and deletetype one of DELETE_ALL, DELETE_DATA, DELETE_CURVES, DELETE_ALL,'
	--select 'exec Util_Delete_Loadset '+quotename('DELETE_ALL',char(39)) + ', 99, '+ Quotename('MyLoadsName', char(39))


end


end
GO


