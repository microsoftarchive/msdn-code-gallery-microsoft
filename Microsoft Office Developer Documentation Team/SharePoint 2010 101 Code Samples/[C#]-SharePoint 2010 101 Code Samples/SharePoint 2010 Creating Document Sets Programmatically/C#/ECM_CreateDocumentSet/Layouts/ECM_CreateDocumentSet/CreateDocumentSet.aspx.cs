using System;
using System.Collections;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ECM_CreateDocumentSet.Layouts.ECM_CreateDocumentSet
{
    /// <summary>
    /// This application page creates a document set based on the default document set
    /// content type. 
    /// </summary>
    /// <remarks>
    /// You must have enabled the site collection level Document Sets feature, and added
    /// the Document Set content type to the document library, before you can create 
    /// document sets.
    /// </remarks>
    public partial class CreateDocumentSet : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void createDocSetButton_Click(object sender, EventArgs e)
        {
            //Get the Shared Documents document library
            SPWeb currentWeb = SPContext.Current.Web;
            SPDocumentLibrary sharedDocsLib = (SPDocumentLibrary)currentWeb.Lists["Shared Documents"];
            //You can use a hashtable to populate properties of the document set
            Hashtable docsetProperties = new Hashtable();
            docsetProperties.Add("Name", nameTextbox.Text);
            docsetProperties.Add("Description", descriptionTextbox.Text);
            //Create the document set
            try
            {
                DocumentSet newDocSet = DocumentSet.Create(sharedDocsLib.RootFolder,
                    nameTextbox.Text, sharedDocsLib.ContentTypes["Document Set"].Id,
                    docsetProperties, true);
                resultLabel.Text = "Document set created";
            }
            catch (Exception ex)
            {
                resultLabel.Text = "An error occurred: " + ex.Message;
            }
        }
    }
}
