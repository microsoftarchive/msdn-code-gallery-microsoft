using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RerWorkflowAppWeb.Pages {
  public partial class Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
      // The following code gets the client context and Title property by using TokenHelper.
      // To access other properties, you may need to request permissions on the host web.

      var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
      var hostWeb = Page.Request["SPHostUrl"];

      using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority)) {
        clientContext.Load(clientContext.Web, web => web.Title);
        clientContext.ExecuteQuery();
        Response.Write(clientContext.Web.Title);
      }
    }
  }
}