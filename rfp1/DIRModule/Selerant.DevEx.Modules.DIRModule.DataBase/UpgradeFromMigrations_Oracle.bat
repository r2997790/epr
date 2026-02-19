@ECHO *********************************************************************************
@ECHO * Create ORACLE release DIR db schema from migrations
@ECHO *********************************************************************************
@ECHO OFF

SET workingPath=%cd%
SET logFilePath="%workingPath%\UpgradeFromMigrations_Oracle.log"

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON
REM  
dbmanager upgrade --clean -m "%workingPath%" -v "91:91" -p "Oracle|DIR_MODULE/devex123@BLSRV-Oracle03:1522/ORCL12C" -s --execute --executeArguments  "INDEXES" "LOBDATA" "1" --logFileMode "CreateNew" --logfile %logFilePath%

@ECHO OFF
popd
Pause