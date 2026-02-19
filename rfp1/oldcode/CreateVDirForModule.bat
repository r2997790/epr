@ECHO OFF

@SETLOCAL ENABLEEXTENSIONS

@cd /d "%~dp0

REM Setting Devex application name
REM IMPORTANT: this value is branch specific! Do not merge it across branches!
SET ApplicationName=devex_module_local_oracle

SET ModuleWebFEPath=%cd%\DIRModule\Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd

ECHO --- Stop IIS services
iisreset /STOP

ECHO --- Deleting existing DIRModule folder in inetpub
@RD /S /Q "C:\inetpub\wwwroot\devex_module_local_oracle\Modules\DIRModule"

ECHO --- Start IIS services
iisreset /START

ECHO --- Creating Module IIS virtual folder
CALL "%~dp0CreateVDir.bat" %ApplicationName% Modules/DIRModule "%ModuleWebFEPath%"

ECHO COMPLETED
pause