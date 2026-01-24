USE [Harmonize]
GO

/****** Object:  Table [dbo].[Unit]    Script Date: 24/01/2026 14:51:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit]') AND type in (N'U'))
DROP TABLE [dbo].[Unit]
GO

/****** Object:  Table [dbo].[Unit]    Script Date: 24/01/2026 14:51:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Unit](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](16) NULL,
	[Name] [nvarchar](64) NULL,
	[Description] [nvarchar](256) NULL,
	[Updatedby] [nvarchar](64) NULL,
	[Updated] [datetime] NULL,
 CONSTRAINT [pk_Unit_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


