@ECHO *********************************************************************************
@ECHO * [SQL Server] Running DBManager.exe to generate scripts to Install Current DIR Module Schema from scratch
@ECHO *********************************************************************************
@ECHO OFF

SET workingPath=%cd%
SET logFilePath="%workingPath%\_Output\CreateSchemaSQLServer.log"

pushd .
CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON

dbmanager create --clean -m "%workingPath%" -p "SqlServer|DEVEX_MAIN_DIR/devex123@BLSRV-MSSQL02\DXMSSQL2014" -s --execute --logFileMode "CreateNew" -l %logFilePath%

@ECHO OFF
popd
Pause
