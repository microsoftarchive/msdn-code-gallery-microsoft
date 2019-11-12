using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Samples;
using Microsoft.IdentityModel.S2S.Tokens;
using System.Net;
using System.IO;
using System.Xml;

namespace AppOnlyRESTWeb
{
    public partial class Home : System.Web.UI.Page
    {
        string siteName;
        string currentUser;
        string rootFolder;

        protected void Page_Load(object sender, EventArgs e)
        {
            TokenHelper.TrustAllCertificates();

            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                SharePointContextToken contextToken =
                    TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                Uri sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
                string accessToken =
                    TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;

                Response.Write("<h2>Valid context token retrieved</h2>");
                Response.Write("<p>" + contextToken + "</p>");
                Response.Flush();


                Response.Write("<h2>Valid access token retrieved</h2>");
                Response.Write("<p>" + accessToken + "</p>");
                Response.Flush();

                //Create a namespace manager for parsing the ATOM XML returned by the queries.
                XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());
                //Add the pertinent namespaces to the namespace manager.
                xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

                //Request the name of the site.
                HttpWebRequest request =
                    (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "_api/Web/title");
                request.Method = "GET";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                request.Headers.Add("Authorization", "Bearer " + accessToken);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                var titleXml = new XmlDocument();
                titleXml.LoadXml(reader.ReadToEnd());
                var webTitle = titleXml.SelectSingleNode("d:Title", xmlnspm);
                siteName = webTitle.InnerXml;

                Response.Write("<h2>Site name retrieved using REST</h2>");
                Response.Write("<p>" + siteName + "</p>");
                Response.Flush();

                //Request information about the current user.
                HttpWebRequest currentUserRequest =
        (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "_api/Web/currentUser");
                currentUserRequest.Method = "GET";
                currentUserRequest.Accept = "application/atom+xml";
                currentUserRequest.ContentType = "application/atom+xml;type=entry";
                currentUserRequest.Headers.Add("Authorization", "Bearer " + accessToken);

                HttpWebResponse currentUserResponse = (HttpWebResponse)currentUserRequest.GetResponse();
                StreamReader currentUserReader = new StreamReader(currentUserResponse.GetResponseStream());

                var currentUserXml = new XmlDocument();
                currentUserXml.LoadXml(currentUserReader.ReadToEnd());
                var currentUserTitle = currentUserXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:LoginName", xmlnspm);
                currentUser = currentUserTitle.InnerXml;

                Response.Write("<h2>Current user</h2>");
                Response.Write("<p>" + currentUser + "</p>");
                Response.Flush();

                string appOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName, sharepointUrl.Authority, contextToken.Realm).AccessToken;

                Response.Write("<h2>Valid app-only access token retrieved</h2>");
                Response.Write("<p>" + appOnlyAccessToken + "</p>");
                Response.Flush();

                //Request information about the welcome page of the root folder.
                HttpWebRequest rootFolderRequest =
                    (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "_api/Web/RootFolder");
                rootFolderRequest.Method = "GET";
                rootFolderRequest.Accept = "application/atom+xml";
                rootFolderRequest.ContentType = "application/atom+xml;type=entry";
                rootFolderRequest.Headers.Add("Authorization", "Bearer " + appOnlyAccessToken);

                HttpWebResponse rootFolderResponse = (HttpWebResponse)rootFolderRequest.GetResponse();
                StreamReader rootFolderReader = new StreamReader(rootFolderResponse.GetResponseStream());

                var welcomePageXml = new XmlDocument();
                welcomePageXml.LoadXml(rootFolderReader.ReadToEnd());

                var rootFolderTitle = welcomePageXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:WelcomePage", xmlnspm);
                rootFolder = rootFolderTitle.InnerXml;

                Response.Write("<h2>Root folder welcome page retrieved using REST</h2>");
                Response.Write("<p>" + rootFolder + "</p>");
                Response.Flush();

            }
        }
    }
}