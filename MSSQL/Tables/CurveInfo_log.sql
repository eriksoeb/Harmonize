USE [Harmonize]
GO

/****** Object:  Table [dbo].[Curveinfo_log]    Script Date: 24/01/2026 14:46:21 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Curveinfo_log]') AND type in (N'U'))
DROP TABLE [dbo].[Curveinfo_log]
GO

/****** Object:  Table [dbo].[Curveinfo_log]    Script Date: 24/01/2026 14:46:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Curveinfo_log](
	[LoadsetId] [int] NOT NULL,
	[Uname] [nvarchar](64) NULL,
	[Oname] [nvarchar](64) NULL,
	[Called] [datetime] NULL
) ON [PRIMARY]
GO


