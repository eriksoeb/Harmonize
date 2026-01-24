USE [Harmonize]
GO

/****** Object:  Table [dbo].[Client_TableMgrLog]    Script Date: 24/01/2026 14:42:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Client_TableMgrLog]') AND type in (N'U'))
DROP TABLE [dbo].[Client_TableMgrLog]
GO

/****** Object:  Table [dbo].[Client_TableMgrLog]    Script Date: 24/01/2026 14:42:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Client_TableMgrLog](
	[TableName] [varchar](64) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Updated_by] [varchar](64) NULL,
	[Col] [varchar](32) NULL,
	[Val] [varchar](32) NULL,
	[SqlType] [varchar](16) NULL,
	[WClause] [varchar](256) NULL,
 CONSTRAINT [pk_TableMgrIDLog] PRIMARY KEY CLUSTERED 
(
	[TableName] ASC,
	[Updated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


