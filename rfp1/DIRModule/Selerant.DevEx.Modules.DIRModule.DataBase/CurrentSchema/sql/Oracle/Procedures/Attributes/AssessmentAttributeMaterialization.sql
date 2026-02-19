---$ START COMMAND
CREATE OR REPLACE PACKAGE DXDIR_PK_BASE_ASSESSMENT AS 

    TYPE t_cursor IS REF CURSOR;

    PROCEDURE MergeValue
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID				DX_ATTRIBUTE_DEF.ID%TYPE,
		pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_VALUE 			DX_ATTVALUE.VALUE%TYPE,
        pATT_REF_SET        VARCHAR2,
		pMergeType			INTEGER
    );
    
    PROCEDURE MergeMemoValue
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID				DX_ATTRIBUTE_DEF.ID%TYPE,
		pATT_ARRAY_INDEX 	DX_ATTVALUE_MEMO.ARRAY_INDEX%TYPE,
		pATT_VALUE 			DX_ATTVALUE_MEMO.VALUE%TYPE,
        pATT_REF_SET        VARCHAR2
    );
    
    PROCEDURE DeletePersistenceData
    (
        pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID     DX_ATTRIBUTE_DEF.ID%TYPE
    );
    
    FUNCTION GetStmForCell
    (
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pMEMBER_ID          DX_ATTRIBUTE_DEF.ID%TYPE,
		bIncludeScopePKeyColumns BOOLEAN DEFAULT TRUE
	)	RETURN VARCHAR2;
    
    FUNCTION GetStmForAttribute
    (
        pATT_ID				DX_ATTRIBUTE_DEF.ID%TYPE
	)	RETURN VARCHAR2;
	
    FUNCTION GetStmForSet
    (
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE
	)	RETURN VARCHAR2;
    
    
    FUNCTION GetStmForContainer
    RETURN VARCHAR2;
    	
	FUNCTION GetStmForItemsByNames
    (
        sNAMES              dx_pk_utility.DX_Varchar2Array
	)   RETURN VARCHAR2;

	FUNCTION GetStmForItemsByContainers
    (
        sNAMES              dx_pk_utility.DX_Varchar2Array
	)   RETURN VARCHAR2;
	
	FUNCTION GetStmForItemsByContainers
    RETURN VARCHAR2;
    
    PROCEDURE UpdateIndexesBeforeInsert
	(
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_REF_SET        VARCHAR2
	);
    
    PROCEDURE UpdateIndexesAfterDelete
	(
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_REF_SET        VARCHAR2
	);
	
	FUNCTION ExistsAnyValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE
	)
	RETURN NUMBER;
	
	FUNCTION ExistsValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE		DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER;
	
    -- If the given attribute value is found in any containers, return 1, otherwise 0.
	FUNCTION ExistsValueForAnyContainers
	(
		pATT_ID DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER;
	
	FUNCTION ExistsMemoValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE		DX_ATTVALUE_MEMO.VALUE%TYPE
	)
	RETURN NUMBER;
	
    -- If the given attribute value is found in any containers, return 1, otherwise 0.
	FUNCTION ExistMemoValueForAnyContainers
	(
		pATT_ID DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE DX_ATTVALUE_MEMO.VALUE%TYPE
	)
	RETURN NUMBER;
    
    FUNCTION ExistsAt
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_SET_ID			DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX	DX_ATTVALUE.ARRAY_INDEX%TYPE
    )   RETURN NUMBER;
    
    FUNCTION IsEmptyRow
    (
        pATT_SET_ID         DX_ATTRIBUTE_DEF.ID%TYPE,
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ARRAY_INDEX    DX_ATTVALUE.ARRAY_INDEX%TYPE
    )   RETURN NUMBER;
    
    FUNCTION GetPackageNameBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2;
    
    FUNCTION GetTableNameBySetId
    (
        pATT_SET_ID	DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2;
    
    FUNCTION GetContainerConditionBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2;
    
	FUNCTION GetContSelfJoinCondBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2;
	
    FUNCTION GetEmptyRowConditionBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2;
    
    FUNCTION GetFirstMemberBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		DX_ATTRIBUTE_DEF.ID%TYPE;
    
    FUNCTION IsMemberOfSet
    (
        pATT_SET_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_MEMBER_ID	DX_ATTRIBUTE_DEF.ID%TYPE
	)   RETURN	NUMBER;
    
    FUNCTION GetArrayIndexBySetPKey
    (
		pATT_SET_ID     DX_ATTRIBUTE_DEF.ID%TYPE,
        pCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		pSET_PK			VARCHAR2
    )	RETURN NUMBER;
    
END;
/
---$ END COMMAND

---$ START COMMAND
CREATE OR REPLACE PACKAGE BODY DXDIR_PK_BASE_ASSESSMENT AS   
    
    PROCEDURE MergeValue
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
		pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_VALUE 			DX_ATTVALUE.VALUE%TYPE,
        pATT_REF_SET        VARCHAR2,
		pMergeType			INTEGER
    )
    IS
        dcSetId             DX_ATTRIBUTE_DEF.ID%TYPE;
    BEGIN
        NULL;
    END;
    
    PROCEDURE MergeMemoValue
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
		pATT_ARRAY_INDEX 	DX_ATTVALUE_MEMO.ARRAY_INDEX%TYPE,
		pATT_VALUE 			DX_ATTVALUE_MEMO.VALUE%TYPE,
        pATT_REF_SET        VARCHAR2
    )
    IS
        dcSetId             DX_ATTRIBUTE_DEF.ID%TYPE;
    BEGIN
        NULL;
    END;
    
    PROCEDURE DeletePersistenceData
    (
        pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID     DX_ATTRIBUTE_DEF.ID%TYPE
    )
    IS
    BEGIN
        NULL;
    END;
    
    FUNCTION GetStmForCell
    (
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pMEMBER_ID          DX_ATTRIBUTE_DEF.ID%TYPE,
		bIncludeScopePKeyColumns BOOLEAN DEFAULT TRUE
	)	RETURN VARCHAR2
    IS
    BEGIN
        NULL;
    END;
    
    FUNCTION GetStmForAttribute
    (
        pATT_ID					DX_ATTRIBUTE_DEF.ID%TYPE
	)	RETURN VARCHAR2
    IS
		vSET_ID					DX_ATTRIBUTE_DEF.ID%TYPE;
    BEGIN              
        NULL;
    END;
    
    FUNCTION GetStmForSet
    (
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE
	) RETURN VARCHAR2
    IS
    BEGIN
        NULL;
    END;
    
    FUNCTION GetStmForContainer
    RETURN VARCHAR2
    IS
        stm     VARCHAR2(32767);
    BEGIN
        RETURN NULL;
    END;
    
    FUNCTION GetStmForItemsByNames
    (
        sNAMES              dx_pk_utility.DX_Varchar2Array
	)   RETURN VARCHAR2
    IS
        stm           VARCHAR2(32767);
        stmForSet     VARCHAR2(32767);
    BEGIN
        RETURN NULL;
       
    END;
    
    FUNCTION GetStmForItemsByContainers
    (
        sNAMES              dx_pk_utility.DX_Varchar2Array
	)   RETURN VARCHAR2
    IS
        stm           VARCHAR2(32767);
        stmForSet     VARCHAR2(32767);
    BEGIN
        RETURN NULL;
       
    END;
    
    FUNCTION GetStmForItemsByContainers
    RETURN VARCHAR2
    IS
        stm           VARCHAR2(32767);
    BEGIN
        RETURN NULL;
    END;
    
    PROCEDURE UpdateIndexesBeforeInsert
	(
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_REF_SET        VARCHAR2
	)
	IS
        dcSetId             DX_ATTRIBUTE_DEF.ID%TYPE;
        stm                 VARCHAR2(32767);      
        sContainerCondition VARCHAR2(32767);
        sTableName          VARCHAR2(30);
    BEGIN
        dcSetId := DX_PK_CUSTOMQUERY.GetToken(pATT_REF_SET, 1, ':');
        sContainerCondition := GetContainerConditionBySetId(dcSetId);
        sTableName := GetTableNameBySetId(dcSetId);
   
        IF (ExistsAt(pCODE, dcSetId, pATT_ARRAY_INDEX) = 1 AND GetFirstMemberBySetId(dcSetId) = pATT_ID) THEN
    
            stm :=    'UPDATE ' || sTableName || ' A'
                            ||' SET A.ARRAY_INDEX = A.ARRAY_INDEX + 1'
                            ||' WHERE ' || sContainerCondition 
                            ||' AND A.ARRAY_INDEX >= :pATT_ARRAY_INDEX';
               
            EXECUTE IMMEDIATE stm USING pCODE, pATT_ARRAY_INDEX;
        
        END IF;
     
	END;
    
    PROCEDURE UpdateIndexesAfterDelete
	(
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX 	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		pATT_REF_SET        VARCHAR2
	)
	IS
        dcSetId             DX_ATTRIBUTE_DEF.ID%TYPE;
        stm                 VARCHAR2(32767);      
        sContainerCondition VARCHAR2(32767);
        sTableName          VARCHAR2(30);
    BEGIN
        dcSetId := DX_PK_CUSTOMQUERY.GetToken(pATT_REF_SET, 1, ':');
        sContainerCondition := GetContainerConditionBySetId(dcSetId);
        sTableName := GetTableNameBySetId(dcSetId);
   
        IF (IsEmptyRow(dcSetId, pCODE, pATT_ARRAY_INDEX) = 1) THEN
    
			-- Delete the row if it is empty
            stm :=    'DELETE ' || sTableName || ' A'
                            ||' WHERE ' || sContainerCondition 
                            ||' AND A.ARRAY_INDEX = :pATT_ARRAY_INDEX';
               
            EXECUTE IMMEDIATE stm USING pCODE, pATT_ARRAY_INDEX;
            
            -- Then update the array indexes
            stm :=    'UPDATE ' || sTableName || ' A'
                            ||' SET A.ARRAY_INDEX = A.ARRAY_INDEX - 1'
                            ||' WHERE ' || sContainerCondition 
                            ||' AND A.ARRAY_INDEX > :pATT_ARRAY_INDEX';
               
            EXECUTE IMMEDIATE stm USING pCODE, pATT_ARRAY_INDEX;
        
        END IF;
     
	END;
	
	FUNCTION ExistsAnyValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE
	)
	RETURN NUMBER
	IS
		v_setId	DX_ATTRIBUTE_DEF.ID%TYPE;
	BEGIN
        RETURN 0;
	END;
	
	FUNCTION ExistsValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE		DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_setId	DX_ATTRIBUTE_DEF.ID%TYPE;
	BEGIN
        RETURN 0;
	END;
	
    -- If the given attribute value is found in any containers, return 1, otherwise 0.
	FUNCTION ExistsValueForAnyContainers
	(
		pATT_ID DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_setId	DX_ATTRIBUTE_DEF.ID%TYPE;
	BEGIN
        RETURN 0;
	END;
	
	FUNCTION ExistsMemoValue
	(
		pCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		pATT_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE		DX_ATTVALUE_MEMO.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_setId	DX_ATTRIBUTE_DEF.ID%TYPE;
	BEGIN
        RETURN 0;
	END;

	-- If the given attribute value is found in any containers, return 1, otherwise 0.
	FUNCTION ExistMemoValueForAnyContainers
	(
		pATT_ID DX_ATTRIBUTE_DEF.ID%TYPE,
		pVALUE DX_ATTVALUE_MEMO.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_setId	DX_ATTRIBUTE_DEF.ID%TYPE;
	BEGIN
        RETURN 0;
	END;
     
    FUNCTION ExistsAt
    (
        pCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_SET_ID			DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_ARRAY_INDEX	DX_ATTVALUE.ARRAY_INDEX%TYPE
    )   RETURN NUMBER
    IS
        stm                 VARCHAR2(32767);
        sContainerCondition VARCHAR2(32767);
        bExists             NUMBER(1);
    BEGIN
        sContainerCondition := GetContainerConditionBySetId(pATT_SET_ID);
        stm := 'SELECT COUNT(*) FROM DUAL WHERE EXISTS ( SELECT NULL FROM ' 
               || GetTableNameBySetId(pATT_SET_ID) || ' A'
               ||' WHERE ' || sContainerCondition 
               ||' AND A.ARRAY_INDEX = :pATT_ARRAY_INDEX)';
        
        EXECUTE IMMEDIATE stm INTO bExists USING pCODE, pATT_ARRAY_INDEX;
        
        RETURN bExists;    
    END;
    
    FUNCTION IsEmptyRow
    (
        pATT_SET_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pCODE					DXDIR_ASSESSMENT.CODE%TYPE,
        pATT_ARRAY_INDEX        DX_ATTVALUE.ARRAY_INDEX%TYPE
    )   RETURN NUMBER
    IS
        stm                 VARCHAR2(32767);
        sContainerCondition VARCHAR2(32767);
        sEmptyRowCondition  VARCHAR2(32767);
        bEmpty              NUMBER(1);
    BEGIN
        sContainerCondition := GetContainerConditionBySetId(pATT_SET_ID);
        sEmptyRowCondition  := GetEmptyRowConditionBySetId(pATT_SET_ID);
        
        stm := 'SELECT COUNT(*) FROM ' || GetTableNameBySetId(pATT_SET_ID) || ' A'
               ||' WHERE ' || sContainerCondition
               ||' AND A.ARRAY_INDEX = :pATT_ARRAY_INDEX' 
               ||' AND ' || sEmptyRowCondition;
        
        EXECUTE IMMEDIATE stm INTO bEmpty USING pCODE, pATT_ARRAY_INDEX;
        
        RETURN bEmpty;    
    END;
        
    FUNCTION GetPackageNameBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2
    IS
    BEGIN
        RETURN NULL;
    END;
    
    
    FUNCTION GetTableNameBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2
    IS
        stm             VARCHAR2(32767);
        tableName       VARCHAR2(30);        
    BEGIN
        stm := 'SELECT ' || GetPackageNameBySetId(pATT_SET_ID) || '.GetTableName() FROM DUAL';
        EXECUTE IMMEDIATE stm INTO tableName;
        
        RETURN tableName;
    END;  
    
    
    FUNCTION GetContainerConditionBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2
    IS
        stm             VARCHAR2(32767);
        condition       VARCHAR2(32767);        
    BEGIN
        stm := 'SELECT ' || GetPackageNameBySetId(pATT_SET_ID) || '.GetContainerCondition() FROM DUAL';
        EXECUTE IMMEDIATE stm INTO condition;
        
        RETURN condition;
    END;


	FUNCTION GetContSelfJoinCondBySetId
	(
		pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
	) RETURN		VARCHAR2  
	IS
		stm             VARCHAR2(32767);
        condition       VARCHAR2(32767);
	BEGIN
	
		stm := 'SELECT '|| GetPackageNameBySetId(pATT_SET_ID) || '.GetContainerSelfJoinCondition() FROM DUAL';

		EXECUTE IMMEDIATE stm INTO condition;
		RETURN condition;

	END;
	
    
    FUNCTION GetEmptyRowConditionBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		VARCHAR2
    IS
        stm             VARCHAR2(32767);
        condition       VARCHAR2(32767);        
    BEGIN
        stm := 'SELECT ' || GetPackageNameBySetId(pATT_SET_ID) || '.GetEmptyRowCondition() FROM DUAL';
        EXECUTE IMMEDIATE stm INTO condition;
        
        RETURN condition;
    END;
    
    FUNCTION GetFirstMemberBySetId
    (
        pATT_SET_ID DX_ATTRIBUTE_DEF.ID%TYPE
    )   RETURN		DX_ATTRIBUTE_DEF.ID%TYPE
    IS
        stm             VARCHAR2(32767);
        firstMemberId   DX_ATTRIBUTE_DEF.ID%TYPE;    
    BEGIN
        stm := 'SELECT ' || GetPackageNameBySetId(pATT_SET_ID) || '.GetFirstMemberId() FROM DUAL';
        EXECUTE IMMEDIATE stm INTO firstMemberId;
        
        RETURN firstMemberId;
    END;
    
    FUNCTION IsMemberOfSet
    (
        pATT_SET_ID		DX_ATTRIBUTE_DEF.ID%TYPE,
        pATT_MEMBER_ID	DX_ATTRIBUTE_DEF.ID%TYPE
	)   RETURN	NUMBER
    IS
        stm             VARCHAR2(32767);
		result			NUMBER;    
    BEGIN
        stm := 'SELECT ' || GetPackageNameBySetId(pATT_SET_ID) || '.IsMemberOf(:pATT_MEMBER_ID) FROM DUAL';
        EXECUTE IMMEDIATE stm INTO result USING pATT_MEMBER_ID;
        
        RETURN result;
    END;
        
    FUNCTION GetArrayIndexBySetPKey
    (
		pATT_SET_ID             DX_ATTRIBUTE_DEF.ID%TYPE,
        pCODE					DXDIR_ASSESSMENT.CODE%TYPE,
		pSET_PK					VARCHAR2
    )	RETURN NUMBER
    IS
    BEGIN
        RETURN NULL;
    END;
     
END;
/
---$ END COMMAND