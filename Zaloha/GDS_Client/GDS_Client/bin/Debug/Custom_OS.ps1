######################################################
#
#SCRIPT_NAME: Custom_OS.ps1
#AUTHOR_NAME: Lukas Keicher
#DATE: 25.2.2014
#VERSION: 4.0
#DESCRIPTION: Skript provadejici prislusna nastaveni na zaklade prazske pobocky
#
######################################################

#Definice promennych
$Temp="D:\Temp"

$Test_TempPath=Test-Path -path $Temp
if(Test-Path D:\Functions.ps1)
{
	. D:\Functions.ps1
	$Network=gwmi Win32_NetworkAdapterConfiguration | select DNSDomain
	# Prida ovladace pokial sa nenachadza na gopasej sieti
	if (!(($Network -like "*gopas*") -or ($Network -like "*skola*"))) 
	{
		Drivers-Add
	}
}

$Network=gwmi Win32_NetworkAdapterConfiguration | select DNSDomain
#Rozhodovani, zda se pocitac nachazi na siti GOPAS - pokud ano, smaze predchozi verze skriptu a stahne si nove verze
if (($Network -like "*gopas*") -or ($Network -like "*skola*")) 
{
	write-host "GOPAS network detected..." -foregroundcolor green
	
	Copy-Item "Z:\Custom_OS\Functions.ps1" "D:\"
	
	. D:\Functions.ps1
	
	if ($Test_TempPath -eq "true") 
	{		
		write-host "Previous versions of scripts in $Temp detected..." -foregroundcolor Yellow
		write-host "Deleting previous scripts from $Temp" -foregroundcolor green
		
		Kill-Process "*GDS*"
		Kill-Service "GDS_Service"
		Kill-Service "GDSClient_Service"
		Kill-Process "*HONMSW_CLIENT*"
		Kill-Process "*GDSAgent*"
		
		Start-Sleep 5
		
		$TempNoDrivers = get-childitem D:\Temp -exclude "Drivers"
		remove-item $TempNoDrivers -force -recurse >> $null
	} 
	else 
	{
		write-host "Previous versions of scripts in $Temp not detected..." -foregroundcolor Yellow
	}
	$errorActionPreference = "SilentlyContinue"
	Kill-Process "AnK"
	
	write-host "Copying new version of scripts..." -foregroundcolor green	
	get-childitem "z:\Custom_OS" | % {copy-item $_.FullName $Temp -force -recurse}
	if(!(Test-Path "$Temp\Functions.ps1"))
	{		
		#Spusteni skriptu Custom_OS.ps1
		write-host "Executing Custom_OS.ps1..." -foregroundcolor green
		C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "D:\Custom_OS.ps1"

	}
}
else
{
	write-host "No GOPAS network detected..." -foregroundcolor Yellow
	write-host "Applying settings based on latest branch detected..." -foregroundcolor green
}

Set-ConsoleFont -x 8 -y 12
(Get-Process -Name cmd).MainWindowHandle | foreach { Set-WindowStyle MAXIMIZE $_ }

#na fullku
Mode $BufferWidth

#Vypise podrobnosti o pocitaci
Write-Header

Proxy-OFF

#Import certifikatu
AddCert


#Aplikovani nastaveni pro prazskou pobocku (Mapovani sitoveho disku, instalace ovladacu, prejmenovani pocitace, nastaveni powerscheme)
write-host ""
write-host "Executing $Temp\Drivers.ps1..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\Drivers.ps1" -verb runas
write-host ""
write-host "Executing $Temp\Rename.ps1..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\Rename.ps1" -verb runas
Write-Host ""
Set-WOL
write-host ""
write-host "Rearming Office and installing KBs..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\Office_rearm.ps1" -verb runas
write-host ""
write-host "Setting Global Settings..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\GlobalSettings.ps1" -verb runas
write-host ""
write-host "Setting User Settings..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\UserSettings.ps1" -verb runas
write-host ""
write-host "Starting GDS Postinstall phase..." -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
Set-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run -Name "Prepare_UserSettings" -Value "C:\Windows\System32\WindowsPowerShell\v1.0\powershell -windowstyle hidden $Temp\Prepare_UserSettings.ps1"
write-host "Registering Gopas Service for remote reinstall" -foregroundcolor Yellow
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\GopasService\GDS_Create_Service.ps1"
write-host "Registering Gopas Client Service" -foregroundcolor Yellow
C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\GDSClient\GDSClient_Create_Service.ps1"

Write-Host ""
# Nastavi automaticke spustanie Aplikacii na kopirovanie
Write-Host "Setting ANK" -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
SetAnK

Write-Host ""
#Odpojeni sitovych disku, ktere v ramci procesu pouzivany, odpojeni registru defaultniho profilu, vypnuti automatickeho prihlasovani
write-host "Cleaning footprints" -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
write-host "Unloading Default profile registry hive..." -foregroundcolor green
reg unload HKU\Default 2>null | Out-Null

CheckNotebookPassword
ChangeBootLink

Write-Host ""
Write-host "Upgrade Autologon" -ForegroundColor $Global:UserInputColor -BackgroundColor $Global:bgColor 
Save-Current-Logon-User
Remove-Autologon-Registry
Set-AutologonAutomatic
Set-Autologon 0

#Rozhodovani, zde se v registru nachazi definice pro AutologonCount
$AutoLogonCount="HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\"
$AutoLogonCount_Value=(Get-ItemProperty $AutologonCount).AutoLogonCount
If ($AutoLogonCount_Value -eq $null)
    {
        write-host "AutologonCount registry value not exist..." -foregroundcolor Yellow
    }
else
    {
        write-host "AutologonCount registry value exist..." -foregroundcolor green
		write-host "Removing AutologonCount registry value..." -foregroundcolor green
        Remove-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" -Name AutoLogonCount
    }

#Nastaveni GHOST sluzby na Automatic a spusteni sluzby
write-host "Detecting GHOST service..." -foregroundcolor green
$GHOST=(Get-Service | where {$_.name -eq "NGCLIENT"}).Name
if ($GHOST -like "NGCLIENT") 
{
	write-host "GHOST service detected..." -foregroundcolor green
	write-host "Starting GHOST service..." -foregroundcolor green
	Set-Service NGCLIENT -startuptype "Automatic" | Out-Null
} 
else 
{
	write-host "GHOST service not detected..." -foregroundcolor Yellow
}

Copy-Item C:\Windows\Temp\Exchange.bat C:\Windows\System32\Exchange.bat -Force
Copy-Item C:\Windows\Temp\Exchange.bat C:\Windows\System32\Outlook.bat -Force

$Windows_version=wmic os get name
if (($Windows_version -like "*Windows 8*") -or ($Windows_version -like "*Windows 10*"))
{
	net user Profile /active:no 2>null | Out-Null
}

if (($Network -like "*gopas*") -or ($Network -like "*skola*")) 
{	
	TimeSynch -ServerName $ServerName	
	write-host ""
	write-host "Installing PostInstalls" -foreground $Global:UserInputColor -BackgroundColor $Global:bgColor
	C:\Windows\System32\WindowsPowerShell\v1.0\powershell -file "$Temp\InstallFromImage_With_Destination.ps1"
}

write-host "Resetting time..." -foregroundcolor green
ResetTime

$wshell = new-object -comobject wscript.shell -erroraction stop		
if (Test-Path -path $Temp\Password.txt) {Remove-Item $Temp\Password.txt -Force}	
bcdedit /timeout 10 | Out-Null
Restart