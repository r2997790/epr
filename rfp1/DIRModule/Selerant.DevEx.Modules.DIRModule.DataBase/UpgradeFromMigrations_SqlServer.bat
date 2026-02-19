@ECHO *********************************************************************************
@ECHO * Create SQL Server release DIR db schema from migrations
@ECHO *********************************************************************************
@ECHO OFF

SET workingPath=%cd%
SET logFilePath="%workingPath%\UpgradeFromMigrations_SqlServer.log"

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON
REM --execute
dbmanager upgrade --clean -m "%workingPath%" -v "91:91" -p "SqlServer|devex_main/devex123@BLSRV-MSSQL02\DXMSSQL2014" -s --execute --logFileMode "CreateNew" --logfile %logFilePath%

@ECHO OFF
popd
Pause