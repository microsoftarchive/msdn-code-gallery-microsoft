using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace PhotoSharingApp {
  public partial class RedirectAccept : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
      TokenHelper.TrustAllCertificates();

      string authCode = Request.QueryString["code"];
      // TODO: Update this with the local site's page where the user should be redirected to after logging in
      Uri rUri = new Uri("http://****.contoso.com/Default.aspx");
      string o365Site = "https://****.sharepoint.com/";

      // validate the auth code & redirect to the requested site
      using (ClientContext context = TokenHelper.GetClientContextWithAuthorizationCode(
                                       o365Site, 
                                       "00000003-0000-0ff1-ce00-000000000000", 
                                       authCode, 
                                       TokenHelper.GetRealmFromTargetUrl(new Uri(o365Site)), 
                                       rUri)) {
        context.Load(context.Web);
        context.ExecuteQuery();
      }
    }
  }
}