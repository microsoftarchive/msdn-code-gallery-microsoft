using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing.Navigation;
using Microsoft.SharePoint.WebControls;

namespace GENERAL_CachedCrossSiteQueries.CachedCrossSiteQueries
{
    [ToolboxItemAttribute(false)]
    public class CachedCrossSiteQueries : WebPart
    {

        //Controls
        Button runQueryButton;
        Label resultsLabel;

        protected override void CreateChildControls()
        {
            //Set up the Run Query button
            runQueryButton = new Button();
            runQueryButton.Text = "Run Query";
            runQueryButton.Click += new EventHandler(runQueryButton_Click);
            this.Controls.Add(runQueryButton);
            //Set up the results label
            resultsLabel = new Label();
            resultsLabel.Text = "Query is not yet run";
            this.Controls.Add(resultsLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render the button
            runQueryButton.RenderControl(writer);
            writer.Write("<br />");
            //Render the results label
            resultsLabel.RenderControl(writer);
        }

        void runQueryButton_Click(object sender, EventArgs e)
        {
            //The PortalSiteMapProvider class provides high-performance queries for 
            //data that changes infrequently because it makes use of SharePoint's 
            //query caches. If you frequently query for the same dataset that changes rarely,
            //you should consider using this object to run your queries.

            //A PortalSiteMapProvider object is large and takes lots of resources to set
            //up. You should not create a new one for your queries but instead use one of
            //the examples SharePoint provides by default. If you need to create your own
            //PortalSiteMapProvider you should configure the web application with web.config.

            //We'll get the example that SharePoint uses to build the breadcrumb trail
            PortalSiteMapProvider portalProvider = PortalSiteMapProvider.CurrentNavSiteMapProviderNoEncode;

            //Write a introductory results line
            resultsLabel.Text = "Sites: <br />";

            //Run a query. This example gets all the child sites.
            SiteMapNodeCollection children = portalProvider.GetChildNodes(((PortalSiteMapNode)portalProvider.CurrentNode).WebNode,
                  Microsoft.SharePoint.Publishing.NodeTypes.Area, Microsoft.SharePoint.Publishing.NodeTypes.Area);

            //Loop through the results and display the site title.
            foreach (SiteMapNode node in children)
            {
                resultsLabel.Text += node.Title + "<br />";
            }
        }
    }
}
