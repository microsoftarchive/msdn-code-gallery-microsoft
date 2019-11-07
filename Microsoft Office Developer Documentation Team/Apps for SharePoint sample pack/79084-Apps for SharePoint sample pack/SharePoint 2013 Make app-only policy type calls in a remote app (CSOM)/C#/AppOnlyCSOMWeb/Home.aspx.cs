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


namespace AppOnlyCSOMWeb
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TokenHelper.TrustAllCertificates();

            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                SharePointContextToken contextToken =
                    TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                Response.Write("<h2>Valid context token found</h2>");
                Response.Write("<p>" + contextToken.ToString() + "</p>");
                Response.Flush();

                Uri sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
                string accessToken =
                    TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;

                Response.Write("<h2>Valid access token retrieved</h2>");
                Response.Write("<p>" + accessToken + "</p>");
                Response.Flush();

                //string appOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName, sharepointUrl.Authority, contextToken.Realm).AccessToken;

                ClientContext clientContext =
                    TokenHelper.GetClientContextWithAccessToken(
                        sharepointUrl.ToString(), accessToken);

                //Load the properties for the web object.
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                //clientContext.Load(clientContext.Web);
                
                //clientContext.ExecuteQuery();

                //Response.Write("<h2>Web title retrieved</h2>");
                //Response.Write("<p>" + clientContext.Web.Title + "</p>");
                //Response.Flush();


                string appOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName, sharepointUrl.Authority, contextToken.Realm).AccessToken;


                Response.Write("<h2>Valid app-only access token retrieved</h2>");
                Response.Write("<p>" + appOnlyAccessToken + "</p>");
                Response.Flush();

                clientContext.Dispose();

                clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), appOnlyAccessToken);

                clientContext.Load(clientContext.Web);
                clientContext.ExecuteQuery();

                Response.Write("<h2>Web title retrieved with an app only token</h2>");
                Response.Write("<p>" + clientContext.Web.Title + "</p>");

                clientContext.Dispose();

            }
        }
    }

}