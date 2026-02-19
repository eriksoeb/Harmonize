USE [Harmonize]
GO

/****** Object:  Table [dbo].[Client_Color]    Script Date: 19/02/2026 14:43:50 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Client_Color]') AND type in (N'U'))
DROP TABLE [dbo].[Client_Color]
GO

/****** Object:  Table [dbo].[Client_Color]    Script Date: 19/02/2026 14:43:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Client_Color](
	[ColorId] [int] NOT NULL,
	[ColorName] [nvarchar](64) NULL,
	[ColorCode] [nvarchar](32) NULL,
	[UpdatedBy] [nvarchar](64) NULL,
	[Updated] [datetime] NULL
) ON [PRIMARY]
GO


