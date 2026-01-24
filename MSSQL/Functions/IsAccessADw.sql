USE [Harmonize]
GO

/****** Object:  UserDefinedFunction [dbo].[IsAccessADw]    Script Date: 24/01/2026 17:41:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER           FUNCTION [dbo].[IsAccessADw] (@Loadsetid int)
RETURNS bit
WITH EXECUTE AS OWNER
AS
BEGIN
    -- Returns 1 if any of the valid login checks match access OK
    -- Returns 0 otherwise access denied

return (1) --always access to get a simpler start.. take this line out **OBS OBS **
--To test access this part need to be uncommented out

/*

    RETURN (
        SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
        FROM Loadset LS
        LEFT JOIN LoadSetADw LSAD ON LSAD.LoadSetId = LS.Id
        WHERE LSAD.Loadsetid = @Loadsetid
            AND LSAD.active = 1
            AND UPPER(LTRIM(RTRIM(LSAD.Username))) = UPPER(LTRIM(RTRIM(ORIGINAL_LOGIN())))
  
    )
*/


END;
GO


