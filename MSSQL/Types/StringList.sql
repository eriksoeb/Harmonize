USE [Harmonize]
GO

/****** Object:  UserDefinedTableType [dbo].[StringList]    Script Date: 24/01/2026 17:38:49 ******/
CREATE TYPE [dbo].[StringList] AS TABLE(
	[Item] [nvarchar](max) NULL
)
GO


