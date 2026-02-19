@ECHO OFF
ECHO * [Oracle] Running DBManager.exe to generate scripts to Install Current DIR Module Schema from scratch
ECHO *********************************************************************************

SET workingPath=%cd%

pushd .

CD "C:\Common\Tools\DbManager\Selerant.DbManager.CLI\bin\Debug"
@ECHO ON

dbmanager create --clean -m "%workingPath%" -p "Oracle|DEVEX_MAIN_DIR/devex123@BLSRV-Oracle03:1522/ORCL12C" -s --execute --executeArguments  "INDEXES" "LOBDATA" "1"

@ECHO OFF
popd
pause
