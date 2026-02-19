spool DropDIRmoduleOracle.sql.log

set serveroutput on
set termout on
-- suppress result of operation message (e.g. "Table Created")
set feedback off 
-- suppress echo of commands (e.g. "CREATE OR REPLACE PACKAGE ...")
set echo off

set sqlblanklines on;

WHENEVER SQLERROR EXIT SQL.SQLCODE

-- For reinserting Module Info on module creation
DELETE FROM DX_MODULE_INFO
WHERE CODE = 'DIR';
COMMIT;

-- https://gist.github.com/jirkapenzes/d5c1b1659c6ab7952449
BEGIN
	FOR cur_rec IN (SELECT object_name, object_type
					FROM user_objects
					WHERE object_name like 'DXDIR_%'
					AND object_type IN
								('TABLE',
								'VIEW',
								'PACKAGE',
								'PROCEDURE',
								'FUNCTION',
								'SEQUENCE'
								))
	LOOP
		BEGIN
			IF cur_rec.object_type = 'TABLE'
			THEN
				EXECUTE IMMEDIATE 'DROP '
									|| cur_rec.object_type
									|| ' "'
									|| cur_rec.object_name
									|| '" CASCADE CONSTRAINTS';
			ELSE
				EXECUTE IMMEDIATE 'DROP '
									|| cur_rec.object_type
									|| ' "'
									|| cur_rec.object_name
									|| '"';
			END IF;
		END;
	END LOOP;

	-- drop types
	FOR cur_rec IN (SELECT type_name FROM user_types
				    WHERE type_name like 'DXDIR_%'
					ORDER BY typecode asc)
	LOOP
		EXECUTE IMMEDIATE 'DROP TYPE "' || cur_rec.type_name || '"';
	END LOOP;
END;
/


DELETE FROM DX_COMPONENT_CONTROLLER
WHERE ID in (319,320,321) or PARENT_ID in (319,320,321);

DELETE FROM DX_RECENT_MATERIAL;

DELETE FROM DX_ATTVALUE_MATERIAL
WHERE ID = 99510;

-- Dropping created materials for DIR and their types
DELETE FROM DX_MATERIAL
WHERE MAT_TYPE in ('DIR_INPUT', 'DIR_OUTPUT', 'DIR_RESOURCE');
COMMIT;

DELETE FROM DX_MATERIAL_TYPE
WHERE CODE in ('DIR_INPUT', 'DIR_OUTPUT', 'DIR_RESOURCE');
COMMIT;

-- Dropping material types PHRASEs
-- TEXT: Material type
DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 11 and SUBTYPE = 'DX_MATERIAL.MAT_TYPE' and CODE in ('DIR_INPUT', 'DIR_OUTPUT', 'DIR_RESOURCE');

-- DEF: Material type
DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 11 and SUBTYPE = 'DX_MATERIAL.MAT_TYPE' and CODE in ('DIR_INPUT', 'DIR_OUTPUT', 'DIR_RESOURCE');
COMMIT;
/

DECLARE
	TYPE t_phraseTable IS TABLE OF VARCHAR2(128) INDEX BY BINARY_INTEGER;
	phaseTables t_phraseTable;
	subtypes_in VARCHAR2(2000);
BEGIN
	phaseTables(1) := 'DX_PHRASE_TEXT';
	phaseTables(2) := 'DX_PHRASE_DEF';
	phaseTables(3) := 'DX_PHRASE_TYPE';

	subtypes_in :=
		'''DXDIR_ASSESSMENT.STATUS'',' ||
		'''DXDIR_INPUT_CATEGORY.TYPE'',' ||
		'''DXDIR_ASSESSMENT.ORG_STRUCTURE'',' ||
		'''DXDIR_BUSINESS_COST.TITLE'',' ||
		'''DXDIR_RESULT.TITLE'',' ||
		'''DXDIR_ASSESSMENT.PROD_CLASSIF''';

	FOR i IN 1 .. phaseTables.COUNT
	LOOP

		EXECUTE IMMEDIATE 'DELETE FROM ' ||
							phaseTables(i) || 
							' WHERE TYPE = 11 and SUBTYPE in (' ||
							subtypes_in ||
							')';
	END LOOP;
END;
/

DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 11 AND SUBTYPE = 'DX_ATTRIBUTE_DEF.SCOPE' AND CODE = 'ASSESSMENT';

DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 11 AND SUBTYPE = 'DX_ATTRIBUTE_DEF.SCOPE' AND CODE = 'ASSESSMENT';

-- Attribute Phrases
DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 10 AND SUBTYPE = '0' AND CODE = '99510';

DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 10 AND SUBTYPE = '0' AND CODE = '99511';

DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 4 AND SUBTYPE = '99510' AND CODE IN ('FOOD', 'NONFOOD');

DELETE FROM DX_PHRASE_TEXT
WHERE TYPE = 4 AND SUBTYPE = '99511' AND CODE IN ('1', '2', '3');

DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 10 AND SUBTYPE = '0' AND CODE = '99510';

DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 10 AND SUBTYPE = '0' AND CODE = '99511';

DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 4 AND SUBTYPE = '99510' AND CODE IN ('FOOD', 'NONFOOD');

DELETE FROM DX_PHRASE_DEF
WHERE TYPE = 4 AND SUBTYPE = '99511' AND CODE IN ('1', '2' , '3');

DELETE FROM DX_PHRASE_TYPE
WHERE TYPE = 4 AND SUBTYPE = '99510';

DELETE FROM DX_PHRASE_TYPE
WHERE TYPE = 4 AND SUBTYPE = '99511';

-- DXDIR_ASSESSMENT_COMMENT, DXDIR_RESOURCE_TYPE, DXDIR_DATA_QUALITY
DELETE FROM DX_ATTRIBUTE_DEF
WHERE ID IN (99500, 99510, 99511);

COMMIT;
/

spool off