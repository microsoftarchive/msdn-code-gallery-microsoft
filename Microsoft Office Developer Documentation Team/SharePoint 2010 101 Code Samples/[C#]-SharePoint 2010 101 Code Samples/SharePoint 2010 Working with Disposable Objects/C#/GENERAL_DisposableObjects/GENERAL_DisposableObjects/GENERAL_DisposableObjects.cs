using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace GENERAL_DisposableObjects.GENERAL_DisposableObjects
{
    //This Web Part illustrates how to correctly dispose of objects that implement the
    //IDisposable interface. This is vital in SharePoint development because, if you do 
    //not correctly dispose of SPWeb, SPSite, and certain other objects, memory usage
    //will rise unnecessarily.
    [ToolboxItemAttribute(false)]
    public class GENERAL_DisposableObjects : WebPart
    {
        //Controls
        Button getSiteButton;
        Label currentSiteInfo;
        Button getWebButton;
        Label currentWebInfo;


        protected override void CreateChildControls()
        {
            //Set up the Get Site button
            getSiteButton = new Button();
            getSiteButton.Text = "Get Site Info";
            getSiteButton.Click += new EventHandler(getSiteButton_Click);
            this.Controls.Add(getSiteButton);
            //Set up the Current Site Info Label
            currentSiteInfo = new Label();
            this.Controls.Add(currentSiteInfo);
            //Set up the Get Web button
            getWebButton = new Button();
            getWebButton.Text = "Get Web Info";
            getWebButton.Click += new EventHandler(getWebButton_Click);
            this.Controls.Add(getWebButton);
            //Set up the Current Web Info Label
            currentWebInfo = new Label();
            this.Controls.Add(currentWebInfo);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render the controls
            getSiteButton.RenderControl(writer);
            currentSiteInfo.RenderControl(writer);
            writer.WriteBreak();
            getWebButton.RenderControl(writer);
            currentWebInfo.RenderControl(writer);
        }

        void getWebButton_Click(object sender, EventArgs e)
        {
            using (SPWeb currentWeb = SPContext.Current.Web)
            {
                currentWebInfo.Text = "Current Web Title: " + currentWeb.Title;
            }
            //At the end of the using block, currentWeb is disposed of implicitly
        }

        void getSiteButton_Click(object sender, EventArgs e)
        {
            //This code is unlikely to fail, but it illustrates the importance of a finally
            //block, to ensure that your dispose of objects. Without the finally block, an
            //exception would prevent the correct object disposal.
            SPSite currentSite = null;
            try
            {
                //Get the current site
                currentSite = SPContext.Current.Site;
                //Display the site ID, just as an example
                currentSiteInfo.Text = "Current Site ID: " + currentSite.ID;
            }
            catch (Exception ex)
            {
                currentSiteInfo.Text = "There was an error getting the site info: " + ex.Message;
            }
            finally
            {
                //The currentSite object is always disposed
                currentSite.Dispose();
            }
        }
    }
}
