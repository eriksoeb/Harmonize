USE [Harmonize]
GO

/****** Object:  Table [dbo].[CallingLog]    Script Date: 24/01/2026 14:40:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CallingLog]') AND type in (N'U'))
DROP TABLE [dbo].[CallingLog]
GO

/****** Object:  Table [dbo].[CallingLog]    Script Date: 24/01/2026 14:40:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CallingLog](
	[curvename] [nvarchar](64) NULL,
	[fdate] [datetime] NULL,
	[myinterval] [nvarchar](128) NULL,
	[myfnlagint] [int] NULL,
	[myforecast] [nvarchar](32) NULL,
	[tzconvert] [int] NULL,
	[format] [nvarchar](8) NULL,
	[agg] [nvarchar](16) NULL,
	[topp] [int] NULL,
	[sort] [nvarchar](4) NULL,
	[json] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


