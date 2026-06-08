USE [Harmonize]
GO

/****** Object:  StoredProcedure [dbo].[StpGetSeries]    Script Date: 22/04/2026 09:56:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE OR ALTER                     procedure [dbo].[StpGetSeries]  
(	

@curvename nvarchar(64),
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfunc  nvarchar(128) = null, ------------------------------------ny
@myfnlagint int = 0 , -- function lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@top int,
@sort nvarchar(4),
@convert2freq char(3) = 'OFF',  -- ANN | MON | DAY | OFF
@json NVARCHAR(MAX)
)

WITH execute as owner
AS
Begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on


declare @mystart datetime
declare @myend datetime
	

	if (@myfunc is NULL or @myfunc='None') --default
	begin
	exec StpGetSeriesOuterTzls @curvename,@basis, @myinterval,@myfnlagint,@format, @agg, @top, @sort, @convert2freq, NULL
	end

	else --this only for functions
	begin
	EXEC @myfunc @curvename ,@basis, @myinterval,@myfnlagint, @format,@agg, @top, @sort, @convert2freq, NULL
	end


end



--select 'end of pure'
GO


