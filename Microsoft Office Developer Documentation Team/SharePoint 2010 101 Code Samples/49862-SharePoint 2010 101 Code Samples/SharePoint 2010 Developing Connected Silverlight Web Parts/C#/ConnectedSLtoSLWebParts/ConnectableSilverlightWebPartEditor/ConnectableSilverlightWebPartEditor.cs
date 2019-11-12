using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace DemoWebParts.SilverlightToSilverlight
{
    [ToolboxItemAttribute(false)]
    public class ConnectableSilverlightWebPartEditor : EditorPart
    {
        private TextBox textboxSilverlightApplication;
        private DropDownList ddlSendMessageToThisWebPart;

        public ConnectableSilverlightWebPartEditor(string webPartID)
        {
            this.ID = "ConnectableSilverlightWebPartEditor" + webPartID;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            //Create the textbox for specifying the Silverlight application
            textboxSilverlightApplication = new TextBox();
            textboxSilverlightApplication.Width = 300;
            this.Controls.Add(textboxSilverlightApplication);
            //Create the drop down list for choosing the web part to connect to
            ddlSendMessageToThisWebPart = new DropDownList();
            //Add an item to the drop down list for every Connectable Silverlight Web Part on the page
            ListItem currentItem;
            SPFile currentPage = SPContext.Current.File;
            SPLimitedWebPartManager WPM = currentPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            SPLimitedWebPartCollection WebPartCollection = WPM.WebParts;
            foreach (System.Web.UI.WebControls.WebParts.WebPart currentWebPart in WebPartCollection)
            {
                //Make sure this is a Connectable Silverlight Web Part
                if (currentWebPart.GetType().ToString() == "DemoWebParts.SilverlightToSilverlight.ConnectableSilverlightWebPart")
                {
                    //Add the item
                    currentItem = new ListItem();
                    currentItem.Text = currentWebPart.Title;
                    currentItem.Value = "SLReceiver_ctl00_m_" + currentWebPart.ClientID;
                    ddlSendMessageToThisWebPart.Items.Add(currentItem);
                }
            }
            this.Controls.Add(ddlSendMessageToThisWebPart);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.WriteLine("Path to Silverlight Application: ");
            textboxSilverlightApplication.RenderControl(writer);
            writer.WriteLine("<br/>");
            writer.WriteLine("Send Messages to This Web Part: ");
            ddlSendMessageToThisWebPart.RenderControl(writer);
            writer.WriteLine("<br/>");
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ConnectableSilverlightWebPart webPart = this.WebPartToEdit as ConnectableSilverlightWebPart;
            if (webPart != null)
            {
                webPart.SilverlightApplication = textboxSilverlightApplication.Text;
                webPart.ReceiverName = ddlSendMessageToThisWebPart.SelectedValue;
            }
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ConnectableSilverlightWebPart webPart = this.WebPartToEdit as ConnectableSilverlightWebPart;
            if (webPart != null)
            {
                textboxSilverlightApplication.Text = webPart.SilverlightApplication;
                ddlSendMessageToThisWebPart.SelectedValue = webPart.ReceiverName;
            }
        }
    }
}
