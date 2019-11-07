using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Microsoft.SharePoint.Client;

namespace PhotoSharingApp {
  public partial class _Default : Page {
    protected void Page_Load(object sender, EventArgs e) {
      // TODO: update this with the URL of the Office 365 site that contains the libraries with photos
      Uri sharePointSiteUrl = new Uri("https://****.sharpoint.com/");

      // if the refresh code is in the URL, cache it
      if (Request.QueryString["code"] != null) {
        TokenCache.UpdateCacheWithCode(Request, Response, sharePointSiteUrl);
      }

      // if haven't previously obtained an refresh token, get an authorization token now
      if (!TokenCache.IsTokenInCache(Request.Cookies)) {
        Response.Redirect(TokenHelper.GetAuthorizationUrl(sharePointSiteUrl.ToString(), "Web.Read List.Read"));
      } else {
        // otherwise, get the access token from ACS
        string refreshToken = TokenCache.GetCachedRefreshToken(Request.Cookies);
        string accessToken = TokenHelper.GetAccessToken(
                               refreshToken, 
                               "00000003-0000-0ff1-ce00-000000000000", 
                               sharePointSiteUrl.Authority, 
                               TokenHelper.GetRealmFromTargetUrl(sharePointSiteUrl)
                             ).AccessToken;

        // use the access token to get a CSOM client context & get values from SharePoint
        using (ClientContext context = TokenHelper.GetClientContextWithAccessToken(sharePointSiteUrl.ToString(), accessToken)) {
          context.Load(context.Web);
          context.ExecuteQuery();
          
          // get contents from specified list
        }
      }
    }
  }
}
