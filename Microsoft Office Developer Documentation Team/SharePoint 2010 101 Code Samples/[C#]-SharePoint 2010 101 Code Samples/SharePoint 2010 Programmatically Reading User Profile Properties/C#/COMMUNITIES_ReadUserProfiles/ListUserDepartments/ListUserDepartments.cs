using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//The following two namespaces are required to work with user profiles
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace COMMUNITIES_ReadUserProfiles.ListUserDepartments
{
    /// <summary>
    /// This web part gets all the user profiles and displays the name of each
    /// user and the department they are in. 
    /// </summary>
    /// <remarks>
    /// You must add references to the Microsoft.Office.Server and
    /// Microsoft.Office.Server.UserProfiles dlls in the 14 hive ISAPI directory. 
    /// You must have a User Profile service application in place.
    /// Also the code below only works in the context of an account that has the
    /// "manage user profiles" right. If you don't have that right you could get
    /// similar results by using the Search API and the People search scope.
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class ListUserDepartments : WebPart
    {
        //Controls
        Button runQueryButton;
        Label resultsLabel;

        protected override void CreateChildControls()
        {
            //Create the Run Query button
            runQueryButton = new Button();
            runQueryButton.Text = "Run Query";
            runQueryButton.Click += new EventHandler(runQueryButton_Click);
            this.Controls.Add(runQueryButton);
            //Create the results display label
            resultsLabel = new Label();
            resultsLabel.Text = String.Empty;
            this.Controls.Add(resultsLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render all controls
            runQueryButton.RenderControl(writer);
            writer.Write("<br />");
            resultsLabel.RenderControl(writer);
        }

        //This method runs when the button is clicked
        void runQueryButton_Click(object sender, EventArgs e)
        {
            //Get the My Sites site collection, ensuring proper disposal
            using (SPSite mySitesCollection = new SPSite("http://intranet.contoso.com/my"))
            {
                //Get the user profile manager
                SPServiceContext context = SPServiceContext.GetContext(mySitesCollection);
                UserProfileManager profileManager = new UserProfileManager(context);
                //How many user profiles are there?
                resultsLabel.Text = "There are " + profileManager.Count + " user profiles.<br />";
                //Loop through all the user profiles
                foreach (UserProfile currentProfile in profileManager)
                {
                    //There is always a display name
                    resultsLabel.Text += "User: " + currentProfile.DisplayName + "<br />";
                    //Be careful to avoid null errors
                    if (currentProfile["Department"].Value != null)
                    {
                        //There is a department listed so display it
                        resultsLabel.Text += "Department: " + currentProfile["Department"].Value.ToString() + "<br /><br />";
                    }
                    else
                    {
                        resultsLabel.Text += "Department: None<br /><br />";
                    }
                }
            }
        }
    }
}
