@ECHO OFF
REM Use this script in "Developer Command Prompt for VS" 
REM to test the correctness of the cs scripts distributed with DEVEX

SET DevexBinFolderPath="C:\Modules\DIR\DIRModule\Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd\bin\DevEx3.10"

csc /out:AutoNumbering_DIR.dll /target:library /reference:Selerant.DevEx.Dal.dll  /reference:Selerant.DevEx.BusinessLayer.dll /reference:Selerant.ApplicationBlocks.dll /lib:%DevexBinFolderPath% AutoNumbering_DIR.cs

pause
