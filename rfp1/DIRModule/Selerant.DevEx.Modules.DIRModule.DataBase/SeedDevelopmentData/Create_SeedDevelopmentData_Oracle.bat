@ECHO off
ECHO * Generating Oracle seeding dev data...
ECHO ****************************************************************************************************

SET workingPath=%cd%

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON

dbmanager single -n "SeedDevelopmentData.cs" -m "%workingPath%" -p Oracle

@ECHO OFF
popd
pause
