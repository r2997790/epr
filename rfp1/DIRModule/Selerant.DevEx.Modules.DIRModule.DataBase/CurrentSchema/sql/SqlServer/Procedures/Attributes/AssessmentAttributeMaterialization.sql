
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$MergeValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$MergeValue
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$MergeValue 
(
	@pCODE				VARCHAR(32),
    @pATT_ID			BIGINT,
	@pATT_ARRAY_INDEX 	BIGINT,
	@pATT_VALUE 		NVARCHAR(MAX),
    @pATT_REF_SET       VARCHAR(MAX),
	@pMergeType		    TINYINT
)
AS
BEGIN
	
	RETURN
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$MergeMemoValue') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$MergeMemoValue
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$MergeMemoValue 
(
	@pCODE				VARCHAR(32),
    @pATT_ID			BIGINT,
	@pATT_ARRAY_INDEX 	BIGINT,
	@pATT_VALUE 		NVARCHAR(MAX),
    @pATT_REF_SET       VARCHAR(MAX)
)
AS
BEGIN
	
	RETURN	
	
END
GO
---$ END COMMAND
   
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$DeletePersistenceData') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$DeletePersistenceData
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$DeletePersistenceData 
(
	@pCODE		VARCHAR(32),
    @pATT_ID	BIGINT
)
AS
BEGIN
	RETURN
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForCell') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForCell
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForCell
(
    @pATT_ID      BIGINT,
    @pMEMBER_ID   BIGINT,
	@bIncludeScopePKeyColumns BIT = 1
)	
RETURNS NVARCHAR(MAX)
BEGIN
    
		RETURN NULL
END
GO
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForAttribute') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForAttribute
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForAttribute
(
    @pATT_ID	BIGINT
)	
RETURNS	NVARCHAR(MAX)
BEGIN   

		RETURN NULL
    

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForSet') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForSet
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForSet
(
    @pATT_ID	BIGINT
) 
RETURNS NVARCHAR(MAX)
BEGIN

		RETURN NULL


END
GO
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForContainer') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForContainer
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForContainer()
RETURNS NVARCHAR(MAX)
BEGIN
	

		RETURN NULL

END
GO
---$ END COMMAND
   
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByNames') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByNames
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByNames
(
    @sNAMES	DX_Varchar2Table readonly
)   
RETURNS NVARCHAR(MAX)
BEGIN
	
		RETURN NULL

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers
(
     @sNAMES	DX_Varchar2Table readonly
)   
RETURNS NVARCHAR(MAX)
BEGIN

		
	RETURN NULL

   
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetStmForItemsByContainers()
RETURNS NVARCHAR(MAX)
BEGIN
	
	
	RETURN NULL

END
GO
---$ END COMMAND
 
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId
(
    @pATT_SET_ID BIGINT
)
RETURNS VARCHAR(MAX)
BEGIN

    RETURN NULL

END
GO
---$ END COMMAND 
  
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$GetContainerConditionBySetId') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetContainerConditionBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetContainerConditionBySetId 
(
    @pATT_SET_ID BIGINT,
	@condition   NVARCHAR(MAX) OUTPUT
)   
AS
BEGIN
	DECLARE
		@stm             NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX),
		@packageName	 NVARCHAR(MAX)

	SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

	SET @stm = N'SELECT @condition = dbo.' + @packageName + N'$GetContainerCondition()'

    SET @paramDefinition = N'@condition NVARCHAR(MAX) OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @condition OUTPUT

END
GO
---$ END COMMAND
  
 
---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$GetTableNameBySetId') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetTableNameBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetTableNameBySetId 
    @pATT_SET_ID BIGINT,
	@tableName   NVARCHAR(MAX) OUTPUT
AS
BEGIN

	DECLARE 
		@stm		 NVARCHAR(MAX),
		@packageName NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX)
	
	SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

    SET @stm = N'SELECT @tableName = dbo.' + @packageName + N'$GetTableName()'

    SET @paramDefinition = N'@tableName NVARCHAR(MAX) OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @tableName OUTPUT

END
GO
---$ END COMMAND 
  

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$GetFirstMemberBySetId') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetFirstMemberBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetFirstMemberBySetId 
(
    @pATT_SET_ID	BIGINT,
    @firstMemberId	BIGINT OUTPUT
)
AS
BEGIN

	DECLARE 
		@stm NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX),
		@packageName NVARCHAR(MAX)
       
	SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

	SET @stm = N'SELECT @firstMemberId = dbo.' + @packageName + N'$GetFirstMemberId()'

    SET @paramDefinition = N'@firstMemberId BIGINT OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @firstMemberId OUTPUT

END
GO
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$ExistsAt') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$ExistsAt
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAt') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAt
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAt 
(
	@pCODE				VARCHAR(32),
    @pATT_ARRAY_INDEX   BIGINT,
	@dcSetId			BIGINT
)
RETURNS BIT
BEGIN
	
	DECLARE
		@bExists BIT = 0


	RETURN @bExists

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesBeforeInsert') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesBeforeInsert
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesBeforeInsert
(
	@pCODE					VARCHAR(32),
    @pATT_ID                BIGINT,
    @pATT_ARRAY_INDEX 	    BIGINT,
	@pATT_REF_SET			VARCHAR(MAX)
)
AS
BEGIN

	DECLARE
		@stm                 NVARCHAR(MAX),
		@sContainerCondition NVARCHAR(MAX),
		@sTableName          NVARCHAR(MAX),
		@paramDefinition     NVARCHAR(MAX),
		@dcSetId             BIGINT,
		@firstMemberBySetId  BIGINT,
		@existsAt			 BIT

    SET @dcSetId = dbo.DX_PK_CUSTOMQUERY$GetToken(@pATT_REF_SET, 1, ':')
    
	EXEC DXDIR_PK_BASE_ASSESSMENT$GetContainerConditionBySetId @dcSetId , @sContainerCondition OUTPUT

    EXEC DXDIR_PK_BASE_ASSESSMENT$GetTableNameBySetId @dcSetId, @sTableName OUTPUT
   
	SET @existsAt = dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAt( @pCODE, @pATT_ARRAY_INDEX, @dcSetId )

	EXEC DXDIR_PK_BASE_ASSESSMENT$GetFirstMemberBySetId @dcSetId, @firstMemberBySetId OUTPUT

    IF (@existsAt = 1 AND @firstMemberBySetId = @pATT_ID)
	BEGIN
		SET @stm = N'UPDATE ' + @sTableName + N' A'
					+ N' SET A.ARRAY_INDEX = A.ARRAY_INDEX + 1'
					+ N' WHERE ' + @sContainerCondition 
					+ N' AND A.ARRAY_INDEX >= @pATT_ARRAY_INDEX'
               
		SET @paramDefinition = N'@pCODE	VARCHAR(32), @pATT_ARRAY_INDEX BIGINT'
		EXECUTE sp_executesql @stm, @paramDefinition,
							  @pCODE = @pCODE,
							  @pATT_ARRAY_INDEX = @pATT_ARRAY_INDEX
	END 
     
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow
GO
---$ END COMMAND


---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow 
(
	@pCODE			  VARCHAR(32),
    @pATT_ARRAY_INDEX BIGINT,
	@dcSetId          BIGINT
)   
RETURNS BIT
BEGIN
	
	DECLARE 
		@bEmpty BIT = 0


	RETURN @bEmpty

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$GetEmptyRowConditionBySetId') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetEmptyRowConditionBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetEmptyRowConditionBySetId 
(
    @pATT_SET_ID BIGINT,
	@condition   NVARCHAR(MAX) OUTPUT
)
AS       
BEGIN

    DECLARE 
		@stm			 NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX),
		@packageName	 NVARCHAR(MAX)

	SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

    SET @stm = N'SELECT @condition = dbo.' + @packageName + N'$GetEmptyRowCondition()'
     
	SET @paramDefinition = N'@condition NVARCHAR(MAX) OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @condition OUTPUT

END
GO
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesAfterDelete') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesAfterDelete
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$UpdateIndexesAfterDelete
(
	@pCODE			   VARCHAR(32),
    @pATT_ID           BIGINT,
    @pATT_ARRAY_INDEX  BIGINT,
	@pATT_REF_SET      VARCHAR(MAX)
)
AS
BEGIN
	
	DECLARE
		@dcSetId             BIGINT,
		@stm                 NVARCHAR(MAX),
		@sContainerCondition NVARCHAR(MAX),
		@paramDefinition     NVARCHAR(MAX),
		@sTableName          NVARCHAR(MAX),
		@sEmptyRowCondition  NVARCHAR(MAX),
		@isEmptyRow		     BIT

    SET @dcSetId = dbo.DX_PK_CUSTOMQUERY$GetToken(@pATT_REF_SET, 1, ':')
	
	EXEC DXDIR_PK_BASE_ASSESSMENT$GetContainerConditionBySetId @dcSetId , @sContainerCondition OUTPUT
	
	EXEC DXDIR_PK_BASE_ASSESSMENT$GetTableNameBySetId @dcSetId, @sTableName OUTPUT

	EXEC DXDIR_PK_BASE_ASSESSMENT$GetEmptyRowConditionBySetId @dcSetId, @sEmptyRowCondition OUTPUT

	SET @isEmptyRow = dbo.DXDIR_PK_BASE_ASSESSMENT$IsEmptyRow( @pCODE, @pATT_ARRAY_INDEX, @dcSetId )

    IF (@isEmptyRow= 1)
    BEGIN 
		-- Delete the row if it is empty
        SET @stm = N'DELETE ' + @sTableName + N' A'
                        + N' WHERE ' + @sContainerCondition 
                        + N' AND A.ARRAY_INDEX = @pATT_ARRAY_INDEX'
               
        SET @paramDefinition = N'@pCODE	VARCHAR(32), @pATT_ARRAY_INDEX BIGINT'
		EXECUTE sp_executesql @stm, @paramDefinition,
							  @pCODE = @pCODE,
							  @pATT_ARRAY_INDEX = @pATT_ARRAY_INDEX

        -- Then update the array indexes
        SET @stm = N'UPDATE ' + @sTableName + N' A'
                        +N' SET A.ARRAY_INDEX = A.ARRAY_INDEX - 1'
                        +N' WHERE ' + @sContainerCondition 
                        +N' AND A.ARRAY_INDEX > @pATT_ARRAY_INDEX';
               
        SET @paramDefinition = N'@pCODE	VARCHAR(32), @pATT_ARRAY_INDEX BIGINT'
		EXECUTE sp_executesql @stm, @paramDefinition,
							  @pCODE = @pCODE,
							  @pATT_ARRAY_INDEX = @pATT_ARRAY_INDEX
        
     END
     
END
GO
---$ END COMMAND



---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAnyValue') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAnyValue
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsAnyValue
(
	@pCODE	 VARCHAR(32),
	@pATT_ID BIGINT
)
RETURNS BIT
BEGIN

    RETURN 0
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValue') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValue
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValue
(
	@pCODE	VARCHAR(32),
	@pATT_ID BIGINT,
	@pVALUE  NVARCHAR(MAX)
)
RETURNS BIT
BEGIN
    RETURN 0
END
GO
---$ END COMMAND

---$ START COMMAND
-- If the given attribute value is found in any containers, return 1, otherwise 0.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValueForAnyContainers') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValueForAnyContainers
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsValueForAnyContainers
(
	@pATT_ID BIGINT,
	@pVALUE  NVARCHAR(MAX)
)
RETURNS BIT
BEGIN
    RETURN 0

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsMemoValue') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsMemoValue
GO
---$ END COMMAND


---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistsMemoValue
(
	@pCODE	 VARCHAR(32),
	@pATT_ID BIGINT,
	@pVALUE  NVARCHAR(MAX)
)
RETURNS BIT
BEGIN
    RETURN 0

END
GO
---$ END COMMAND


---$ START COMMAND
-- If the given attribute value is found in any containers, return 1, otherwise 0.
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$ExistMemoValueForAnyContainers') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistMemoValueForAnyContainers
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$ExistMemoValueForAnyContainers
(
	@pATT_ID BIGINT,
	@pVALUE NVARCHAR(MAX)
)
RETURNS BIT
BEGIN
	
    RETURN 0
END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$GetContainerSelfJoinConditionBySetId') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetContainerSelfJoinConditionBySetId
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$GetContainerSelfJoinConditionBySetId 
(
    @pATT_SET_ID BIGINT,
	@condition   NVARCHAR(MAX) OUTPUT
)   
AS
BEGIN
	DECLARE
		@stm             NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX),
		@packageName	 NVARCHAR(MAX)

	SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

	SET @stm = N'SELECT @condition = dbo.' + @packageName + N'$GetContainerSelfJoinCondition()'

    SET @paramDefinition = N'@condition NVARCHAR(MAX) OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @condition OUTPUT

END
GO
---$ END COMMAND

---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'DXDIR_PK_BASE_ASSESSMENT$IsMemberOfSet') AND OBJECTPROPERTY(id, N'IsProcedure')=1 )  
	DROP PROCEDURE DXDIR_PK_BASE_ASSESSMENT$IsMemberOfSet
GO
---$ END COMMAND

---$ START COMMAND
CREATE PROCEDURE DXDIR_PK_BASE_ASSESSMENT$IsMemberOfSet 
(
    @pATT_SET_ID		BIGINT,
    @pATT_MEMBER_ID		BIGINT,
	@result				BIT OUTPUT
) 
AS
BEGIN
	
	DECLARE 
		@stm			 NVARCHAR(MAX),
		@paramDefinition NVARCHAR(MAX),
		@packageName	 NVARCHAR(MAX)
       
    SET @packageName = dbo.DXDIR_PK_BASE_ASSESSMENT$GetPackageNameBySetId(@pATT_SET_ID)

	SET @stm = N'SELECT @result = dbo.' + @packageName + N'$IsMemberOf(@pATT_MEMBER_ID)'

    SET @paramDefinition = N'@pATT_MEMBER_ID BIGINT, @result BIT OUTPUT'
	EXECUTE sp_executesql @stm, @paramDefinition, @pATT_MEMBER_ID = @pATT_MEMBER_ID, @result = @result OUTPUT

END
GO
---$ END COMMAND


---$ START COMMAND
IF EXISTS ( SELECT NULL FROM dbo.sysobjects WHERE id = object_id(N'dbo.DXDIR_PK_BASE_ASSESSMENT$GetArrayIndexBySetPKey') AND OBJECTPROPERTY(id, N'IsScalarFunction')=1 )  
	DROP FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetArrayIndexBySetPKey
GO
---$ END COMMAND

---$ START COMMAND
CREATE FUNCTION dbo.DXDIR_PK_BASE_ASSESSMENT$GetArrayIndexBySetPKey  
(
	@pATT_SET_ID     BIGINT,
	@pCODE	         VARCHAR(32),
	@pSET_PK		 VARCHAR(MAX)
)	
RETURNS BIGINT
BEGIN


	RETURN NULL

END
GO
---$ END COMMAND