using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DemoWebParts.SilverlightToSilverlight
{
    /// <summary>
    /// This web part simply hosts the Silverlight applications (Comsumer and Provider)
    /// However, its Editor Web Part enables you to set the name of a Web Part to connect
    /// to. This is stored in the InitParameters collection when the silverlight application
    /// is rendered. Since the Silverlight Consumer and Provider apps used this parameter
    /// to initiallize their Message Sender and Message Receiver, this enables them to 
    /// send and receive message, just like connected web parts.
    /// </summary>
    /// <remarks>
    /// To use this example, compile the SilverlightMessageConsumer and SilverlightMessageProvider
    /// applications and upload them to a SharePoint location, such as 
    /// http://intranet.contoso.com/SiteAssets. Then run the SilverlightToSilverlight 
    /// project to deploy the Web Part. Add the web part to a Web Part page and edit its
    /// properties. For the Silverlight Application URL type the path to the Provider 
    /// application (e.g. http://intranet.contoso.com/SiteAssets/SilverlightMessageProvider.xap)
    /// Then add the same web part to the page a second time and edit it. Set a different
    /// Web Part name and type the path to the Consumer application. Edit the web part that 
    /// hosts the provider application and configure it to send messages to the web
    /// part with the consumer application.
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class ConnectableSilverlightWebPart : WebPart, IWebEditable
    {
        private string silverlightApplication;
        private string receiverName;
        
        public ConnectableSilverlightWebPart()
        {
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            //Render the HTML for the Silverlight control
            writer.WriteLine("<object id=\"ConnectableSilverlightWebPart" + this.ClientID + "\"");
            writer.WriteLine("data=\"data:application/x-silverlight-2,\"");
            writer.WriteLine("type=\"application/x-silverlight-2\"");
            writer.WriteLine("style=\"display:block; height: 100%; width: 100%;\"");
            writer.WriteLine("class=\"ConnectableSilverlightWebPart\">");
            //Add a parameter to specify the Silverlight application to render
            writer.WriteLine("<param name=\"source\" value=\"" + silverlightApplication + "\" />");
            writer.WriteLine("<param name=\"windowless\" value=\"true\" />");
            //Add init parameters
            writer.Write("<param name=\"initParams\" value=\"ClientID=" + this.ClientID + ",");
            writer.WriteLine("ReceiverName=" + this.receiverName + "\" />");
            //Add tags for getting silverlight here!
            writer.WriteLine("</object>");
        }

        //Silverlight application to load
        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string SilverlightApplication
        {
            get { return silverlightApplication; }
            set { silverlightApplication = value; }
        }

        //Name of the receiver to send messages to
        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string ReceiverName
        {
            get { return receiverName; }
            set { receiverName = value; }
        }

        //IWebEditable Members
        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            editors.Add(new ConnectableSilverlightWebPartEditor(this.ID));
            return new EditorPartCollection(editors);
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }
    }
}
