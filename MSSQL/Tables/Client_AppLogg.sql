USE [Harmonize]
GO

/****** Object:  Table [dbo].[Client_AppLogg]    Script Date: 24/01/2026 14:41:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Client_AppLogg]') AND type in (N'U'))
DROP TABLE [dbo].[Client_AppLogg]
GO

/****** Object:  Table [dbo].[Client_AppLogg]    Script Date: 24/01/2026 14:41:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Client_AppLogg](
	[ClientId] [nvarchar](64) NULL,
	[User] [nvarchar](64) NULL,
	[AppName] [nvarchar](32) NULL,
	[Started] [datetime] NULL,
	[Ver] [nvarchar](16) NULL,
	[Slogan] [nvarchar](128) NULL,
	[StatusMsg] [nvarchar](32) NULL,
	[StatusFlag] [int] NULL
) ON [PRIMARY]
GO


