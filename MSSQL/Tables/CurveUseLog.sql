USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveUseLog]    Script Date: 24/01/2026 14:47:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurveUseLog]') AND type in (N'U'))
DROP TABLE [dbo].[CurveUseLog]
GO

/****** Object:  Table [dbo].[CurveUseLog]    Script Date: 24/01/2026 14:47:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CurveUseLog](
	[Logged] [datetime] NULL,
	[CurveId] [int] NULL,
	[LoadsetId] [int] NULL,
	[Passed] [bit] NULL,
	[ClientId] [nvarchar](64) NULL
) ON [PRIMARY]
GO


