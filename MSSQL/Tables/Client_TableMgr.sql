USE [Harmonize]
GO

/****** Object:  Table [dbo].[Client_TableMgr]    Script Date: 24/01/2026 14:41:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Client_TableMgr]') AND type in (N'U'))
DROP TABLE [dbo].[Client_TableMgr]
GO

/****** Object:  Table [dbo].[Client_TableMgr]    Script Date: 24/01/2026 14:41:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Client_TableMgr](
	[Id] [int] NOT NULL,
	[Name] [varchar](64) NULL,
	[Pid1] [varchar](32) NULL,
	[Pid2] [varchar](32) NULL,
	[Pid3] [varchar](32) NULL,
	[RAccess] [varchar](32) NULL,
	[WAccess] [varchar](32) NULL,
	[Orderby] [varchar](32) NULL,
	[Topp] [int] NULL,
	[Object_type] [varchar](3) NULL,
 CONSTRAINT [pk_TableMgrID] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


