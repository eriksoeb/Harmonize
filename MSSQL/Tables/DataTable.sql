USE [Harmonize]
GO

/****** Object:  Table [dbo].[DataTable]    Script Date: 24/01/2026 14:47:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataTable]') AND type in (N'U'))
DROP TABLE [dbo].[DataTable]
GO

/****** Object:  Table [dbo].[DataTable]    Script Date: 24/01/2026 14:47:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DataTable](
	[TableId] [int] NOT NULL,
	[TableName] [nvarchar](64) NULL,
	[TableDesc] [nvarchar](256) NULL,
	[Created] [datetime] NULL,
	[Active] [bit] NULL,
	[CurveTypeId] [int] NOT NULL,
 CONSTRAINT [pk_DataTableTableId] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


