using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace UI_DelegateControl.CONTROLTEMPLATES
{
    /// <summary>
    /// This custom delegate control replaces the standard SharePoint Global navigation
    /// By way of example, it simply displays links to every list in the site.
    /// </summary>
    /// <remarks>
    /// To build a delegate control, map a folder to the SharePoint CONTROLTEMPLATES 
    /// folder, then add a SharePoint User Control in it. Add an empty module to declare
    /// the control as a delegate. See GlobalNavModule/Elements.xml for the syntax. 
    /// </remarks>
    public partial class GlobalNavDelegate : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string linksHtml = String.Empty;
            //Get the current site, ensuring correct disposal
            using (SPWeb currentWeb = SPContext.Current.Web)
            {
                //Loop through every list in the site
                foreach (SPList list in currentWeb.Lists)
                {
                    //Formulate a link to the list
                    linksHtml += "<a href=\"" + list.DefaultViewUrl + "\">";
                    linksHtml += list.Title;
                    linksHtml += "</a> ";
                }
                //Display all the list links
                linksLabel.Text = linksHtml;
            }
        }
    }
}
