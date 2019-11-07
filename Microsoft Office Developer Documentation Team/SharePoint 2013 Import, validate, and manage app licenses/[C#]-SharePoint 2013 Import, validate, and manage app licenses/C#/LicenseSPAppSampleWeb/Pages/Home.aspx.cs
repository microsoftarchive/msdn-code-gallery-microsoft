using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LicenseSPAppSampleWeb.Pages
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string contextToken = TokenHelper.GetContextTokenFromRequest(Request);

            //Save objects to the session so other pages can use them 
            Session["contexToken"] = contextToken; 
            Session["SPHostUrl"] = Request.QueryString["SPHostUrl"];
            Session["SPAppWebUrl"] = Request.QueryString["SPAppWebUrl"];
            viewLicenses.NavigateUrl = Session["SPHostUrl"] + "/_layouts/15/AllAppLicensesManagement.aspx";
        }
    }
}