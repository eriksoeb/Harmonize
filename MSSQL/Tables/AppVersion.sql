USE [Harmonize]
GO

/****** Object:  Table [dbo].[AppVersion]    Script Date: 24/01/2026 14:39:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppVersion]') AND type in (N'U'))
DROP TABLE [dbo].[AppVersion]
GO

/****** Object:  Table [dbo].[AppVersion]    Script Date: 24/01/2026 14:39:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppVersion](
	[AppId] [int] NOT NULL,
	[Version] [nvarchar](16) NOT NULL,
	[DispName] [nvarchar](32) NULL,
	[Active] [bit] NULL,
	[Valid_fom] [date] NULL,
	[Warning_fom] [date] NULL,
	[Valid_tom] [date] NULL,
	[OnPrem] [bit] NULL,
	[Env] [nvarchar](4) NULL,
	[EnvColor] [nvarchar](32) NULL,
 CONSTRAINT [pk_AppVerdion] PRIMARY KEY CLUSTERED 
(
	[AppId] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


