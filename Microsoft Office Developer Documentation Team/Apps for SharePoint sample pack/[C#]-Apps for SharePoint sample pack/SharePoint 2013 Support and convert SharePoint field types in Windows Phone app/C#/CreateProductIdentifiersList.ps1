<#############################################################

Creates Product Orders list for SPListAppGuidConversion project.

Run script from the SharePoint Management Shell.

#############################################################>
Add-PSSnapin Microsoft.Sharepoint.Powershell -ErrorAction SilentlyContinue

$webUrl = Read-Host "Enter URL of SharePoint site"
$spWeb = Get-SPWeb $webUrl
$listName = "Product Identifiers"

if ($spWeb -ne $null)
{
	$spList = $spWeb.Lists[$listName]
	if ($spList -ne $null)
	{
		$title = "The $listName list already exists."
		$prompt = "Do you want to delete it?"
		$choices = [System.Management.Automation.Host.ChoiceDescription[]](
		
(New-Object System.Management.Automation.Host.ChoiceDescription "&Yes","Delete existing list before creating new one."),
		
(New-Object System.Management.Automation.Host.ChoiceDescription "&No","Keep existing list and quit."))

		$Answer = $host.ui.PromptForChoice($title, $prompt, $choices, (1))

		if ($Answer -eq (0))
		{
			$spList.Delete()
		}
		else
		{
			exit
		}
	}

	try
	{
		$spWeb.Lists.Add($listName, $listName + "List", [Microsoft.SharePoint.SPListTemplateType]::GenericList)

		$spList = $spWeb.Lists[$listName]

		Write-Host "The $listName list was added to the site ($spWeb)." -foregroundcolor Yellow 

		$spView = $spList.DefaultView

		$spFieldType = [Microsoft.SharePoint.SPFieldType]::Guid
		$spField = $spList.Fields.Add("Identifier", $spFieldType, $false)

		$spView.ViewFields.Add($spField) 
		$spView.Update()

		$spWeb.Update()

		Write-Host "Columns added to $listName list." -foregroundcolor Yellow 
	}
	catch [Net.WebException]
	{
		Write-Host $_.Exception.ToString() -foregroundcolor Red
	}
	catch
	{
		Write-Host "Error encountered." -foregroundcolor Red
	}
	finally
	{
		$spWeb.Dispose()
	}
}
else
{
	Write-Host "Web site is not available." -foregroundcolor Red
}



