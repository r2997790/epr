@ECHO off 

ECHO ****************************************************************************************************
ECHO * Droping DIR module schema

CALL DropModuleSchemaOracle.bat 1

IF %ERRORLEVEL% NEQ 0 ECHO Error: %ERRORLEVEL%
IF %ERRORLEVEL% NEQ 0 GOTO wait

ECHO ****************************************************************************************************
ECHO * Recreating DIR module schema

IF %ERRORLEVEL% == 0 CALL CreateSchemaOracle.bat
IF %ERRORLEVEL% == 0 GOTO end

:wait
pause

:end