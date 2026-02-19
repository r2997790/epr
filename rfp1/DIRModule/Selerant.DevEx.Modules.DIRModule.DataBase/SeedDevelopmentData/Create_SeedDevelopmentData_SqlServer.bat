@ECHO off
ECHO * Generating SqlServer seeding dev data...
ECHO ****************************************************************************************************

SET workingPath=%cd%

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON

dbmanager single --clean -n "SeedDevelopmentData.cs" -m "%workingPath%" --outputfile "run_SeedDevelopmentData_SqlServer.sql" -p SqlServer

@ECHO OFF
popd
pause
