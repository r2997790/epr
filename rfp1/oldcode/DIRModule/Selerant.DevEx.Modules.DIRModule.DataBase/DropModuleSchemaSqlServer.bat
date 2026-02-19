@ECHO off
ECHO * [SQLServer] Dropping all DIR module DB artifacts...
ECHO ****************************************************************************************************
REM -o DropModuleSchemaSqlServer.log 
sqlcmd -S BLSRV-MSSQL02\DXMSSQL2014 -d devex_main_dir -U DEVEX_MAIN_DIR -P devex123 -i DropModuleSchemaSqlServer.sql -b -r1 1> NUL

REM if arg is not supplied pause script
IF [%1]==[] GOTO pauseScript

IF %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%
GOTO finish

:pauseScript
pause

:finish