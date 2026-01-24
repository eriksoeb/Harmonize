USE [Harmonize]
GO

/****** Object:  Table [dbo].[Functions]    Script Date: 24/01/2026 14:48:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Functions]') AND type in (N'U'))
DROP TABLE [dbo].[Functions]
GO

/****** Object:  Table [dbo].[Functions]    Script Date: 24/01/2026 14:48:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Functions](
	[ID] [int] NOT NULL,
	[FName] [nvarchar](32) NOT NULL,
	[FDesc] [nvarchar](255) NULL,
	[Created] [datetime] NULL,
	[Updated] [datetime] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [pk_myFunk] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


