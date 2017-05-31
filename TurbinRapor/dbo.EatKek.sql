-- =============================================
-- Author:		Devrim Ersanli
-- Create date: 3/6/2017
-- Description:	Test SP
-- =============================================
ALTER PROCEDURE [dbo].[EatKek] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here




		-- Declare the return variable here
	DECLARE @ResultVar as int;

	DECLARE @ID as bigint
	DECLARE @RPMLOW as float
	DECLARE @VLL1 as float, @VLL2 as float, @VLL3 as float
	DECLARE @I1 as float, @I2 as float, @I3 as float
	DECLARE @CALCULATEDPOWER as float

	SET @ResultVar = -1;



      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE WTPLCTAG CURSOR READ_ONLY
      FOR
      SELECT WTPLCTAGID,
			LOWSPEEDRPM,
			PACALTERNATORVOLTAGEL1, PACALTERNATORVOLTAGEL2, PACALTERNATORVOLTAGEL3,
			PACALTERNATORCURRENTL1, PACALTERNATORCURRENTL2, PACALTERNATORCURRENTL3
      FROM WTPLCTAGS
 
      --OPEN CURSOR.
      OPEN WTPLCTAG
 
      --FETCH THE RECORD INTO THE VARIABLES.
      FETCH NEXT FROM WTPLCTAG INTO
      @ID, @RPMLOW, @VLL1, @VLL2, @VLL3, @I1, @I2, @I3


	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		IF (@VLL1 > 0.0 AND @RPMLOW < 30) 
			PRINT RTRIM(CAST(@VLL1 AS VARCHAR(10)))-- + N' ' + CAST(@I1 AS VARCHAR(10));
			SET @CALCULATEDPOWER = dbo.CalculatePower(@VLL1, @VLL2, @VLL3, @I1, @I2, @I3);
			UPDATE WTPLCTAGS SET GENERATEDPOWER = @CALCULATEDPOWER WHERE WTPLCTAGID = @ID
--		ELSE
--			UPDATE WTPLCTAGS SET GENERATEDPOWER = 0 WHERE WTPLCTAGID = @ID

		FETCH NEXT FROM WTPLCTAG INTO @ID, @RPMLOW, @VLL1, @VLL2, @VLL3, @I1, @I2, @I3 
	END 

	  CLOSE WTPLCTAG
	  DEALLOCATE WTPLCTAG
	-- Return the result of the function
	RETURN @ResultVar;

END