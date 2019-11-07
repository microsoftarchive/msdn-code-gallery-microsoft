using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statments that make our code below easier to read/write
using Microsoft.SharePoint.Client;
using System.Web.Configuration;
using WD_SharePointTranslation_csWeb;


namespace WD_SharePointTranslation_csWeb.Pages
{
    public partial class OAuth : System.Web.UI.Page
    {
        
        // Variables used in events and to control the actual page content
        public bool connected = false;
        public string siteTitle = "Not Connected";
        public string connectedSiteUrl = "";
        public string accessToken = "";
        public string refreshToken = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                // Get values from Web.config.
                // NOTE: It is VERY important that you read the ReadMe document
                // so that you will have valid data in your Web.config files.
                // The values provided by this sample will NOT work for you!
                clientId.Value = WebConfigurationManager.AppSettings.Get("ClientId");
                redirectUrl.Value = WebConfigurationManager.AppSettings.Get("RedirectUrl");
                Uri sharePointSiteUrl = null;

                // Retrieve data from the form or querystring.
                string siteUrl = Request.Form["siteUrl"];
                if (siteUrl == null)
                {
                    siteUrl = Request.QueryString["siteUrl"];
                }
                string code = Request.Form["code"];
                if (code == null)
                {
                    code = Request.QueryString["code"];
                }
                if (!TokenCache.IsTokenInCache(Request.Cookies) && siteUrl == null && code == null)
                {
                    //If called with no params and no cached token then render the main page
                    return;
                }

                //If siteUrl has been provided, obtain it, otherwise prompt the user
                if (siteUrl != null && siteUrl != "")
                {
                    sharePointSiteUrl = new Uri(siteUrl);
                }
                else
                {
                    //If not SharePoint Site Url in context, we need to prompt the user
                    return;
                }

                // Work with cookies and the token cache to ensure we have valid OAuth tokens.
                if (code != null && code != "")
                {
                    TokenCache.UpdateCacheWithCode(Request, Response, code, sharePointSiteUrl);
                }
                if (!TokenCache.IsTokenInCache(Request.Cookies))
                {
                    return;
                }
                else
                {
                    refreshToken = TokenCache.GetCachedRefreshToken(Request.Cookies);
                    accessToken = TokenHelper.GetAccessToken(refreshToken, "00000003-0000-0ff1-ce00-000000000000", sharePointSiteUrl.Authority, TokenHelper.GetRealmFromTargetUrl(sharePointSiteUrl)).AccessToken;
                    connectedSiteUrl = sharePointSiteUrl.ToString();
                    using (ClientContext context = TokenHelper.GetClientContextWithAccessToken(sharePointSiteUrl.ToString(), accessToken))
                    {
                        context.Load(context.Web);
                        context.ExecuteQuery();
                        connected = true;
                        siteTitle = context.Web.Title;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                Response.Write("\n" + ex.StackTrace);
                Response.End();
            }
        }
    }
}