@ECHO OFF
SETLOCAL

REM Setup the current batch to use local variables.
SETLOCAL ENABLEDELAYEDEXPANSION

REM determine whether script was executed in interactive mode
set interactive=1
echo !cmdcmdline! | find /i "%~f0" >nul
if not errorlevel 1	if _%1_==__ set interactive=0

REM MAIN
set prodonly=%~1
if "!prodonly!"=="--prod" (
	call "%~dp0..\..\..\..\Tools\NodeJs\Selerant.DevEx.Yarn\bin\restore_packages" "%~dp0package.json" --prod
) else (
	call "%~dp0..\..\..\..\Tools\NodeJs\Selerant.DevEx.Yarn\bin\restore_packages" "%~dp0package.json"
)
IF !errorlevel! NEQ 0 GOTO BuildError

:End
REM if interactive mode then pause
if _!interactive!_==_0_ pause
ENDLOCAL
EXIT /B 0

:BuildError
REM if interactive mode then pause
if _!interactive!_==_0_ pause
ENDLOCAL
EXIT /B 1
