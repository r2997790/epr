CREATE OR REPLACE PACKAGE DXDIR_PK_BUSINESS_COST AS

	TYPE t_cursor IS REF CURSOR;

	PROCEDURE InsertRecord
	(
		dcID NUMBER,
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		sTYPE VARCHAR2,
		sTITLE NVARCHAR2,
		dcSORT_ORDER NUMBER,
		dcCOST NUMBER
	);

	PROCEDURE UpdateRecord
	(
		dcID NUMBER,
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		sTYPE VARCHAR2,
		sTITLE NVARCHAR2,
		dcSORT_ORDER NUMBER,
		dcCOST NUMBER
	);

	PROCEDURE DeleteRecord
	(
		dcID NUMBER
	);

	PROCEDURE GetNextId
	(
        dcNextID OUT NUMBER
    );

	PROCEDURE GetBusinessCostGridItems
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
        tblPBC_CARRIED DXDIR_BizCostCarriedKeyTable,
		po_cur IN OUT t_cursor
	);

    FUNCTION GetLcStageCarriedOverSequence
    (
        sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER
    )
    RETURN DX_NumberTable;

	PROCEDURE GetCarriedOverBusinessCosts
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		po_cur OUT t_cursor
	);

END;
/

CREATE OR REPLACE PACKAGE BODY DXDIR_PK_BUSINESS_COST AS

	PROCEDURE InsertRecord
	(
		dcID NUMBER,
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		sTYPE VARCHAR2,
		sTITLE NVARCHAR2,
		dcSORT_ORDER NUMBER,
		dcCOST NUMBER
	)
	IS
	BEGIN
		INSERT INTO DXDIR_BUSINESS_COST
		(
			ID,
			ASSESSMENT_CODE,
			ASSESSMENT_LC_STAGE_ID,
			TYPE,
			TITLE,
			SORT_ORDER,
			COST
		)
		VALUES
		(
			dcID,
			sASSESSMENT_CODE,
			dcASSESSMENT_LC_STAGE_ID,
			sTYPE,
			sTITLE,
			dcSORT_ORDER,
			dcCOST
		);
	END;

	PROCEDURE UpdateRecord
	(
		dcID NUMBER,
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		sTYPE VARCHAR2,
		sTITLE NVARCHAR2,
		dcSORT_ORDER NUMBER,
		dcCOST NUMBER
	)
	IS
	BEGIN
		UPDATE DXDIR_BUSINESS_COST
		SET 
			ASSESSMENT_CODE = sASSESSMENT_CODE,
			ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID,
			TYPE = sTYPE,
			TITLE = sTITLE,
			SORT_ORDER = dcSORT_ORDER,
			COST = dcCOST
		WHERE 
			ID = dcID;
	END;

	PROCEDURE DeleteRecord
	(
		dcID NUMBER
	)
	IS
	BEGIN
		DELETE FROM DXDIR_BUSINESS_COST
		WHERE 
			ID = dcID;
	END;

	PROCEDURE GetNextId
	(
        dcNextID OUT NUMBER
    )
	IS
	BEGIN
		SELECT DXDIR_BUSINESS_COST_SEQ.NEXTVAL INTO dcNextID FROM DUAL;
	END;

	PROCEDURE GetBusinessCostGridItems
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
        tblPBC_CARRIED DXDIR_BizCostCarriedKeyTable,
		po_cur IN OUT t_cursor
	)
	IS
		totalMass NUMBER(25, 3);
		factor NUMBER(25, 12);

		coProductMargin NUMBER(25, 12);
		coProductWeight	NUMBER(25, 3);

		coProductSecondMargin NUMBER(25, 12);
		coProductSecondWeight NUMBER(25, 3);

		foodRescueMargin NUMBER(25, 12);
		foodRescueWeight NUMBER(25, 3);

        outputRevenueFactor NUMBER(25, 14);

		v_COPRODUCT_DEST_CODE DXDIR_OUTPUT.DESTINATION_CODE%TYPE := 'COPRODUCT';
		v_COPRODUCT_2ND_DEST_CODE DXDIR_OUTPUT.DESTINATION_CODE%TYPE := 'COPRODUCT_2';
		v_FOOD_RESCUE_DEST_CODE DXDIR_OUTPUT.DESTINATION_CODE%TYPE := 'FOOD_RESCUE';
	BEGIN
		DXDIR_PK_CALCULATIONS.GetTotalMass(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, totalMass);

		IF (totalMass = 0)
		THEN
			factor := 0;
            outputRevenueFactor := 0;
		ELSE
			-- massOfWaste / totalInputMass
			SELECT ROUND((NVL(SUM(WEIGHT), 0) / totalMass), 12)
			INTO factor
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE = sASSESSMENT_CODE
			  AND ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID
			  AND WEIGHT IS NOT NULL
			  AND OUTPUT_CATEGORY_ID <> 1;
              
            outputRevenueFactor := DXDIR_PK_CALCULATIONS.GetOutputRevenueFactor(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, totalMass);
		END IF;

		DXDIR_PK_CALCULATIONS.GetMargin(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, v_COPRODUCT_DEST_CODE, coProductMargin);
		DXDIR_PK_CALCULATIONS.GetMargin(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, v_COPRODUCT_2ND_DEST_CODE, coProductSecondMargin);
		DXDIR_PK_CALCULATIONS.GetMargin(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, v_FOOD_RESCUE_DEST_CODE, foodRescueMargin);
        
		BEGIN
			SELECT NVL(WEIGHT, 0)
			INTO coProductWeight
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE		 = sASSESSMENT_CODE			AND
				  ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID	AND
				  DESTINATION_CODE	     = v_COPRODUCT_DEST_CODE;
		EXCEPTION
			WHEN NO_DATA_FOUND THEN
				coProductWeight := 0;
		END;

		BEGIN
			SELECT NVL(WEIGHT, 0)
			INTO coProductSecondWeight
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE		 = sASSESSMENT_CODE			AND
				  ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID	AND
				  DESTINATION_CODE	     = v_COPRODUCT_2ND_DEST_CODE;
		EXCEPTION
			WHEN NO_DATA_FOUND THEN
				coProductSecondWeight := 0;
		END;

		BEGIN
			SELECT NVL(WEIGHT, 0)
			INTO foodRescueWeight
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE		 = sASSESSMENT_CODE			AND
				  ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID	AND
				  DESTINATION_CODE	     = v_FOOD_RESCUE_DEST_CODE;
		EXCEPTION
			WHEN NO_DATA_FOUND THEN
				foodRescueWeight := 0;
		END;    
        
        OPEN po_cur FOR
            WITH filteredBusinessCost AS
            (
                SELECT ID, TITLE, SORT_ORDER, COST    
                FROM DXDIR_BUSINESS_COST
                WHERE ASSESSMENT_CODE		   = sASSESSMENT_CODE	AND
                      ASSESSMENT_LC_STAGE_ID   = dcASSESSMENT_LC_STAGE_ID
            ),
            joinedWithPCBCarried AS
            (
                SELECT 
                    COALESCE(BC.ID, -CR.SORT_ORDER)            ID,
                    COALESCE(BC.TITLE, CR.TITLE_PREV)          TITLE,
                    COALESCE(BC.SORT_ORDER, -CR.SORT_ORDER)    SORT_ORDER,
                    BC.COST                                    COST,
                    NVL(BC.COST, 0) + NVL(CR.PBC_COST_PREV, 0) TOTAL_COST
                FROM filteredBusinessCost BC
                FULL OUTER JOIN TABLE(tblPBC_CARRIED) CR
                ON BC.TITLE = CR.TITLE_PREV
            )
            SELECT
                ID,
                sASSESSMENT_CODE ASSESSMENT_CODE,
                dcASSESSMENT_LC_STAGE_ID ASSESSMENT_LC_STAGE_ID,
                NULL TYPE,
                TITLE,
                SORT_ORDER,
                COST,
                TOTAL_COST,
                DXDIR_PK_CALCULATIONS.GetWasteRelatedCost(totalMass, factor, TOTAL_COST,
				                                             coProductMargin, coProductWeight,
															 coProductSecondMargin, coProductSecondWeight,
															 foodRescueMargin, foodRescueWeight,
															 outputRevenueFactor) WASTE_COST
                FROM joinedWithPCBCarried;
	END;
    
    FUNCTION GetLcStageCarriedOverSequence
    (
        sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER
    )
    RETURN DX_NumberTable
    IS
        dcRESULT DX_NumberTable;
        idxResult NUMBER;
        
        arrIds DX_PK_Utility.DX_NumberArray;
        arrSource_ids DX_PK_Utility.DX_NumberArray;
        
        TYPE r_IdSourceIdPair IS RECORD
        (
            ID         DXDIR_ASSESSMENT_LC_STAGE.ID%TYPE,
            SOURCE_ID  DXDIR_ASSESSMENT_LC_STAGE.SOURCE_ASMT_LC_STAGE_ID%TYPE
        );
        currPair r_IdSourceIdPair;
        nextPair r_IdSourceIdPair;
    BEGIN
        dcRESULT := DX_NumberTable();
    
        SELECT            ID,     SOURCE_ASMT_LC_STAGE_ID 
        BULK COLLECT INTO arrIds, arrSource_ids
        FROM DXDIR_ASSESSMENT_LC_STAGE
        WHERE ASSESSMENT_CODE = sASSESSMENT_CODE
          AND SORT_ORDER <= (SELECT SORT_ORDER FROM DXDIR_ASSESSMENT_LC_STAGE WHERE ID = dcASSESSMENT_LC_STAGE_ID)
        ORDER BY SORT_ORDER DESC;
        
        IF arrIds.COUNT > 0
        THEN
            idxResult := 0;
            
            FOR i IN 1..arrIds.COUNT
            LOOP
                currPair.ID        := arrIds(i);
                currPair.SOURCE_ID := arrSource_ids(i);
            
                IF i < arrIds.COUNT
                THEN
                    nextPair.ID        := arrIds(i + 1);
                    nextPair.SOURCE_ID := arrSource_ids(i + 1);
                END IF;
            
                IF ((nextPair.ID IS NOT NULL) AND (nextPair.ID = currPair.SOURCE_ID)) THEN
                    IF i = 1 THEN
                        dcRESULT.EXTEND;
                        idxResult := idxResult + 1;
                        dcRESULT(idxResult) := currPair.ID;
                    END IF;
                        
                    dcRESULT.EXTEND;
                    idxResult := idxResult + 1;
                    dcRESULT(idxResult) := nextPair.ID;
                    
                ELSIF i = 1 THEN
                
                    dcRESULT.EXTEND;
                    idxResult := idxResult + 1;
                    dcRESULT(idxResult) := currPair.ID; -- allways add starting stage id
                    EXIT; 
                ELSE
                    EXIT;
                END IF;
                
                nextPair := NULL;
                        
            END LOOP;
        END IF;        

        RETURN dcRESULT;  
    END;

	PROCEDURE GetCarriedOverBusinessCosts
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		po_cur OUT t_cursor
	)
	IS
        v_AssessmentLcStageIds DX_NumberTable;
        lcStagesCount NUMBER;
        
        v_itterationCounter NUMBER := 0;
        
        TYPE r_BussinessCostGridItem IS RECORD
		(
			ID					   DXDIR_BUSINESS_COST.ID%TYPE,
			ASSESSMENT_CODE		   DXDIR_BUSINESS_COST.ASSESSMENT_CODE%TYPE,
			ASSESSMENT_LC_STAGE_ID DXDIR_BUSINESS_COST.ASSESSMENT_LC_STAGE_ID%TYPE,
			TYPE				   DXDIR_BUSINESS_COST.TYPE%TYPE,
			TITLE				   DXDIR_BUSINESS_COST.TITLE%TYPE,
			SORT_ORDER			   DXDIR_BUSINESS_COST.SORT_ORDER%TYPE,
			COST				   DXDIR_BUSINESS_COST.COST%TYPE,
            TOTAL_COST             NUMBER(26, 4),
			WASTE_COST			   NUMBER(26, 4)
		);
		PBCrec r_BussinessCostGridItem;
        
        PBCsCarried DXDIR_BizCostCarriedKeyTable := DXDIR_BizCostCarriedKeyTable();
        idx_PBCsCarried NUMBER;
	BEGIN
        v_AssessmentLcStageIds := GetLcStageCarriedOverSequence(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID);
         lcStagesCount := v_AssessmentLcStageIds.COUNT; -- used to exit loop when final stage is calculated
        
        IF lcStagesCount > 0 THEN 
            FOR asmtLcStageId IN (
                SELECT ID FROM DXDIR_ASSESSMENT_LC_STAGE
                WHERE ASSESSMENT_CODE = sASSESSMENT_CODE AND
                      ID IN (SELECT COLUMN_VALUE FROM TABLE(v_AssessmentLcStageIds))
                ORDER BY SORT_ORDER ASC
            )
            LOOP
                IF (v_itterationCounter = 0) THEN
                    DXDIR_PK_BUSINESS_COST.GetBusinessCostGridItems(SASSESSMENT_CODE, asmtLcStageId.ID, PBCsCarried, po_cur);
                ELSE                            
                    DXDIR_PK_BUSINESS_COST.GetBusinessCostGridItems(SASSESSMENT_CODE, asmtLcStageId.ID, PBCsCarried, po_cur);
                    PBCsCarried := DXDIR_BizCostCarriedKeyTable();
                END IF;
                            
                v_itterationCounter := v_itterationCounter + 1;
                
                IF (v_itterationCounter = lcStagesCount) THEN
                    PBCsCarried.DELETE;
                    -- exit, at this point po_cur is holding target stage results (see GetBusinessCostGridItems for columns)
                    EXIT;
                END IF;
                
                idx_PBCsCarried := 1;
                 -- Build table var of carried cost to be used in next LC stage
                LOOP
                    FETCH po_cur INTO PBCrec;
                    EXIT WHEN po_cur%NOTFOUND;
                    
                    PBCsCarried.EXTEND;
                    PBCsCarried(idx_PBCsCarried) := DXDIR_BizCostCarriedKey(idx_PBCsCarried, PBCrec.TITLE, (PBCrec.TOTAL_COST - PBCrec.WASTE_COST));   
                    idx_PBCsCarried := idx_PBCsCarried + 1;
                END LOOP;
    
            END LOOP;
        END IF;
        
    END;

END;
/
