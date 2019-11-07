using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Deployment;
using Microsoft.SharePoint.WebControls;

namespace WCM_ImportContent.Layouts.WCM_ImportContent
{
    /// <summary>
    /// This sample SharePoint application page demonstrates how to use the Content
    /// Deployment API to import a single List and all its contents from a Content 
    /// Migration Package (CMP) file.
    /// </summary>
    /// <remarks>
    /// This example imports the content of a CMP file in the root of the C: drive
    /// </remarks>
    public partial class ImportTasksList : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void importButton_Click(object sender, EventArgs args)
        {
            //Create an ImportSettings object to configure the import
            SPImportSettings importSettings = new SPImportSettings();
            //Set the name and location of the import file
            importSettings.BaseFileName = "demoTasksExport.cmp";
            importSettings.FileLocation = @"C:\";
            //Set the URL of the site to import into
            importSettings.SiteUrl = "http://intranet.contoso.com";

            //Do the import
            SPImport import = new SPImport(importSettings);
            try
            {
                import.Run();
                resultsLabel.Text = "Import was successful";
            }
            catch (Exception ex)
            {
                resultsLabel.Text = "The Import caused an error: " + ex.Message;
            }

        }
    }
}
