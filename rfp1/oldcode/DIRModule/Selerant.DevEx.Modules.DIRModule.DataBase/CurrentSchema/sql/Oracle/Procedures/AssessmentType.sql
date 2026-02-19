CREATE OR REPLACE PACKAGE DXDIR_PK_ASSESSMENT_TYPE AS
   
    PROCEDURE InsertRecord
    (
        sCODE VARCHAR2,
        sDESCRIPTION NVARCHAR2,
		dcACTIVE NUMBER
    );

    PROCEDURE UpdateRecord
    (
        sCODE VARCHAR2,
        sDESCRIPTION NVARCHAR2,
		dcACTIVE NUMBER
    );

    PROCEDURE DeleteRecord
    (
        sCODE VARCHAR2
    );

END;
/

CREATE OR REPLACE PACKAGE BODY DXDIR_PK_ASSESSMENT_TYPE AS

    PROCEDURE InsertRecord
    (
        sCODE VARCHAR2,
        sDESCRIPTION NVARCHAR2,
		dcACTIVE NUMBER
    )
    IS
    BEGIN 
        INSERT INTO DXDIR_ASSESSMENT_TYPE
        (
			CODE,
			DESCRIPTION,
			ACTIVE
        )
        VALUES
        (
			sCODE,
			sDESCRIPTION,
			dcACTIVE
        );
    END;

    PROCEDURE UpdateRecord
    (
        sCODE VARCHAR2,
        sDESCRIPTION NVARCHAR2,
		dcACTIVE NUMBER
    )
    IS
    BEGIN
        UPDATE DXDIR_ASSESSMENT_TYPE
        SET 
            DESCRIPTION = sDESCRIPTION,
			ACTIVE = dcACTIVE
        WHERE
            CODE = sCODE;
    END;

    PROCEDURE DeleteRecord
    (
        sCODE VARCHAR2
    )
    IS
    BEGIN
        DELETE FROM DXDIR_ASSESSMENT_TYPE
        WHERE
            CODE = sCODE;
    END;
END;
/