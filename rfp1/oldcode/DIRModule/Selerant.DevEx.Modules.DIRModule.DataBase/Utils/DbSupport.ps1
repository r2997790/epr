# This script is used to generate Module Sql for CurrentSchema or Migration during developemnt or CI/CD procedures 

param(
	[Parameter(Mandatory=$true, HelpMessage="Module short name. e.g.: DIR, SCP, etc")]
	[string]$ModuleName,

	[Parameter(Mandatory=$true, HelpMessage="'create' for CurrentSchema or 'upgrade' for Migration. This parameter is passed as first parameter to DbManager")]
	[ValidateSet("create", "upgrade")]
	[string]$Operation,

	[Parameter(Mandatory=$false, HelpMessage="In case of 'Migration' operation - specify the range of Module Migrations to generate SQL. e.g.: '0001:0011' or 'INITIAL:LATEST' ")]
	[string]$MigrationRange = "INITIAL:LATEST",

	[Parameter(Mandatory=$true, HelpMessage="Database type to generate module SQL. 'Oracle' or 'SqlServer' or 'Oracle|{dbUserName}/{dbUserPassword}@{dbDataSource}|{index_tspace}|{lob_tspace}'")]
	[string]$Provider,
	
	[Parameter(Mandatory=$false, HelpMessage='Specify output folder where to store generated files. Default is .{this_script_path}\Output')]
	[string]$Output_folder,

	[Parameter(Mandatory=$false, HelpMessage="Specify DbManagerVersion to use for generation. In not specified - env:DbManagerVersion will be taken. If not specified - latest")]
	[string]$DbManagerVersion,

	[Parameter(Mandatory=$false, HelpMessage="Specify 'true' if you want to execute generated SQL with DbManager --execute option")]
	[string]$ExecuteSql = "false"

)

("Process started at " + (Get-Date).tostring() )

$devex_root_path = ($PSScriptRoot -Split ("\\net projects"))[0]
$tools_path = $PSScriptRoot + "\Tools"
if(-not $output_folder){ $output_folder = "$PSScriptRoot\Output"}


@($tools_path, $Output_folder) | %{
  if(Test-Path($_)){Remove-Item $_ -Force -Recurse};New-Item $_ -ItemType Directory | Out-Null
}

$shared_config_file = $devex_root_path + "\Build Projects\DevEX\SharedStreamConfigVariables.bat"
if(Test-Path($shared_config_file)){
 " == Setting config variables by calling: $shared_config_file"
 & $shared_config_file
}else{
 "File not found: $shared_config_file"
}

" == Evaluating DBManager version to use =="
if(-not $DBManagerVersion){
  if(-not ($env:DBManagerVersion)){
   "DBManagerVersion is not specified in input params and in environmant variables, using LATEST"
   $DBManagerVersion = "latest"
 }else{
   ("Using DbManager from environment variable:" + $env:DBManagerVersion)
   $DBManagerVersion = $env:DBManagerVersion
 }
}else{
  " == Using DBManagerVersion from input params: $DBManagerVersion"
}

" == Installing DBManager from Nuget == "
if($DBManagerVersion.tolower() -eq "latest"){
  & "$devex_root_path\Build Projects\DevEX\NuGet\nuget.exe" install "Selerant.DBManager.CLI"                            -ExcludeVersion -NonInteractive -PreRelease -OutputDirectory "$tools_path" 
}else{
  & "$devex_root_path\Build Projects\DevEX\NuGet\nuget.exe" install "Selerant.DBManager.CLI" -Version $DBManagerVersion -ExcludeVersion -NonInteractive -PreRelease -OutputDirectory "$tools_path" 
}

" == Installing GlobalDataReservation from Nuget == "
& "$devex_root_path\Build Projects\DevEX\NuGet\nuget.exe" install "Selerant.DevEX.GlobalDataReservation" -ExcludeVersion -NonInteractive -PreRelease -OutputDirectory "$tools_path" 

$dbmanager_path="$tools_path\Selerant.DBManager.CLI\tools\DbManager.exe"
$module_database_path = "$devex_root_path\Net Projects\Selerant\DevEX\Modules\" + $ModuleName + "Module\Selerant.DevEx.Modules." + $ModuleName + "Module.DataBase"

if($Operation -eq "create"){
 $outFileName = ("Install" + $ModuleName + ".sql")
}else{
 $outFileName = ("Upgrade" + $ModuleName + ".sql")
}

$DbManager_params = @(
 $Operation,
 "-m", $module_database_path,
 "-c", "$tools_path\Selerant.DevEX.GlobalDataReservation",
 "-o", $output_folder,
 "--outputfile", $outFileName
)
if($Operation -eq "upgrade"){
 $DbManager_params += @("-v", $MigrationRange)
}

$executeArgs = @()

if($ExecuteSql -eq "true"){
 $DbManager_params += "--execute"

 if( $provider.tolower().Contains("oracle") ){
  "active code page"
  chcp
  "setting to 437 to avoid sqlplus error with unicode input"
   chcp 437

 	 $parts = $provider.Split("|")
	 if($parts.count -eq 5){
	  $provider = $parts[0] + "|" + $parts[1]
    $executeArgs += @("--executeArguments", $parts[2], $parts[3], $parts[4])
	 }else{
		 "cant extract and pass to --executeArguments parameters fromn '$provider'. please use fomrat 'Oracle|{dbUserName}/{dbUserPassword}@{dbDataSource}|{index_tspace}|{lob_tspace}' to pass parameters for execution "
	   exit -1
	 }
 }
}
#escaping provider string with "..." 
if($provider.Contains("|")){
	if(-not ($provider.StartsWith('"'))){$provider = '"' + $provider}
	if(-not ($provider.EndsWith('"'))){$provider = $provider + '"'}
	"provider: $provider" 
}

$DbManager_params += "-p"
$DbManager_params += $provider

if($executeArgs.count -gt 1){
  $DbManager_params += $executeArgs
}

" == DbManager tool location: $dbmanager_path"
(" == DbManager params: " + $DbManager_params )
& $dbmanager_path $DbManager_params *>&1
