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

namespace RemoteAppUsingHttpsWeb
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


                ClientContext clientContext =
                    TokenHelper.GetClientContextWithAccessToken(
                        sharepointUrl.ToString(), accessToken);
                clientContext.Load(clientContext.Web);
                clientContext.ExecuteQuery();

                Response.Write("<h2>Web title retrieved</h2>");
                Response.Write("<p>" + clientContext.Web.Title + "</p>");
                Response.Flush();


                HttpWebRequest request =
                    (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "_api/Web/title");
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                Response.Write("<h2>Web title retrieved using REST</h2>");
                Response.Write("<p>" + reader.ReadToEnd() + "</p>");
                Response.Flush();


                //Load the properties for the web object
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                //Load the lists from the web object
                ListCollection lists = web.Lists;
                clientContext.Load<ListCollection>(lists);
                clientContext.ExecuteQuery();

                //print out the information
                clientContext.Load(web.CurrentUser);
                clientContext.ExecuteQuery();
                string str = clientContext.Web.CurrentUser.LoginName + "<br>";

                UserCollection users = web.SiteUsers;
                clientContext.Load<UserCollection>(users);
                clientContext.ExecuteQuery();

                foreach (User siteUser in users)
                {
                    str += "SiteUser: " + siteUser.LoginName + "<br>";
                }

                foreach (string key in Request.QueryString.AllKeys)
                {
                    str += key + " = " + Request.QueryString[key] + "<br>";
                }

                foreach (List list in lists)
                {
                    str += "List: " + list.Title + "<br>";
                }
                Response.Write(str);

                Response.Write("<p>Web title retrieved: " + clientContext.Web.Title + "</p>");
                Response.Flush();


            }
        }
    }
}