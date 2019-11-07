function LoadSharePointPowerShellEnvironment
{
	write-host 
	write-host "Setting up PowerShell environment for SharePoint" -foregroundcolor Yellow
	write-host 
	Add-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue
	write-host "SharePoint PowerShell Snapin loaded." -foregroundcolor Green
}

#
# +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
# Provision create service app & start service app instance
# +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#

write-host 
LoadSharePointPowerShellEnvironment

write-host 
write-host "[[STEP]] Creating Day Namer Service Application." -foregroundcolor Yellow
write-host 

write-host "Ensure service application not already created..." -foregroundcolor Gray
$serviceApp = Get-SPServiceApplication | where { $_.GetType().FullName -eq "WCF_CustomServiceApplication.Server.DayNamerServiceApplication" -and $_.Name -eq "Day Namer Service Application" }
if ($serviceApp -eq $null){
	write-host "Creating service application..." -foregroundcolor Gray
	$guid = [Guid]::NewGuid()
	$serviceApp = New-DayNamerServiceApplication -Name "Day Namer Service Application" -ApplicationPool "SharePoint Web Services System"
    if ($serviceApp -ne $null){
	    write-host "Day Namer Service Application created." -foregroundcolor Green
	}
}


# [[[[[[[[STEP]]]]]]]]


write-host 
write-host "[[STEP]] Configuring permissions on Day Namer Service Application." -foregroundcolor Yellow
write-host 

write-host "Configure permissions on the service app..." -foregroundcolor Gray
$user = $env:userdomain + '\' + $env:username

write-host "  Creating new claim for $user..." -foregroundcolor Gray
$userClaim = New-SPClaimsPrincipal -Identity $user -IdentityType WindowsSamAccountName
$security = Get-SPServiceApplicationSecurity $serviceApp

write-host "  Granting $user 'FULL CONTROL' to service application..." -foregroundcolor Gray
Grant-SPObjectSecurity $security $userClaim -Rights "Full Control"
Set-SPServiceApplicationSecurity $serviceApp $security

write-host "Day Namer Service Application permissions set." -foregroundcolor Green

# [[[[[[[[STEP]]]]]]]]

write-host 
write-host "[[STEP]] Starting Day Namer Service Application instance on local server." -foregroundcolor Yellow
write-host 

write-host "Ensure service instance is running on server $env:computername..." -foregroundcolor Gray
$localServiceInstance = Get-SPServiceInstance -Server $env:computername | where { $_.GetType().FullName -eq "WCF_CustomServiceApplication.Server.DayNamerServiceInstance" -and $_.Name -eq "" }
if ($localServiceInstance.Status -ne 'Online'){
	write-host "Starting service instance on server $env:computername..." -foregroundcolor Gray
	Start-SPServiceInstance $localServiceInstance
	write-host "Day Namer Service Application instance started." -foregroundcolor Green
}



write-host "[[[[ Day Namer Service Application provisioned & instance started. ]]]]" -foregroundcolor Green

write-host 