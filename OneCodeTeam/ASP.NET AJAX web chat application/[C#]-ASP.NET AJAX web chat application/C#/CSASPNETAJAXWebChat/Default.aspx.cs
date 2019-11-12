using System;
using System.Web;
using System.Web.UI;

namespace WebChat
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["User"] = Request.UserHostAddress;
            txtAlias.Text = Request.UserHostAddress;
        }
    }
}