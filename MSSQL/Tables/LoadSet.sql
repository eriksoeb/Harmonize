USE [Harmonize]
GO

/****** Object:  Table [dbo].[LoadSet]    Script Date: 24/01/2026 14:49:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoadSet]') AND type in (N'U'))
DROP TABLE [dbo].[LoadSet]
GO

/****** Object:  Table [dbo].[LoadSet]    Script Date: 24/01/2026 14:49:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadSet](
	[Id] [int] NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Url] [nvarchar](256) NULL,
	[Freq] [nvarchar](16) NULL,
	[Source] [nvarchar](256) NULL,
	[TableId] [int] NULL,
	[Active] [bit] NULL,
	[AccessAll] [bit] NULL,
	[PlattformOwner] [nvarchar](64) NULL,
	[BusinessOwner] [nvarchar](64) NULL,
	[IntervalId] [int] NULL,
	[HierChildId] [int] NULL,
	[parentGroupId] [int] NULL,
	[C2Timezone] [nvarchar](64) NULL,
	[Updated] [datetime] NULL,
	[UpdatedBy] [nvarchar](64) NULL,
 CONSTRAINT [PK_LoadSetName] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_LoadSetId] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


