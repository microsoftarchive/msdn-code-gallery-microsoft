using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_ApplicationPage.Layouts.UI_ApplicationPage
{
    //This is the code-behind file for an application page
    public partial class DemoApplicationPage : LayoutsPageBase
    {
        SPWeb currentWeb = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }

            //Display some details about the site
            titleLabel.Text = currentWeb.Title;
            descriptionLabel.Text = currentWeb.Description;
            authorLabel.Text =  currentWeb.Author.Name;
            createdOnLabel.Text = currentWeb.Created.ToLongDateString();
            currentUserLabel.Text = currentWeb.CurrentUser.Name;
            //Because this is a shared resource, we don't need to cal currentWeb.Dispose()
        }

        public void setDescriptionButton_Click(Object sender, EventArgs args)
        {
            //Edit the description and call update
            currentWeb.Description = newSiteDescriptionTextBox.Text;
            currentWeb.Update();
        }


    }
}
