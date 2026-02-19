USE [Harmonize]
GO

/****** Object:  Table [dbo].[LSGroups]    Script Date: 19/02/2026 14:48:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LSGroups]') AND type in (N'U'))
DROP TABLE [dbo].[LSGroups]
GO

/****** Object:  Table [dbo].[LSGroups]    Script Date: 19/02/2026 14:48:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LSGroups](
	[GrpId] [int] NOT NULL,
	[Name] [nvarchar](64) NULL,
	[UpdatedBy] [nvarchar](64) NULL,
	[Updated] [datetime] NULL,
 CONSTRAINT [pk_GrpId] PRIMARY KEY CLUSTERED 
(
	[GrpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


