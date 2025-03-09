cd c:\manut
md test000
md test002 
md test003
md test004
md test005
md test006
md test007
md test008
md test009
md test010
pause
Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon' -Name 'AutoAdminLogon' -Value 0
$lastLogon = Get-ItemPropertyValue -Path 'HKLM:\SOFTWARE\RemoteCmd' -Name 'current logged user'
Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon' -Name 'LastUsedUsername' -Value $lastLogon
shutdown -r -f -t 40
