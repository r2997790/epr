REM --------------------------------------------------------------------------------
REM Check for and create VDir under Default Web Site
REM
REM %1 is the Application where to create the virtual directory (e.g. DevEX)
REM %2 is the Virtual Directory to create
REM %3 is the Physical path to the Virtual Directory 
REM --------------------------------------------------------------------------------

IF %1=="" GOTO SYNTAX
IF %2=="" GOTO SYNTAX
IF %3=="" GOTO SYNTAX

"%windir%\system32\inetsrv\appcmd" list vdir "Default Web Site/%1/%2" | findstr /I /C:"Default Web Site/%1/%2" > NULL
IF NOT ERRORLEVEL 1 ( 
    echo ---- Virtual directory %1/%2 already exists
) else (
    "%windir%\system32\inetsrv\appcmd" add vdir /app.name:"Default Web Site/%1" /path:/%2 /physicalPath:%3
	IF NOT ERRORLEVEL 1 ( 
		echo ---- Virtual directory %1/%2 successfully created		
	) else (
		echo ---- Error during creation of virtual directory %1/%2
		exit /b 1
	)
)

exit /b 0

:SYNTAX
ECHO.
ECHO Application Name, Virtual Directory Name and Physical Path are required
ECHO.
ECHO CreateVDir.CMD ^<AppName^> ^<VDirName^> C:\PhysicalPath
ECHO.
