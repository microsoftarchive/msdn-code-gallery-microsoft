using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Deployment;
using Microsoft.SharePoint.WebControls;

namespace WCM_ExportContent.Layouts.WCM_ExportContent
{
    /// <summary>
    /// This sample SharePoint application page demonstrates how to use the Content
    /// Deployment API to export a single List and all its contents to a Content 
    /// Migration Package (CMP) file.
    /// </summary>
    /// <remarks>
    /// This example exports the Tasks list to a CMP file in the root of the C: drive
    /// </remarks>
    public partial class ExportTasksList : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void exportButton_Click(object sender, EventArgs e)
        {
            //Create an export settings object to configure the export
            SPExportSettings exportSettings = new SPExportSettings();
            //Set the site you want to export from. Alter this to match your site.
            exportSettings.SiteUrl = "http://intranet.contoso.com";
            //Select a full export (alternatively you could do an incremental export to just include changes)
            exportSettings.ExportMethod = SPExportMethodType.ExportAll;
            //Set the location and name of the export file
            exportSettings.BaseFileName = "demoTasksExport";
            exportSettings.FileLocation = @"C:\";

            //To export just the Tasks list, first create an ExportObject
            SPExportObject tasksEO = new SPExportObject();
            //We must set it to export a SharePoint List
            tasksEO.Type = SPDeploymentObjectType.List;
            //We want to export everything in the list
            tasksEO.IncludeDescendants = SPIncludeDescendants.All;
            //Set the URL of the list to export.
            tasksEO.Url = "http://intranet.contoso.com/lists/tasks";
            //Add that to the Export Objects collection
            exportSettings.ExportObjects.Add(tasksEO);

            //Do the export
            SPExport export = new SPExport(exportSettings);
            try
            {
                export.Run();
                resultsLabel.Text = "The export ran successfully";
            }
            catch (Exception ex)
            {
                resultsLabel.Text = "The export generated an error: " + ex.Message;
            }
            
        }

    }
}
