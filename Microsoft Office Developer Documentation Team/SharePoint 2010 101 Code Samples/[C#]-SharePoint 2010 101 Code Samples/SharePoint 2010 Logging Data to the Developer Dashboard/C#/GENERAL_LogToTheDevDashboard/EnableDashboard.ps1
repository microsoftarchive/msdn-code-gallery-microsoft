function LoadSharePointPowerShellEnvironment
{
	write-host "Setting up PowerShell environment for SharePoint"
	Add-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue
	write-host "SharePoint PowerShell Snapin loaded."
}

#
# +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
# This script enables the developer dashboard in On Demand mode
# +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#

LoadSharePointPowerShellEnvironment

write-host "Setting the Developer Dashboard display level to On Demand..."

$service = [Microsoft.SharePoint.Administration.SPWebService]::ContentService
$addsetting =$service.DeveloperDashboardSettings
$addsetting.DisplayLevel = [Microsoft.SharePoint.Administration.SPDeveloperDashboardLevel]::OnDemand
$addsetting.Update()

write-host "Configuration complete"