USE [Harmonize]
GO

/****** Object:  Table [dbo].[LoadsetADw]    Script Date: 19/02/2026 14:53:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoadsetADw]') AND type in (N'U'))
DROP TABLE [dbo].[LoadsetADw]
GO

/****** Object:  Table [dbo].[LoadsetADw]    Script Date: 19/02/2026 14:53:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadsetADw](
	[LoadSetId] [int] NOT NULL,
	[username] [nvarchar](128) NULL,
	[Method] [nvarchar](50) NULL,
	[Active] [bit] NULL,
	[UpdatedBy] [nvarchar](64) NULL,
	[Updated] [datetime] NULL
) ON [PRIMARY]
GO


