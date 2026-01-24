USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[UTILS_PutTime]    Script Date: 24/01/2026 17:35:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






--des 2025 erik

CREATE OR ALTER                        procedure [dbo].[UTILS_PutTime]
--@loadsetid int,
@passedlsname nvarchar(64), 
@sname nvarchar(128),
@sdesc nvarchar(256),
@unit_id int,
@datestr nvarchar(32) ,
@value decimal (38,8)--,-- float,--decimal (24,8),
--@error nvarchar(1024) --this param can be removed..
--remove for ffi @error nvarchar(1024) output ups cpi ppi



WITH execute as owner
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

-- hente curve type loadsets tableid 

declare @myerror nvarchar(1024)
DECLARE @CURVENAME varchar(64)
DECLARE @CURVEID INT
--declare @value decimal(18,8) = try_cast(@valuep as decimal(18,8))

--select 'debugger ',@sname, @value as value-- ,@valuep as valuep

--erik dynamisk input
declare @tablename nvarchar(128)
--declare @lsname nvarchar(64)
declare @Loadsetid int = (select Id from LoadSet where Name = @passedlsname)


if (@loadsetid is null) 
begin
--set @error = 'Loadset does noet exist: '+ @passedlsname
 THROW 50001, ' UPS! no loadset Found :' , 1;
return;
end


--select 'here ??'

select 
@tablename = isnull(DT.[Tablename],'CurveData'), --default table
@passedlsname = LS.Name
--@lsid = Ls.Loadsetid
from LoadSet LS
inner join Datatable DT on DT.tableid = LS.tableid where LS.Id = @LoadsetId 

--select @tablename


declare @mysql1 nvarchar(2048) 

--if ((@lsname = @passedlsname) and (@lsid= @loadsetid))
--ad acess write table
--if (dbo.IsAccessADw(@loadsetid ) >=1)  --  : 1 har skrive tilgang, 0 ikke tilgang returns BIT

--update LoadsetADw
--set username = ORIGINAL_LOGIN(),
--Method = 'inserted fr pr'
--where Loadsetid =@Loadsetid


--delete from Client_Applogg;
--row in loadsetadw must be active !
/*
INSERT INTO Client_Applogg
VALUES (
ORIGINAL_LOGIN(),  
'tstWrite',
getdate(), 
 @loadsetid, 
 @loadsetid,
 @loadsetid, 
 @loadsetid);
*/ 
 





--if (2 >1) --alltid ok
if (dbo.IsAccessADw(@loadsetid ) <> 0 )   --1,2,3 er access ok
begin --write access is ok


MERGE Curve AS [Target]
--USING (SELECT  CurveName= @sname) AS [Source] 
-- ON  [Target].CurveName = [Source].CurveName

USING (SELECT  CurveName= @sname, LoadSetID = @loadsetid) AS [Source] 
    ON  [Target].CurveName = [Source].CurveName and [Target].LoadSetID = [Source].LoadSetID 

WHEN MATCHED THEN
  UPDATE SET 
  [Target].Descr=@sdesc, 
  [Target].Doc='doc to tbc', 
  [Target].Unit_id  = @unit_id


WHEN NOT MATCHED THEN
    INSERT (CurveName, CurveTypeId,DataTypeId,Descr, Doc, Unit_id, LoadSetId, Created ) 
  values ( @sname, 1, 10, @sdesc,NULL, @unit_id, @loadsetid , getutcdate());

  SELECT @CURVEID = (SELECT max(CurveId) FROM Curve WHERE CurveName = @sname and loadsetid = @loadsetid)



set @mysql1 = 'MERGE  '+@tablename + ' as [TARGET] ' +

 'USING ( select curveid = '+ cast(@curveid as varchar(32)) + ', Vdate ='+ quotename(@datestr,char(39)) +' ) as [Source] ' + 
   ' ON  [Target].curveid = [Source].curveid ' +
    ' AND [Target].VDate = [Source].VDate '+
	' WHEN MATCHED THEN '+
 ' UPDATE SET [Target].Updated=getutcdate() '+ ',Value = '+ quotename(@value,char(39)) +
 --om man vil ha inn null' UPDATE SET [Target].Updated=getutcdate() '+ ',Value = '+ isnull(cast(@value as nvarchar(30)), 'NULL')+ 
 ' WHEN NOT MATCHED THEN '+
  ' INSERT   (CurveId, VDate,Value,Updated) '+
  ' VALUES ('+ cast(@curveid as varchar(32)) + ', '+ 
 quotename( @datestr, char(39))  +' ,'+ quotename(@value, char(39)) + ',getutcdate() );'
 --funker m nullquotename( @datestr, char(39))  +' ,'+  isnull(cast(@value as varchar(30)), 'NULL')+ ',getutcdate() );'

  
 --select  ( @mysql1)
 
exec ( @mysql1)


 
end --iif access --slutt dersom ok


else --access is not OK
begin
--select 'Did not pass write access test: ' + original_login()
--set @error = isnull(@error,'')+ 'UPS no access '+ original_login()
--RETURN(@error);

--DECLARE @errorCode int = 9;
--DECLARE @error nvarchar(1024);

--SET @error = 'no access '+ ORIGINAL_LOGIN();
--RETURN(@errorCode);
--return;


  --THROW 50001, 'UPS! No Access to loadset', 1;

  DECLARE @msg nvarchar(400)
   SET @msg = 'Access denied for user:  '  + 'Original login: ' + ORIGINAL_LOGIN();
    --SET @msg = 'Access denied! '  + 'Original login: ' + ORIGINAL_LOGIN() + ', Session login: ' + SUSER_SNAME();
    --SET @msg = 'Access denied! '  + 'Original login: ' + ORIGINAL_LOGIN() + ', Session login: ' + SUSER_SNAME() +  ', System: ' + SYSTEM_USER;
    THROW 50001, @msg, 1





end

end
GO


