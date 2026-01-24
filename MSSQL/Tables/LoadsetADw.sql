USE [Harmonize]
GO

/****** Object:  Table [dbo].[LoadsetADw]    Script Date: 24/01/2026 14:50:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoadsetADw]') AND type in (N'U'))
DROP TABLE [dbo].[LoadsetADw]
GO

/****** Object:  Table [dbo].[LoadsetADw]    Script Date: 24/01/2026 14:50:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadsetADw](
	[LoadSetId] [int] NOT NULL,
	[username] [nvarchar](128) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[Method] [nvarchar](50) NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO


