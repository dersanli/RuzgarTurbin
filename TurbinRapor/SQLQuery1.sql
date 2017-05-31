USE [WINDTURBINE]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[EatKek]

SELECT	@return_value as 'Return Value'

GO
