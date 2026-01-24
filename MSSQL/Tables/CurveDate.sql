USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveDate]    Script Date: 24/01/2026 14:44:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurveDate]') AND type in (N'U'))
DROP TABLE [dbo].[CurveDate]
GO

/****** Object:  Table [dbo].[CurveDate]    Script Date: 24/01/2026 14:44:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CurveDate](
	[CurveId] [int] NOT NULL,
	[VDate] [date] NOT NULL,
	[Value] [decimal](18, 8) NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [pk_CurveDateID] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC,
	[VDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


