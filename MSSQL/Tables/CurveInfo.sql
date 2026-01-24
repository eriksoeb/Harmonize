USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveInfo]    Script Date: 24/01/2026 14:45:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurveInfo]') AND type in (N'U'))
DROP TABLE [dbo].[CurveInfo]
GO

/****** Object:  Table [dbo].[CurveInfo]    Script Date: 24/01/2026 14:45:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CurveInfo](
	[CurveId] [int] NOT NULL,
	[MinDate] [datetime] NULL,
	[MaxDate] [datetime] NULL,
	[NumOfObs] [int] NULL,
	[Updated] [datetime] NULL,
	[Calledby] [nvarchar](64) NULL,
	[Executedby] [nvarchar](64) NULL,
	[Executed] [datetime] NULL,
	[Loadsetid] [int] NULL,
	[Hit] [int] NULL,
	[lastdiff] [decimal](38, 8) NULL,
 CONSTRAINT [pk_CurveInfoId] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


