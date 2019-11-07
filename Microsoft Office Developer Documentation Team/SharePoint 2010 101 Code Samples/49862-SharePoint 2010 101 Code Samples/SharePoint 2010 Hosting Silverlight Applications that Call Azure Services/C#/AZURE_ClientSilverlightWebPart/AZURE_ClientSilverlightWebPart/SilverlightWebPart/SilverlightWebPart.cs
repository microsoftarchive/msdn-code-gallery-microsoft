using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Drawing;

namespace AZURE_ClientSilverlightWebPart.SilverlightWebPart
{
    //This example uses the Silverlight Web Part project template
    //that you can get from http://archive.msdn.microsoft.com/vsixforsp
    //This project is a Web Part that hosts the silverlight application
    [ToolboxItemAttribute(false)]
    public class SilverlightWebPart : WebPart
    {
        private SilverlightPluginGenerator _silverlightPluginGenerator = null;

        public SilverlightWebPart()
        {
            this._silverlightPluginGenerator = new SilverlightPluginGenerator
            {
                Source = new Uri("/SiteAssets/Silverlight/DayNamerClientApp/DayNamerClientApp.xap", UriKind.Relative),
                Width = new Unit(400, UnitType.Pixel),
                Height = new Unit(150, UnitType.Pixel),
                BackGround = Color.White,
                Version = SilverlightVersion.v3,
                AutoUpgrade = true,
                OnError = "onSilverlightError",
            };
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.Controls.Add(new LiteralControl(@"<script type=""text/javascript"">" + Resources.onSilverlightErrorHandler + "</script>"));

            // Set the SiteUrl here.  Can't do this earlier since SPContext may be null during instantiation
            this._silverlightPluginGenerator.InitParams.Add(new InitParam("SiteUrl", SPContext.Current.Site.Url));
            this.Controls.Add(new LiteralControl(this._silverlightPluginGenerator.ToString()));
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }
    }
}
