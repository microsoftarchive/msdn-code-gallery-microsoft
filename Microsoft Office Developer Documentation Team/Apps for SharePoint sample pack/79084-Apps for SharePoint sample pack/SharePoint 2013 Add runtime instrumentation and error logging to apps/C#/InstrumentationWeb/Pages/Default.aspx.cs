using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Microsoft.SharePoint.Client.Utilities;
using System.Web.Configuration;
using System.Configuration;
using System.Net;


namespace InstrumentationWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {

        SharePointContextToken contextToken;
        string accessToken;
        Uri sharepointUrl;
        string contextTokenString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
                TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
                section.Enabled = false;
                configuration.Save();
            }

            // Get the client context.
            TokenHelper.TrustAllCertificates();
            contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                // Get context token
                contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                // Get the host web's URL and the access token.
                sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
                accessToken = TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;

                // Pass the access token to the button event handler.
                Button1.CommandArgument = accessToken;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string accessToken = ((Button)sender).CommandArgument;
            var clientContext = TokenHelper.GetClientContextWithAccessToken(Request.QueryString["SPHostUrl"].ToString(), accessToken);

            try
            {

                if (IsPostBack)
                {
                    // Get the host web's URL.
                    sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
                }

                // REST/OData section

                // Get the titles of all lists on the host web.
                string oDataUrl = "/_api/Web/lists?$select=Title";

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + oDataUrl);
                request.Method = "GET";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Response markup parsing section
                XDocument oDataXML = XDocument.Load(response.GetResponseStream(), LoadOptions.None);
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
                XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

                // The ATOM markup for a SharePoint list nests field elements under <entry> <content> <properties>.
                List<XElement> entries = oDataXML.Descendants(atom + "entry")
                                         .Elements(atom + "content")
                                         .Elements(m + "properties")
                                         .ToList();

                var entryFieldValues = from entry in entries
                                       select new
                                       {
                                           Title = entry.Element(d + "Title").Value
                                       };

                // Bind data to the grid on the page.
                GridView1.DataSource = entryFieldValues;
                GridView1.DataBind();

                // Throw an exception to illustrate custom logging.
                throw new Exception("Exception thrown by POPULATE DATA button");

            }
            catch (Exception ex)
            {
                // Catch exception and log it.
                Utility.LogCustomRemoteAppError(clientContext, Global.ProductId, ex.Message);
                clientContext.ExecuteQuery();

                // Show the link to the diagnostics page.
                hyperlink1.Visible = true;
                hyperlink1.NavigateUrl = "https://" + Request.Url.Authority + "/Pages/Diagnostics.aspx?SPHostUrl=" + sharepointUrl;

            }

        }
    }
}