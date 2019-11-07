using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace UpgradeEventDemoWeb.Pages {
  public partial class Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {

      // installing

      string urlAppWeb = Request.QueryString["SPAppWebUrl"];
      Uri uriAppWeb = new Uri(urlAppWeb);

      
      using (var cc = TokenHelper.GetS2SClientContextWithWindowsIdentity(uriAppWeb, Request.LogonUserIdentity)) {

        Site sc = cc.Site;
        Web appWeb = cc.Web;
        ListCollection lists = appWeb.Lists;
        List list = appWeb.Lists.GetByTitle("Books");

        cc.Load(sc, p1 => p1.Url);
        cc.Load(appWeb);
        cc.Load(lists);
        cc.Load(list, l => l.DefaultView.ServerRelativeUrl);
        cc.ExecuteQuery();

        string urlList = sc.Url + list.DefaultView.ServerRelativeUrl;

        string html = "<p>Click " +
                      "<a href='" + urlList + "'>here</a>" +
                      " to view the Customers list in the app web</p>";

        placeHolderMain.Controls.Add(new LiteralControl(html)); 
      }
    }
  }
}