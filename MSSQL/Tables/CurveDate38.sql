USE [Harmonize]
GO

/****** Object:  Table [dbo].[CurveDate38]    Script Date: 24/01/2026 14:44:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurveDate38]') AND type in (N'U'))
DROP TABLE [dbo].[CurveDate38]
GO

/****** Object:  Table [dbo].[CurveDate38]    Script Date: 24/01/2026 14:44:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CurveDate38](
	[CurveId] [int] NOT NULL,
	[VDate] [date] NOT NULL,
	[Value] [decimal](38, 8) NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [pk_CurveDate38ID] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC,
	[VDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


