IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_CALCULATIONS$GetTotalMass'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_CALCULATIONS$GetTotalMass]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_CALCULATIONS$GetTotalMass]
	@sASSESSMENT_CODE		  VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT,
	@dcTotalMass			  NUMERIC(25, 3) OUTPUT
AS
BEGIN
	SELECT @dcTotalMass = ISNULL(SUM(MASS), 0)
	FROM DXDIR_INPUT
	WHERE ASSESSMENT_CODE		 = @sASSESSMENT_CODE AND
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID;
END
GO


IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'dbo.DXDIR_PK_CALCULATIONS$GetValuePerMass')
			AND OBJECTPROPERTY(id, N'IsScalarFunction') = 1
		)
	DROP FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetValuePerMass]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Use for single row of output Product, CoProduct and Food Rescue
CREATE FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetValuePerMass]
(
	@sASSESSMENT_CODE			VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID	BIGINT,
	@dcOUTPUT_CATEGORY_ID		BIGINT
)
RETURNS NUMERIC(25, 12) 
AS
BEGIN
	DECLARE @resultValuePerMass NUMERIC(25, 12) = 0;

	-- TODO: this needs to be confirmed
	SELECT @resultValuePerMass = SUM(ISNULL(INCOME, 0)) / SUM(WEIGHT)
	FROM DXDIR_OUTPUT
	WHERE ASSESSMENT_CODE = @sASSESSMENT_CODE
	  AND ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID
	  AND OUTPUT_CATEGORY_ID = @dcOUTPUT_CATEGORY_ID
	  AND ISNULL(WEIGHT, 0) <> 0;

	RETURN @resultValuePerMass;
END
GO


IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_CALCULATIONS$GetMargin'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_CALCULATIONS$GetMargin]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_CALCULATIONS$GetMargin]
	@sASSESSMENT_CODE			VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID	BIGINT,
	@dcOUTPUT_CATEGORY_ID		BIGINT,
	@dcMargin					NUMERIC(25, 12) OUTPUT
AS
BEGIN
	DECLARE @productValuePerMass NUMERIC(25, 12),
			@valuePerMass NUMERIC(25, 12);

	IF (@dcOUTPUT_CATEGORY_ID = 1)
		SET @dcMargin = 1;
	ELSE IF (@dcOUTPUT_CATEGORY_ID IN (2, 3)) -- COPRODUCT, FOOD_RESCUE
	  BEGIN

        SET @productValuePerMass = dbo.DXDIR_PK_CALCULATIONS$GetValuePerMass(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, 1); 
        
		IF (@productValuePerMass = 0)
		  BEGIN
			SET @dcMargin = 0;
		  END
		ELSE
		  BEGIN
			SET @valuePerMass = dbo.DXDIR_PK_CALCULATIONS$GetValuePerMass(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @dcOUTPUT_CATEGORY_ID);
			SET @dcMargin = @valuePerMass / @productValuePerMass;
		  END
	  END
	ELSE
		SET @dcMargin = 0;
END
GO


IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'dbo.DXDIR_PK_CALCULATIONS$GetOutputRevenueFactor')
			AND OBJECTPROPERTY(id, N'IsScalarFunction') = 1
		)
	DROP FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetOutputRevenueFactor]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetOutputRevenueFactor]
(
	@sASSESSMENT_CODE			VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID	BIGINT,
	@totalInputMass				NUMERIC(25, 3)
)
RETURNS NUMERIC(25, 14) 
AS
BEGIN
	DECLARE @result NUMERIC(25, 14) = 0,
			@productValuePerMass NUMERIC(25, 12) = dbo.DXDIR_PK_CALCULATIONS$GetValuePerMass(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, 1);
	
	IF (@productValuePerMass <> 0)
    BEGIN
	    -- {} - value per tonne
		---[] - margin for
	    -- (WEIGHTS / @totalInputMass) * [ { INCOMES / WEIGHTS } / @productValuePerMass ]
        SELECT @result = ISNULL(SUM(CAST( WEIGHTS / @totalInputMass as NUMERIC(24, 14) ) *
		                            CAST( CAST(INCOMES / WEIGHTS as NUMERIC(24, 14)) / @productValuePerMass as NUMERIC(24, 14) )
							       )
						    , 0)
		FROM
		(
			SELECT CAST(SUM(INCOME) as NUMERIC(24, 4)) as INCOMES,
			       CAST(SUM(WEIGHT) as NUMERIC(24, 3)) as WEIGHTS 
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE = @sASSESSMENT_CODE
			 AND ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID
			 AND OUTPUT_CATEGORY_ID in (4,5,6)
			GROUP BY OUTPUT_CATEGORY_ID, DESTINATION_CODE
			HAVING SUM(INCOME) > 0 AND SUM(WEIGHT) > 0
		) as r;
    END;

	RETURN @result;
END
GO


IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'dbo.DXDIR_PK_CALCULATIONS$GetWasteRelatedCost')
			AND OBJECTPROPERTY(id, N'IsScalarFunction') = 1
		)
	DROP FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetWasteRelatedCost]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DXDIR_PK_CALCULATIONS$GetWasteRelatedCost]
(
	@dcTotalMass		   NUMERIC(25, 3),
	@dcFactor			   NUMERIC(25, 12),
	@dcCost				   NUMERIC(25, 14),
	@dcCoProductMargin     NUMERIC(26, 12),
	@dcCoProductWeight	   NUMERIC(25, 3),
	@dcFoodRescueMargin	   NUMERIC(26, 12),
	@dcFoodRescueWeight	   NUMERIC(25, 3),
	@dcOutputRevenueFactor NUMERIC(25, 14)
)
RETURNS NUMERIC(25, 4) 
AS
BEGIN
	IF (@dcTotalMass = 0)
	BEGIN
		RETURN 0;
	END
	
	RETURN (@dcCost * @dcFactor) - ((@dcCoProductWeight / @dcTotalMass) * @dcCost * @dcCoProductMargin) - ((@dcFoodRescueWeight / @dcTotalMass) * @dcCost * @dcFoodRescueMargin) - (@dcOutputRevenueFactor * @dcCost);
END
GO