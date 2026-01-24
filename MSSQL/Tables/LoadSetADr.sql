USE [Harmonize]
GO

/****** Object:  Table [dbo].[LoadsetADr]    Script Date: 24/01/2026 14:49:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoadsetADr]') AND type in (N'U'))
DROP TABLE [dbo].[LoadsetADr]
GO

/****** Object:  Table [dbo].[LoadsetADr]    Script Date: 24/01/2026 14:49:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadsetADr](
	[LoadSetId] [int] NOT NULL,
	[username] [nvarchar](128) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[Method] [nvarchar](50) NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO


