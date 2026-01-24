USE [Harmonize]
GO

/****** Object:  Table [dbo].[DataType]    Script Date: 24/01/2026 14:48:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataType]') AND type in (N'U'))
DROP TABLE [dbo].[DataType]
GO

/****** Object:  Table [dbo].[DataType]    Script Date: 24/01/2026 14:48:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DataType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](16) NULL,
	[Descr] [nvarchar](256) NULL,
	[Created] [datetime] NULL,
 CONSTRAINT [pk_DataTypeId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


