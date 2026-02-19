/* Estimated true cost of waste*/
IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_RESULTCALCULATIONS$EstimatedCostOfWaste'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_RESULTCALCULATIONS$EstimatedCostOfWaste]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_RESULTCALCULATIONS$EstimatedCostOfWaste]
	@sASSESSMENT_CODE VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT,
	@dcRESULT NUMERIC(26, 12) OUTPUT
AS
BEGIN
	DECLARE @coProductMargin				 NUMERIC(16, 12),
			@coProductSecondMargin           NUMERIC(16, 12),
			@foodRescueMargin				 NUMERIC(16, 12),
			@sumCostRelatedWaste			 NUMERIC(24, 12),
			@costCoproduct					 NUMERIC(25, 4) = 0,
			@costCoproductSecond             NUMERIC(25, 4) = 0,
			@costFoodRescue					 NUMERIC(25, 4) = 0,
			@sumCostOutput					 NUMERIC(25, 4),
			@wasteCollectionTreat			 NUMERIC(25, 4),
			@revenueFromIncome				 NUMERIC(26, 12);

	DECLARE @tableBusinessCost TABLE (
			   ID BIGINT NOT NULL,
			   ASSESSMENT_CODE VARCHAR(32) NOT NULL,
			   ASSESSMENT_LC_STAGE_ID BIGINT NOT NULL,
			   TYPE VARCHAR(24),
			   TITLE NVARCHAR(256) COLLATE Latin1_General_CS_AS NOT NULL,
			   SORT_ORDER INT,
			   COST NUMERIC(24, 4),
			   TOTAL_COST NUMERIC(25, 4),
			   WASTE_COST NUMERIC(25, 4));

	DECLARE	@v_COPRODUCT_DEST_CODE           VARCHAR(64) = 'COPRODUCT',
		    @v_COPRODUCT_2ND_DEST_CODE       VARCHAR(64) = 'COPRODUCT_2',
		    @v_FOOD_RESCUE_DEST_CODE         VARCHAR(64) = 'FOOD_RESCUE';

	EXEC [dbo].[DXDIR_PK_CALCULATIONS$GetMargin] @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_COPRODUCT_DEST_CODE, @coProductMargin OUTPUT;
	EXEC [dbo].[DXDIR_PK_CALCULATIONS$GetMargin] @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_COPRODUCT_2ND_DEST_CODE, @coProductSecondMargin OUTPUT;
	EXEC [dbo].[DXDIR_PK_CALCULATIONS$GetMargin] @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_FOOD_RESCUE_DEST_CODE, @foodRescueMargin OUTPUT;

	SELECT @costCoproduct = ISNULL(COST, 0)
	FROM DXDIR_OUTPUT
	WHERE  ASSESSMENT_CODE		 = @sASSESSMENT_CODE AND 
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID AND 
		  DESTINATION_CODE       = @v_COPRODUCT_DEST_CODE;

	SELECT @costCoproductSecond = ISNULL(COST, 0)
	FROM DXDIR_OUTPUT
	WHERE  ASSESSMENT_CODE		 = @sASSESSMENT_CODE AND 
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID AND 
		  DESTINATION_CODE       = @v_COPRODUCT_2ND_DEST_CODE;

	SELECT @costFoodRescue = ISNULL(COST, 0)
	FROM DXDIR_OUTPUT
	WHERE ASSESSMENT_CODE		 = @sASSESSMENT_CODE AND 
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID AND 
		  DESTINATION_CODE       = @v_FOOD_RESCUE_DEST_CODE;

	SET @sumCostRelatedWaste	 = @costCoproduct * @coProductMargin + @costCoproductSecond * @coProductSecondMargin + @costFoodRescue * @foodRescueMargin; 
	SET @sumCostOutput			 = dbo.DXDIR_PK_RESULTCALCULATIONS$GetCostOutput(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID);
	SET @wasteCollectionTreat	 = dbo.DXDIR_PK_RESULTCALCULATIONS$WasteCollectionTreatment(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID);
	SET @revenueFromIncome       = dbo.DXDIR_PK_RESULTCALCULATIONS$RevenueFromOutputIncome(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID);

	INSERT INTO @tableBusinessCost EXEC [dbo].[DXDIR_PK_BUSINESS_COST$GetCarriedOverBusinessCosts] @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID;

	SELECT @dcRESULT = ISNULL(SUM(WASTE_COST), 0) + @wasteCollectionTreat - @sumCostRelatedWaste - @revenueFromIncome + @sumCostOutput FROM @tableBusinessCost;
END
GO
