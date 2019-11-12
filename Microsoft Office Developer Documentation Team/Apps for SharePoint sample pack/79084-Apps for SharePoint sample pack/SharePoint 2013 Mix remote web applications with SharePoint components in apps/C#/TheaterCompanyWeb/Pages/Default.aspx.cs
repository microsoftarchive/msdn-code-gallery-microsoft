using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint.Client;
using Microsoft.IdentityModel.S2S.Tokens;
using System.Net;
using System.IO;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;

using TheaterCompanyWeb;

namespace TheaterCompanyWeb.Pages

{
    public partial class Default : System.Web.UI.Page
    {
        SharePointContextToken contextToken;
        string accessToken;
        Uri sharepointUrl;


        protected void Page_Load(object sender, EventArgs e)
        {
            TokenHelper.TrustAllCertificates();
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                // Get context token
                contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                // Get access token
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
                accessToken = TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;

                // Pass the access token to the button event handler.
                Button1.CommandArgument = accessToken;
            }
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            // Retrieve the access token that the Page_Load method stored
            // in the button's command argument.
            string accessToken = ((Button)sender).CommandArgument;

            if (IsPostBack)
            {
                // Get the app web's URL.
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            // REST/OData section

            // Use the $select query to bring only the fields that will actually be used over the network.
            string oDataUrl = "/_api/Web/lists/getbytitle('Characters In Hamlet')/items?$select=Title,Actor,CastingStatus";

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
                                       Character = entry.Element(d + "Title").Value,
                                       Actor = entry.Element(d + "Actor").Value,
                                       CastingStatus = entry.Element(d + "CastingStatus").Value
                                   };

            // Bind data to the grid on the page.
            GridView1.DataSource = entryFieldValues;
            GridView1.DataBind();
        }

        //private Uri GetModifiedAppWebUrl()
        //{
        //    Uri hostWebUrl = new Uri(Request.QueryString["SPHostUrl"]);
        //    Uri appWebUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
        //    string appWebString = appWebUrl.ToString();
        //    return new Uri(appWebString.Replace(appWebUrl.Authority, hostWebUrl.Authority));
        //}

    }
}