USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveData]    Script Date: 24/01/2026 14:43:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurveData]') AND type in (N'U'))
DROP TABLE [dbo].[CurveData]
GO

/****** Object:  Table [dbo].[CurveData]    Script Date: 24/01/2026 14:43:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CurveData](
	[CurveId] [int] NOT NULL,
	[VDate] [datetime] NOT NULL,
	[Value] [decimal](18, 8) NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [pk_CurveIdDate] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC,
	[VDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


