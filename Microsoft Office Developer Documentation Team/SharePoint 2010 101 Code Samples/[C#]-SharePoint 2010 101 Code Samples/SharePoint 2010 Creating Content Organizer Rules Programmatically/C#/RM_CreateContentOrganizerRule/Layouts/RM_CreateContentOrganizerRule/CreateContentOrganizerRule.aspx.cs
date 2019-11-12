using System;
using Microsoft.Office.RecordsManagement.RecordsRepository;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RM_CreateContentOrganizerRule.Layouts.RM_CreateContentOrganizerRule
{
    /// <summary>
    /// This application page demonstrates how to create and configure a Content
    /// Organizater Routing Rule in SharePoint. It creates a rule that copies
    /// Documents from the Shared Documents library to a folder of that library 
    /// called DocumentDestination. 
    /// </summary>
    /// <remarks>
    /// For this code to work, you must have created the folder and library described
    /// above and enabled the Content Organizer feature at the site level. You can view
    /// the Content Organizer rule created in Site Actions/Site Settings/Site Administration
    /// /Content Organizer Rules and you can test it by creating a new Document in the
    /// Shared Document library.
    /// </remarks>
    public partial class CreateContentOrganizerRule : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void createRuleButton_Click(object sender, EventArgs e)
        {

            SPWeb currentWeb = SPContext.Current.Web;
            //We must get the EcmDocumentRoutingWeb object that corresponds to the current SPWeb
            EcmDocumentRoutingWeb routingWeb = new EcmDocumentRoutingWeb(currentWeb);

            //Get handles on the Document content type and the Shared Documents list
            SPContentType contentTypeForRule = currentWeb.ContentTypes["Document"];
            SPList listForRule = currentWeb.Lists["Shared Documents"];
            SPFolder folderForRule = currentWeb.GetFolder(currentWeb.Url + "/Shared%20Documents/DocumentDestination");

            //Check that the content type is included in that library
            if (listForRule.ContentTypes.BestMatch(contentTypeForRule.Id) == null)
            {
                resultsLabel.Text = "The Document content type is not available in the " +
                    "Shared Documents folder so the rule cannot be created";
            }
            else
            {
                //Create a rule object
                EcmDocumentRouterRule newRule = new EcmDocumentRouterRule(currentWeb);

                //Set the properties
                newRule.Name = "Move all Documents";
                newRule.Description = "This rule was create by C# code.";
                newRule.ContentTypeString = contentTypeForRule.Name;
                newRule.RouteToExternalLocation = false;
                newRule.Priority = "5";
                newRule.TargetPath = folderForRule.ServerRelativeUrl;

                //Commit your changes
                newRule.Update();

            }

            //Tell the user what happened
            resultsLabel.Text = "Rule created successfully.";

        }
    }
}
