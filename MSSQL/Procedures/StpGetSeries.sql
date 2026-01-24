USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[StpGetSeries]    Script Date: 24/01/2026 17:34:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE OR ALTER                     procedure [dbo].[StpGetSeries]  --forsøker på dyn intervall fra view, samt utv 
(	

@curvename nvarchar(64),
--@fdate  datetime =  NULL,
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfunc  nvarchar(128) = null, ------------------------------------ny
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@top int,
@sort nvarchar(4),
@json NVARCHAR(MAX)
)

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on


declare @mystart datetime
declare @myend datetime
	

	if (@myfunc is NULL or @myfunc='None')
	begin
	--select 'here'

	--exec StpGetCurveDataOuter2 @curvename,@basis, @myinterval,@myfnlagint,@format, @agg, @top, @sort, NULL
	
	--exec StpGetSeriesOuter @curvename,@basis, @myinterval,@myfnlagint,@format, @agg, @top, @sort, NULL

	--exec StpGetSeriesOuterTz

	exec StpGetSeriesOuterTzls @curvename,@basis, @myinterval,@myfnlagint,@format, @agg, @top, @sort, NULL

	
	
	end

	else
	begin 
	--select @myfunc
	--EXEC  @myfunc @curvename ,@basis, 1, @agg,@top,@sort
	EXEC @myfunc @curvename ,@basis, @myinterval,@myfnlagint, @format,@agg, @top, @sort, NULL
	end


end





--select 'end of pure'
GO


