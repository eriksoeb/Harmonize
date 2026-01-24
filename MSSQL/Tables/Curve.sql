USE [Harmonize]
GO

ALTER TABLE [dbo].[Curve] DROP CONSTRAINT [DF__Curve__Publish__36B12243]
GO

/****** Object:  Table [dbo].[Curve]    Script Date: 24/01/2026 14:43:03 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Curve]') AND type in (N'U'))
DROP TABLE [dbo].[Curve]
GO

/****** Object:  Table [dbo].[Curve]    Script Date: 24/01/2026 14:43:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Curve](
	[CurveId] [int] IDENTITY(100000,1) NOT NULL,
	[CurveName] [nvarchar](64) NULL,
	[Descr] [nvarchar](256) NULL,
	[Doc] [nvarchar](256) NULL,
	[LoadSetId] [int] NULL,
	[Created] [datetime] NULL,
	[Updated] [datetime] NULL,
	[CurveTypeId] [int] NULL,
	[Unit_id] [int] NULL,
	[GeoPointId] [int] NULL,
	[Publish] [bit] NULL,
	[DatatypeId] [int] NULL,
 CONSTRAINT [pk_CurveIdent] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Curve] ADD  DEFAULT ((0)) FOR [Publish]
GO


