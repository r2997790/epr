
IF EXISTS ( SELECT NULL FROM dbo.systypes WHERE name = N'DXDIR_PK_NEWATTVALUEASSESSMENT$MemberDefTableType'  )  
	DROP TYPE DXDIR_PK_NEWATTVALUEASSESSMENT$MemberDefTableType
GO

CREATE TYPE DXDIR_PK_NEWATTVALUEASSESSMENT$MemberDefTableType AS TABLE 
(
	[ROWNUM]		INT IDENTITY(1,1) UNIQUE,
	[ID]			BIGINT,
	[TYPE]			VARCHAR(MAX) COLLATE Latin1_General_CS_AS,
	[STORAGE]		VARCHAR(MAX) COLLATE Latin1_General_CS_AS
)
GO
 

IF EXISTS ( SELECT NULL FROM dbo.systypes WHERE name = N'DXDIR_PK_NEWATTVALUEASSESSMENT$ElementType'  )  
	DROP TYPE DXDIR_PK_NEWATTVALUEASSESSMENT$ElementType
GO

CREATE TYPE DXDIR_PK_NEWATTVALUEASSESSMENT$ElementType AS TABLE
(
	[ROWNUM]		INT IDENTITY(1,1) UNIQUE,
    [OBJECT_PK]		VARCHAR(MAX) COLLATE Latin1_General_CS_AS,
	[CODE]			VARCHAR(32) COLLATE Latin1_General_CS_AS,
    [NAME]			VARCHAR(MAX) COLLATE Latin1_General_CS_AS,
    [ID]			BIGINT,
    [ARRAY_INDEX]	BIGINT,
    [VALUE]			NVARCHAR(MAX) COLLATE Latin1_General_CS_AS,
    [REF_SET]		VARCHAR(MAX)  COLLATE Latin1_General_CS_AS  
)
GO

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn 
(
	@sCOLUMN_NAME VARCHAR(MAX)
)
AS
BEGIN
	
	IF @sCOLUMN_NAME NOT IN 
		('CODE', 'TYPE_CODE', 'DESCRIPTION', 'STATUS', 'COMPANY_NAME', 'COMPLETING_BY', 'PHONE', 'EMAIL', 'TIMEFRAME_FROM', 'TIMEFRAME_TO', 'CREATE_DATE', 'CREATED_BY', 'MOD_DATE', 'MODIFIED_BY', 'AUTHORIZATION_ROLE', 'ORG_STRUCTURE', 'LOCATION', 'PROD_CLASSIF')
		RAISERROR('Invalid column name', 16, 0)
END
GO

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable
	@sCODE			VARCHAR(32),
	@sATT_VALUE		NVARCHAR(max),
	@sATT_TYPE		VARCHAR(max),
	@sFIELD			VARCHAR(max)
AS
BEGIN
	BEGIN TRY
	
		-- Prevent SQL injection on field
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn @sFIELD

		DECLARE @vc2VALUE VARCHAR(MAX),
				@vc2UNIT_VALUE VARCHAR(16),
				@v_field_unit VARCHAR(32),
				@v_updateSqlString NVARCHAR(MAX)

		SET @v_updateSqlString = NULL

		IF (@sATT_VALUE IS NULL)
		BEGIN
			IF @sATT_TYPE = 'AMOUNT' OR @sATT_TYPE = 'MEASURE'
			BEGIN
				IF @sATT_TYPE = 'AMOUNT'
					SET @v_field_unit = @sFIELD + '_C'
				ELSE IF @sATT_TYPE = 'MEASURE'
					SET @v_field_unit = @sFIELD + '_UM'

				SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = NULL' + ',' + @v_field_unit + '= NULL WHERE CODE = @sCODE'
			END
			ELSE
				SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = NULL WHERE CODE = @sCODE'
			
			EXEC sp_executesql @v_updateSqlString, N'@sCODE VARCHAR(32)', @sCODE
		END
		ELSE
		BEGIN
			IF @sATT_TYPE = 'AMOUNT' OR @sATT_TYPE = 'MEASURE'
			BEGIN
				IF @sATT_TYPE = 'AMOUNT'
					SET @v_field_unit = @sFIELD + '_C'
				ELSE IF @sATT_TYPE = 'MEASURE'
					SET @v_field_unit = @sFIELD + '_UM'

				EXECUTE dbo.DX_PK_NEWATTRIBUTESUTILITIES$GetAggregateValueTokens 
					@vc2VALUE,
					@vc2VALUE OUTPUT,
					@vc2UNIT_VALUE OUTPUT

				SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = CAST(@vc2VALUE AS numeric(28,15)), ' + @v_field_unit + '= @vc2UNIT_VALUE WHERE CODE = @sCODE'
				EXEC sp_executesql @v_updateSqlString, N'@vc2VALUE NVARCHAR(max), @vc2UNIT_VALUE NVARCHAR(max), @sCODE VARCHAR(32)', @vc2VALUE, @vc2UNIT_VALUE, @sCODE
			END
			ELSE
			BEGIN
				IF @sATT_TYPE = 'STRING' OR @sATT_TYPE = 'LISTOFVAL' OR @sATT_TYPE = 'MEMO'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = @sATT_VALUE WHERE CODE = @sCODE'
				ELSE IF @sATT_TYPE = 'NUMBER'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = CAST(@sATT_VALUE AS numeric(28,15)) WHERE CODE = @sCODE'
				ELSE IF @sATT_TYPE = 'BOOL'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = CAST(@sATT_VALUE AS BIT) WHERE CODE = @sCODE'
				ELSE IF @sATT_TYPE = 'DATETIME'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = dbo.DX_PK_UTILITY_SQLSRV$to_date2(@sATT_VALUE, ''YYYYMMDDHH24MISS'') WHERE CODE = @sCODE'
				ELSE IF @sATT_TYPE = 'DATE'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = dbo.DX_PK_UTILITY_SQLSRV$to_date2(@sATT_VALUE, ''YYYYMMDD'') WHERE CODE = @sCODE'
				ELSE IF @sATT_TYPE = 'TIME'
					SET @v_updateSqlString = 'UPDATE DXDIR_ASSESSMENT SET ' + @sFIELD + ' = dbo.DX_PK_UTILITY_SQLSRV$to_date2(@sATT_VALUE, ''YYYYMMDDHH24MISS'') WHERE CODE = @sCODE'
			
				EXEC sp_executesql @v_updateSqlString, N'@sATT_VALUE NVARCHAR(max), @sCODE VARCHAR(32)', @sATT_VALUE, @sCODE
			END
		END
	END TRY
	BEGIN CATCH
		DECLARE @ERROR_NUMBER INT = ERROR_NUMBER()
		DECLARE @ERROR_MESSAGE NVARCHAR(MAX) = ERROR_MESSAGE()
		DECLARE @ERROR_SEVERITY INT = ERROR_SEVERITY()
		DECLARE @ERROR_STATE INT = ERROR_STATE()

		IF @ERROR_NUMBER = 515 -- Inserted NULL in a non-nullable column
			RETURN
				
		RAISERROR (@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
	END CATCH
END
GO


IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsOnMasterTable') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsOnMasterTable
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsOnMasterTable 
(
	@sFIELD VARCHAR(MAX),
	@sWHERE_CLAUSES VARCHAR(MAX),
	@v_exists BIT OUTPUT
)
AS
BEGIN
	BEGIN TRY

		-- Prevent SQL injection on field
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn @sFIELD

		DECLARE @v_statement NVARCHAR(MAX) = N'SELECT @v_exists = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END) FROM DXDIR_ASSESSMENT WHERE CODE = @sCODE AND ' + @sFIELD + N' IS NOT NULL'

		EXEC sp_executesql @v_statement, N'@v_exists BIT OUT', @v_exists OUTPUT

	END TRY
	BEGIN CATCH
		DECLARE @ERROR_NUMBER INT = ERROR_NUMBER()
		DECLARE @ERROR_MESSAGE NVARCHAR(MAX) = ERROR_MESSAGE()
		DECLARE @ERROR_SEVERITY INT = ERROR_SEVERITY()
		DECLARE @ERROR_STATE INT = ERROR_STATE()
				
		RAISERROR (@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
	END CATCH
END
GO


IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTable') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTable
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTable
(
	@sCODE		VARCHAR(32),
	@sFIELD NVARCHAR(MAX),
	@sATT_TYPE VARCHAR(MAX),
	@sATT_VALUE NVARCHAR(MAX),
	@v_exists BIT OUTPUT
)
AS
BEGIN

	BEGIN TRY

		-- Prevent SQL injection on field
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn @sFIELD

		SET @v_exists = 0
		DECLARE @v_statement NVARCHAR(MAX) = N'SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE FROM DXDIR_ASSESSMENT WHERE '

		IF @sATT_VALUE IS NULL
		BEGIN
			SET @v_statement = @v_statement + @sFIELD + N' IS NULL AND CODE = @sCODE'
			EXEC sp_executesql @v_statement, N'@v_exists BIT OUT, @sCODE VARCHAR(32)', @v_exists OUTPUT, @sCODE
		END
		ELSE
		BEGIN
			SET @v_statement = 
			CASE 
				WHEN @sATT_TYPE = 'AMOUNT'
					THEN @v_statement + @sFIELD + N' = SUBSTRING(@sATT_VALUE, 1, CHARINDEX(@sATT_VALUE, N''|'') - 1) AND CODE = @sCODE'
				WHEN @sATT_TYPE = 'MEASURE'
					THEN @v_statement + @sFIELD + N' = SUBSTRING(@sATT_VALUE, 1, CHARINDEX(@sATT_VALUE, N''|'') - 1) AND CODE = @sCODE'
				WHEN @sATT_TYPE = 'NUMBER'
					THEN @v_statement + @sFIELD + N' = @sATT_VALUE AND CODE = @sCODE'
				WHEN @sATT_TYPE = 'DATETIME'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDDHH24MISS'' ) AND CODE = @sCODE'
				WHEN @sATT_TYPE = 'TIME'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDDHH24MISS'' ) AND CODE = @sCODE'
				WHEN @sATT_TYPE = 'DATE'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDD'' ) AND CODE = @sCODE'
				ELSE 
					@v_statement + @sFIELD + N' = @sATT_VALUE AND CODE = @sCODE'
			END

			EXEC sp_executesql @v_statement, N'@v_exists BIT OUT, @sCODE VARCHAR(32)', @v_exists OUTPUT, @sCODE
		END

	END TRY
	BEGIN CATCH
		DECLARE @ERROR_NUMBER INT = ERROR_NUMBER()
		DECLARE @ERROR_MESSAGE NVARCHAR(MAX) = ERROR_MESSAGE()
		DECLARE @ERROR_SEVERITY INT = ERROR_SEVERITY()
		DECLARE @ERROR_STATE INT = ERROR_STATE()
				
		RAISERROR (@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
	END CATCH

END
GO

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTableAnyCnt') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTableAnyCnt
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTableAnyCnt
(
	@sFIELD NVARCHAR(MAX),
	@sATT_TYPE VARCHAR(MAX),
	@sATT_VALUE NVARCHAR(MAX),
	@v_exists BIT OUTPUT
)
AS
BEGIN

	BEGIN TRY
		
		-- Prevent SQL injection on field
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$AssertIsMasterTableColumn @sFIELD

		SET @v_exists = 0
		DECLARE @v_statement NVARCHAR(MAX) = N'SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE FROM DXDIR_ASSESSMENT WHERE '

		IF @sATT_VALUE IS NULL
		BEGIN
			SET @v_statement = @v_statement + @sFIELD + N' IS NULL'
			EXEC sp_executesql @v_statement, N'@v_exists BIT OUT, @sCODE VARCHAR(32)', @v_exists OUTPUT
		END
		ELSE
		BEGIN
			SET @v_statement = 
			CASE 
				WHEN @sATT_TYPE = 'AMOUNT'
					THEN @v_statement + @sFIELD + N' = SUBSTRING(@sATT_VALUE, 1, CHARINDEX(@sATT_VALUE, N''|'') - 1)'
				WHEN @sATT_TYPE = 'MEASURE'
					THEN @v_statement + @sFIELD + N' = SUBSTRING(@sATT_VALUE, 1, CHARINDEX(@sATT_VALUE, N''|'') - 1)'
				WHEN @sATT_TYPE = 'NUMBER'
					THEN @v_statement + @sFIELD + N' = @sATT_VALUE'
				WHEN @sATT_TYPE = 'DATETIME'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDDHH24MISS'' )'
				WHEN @sATT_TYPE = 'TIME'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDDHH24MISS'' )'
				WHEN @sATT_TYPE = 'DATE'
					THEN @v_statement + @sFIELD + N' = dbo.DX_PK_UTILITY_SQLSRV$to_char_date(@sATT_VALUE, ''YYYYMMDD'' )'
				ELSE 
					@v_statement + @sFIELD + N' = @sATT_VALUE'
			END

			EXEC sp_executesql @v_statement, N'@v_exists BIT OUT', @v_exists OUTPUT
		END
	END TRY
	BEGIN CATCH
		DECLARE @ERROR_NUMBER INT = ERROR_NUMBER()
		DECLARE @ERROR_MESSAGE NVARCHAR(MAX) = ERROR_MESSAGE()
		DECLARE @ERROR_SEVERITY INT = ERROR_SEVERITY()
		DECLARE @ERROR_STATE INT = ERROR_STATE()
				
		RAISERROR (@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
	END CATCH

END
GO


	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$InsertAndUpdateIndexes') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertAndUpdateIndexes
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertAndUpdateIndexes 
(
    @sCODE				VARCHAR(32),
	@dcID				BIGINT,
    @dcINDEX_TO_INSERT	BIGINT,
	@sATT_VALUE			NVARCHAR(MAX)
)
AS
BEGIN

	DECLARE @bExistsCell BIT = 0

	
	-- Check if requested cell already exists
	IF EXISTS ( 
		SELECT NULL
		FROM DXDIR_ATTVALUE_ASSESSMENT
		WHERE
			CODE = @sCODE
		AND ID = @dcID
		AND COUNTRY = 'NNN' 
		AND [LANGUAGE] = 'nn' 
		AND ARRAY_INDEX = @dcINDEX_TO_INSERT
	) 
		SET @bExistsCell = 1


	IF @bExistsCell = 1
	BEGIN

		-- Cell exists, shift the existing cells
		DECLARE
			@lastValue NVARCHAR(MAX),
			@lastArrayIndex BIGINT

		SELECT
			@lastArrayIndex = ARRAY_INDEX, 
			@lastValue = VALUE
		FROM DXDIR_ATTVALUE_ASSESSMENT A1
		WHERE
			CODE = @sCODE
		AND ID = @dcID
		AND COUNTRY='NNN' 
		AND [LANGUAGE]='nn'
		AND ARRAY_INDEX = ( 
			SELECT MAX(ARRAY_INDEX)
			FROM DXDIR_ATTVALUE_ASSESSMENT A2 
			WHERE
				A2.[CODE] = A1.[CODE]
			AND A2.ID = A1.ID
			AND A2.COUNTRY='NNN' 
			AND A2.[LANGUAGE]='nn'
		)

		
		IF @dcINDEX_TO_INSERT<@lastArrayIndex
		BEGIN
			-- All cells that follow the one to be inserted must be shifted one position down
			UPDATE DXDIR_ATTVALUE_ASSESSMENT
			SET 
				DXDIR_ATTVALUE_ASSESSMENT.VALUE = SHIFTED_VALUES.VALUE
			FROM (
				SELECT  A1.CODE,
						A1.ID,
						A1.COUNTRY,
						A1.[LANGUAGE],
						A1.ARRAY_INDEX,
						A1.VALUE,
						MIN(A2.ARRAY_INDEX) AS NEXT_ARRAY_INDEX
				FROM DXDIR_ATTVALUE_ASSESSMENT A1
					INNER JOIN DXDIR_ATTVALUE_ASSESSMENT A2
						ON A1.[CODE] = A2.[CODE]
						AND A1.ID = A2.ID
						AND A1.COUNTRY= A2.COUNTRY
						AND A1.[LANGUAGE]= A2.[LANGUAGE]
				WHERE A1.CODE = @sCODE
				AND A1.ID = @dcID
				AND A1.COUNTRY='NNN' 
				AND A1.[LANGUAGE]='nn'
				AND A1.ARRAY_INDEX >= @dcINDEX_TO_INSERT
				AND A2.ARRAY_INDEX > A1.ARRAY_INDEX
				GROUP BY A1.CODE,
						A1.ID,
						A1.COUNTRY,
						A1.[LANGUAGE],
						A1.ARRAY_INDEX,
						A1.VALUE
			) SHIFTED_VALUES
			WHERE
				DXDIR_ATTVALUE_ASSESSMENT.[CODE] = SHIFTED_VALUES.[CODE]
			AND DXDIR_ATTVALUE_ASSESSMENT.ID = SHIFTED_VALUES.ID
			AND DXDIR_ATTVALUE_ASSESSMENT.COUNTRY= SHIFTED_VALUES.COUNTRY
			AND DXDIR_ATTVALUE_ASSESSMENT.[LANGUAGE]= SHIFTED_VALUES.[LANGUAGE]
			AND DXDIR_ATTVALUE_ASSESSMENT.ARRAY_INDEX = SHIFTED_VALUES.NEXT_ARRAY_INDEX
		END
	
		-- In order to shift all cells down, a new cell must be added at the end of the previous list
		INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
			(CODE, ID, COUNTRY, [LANGUAGE], ARRAY_INDEX, VALUE)
		VALUES
			(@sCODE, @dcID, 'NNN', 'nn', @lastArrayIndex+1,  @lastValue)

	END

	IF @sATT_VALUE IS NULL
	BEGIN 
		IF @bExistsCell = 1
			-- The client has requested the insertion of a NULL value in this cell position
			-- No cell will be actually inserted, cells needed just to be shifted down and requested cell must be deleted
			DELETE DXDIR_ATTVALUE_ASSESSMENT
			WHERE 
				CODE = @sCODE
			AND ID= @dcID
			AND COUNTRY='NNN' 
			AND [LANGUAGE]='nn'  
			AND ARRAY_INDEX = @dcINDEX_TO_INSERT
	END
	ELSE
	BEGIN
		IF @bExistsCell = 1
			-- Update the existing cell
			UPDATE DXDIR_ATTVALUE_ASSESSMENT
				SET VALUE = @sATT_VALUE
    WHERE 
		CODE = @sCODE 
		AND ID=@dcID
		AND COUNTRY='NNN' 
		AND [LANGUAGE]='nn' 
			AND ARRAY_INDEX = @dcINDEX_TO_INSERT
		ELSE
			-- Insert a new cell
			INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
				(CODE, ID, COUNTRY, [LANGUAGE], ARRAY_INDEX, VALUE)
					VALUES
				(@sCODE, @dcID, 'NNN', 'nn', @dcINDEX_TO_INSERT,  @sATT_VALUE)
	END

END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAndUpdateIndexes') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAndUpdateIndexes
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAndUpdateIndexes 
(
    @sCODE			VARCHAR(32),
	@dcID			BIGINT,
    @dcDELETE_INDEX	BIGINT,
	@bUpdateIndexes	BIT = 1
)
AS
BEGIN
	
	IF @bUpdateIndexes=0
	BEGIN
		
		-- Just need to delete the cell without moving the positions of the remaining ones
		-- This mainly occurs because the attribute is member of a set and then rows position must not be changed
		DELETE FROM DXDIR_ATTVALUE_ASSESSMENT 
		WHERE CODE = @sCODE
		AND ID = @dcID 
		AND COUNTRY = 'NNN' 
		AND [LANGUAGE] = 'nn' 
		AND ARRAY_INDEX = @dcDELETE_INDEX
		
		RETURN 

	END
	
	-- Shift the position of the remaining cells

	DECLARE
		@valueAfterDelete NVARCHAR(MAX),
		@arrayIndexAfterDelete BIGINT,
		@lastArrayIndex BIGINT

	SELECT
		@arrayIndexAfterDelete = ARRAY_INDEX, 
		@valueAfterDelete = VALUE
	FROM DXDIR_ATTVALUE_ASSESSMENT A1
	WHERE CODE = @sCODE
	AND ID = @dcID
	AND COUNTRY='NNN' 
	AND [LANGUAGE]='nn'
	AND ARRAY_INDEX = ( 
		SELECT MIN(ARRAY_INDEX)
		FROM DXDIR_ATTVALUE_ASSESSMENT A2 
		WHERE A2.[CODE] = A1.[CODE]
		AND A2.ID = A1.ID
		AND A2.COUNTRY='NNN' 
		AND A2.[LANGUAGE]='nn'
		AND A2.ARRAY_INDEX > @dcDELETE_INDEX
	)

	IF @arrayIndexAfterDelete IS NULL
		-- No cells left or the cell with the highest position is going to be deleted
		-- No need to shift any cell
	BEGIN

		DELETE FROM DXDIR_ATTVALUE_ASSESSMENT 
		WHERE CODE = @sCODE
		AND ID = @dcID 
		AND COUNTRY = 'NNN' 
		AND [LANGUAGE] = 'nn' 
		AND ARRAY_INDEX = @dcDELETE_INDEX
		
		RETURN 

	END

	-- Get the last cell index
	SELECT @lastArrayIndex = MAX(ARRAY_INDEX)
	FROM DXDIR_ATTVALUE_ASSESSMENT
	WHERE CODE = @sCODE
	AND ID = @dcID
	AND COUNTRY='NNN' 
	AND [LANGUAGE]='nn'


	IF @arrayIndexAfterDelete<@lastArrayIndex
	BEGIN
		-- There is more than one cell following the one to be deleted
		-- All cells must be shifted one position up
		UPDATE DXDIR_ATTVALUE_ASSESSMENT
		SET 
			DXDIR_ATTVALUE_ASSESSMENT.VALUE = SHIFTED_VALUES.VALUE
		FROM (
			SELECT  A1.CODE,
					A1.ID,
					A1.COUNTRY,
					A1.[LANGUAGE],
					A1.ARRAY_INDEX,
					A1.VALUE,
					MAX(A2.ARRAY_INDEX) AS PREV_ARRAY_INDEX
			FROM DXDIR_ATTVALUE_ASSESSMENT A1
				INNER JOIN DXDIR_ATTVALUE_ASSESSMENT A2 
					ON A2.[CODE] = A1.[CODE]
					AND A2.ID = A1.ID
					AND A2.COUNTRY= A1.COUNTRY
					AND A2.[LANGUAGE]= A1.[LANGUAGE]
			WHERE A1.CODE = @sCODE
			AND A1.ID = @dcID
			AND A1.COUNTRY='NNN' 
			AND A1.[LANGUAGE]='nn'
			AND A1.ARRAY_INDEX >= @arrayIndexAfterDelete
			AND A2.ARRAY_INDEX < A1.ARRAY_INDEX
			GROUP BY A1.CODE,
					A1.ID,
					A1.COUNTRY,
					A1.[LANGUAGE],
					A1.ARRAY_INDEX,
					A1.VALUE
		) SHIFTED_VALUES
		WHERE DXDIR_ATTVALUE_ASSESSMENT.[CODE] = SHIFTED_VALUES.[CODE]
		AND DXDIR_ATTVALUE_ASSESSMENT.ID = SHIFTED_VALUES.ID
		AND DXDIR_ATTVALUE_ASSESSMENT.COUNTRY= SHIFTED_VALUES.COUNTRY
		AND DXDIR_ATTVALUE_ASSESSMENT.[LANGUAGE]= SHIFTED_VALUES.[LANGUAGE]
		AND DXDIR_ATTVALUE_ASSESSMENT.ARRAY_INDEX = SHIFTED_VALUES.PREV_ARRAY_INDEX

	END
	ELSE
		-- Only one cell is present after the one to be deleted; set its value in the position of the cell to be deleted 
		-- not to leave a hole in the ARRAY_INDEX values
		UPDATE DXDIR_ATTVALUE_ASSESSMENT
			SET VALUE = @valueAfterDelete
		WHERE CODE = @sCODE
		AND ID = @dcID
		AND COUNTRY='NNN' 
		AND [LANGUAGE]='nn'
		AND ARRAY_INDEX = @dcDELETE_INDEX


	-- Delete last cell after having shifted cells values
	DELETE DXDIR_ATTVALUE_ASSESSMENT
	WHERE CODE = @sCODE
		AND ID=@dcID
		AND COUNTRY='NNN' 
		AND [LANGUAGE]='nn'
	AND ARRAY_INDEX = @lastArrayIndex

END
GO


-- Inserts a new value to the database.
-- If needed, update array index before insert.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$InsertRecord') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertRecord
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertRecord   
(
	@sCODE		VARCHAR(32),
    @sNAME          VARCHAR(MAX),
    @dcId			BIGINT,
    @dcARRAY_INDEX	BIGINT,
    @sVALUE         NVARCHAR(MAX),
    @sREF_SET       VARCHAR(MAX)
)
AS    
BEGIN

    DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
    DECLARE @v_type  VARCHAR(128)
	DECLARE @v_objectPkey varchar(max) = @sCODE
		
	-- Retrieve information about attribute storage
	EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetStorageInfoFromAttribute @dcID, @v_table OUTPUT, @v_field OUTPUT, @v_type OUTPUT
        
        -- MEMO: handle the memo values first
    IF @v_table = 'DX_ATTVALUE_MEMO'
	BEGIN
		EXEC DX_PK_NEWATTVALUEMEMO$InsertRecord @v_objectPkey, @sNAME,  @dcID, @dcARRAY_INDEX,  @sVALUE, @sREF_SET
        RETURN
    END
        
    -- MATERIALIZATION: Insert materialized attribute first, if it is the case
	IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_BASE_ASSESSMENT$MergeValue @sCODE, dcID, @dcARRAY_INDEX, @sVALUE, @sREF_SET, 0
        RETURN
    END

    IF @v_field IS NOT NULL
	BEGIN
		-- Master Table
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable  @sCODE, @sVALUE, @v_type, @v_field
	END
    ELSE
	BEGIN
        IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
			EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$InsertAndUpdateIndexes @sCODE, @dcID, @dcARRAY_INDEX, @sVALUE
        ELSE -- Generic Table (DX_ATTVALUE)
			EXEC DX_PK_NEWATTRIBUTESUTILITIES$InsertRecordOnStandardTables @v_objectPkey, @dcID, @dcARRAY_INDEX,  @sVALUE, @v_table
    END
END
GO
  
	
-- Inserts a new value to the database.
-- No array index update is performed because this procedure is supposed to be called when inserting data massively.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$BulkInsertRecord') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$BulkInsertRecord
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$BulkInsertRecord
(
	@sCODE			VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcID            BIGINT,
    @dcARRAY_INDEX   BIGINT,
    @sVALUE          NVARCHAR(MAX),
    @sREF_SET        VARCHAR(MAX)
)
AS
BEGIN
		
	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
    DECLARE @v_type  VARCHAR(128)
	DECLARE @v_objectPkey VARCHAR(MAX) = @sCODE

    -- MATERIALIZATION: Insert materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_BASE_ASSESSMENT$MergeValue @sCODE, @dcID, @dcARRAY_INDEX, @sVALUE, @sREF_SET, 3
        RETURN
    END

    EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetStorageInfoFromAttribute @dcID,  @v_table OUTPUT, @v_field OUTPUT, @v_type OUTPUT
        
    IF @v_field IS NOT NULL
		-- Master Table
        EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable  @sCODE, @sVALUE, @v_type, @v_field
    ELSE
	BEGIN
        IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
		BEGIN
            -- Specific Table
            IF @sVALUE IS NOT NULL
                INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
                    (CODE, ID, COUNTRY, [LANGUAGE], ARRAY_INDEX, VALUE)
                VALUES
                    (@sCODE, @dcID, 'NNN', 'nn', @dcARRAY_INDEX, @sVALUE)
		END
        ELSE
			-- Generic Tables
            EXEC DX_PK_NEWATTRIBUTESUTILITIES$BulkInsertRecordOnStdTables @v_objectPkey, @dcID, @dcARRAY_INDEX, @sVALUE,  @v_table
    END
END
GO
    
-- Inserts a new value to the database.
-- It can be called from a SQL script as it accepts minimal information rather than the InsertRecord procedure
-- which has been built to be called from the application.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$InsertValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertValue
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$InsertValue
(
	@sCODE			 VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcARRAY_INDEX   BIGINT,
    @sVALUE          NVARCHAR(MAX)
)
AS
BEGIN
		
	DECLARE @v_attributeId BIGINT
	DECLARE @v_refSet	  VARCHAR(MAX)
	DECLARE @v_objectPkey VARCHAR(MAX) = @sCODE

	SELECT @v_attributeId = A.ID, 
			@v_refSet =(CASE WHEN B.ID IS NOT NULL THEN CONVERT(VARCHAR(max),B.ID) + ':' + C.NAME ELSE NULL END)
	FROM DX_ATTRIBUTE_DEF A
		LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
		LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
	WHERE A.NAME = @sNAME
	AND A.SCOPE = 'ASSESSMENT'
			
	EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$InsertRecord @sCODE, @sNAME,  @v_attributeId, @dcARRAY_INDEX, @sVALUE, @v_refSet
END
GO


IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateRecord') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateRecord
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateRecord
(
    @sCODE			 VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcID            BIGINT,
    @dcARRAY_INDEX   BIGINT,
    @sVALUE          NVARCHAR(MAX),
    @sREF_SET        VARCHAR(MAX)
)
AS
BEGIN

	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
    DECLARE @v_type  VARCHAR(128)
    DECLARE @v_objectPkey varchar(max) = @sCODE

    DECLARE @attValueCount INT = 0
    
	-- Retrieve information about attribute storage
	EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetStorageInfoFromAttribute @dcID, @v_table OUTPUT, @v_field OUTPUT, @v_type OUTPUT
        
    -- MEMO: handle the memo values first
    IF @v_table = 'DX_ATTVALUE_MEMO'
	BEGIN
		EXEC DX_PK_NEWATTVALUEMEMO$UpdateRecord @v_objectPkey, @sNAME,  @dcID, @dcARRAY_INDEX, @sVALUE, @sREF_SET
        RETURN
    END
        
    -- MATERIALIZATION: Update materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_BASE_ASSESSMENT$MergeValue @sCODE, @dcID, @dcARRAY_INDEX, @sVALUE, @sREF_SET, 1
        RETURN
    END

    IF @v_field IS NOT NULL
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable @sCODE, @sVALUE, @v_type, @v_field
    ELSE
	BEGIN
        IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
		BEGIN
			-- Specific Table
			IF EXISTS (
				SELECT NULL
				FROM DXDIR_ATTVALUE_ASSESSMENT 
				WHERE CODE = @sCODE 
				AND ID = @dcID
				AND COUNTRY = 'NNN' 
				AND [LANGUAGE] = 'nn' 
				AND ARRAY_INDEX = @dcARRAY_INDEX
			)
				SET @attValueCount = 1
                
            IF  @attValueCount<> 0
			BEGIN 
                IF @sVALUE IS NOT NULL
                    UPDATE DXDIR_ATTVALUE_ASSESSMENT 
					SET VALUE = @sVALUE 
					WHERE CODE = @sCODE 
					AND ID = @dcID AND COUNTRY = 'NNN' 
					AND [LANGUAGE] = 'nn' 
					AND ARRAY_INDEX = @dcARRAY_INDEX
                ELSE
				BEGIN
					DECLARE @bUpdateIndexes BIT = ( CASE WHEN @sREF_SET IS NULL THEN 1 ELSE 0 END)
					EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAndUpdateIndexes @sCODE, @dcID, @dcARRAY_INDEX, @bUpdateIndexes
                END
			END
            ELSE IF @sVALUE IS NOT NULL
				INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
					(CODE, ID, COUNTRY, [LANGUAGE], ARRAY_INDEX, VALUE)
				VALUES
					(@sCODE, @dcID, 'NNN', 'nn', @dcARRAY_INDEX, @sVALUE)
		END
        ELSE
			EXEC DX_PK_NEWATTRIBUTESUTILITIES$UpdateRecordOnStandarTables @v_objectPkey, @dcID, @dcARRAY_INDEX, @sVALUE, @sREF_SET, @v_table
    END
        
END
GO

  
-- Updates the given value to the database.
-- It can be called from a SQL script as it accepts minimal information rather than the UpdateRecord procedure
-- which has been built to be called from the application.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateValue
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateValue
(
	@sCODE			 VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcARRAY_INDEX   BIGINT,
    @sVALUE          NVARCHAR(MAX)
)
AS
BEGIN

	DECLARE @v_attributeId BIGINT
	DECLARE @v_refSet	   VARCHAR(MAX)
	DECLARE @v_objectPkey varchar(max) = @sCODE

	SELECT 
		@v_attributeId = A.ID, 
		@v_refSet = (CASE WHEN B.ID IS NOT NULL THEN CONVERT(VARCHAR(max),B.ID) + ':' + C.NAME ELSE NULL END)
	FROM DX_ATTRIBUTE_DEF A
		LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
		LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
	WHERE A.NAME = @sNAME
	AND A.SCOPE = 'ASSESSMENT';
			
	EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateRecord @sCODE, @sNAME, @v_attributeId, @dcARRAY_INDEX, @sVALUE, @v_refSet
END
GO


IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteRecord') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteRecord
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteRecord
(
    @sCODE			 VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcID            BIGINT,
    @dcARRAY_INDEX   BIGINT,
    @sREF_SET        VARCHAR(MAX)
)
AS
BEGIN
		
	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
    DECLARE @v_type  VARCHAR(128)
	DECLARE @v_objectPkey varchar(max) = @sCODE
	
	-- Retrieve information about attribute storage
	EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetStorageInfoFromAttribute @dcID, @v_table OUTPUT, @v_field OUTPUT, @v_type OUTPUT
        
    -- MEMO: handle the memo values first
    IF @v_table = 'DX_ATTVALUE_MEMO'
	BEGIN
		EXEC DX_PK_NEWATTVALUEMEMO$DeleteRecord @v_objectPkey, @sNAME,  @dcID, @dcARRAY_INDEX, @sREF_SET       
        RETURN
    END
		
    -- MATERIALIZATION: Delete materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_BASE_ASSESSMENT$MergeValue @sCODE, @dcID, @dcARRAY_INDEX, NULL, @sREF_SET, 2
        RETURN
    END      
        
	IF @v_field IS NOT NULL
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable @sCODE, NULL, @v_type, @v_field
	ELSE
	BEGIN
		IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
			EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAndUpdateIndexes  @sCODE, @dcID, @dcARRAY_INDEX
		ELSE
			EXEC DX_PK_NEWATTRIBUTESUTILITIES$DeleteRecordOnStandarTables @v_objectPkey, @dcID, @dcARRAY_INDEX, @v_table
	END
        
END
GO

    
-- Deletes a value from the database.
-- It can be called from a SQL script as it accepts minimal information rather than the DeleteRecord procedure
-- which has been built to be called from the application.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteValue
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteValue
(
	@sCODE			 VARCHAR(32),
    @sNAME           VARCHAR(MAX),
    @dcARRAY_INDEX   BIGINT
)
AS
BEGIN
		
	DECLARE @v_attributeId BIGINT
	DECLARE @v_refSet	   VARCHAR(MAX)
	DECLARE @v_objectPkey varchar(max) = @sCODE

	SELECT @v_attributeId = A.ID, 
			@v_refSet = (CASE WHEN B.ID IS NOT NULL THEN CONVERT(VARCHAR(max),B.ID) + ':' + C.NAME ELSE NULL END)
	FROM DX_ATTRIBUTE_DEF A
		LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
		LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
	WHERE A.NAME = @sNAME
		AND A.SCOPE = 'ASSESSMENT'
			
	EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteRecord  @sCODE, @sNAME, @v_attributeId, @dcARRAY_INDEX, @v_refSet
END
GO

    
-- Delete all values for a specific attribute (set, set-member, simple, multi-value)
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeletePersistenceData') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeletePersistenceData
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeletePersistenceData
(
    @sCODE		VARCHAR(32),
    @dcID		BIGINT
)
AS
BEGIN

	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)

    DECLARE @v_objectPkey varchar(max) = @sCODE
	DECLARE @v_MemberDefValues		DXDIR_PK_NEWATTVALUEASSESSMENT$MemberDefTableType
    
	-- MATERIALIZATION: Delete materialized attribute first, if it is the case
	IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_BASE_ASSESSMENT$DeletePersistenceData @sCODE, @dcID
		RETURN
	END
       
	INSERT @v_MemberDefValues 
	SELECT ID, TYPE, STORAGE
	FROM 
	(
		SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B WHERE A.ID=B.MEMBER_ID AND B.ID=@dcID
		UNION
		SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A WHERE A.ID=@dcID AND A.TYPE <> 'SET'
	) AS T

	DECLARE @i INT = 1, @cnt INT
	DECLARE @v_MemberDefRecordId BIGINT
	DECLARE @v_MemberDefRecordStorage VARCHAR(MAX)
	DECLARE @v_MemberDefRecordType	   VARCHAR(MAX)

		
	SELECT @cnt = MAX(ROWNUM) FROM @v_MemberDefValues

	WHILE (@i<=@cnt)
	BEGIN
		
		SELECT 
			@v_MemberDefRecordId = [ID],
			@v_MemberDefRecordStorage = [STORAGE],
			@v_MemberDefRecordType = [TYPE]
		FROM @v_MemberDefValues 
		WHERE ROWNUM = @i

		EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetTableAndFieldFromStorage @v_MemberDefRecordStorage, @v_table OUTPUT, @v_field OUTPUT
			
		IF @v_field IS NOT NULL
			EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$UpdateMasterTable  @sCODE, NULL, @v_MemberDefRecordType, @v_field
		ELSE
		BEGIN
			IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
				DELETE FROM DXDIR_ATTVALUE_ASSESSMENT WHERE CODE = @sCODE AND ID = @v_MemberDefRecordId
			ELSE
				EXEC DX_PK_NEWATTRIBUTESUTILITIES$DeletePersistenceData @v_objectPkey, @v_MemberDefRecordId, @v_table
		END

		set @i = @i + 1
	END
END
GO



-- Delete all values for a specific container.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAttrValuesByContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAttrValuesByContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAttrValuesByContainer
(
    @sCODE			 VARCHAR(32),
	@sAttributeNames DX_Varchar2Table readonly
)
AS
BEGIN
		
	DECLARE @v_materializedAttributesIds DX_NumberTable
	DECLARE @v_specificAttributesIds	 DX_NumberTable
	DECLARE @v_genericAttributesIds		 DX_NumberTable
	DECLARE @v_memoAttributesIds		 DX_NumberTable
	DECLARE @v_masterAttributesIds	     DX_Varchar2Table
	DECLARE @attributeInfoTable			 DX_AttributeInfoTable
	
	
	DECLARE @materializedFlagPos BIGINT = dbo.DX_PK_ATTRIBUTEDEF$GetFlag('MATERIALIZED_FLAG_POS')
	DECLARE @v_objectPkey varchar(max) = @sCODE
	
	DECLARE @v_attrArrayEmpty BIT = 0
	SET @v_attrArrayEmpty = dbo.DX_PK_UTILITY$IsEmptyVarcharTable(@sAttributeNames)

	-- Check specific attributes
	IF @v_attrArrayEmpty=0
	BEGIN

		INSERT @attributeInfoTable ( [ID], [NAME], [STORAGE], [SET_ID], [SET_NAME], [REF_SET] )
		SELECT [ID], [NAME], [STORAGE], [SET_ID], [SET_NAME], [REF_SET] 
		FROM dbo.DX_PK_NEWATTRIBUTESUTILITIES$NamesToAttributeInfoTable(@sAttributeNames, 'ASSESSMENT')

	END
	
	
	-- Insert attributes IDs into dedicated containers
	INSERT @v_materializedAttributesIds (CONTENT)
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT'
	AND STORAGE NOT LIKE 'DXDIR_ASSESSMENT.%'
	AND (FLAGS &  POWER(2, @materializedFlagPos)) <> 0
	AND ( 
		@v_attrArrayEmpty = 1
		OR
		NAME IN ( SELECT NAME FROM @attributeInfoTable )
	)
	

	INSERT @v_specificAttributesIds (CONTENT)
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT'
	AND STORAGE = 'DXDIR_ATTVALUE_ASSESSMENT'
	AND ( 
		@v_attrArrayEmpty = 1
		OR
		NAME IN ( SELECT NAME FROM @attributeInfoTable )
	)

	INSERT @v_genericAttributesIds (CONTENT)
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT' 
	AND STORAGE = 'DX_ATTVALUE'
	AND ( 
		@v_attrArrayEmpty = 1
		OR
		NAME IN ( SELECT NAME FROM @attributeInfoTable )
	)

	INSERT @v_memoAttributesIds (CONTENT)
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT'
	AND STORAGE = 'DX_ATTVALUE_MEMO'
	AND ( 
		@v_attrArrayEmpty = 1
		OR
		NAME IN ( SELECT NAME FROM @attributeInfoTable )
	)

	-- Delete master table attributes only if a list of attributes is provided
	INSERT @v_masterAttributesIds (CONTENT)
	SELECT ID
	FROM DX_ATTRIBUTE_DEF DEF
	WHERE SCOPE = 'ASSESSMENT'
	AND DEF.STORAGE LIKE 'DXDIR_ASSESSMENT.%'
	AND NAME IN ( SELECT NAME FROM @attributeInfoTable )
	


	DECLARE @i INT = 1, @cnt INT
	DECLARE @v_elementId BIGINT

	-- Process materialized attributes
	SELECT 
		@i = MIN(ROWNUM),
		@cnt = MAX(ROWNUM)
	FROM @v_materializedAttributesIds

	WHILE (@i<=@cnt)
	BEGIN

		SELECT @v_elementId = CONTENT
		FROM @v_materializedAttributesIds
		WHERE ROWNUM = @i

		EXEC dbo.DXDIR_PK_BASE_ASSESSMENT$DeletePersistenceData @sCODE, @v_elementId

		SET @i = @i +1

	END

	-- Process specific attributes
	IF EXISTS ( SELECT NULL FROM @v_specificAttributesIds )
	BEGIN
		DELETE DXDIR_ATTVALUE_ASSESSMENT 
		WHERE CODE = @sCODE 
			AND ID IN (SELECT CONTENT FROM @v_specificAttributesIds)
	END
		
	-- Process generic attributes
	IF EXISTS ( SELECT NULL FROM @v_genericAttributesIds )
		EXEC DX_PK_NEWATTRIBUTESUTILITIES$DeletePersistenceDataArray @v_objectPkey, @v_genericAttributesIds, 'DX_ATTVALUE'
		
	-- Process memo attributes
	IF EXISTS ( SELECT NULL FROM @v_memoAttributesIds )
		EXEC DX_PK_NEWATTRIBUTESUTILITIES$DeletePersistenceDataArray  @v_objectPkey, @v_memoAttributesIds, 'DX_ATTVALUE_MEMO'
		
	-- Process master table attributes
	IF EXISTS ( SELECT NULL FROM @v_masterAttributesIds)
	BEGIN
		
		SELECT 
			@i = MIN(ROWNUM),
			@cnt = MAX(ROWNUM)
		FROM @v_masterAttributesIds

		WHILE (@i<=@cnt)
		BEGIN

			SELECT @v_elementId = CONTENT
			FROM @v_masterAttributesIds
			WHERE ROWNUM = @i

			EXEC dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$DeletePersistenceData @sCODE, @v_elementId

			SET @i = @i +1

		END

	END
	
END
GO


-- Delete all values for a specific container.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteByContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteByContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteByContainer
(
    @sCODE		VARCHAR(32)
)
AS
BEGIN
	
	DECLARE @v_attributeNames DX_Varchar2Table

	-- Delete all attributes by passing an empty attribute names array
	EXEC dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAttrValuesByContainer @sCODE, @v_attributeNames

END
GO
	
-- Returns 1 when any value exists for the given container and attribute ID, otherwise 0.
-- It can be called within a SQL statement.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsAnyValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsAnyValue
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsAnyValue
(
	@sCODE		VARCHAR(32),
	@dcID		BIGINT,
	@v_exists	BIT OUTPUT
)
AS
BEGIN
		
	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
	DECLARE @v_MemberDefValues DXDIR_PK_NEWATTVALUEASSESSMENT$MemberDefTableType
	DECLARE @v_objectPkey varchar(max) = @sCODE

	SET @v_exists = 0

	-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
	IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsAnyValue @sCODE, @dcID, @v_exists OUTPUT
		RETURN 
	END
        
	INSERT @v_MemberDefValues
	SELECT ID, TYPE, STORAGE
	FROM
	(
		SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B WHERE A.ID=B.MEMBER_ID AND B.ID=@dcID
		UNION
		SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A WHERE A.ID=@dcID AND A.TYPE <> 'SET'
	) AS T


	DECLARE @i INT = 1, @cnt INT
	DECLARE @v_MemberDefRecordId	  BIGINT
	DECLARE @v_MemberDefRecordStorage VARCHAR(MAX)
	DECLARE @v_MemberDefRecordType	  VARCHAR(MAX)

	SELECT @cnt = MAX(ROWNUM) FROM @v_MemberDefValues

	WHILE (@i<=@cnt)
	BEGIN
		
		SELECT @v_MemberDefRecordId = ID FROM @v_MemberDefValues WHERE ROWNUM = @i
			
		SELECT @v_MemberDefRecordStorage = [STORAGE],
				@v_MemberDefRecordType = [TYPE]
		FROM @v_MemberDefValues 
		WHERE ID = @v_MemberDefRecordId

		EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetTableAndFieldFromStorage @v_MemberDefRecordStorage, @v_table OUTPUT, @v_field OUTPUT

		IF @v_field IS NOT NULL
			EXEC dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsOnMasterTable @sCODE, @v_field, @v_exists OUTPUT
		ELSE
		BEGIN
			IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
				SELECT @v_exists = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END) 
				FROM DXDIR_ATTVALUE_ASSESSMENT 
				WHERE CODE = @sCODE 
				AND ID = @v_MemberDefRecordId
			ELSE
				SET @v_exists = dbo.DX_PK_NEWATTRIBUTESUTILITIES$ExistsOnStandardTables( @v_objectPkey, @v_MemberDefRecordId, @v_table )
		END
			
		IF @v_exists <> 0
			-- Exit from the actual loop as we found at least a value.
			BREAK

		SET @i = @i +1

	END
		
END
GO
    
-- Returns 1 when any values exist for the given container and attribute ID, otherwise 0.
-- It may be called by the application, so result is stored as output parameter.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$Exists') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$Exists
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$Exists
(
	@sCODE			VARCHAR(32),
	@dcID			BIGINT,
	@dcExists		BIT OUTPUT
)
AS
BEGIN
		
	EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsAnyValue @sCODE, @dcID, @dcExists OUTPUT

END
GO
    

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValue
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValue
(
	@sCODE			VARCHAR(32),
	@dcID			BIGINT,
	@sVALUE			NVARCHAR(MAX),
	@v_exists		BIT OUTPUT
)
AS
BEGIN

	DECLARE @v_type VARCHAR(MAX)
	DECLARE @v_storage VARCHAR(MAX)

	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
        
	DECLARE @v_objectPkey varchar(max) = @sCODE

	SET @v_exists = 0

	-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
	IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
		SET @v_exists = dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValue( @sCODE, @dcID, @sVALUE )
	ELSE
	BEGIN

		SELECT @v_type = TYPE, @v_storage = STORAGE FROM DX_ATTRIBUTE_DEF WHERE ID = @dcID

		EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetTableAndFieldFromStorage @v_storage, @v_table OUTPUT, @v_field OUTPUT
	
		IF @v_field IS NOT NULL
			EXEC dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTable @sCODE, @v_field, @v_type, @sVALUE, @v_exists OUTPUT
		ELSE
		BEGIN
			IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
				SELECT @v_exists = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END) 
				FROM DXDIR_ATTVALUE_ASSESSMENT 
				WHERE CODE = @sCODE 
				AND ID = @dcID 
				AND VALUE = @sVALUE
			ELSE
				SET @v_exists = dbo.DX_PK_NEWATTRIBUTESUTILITIES$ExistsValueOnStandardTables( @v_objectPkey, @dcID, @sVALUE, @v_table )
		END
	END
		
END
GO
    
-- If the given attribute value is found in any containers, return 1, otherwise 0.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueForAnyContainers') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueForAnyContainers
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueForAnyContainers 
(
	@dcID			BIGINT,
	@sVALUE			NVARCHAR(MAX),
	@v_exists		BIT OUTPUT
)
AS
BEGIN

	DECLARE @v_type VARCHAR(MAX)
	DECLARE @v_storage VARCHAR(MAX)

	DECLARE @v_table VARCHAR(128)
    DECLARE @v_field VARCHAR(128)
        
	SET @v_exists = 0 

	-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
	IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
		SET @v_exists = dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValueForAnyContainers(@dcID, @sVALUE)
	ELSE
	BEGIN
		SELECT @v_type = [TYPE], @v_storage = [STORAGE] FROM DX_ATTRIBUTE_DEF WHERE ID = @dcID

		EXEC DX_PK_NEWATTRIBUTESUTILITIES$GetTableAndFieldFromStorage @v_storage, @v_table OUTPUT, @v_field OUTPUT
				
		IF @v_field IS NOT NULL
			EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$ExistsValueOnMasterTableAnyCnt @v_field, @v_type, @sVALUE, @v_exists OUTPUT
		ELSE
		BEGIN
			IF @v_table = 'DXDIR_ATTVALUE_ASSESSMENT'
				SELECT @v_exists = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END) 
				FROM DXDIR_ATTVALUE_ASSESSMENT 
				WHERE ID = @dcID 
				AND VALUE = @sVALUE
			ELSE
				SET @v_exists = dbo.DX_PK_NEWATTRIBUTESUTILITIES$ExistsValForAnyContOnStdTables(@dcID, @sVALUE, @v_table)
		END
	END
		
END
GO

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
RETURNS NVARCHAR(MAX)
BEGIN
		
	declare @stm	  NVARCHAR(max)

    SET @stm = N'
        SELECT 
			A.CODE OBJECT_PK,
			A.CODE,
			C.NAME, A.ID, A.ARRAY_INDEX, A.VALUE, C.REF_SET
			FROM DXDIR_ATTVALUE_ASSESSMENT A,
            tPKEY B,
			tDEF C
			WHERE
		    A.[CODE] = B.[CODE]
			AND A.ID = C.ID

			UNION ALL

			SELECT 
			A.OBJECT_PK,
			B.CODE AS CODE,
			C.NAME, A.ID, A.ARRAY_INDEX, A.VALUE, C.REF_SET
			FROM DX_ATTVALUE A,
            tPKEY B,
            tDEF C
            WHERE A.OBJECT_PK = B.CODE
			AND A.ID = C.ID

			UNION ALL 
			 
			'
			+  dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetMemoSelectStm('TRUE') + '
			
			UNION ALL
			 
			'
			+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetContainerSelectStm()
            

    RETURN @stm
		
END
GO
    
	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetContainerSelectStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetContainerSelectStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetContainerSelectStm()
RETURNS NVARCHAR(MAX)
BEGIN

    declare @stm	  NVARCHAR(max)

    SET @stm = N'
            SELECT
            A.CODE OBJECT_PK,
            A.CODE,
            C.NAME, C.ID, 0 ARRAY_INDEX,
            CASE
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.CODE'' THEN CONVERT( NVARCHAR(max), A.[CODE])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.TYPE_CODE'' THEN CONVERT( NVARCHAR(max), A.[TYPE_CODE])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.DESCRIPTION'' THEN A.[DESCRIPTION]
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.STATUS'' THEN CONVERT( NVARCHAR(max), A.[STATUS])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.COMPANY_NAME'' THEN CONVERT( NVARCHAR(max), A.[COMPANY_NAME])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.COMPLETING_BY'' THEN CONVERT( NVARCHAR(max), A.[COMPLETING_BY])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.PHONE'' THEN CONVERT( NVARCHAR(max), A.[PHONE])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.EMAIL'' THEN CONVERT( NVARCHAR(max), A.[EMAIL])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.TIMEFRAME_FROM'' THEN dbo.DX_PK_UTILITY_SQLSRV$to_char_date(A.[TIMEFRAME_FROM], ''YYYYMMDDHH24MISS'')
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.TIMEFRAME_TO'' THEN dbo.DX_PK_UTILITY_SQLSRV$to_char_date(A.[TIMEFRAME_TO], ''YYYYMMDDHH24MISS'')
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.CREATE_DATE'' THEN dbo.DX_PK_UTILITY_SQLSRV$to_char_date(A.[CREATE_DATE], ''YYYYMMDDHH24MISS'')
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.CREATED_BY'' THEN CONVERT( NVARCHAR(max), A.[CREATED_BY])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.MOD_DATE'' THEN dbo.DX_PK_UTILITY_SQLSRV$to_char_date(A.[MOD_DATE], ''YYYYMMDDHH24MISS'')
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.MODIFIED_BY'' THEN CONVERT( NVARCHAR(max), A.[MODIFIED_BY])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.AUTHORIZATION_ROLE'' THEN CONVERT( NVARCHAR(max), A.[AUTHORIZATION_ROLE])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.ORG_STRUCTURE'' THEN CONVERT( NVARCHAR(max), A.[ORG_STRUCTURE])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.LOCATION'' THEN CONVERT( NVARCHAR(max), A.[LOCATION])
			WHEN C.STORAGE=''DXDIR_ASSESSMENT.PROD_CLASSIF'' THEN CONVERT( NVARCHAR(max), A.[PROD_CLASSIF])
			ELSE NULL
		    END VALUE,
            C.REF_SET
            FROM DXDIR_ASSESSMENT A,
            tPKEY B,
			tDEF C
            WHERE
            A.CODE = B.CODE
            AND C.STORAGE LIKE ''DXDIR_ASSESSMENT.%''
        '
                    
    RETURN @stm
    
END
GO
    


    

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetMemoSelectStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetMemoSelectStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetMemoSelectStm
(
    @usePlaceHolderValue BIT
)
RETURNS NVARCHAR(MAX)
BEGIN

	DECLARE @stm	  NVARCHAR(MAX)
	DECLARE @stmValue NVARCHAR(MAX)

	IF(@usePlaceHolderValue = 1)
		set @stmValue = N'CASE WHEN (LEN(A.VALUE)<=2000) THEN A.VALUE ELSE N''Unloaded Memo Value'' COLLATE Latin1_General_CS_AS END AS VALUE'
	ELSE
		set @stmValue = N'A.VALUE'

	set @stm = N'
		SELECT
			A.OBJECT_PK,
			B.CODE AS CODE,
			C.NAME, 
			A.ID, 
			A.ARRAY_INDEX,
			' + @stmValue +',
			C.REF_SET
			FROM DX_ATTVALUE_MEMO A,
			tPKEY B,
			tDEF C
			WHERE A.OBJECT_PK = B.CODE
			AND A.ID = C.ID'
    
	RETURN @stm

END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValueByAttributeStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValueByAttributeStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValueByAttributeStm()
RETURNS NVARCHAR(MAX)
BEGIN

	DECLARE @stm NVARCHAR(MAX)
	SET @stm = N'
            WITH 
			tPKEY AS (SELECT @pCODE CODE),
			tDEF  AS ( 
			SELECT A.ID, 
					A.NAME,
					A.STORAGE, 
					C.ID SET_ID,
					C.NAME SET_NAME, 
					(CASE WHEN C.ID IS NOT NULL THEN CONVERT(VARCHAR(max),C.ID) + '':'' + C.NAME ELSE NULL END) REF_SET
					FROM DX_ATTRIBUTE_DEF A
						LEFT JOIN DX_ATTRIBUTE_SET B
							ON A.ID = B.MEMBER_ID
						LEFT JOIN DX_ATTRIBUTE_DEF C
							ON B.ID = C.ID
					WHERE A.SCOPE = ''ASSESSMENT''
					AND A.ID = @dcId
		)
		'
        +  dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetMemoSelectStm('FALSE')
        + ' AND A.ARRAY_INDEX = @dcARRAY_INDEX'
            
    RETURN @stm

END
GO
	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByAttributeStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByAttributeStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByAttributeStm()
RETURNS NVARCHAR(MAX)
BEGIN

	DECLARE @stm NVARCHAR(MAX)

	SET @stm = N'
        WITH
			tPKEY AS (SELECT @pCODE CODE),
			tDEF AS (SELECT ID, NAME, STORAGE, NULL SET_ID, NULL SET_NAME, NULL REF_SET FROM DX_ATTRIBUTE_DEF WHERE ID = @dcID)'
		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()

	RETURN @stm

END
GO

   
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesBySetStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesBySetStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesBySetStm()
RETURNS NVARCHAR(MAX)
BEGIN

	DECLARE @stm NVARCHAR(MAX)

	SET @stm = N'
        WITH 
				tPKEY AS (SELECT @pCODE CODE),
				tDEF AS (
				SELECT A.ID, A.NAME, 
						A.STORAGE, 
						C.ID SET_ID, 
						C.NAME SET_NAME, 
						(CASE WHEN C.ID IS NOT NULL THEN CONVERT(VARCHAR(max),C.ID) + '':'' + C.NAME ELSE NULL END) REF_SET 
						FROM DX_ATTRIBUTE_DEF A
							INNER JOIN DX_ATTRIBUTE_SET B
								ON A.ID = B.MEMBER_ID 
							INNER JOIN DX_ATTRIBUTE_DEF C 
								ON B.ID = C.ID 
						WHERE A.SCOPE = ''ASSESSMENT''
						AND C.ID = @dcID )'
			
		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
		;
		
	return @stm;

END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByContainerStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByContainerStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByContainerStm
(
    @include_materialized BIT = 'TRUE'
)
RETURNS NVARCHAR(MAX)
BEGIN
		
	DECLARE @stm                 NVARCHAR(MAX)
    DECLARE @stm_materialized    NVARCHAR(MAX)

	SET @stm = N'
        WITH 
			tPKEY AS (SELECT @pCODE CODE),
			tDEF AS ( SELECT A.ID, 
								A.NAME, 
								A.STORAGE, 
								C.ID SET_ID, 
								C.NAME SET_NAME, 
								(CASE WHEN C.ID IS NOT NULL THEN CONVERT(VARCHAR(max),C.ID) + '':'' + C.NAME ELSE NULL END) REF_SET 
								FROM DX_ATTRIBUTE_DEF A
								LEFT JOIN DX_ATTRIBUTE_SET B
									ON A.ID = B.MEMBER_ID
								LEFT JOIN DX_ATTRIBUTE_DEF C 
									ON B.ID = C.ID
								WHERE A.SCOPE = ''ASSESSMENT'' )'

		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
			
		
	IF @include_materialized = 'TRUE'
	BEGIN
			
		IF EXISTS ( SELECT NULL FROM SYSOBJECTS WHERE NAME = 'DXDIR_PK_BASE_ASSESSMENT$GetStmForContainer')
			SET @stm_materialized = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForContainer()

		IF @stm_materialized IS NOT NULL
			SET @stm = @stm + ' UNION ALL ' + @stm_materialized
	END
		
	return @stm
END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByNamesStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByNamesStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByNamesStm
(
	@sNAMES  DX_Varchar2Table readonly
)
RETURNS NVARCHAR(MAX)
BEGIN

	DECLARE @stm                 NVARCHAR(MAX)
    DECLARE @stm_materialized    NVARCHAR(MAX)

    SET @stm = N'
        WITH 
			tPKEY AS (SELECT @pCODE CODE),
			tDEF AS (SELECT ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM @attributeInfoTable )'
		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
          
	IF EXISTS ( SELECT NULL FROM SYSOBJECTS WHERE NAME = 'DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByNames')
		SET @stm_materialized = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByNames(@sNAMES)

    IF @stm_materialized IS NOT NULL
        set @stm = @stm + ' UNION ALL ' + @stm_materialized
		
	return @stm
		
END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersStm()
RETURNS NVARCHAR(MAX)
BEGIN
            
	DECLARE @stm				NVARCHAR(MAX)
    DECLARE @stm_materialized   NVARCHAR(MAX)

    SET @stm = N'
        WITH 
            tPKEY AS (SELECT CODE FROM @containerKeyTable /* DXDIR_ASSESSMENTKeyTable */ ),
            tDEF  AS (SELECT ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM @attributeInfoTable /* DX_AttributeInfoTable */ )'

		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
			
	IF EXISTS ( SELECT NULL FROM SYSOBJECTS WHERE NAME = 'DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers')
		SET @stm_materialized = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers()

    IF @stm_materialized IS NOT NULL 
        SET @stm = @stm + ' UNION ALL ' + @stm_materialized;
        
	return @stm

END
GO
    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersNamesStm') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersNamesStm
GO
CREATE FUNCTION dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersNamesStm
(
	@sNAMES		DX_Varchar2Table readonly
)
RETURNS NVARCHAR(MAX)
BEGIN

    DECLARE @stm				NVARCHAR(MAX)
    DECLARE @stm_materialized   NVARCHAR(MAX)

    SET @stm = N'
        WITH 
			tPKEY AS (SELECT CODE FROM @containerKeyTable /* DXDIR_ASSESSMENTKeyTable */ ),
			tDEF AS (SELECT ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM @attributeInfoTable /* DX_AttributeInfoTable */ )'
		+ dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetBaseSelectStm()
			
	IF EXISTS ( SELECT NULL FROM SYSOBJECTS WHERE NAME = 'DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers')
		SET @stm_materialized = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers()

    IF @stm_materialized IS NOT NULL
        set @stm = @stm + ' UNION ALL ' + @stm_materialized
        
	return @stm

END
GO
    

    
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectOne') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectOne
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectOne 
(
    @sCODE				VARCHAR(32),
    @dcID               BIGINT,
    @dcARRAY_INDEX		BIGINT,
	@debug				bit = 0
)
AS
BEGIN
		
	declare @sScope  VARCHAR(MAX)
    declare @dcSetID BIGINT

	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)
		
    -- MATERIALIZATION: Read materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
			
		EXEC DXM_PK_BASE$GetInfo @dcID, @sScope, @dcSetID OUTPUT
            
		set @sql = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForCell(@dcSetID, @dcID, DEFAULT)

		set @paramDefinition = N'@pCODE	VARCHAR(32), @pATT_ID BIGINT, @pARRAY_INDEX BIGINT'
		
		if (@debug<>0)
			print CAST(@sql AS NTEXT)
		else
			execute sp_executesql @sql, @paramDefinition,
							@pCODE = @sCODE,
							@pATT_ID = @dcID,
							@pARRAY_INDEX = @dcARRAY_INDEX
        RETURN
    END
        
    set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValueByAttributeStm()
	set @paramDefinition = N'@pCODE	VARCHAR(32), @dcID BIGINT, @dcARRAY_INDEX BIGINT'
		
	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition,
						@pCODE = @sCODE,
						@dcID = @dcID,
						@dcARRAY_INDEX = @dcARRAY_INDEX
      
END
GO
    

IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForAttribute') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForAttribute
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForAttribute 
(
    @sCODE				VARCHAR(32),
    @dcID               BIGINT,
	@debug				bit = 0
)
AS
BEGIN
       	
	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)

    -- MATERIALIZATION: Read materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
			
		set @sql = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForAttribute(@dcID)
        set @paramDefinition = N'@pCODE	VARCHAR(32), @pMemberId BIGINT'
		
		if (@debug<>0)
			print CAST(@sql AS NTEXT)
		else
			execute sp_executesql @sql, @paramDefinition,
							@pCODE = @sCODE,
							@pMemberId = @dcID

        RETURN
    END
        	
	set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByAttributeStm()
	set @paramDefinition = N'@pCODE	VARCHAR(32), @dcID BIGINT'
		
	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition,
							  @pCODE = @sCODE,
							  @dcID = @dcID
END
GO
    
	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForSet') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForSet
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForSet 
(
    @sCODE	VARCHAR(32),
    @dcID	BIGINT,
    @debug	bit = 0
)
AS
BEGIN
		
	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)

    -- MATERIALIZATION: Read materialized attribute first, if it is the case
    IF dbo.DX_PK_ATTRIBUTEDEF$IsMaterialized(@dcID) = 1
	BEGIN
			
		set @sql = dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForSet(@dcID)
        set @paramDefinition = N'@pCODE	VARCHAR(32), @pID BIGINT'
		
		if (@debug<>0)
			print CAST(@sql AS NTEXT)
		else
			execute sp_executesql @sql, @paramDefinition,
							@pCODE = @sCODE,
							@pID = @dcID

        RETURN
    END
		
	set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesBySetStm()
	set @paramDefinition = N'@pCODE	VARCHAR(32), @dcID BIGINT'
		
	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition,
							  @pCODE = @sCODE,
							  @dcID = @dcID
END
GO
    
-- Read all values of all attributes of the given container
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForContainer 
(
    @sCODE	VARCHAR(32),
	@debug	BIT = 0
)
AS
BEGIN
		
	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)

	set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByContainerStm('TRUE')
	set @paramDefinition = N'@pCODE	VARCHAR(32)'

	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition, @pCODE = @sCODE

END
GO

    
-- Read all non materialized attributes values of the given container
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectNonMtzdValForContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectNonMtzdValForContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectNonMtzdValForContainer 
    (
    @sCODE	VARCHAR(32),
    @debug	BIT = 0
)
AS
BEGIN

	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)

	set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetValuesByContainerStm('FALSE')
	set @paramDefinition = N'@pCODE	VARCHAR(32)'

	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition, @pCODE = @sCODE
END
GO
    
	
-- Returns 1 when any values exist for the given container, otherwise 0.
-- Master table attributes are not considered.
-- It can be called within a SQL statement.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$ExistAnyValuesForContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistAnyValuesForContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$ExistAnyValuesForContainer 
(
	@sCODE		VARCHAR(32),
	@v_result   BIT OUTPUT
)
AS
BEGIN
		
	DECLARE @v_allValuesForContainerTable DXDIR_PK_NEWATTVALUEASSESSMENT$ElementType
	DECLARE @v_excludedAttributesIdsTable DX_NumberTable
	DECLARE @v_count INT

	DECLARE @v_objectPkey varchar(max) = @sCODE

	SET @v_result = 0

	-- Collect a list of attributes IDs mapped to master tables which need to be skipped.
	INSERT @v_excludedAttributesIdsTable
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT'
	AND STORAGE LIKE 'DXDIR_ASSESSMENT.%'

	INSERT @v_allValuesForContainerTable 
	EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForContainer @sCODE
		
	IF EXISTS (
		SELECT NULL
		FROM @v_allValuesForContainerTable
		WHERE ID NOT IN (
			SELECT CONTENT
			FROM @v_excludedAttributesIdsTable
		)
	)
			SET @v_result = 1
		
END
GO
   
	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByNames') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByNames
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByNames 
(
    @sCODE		VARCHAR(32),
    @sNAMES		DX_Varchar2Table readonly,
	@debug		bit = 0
)
AS
BEGIN
		
	DECLARE @sql NVARCHAR(MAX)
	DECLARE @paramDefinition NVARCHAR(MAX)
    DECLARE @attributeInfoTable  DX_AttributeInfoTable
    
	insert @attributeInfoTable select * from dbo.DX_PK_NEWATTRIBUTESUTILITIES$NamesToAttributeInfoTable(@sNAMES,'ASSESSMENT')
        
	set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByNamesStm(@sNAMES)
	set @paramDefinition = N'@pCODE	VARCHAR(32), @attributeInfoTable  DX_AttributeInfoTable readonly'

	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition, @pCODE = @sCODE, @attributeInfoTable = @attributeInfoTable

END
GO
	

-- Read all non materialized attributes values of the given container
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByContainers') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByContainers
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByContainers 
(
    @sCODE		 DX_Varchar2Table readonly,
    @sNAMES		 DX_Varchar2Table readonly,
    @debug		 BIT = 0
)
AS
BEGIN
	declare @sql nvarchar(max)
	declare @paramDefinition nvarchar(max)

	declare @containerKeyTable   DXDIR_ASSESSMENTKeyTable
    declare @attributeInfoTable  DX_AttributeInfoTable

    insert @containerKeyTable select * from dbo.DX_PK_Utility$ASSESSMENTKeyArraysToTable(@sCODE)

	if not exists ( SELECT NULL from @sNAMES )
    begin
		-- No names provided
		set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersStm()
		insert @attributeInfoTable select * from dbo.DX_PK_NEWATTRIBUTESUTILITIES$AllAttrsToAttributeInfoTable ('ASSESSMENT')
	end
	else
	begin
		-- Names provided
		set @sql = dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$GetItemsByContainersNamesStm(@sNAMES)
		insert @attributeInfoTable select * from dbo.DX_PK_NEWATTRIBUTESUTILITIES$NamesToAttributeInfoTable (@sNAMES,'ASSESSMENT')
	end

	set @paramDefinition = N'@containerKeyTable DXDIR_ASSESSMENTKeyTable readonly, @attributeInfoTable DX_AttributeInfoTable readonly'

	if (@debug<>0)
		print CAST(@sql AS NTEXT)
	else
		execute sp_executesql @sql, @paramDefinition, @containerKeyTable = @containerKeyTable, @attributeInfoTable = @attributeInfoTable
        
END
GO


	
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAttributesToContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAttributesToContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAttributesToContainer 
(
	@sFROMCODE		 VARCHAR(32),
	@sTOCODE		 VARCHAR(32),
	@sAttributeNames DX_Varchar2Table readonly,
	@dcCustomExcludedAttributesIds DX_NumberTable readonly
)
AS
BEGIN

	DECLARE @v_element					  DXDIR_PK_NEWATTVALUEASSESSMENT$ElementType
	DECLARE @v_memoElement				  DX_PK_NEWATTVALUEMEMO$ElementType
	DECLARE @v_excludedAttributesIdsTable DX_NumberTable
	DECLARE @v_memoAttributesIdsTable	  DX_NumberTable
	DECLARE @v_objectFromPkey varchar(max) = @sFROMCODE
	DECLARE @v_objectToPkey varchar(max) = @sTOCODE
	DECLARE @v_count INT
	DECLARE @v_arrayEmpty BIT = 0

	SET @v_arrayEmpty = dbo.DX_PK_UTILITY$IsEmptyVarcharTable(@sAttributeNames)
	

	IF @v_arrayEmpty = 0 
	BEGIN
		-- An attributes names list is present

		-- Delete existing data first.
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteAttrValuesByContainer @sTOCODE, @sAttributeNames

		INSERT @v_element 
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$SelectItemsByNames @sFROMCODE, @sAttributeNames
		
	END
	ELSE
	BEGIN

		-- DELETE existing data first.
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$DeleteByContainer @sTOCODE
	
		-- Collect a list of attributes IDs mapped to master tables which need to be skipped.
		INSERT @v_excludedAttributesIdsTable 
		SELECT ID
		FROM DX_ATTRIBUTE_DEF 
		WHERE SCOPE = 'ASSESSMENT'
		AND STORAGE LIKE 'DXDIR_ASSESSMENT.%'
		UNION SELECT CONTENT FROM @dcCustomExcludedAttributesIds

		-- Get all values from source container.
		INSERT @v_element 
		EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$SelectAllValuesForContainer @sFROMCODE

	END
			
	-- Collect a list of memo attributes IDs because they need to be processed one by one.
	INSERT @v_memoAttributesIdsTable
	SELECT ID
	FROM DX_ATTRIBUTE_DEF 
	WHERE SCOPE = 'ASSESSMENT'
	AND TYPE = 'MEMO'
			
	DECLARE @i INT = 1, @cnt INT
	DECLARE @v_elementId BIGINT

	DECLARE @v_elementValue NVARCHAR(MAX)
	DECLARE @v_elementName  VARCHAR(MAX)
	DECLARE @v_elementRefSet VARCHAR(MAX)
	DECLARE @v_elementArrayIndex	BIGINT

	SELECT @cnt = MAX(ROWNUM) FROM @v_element


	-- Insert values except memo attributes and master table ones (if not explicitly requested in the attribute name list)
	WHILE (@i<=@cnt)
	BEGIN

		-- Get values for current ID
		SELECT 
			@v_elementId = ID,
			@v_elementArrayIndex = ARRAY_INDEX,
			@v_elementName = NAME,
			@v_elementValue = VALUE,
			@v_elementRefSet = REF_SET
		FROM @v_element 
		WHERE ROWNUM = @i


		-- If attribute names list has not been provided, check whether the current attribute is in the list of excluded ones
		IF @v_arrayEmpty=0 OR NOT EXISTS (SELECT NULL FROM @v_excludedAttributesIdsTable WHERE content = @v_elementId) 
		BEGIN
			-- Check whether the current attribute is a Memo attribute, becuse memos must be treated one by one.
			IF EXISTS ( SELECT NULL FROM @v_memoAttributesIdsTable WHERE content = @v_elementId ) 
			BEGIN
				-- Process Memo attribute because NCLOB data type cannot be used with a UNION, so no data can be loaded
				-- massively.
				DELETE FROM @v_memoElement

				INSERT @v_memoElement
				EXEC DX_PK_NEWATTVALUEMEMO$SelectOne @v_objectFromPkey, @v_elementId, @v_elementArrayIndex
					
				IF EXISTS (SELECT NULL FROM @v_memoElement)
				BEGIN
					SELECT TOP 1
						@v_elementArrayIndex = ARRAY_INDEX,
						@v_elementName = NAME,
						@v_elementValue = VALUE,
						@v_elementRefSet = REF_SET
					FROM @v_memoElement

					EXEC DX_PK_NEWATTVALUEMEMO$BulkInsertRecord @v_objectToPkey, @v_elementName, @v_elementId, @v_elementArrayIndex, @v_elementValue, @v_elementRefSet
				END
			END
			ELSE
				EXEC DXDIR_PK_NEWATTVALUEASSESSMENT$BulkInsertRecord @sTOCODE, @v_elementName, @v_elementId, @v_elementArrayIndex, @v_elementValue, @v_elementRefSet
		END

		SET @i = @i + 1
	END

END
GO



IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAllValuesToContainer') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAllValuesToContainer
GO
CREATE PROCEDURE DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAllValuesToContainer 
(
	@sFROMCODE		VARCHAR(32),
	@sTOCODE		VARCHAR(32)
)
AS
BEGIN

	-- Copy all attributes by passing an empty attribute names array
	DECLARE @v_attributeNames DX_Varchar2Table
	EXEC dbo.DXDIR_PK_NEWATTVALUEASSESSMENT$CopyAttributesToContainer @sFROMCODE, @sTOCODE, @v_attributeNames

END
GO