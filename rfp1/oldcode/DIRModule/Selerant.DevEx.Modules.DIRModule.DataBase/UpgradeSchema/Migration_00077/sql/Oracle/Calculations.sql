CREATE OR REPLACE PACKAGE DXDIR_PK_CALCULATIONS AS

	PROCEDURE GetTotalMass
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcTotalMass OUT NUMBER
	);

	FUNCTION GetValuePerMass
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcOUTPUT_CATEGORY_ID NUMBER
	)
    RETURN NUMBER;

	PROCEDURE GetMargin
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcOUTPUT_CATEGORY_ID NUMBER,
		dcMargin OUT NUMBER
	);
    
    FUNCTION GetOutputRevenueFactor
    (
        sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
        totalInputMass NUMBER
    )
    RETURN NUMBER;  

	FUNCTION GetWasteRelatedCost
	(
		dcTotalMass	NUMBER,
		dcFactor NUMBER,
		dcCost NUMBER,
		dcCoProductMargin NUMBER,
		dcCoProductWeight NUMBER,
		dcFoodRescueMargin NUMBER,
		dcFoodRescueWeight NUMBER,
        dcOutputRevenueFactor NUMBER
	)
	RETURN NUMBER;

END;
/

CREATE OR REPLACE PACKAGE BODY DXDIR_PK_CALCULATIONS AS 

	PROCEDURE GetTotalMass
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcTotalMass OUT NUMBER
	)
	IS
	BEGIN
		SELECT NVL(SUM(MASS), 0)
		INTO dcTotalMass
		FROM DXDIR_INPUT
		WHERE ASSESSMENT_CODE		 = sASSESSMENT_CODE AND
				ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID;
	END;

    -- Use for single row of output Product, CoProduct and Food Rescue
	FUNCTION GetValuePerMass
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcOUTPUT_CATEGORY_ID NUMBER
	)
    RETURN NUMBER
	IS
		resultValuePerMass NUMBER(26, 12) := 0;
	BEGIN
		BEGIN
			-- TODO: this needs to be confirmed
			SELECT SUM(NVL(INCOME, 0)) / SUM(WEIGHT)
			INTO resultValuePerMass
			FROM DXDIR_OUTPUT
			WHERE ASSESSMENT_CODE		 = sASSESSMENT_CODE			AND
				  ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID	AND
				  OUTPUT_CATEGORY_ID	 = dcOUTPUT_CATEGORY_ID     AND
                  NVL(WEIGHT, 0)         <> 0;
			
		EXCEPTION
			WHEN NO_DATA_FOUND THEN
				resultValuePerMass := 0;
		END;
        
        RETURN resultValuePerMass;
	END;

	PROCEDURE GetMargin
	(
		sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
		dcOUTPUT_CATEGORY_ID NUMBER,
		dcMargin OUT NUMBER
	)
	IS
		productValuePerMass NUMBER(25, 12);
		valuePerMass NUMBER(25, 12);
	BEGIN
		IF (dcOUTPUT_CATEGORY_ID = 1)
		THEN
			dcMargin := 1;
		ELSIF (dcOUTPUT_CATEGORY_ID IN (2, 3)) -- COPRODUCT, FOOD_RESCUE
		THEN
            productValuePerMass := GetValuePerMass(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, 1); 
        
			IF (productValuePerMass = 0)
			THEN
				dcMargin := 0;
			ELSE
                valuePerMass := GetValuePerMass(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, dcOUTPUT_CATEGORY_ID);
				dcMargin := valuePerMass / productValuePerMass;
			END IF;
		ELSE
			dcMargin := 0;
		END IF;
	END;

    FUNCTION GetOutputRevenueFactor
    (
        sASSESSMENT_CODE VARCHAR2,
		dcASSESSMENT_LC_STAGE_ID NUMBER,
        totalInputMass NUMBER
    )
    RETURN NUMBER
    IS
        result NUMBER(25, 14) := 0;
        productValuePerMass NUMBER(26, 12);
    BEGIN
        productValuePerMass := DXDIR_PK_CALCULATIONS.GetValuePerMass(sASSESSMENT_CODE, dcASSESSMENT_LC_STAGE_ID, 1);

        IF (productValuePerMass <> 0)
        THEN
            SELECT NVL(SUM((SUM(WEIGHT) / totalInputMass) * SUM(INCOME) / SUM(WEIGHT) / productValuePerMass), 0)
            INTO result
            FROM DXDIR_OUTPUT
            WHERE ASSESSMENT_CODE = sASSESSMENT_CODE
              AND ASSESSMENT_LC_STAGE_ID = dcASSESSMENT_LC_STAGE_ID
              AND OUTPUT_CATEGORY_ID in (4,5,6)
                GROUP BY OUTPUT_CATEGORY_ID, DESTINATION_CODE
                HAVING SUM(INCOME) > 0 AND SUM(WEIGHT) > 0;
        END IF;
        
        RETURN result; 
    END;
    

	FUNCTION GetWasteRelatedCost
	(
		dcTotalMass	NUMBER,
		dcFactor NUMBER,
		dcCost NUMBER,
		dcCoProductMargin NUMBER,
		dcCoProductWeight NUMBER,
		dcFoodRescueMargin NUMBER,
		dcFoodRescueWeight NUMBER,
        dcOutputRevenueFactor NUMBER
	)
	RETURN NUMBER
	IS
		result NUMBER(26, 4) := 0;
	BEGIN
		IF (dcTotalMass <> 0)
		THEN
			result := (dcCost * dcFactor) - ((dcCoProductWeight / dcTotalMass) * dcCost * dcCoProductMargin) - ((dcFoodRescueWeight / dcTotalMass) * dcCost * dcFoodRescueMargin) - (dcOutputRevenueFactor * dcCost);
		END IF;

		RETURN result;
	END;
END;
/