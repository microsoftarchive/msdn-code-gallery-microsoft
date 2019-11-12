using System;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ECM_ManageDocumentSetTemplate.Layouts.ECM_ManageDocumentSetTemplate
{
    /// <summary>
    /// This application page illustrates how to get all the document sets in a
    /// document library and read their properties. You need a combination of objects
    /// to do this. For example, each DocumentSet object corresponds to an SPFolder
    /// object. Use SPFolder.Name for the Name of the set and DocumentSet for the
    /// Welcome Page URL.
    /// </summary>
    /// <remarks>
    /// This example assumes that there is a Shared Documents document library in the
    /// current site.
    /// </remarks>
    public partial class ManageDocumentSetTemplate : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void getDocSetsButton_Click(object sender, EventArgs e)
        {
            int count = 0;
            //Get the Shared Documents document libary
            SPWeb currentWeb = SPContext.Current.Web;
            SPDocumentLibrary sharedDocuments = (SPDocumentLibrary)currentWeb.Lists["Shared Documents"];
            //Loop though all the folders. Some of them will be document sets
            //NOTE: don't use the sharedDocuments.Folders collection! Use sharedDocuments.RootFolder.SubFolders
            foreach(SPFolder currentFolder in sharedDocuments.RootFolder.SubFolders)
            {
                //Get the corresponding document set
                DocumentSet currentDocSet = DocumentSet.GetDocumentSet(currentFolder);
                //Strangely the previous line always returns a document set object
                //Even if the current folder is not a document set. So we use the
                //following test to find out if the current folder is a document set
                if (currentDocSet.Item != null)
                {
                    //This folder is a document set. Increase the count
                    count += 1;
                    //Read some properties of the document set
                    resultLabel.Text += "Name: " + currentFolder.Name + "<br />";
                    resultLabel.Text += "Content Type: " + currentDocSet.ContentType.Name + "<br />";
                    resultLabel.Text += "Document Count: " + currentDocSet.Folder.ItemCount + "<br />";
                    resultLabel.Text += "Welcome Page: " + currentDocSet.WelcomePageUrl + "<br /><br />";
                }
            }
            //Display the count
            docSetCountLabel.Text = count.ToString();
        }
    }
}
