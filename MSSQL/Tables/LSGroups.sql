USE [Harmonize]
GO

/****** Object:  Table [dbo].[LSGroups]    Script Date: 24/01/2026 14:51:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LSGroups]') AND type in (N'U'))
DROP TABLE [dbo].[LSGroups]
GO

/****** Object:  Table [dbo].[LSGroups]    Script Date: 24/01/2026 14:51:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LSGroups](
	[GrpId] [int] NOT NULL,
	[Name] [nvarchar](64) NULL,
 CONSTRAINT [pk_GrpId] PRIMARY KEY CLUSTERED 
(
	[GrpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


