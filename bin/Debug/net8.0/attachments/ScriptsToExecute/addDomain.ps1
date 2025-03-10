function addDomain{Param([Parameter(Mandatory=$false, ValueFromPipeLine)]
$DomainP,
$UserNameP,
$PasswordP

)
CLS
if($DomainP)
{
$Pwd = $PasswordP | ConvertTo-SecureString -asPlainText -Force
$Dom = $DomainP.Split('.')

$DomainUserNAme = $Dom[0] +"\"+ $UserNameP
$credential = New-Object System.Management.Automation.PSCredential($DomainUserNAme, $Pwd)
Add-Computer -DomainName $DomainP -Credential $credential #-ErrorAction SilentlyContinue
}
else
{
$domain = Read-Host("Please, insert domain.")
$username = Read-Host("Please, insert your username")
$password = Read-Host("Please, insert password") | ConvertTo-SecureString -asPlainText -Force

$Dom = $DomainP.Split('.')

$DomainUserNAme = $Dom[0] +"\"+ $UserNameP

$credential = New-Object System.Management.Automation.PSCredential($DomainUserNAme, $password)
Add-Computer -DomainName $domain -Credential $credential #-ErrorAction SilentlyContinue
}
}
addDomain -DomainP "locpipa.intra" -UserNameP "adm01" -PasswordP "sMtp2007$&"