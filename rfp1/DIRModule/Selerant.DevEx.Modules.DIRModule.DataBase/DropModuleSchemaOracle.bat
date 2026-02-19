@ECHO off
ECHO * [Oracle] Dropping all DIR module DB artifacts...
ECHO ****************************************************************************************************

echo exit | sqlplus DIR_MODULE/devex123@BLSRV-Oracle03:1522/orcl12c @DropModuleSchemaOracle.sql

REM if arg is not supplied pause script
IF [%1]==[] GOTO pauseScript

IF %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%
GOTO finish

:pauseScript
pause

:finish