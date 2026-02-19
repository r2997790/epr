IF EXISTS ( SELECT NULL FROM SYS.TYPES WHERE NAME = 'DXDIR_BizCostCarriedKeyTable' )
	DROP TYPE dbo.DXDIR_BizCostCarriedKeyTable
GO

CREATE TYPE dbo.DXDIR_BizCostCarriedKeyTable AS TABLE
(
	[SORT_ORDER]		   INT IDENTITY(1,1) NOT NULL,
	[TITLE_PREV]           NVARCHAR(256) COLLATE Latin1_General_CS_AS NULL, 
	[PBC_COST_PREV]        NUMERIC(25, 4) NULL,
	UNIQUE NONCLUSTERED
	(
		[SORT_ORDER] ASC
	)WITH (IGNORE_DUP_KEY = OFF)
)
GO


IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$InsertRecord'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$InsertRecord]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$InsertRecord]
	@dcID BIGINT,
	@sASSESSMENT_CODE VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT,
	@sTYPE VARCHAR(24),
	@sTITLE NVARCHAR(256),
	@dcSORT_ORDER BIGINT,
	@dcCOST NUMERIC(24, 4)
AS
BEGIN
	INSERT INTO [dbo].[DXDIR_BUSINESS_COST]
	(
		[ID],
		[ASSESSMENT_CODE],
		[ASSESSMENT_LC_STAGE_ID],
		[TYPE],
		[TITLE],
		[SORT_ORDER],
		[COST]
	)
	VALUES
	(
		@dcID,
		@sASSESSMENT_CODE,
		@dcASSESSMENT_LC_STAGE_ID,
		@sTYPE,
		@sTITLE,
		@dcSORT_ORDER,
		@dcCOST
	);
END
GO

IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$UpdateRecord'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$UpdateRecord]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$UpdateRecord]
	@dcID BIGINT,
	@sASSESSMENT_CODE VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT,
	@sTYPE VARCHAR(24),
	@sTITLE NVARCHAR(256),
	@dcSORT_ORDER BIGINT,
	@dcCOST NUMERIC(24, 4)
AS
BEGIN
	UPDATE [dbo].[DXDIR_BUSINESS_COST]
	SET
		[ASSESSMENT_CODE] = @sASSESSMENT_CODE,
		[ASSESSMENT_LC_STAGE_ID] = @dcASSESSMENT_LC_STAGE_ID,
		[TYPE] = @sTYPE,
		[TITLE] = @sTITLE,
		[SORT_ORDER] = @dcSORT_ORDER,
		[COST] = @dcCOST
	WHERE
		[ID] = @dcID;
END
GO

IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$DeleteRecord'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$DeleteRecord]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$DeleteRecord]
	@dcID BIGINT
AS
BEGIN
	DELETE FROM [dbo].[DXDIR_BUSINESS_COST]
	WHERE [ID] = @dcID;
END
GO

IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$GetNextId'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetNextId]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetNextId]
	@dcNextId BIGINT OUTPUT
AS
BEGIN
	SET @dcNextId = NULL;

	DECLARE @dbname NVARCHAR(128) = db_name();

	EXEC DX_PK_UTILITY_SQLSRV$db_sp_get_next_sequence_value @dbname
		,N'dbo'
		,N'DXDIR_BUSINESS_COST_SEQ'
		,@dcNextId OUTPUT;
END
GO


IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$GetBusinessCostGridItems'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetBusinessCostGridItems]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetBusinessCostGridItems]
	@sASSESSMENT_CODE VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT,
	@tblPBC_CARRIED DXDIR_BizCostCarriedKeyTable READONLY
AS
BEGIN
	DECLARE @totalMass						 NUMERIC(25, 3),
	        @factor							 NUMERIC(25, 12),

	        @coProductMargin				 NUMERIC(25, 12),
	        @coProductWeight				 NUMERIC(25, 3) = 0,

			@coProductSecondMargin            NUMERIC(25, 12),
		    @coProductSecondWeight            NUMERIC(25, 3) = 0,

	        @foodRescueMargin				 NUMERIC(25, 12),
	        @foodRescueWeight				 NUMERIC(25, 3) = 0,
			@outputRevenueFactor			 NUMERIC(25, 14),

			@v_COPRODUCT_DEST_CODE           VARCHAR(64) = 'COPRODUCT',
			@v_COPRODUCT_2ND_DEST_CODE       VARCHAR(64) = 'COPRODUCT_2',
			@v_FOOD_RESCUE_DEST_CODE         VARCHAR(64) = 'FOOD_RESCUE';

	EXEC dbo.DXDIR_PK_CALCULATIONS$GetTotalMass @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @totalMass OUTPUT;

	IF (@totalMass = 0)
		BEGIN
			SET @factor = 0;
			SET @outputRevenueFactor = 0;
		END
	ELSE
		BEGIN
			-- massOfWaste / totalInputMass
			SELECT @factor = ROUND(CAST(ISNULL(SUM(WEIGHT), 0) as NUMERIC(25, 12)) / CAST(@totalMass as NUMERIC(25, 12)), 12)
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE = @sASSESSMENT_CODE AND
				  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID AND
				  WEIGHT IS NOT NULL AND
				  OUTPUT_CATEGORY_ID <> 1;

			SET @outputRevenueFactor = dbo.DXDIR_PK_CALCULATIONS$GetOutputRevenueFactor(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @totalMass);
		END

	EXEC dbo.DXDIR_PK_CALCULATIONS$GetMargin @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_COPRODUCT_DEST_CODE, @coProductMargin OUTPUT;
	EXEC dbo.DXDIR_PK_CALCULATIONS$GetMargin @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_COPRODUCT_2ND_DEST_CODE, @coProductSecondMargin OUTPUT;
	EXEC dbo.DXDIR_PK_CALCULATIONS$GetMargin @sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID, @v_FOOD_RESCUE_DEST_CODE, @foodRescueMargin OUTPUT;

	SELECT @coProductWeight = ISNULL(WEIGHT, 0)
	FROM DXDIR_OUTPUT
	WHERE ASSESSMENT_CODE		 = @sASSESSMENT_CODE			AND
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID	AND
		  DESTINATION_CODE	     = @v_COPRODUCT_DEST_CODE;

	SELECT @coProductSecondWeight = ISNULL(WEIGHT, 0)
	FROM DXDIR_OUTPUT
	WHERE ASSESSMENT_CODE		 = @sASSESSMENT_CODE			AND
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID	AND
		  DESTINATION_CODE	     = @v_COPRODUCT_2ND_DEST_CODE;

	SELECT @foodRescueWeight = ISNULL(WEIGHT, 0)
	FROM DXDIR_OUTPUT
	WHERE ASSESSMENT_CODE		 = @sASSESSMENT_CODE			AND
		  ASSESSMENT_LC_STAGE_ID = @dcASSESSMENT_LC_STAGE_ID	AND
		  DESTINATION_CODE	 = @v_FOOD_RESCUE_DEST_CODE;

	WITH filteredBusinessCost AS
    (
        SELECT ID, TITLE, SORT_ORDER, COST
        FROM DXDIR_BUSINESS_COST
        WHERE ASSESSMENT_CODE		   = @sASSESSMENT_CODE	AND
              ASSESSMENT_LC_STAGE_ID   = @dcASSESSMENT_LC_STAGE_ID
    ),
	joinedWithPCBCarried AS
	(
        SELECT
            COALESCE(BC.ID, -CR.SORT_ORDER)                  AS ID,
            COALESCE(BC.TITLE, CR.TITLE_PREV)                AS TITLE,
            COALESCE(BC.SORT_ORDER, -CR.SORT_ORDER)          AS SORT_ORDER,
            BC.COST                                          AS COST,
            ISNULL(BC.COST, 0) + ISNULL(CR.PBC_COST_PREV, 0) AS TOTAL_COST,
			ISNULL(CR.PBC_COST_PREV, 0)						 AS CARRIED_COST
        FROM filteredBusinessCost AS BC
        FULL OUTER JOIN @tblPBC_CARRIED AS CR
        ON BC.TITLE = CR.TITLE_PREV
	)
	SELECT 
	   ID,
       @sASSESSMENT_CODE AS ASSESSMENT_CODE,
       @dcASSESSMENT_LC_STAGE_ID AS ASSESSMENT_LC_STAGE_ID,
       NULL AS TYPE,
       TITLE,
       SORT_ORDER,
       COST,
	   TOTAL_COST,
	   CARRIED_COST,
	   dbo.DXDIR_PK_CALCULATIONS$GetWasteRelatedCost(@totalMass, @factor, TOTAL_COST,
	                                                 @coProductMargin, @coProductWeight,
													 @coProductSecondMargin, @coProductSecondWeight,
													 @foodRescueMargin, @foodRescueWeight,
													 @outputRevenueFactor) AS WASTE_COST
    FROM joinedWithPCBCarried;
END
GO


IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'dbo.DXDIR_PK_BUSINESS_COST$GetLcStageCarriedOverSequence')
			AND OBJECTPROPERTY(id, N'IsTableFunction') = 1
		)
	DROP FUNCTION dbo.DXDIR_PK_BUSINESS_COST$GetLcStageCarriedOverSequence
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION dbo.DXDIR_PK_BUSINESS_COST$GetLcStageCarriedOverSequence
(
	@sASSESSMENT_CODE			VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID	BIGINT
)
RETURNS @dcRESULT TABLE	(
		ROWNUM INT IDENTITY(1, 1) UNIQUE,
		CONTENT BIGINT
	)
AS
BEGIN

	DECLARE @arr_Ids_SourceIds TABLE (
				ROWNUM INT IDENTITY(1, 1) PRIMARY KEY,
				ID BIGINT NOT NULL,
				SOURCE_ID BIGINT NULL
			);

	DECLARE @idx INT,
			@cnt INT;

	DECLARE @curr_Id BIGINT,
			@curr_SourceId BIGINT,
			@next_Id BIGINT,
			@next_SourceId BIGINT;

	INSERT INTO @arr_Ids_SourceIds (ID, SOURCE_ID)
	SELECT							ID, SOURCE_ASMT_LC_STAGE_ID
	FROM DXDIR_ASSESSMENT_LC_STAGE
	WHERE ASSESSMENT_CODE = @sASSESSMENT_CODE
	  AND SORT_ORDER <= (SELECT SORT_ORDER FROM DXDIR_ASSESSMENT_LC_STAGE WHERE ID = @dcASSESSMENT_LC_STAGE_ID)
	ORDER BY SORT_ORDER DESC;

	SET @idx = 1;
	SELECT @cnt = COUNT(*) FROM @arr_Ids_SourceIds;

	WHILE @idx <= @cnt
	BEGIN
		SET @curr_Id = NULL;
		SET @curr_SourceId = NULL;

		SELECT @curr_Id = ID,
		       @curr_SourceId = SOURCE_ID
		FROM @arr_Ids_SourceIds WHERE ROWNUM = @idx;

		IF @idx < @cnt
		BEGIN
			SELECT @next_Id = ID,
				   @next_SourceId = SOURCE_ID
			FROM @arr_Ids_SourceIds WHERE ROWNUM = (@idx + 1);
		END


		IF ((@next_Id IS NOT NULL) AND (@next_Id = @curr_SourceId))
		BEGIN
			IF @idx = 1
				INSERT INTO @dcRESULT (CONTENT) VALUES (@curr_Id);

			INSERT INTO @dcRESULT (CONTENT) VALUES (@next_Id);
		END
		ELSE IF @idx = 1 
		BEGIN
			-- allways add starting stage id
			INSERT INTO @dcRESULT (CONTENT) VALUES (@curr_Id);
			BREAK; -- exit loop
		END
		ELSE
		BEGIN
			BREAK; -- exit loop
		END

		SET @next_Id = NULL;
		SET @next_SourceId = NULL;

		SET @idx = @idx + 1;
	END --LOOP while


	RETURN
END
GO


IF EXISTS ( SELECT *
			FROM sys.objects so
			JOIN sys.schemas sc ON so.schema_id = sc.schema_id
			WHERE so.NAME = N'DXDIR_PK_BUSINESS_COST$GetCarriedOverBusinessCosts'
			  AND so.type IN (N'P') AND sc.NAME = N'dbo' )
	DROP PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetCarriedOverBusinessCosts]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DXDIR_PK_BUSINESS_COST$GetCarriedOverBusinessCosts]
	@sASSESSMENT_CODE VARCHAR(32),
	@dcASSESSMENT_LC_STAGE_ID BIGINT
AS
BEGIN
	DECLARE @v_AssessmentLcStageIds DX_NumberTable,
			@asceding_SeqOfLcStageIds DX_NumberTable;

	DECLARE @tablePBCsCarried DXDIR_BizCostCarriedKeyTable;

	DECLARE @lcStagesCount INT = 0,
			@idx_MainLoop INT,
	        @iterationCounter INT;

	DECLARE @asmtLcStageId BIGINT;
	
	DECLARE @tableBusinessCost TABLE (
			   ID BIGINT NOT NULL,
			   ASSESSMENT_CODE VARCHAR(32) NOT NULL,
			   ASSESSMENT_LC_STAGE_ID BIGINT NOT NULL,
			   TYPE VARCHAR(24),
			   TITLE NVARCHAR(256) COLLATE Latin1_General_CS_AS NOT NULL,
			   SORT_ORDER INT,
			   COST NUMERIC(24, 4),
			   TOTAL_COST NUMERIC(25, 4),
			   CARRIED_COST NUMERIC(25, 4),
			   WASTE_COST NUMERIC(25, 4));


	INSERT INTO @v_AssessmentLcStageIds
	SELECT CONTENT FROM dbo.DXDIR_PK_BUSINESS_COST$GetLcStageCarriedOverSequence(@sASSESSMENT_CODE, @dcASSESSMENT_LC_STAGE_ID);

	SELECT @lcStagesCount = COUNT(*) FROM @v_AssessmentLcStageIds;
	
	IF @lcStagesCount > 0
	BEGIN
		INSERT INTO @asceding_SeqOfLcStageIds
		SELECT ID FROM DXDIR_ASSESSMENT_LC_STAGE
		WHERE ASSESSMENT_CODE = @sASSESSMENT_CODE AND
			  ID IN (SELECT CONTENT FROM @v_AssessmentLcStageIds)
		ORDER BY SORT_ORDER ASC;

		SELECT @lcStagesCount = COUNT(*) FROM @asceding_SeqOfLcStageIds;

		SET @idx_mainLoop = 1;

		SET @iterationCounter = 0;

		-- main loop (Oracle FOR)
		WHILE @idx_MainLoop <= @lcStagesCount
		BEGIN
			SET @asmtLcStageId = (SELECT CONTENT FROM @asceding_SeqOfLcStageIds WHERE ROWNUM = @idx_MainLoop);
			
			IF @iterationCounter = 0
			BEGIN
				INSERT INTO  @tableBusinessCost
				EXEC dbo.DXDIR_PK_BUSINESS_COST$GetBusinessCostGridItems @sASSESSMENT_CODE, @asmtLcStageId, @tablePBCsCarried;
			END
			ELSE -- iterations after initial
			BEGIN
				DELETE FROM @tableBusinessCost;
				
				INSERT INTO  @tableBusinessCost
				EXEC dbo.DXDIR_PK_BUSINESS_COST$GetBusinessCostGridItems @sASSESSMENT_CODE, @asmtLcStageId, @tablePBCsCarried;

				DELETE FROM @tablePBCsCarried;
			END

			SET @iterationCounter = @iterationCounter + 1;

			IF @iterationCounter = @lcStagesCount
			BEGIN
				DELETE FROM @tablePBCsCarried;
				-- break loop, at this point @tableBusinessCost is holding target stage results just selected them at the end
				BREAK;
			END

			INSERT INTO @tablePBCsCarried (TITLE_PREV, PBC_COST_PREV)
			SELECT TITLE, (ISNULL(TOTAL_COST, 0) - ISNULL(WASTE_COST, 0))
			FROM @tableBusinessCost;
			
			SET @idx_mainLoop = @idx_mainLoop + 1;
		END -- main loop (Oracle FOR)
	END -- if

	-- result set
	SELECT ID,
			ASSESSMENT_CODE,
			ASSESSMENT_LC_STAGE_ID,
			TYPE,
			TITLE,
			SORT_ORDER,
			COST,
			TOTAL_COST,
			CARRIED_COST,
			WASTE_COST
	FROM @tableBusinessCost;
END
GO
