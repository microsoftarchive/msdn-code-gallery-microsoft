using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Drawing;

namespace AZURE_ClientSilverlightWebPart
{
    public class SilverlightPluginGenerator
    {
        /// <summary>Specifies the initial width of the Silverlight plug-in area in the HTML page. Can be as a pixel value or as a percentage (a value that ends with the % character specifies a percentage value). For example, "400" specifies 400 pixels, and "50%" specifies 50% (half) of the available width of the browser content area.</summary>
        public Unit Width { get; set; }
        /// <summary>Specifies the initial height of the Silverlight plug-in area in the HTML page. Can be set either as a pixel value or a percentage (a value that ends with the % character specifies a percentage value). For example, "300" specifies 300 pixels, and "50%" specifies 50% (half) of the available height of the browser content area. </summary>
        public Unit Height { get; set; }
        /// <summary>Gets or sets the Uniform Resource Identifier (URI) of the XAP package</summary>
        public Uri Source { get; set; }
        /// <summary>Specifies the ID of the object element</summary>
        public string ID { get; set; }
        /// <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in can use the HtmlPage.PopupWindow method to display a new browser window.</summary>
        public bool? AllowHtmlPopupWindow { get; set; }
        /// <summary>Sets a value that indicates whether a Silverlight plug-in version earlier than the specified minRuntimeVersion will attempt to update automatically.</summary>
        public bool? AutoUpgrade { get; set; }
        /// <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in and in the associated run-time code has access to the browser Document Object Model (DOM).</summary>
        public bool? Enablehtmlaccess { get; set; }
        /// <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in can use a HyperlinkButton to navigate to external URIs.</summary>
        public bool? EnableNavigation { get; set; }

        /// <summary>Gets or sets the background color of the rectangular region that displays XAML content.</summary>
        public Color BackGround { get; set; }

        /// <summary>Specifies the name of the handler to call when the Silverlight plug-in generates a XAML parse error or run-time error at the native-code level.</summary>
        public string OnError { get; set; }
        /// <summary>Specifies the handler for a FullScreenChanged event that occurs whenever the FullScreen property of the Silverlight plug-in changes.</summary>
        public string OnFullScreenChanged { get; set; }
        /// <summary>Establishes the handler for a Loaded event that occurs when the Silverlight plug-in has finished loading into the browser DOM.</summary>
        public string OnLoad { get; set; }
        /// <summary>Specifies a handler for the Resized event that occurs when the Silverlight plug-in's object tag is resized and the  ActualHeight or the ActualWidth of the Silverlight plug-in change.</summary>
        public string OnResize { get; set; }
        /// <summary>Gets or sets the name of the event handler that is called when the source download has finished.</summary>
        public string OnSourceDownloadComplete { get; set; }
        /// <summary>Gets or sets the name of the event handler that is called when the source download progress changes.</summary>
        public string OnSourceDownloadProgressChanged { get; set; }

        /// <summary>Gets or sets user-defined initialization parameters.</summary>
        public List<InitParam> InitParams = new List<InitParam>();

        /// <summary>Gets or sets the Silverlight version of the control.  Sets the minimum version and install links</summary>
        public SilverlightVersion Version { get; set; }


        public SilverlightPluginGenerator() { }

        public SilverlightPluginGenerator(Uri source)
        {
            this.Source = source;
        }

        public SilverlightPluginGenerator(Uri source, Unit width, Unit height)
        {
            this.Source = source;
            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            XDocument xDocument = new XDocument();
            XElement divXElement = new XElement("div");
            divXElement.Add(new XAttribute("id", "silverlightControlHost"));

            XElement objectXElement = new XElement("object");

            if (this.ID != null)
                objectXElement.Add(new XAttribute("id", this.ID));
            objectXElement.Add(new XAttribute("data", "data:application/x-silverlight-2,"));
            objectXElement.Add(new XAttribute("type", "application/x-silverlight-2"));

            // Only add these if they are specified
            if (this.Width != null)
                objectXElement.Add(new XAttribute("width", this.Width.Value + this.GetUnitString(this.Width.Type)));
            if (this.Height != null)
                objectXElement.Add(new XAttribute("height", this.Height.Value + this.GetUnitString(this.Height.Type)));


            // Conditionally add params to the source
            objectXElement.Add(this.CreateParameter("source", this.Source.ToString()));

            if (this.AllowHtmlPopupWindow != null)
                objectXElement.Add(this.CreateParameter("allowHtmlPopupWindow", this.AllowHtmlPopupWindow.ToString()));
            if (this.AutoUpgrade != null)
                objectXElement.Add(this.CreateParameter("autoUpgrade", this.AutoUpgrade.ToString()));
            if (this.Enablehtmlaccess != null)
                objectXElement.Add(this.CreateParameter("enablehtmlaccess", this.Enablehtmlaccess.ToString()));
            if (this.EnableNavigation != null)
                objectXElement.Add(this.CreateParameter("enableNavigation", this.EnableNavigation.ToString()));
            if (this.Version != SilverlightVersion.Unknown)
                objectXElement.Add(this.CreateParameter("minRuntimeVersion", SilverlightVersionHelper.GetMinimumVersion(this.Version)));
            if (this.OnError != null)
                objectXElement.Add(this.CreateParameter("onError", this.OnError));
            if (this.OnFullScreenChanged != null)
                objectXElement.Add(this.CreateParameter("onFullScreenChanged", this.OnFullScreenChanged));
            if (this.OnLoad != null)
                objectXElement.Add(this.CreateParameter("onLoad", this.OnLoad));
            if (this.OnResize != null)
                objectXElement.Add(this.CreateParameter("onResize", this.OnResize));
            if (this.OnSourceDownloadComplete != null)
                objectXElement.Add(this.CreateParameter("onSourceDownloadComplete", this.OnSourceDownloadComplete));
            if (this.OnSourceDownloadProgressChanged != null)
                objectXElement.Add(this.CreateParameter("onSourceDownloadProgressChanged", this.OnSourceDownloadProgressChanged));
            if (this.BackGround != null)
                objectXElement.Add(this.CreateParameter("background", this.BackGround.Name.ToLower()));

            // Add the initparams
            if (this.InitParams != null && this.InitParams.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (InitParam initParam in this.InitParams)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(",");
                    stringBuilder.Append(initParam.Key + "=" + initParam.Value);
                }
                objectXElement.Add(this.CreateParameter("initParams", stringBuilder.ToString()));
            }


            // Create the no Silverlight installed link
            XElement installLinkXElemenet = new XElement("a");
            installLinkXElemenet.Add(new XAttribute("href", SilverlightVersionHelper.GetUpgradeUrl(this.Version)));
            installLinkXElemenet.Add(new XAttribute("style", "text-decoration:none"));

            XElement silverlightImageXElement = new XElement("img");
            silverlightImageXElement.Add(new XAttribute("src", "http://go.microsoft.com/fwlink/?LinkId=108181"));
            silverlightImageXElement.Add(new XAttribute("alt", "Get Microsoft Silverlight"));
            silverlightImageXElement.Add(new XAttribute("style", "border-style:none"));

            installLinkXElemenet.Add(silverlightImageXElement);
            objectXElement.Add(installLinkXElemenet);


            // This iframe is for Safari, it prevents Safari from caching the page
            XElement iFrameXElement = new XElement("iframe", " ");  // Adding a space for the content forces the iframe to not self close.  If it self closes, the ribbon breaks.
            iFrameXElement.Add(new XAttribute("id", "_sl_historyFrame"));
            iFrameXElement.Add(new XAttribute("style", "visibility:hidden;height:0px;width:0px;border:0px"));

            // Add the object tag and iFrame to the div
            divXElement.Add(objectXElement);
            divXElement.Add(iFrameXElement);

            // Set the div on the root node
            xDocument.Add(divXElement);

            return xDocument.ToString();
        }

        private XElement CreateParameter(string name, string value)
        {
            XElement paramXElement = new XElement("param");
            paramXElement.Add(new XAttribute("name", name));
            paramXElement.Add(new XAttribute("value", value));
            return paramXElement;
        }

        /// <summary>Returns the HTML version of the unit</summary>
        /// <param name="unitType">Unit type to retrieve the HTML string for</param>
        /// <returns></returns>
        private string GetUnitString(UnitType unitType)
        {
            string ret = "";
            switch (unitType)
            {
                case UnitType.Pixel:    ret = "px"; break;
                case UnitType.Em:       ret = "em"; break;
                case UnitType.Ex:       ret = "ex"; break;
                case UnitType.Inch:     ret = "in"; break;
                case UnitType.Cm:       ret = "cm"; break;
                case UnitType.Mm:       ret = "mm"; break;
                case UnitType.Point:    ret = "pt"; break;
                case UnitType.Pica:     ret = "pc"; break;
                default:
                    throw new NotImplementedException("UnitType " + unitType.ToString() + " is not supported");
            }

            return ret;
        }
    }

    public static class SilverlightVersionHelper
    {
        /// <summary>Returns the minimum version for the specified version of Silverlight</summary>
        /// <param name="version">Silveright version</param>
        public static string GetMinimumVersion(SilverlightVersion version)
        {
                string minimumVersion = "";
                switch (version)
                {
                    case SilverlightVersion.v2: minimumVersion = "2.0.31005"; break;
                    case SilverlightVersion.v3: minimumVersion = "3.0.40624"; break;
                    default: minimumVersion = "3.0.40624"; break;
                }
                return minimumVersion;
        }

        /// <summary>Returns the upgrade URL for the specified version of Silverlight</summary>
        /// <param name="version">Silveright version</param>
        public static string GetUpgradeUrl(SilverlightVersion version)
        {
                string upgradeUrl = "";
                switch (version)
                {
                    case SilverlightVersion.v2: upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=2.0.31005.0"; break;
                    case SilverlightVersion.v3: upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40624.0"; break;
                    default: upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40624.0"; break;
                }
                return upgradeUrl;
        }
    }

    public enum SilverlightVersion
    {
        Unknown,
        v2,
        v3
    }

    public class InitParam
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public InitParam(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
