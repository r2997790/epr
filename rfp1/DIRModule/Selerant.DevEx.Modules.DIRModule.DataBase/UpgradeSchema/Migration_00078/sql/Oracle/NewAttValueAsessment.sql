---$ START COMMAND
CREATE OR REPLACE PACKAGE DXDIR_PK_NEWATTVALUEASSESSMENT AS

	TYPE t_cursor IS REF CURSOR;
	
	TYPE DefType IS RECORD
    (
		ID				DX_ATTRIBUTE_DEF.ID%TYPE,
		TYPE			DX_ATTRIBUTE_DEF.TYPE%TYPE,
		STORAGE			DX_ATTRIBUTE_DEF.STORAGE%TYPE
    );

    TYPE MemberDefTableType IS TABLE OF DefType INDEX BY BINARY_INTEGER;
    
    TYPE ElementType IS RECORD
    (
        OBJECT_PK		DX_ATTVALUE.OBJECT_PK%TYPE,
		CODE			DXDIR_ASSESSMENT.CODE%TYPE,
        NAME			DX_ATTRIBUTE_DEF.NAME%TYPE,
        ID				DX_ATTRIBUTE_DEF.ID%TYPE,
        ARRAY_INDEX		DX_ATTVALUE.ARRAY_INDEX%TYPE,
        VALUE			DX_ATTVALUE.VALUE%TYPE,
        REF_SET			VARCHAR2(44)  
    );
    
	-- Inserts a new value to the database.
	-- If needed, update array index before insert.
	PROCEDURE InsertRecord
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          NCLOB,
        sREF_SET        VARCHAR2
    );
    
	-- Inserts a new value to the database.
	-- No array index update is performed because this procedure is supposed to be called when inserting data massively.
	PROCEDURE BulkInsertRecord
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE,
        sREF_SET        VARCHAR2
    );
    
	-- Inserts a new value to the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the InsertRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE InsertValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE
    );
    
    PROCEDURE UpdateRecord
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          NCLOB,
        sREF_SET        VARCHAR2
    );
    
	-- Updates the given value to the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the UpdateRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE UpdateValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE
    );
    
    PROCEDURE DeleteRecord
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sREF_SET        VARCHAR2
    );
    
	-- Deletes a value from the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the DeleteRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE DeleteValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE
    );
    
    PROCEDURE DeletePersistenceData
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        dcID			DX_ATTRIBUTE_DEF.ID%TYPE
    );
    
    PROCEDURE DeleteByContainer
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE
    );

	PROCEDURE DeleteAttrValuesByContainer
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sAttributeNames IN dx_pk_utility.DX_Varchar2Array
    );

    PROCEDURE SelectAttributesToContainer
    (
        sAttributeNames IN dx_pk_utility.DX_Varchar2Array,
        po_cur             OUT t_cursor
    );

    
    -- Returns 1 when any value exists for the given container and attribute ID, otherwise 0.
    -- It can be called within a SQL statement.
    FUNCTION ExistsAnyValue
	(
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE
	)
	RETURN NUMBER;
    
    -- Returns 1 when any values exist for the given container and attribute ID, otherwise 0.
    -- It may be called by the application, so result is stored as output parameter.
    PROCEDURE Exists
	(
		dcExists		OUT NUMBER,
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE
	);
	
    FUNCTION ExistsValue
	(
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE,
		sVALUE			DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER;
	
    -- If the given attribute value is found in any containers, return 1, otherwise 0.
    FUNCTION ExistsValueForAnyContainers
	(
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE,
		sVALUE			DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER;
    
	PROCEDURE InsertAndUpdateIndexes
	(
	    sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		dcID				DX_ATTRIBUTE_DEF.ID%TYPE,
	    dcINDEX_TO_INSERT	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		sATT_VALUE			DX_ATTVALUE.VALUE%TYPE
	);	
	
	PROCEDURE DeleteAndUpdateIndexes 
	(
	    sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE,
	    dcDELETE_INDEX	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		bUpdateIndexes	BOOLEAN := TRUE
	);
      
    FUNCTION GetBaseSelectStm
    RETURN VARCHAR2;
    
    FUNCTION GetMemoSelectStm
    (
        usePlaceHolderValue BOOLEAN
    ) 
    RETURN VARCHAR2;
    
    FUNCTION GetContainerSelectStm
    RETURN VARCHAR2;
    
    FUNCTION GetValuesByAttributeStm
    RETURN VARCHAR2;
    
    FUNCTION GetValuesBySetStm
    RETURN VARCHAR2;
    
    FUNCTION GetValuesByContainerStm
    (
        include_materialized BOOLEAN := TRUE
    )
    RETURN VARCHAR2;
        
    FUNCTION GetItemsByNamesStm
    (
		sNAMES              dx_pk_utility.DX_Varchar2Array
    )
    RETURN VARCHAR2;
        
    FUNCTION GetItemsByContainersStm
    RETURN VARCHAR2;
        
    FUNCTION GetItemsByContainersNamesStm
    (
		sNAMES              dx_pk_utility.DX_Varchar2Array
    )
    RETURN VARCHAR2;
    
    PROCEDURE SelectOne
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        dcID                DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX		DX_ATTVALUE.ARRAY_INDEX%TYPE,
        po_cur              OUT t_cursor
    );
    
    PROCEDURE SelectAllValuesForAttribute
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        dcID				DX_ATTRIBUTE_DEF.ID%TYPE,
        po_cur              OUT t_cursor
    );
      
	PROCEDURE SelectAllValuesForSet
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        dcID				DX_ATTRIBUTE_DEF.ID%TYPE,
        po_cur              OUT t_cursor
    );
    
    -- Opens a cursor for reading all values of all attributes of the given container
    PROCEDURE SelectAllValuesForContainer
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        po_cur              OUT t_cursor
    );
    
    -- Opens a cursor for reading all non materialized attributes values of the given container
    PROCEDURE SelectNonMtzdValForContainer
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        po_cur              OUT t_cursor
    );
    
    -- Returns 1 when any value exists for the given container, otherwise 0.
    -- Master table attributes are not considered.
    -- It can be called within a SQL statement.
    FUNCTION ExistAnyValuesForContainer
	(
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE
	)
	RETURN NUMBER;

    PROCEDURE SelectItemsByNames
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sNAMES		dx_pk_utility.DX_Varchar2Array,
        po_cur		OUT t_cursor
    );
    
    PROCEDURE SelectItemsByContainers
    (
        sCODE		dx_pk_utility.DX_VARCHAR2Array,
        po_cur      OUT t_cursor
    );
   
    PROCEDURE SelectItemsByContainers
    (
        sCODE		dx_pk_utility.DX_VARCHAR2Array,
        sNAMES      dx_pk_utility.DX_Varchar2Array,
        po_cur      OUT t_cursor
    );
    
    PROCEDURE CopyAllValuesToContainer
    (
		sFROMCODE	DXDIR_ASSESSMENT.CODE%TYPE,
		sTOCODE		DXDIR_ASSESSMENT.CODE%TYPE
    );

	PROCEDURE CopyAttributesToContainer
    (
		sFROMCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		sTOCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		sAttributeNames IN dx_pk_utility.DX_Varchar2Array
    );

	PROCEDURE UpdateMasterTable
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sATT_VALUE      NVARCHAR2,
        sATT_TYPE       VARCHAR2,
        sFIELD          VARCHAR2
    );

	FUNCTION ExistsOnMasterTable
    (
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sFIELD      VARCHAR2
    )
	RETURN NUMBER;

	FUNCTION ExistsValueOnMasterTable
    (
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sFIELD      VARCHAR2,
        sATT_TYPE	VARCHAR2,
        sATT_VALUE	DX_ATTVALUE.VALUE%TYPE
    )
    RETURN NUMBER;

	FUNCTION ExistsValueOnMasterTableAnyCnt
    (
		sFIELD				VARCHAR2,
        sATT_TYPE			VARCHAR2,
        sATT_VALUE			DX_ATTVALUE.VALUE%TYPE
    )
    RETURN NUMBER;

	PROCEDURE AssertIsMasterTableColumn
	(
		sCOLUMN_NAME		VARCHAR2
	);
    
END;
/

CREATE OR REPLACE PACKAGE BODY DXDIR_PK_NEWATTVALUEASSESSMENT AS   

    FUNCTION ASSESSMENTKeyArraysToTable
	(
        CODEArray DX_PK_UTILITY.DX_Varchar2Array
	)
    RETURN DXDIR_ASSESSMENTKeyTable
    IS
		scopeKeyTable DXDIR_ASSESSMENTKeyTable;
    BEGIN
        scopeKeyTable := DXDIR_ASSESSMENTKeyTable();

        IF CODEArray IS NOT NULL AND CODEArray.COUNT > 0
        THEN
            FOR arrayIndex IN 1..CODEArray.COUNT LOOP
                scopeKeyTable.EXTEND;
                scopeKeyTable(arrayIndex) := DXDIR_ASSESSMENTKey(CODEArray(arrayIndex));
            END LOOP;
        END IF;

        RETURN scopeKeyTable;
    END;

	-- Inserts a new value to the database.
	-- If needed, update array index before insert.
    PROCEDURE InsertRecord
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          NCLOB,
        sREF_SET        VARCHAR2
    )
    IS
        v_table			VARCHAR2(32);
        v_field			VARCHAR2(32);
        v_type			DX_ATTRIBUTE_DEF.TYPE%TYPE;    
    BEGIN
		-- Retrieve information about attribute storage
		dx_pk_NewAttributesUtilities.GetStorageInfoFromAttribute(dcID, v_table, v_field, v_type);
        
         -- MEMO: handle the memo values first
        IF v_table = 'DX_ATTVALUE_MEMO' THEN
            DX_PK_NEWATTVALUEMEMO.InsertRecord(sCODE, sNAME,  dcID, dcARRAY_INDEX, sVALUE, sREF_SET);        
            RETURN;
        END IF;
        
        -- MATERIALIZATION: Insert materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            DXDIR_PK_BASE_ASSESSMENT.MergeValue(sCODE, dcID, dcARRAY_INDEX, sVALUE, sREF_SET, 0);
            RETURN;
        END IF;

        IF v_field IS NOT NULL THEN
			-- Master Table
            UpdateMasterTable(sCODE, sVALUE, v_type, v_field);
        ELSE
            IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
				InsertAndUpdateIndexes(sCODE, dcID, dcARRAY_INDEX, sVALUE);
            ELSE
				-- Generic Table (DX_ATTVALUE)
                dx_pk_NewAttributesUtilities.InsertRecordOnStandardTables(sCODE, dcID, dcARRAY_INDEX, sVALUE, v_table);
            END IF;
        END IF;
    END;
    
	-- Inserts a new value to the database.
	-- No array index update is performed because this procedure is supposed to be called when inserting data massively.
    PROCEDURE BulkInsertRecord
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE,
        sREF_SET        VARCHAR2
    )
    IS
        v_table			VARCHAR2(32);
        v_field			VARCHAR2(32);
        v_type			DX_ATTRIBUTE_DEF.TYPE%TYPE;    
    BEGIN
        -- MATERIALIZATION: Insert materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            DXDIR_PK_BASE_ASSESSMENT.MergeValue(sCODE, dcID, dcARRAY_INDEX, sVALUE, sREF_SET, 3);
            RETURN;
        END IF;

        dx_pk_NewAttributesUtilities.GetStorageInfoFromAttribute(dcID, v_table, v_field, v_type);
        
        IF v_field IS NOT NULL THEN
			-- Master Table
            UpdateMasterTable(sCODE, sVALUE, v_type, v_field);
        ELSE
            IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
                -- Specific Table
                IF  sVALUE IS NOT NULL THEN
                    INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
                        (CODE, ID, COUNTRY, LANGUAGE, ARRAY_INDEX, VALUE)
                    VALUES
                        (sCODE, dcID, 'NNN', 'nn', dcARRAY_INDEX, sVALUE);
                END IF;
            ELSE
				-- Generic Tables
                dx_pk_NewAttributesUtilities.BulkInsertRecordOnStdTables(sCODE, dcID, dcARRAY_INDEX, sVALUE, v_table);
            END IF;
        END IF;
    END;
    
	-- Inserts a new value to the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the InsertRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE InsertValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE
    )
    IS
		v_attributeId DX_ATTRIBUTE_DEF.ID%TYPE;
		v_refSet VARCHAR2(128);
    BEGIN
		SELECT A.ID, NVL2(B.ID, B.ID || ':' || C.NAME, NULL) INTO v_attributeId, v_refSet
		FROM DX_ATTRIBUTE_DEF A
			LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
			LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
		WHERE A.NAME = sNAME
			AND A.SCOPE = 'ASSESSMENT';
			
		InsertRecord(sCODE, sNAME, v_attributeId, dcARRAY_INDEX, sVALUE, v_refSet);
    END;
    
    PROCEDURE UpdateRecord
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          NCLOB,
        sREF_SET        VARCHAR2
    )
    IS
		v_table			VARCHAR2(32);
        v_field			VARCHAR2(32);
        v_type			DX_ATTRIBUTE_DEF.TYPE%TYPE;
        
        attValueCount	BINARY_INTEGER;
		bUpdateIndexes  BOOLEAN;
    BEGIN
		-- Retrieve information about attribute storage
		dx_pk_NewAttributesUtilities.GetStorageInfoFromAttribute(dcID, v_table, v_field, v_type);
        
        -- MEMO: handle the memo values first
        IF v_table = 'DX_ATTVALUE_MEMO' THEN
            DX_PK_NEWATTVALUEMEMO.UpdateRecord(sCODE, sNAME,  dcID, dcARRAY_INDEX, sVALUE, sREF_SET);        
            RETURN;
        END IF;
        
        -- MATERIALIZATION: Update materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            DXDIR_PK_BASE_ASSESSMENT.MergeValue(sCODE, dcID, dcARRAY_INDEX, sVALUE, sREF_SET, 1);
            RETURN;
        END IF;

        IF v_field IS NOT NULL THEN
            UpdateMasterTable(sCODE, sVALUE, v_type, v_field);
        ELSE
            IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
				-- Specific Table
                SELECT COUNT(*) INTO attValueCount FROM DXDIR_ATTVALUE_ASSESSMENT WHERE CODE = sCODE AND ID = dcID AND COUNTRY = 'NNN' AND LANGUAGE = 'nn' AND ARRAY_INDEX = dcARRAY_INDEX;
                
                IF  attValueCount <> 0 THEN 
                    IF sVALUE IS NOT NULL THEN
                        UPDATE DXDIR_ATTVALUE_ASSESSMENT SET VALUE = sVALUE WHERE CODE = sCODE AND ID = dcID AND COUNTRY = 'NNN' AND LANGUAGE = 'nn' AND ARRAY_INDEX = dcARRAY_INDEX;
                    ELSE 
						IF sREF_SET IS NULL THEN
							bUpdateIndexes := TRUE;
						ELSE bUpdateIndexes := FALSE;
						END IF;
						
						DeleteAndUpdateIndexes(sCODE, dcID, dcARRAY_INDEX, bUpdateIndexes);
                    END IF;
                ELSE 
                    IF sVALUE IS NOT NULL THEN
                        INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
                            (CODE, ID, COUNTRY, LANGUAGE, ARRAY_INDEX, VALUE)
                        VALUES
                            (sCODE, dcID, 'NNN', 'nn', dcARRAY_INDEX, sVALUE);
                    END IF;
                END IF;
            ELSE
                dx_pk_NewAttributesUtilities.UpdateRecordOnStandarTables(sCODE, dcID, dcARRAY_INDEX, sVALUE, sREF_SET, v_table);
            END IF;
        END IF;
        
    END;
    
	-- Updates the given value to the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the UpdateRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE UpdateValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sVALUE          DX_ATTVALUE.VALUE%TYPE
    )
    IS
		v_attributeId DX_ATTRIBUTE_DEF.ID%TYPE;
		v_refSet VARCHAR2(128);
    BEGIN
		SELECT A.ID, NVL2(B.ID, B.ID || ':' || C.NAME, NULL) INTO v_attributeId, v_refSet
		FROM DX_ATTRIBUTE_DEF A
			LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
			LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
		WHERE A.NAME = sNAME
			AND A.SCOPE = 'ASSESSMENT';
			
		UpdateRecord(sCODE, sNAME, v_attributeId, dcARRAY_INDEX, sVALUE, v_refSet);
    END;
    
    PROCEDURE DeleteRecord
    (
        sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcID            DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE,
        sREF_SET        VARCHAR2
    )
    IS
		v_table			VARCHAR2(32);
        v_field			VARCHAR2(32);
        v_type			DX_ATTRIBUTE_DEF.TYPE%TYPE;
    BEGIN
		-- Retrieve information about attribute storage
		dx_pk_NewAttributesUtilities.GetStorageInfoFromAttribute(dcID, v_table, v_field, v_type);
        
        -- MEMO: handle the memo values first
        IF v_table = 'DX_ATTVALUE_MEMO' THEN
            DX_PK_NEWATTVALUEMEMO.DeleteRecord(sCODE, sNAME,  dcID, dcARRAY_INDEX, sREF_SET);        
            RETURN;
        END IF;
		        
        -- MATERIALIZATION: Delete materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            DXDIR_PK_BASE_ASSESSMENT.MergeValue(sCODE, dcID, dcARRAY_INDEX, NULL, sREF_SET, 2);
            RETURN;
        END IF;      
        
		IF v_field IS NOT NULL THEN
			UpdateMasterTable(sCODE, NULL, v_type, v_field);
		ELSE
			IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
				DeleteAndUpdateIndexes(sCODE, dcID, dcARRAY_INDEX);
			ELSE
				dx_pk_NewAttributesUtilities.DeleteRecordOnStandarTables(sCODE, dcID, dcARRAY_INDEX, v_table);
			END IF;
		END IF;
        
    END;
    
	-- Deletes a value from the database.
	-- It can be called from a SQL script as it accepts minimal information rather than the DeleteRecord procedure
	-- which has been built to be called from the application.
	PROCEDURE DeleteValue
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sNAME           VARCHAR2,
        dcARRAY_INDEX   DX_ATTVALUE.ARRAY_INDEX%TYPE
    )
    IS
		v_attributeId DX_ATTRIBUTE_DEF.ID%TYPE;
		v_refSet VARCHAR2(128);
    BEGIN
		SELECT A.ID, NVL2(B.ID, B.ID || ':' || C.NAME, NULL) INTO v_attributeId, v_refSet
		FROM DX_ATTRIBUTE_DEF A
			LEFT OUTER JOIN DX_ATTRIBUTE_SET B ON B.MEMBER_ID = A.ID
			LEFT OUTER JOIN DX_ATTRIBUTE_DEF C ON C.ID = B.ID 
		WHERE A.NAME = sNAME
			AND A.SCOPE = 'ASSESSMENT';
			
		DeleteRecord(sCODE, sNAME, v_attributeId, dcARRAY_INDEX, v_refSet);
    END;
    
    -- Delete all values for a specific attribute (set, set-member, simple, multi-value)
    PROCEDURE DeletePersistenceData
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        dcID		DX_ATTRIBUTE_DEF.ID%TYPE
    )
    IS
		v_table				VARCHAR2(32);
		v_field				VARCHAR2(32);
		      
		v_MemberDefValues	MemberDefTableType;
    BEGIN
    
		-- MATERIALIZATION: Delete materialized attribute first, if it is the case
		IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
			DXDIR_PK_BASE_ASSESSMENT.DeletePersistenceData(sCODE, dcID);
			RETURN;
		END IF;
       
		SELECT ID, TYPE, STORAGE BULK COLLECT INTO v_MemberDefValues FROM 
		(
			SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B WHERE A.ID=B.MEMBER_ID AND B.ID=dcID
			UNION
			SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A WHERE A.ID=dcID AND A.TYPE <> 'SET'
		);
			
		FOR i IN 1..v_MemberDefValues.COUNT LOOP
			dx_pk_NewAttributesUtilities.GetTableAndFieldFromStorage(v_MemberDefValues(i).STORAGE, v_table, v_field);
			
			IF v_field IS NOT NULL THEN
				UpdateMasterTable(sCODE, NULL, v_MemberDefValues(i).TYPE, v_field);
			ELSE
				IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
					DELETE FROM DXDIR_ATTVALUE_ASSESSMENT WHERE CODE = sCODE AND ID = v_MemberDefValues(i).ID;
				ELSE
					dx_pk_NewAttributesUtilities.DeletePersistenceData(sCODE, v_MemberDefValues(i).ID, v_table);
				END IF;
			END IF;
		END LOOP;
    END;
    

	PROCEDURE SelectAttributesToContainer
    (
        sAttributeNames IN dx_pk_utility.DX_Varchar2Array,
        po_cur             OUT t_cursor
    )
    IS
         v_stm              VARCHAR2(32767);
         v_arrayEmpty  CHAR(1) := 'N';
         v_attributeInfos        DX_AttributeInfoTable;
    BEGIN
      
      IF DX_PK_UTILITY.IsEmptyArray(sAttributeNames) THEN
        v_arrayEmpty := 'Y';
      END IF;
     
      IF v_arrayEmpty = 'N' THEN
        v_attributeInfos := DX_PK_NEWATTRIBUTESUTILITIES.NamesToAttributeInfoTable(snames => sAttributeNames,
                                                                                   sscope => 'ASSESSMENT');
        v_stm := 'SELECT ID, STORAGE, FLAGS '
                  || ' FROM DX_ATTRIBUTE_DEF ' 
                  || ' WHERE SCOPE = ''ASSESSMENT'' ' 
                  || ' AND NAME IN (SELECT NAME FROM TABLE(CAST(:attributeInfos AS DX_AttributeInfoTable)))';  
        OPEN po_cur FOR v_stm USING v_attributeInfos;                                                                                  
      ELSE
        v_stm := 'SELECT ID, STORAGE, FLAGS '
                  || ' FROM DX_ATTRIBUTE_DEF ' 
                  || ' WHERE SCOPE = ''ASSESSMENT'' ' 
                  || ' AND STORAGE NOT LIKE ''ASSESSMENT.%'' ';
        OPEN po_cur FOR v_stm;
      END IF;
     
    END;
	
	-- Delete all values for a specific container.
    PROCEDURE DeleteByContainer
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE
    )
	IS
		 v_attributeNames dx_pk_utility.DX_Varchar2Array;
	BEGIN
    
		 DeleteAttrValuesByContainer(sCODE, v_attributeNames);
	END;

    -- Delete Attribute values for a specific container.
    PROCEDURE DeleteAttrValuesByContainer
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		sAttributeNames IN dx_pk_utility.DX_Varchar2Array
    )
	IS
		v_materializedAttributesIds DX_PK_Utility.DX_NumberArray;
		v_specificAttributesIds DX_PK_Utility.DX_NumberArray;
		v_genericAttributesIds DX_PK_Utility.DX_NumberArray;
		v_memoAttributesIds DX_PK_Utility.DX_NumberArray;
		v_masterAttributesIds DX_PK_Utility.DX_NumberArray;
		
		v_specificAttributesIdsTable DX_NumberTable;
		v_cursor                t_cursor;
		v_element               DX_PK_NEWATTRIBUTESUTILITIES.AttributeElement;

	BEGIN
		
		SelectAttributesToContainer(sAttributeNames, v_cursor);

		LOOP
			FETCH v_cursor INTO v_element;
			EXIT WHEN v_cursor%NOTFOUND;
        
			IF BITAND(v_element.FLAGS, POWER(2, DX_PK_AttributeDef.MATERIALIZED_FLAG_POS)) <> 0 THEN
			  v_materializedAttributesIds(v_materializedAttributesIds.COUNT + 1) := v_element.ID;
			ELSIF v_element.STORAGE = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
			  v_specificAttributesIds(v_specificAttributesIds.COUNT + 1) := v_element.ID;
			ELSIF v_element.STORAGE = 'DX_ATTVALUE' THEN
			  v_genericAttributesIds(v_genericAttributesIds.COUNT + 1) := v_element.ID;
			ELSIF v_element.STORAGE = 'DX_ATTVALUE_MEMO' THEN
			  v_memoAttributesIds(v_memoAttributesIds.COUNT + 1) := v_element.ID;
			ELSIF INSTR(v_element.STORAGE, 'ASSESSMENT.') = 1 THEN
			  v_masterAttributesIds(v_masterAttributesIds.COUNT + 1) := v_element.ID;
			END IF;
        END LOOP;
		CLOSE v_cursor;
		
		FOR i IN 1..v_materializedAttributesIds.COUNT LOOP
			DXDIR_PK_BASE_ASSESSMENT.DeletePersistenceData(sCODE, v_materializedAttributesIds(i));
		END LOOP;
		
		IF v_specificAttributesIds.COUNT > 0 THEN
			v_specificAttributesIdsTable := DX_PK_Utility.ArrayToTable(v_specificAttributesIds);
			DELETE DXDIR_ATTVALUE_ASSESSMENT 
			WHERE CODE = sCODE
			  AND ID IN (SELECT COLUMN_VALUE FROM THE(SELECT CAST(v_specificAttributesIdsTable AS DX_NumberTable) FROM DUAL));
		END IF;
			
		IF v_genericAttributesIds.COUNT > 0 THEN
			DX_PK_NewAttributesUtilities.DeletePersistenceData(sCODE, v_genericAttributesIds, 'DX_ATTVALUE');
		END IF;
		
		IF v_memoAttributesIds.COUNT > 0 THEN
			DX_PK_NewAttributesUtilities.DeletePersistenceData(sCODE, v_memoAttributesIds, 'DX_ATTVALUE_MEMO');
		END IF;

		FOR i IN 1..v_masterAttributesIds.COUNT LOOP
            DeletePersistenceData(sCODE, v_masterAttributesIds(i));
        END LOOP;

	END;    
	
    -- Returns 1 when any value exists for the given container and attribute ID, otherwise 0.
    -- It can be called within a SQL statement.
	FUNCTION ExistsAnyValue
	(
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		dcID DX_ATTRIBUTE_DEF.ID%TYPE
	)
	RETURN NUMBER
	IS
		v_exists NUMBER(1) := 0;
		v_table VARCHAR2(32);
		v_field VARCHAR2(32);
		v_MemberDefValues MemberDefTableType;
	BEGIN
		-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
		IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
			RETURN DXDIR_PK_BASE_ASSESSMENT.ExistsAnyValue(sCODE, dcID);
		END IF;
              
		SELECT ID, TYPE, STORAGE BULK COLLECT INTO v_MemberDefValues FROM 
		(
			SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B WHERE A.ID=B.MEMBER_ID AND B.ID=dcID
			UNION
			SELECT A.ID, A.TYPE, A.STORAGE FROM DX_ATTRIBUTE_DEF A WHERE A.ID=dcID AND A.TYPE <> 'SET'
		);

		FOR i IN 1..v_MemberDefValues.COUNT LOOP
			dx_pk_NewAttributesUtilities.GetTableAndFieldFromStorage(v_MemberDefValues(i).STORAGE, v_table, v_field);
			
			IF v_field IS NOT NULL THEN
				v_exists := ExistsOnMasterTable(sCODE, v_field);
			ELSE
				IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
					SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END INTO v_exists FROM DXDIR_ATTVALUE_ASSESSMENT WHERE CODE = sCODE AND ID=v_MemberDefValues(i).ID;
				ELSE
					v_exists := dx_pk_NewAttributesUtilities.ExistsOnStandardTables(sCODE, v_MemberDefValues(i).ID, v_table);
				END IF;
			END IF;
			
			IF v_exists <> 0 THEN 
				-- Exit from the actual loop as we found at least a value.
				EXIT;
			END IF;
		END LOOP;
		
		RETURN v_exists;
    END;
    
    -- Returns 1 when any values exist for the given container and attribute ID, otherwise 0.
    -- It may be called by the application, so result is stored as output parameter.
	PROCEDURE Exists
	(
		dcExists	OUT NUMBER,
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		dcID		DX_ATTRIBUTE_DEF.ID%TYPE
	)
	IS
	BEGIN
		dcExists := ExistsAnyValue(sCODE, dcID);
    END;
    
    FUNCTION ExistsValue
	(
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		dcID		DX_ATTRIBUTE_DEF.ID%TYPE,
		sVALUE		DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_type DX_ATTRIBUTE_DEF.TYPE%TYPE;
		v_storage DX_ATTRIBUTE_DEF.STORAGE%TYPE;

		v_table VARCHAR2(32);
		v_field VARCHAR2(32);
		
		v_exists NUMBER(1) := 0;
	BEGIN
		-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
		IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
			v_exists := DXDIR_PK_BASE_ASSESSMENT.ExistsValue(sCODE, dcID, sVALUE);
		ELSE
			SELECT TYPE, STORAGE INTO v_type, v_storage FROM DX_ATTRIBUTE_DEF WHERE ID = dcID;

			dx_pk_NewAttributesUtilities.GetTableAndFieldFromStorage(v_storage, v_table, v_field);
				
			IF v_field IS NOT NULL THEN
				v_exists := ExistsValueOnMasterTable(sCODE, v_field, v_type, sVALUE);
			ELSE
				IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
					SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE INTO v_exists FROM DXDIR_ATTVALUE_ASSESSMENT WHERE CODE = sCODE AND ID = dcID AND VALUE = sVALUE;
				ELSE
					v_exists := dx_pk_NewAttributesUtilities.ExistsValueOnStandardTables(sCODE, dcID, sVALUE, v_table);
				END IF;
			END IF;
		END IF;
		
		RETURN v_exists;
    END;
    
    -- If the given attribute value is found in any containers, return 1, otherwise 0.
    FUNCTION ExistsValueForAnyContainers
	(
		dcID			DX_ATTRIBUTE_DEF.ID%TYPE,
		sVALUE			DX_ATTVALUE.VALUE%TYPE
	)
	RETURN NUMBER
	IS
		v_type DX_ATTRIBUTE_DEF.TYPE%TYPE;
		v_storage DX_ATTRIBUTE_DEF.STORAGE%TYPE;

		v_table VARCHAR2(32);
		v_field VARCHAR2(32);
		
		v_exists NUMBER(1) := 0;
	BEGIN
		-- MATERIALIZATION: Check the materialized attribute first, if it is the case.
		IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
			v_exists := DXDIR_PK_BASE_ASSESSMENT.ExistsValueForAnyContainers(dcID, sVALUE);
		ELSE
			SELECT TYPE, STORAGE INTO v_type, v_storage FROM DX_ATTRIBUTE_DEF WHERE ID = dcID;

			dx_pk_NewAttributesUtilities.GetTableAndFieldFromStorage(v_storage, v_table, v_field);
				
			IF v_field IS NOT NULL THEN
				v_exists := ExistsValueOnMasterTableAnyCnt(v_field, v_type, sVALUE);
			ELSE
				IF v_table = 'DXDIR_ATTVALUE_ASSESSMENT' THEN
					SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE INTO v_exists FROM DXDIR_ATTVALUE_ASSESSMENT WHERE ID = dcID AND VALUE = sVALUE;
				ELSE
					v_exists := dx_pk_NewAttributesUtilities.ExistsValForAnyContOnStdTables(dcID, sVALUE, v_table);
				END IF;
			END IF;
		END IF;
		
		RETURN v_exists;
    END;
    

	PROCEDURE InsertAndUpdateIndexes 
	(
	    sCODE				DXDIR_ASSESSMENT.CODE%TYPE,
		dcID				DX_ATTRIBUTE_DEF.ID%TYPE,
	    dcINDEX_TO_INSERT	DX_ATTVALUE.ARRAY_INDEX%TYPE,
		sATT_VALUE			DX_ATTVALUE.VALUE%TYPE
	)
	IS
		v_existsCell NUMBER(1) := 0;
		lastArrayIndex  DX_ATTVALUE.ARRAY_INDEX%TYPE;
		lastValue	    DX_ATTVALUE.VALUE%TYPE;
	BEGIN
	
		-- Check if requested cell already exists
		
		SELECT COUNT(*) INTO v_existsCell
		FROM DXDIR_ATTVALUE_ASSESSMENT
		WHERE CODE = sCODE
		 AND ID = dcID
		 AND COUNTRY = 'NNN' 
		 AND LANGUAGE = 'nn' 
		 AND ARRAY_INDEX = dcINDEX_TO_INSERT;

		IF v_existsCell > 0 THEN
	
			BEGIN
				-- Cell exists, shift the existing cells
				SELECT
					ARRAY_INDEX, VALUE into lastArrayIndex, lastValue
				FROM DXDIR_ATTVALUE_ASSESSMENT A1
				WHERE CODE = sCODE
				 AND ID = dcID
				 AND COUNTRY='NNN' 
				 AND LANGUAGE='nn'
				 AND ARRAY_INDEX = ( 
					SELECT MAX(ARRAY_INDEX)
					FROM DXDIR_ATTVALUE_ASSESSMENT A2 
					WHERE
						A2.CODE = A1.CODE
					AND A2.ID = A1.ID
					AND A2.COUNTRY='NNN' 
					AND A2.LANGUAGE='nn'
				);

			EXCEPTION 
				WHEN NO_DATA_FOUND THEN 
					lastArrayIndex := NULL;
                    lastValue := NULL;
			END;
			
			IF lastArrayIndex IS NOT NULL AND dcINDEX_TO_INSERT<lastArrayIndex THEN

				-- All cells that follow the one to be inserted must be shifted one position down
				UPDATE DXDIR_ATTVALUE_ASSESSMENT A1
				SET 
					VALUE =
						(     
							SELECT VALUE
							FROM DXDIR_ATTVALUE_ASSESSMENT A2
							WHERE A2.CODE = A1.CODE
							AND A2.ID = A1.ID
							AND A2.COUNTRY = A1.COUNTRY
							AND A2.LANGUAGE = A1.LANGUAGE
							AND A2.ARRAY_INDEX = (

								SELECT MAX(ARRAY_INDEX)
								FROM DXDIR_ATTVALUE_ASSESSMENT A3
								WHERE A3.CODE = A2.CODE
								AND A3.ID = A2.ID
								AND A3.COUNTRY = A2.COUNTRY
								AND A3.LANGUAGE = A2.LANGUAGE
								AND A3.ARRAY_INDEX >= dcINDEX_TO_INSERT
								AND A3.ARRAY_INDEX < A1.ARRAY_INDEX
							)
						)   
				WHERE A1.CODE = sCODE
				AND A1.ID = dcID
				AND A1.COUNTRY='NNN' 
				AND A1.LANGUAGE='nn' 
				AND A1.ARRAY_INDEX > dcINDEX_TO_INSERT;


			END IF;
		
			-- In order to shift all cells down, a new cell must be added at the end of the previous list
			INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
				(CODE, ID, COUNTRY, LANGUAGE, ARRAY_INDEX, VALUE)
			VALUES
				(sCODE, dcID, 'NNN', 'nn', lastArrayIndex+1,  lastValue);
	
		END IF;
	
		IF sATT_VALUE IS NULL THEN
			IF v_existsCell > 0 THEN
				-- The client has requested the insertion of a NULL value in this cell position
				-- No cell will be actually inserted, cells needed just to be shifted down and requested cell must be deleted
				DELETE DXDIR_ATTVALUE_ASSESSMENT
				WHERE 
					CODE = sCODE
				AND ID= dcID
				AND COUNTRY='NNN' 
				AND LANGUAGE='nn'  
				AND ARRAY_INDEX = dcINDEX_TO_INSERT;
			END IF;
		ELSE
			IF v_existsCell > 0 THEN
				-- Update the existing cell
				UPDATE DXDIR_ATTVALUE_ASSESSMENT
					SET VALUE = sATT_VALUE
			    WHERE 
					CODE = sCODE
					AND ID=dcID
					AND COUNTRY='NNN' 
					AND LANGUAGE='nn' 
					AND ARRAY_INDEX = dcINDEX_TO_INSERT;
			ELSE
				-- Insert a new cell
				INSERT INTO DXDIR_ATTVALUE_ASSESSMENT
					(CODE, ID, COUNTRY, LANGUAGE, ARRAY_INDEX, VALUE)
						VALUES
					(sCODE, dcID, 'NNN', 'nn', dcINDEX_TO_INSERT,  sATT_VALUE);
			END IF;

		END IF;
	
	END;
	
    
	PROCEDURE DeleteAndUpdateIndexes 
	(
		sCODE				DXDIR_ASSESSMENT.CODE%TYPE,
		dcID				DX_ATTRIBUTE_DEF.ID%TYPE,
	    dcDELETE_INDEX		DX_ATTVALUE.ARRAY_INDEX%TYPE,
		bUpdateIndexes		BOOLEAN := TRUE
	)
	IS
		
		arrayIndexAfterDelete DX_ATTVALUE.ARRAY_INDEX%TYPE;
		valueAfterDelete DX_ATTVALUE.VALUE%TYPE;
		lastArrayIndex DX_ATTVALUE.ARRAY_INDEX%TYPE;

	BEGIN
		
		IF bUpdateIndexes = FALSE THEN
			
			-- Just need to delete the cell without moving the positions of the remaining ones
			-- This mainly occurs because the attribute is member of a set and then rows position must not be changed
			DELETE FROM DXDIR_ATTVALUE_ASSESSMENT 
			WHERE CODE = sCODE
			AND ID = dcID 
			AND COUNTRY = 'NNN' 
			AND LANGUAGE = 'nn' 
			AND ARRAY_INDEX = dcDELETE_INDEX;
			
			RETURN;
	
		END IF;
		
		-- Shift the position of the remaining cells
	
		BEGIN
			SELECT
				ARRAY_INDEX, VALUE into arrayIndexAfterDelete, valueAfterDelete
			FROM DXDIR_ATTVALUE_ASSESSMENT A1
			WHERE CODE = sCODE
			AND ID = dcID
			AND COUNTRY='NNN' 
			AND LANGUAGE='nn'
			AND ARRAY_INDEX = ( 
				SELECT MIN(ARRAY_INDEX)
				FROM DXDIR_ATTVALUE_ASSESSMENT A2 
				WHERE A2.CODE = A1.CODE
				AND A2.ID = A1.ID
				AND A2.COUNTRY='NNN' 
				AND A2.LANGUAGE='nn'
				AND A2.ARRAY_INDEX > dcDELETE_INDEX
			);

		EXCEPTION 
			WHEN NO_DATA_FOUND THEN 
				arrayIndexAfterDelete := NULL;
				valueAfterDelete := NULL;
		END;
			
	
		IF arrayIndexAfterDelete IS NULL
			-- No cells left or the cell with the highest position is going to be deleted
			-- No need to shift any cell
		THEN
	
			DELETE FROM DXDIR_ATTVALUE_ASSESSMENT 
			WHERE CODE = sCODE
			AND ID = dcID 
			AND COUNTRY = 'NNN' 
			AND LANGUAGE = 'nn' 
			AND ARRAY_INDEX = dcDELETE_INDEX;
			
			RETURN;
	
		END IF;
	
		-- Get the last cell index
		SELECT MAX(ARRAY_INDEX) INTO lastArrayIndex
		FROM DXDIR_ATTVALUE_ASSESSMENT
		WHERE CODE = sCODE
		AND ID = dcID
		AND COUNTRY='NNN' 
		AND LANGUAGE='nn';
	
	
		IF lastArrayIndex IS NOT NULL AND arrayIndexAfterDelete<lastArrayIndex THEN

			-- There is more than one cell following the one to be deleted
			-- All cells must be shifted one position up

			UPDATE DXDIR_ATTVALUE_ASSESSMENT A1
			SET 
				VALUE =
					(     
						SELECT VALUE
						FROM DXDIR_ATTVALUE_ASSESSMENT A2
						WHERE A2.CODE = A1.CODE
						AND A2.ID = A1.ID
						AND A2.COUNTRY= A1.COUNTRY
						AND A2.LANGUAGE= A1.LANGUAGE
						AND A2.ARRAY_INDEX = (
							SELECT MIN(ARRAY_INDEX)
							FROM DXDIR_ATTVALUE_ASSESSMENT A3
							WHERE A3.CODE = A2.CODE
							AND A3.ID = A2.ID
							AND A3.COUNTRY= A2.COUNTRY
							AND A3.LANGUAGE= A2.LANGUAGE
							AND A3.ARRAY_INDEX > dcDELETE_INDEX
							AND A3.ARRAY_INDEX > A1.ARRAY_INDEX
						)
					)   
			WHERE A1.CODE = sCODE
			AND A1.ID = dcID
			AND A1.COUNTRY='NNN' 
			AND A1.LANGUAGE='nn' 
			AND A1.ARRAY_INDEX >= dcDELETE_INDEX;

		ELSE

			-- Only one cell is present after the one to be deleted; set its value in the position of the cell to be deleted 
			-- not to leave a hole in the ARRAY_INDEX values
			UPDATE DXDIR_ATTVALUE_ASSESSMENT
				SET VALUE = valueAfterDelete
			WHERE CODE = sCODE
			AND ID = dcID
			AND COUNTRY='NNN' 
			AND LANGUAGE='nn'
			AND ARRAY_INDEX = dcDELETE_INDEX;

		END IF;
	
	
		-- Delete last cell after having shifted cells values
		DELETE DXDIR_ATTVALUE_ASSESSMENT
		WHERE CODE = sCODE
			AND ID=dcID
			AND COUNTRY='NNN' 
			AND LANGUAGE='nn'
		AND ARRAY_INDEX = lastArrayIndex;
	
	END;


    FUNCTION GetBaseSelectStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
    BEGIN
        stm :=
            'SELECT /*+ ORDERED_PREDICATES USE_NL(B,A) */' 
			|| ' A.CODE OBJECT_PK,'
			|| ' A.CODE,' 
			|| ' C.NAME, A.ID, A.ARRAY_INDEX, A.VALUE, C.REF_SET'
			|| ' FROM DXDIR_ATTVALUE_ASSESSMENT A,'
            || ' tPKEY B,'
            || ' tDEF C '
			|| ' WHERE'
		    || ' A.CODE = B.CODE'
			|| ' AND A.ID = C.ID'

			|| ' UNION ALL'

			|| ' SELECT /*+ ORDERED_PREDICATES USE_NL(B,A) */'
			|| ' A.OBJECT_PK,' 
			|| ' B.CODE AS CODE,' 
			|| ' C.NAME, A.ID, A.ARRAY_INDEX, A.VALUE, C.REF_SET'
			|| ' FROM DX_ATTVALUE A,'
            || ' tPKEY B,'
            || ' tDEF C'
            || ' WHERE'
			|| ' A.OBJECT_PK = B.CODE'
			|| ' AND A.ID = C.ID'

			|| ' UNION ALL'
			
			|| ' ' ||  GetMemoSelectStm(TRUE)
			
			|| ' UNION ALL'

			|| ' ' || GetContainerSelectStm()
            ;

        RETURN stm;
		
    END;
    
    FUNCTION GetContainerSelectStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
    BEGIN
        stm :=
           'SELECT /*+ ORDERED_PREDICATES USE_NL(B,A) */'
            || ' A.CODE OBJECT_PK,'
            || ' A.CODE,'
            || ' C.NAME, C.ID, 0 ARRAY_INDEX,'
            || ' CASE'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.CODE'' THEN TO_NCHAR(A.CODE)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.TYPE_CODE'' THEN TO_NCHAR(A.TYPE_CODE)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.DESCRIPTION'' THEN A.DESCRIPTION'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.STATUS'' THEN TO_NCHAR(A.STATUS)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.COMPANY_NAME'' THEN TO_NCHAR(A.COMPANY_NAME)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.COMPLETING_BY'' THEN TO_NCHAR(A.COMPLETING_BY)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.PHONE'' THEN TO_NCHAR(A.PHONE)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.EMAIL'' THEN TO_NCHAR(A.EMAIL)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.TIMEFRAME_FROM'' THEN TO_NCHAR(A.TIMEFRAME_FROM, ''YYYYMMDDHH24MISS'')'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.TIMEFRAME_TO'' THEN TO_NCHAR(A.TIMEFRAME_TO, ''YYYYMMDDHH24MISS'')'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.CREATE_DATE'' THEN TO_NCHAR(A.CREATE_DATE, ''YYYYMMDDHH24MISS'')'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.CREATED_BY'' THEN TO_NCHAR(A.CREATED_BY)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.MOD_DATE'' THEN TO_NCHAR(A.MOD_DATE, ''YYYYMMDDHH24MISS'')'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.MODIFIED_BY'' THEN TO_NCHAR(A.MODIFIED_BY)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.AUTHORIZATION_ROLE'' THEN TO_NCHAR(A.AUTHORIZATION_ROLE)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.ORG_STRUCTURE'' THEN TO_NCHAR(A.ORG_STRUCTURE)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.LOCATION'' THEN TO_NCHAR(A.LOCATION)'
			||    ' WHEN C.STORAGE=''DXDIR_ASSESSMENT.PROD_CLASSIF'' THEN TO_NCHAR(A.PROD_CLASSIF)'
			||	  ' ELSE NULL'			
		    || ' END VALUE,'
            || ' C.REF_SET'
            || ' FROM DXDIR_ASSESSMENT A,'
            || ' tPKEY B,'
            || ' tDEF C'
            || ' WHERE'
            || ' A.CODE = B.CODE'
            || ' AND C.STORAGE LIKE ''DXDIR_ASSESSMENT.%'''
            ;
                    
        RETURN stm;
    
    END;
    
    FUNCTION GetMemoSelectStm
    (
        usePlaceHolderValue BOOLEAN
    )
    RETURN VARCHAR2
    IS
        stm					VARCHAR2(32767);
        stmValue			VARCHAR2(200);
    BEGIN
       
        IF(usePlaceHolderValue = TRUE) THEN
            stmValue := 'CASE WHEN (DBMS_LOB.GETLENGTH(A.VALUE)<=2000) THEN TO_NCHAR(A.VALUE) ELSE N''Unloaded Memo Value'' END';
        ELSE
            stmValue := 'A.VALUE';
        END IF;
    
        stm := 
            'SELECT /*+ ORDERED_PREDICATES USE_NL(B,A) */'
			|| ' A.OBJECT_PK,' 
			|| ' B.CODE AS CODE,'	
			|| ' C.NAME, A.ID, A.ARRAY_INDEX,'
            || ' ' || stmValue ||',' 
            || ' C.REF_SET'
            || ' FROM DX_ATTVALUE_MEMO A,'
            || ' tPKEY B,'
            || ' tDEF C'
            || ' WHERE'
			|| ' A.OBJECT_PK = B.CODE'
			|| ' AND A.ID = C.ID'
        ;

        RETURN stm;

    END;
    
    FUNCTION GetValueByAttributeStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
    BEGIN
        stm :=
           'WITH '
            || ' tPKEY AS (SELECT /*+ INLINE */ :pCODE CODE FROM DUAL),'
            || ' tDEF AS (SELECT /*+ INLINE */ A.ID, A.NAME, A.STORAGE, C.ID SET_ID, C.NAME SET_NAME, NVL2(C.ID, C.ID || '':'' || C.NAME, NULL) REF_SET FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B, DX_ATTRIBUTE_DEF C WHERE A.SCOPE = ''ASSESSMENT'' AND A.ID = B.MEMBER_ID(+) AND B.ID = C.ID(+) AND A.ID = :dcID)'
            
            || GetMemoSelectStm(FALSE)
           
            || ' AND A.ARRAY_INDEX = :dcARRAY_INDEX'
            ;

        return stm;
    END;
    
    FUNCTION GetValuesByAttributeStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
    BEGIN
		stm :=
           'WITH '
			|| ' tPKEY AS (SELECT /*+ INLINE */ :pCODE CODE FROM DUAL),'
			|| ' tDEF AS (SELECT /*+ INLINE */ ID, NAME, STORAGE, NULL SET_ID, NULL SET_NAME, NULL REF_SET FROM DX_ATTRIBUTE_DEF WHERE ID = :dcID)'

			|| GetBaseSelectStm()
			;
		
		return stm;
    END;
    
    FUNCTION GetValuesBySetStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
    BEGIN
		stm :=
           'WITH '
			|| ' tPKEY AS (SELECT /*+ INLINE */ :pCODE CODE FROM DUAL),'
			|| ' tDEF AS (SELECT /*+ INLINE */ A.ID, A.NAME, A.STORAGE, C.ID SET_ID, C.NAME SET_NAME, NVL2(C.ID, C.ID || '':'' || C.NAME, NULL) REF_SET FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B, DX_ATTRIBUTE_DEF C WHERE A.SCOPE = ''ASSESSMENT'' AND A.ID = B.MEMBER_ID AND B.ID = C.ID AND C.ID = :dcID)'
			
			|| GetBaseSelectStm()
			;
		
		return stm;
    END;
    
    FUNCTION GetValuesByContainerStm
    (
        include_materialized BOOLEAN := TRUE
    )
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
        stm_materialized    VARCHAR2(32767);
    BEGIN
		stm :=
           'WITH '
			|| ' tPKEY AS (SELECT /*+ INLINE */ :pCODE CODE FROM DUAL),'
			|| ' tDEF AS (SELECT /*+ INLINE */ A.ID, A.NAME, A.STORAGE, C.ID SET_ID, C.NAME SET_NAME, NVL2(C.ID, C.ID || '':'' || C.NAME, NULL) REF_SET FROM DX_ATTRIBUTE_DEF A, DX_ATTRIBUTE_SET B, DX_ATTRIBUTE_DEF C WHERE A.SCOPE = ''ASSESSMENT'' AND A.ID = B.MEMBER_ID(+) AND B.ID = C.ID(+))'

			|| GetBaseSelectStm()
			;
		
		IF (include_materialized = TRUE) THEN
			stm_materialized := DXDIR_PK_BASE_ASSESSMENT.GetStmForContainer();

			IF stm_materialized IS NOT NULL THEN
				stm := stm || ' UNION ALL ' || stm_materialized;
			END IF;
		END IF;
		
		return stm;
    END;
    
    FUNCTION GetItemsByNamesStm
    (
		sNAMES              dx_pk_utility.DX_Varchar2Array
    )
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
        stm_materialized    VARCHAR2(32767);
    BEGIN
        stm :=
           'WITH '
			|| ' tPKEY AS (SELECT /*+ INLINE */ :pCODE CODE FROM DUAL),'
			|| ' tDEF AS (SELECT /*+ INLINE */ ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM TABLE(CAST(:attributeInfoTable AS DX_AttributeInfoTable)))'
			|| GetBaseSelectStm()
			;
            
        stm_materialized := DXDIR_PK_BASE_ASSESSMENT.GetStmForItemsByNames(sNAMES);

        IF stm_materialized IS NOT NULL THEN
            stm := stm || ' UNION ALL ' || stm_materialized;
        END IF;
		
		return stm;
    END;
    
    FUNCTION GetItemsByContainersStm
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
        stm_materialized    VARCHAR2(32767);
    BEGIN
            
        stm :=
           'WITH '
            || ' tPKEY AS (SELECT /*+ INLINE */ CODE, FROM TABLE(CAST(:containerKeyTable AS DXDIR_ASSESSMENTKeyTable))),'
            || ' tDEF AS (SELECT /*+ INLINE */ ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM TABLE(CAST(:attributeInfoTable AS DX_AttributeInfoTable)))'

			|| GetBaseSelectStm()
			;
			
        stm_materialized := DXDIR_PK_BASE_ASSESSMENT.GetStmForItemsByContainers();

        IF stm_materialized IS NOT NULL THEN
            stm := stm || ' UNION ALL ' || stm_materialized;
        END IF;
		
		return stm;
    END;
    
    FUNCTION GetItemsByContainersNamesStm
    (
		sNAMES              dx_pk_utility.DX_Varchar2Array
    )
    RETURN VARCHAR2
    IS
        stm                 VARCHAR2(32767);
        stm_materialized    VARCHAR2(32767);
    BEGIN  
        stm :=
           'WITH '
            || ' tPKEY AS (SELECT /*+ INLINE */ CODE, FROM TABLE(CAST(:containerKeyTable AS DXDIR_ASSESSMENTKeyTable))),'
            || ' tDEF AS (SELECT /*+ INLINE */ ID, NAME, STORAGE, SET_ID, SET_NAME, REF_SET FROM TABLE(CAST(:attributeInfoTable AS DX_AttributeInfoTable)))'
			|| GetBaseSelectStm()
			;
			
        stm_materialized := DXDIR_PK_BASE_ASSESSMENT.GetStmForItemsByContainers(sNAMES);

        IF stm_materialized IS NOT NULL THEN
            stm := stm || ' UNION ALL ' || stm_materialized;
        END IF;
        
		return stm;
    END;
    
    PROCEDURE SelectOne
    (
        sCODE				DXDIR_ASSESSMENT.CODE%TYPE,
        dcID                DX_ATTRIBUTE_DEF.ID%TYPE,
        dcARRAY_INDEX		DX_ATTVALUE.ARRAY_INDEX%TYPE,
        po_cur              OUT t_cursor
    )
    IS
		sScope                  DX_ATTRIBUTE_DEF.SCOPE%TYPE;
        dcSetID                 DX_ATTRIBUTE_DEF.ID%TYPE;
    BEGIN

        -- MATERIALIZATION: Read materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            DXM_PK_BASE.GetInfo(dcID, sScope, dcSetID);
            OPEN po_cur FOR DXDIR_PK_BASE_ASSESSMENT.GetStmForCell(dcSetID, dcID) USING sCODE, dcID, dcARRAY_INDEX;
            RETURN;
        END IF;
        
        OPEN po_cur FOR GetValueByAttributeStm() USING sCODE, dcID, dcARRAY_INDEX;
      
    END;
    
    PROCEDURE SelectAllValuesForAttribute
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        dcID		DX_ATTRIBUTE_DEF.ID%TYPE,
        po_cur      OUT t_cursor
    )
    IS
		stm                 VARCHAR2(32767);
    BEGIN
       		
       	-- MATERIALIZATION: Read materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            OPEN po_cur FOR DXDIR_PK_BASE_ASSESSMENT.GetStmForAttribute(dcID) USING sCODE, dcID;
            RETURN;
        END IF;
        	
		OPEN po_cur FOR GetValuesByAttributeStm() USING sCODE, dcID;

    END;
    
    PROCEDURE SelectAllValuesForSet
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        dcID		DX_ATTRIBUTE_DEF.ID%TYPE,
        po_cur      OUT t_cursor
    )
    IS
		stm                 VARCHAR2(32767);
    BEGIN

        -- MATERIALIZATION: Read materialized attribute first, if it is the case
        IF(DX_PK_ATTRIBUTEDEF.IsMaterialized(dcID) = 1) THEN
            OPEN po_cur FOR DXDIR_PK_BASE_ASSESSMENT.GetStmForSet(dcID) USING sCODE;
            RETURN;
        END IF;
		
		OPEN po_cur FOR GetValuesBySetStm() USING sCODE, dcID;

    END;
    
    -- Opens a cursor for reading all values of all attributes of the given container
    PROCEDURE SelectAllValuesForContainer
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        po_cur      OUT t_cursor
    )
    IS
    BEGIN
        OPEN po_cur FOR GetValuesByContainerStm(TRUE) USING sCODE;
    END;
    
    -- Opens a cursor for reading all non materialized attributes values of the given container
    PROCEDURE SelectNonMtzdValForContainer
     (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        po_cur      OUT t_cursor
    )
    IS
    BEGIN
        OPEN po_cur FOR GetValuesByContainerStm(FALSE) USING sCODE;
    END;
    
    -- Returns 1 when any values exist for the given container, otherwise 0.
    -- Master table attributes are not considered.
    -- It can be called within a SQL statement.
    FUNCTION ExistAnyValuesForContainer
	(
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE
	)
	RETURN NUMBER
	IS
		v_result NUMBER(1) := 0;
		v_cursor t_cursor;
		v_element ElementType;
		v_excludedAttributesIdsTable DX_NumberTable;
		v_count BINARY_INTEGER;
	BEGIN
		-- Collect a list of attributes IDs mapped to master tables which need to be skipped.
		SELECT ID BULK COLLECT INTO v_excludedAttributesIdsTable
		FROM DX_ATTRIBUTE_DEF 
		WHERE SCOPE = 'ASSESSMENT'
			AND STORAGE LIKE 'DXDIR_ASSESSMENT.%';

		SelectAllValuesForContainer(sCODE, v_cursor);
		
		LOOP
			FETCH v_cursor INTO v_element;
			EXIT WHEN v_cursor%NOTFOUND;
			
			-- Check whether the current attribute is in the list of excluded ones.
			SELECT COUNT(*) INTO v_count FROM THE(SELECT CAST(v_excludedAttributesIdsTable AS DX_NumberTable) FROM DUAL) WHERE COLUMN_VALUE = v_element.ID;
			IF v_count = 0 THEN
				v_result := 1;
				EXIT;
			END IF;
		END LOOP;
		CLOSE v_cursor;
		
		RETURN v_result;
	END;
   
    PROCEDURE SelectItemsByNames
    (
        sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sNAMES      dx_pk_utility.DX_Varchar2Array,
        po_cur      OUT t_cursor
    )
    IS
        attributeInfoTable  DX_AttributeInfoTable;
    BEGIN
        attributeInfoTable := DX_PK_NEWATTRIBUTESUTILITIES.NamesToAttributeInfoTable(sNAMES,'ASSESSMENT');
        
        OPEN po_cur FOR GetItemsByNamesStm(sNAMES) USING sCODE, attributeInfoTable;
    END;
  
    PROCEDURE SelectItemsByContainers
    (
        sCODE		 dx_pk_utility.DX_VARCHAR2Array,
        po_cur              OUT t_cursor
    )
    IS  
        containerKeyTable DXDIR_ASSESSMENTKeyTable;
        attributeInfoTable  DX_AttributeInfoTable;
    BEGIN
        containerKeyTable := ASSESSMENTKeyArraysToTable(sCODE);
        attributeInfoTable := DX_PK_NEWATTRIBUTESUTILITIES.AllAttrsToAttributeInfoTable('ASSESSMENT');
              
        OPEN po_cur FOR GetItemsByContainersStm() USING containerKeyTable, attributeInfoTable;

    END;
	
    PROCEDURE SelectItemsByContainers
    (
        sCODE		 dx_pk_utility.DX_VARCHAR2Array,
        sNAMES          dx_pk_utility.DX_Varchar2Array,
        po_cur          OUT t_cursor
    )
    IS
        containerKeyTable   DXDIR_ASSESSMENTKeyTable;
        attributeInfoTable  DX_AttributeInfoTable;
    BEGIN
        containerKeyTable := ASSESSMENTKeyArraysToTable(sCODE);
        attributeInfoTable := DX_PK_NEWATTRIBUTESUTILITIES.NamesToAttributeInfoTable(sNAMES,'ASSESSMENT');
        
        OPEN po_cur FOR GetItemsByContainersNamesStm(sNAMES) USING containerKeyTable, attributeInfoTable;
        
    END;
     
	PROCEDURE CopyAllValuesToContainer
    (
		sFROMCODE		DXDIR_ASSESSMENT.CODE%TYPE,
		sTOCODE			DXDIR_ASSESSMENT.CODE%TYPE
    )
    IS
      v_attributeNames dx_pk_utility.DX_Varchar2Array;
    BEGIN
		CopyAttributesToContainer(sFROMCODE, sTOCODE, v_attributeNames); 
    END;

    PROCEDURE CopyAttributesToContainer
    (
		sFROMCODE			DXDIR_ASSESSMENT.CODE%TYPE,
		sTOCODE				DXDIR_ASSESSMENT.CODE%TYPE,
		sAttributeNames IN dx_pk_utility.DX_Varchar2Array
    )
    IS
		v_cursor t_cursor;
		v_memoCursor t_cursor;
		v_element ElementType;
		v_memoElement DX_PK_NewAttValueMemo.ElementType;
		v_excludedAttributesIds DX_PK_Utility.DX_NumberArray;
		v_memoAttributesIds DX_PK_Utility.DX_NumberArray;
		v_index BINARY_INTEGER;
		v_arrayEmpty	CHAR(1) := 'N';
    BEGIN

		IF DX_PK_UTILITY.IsEmptyArray(sAttributeNames) THEN
			v_arrayEmpty := 'Y';
		END IF;

		IF v_arrayEmpty = 'N' THEN
			-- Delete existing data first.
			DeleteAttrValuesByContainer(sTOCODE, sAttributeNames);
  
			SelectItemsByNames(sFROMCODE, sAttributeNames, v_cursor);
		ELSE
			-- Delete existing data first.
			DeleteByContainer(sTOCODE);
		
			-- Collect a list of attributes IDs mapped to master tables which need to be skipped.
			SELECT ID BULK COLLECT INTO v_excludedAttributesIds
			FROM DX_ATTRIBUTE_DEF 
			WHERE SCOPE = 'ASSESSMENT'
				AND STORAGE LIKE 'DXDIR_ASSESSMENT.%';

			-- Get all values from source container.
			SelectAllValuesForContainer(sFROMCODE, v_cursor);

		END IF;
			
		-- Collect a list of memo attributes IDs because they need to be processed one by one.
		SELECT ID BULK COLLECT INTO v_memoAttributesIds
		FROM DX_ATTRIBUTE_DEF 
		WHERE SCOPE = 'ASSESSMENT'
			AND TYPE = 'MEMO';
		
		
		-- Insert values except of master table and memo attributes.
		LOOP
			FETCH v_cursor INTO v_element;
			EXIT WHEN v_cursor%NOTFOUND;

			IF v_arrayEmpty = 'Y' THEN
				-- Check whether the current attribute is in the list of excluded ones.
				v_index := DX_PK_Utility.ArrayIndexOf(v_excludedAttributesIds, v_element.ID);
			ELSE
				v_index := 0;
			END IF;

			IF v_index = 0 THEN
				-- Check whether the current attribute is a Memo attribute, becuse memos must be treated one by one.
				v_index := DX_PK_Utility.ArrayIndexOf(v_memoAttributesIds, v_element.ID);
				IF v_index <> 0 THEN
					-- Process Memo attribute because NCLOB data type cannot be used with a UNION, so no data can be loaded
					-- massively.
					DX_PK_NewAttValueMemo.SelectOne(sFROMCODE, v_element.ID, v_element.ARRAY_INDEX, v_memoCursor);
					
					FETCH v_memoCursor INTO v_memoElement;
					IF NOT v_memoCursor%NOTFOUND THEN
						DX_PK_NewAttValueMemo.BulkInsertRecord(sTOCODE, v_memoElement.NAME, v_memoElement.ID, v_memoElement.ARRAY_INDEX, v_memoElement.VALUE, v_memoElement.REF_SET);
					END IF;
					CLOSE v_memoCursor;
				ELSE
					BulkInsertRecord(sTOCODE, v_element.NAME, v_element.ID, v_element.ARRAY_INDEX, v_element.VALUE, v_element.REF_SET);
				END IF;
			END IF;
		END LOOP;
		CLOSE v_cursor;
    END;

	PROCEDURE UpdateMasterTable
    (
		sCODE			DXDIR_ASSESSMENT.CODE%TYPE,
        sATT_VALUE      NVARCHAR2,
        sATT_TYPE       VARCHAR2,
        sFIELD          VARCHAR2
    )
    IS
		v_FIELD_UNIT        VARCHAR2(30);

		vc2VALUE            VARCHAR2(4000);
        vc2UNIT_VALUE       VARCHAR2(16);

		v_updateSqlString   VARCHAR2(4000);

        notNullException     EXCEPTION;
        PRAGMA EXCEPTION_INIT(notNullException, -1407);
    BEGIN
        
		-- Prevent SQL injection on field
		AssertIsMasterTableColumn(sFIELD);

        IF sATT_VALUE IS NULL THEN
            IF sATT_TYPE='AMOUNT' OR sATT_TYPE='MEASURE' THEN
               IF sATT_TYPE='AMOUNT' THEN
                     v_FIELD_UNIT := sFIELD || '_C';
               ELSIF sATT_TYPE='MEASURE' THEN
                     v_FIELD_UNIT := sFIELD || '_UM';
               END IF;
			   
               v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = NULL, ' || v_FIELD_UNIT || '= NULL WHERE CODE = :sCODE';
            ELSE
               v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = NULL WHERE CODE = :sCODE';
            END IF;
			
			EXECUTE IMMEDIATE v_updateSqlString USING sCODE;
        ELSE
			IF sATT_TYPE='AMOUNT' OR sATT_TYPE='MEASURE' THEN
				IF sATT_TYPE='AMOUNT' THEN
					v_FIELD_UNIT := sFIELD || '_C';
				ELSIF sATT_TYPE='MEASURE' THEN
					v_FIELD_UNIT := sFIELD || '_UM';
				END IF;

				dx_pk_NewAttributesUtilities.GetAggregateValueTokens(TRANSLATE(sATT_VALUE USING CHAR_CS), vc2VALUE, vc2UNIT_VALUE);
				v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = TO_NUMBER(:vc2VALUE), ' || v_FIELD_UNIT || ' = :vc2UNIT_VALUE WHERE CODE = :sCODE';
				
				EXECUTE IMMEDIATE v_updateSqlString USING vc2VALUE, vc2UNIT_VALUE, sCODE;
            ELSE
				IF sATT_TYPE='STRING' OR sATT_TYPE='LISTOFVAL' OR sATT_TYPE='MEMO' THEN
					v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = :sATT_VALUE WHERE CODE = :sCODE';
				ELSIF sATT_TYPE = 'NUMBER' OR sATT_TYPE = 'BOOL' THEN
					v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = TO_NUMBER(TRANSLATE(:sATT_VALUE USING CHAR_CS)) WHERE CODE = :sCODE';
				ELSIF sATT_TYPE = 'DATETIME' THEN
					v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = TO_DATE(TRANSLATE(:sATT_VALUE USING CHAR_CS), ''YYYYMMDDHH24MISS'') WHERE CODE = :sCODE';
				ELSIF sATT_TYPE = 'DATE' THEN
					v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = TO_DATE(SUBSTR(TRANSLATE(:sATT_VALUE USING CHAR_CS),0,8), ''YYYYMMDD'') WHERE CODE = :sCODE';
				ELSIF sATT_TYPE = 'TIME' THEN
					v_updateSqlString := 'UPDATE DXDIR_ASSESSMENT SET ' || sFIELD || ' = TO_DATE(TRANSLATE(:sATT_VALUE USING CHAR_CS), ''YYYYMMDDHH24MISS'') WHERE CODE = :sCODE';
				END IF;

				EXECUTE IMMEDIATE v_updateSqlString USING sATT_VALUE, sCODE;
			END IF;
          
        END IF;

    EXCEPTION
        WHEN notNullException THEN
            RETURN;
    END;

	FUNCTION ExistsOnMasterTable
    (
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sFIELD      VARCHAR2
    )
    RETURN NUMBER
    IS
		v_exists NUMBER(1) := 0;
    BEGIN
		-- Prevent SQL injection on field
		AssertIsMasterTableColumn(sFIELD);

        EXECUTE IMMEDIATE 'SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE FROM  DXDIR_ASSESSMENT WHERE CODE = :sCODE ' || sFIELD || ' IS NOT NULL' INTO v_exists;
		 
        RETURN v_exists;
    END;

	FUNCTION ExistsValueOnMasterTable
    (
		sCODE		DXDIR_ASSESSMENT.CODE%TYPE,
        sFIELD      VARCHAR2,
        sATT_TYPE	VARCHAR2,
        sATT_VALUE	DX_ATTVALUE.VALUE%TYPE
    )
    RETURN NUMBER
    IS
        v_exists NUMBER(1) := 0;
        v_statement VARCHAR2(32767);
    BEGIN
		-- Prevent SQL injection on field
		AssertIsMasterTableColumn(sFIELD);

        v_statement := 'SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE FROM DXDIR_ASSESSMENT WHERE ';
       
        IF sATT_VALUE IS NULL THEN
			v_statement := v_statement || sFIELD || ' IS NULL AND CODE = :sCODE';
			EXECUTE IMMEDIATE v_statement INTO v_exists USING sCODE;
		ELSE
			CASE sATT_TYPE
				WHEN 'AMOUNT' THEN
					v_statement := v_statement || sFIELD || ' = SUBSTR(:sATT_VALUE, 1, INSTR(:sATT_VALUE, ''|'') - 1) AND CODE = :sCODE';
				WHEN 'MEASURE' THEN
					v_statement := v_statement || sFIELD || ' = SUBSTR(:sATT_VALUE, 1, INSTR(:sATT_VALUE, ''|'') - 1) AND CODE = :sCODE';
				WHEN 'NUMBER' THEN
					v_statement := v_statement || sFIELD || ' = :sATT_VALUE AND CODE = :sCODE';
				WHEN 'DATETIME' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(:sATT_VALUE, ''YYYYMMDDHH24MISS'') AND CODE = :sCODE';
				WHEN 'TIME' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(:sATT_VALUE, ''YYYYMMDDHH24MISS'') AND CODE = :sCODE';
				WHEN 'DATE' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(SUBSTR(:sATT_VALUE,0,8), ''YYYYMMDD'') AND CODE = :sCODE';
				ELSE
					v_statement := v_statement || sFIELD || ' = :sATT_VALUE AND CODE = :sCODE';
			END CASE;			
	
			EXECUTE IMMEDIATE v_statement INTO v_exists USING sATT_VALUE, sCODE;
		END IF;

        RETURN v_exists;
    END;

	FUNCTION ExistsValueOnMasterTableAnyCnt
    (
		sFIELD              VARCHAR2,
        sATT_TYPE			VARCHAR2,
        sATT_VALUE			DX_ATTVALUE.VALUE%TYPE
    )
    RETURN NUMBER
    IS
		v_exists NUMBER(1) := 0;
        v_statement VARCHAR2(32767);
    BEGIN
		-- Prevent SQL injection on field
		AssertIsMasterTableColumn(sFIELD);
		
        v_statement := 'SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END CASE FROM DXDIR_ASSESSMENT WHERE ';
       
        IF sATT_VALUE IS NULL THEN
			v_statement := v_statement || sFIELD || ' IS NULL';
			EXECUTE IMMEDIATE v_statement INTO v_exists;
		ELSE
			CASE sATT_TYPE
				WHEN 'AMOUNT' THEN
					v_statement := v_statement || sFIELD || ' = SUBSTR(:sATT_VALUE, 1, INSTR(:sATT_VALUE, ''|'') - 1)';
				WHEN 'MEASURE' THEN
					v_statement := v_statement || sFIELD || ' = SUBSTR(:sATT_VALUE, 1, INSTR(:sATT_VALUE, ''|'') - 1)';
				WHEN 'NUMBER' THEN
					v_statement := v_statement || sFIELD || ' = :sATT_VALUE';
				WHEN 'DATETIME' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(:sATT_VALUE, ''YYYYMMDDHH24MISS'')';
				WHEN 'TIME' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(:sATT_VALUE, ''YYYYMMDDHH24MISS'')';
				WHEN 'DATE' THEN
					v_statement := v_statement || sFIELD || ' = TO_DATE(SUBSTR(:sATT_VALUE,0,8), ''YYYYMMDD'')';
				ELSE
					v_statement := v_statement || sFIELD || ' = :sATT_VALUE';
			END CASE;			
	
			EXECUTE IMMEDIATE v_statement INTO v_exists USING sATT_VALUE;
		END IF;

        RETURN v_exists;
    END;

	PROCEDURE AssertIsMasterTableColumn
	(
		sCOLUMN_NAME		VARCHAR2
	)
	IS
    BEGIN
		IF sCOLUMN_NAME NOT IN 
		('CODE', 'TYPE_CODE', 'DESCRIPTION', 'STATUS', 'COMPANY_NAME', 'COMPLETING_BY', 'PHONE', 'EMAIL', 'TIMEFRAME_FROM', 'TIMEFRAME_TO', 'CREATE_DATE', 'CREATED_BY', 'MOD_DATE', 'MODIFIED_BY', 'AUTHORIZATION_ROLE', 'ORG_STRUCTURE', 'LOCATION', 'PROD_CLASSIF') THEN
			raise_application_error(-20001, 'Invalid column name');
		END IF;
    END;
    
END;
/