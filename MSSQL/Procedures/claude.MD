StpGetSeries calls [StpGetSeriesOuterTzls] which then again calls [dbo].[Stp_GetSeriesFromInnerTzls] 



calling StpGetSeries
declare @json NVARCHAR(MAX)
execute StpGetSeries 'CPI:CTOTAL.IDX',NULL,'AllData',NULL,0,'client',NULL,100,'desc',@json
returns
2021-01-01 00:00:00.000	NULL	[1609459200000,'null']	101822	2021-01-01 00:00:00.000
2022-01-01 00:00:00.000	89.15833333	[1640995200000,89.15833333]	101822	2022-01-01 00:00:00.000
2023-01-01 00:00:00.000	94.05833333	[1672531200000,94.05833333]	101822	2023-01-01 00:00:00.000
2024-01-01 00:00:00.000	NULL	[1704067200000,'null']	101822	2024-01-01 00:00:00.000
2025-01-01 00:00:00.000	NULL	[1735689600000,'null']	101822	2025-01-01 00:00:00.000
which is correct as the Epodateval has [epoch,null] 




calling the inner directly by:
declare @json NVARCHAR(MAX)
exec Stp_GetSeriesFromInnerTzls 'CPI', 101822, NULL,'2020-01-01', '2026-01-01',NULL,100,'desc'
returns: 
BynDate VDate Value Epodateval CurveId
2021-01-01	2021-01-01 00:00:00.000	NULL	[1609459200000, null]	101822
2022-01-01	2022-01-01 00:00:00.000	89.1583333300	[1640995200000, 89.15833333]	101822
2023-01-01	2023-01-01 00:00:00.000	94.0583333300	[1672531200000, 94.05833333]	101822
2024-01-01	2024-01-01 00:00:00.000	NULL	[1704067200000, null]	101822
2025-01-01	2025-01-01 00:00:00.000	NULL	[1735689600000, null]	101822
which looks good , as the EpodateVal has [epooc,null]




The 3 procedures:



CREATE OR ALTER    procedure [dbo].[StpGetSeries]  --forsøker på dyn intervall fra view, samt utv 
(	
@curvename nvarchar(64),
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfunc  nvarchar(128) = null,
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@top int,
@sort nvarchar(4),
@json NVARCHAR(MAX)
)




CREATE OR ALTER   procedure [dbo].[StpGetSeriesOuterTzls]  --expect problem to be here
(	
@curvename nvarchar(64),
@basis int,  --baseyeR
@myinterval  nvarchar(128) = null,
@myfnlagint int = 0 , -- funktion lag bruker bare i funksjoner 1,2, 12 pct(12)
@format  nvarchar(8) = null,
@agg  nvarchar(16) ,
@top int,
@sort nvarchar(4),
@json NVARCHAR(MAX)
)




CREATE OR ALTER          Procedure [dbo].[Stp_GetSeriesFromInnerTzls]  -- dyn intervall fra view, samt utv 
(
@SetName  nvarchar(32) = null,
@mycurveid  int = null,
@basis int, --basis year periode ikke lenger date ikke nais, men works.
@mystart datetime,
@myend datetime,
@aggp nvarchar(16) ,
@top int =1000,  --if noting spec
@sort nvarchar(4)
)