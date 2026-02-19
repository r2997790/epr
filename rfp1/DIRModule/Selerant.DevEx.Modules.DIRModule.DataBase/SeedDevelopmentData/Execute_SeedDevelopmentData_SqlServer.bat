@ECHO off
ECHO * Generating Oracle seeeding dev data...
ECHO ****************************************************************************************************

SET workingPath=%cd%
SET logFilePath="%workingPath%\_Output\Execute_SeedDevelopmentData_SqlServer.log"

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON

dbmanager single --clean -n "SeedDevelopmentData.cs" -m "%workingPath%" -p "SqlServer|DEVEX_MAIN_ARD/devex123@BLSRV-MSSQL02\DXMSSQL2014" -s --execute --logFileMode "CreateNew" --logfile %logFilePath%

@ECHO OFF
popd
pause
